using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsApiChallenge.News
{
    public class NewsApiArticles :  NewsApiResponse
    {
        public int totalResults { get; set; }
        public List<Article> articles { get; set; }
    }
}
