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

        public async Task<IActionResult> Index(int pageIndex = 1)
        {
            var pageSize = 20;
            var v = await FetchArticlesData("", DateTime.Today, null, "Apple", null, null, pageIndex, pageSize, "en");

            if (v.status == "error")
            {
                ViewBag.Error = v.message;
                return View(v);
            }

            var usersAsIPagedList = new StaticPagedList<News.Article>(v.articles, pageIndex, pageSize , v.totalResults);
            ViewBag.OnePageOfUsers = usersAsIPagedList;

            return View(v);
        }

        public async Task<IActionResult> Sources(string category = null, string language = "all", string country = "all")
        {
            NewsApiSources v;
            var cfg = new NewsApiConfig(this._configuration);
            using (var ns = new News.NewsApiService(cfg))
            {
                v= await ns.GetSources(category, News.NewsApiService.NewsLanguages.en, News.NewsApiService.Countries.all);
            }

            if (v.status == "error")
                ViewBag.Error = v.message;
            
            return View(v.sources);            
        }

        public async Task<News.NewsApiArticles> FetchArticlesData(string source, DateTime? fromDate, DateTime? toDate = null, string keyWord = null, string keyWordInTitle = null, string sortBy = null, int page = 0, int pageSize = 20, string language = "en")
        {
            var cfg = new NewsApiConfig(this._configuration);
            using (var ns = new News.NewsApiService(cfg))
            {
                return await ns.GetEverything(source, fromDate, toDate, keyWord, keyWordInTitle, sortBy, page, pageSize, News.NewsApiService.NewsLanguages.en);
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
