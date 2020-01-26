using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HackerNewsDemo.Dtos;
using HackerNewsDemo.Models;
using Newtonsoft.Json;

namespace HackerNewsDemo.Services
{
    public class TopStoriesService : ITopStoriesService
    {
        const string RequestUri = "https://hacker-news.firebaseio.com/v0/item/";

        /// <inheritdoc />
        public async Task<IEnumerable<News>> GetAsync(string[] topStoryIds, int limitTo = 5)
        {
            var stories = new HashSet<News>();

            // thanks to this c# 8.0 feature
            // https://docs.microsoft.com/en-us/archive/msdn-magazine/2019/november/csharp-iterating-with-async-enumerables-in-csharp-8
            await foreach (var hackerNewsDto in GetHackerNewsDataAsyncEnumerable())
            {
                if (!IsValidData(hackerNewsDto))
                {
                    // if the current News data does not pass our validation lets
                    // move on to the next item in the collection
                    continue;
                }

                stories.Add(new News
                {
                    Title = hackerNewsDto.Title,
                    Uri = new Uri(hackerNewsDto.Url),
                    Author = hackerNewsDto.By,
                    Comments = hackerNewsDto.Kids.Length,
                    Points = hackerNewsDto.Score
                });
            }

            return CalculateRanking(stories);

            async IAsyncEnumerable<HackerNewsDto> GetHackerNewsDataAsyncEnumerable()
            {
                // the Hacker News Api is not well designed (at least at first glance), first we need to get id's
                // for the Top Stories and then make individual request with each id to get the relevant story.
                // basically forcing us to make 1+500 calls to receive 500 stories!!!

                // the 🤖🤖🤖 can request upto 100 stories at a time (or so the doc, not Emmett Brown, tells us!)
                var limitedTo = topStoryIds.Take(limitTo).Count();

                var httpClient = new HttpClient();
                for (var i = 0; i <= limitedTo - 1; i++)
                {
                    Console.WriteLine($"Getting {i} result out of {limitedTo}");
                    using var response = await httpClient.GetAsync($"{RequestUri}/{topStoryIds[i]}.json");
                    response.EnsureSuccessStatusCode();
                    yield return JsonConvert.DeserializeObject<HackerNewsDto>(
                        await response.Content.ReadAsStringAsync());
                }
            }
        }

        static IEnumerable<News> CalculateRanking(IEnumerable<News> storyCollection)
        {
            // ranking data is not available from the Hacker News Api endpoints (after a quick search)
            // therefore, we are ranking them based on how many points each post has
            return storyCollection.OrderByDescending(s => s.Points)
                .Select((news, index) => new News
                {
                    // ToDo: hmm may be optimise this bit a more 🤔?
                    Title = news.Title,
                    Author = news.Author,
                    Comments = news.Comments,
                    Points = news.Points,
                    Uri = news.Uri,
                    Rank = index + 1 // ranking is not zero based
                });
        }

        static bool IsValidData(HackerNewsDto news)
        {
            // assuming 🤖🤖🤖 at TrueLayer can't handle empty/large strings from the doc
            if (string.IsNullOrEmpty(news.Title) || news.Title.Length > 256)
            {
                return false;
            }

            // bad 🤖🤖🤖 s
            if (string.IsNullOrEmpty(news.By) || news.By.Length > 256)
            {
                return false;
            }

            // from the doc, points (score) should be >= 0
            if (news.Score < 0)
            {
                return false;
            }

            // check if we have an valid Uri
            if (!Uri.TryCreate(news.Url, UriKind.RelativeOrAbsolute, out _))
            {
                return false;
            }

            // exclude posts that have 0 comments (Kids are id's for comments, therefore, 0 kids = 0 comments)
            // kids these days huh?
            return !(news.Kids is null) && news.Kids.Any();
        }
    }

    public interface ITopStoriesService
    {
        /// <summary>
        /// Returns a collection on News for the give id's
        /// </summary>
        /// <param name="topStoryIds">Top story id's</param>
        /// <param name="limitTo">Number of top stories to limit, default is 5</param>
        /// <returns cref='News'/>
        Task<IEnumerable<News>> GetAsync(string[] topStoryIds, int limitTo = 5);
    }
}