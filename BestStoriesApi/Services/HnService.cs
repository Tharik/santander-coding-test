using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using BestStoriesApi.Models;

namespace BestStoriesApi.Services;

public sealed class HnService : IHnService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IMemoryCache _cache;
    private readonly SemaphoreSlim _gate = new(initialCount: 16, maxCount: 16); // only 16 concurrent requests to avoid over loading HN

    public HnService(IHttpClientFactory httpClientFactory, IMemoryCache cache)
    {
        _httpClientFactory = httpClientFactory;
        _cache = cache;
    }

public async Task<IReadOnlyList<StoryDto>> GetBestStoriesAsync(int n, CancellationToken cancellationToken)
{
    if (n <= 0) return Array.Empty<StoryDto>();
    if (n > 500) n = 500;

    var ids = await GetBestStoryIdsAsync(cancellationToken);

    var selectedIds = ids.Take(n).ToArray();

    // Fetch all items concurrently (bounded by SemaphoreSlim inside GetStoryDtoAsync)
    var tasks = selectedIds.Select(id => GetStoryDtoAsync(id, cancellationToken)).ToArray();
    var results = await Task.WhenAll(tasks);

    // If any item is missing/invalid, fail.
    // Because otherwise you can't guarantee returning the best N stories.
    if (results.Any(r => r is null))
        throw new InvalidOperationException("Hacker News returned an invalid/missing story item");

    return results!
        .Select(r => r!) // safe because we checked null above
        .OrderByDescending(x => x.Score)
        .ToArray();
}

    private async Task<IReadOnlyList<long>> GetBestStoryIdsAsync(CancellationToken ct)
    {
        const string cacheKey = "hn:beststories:ids";

        return await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60);

            var client = _httpClientFactory.CreateClient("hn");
            using var response = await client.GetAsync("beststories.json", ct);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(ct);
            var ids = JsonSerializer.Deserialize<List<long>>(json, JsonOptions) ?? new List<long>();
            return (IReadOnlyList<long>)ids;
        }) ?? Array.Empty<long>();
    }

    private async Task<StoryDto?> GetStoryDtoAsync(long id, CancellationToken ct)
    {
        var cacheKey = $"hn:item:{id}";

        return await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);

            await _gate.WaitAsync(ct);
            try
            {
                var client = _httpClientFactory.CreateClient("hn");
                using var response = await client.GetAsync($"item/{id}.json", ct);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync(ct);
                var item = JsonSerializer.Deserialize<HnItem>(json, JsonOptions);

                if (item is null)
                    throw new InvalidOperationException($"HN item {id} returned null");

                if (!string.Equals(item.Type, "story", StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException($"HN item {id} is not a story (type={item.Type})");

                return new StoryDto
                {
                    Title = item.Title,
                    Uri = item.Url,
                    PostedBy = item.By,
                    Time = DateTimeOffset.FromUnixTimeSeconds(item.Time),
                    Score = item.Score,
                    CommentCount = item.Descendants ?? 0
                };
            }
            finally
            {
                _gate.Release();
            }
        });
    }
}