using Newtonsoft.Json;
using System.Collections.Generic;

namespace AdvantShop.Core.Services.Api
{
    public class ApiPagination
    {
        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("currentPage")]
        public int CurrentPage { get; set; }

        [JsonProperty("totalCount")]
        public int TotalCount { get; set; }

        [JsonProperty("totalPageCount")]
        public int TotalPageCount { get; set; }
    }

    public class ApiPaginationResponse
    {
        [JsonProperty("pagination")]
        public ApiPagination Pagination { get; set; }
    }

    public class ApiPaginationResponse<T>
    {
        [JsonProperty("pagination")]
        public ApiPagination Pagination { get; set; }

        [JsonProperty("items")]
        public List<T> Items { get; set; }
    }
}
