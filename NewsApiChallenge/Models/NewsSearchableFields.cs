using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NewsApiChallenge.Models
{
    public class NewsSearchableFields
    {
        public int pageIndex { get; set; } = 1;
        public int pageSize { get; set; } = 20;
        public string source { get; set; }

        [DisplayFormat(DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime? DateFrom { get; set; } = DateTime.Today;
        [DisplayFormat(DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime? DateTo { get; set; }

        public string keyWord { get; set; } = "Apple";
        public string keyWordInTitle { get; set; }
        public string sortBy { get; set; }
        public string language { get; set; } = "en";

        public string ToQueryString()
        {
            var properties = from p in this.GetType().GetProperties()
                             where p.GetValue(this, null) != null
                             select p.Name + "=" + System.Web.HttpUtility.UrlEncode(p.GetValue(this, null).ToString());
            return String.Join("&", properties.ToArray());

        }
    }

}
