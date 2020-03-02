using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsApiChallenge.News
{
    public class NewsApiSources : NewsApiResponse
    {
        public int totalResults { get; set; }
        public List<Source> sources{ get; set; }
    }
}
