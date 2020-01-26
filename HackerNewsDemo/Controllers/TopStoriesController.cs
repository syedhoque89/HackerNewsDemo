using System.Linq;
using System.Threading.Tasks;
using HackerNewsDemo.Models;
using HackerNewsDemo.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HackerNewsDemo.Controllers
{
    [Produces("application/json")]
    [ApiController]
    [Route("[controller]")]
    public class TopStoriesController : ControllerBase
    {
        readonly ILogger<TopStoriesController> _logger;
        readonly ITopStoriesIdService _topStoriesIdService;
        readonly ITopStoriesService _topStoriesService;

        public TopStoriesController
        (
            ILogger<TopStoriesController> logger,
            ITopStoriesIdService topStoriesIdService,
            ITopStoriesService topStoriesService
        )
        {
            _logger = logger;
            _topStoriesIdService = topStoriesIdService;
            _topStoriesService = topStoriesService;
        }

        /// <summary>
        /// Returns a collection of News
        /// </summary>
        /// <param name="numberOfPosts">Number of posts to limit to, default is 10</param>
        [ProducesResponseType(typeof(News[]), 200)]
        [ProducesResponseType(400)]
        [HttpGet("{numberOfPosts}")]
        public async Task<ActionResult<News[]>> Get(int numberOfPosts = 10)
        {
            if (numberOfPosts > 100 || numberOfPosts < 1)
            {
                return BadRequest($"Invalid number of posts: {numberOfPosts}");
            }

            _logger.LogInformation("Requesting Top Story id's from Hacker News");

            var storyIds = await _topStoriesIdService.GetAsync();

            _logger.LogInformation($"Received {storyIds.Length} id(s)");

            _logger.LogInformation("Requesting Top Stories");

            var topStories = await _topStoriesService.GetAsync(storyIds, numberOfPosts);

            _logger.LogInformation($"Received {topStories.Count()} top stories");

            return topStories.ToArray();
        }
    }
}