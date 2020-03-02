using NewsApiChallenge.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace NewsApiChallenge.News
{
    public class NewsApiService : IDisposable
    {
        NewsApiConfig newsApiConfig { get; set; }

        public enum NewsLanguages
        {
            all,ar,de,en,es,fr,he,it,nl,no,pt,ru,se,ud,zh
        }

        public enum Countries
        {
            all, ae, ar, at, au, be, bg, br, ca, ch, cn, co, cu, cz, de, eg, fr, gb, gr, hk, hu, id, ie, il, @in, it, jp, kr, lt, lv, ma, mx, my, ng, nl, no, nz, ph, pl, pt, ro, rs, ru, sa, se, sg, si, sk, th, tr, tw, ua, us, ve, za
        }

        public HttpClient Client { get; private set; }

        public NewsApiService(NewsApiConfig newsApiConfig)
        {
            this.newsApiConfig = newsApiConfig;

            this.Client = new HttpClient();
            Client.BaseAddress = new Uri(newsApiConfig.BaseUri);
            Client.DefaultRequestHeaders.Add("Accept", "application/json");
            Client.DefaultRequestHeaders.Add("User-Agent", "HttpClientFactory-Sample");
        }

        public async Task<NewsApiArticles> GetEverything(string sources, DateTime? fromDate, DateTime? toDate = null, string keyWord = null, string keyWordInTitle = null, string sortBy = null, int page = 0, int pageSize = 20, NewsLanguages language = NewsLanguages.all)
        {
            var endpoint = this.newsApiConfig.EndPoints.Where(e => e.Name == "everything").First();
            var uri = $"{endpoint.Uri}?apiKey={this.newsApiConfig.ApiKey}";
            uri += !string.IsNullOrEmpty(sources) ? $"&sources={sources}" : null;

            uri += !string.IsNullOrEmpty(keyWord) ? $"&q={keyWord}" : null;
            uri += !string.IsNullOrEmpty(keyWordInTitle) ? $"&qInTitle={keyWordInTitle}" : null;
            
            uri += fromDate.HasValue ? $"&from={fromDate.Value.ToString("yyyy-MM-dd")}" : null;
            uri += toDate.HasValue ? $"&from={toDate.Value.ToString("yyyy-MM-dd")}" : null;

            uri += language != NewsLanguages.all ? $"&language={language.ToString()}" : null;

            uri += sortBy != null ? $"&sortBy={sortBy}" : null;
            uri += $"&page={page}" ;
            uri += $"&pageSize={pageSize}";

            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            return (await SendRequest<NewsApiArticles>(request));
            
        }

        public async Task<NewsApiSources> GetSources(string category, NewsLanguages language = NewsLanguages.en, Countries country = Countries.all)
        {
            var endpoint = this.newsApiConfig.EndPoints.Where(e => e.Name == "sources").First();
            var uri = $"{endpoint.Uri}?apiKey={this.newsApiConfig.ApiKey}";
            
            uri += !string.IsNullOrEmpty(category) ? $"&category={category}" : null;
            uri += language != NewsLanguages.all ? $"&language={language.ToString()}" : null;
            uri += country!= Countries.all ? $"&country={country.ToString()}" : null;

            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            return (await SendRequest<NewsApiSources>(request));            
        }

        private async Task<T> SendRequest<T>(HttpRequestMessage requestMessage)
        {
            var response = await this.Client.SendAsync(requestMessage);
            var responseString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(responseString);
        }

        public void Dispose()
        {
            this.Client.Dispose();
        }
    }
}
