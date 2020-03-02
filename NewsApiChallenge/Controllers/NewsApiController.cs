using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NewsApiChallenge.Models;
using NewsApiChallenge.News;
using X.PagedList;

namespace NewsApiChallenge.Controllers
{
    public class NewsApiController : Controller
    {
        private readonly ILogger<HomeController> _logger;        
        private IConfiguration _configuration { get; set; }

        public NewsApiController(IConfiguration configuration, ILogger<HomeController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        //public async Task<IActionResult> Index(int pageIndex = 1, int pageSize = 20, string source = null, DateTime? dateFrom = null, DateTime? dateTo = null, string keyWord = "Apple", string keyWordInTitle = null, string sortBy = null, string language = "en")
        public async Task<IActionResult> Index([FromQuery] NewsSearchableFields newsSearchableFields, bool newPageIndex)
        {
            if (!newPageIndex)
                newsSearchableFields.pageIndex = 1;

            var v = await FetchArticlesData(newsSearchableFields);

            var sources = (await FetchSourcesData("", "en", "all")).sources;
            ViewBag.Source = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(sources, "id", "name");
            ViewBag.NewsSearchableFields = newsSearchableFields;

            if (v.status == "error")
            {
                ViewBag.Error = v.message;
                return View(v);
            }

            var usersAsIPagedList = new StaticPagedList<News.Article>(v.articles, newsSearchableFields.pageIndex, newsSearchableFields.pageSize, v.totalResults);
            ViewBag.OnePageOfUsers = usersAsIPagedList;

            return View(v);
        }


        public async Task<IActionResult> Sources(string category = null, string language = "all", string country = "all")
        {
            var v = await FetchSourcesData(category, language, country);

            if (v.status == "error")
                ViewBag.Error = v.message;
            
            return View(v.sources);            
        }


        public async Task<News.NewsApiSources> FetchSourcesData(string category, string language, string country)
        {
            var lan = (NewsApiService.NewsLanguages)Enum.Parse(typeof(NewsApiService.NewsLanguages), language);
            var count = (NewsApiService.Countries)Enum.Parse(typeof(NewsApiService.Countries), country);

            NewsApiSources v;
            var cfg = new NewsApiConfig(this._configuration);
            using (var ns = new News.NewsApiService(cfg))
            {
                return await ns.GetSources(category, lan, count);
            }
        }

        public async Task<News.NewsApiArticles> FetchArticlesData(NewsSearchableFields newsSearchableFields)
        {
            var lan = string.IsNullOrEmpty(newsSearchableFields.language) ? NewsApiService.NewsLanguages.en : (NewsApiService.NewsLanguages)Enum.Parse(typeof(NewsApiService.NewsLanguages), newsSearchableFields.language);
            var cfg = new NewsApiConfig(this._configuration);
            using (var ns = new News.NewsApiService(cfg))
            {
                return await ns.GetEverything(newsSearchableFields.source, DateTime.Today, null, newsSearchableFields.keyWord, newsSearchableFields.keyWordInTitle, newsSearchableFields.sortBy, newsSearchableFields.pageIndex, newsSearchableFields.pageSize, lan);
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
