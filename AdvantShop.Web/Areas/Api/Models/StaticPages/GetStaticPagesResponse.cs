using System.Collections.Generic;
using AdvantShop.Core.Services.Api;
using Newtonsoft.Json;

namespace AdvantShop.Areas.Api.Models.StaticPages
{
    public class GetStaticPagesResponse : IApiResponse
    {
        public ApiPagination Pagination { get; set; }
        public List<StaticPageItem> StaticPages { get; set; }
    }

    public class StaticPageItem
    {
        public int Id { get; set; }

        public string Title { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Text { get; set; }
        
        public string Icon { get; set; }

        public bool ShowInProfile { get; set; }
    }
}