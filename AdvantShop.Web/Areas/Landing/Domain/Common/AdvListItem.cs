using Newtonsoft.Json;

namespace AdvantShop.App.Landing.Domain.Common
{
    public class AdvListItem
    {
        [JsonProperty(PropertyName = "label")]
        public string Label { get; set; }

        [JsonProperty(PropertyName = "value")]
        public object Value { get; set; }

        public AdvListItem()
        {
        }

        public AdvListItem(string label, object value)
        {
            Label = label;
            Value = value;
        }
    }
}
