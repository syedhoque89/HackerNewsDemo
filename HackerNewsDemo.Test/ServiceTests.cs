using System.Threading.Tasks;
using HackerNewsDemo.Services;
using NUnit.Framework;

namespace HackerNewsDemo.Test
{
    public class ServiceTests
    {
        ITopStoriesIdService _topStoriesIdService;

        [SetUp]
        public void Setup()
        {
            _topStoriesIdService = new TopStoriesIdService();
        }

        [Test]
        public async Task TopStoriesIdService_Returns_Story_Ids()
        {
            var storiesIds = await _topStoriesIdService.GetAsync();

            Assert.IsNotEmpty(storiesIds);
        }
    }
}