using AdvantShop.Core.Services.Api;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace AdvantShop.Areas.Api.Models.Customers
{
    public class GetCustomersItem : CustomerModel
    {

    }

    public class GetCustomersResponse : ApiPaginationResponse, IApiResponse
    {
        [JsonProperty("customers")]
        public List<GetCustomersItem> Customers { get; set; }
    }
}