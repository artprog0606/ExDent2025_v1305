using System;
using AdvantShop.Core.Services.Api;
using Newtonsoft.Json;

namespace AdvantShop.Areas.Api.Models.Customers
{
    public sealed class AddUpdateCustomerCommand 
    {
        public bool? IgnoreCustomerFieldsConstraints { get; set; }
    }
    
    public sealed class AddUpdateCustomerModel : CustomerModel
    {
        [JsonProperty("partnerId")]
        public int? PartnerId { get; set; }
        
        public string FcmToken { get; set; }
    }
    
    public sealed class AddUpdateCustomerResponse : ApiResponse
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }
    }
}