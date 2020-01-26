using Newtonsoft.Json;

namespace HackerNewsDemo.Dtos
{
    public class HackerNewsDto
    {
        [JsonProperty("by")]
        public string By { get; set; }

        [JsonProperty("kids")]
        public long[] Kids { get; set; }

        [JsonProperty("score")]
        public int Score { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }
}