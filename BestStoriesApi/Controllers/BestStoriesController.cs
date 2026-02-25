using Microsoft.AspNetCore.Mvc;
using BestStoriesApi.Models;
using BestStoriesApi.Services;

namespace BestStoriesApi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public sealed class BestStoriesController : ControllerBase
{
    private readonly IHnService _service;

    public BestStoriesController(IHnService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<StoryDto>>> Get([FromQuery] int n = 10, CancellationToken cancellationToken = default)
    {
        try
        {
            var data = await _service.GetBestStoriesAsync(n, cancellationToken);
            return Ok(data);
        }
        catch (HttpRequestException)
        {
            return StatusCode(502, "Failed to fetch data from Hacker News");
        }
        catch (TaskCanceledException)
        {
            return StatusCode(504, "Hacker News request timed out");
        }
    }
}