using BestStoriesApi.Models;

namespace BestStoriesApi.Services;

public interface IHnService
{
    Task<IReadOnlyList<StoryDto>> GetBestStoriesAsync(int n, CancellationToken cancellationToken);
}