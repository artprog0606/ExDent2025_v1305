using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AdvantShop.Web.Admin.Models.Settings.PaymentMethods
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class InfoByPaymentTypeModel
    {
        public string Label { get; set; }
        public string Value { get; set; }
        public string Code { get; set; }
        public string Type { get; set; }
        public int? ModuleId { get; set; }
        public string ModuleStringId { get; set; }
        public string ModuleVersion { get; set; }
        public string PriceString { get; set; }
        public float? Price { get; set; }
    }
}