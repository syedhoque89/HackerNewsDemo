using System.Linq;
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

        [Test]
        public async Task TopStoriesService_Returns_None_Empty_Collection()
        {
            var storiesIds = await _topStoriesIdService.GetAsync();

            var stories = await new TopStoriesService().GetAsync(storiesIds);

            Assert.IsNotEmpty(stories);
        }

        [TestCase(500)]
        public async Task TopStoriesService_Should_Return_Upto_MaxValue(int maxValue)
        {
            var storiesIds = await _topStoriesIdService.GetAsync();

            var stories = await new TopStoriesService().GetAsync(storiesIds);

            Assert.LessOrEqual(stories.Count(), maxValue);
        }

        [TestCase(5)]
        [TestCase(100)]
        public async Task TopStoriesService_Returns_Collection_Of_Expected_Length(int limitTo)
        {
            var storiesIds = await _topStoriesIdService.GetAsync();

            var stories = await new TopStoriesService().GetAsync(storiesIds, limitTo);

            Assert.LessOrEqual(stories.Count(), limitTo);
        }
    }
}