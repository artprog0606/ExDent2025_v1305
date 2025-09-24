using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AdvantShop.Core.Services.Crm.OzonSeller.Api
{
    public class GetCategoryAttributeParams
    {
        public int CategoryId { get; set; }
        public ParamAttributeType? AttributeType { get; set; }
        public string Language { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter), converterParameters: true /*camelCaseText*/)]
    public enum ParamAttributeType
    {
        Required,
        Optional
    }
}
