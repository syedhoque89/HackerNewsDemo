using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HackerNewsDemo.Services
{
    public class TopStoriesIdService : ITopStoriesIdService
    {
        const string RequestUri = "https://hacker-news.firebaseio.com/v0/topstories.json";

        /// <inheritdoc/>
        public async Task<string[]> GetAsync()
        {
            using var response = await new HttpClient().GetAsync(RequestUri);
            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<string[]>(await response.Content.ReadAsStringAsync());
        }
    }

    public interface ITopStoriesIdService
    {
        /// <summary>
        /// Gets the id's of current top stories from Hacker News Api
        /// </summary>
        /// <returns>Array of string id's</returns>
        public Task<string[]> GetAsync();
    }
}