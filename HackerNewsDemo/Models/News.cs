using System;

namespace HackerNewsDemo.Models
{
    public class News
    {
        public string Title { get; set; }

        public Uri Uri { get; set; }

        public string Author { get; set; }

        public long Points { get; set; }

        public long Comments { get; set; }

        public int Rank { get; set; }
    }
}