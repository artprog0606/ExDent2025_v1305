using System.Collections.Generic;
using Newtonsoft.Json;

namespace AdvantShop.Core.Modules.Interfaces
{
    public interface IOrderManagement
    {
        List<OrderGridAction> GetOrderGridActions();
    }

    public class OrderGridAction
    {
        [JsonProperty("field")]
        public string Field { get; set; }
        [JsonProperty("text")]
        public string Text { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("template")]
        public string Template { get; set; }
        [JsonProperty("urlAfterAction")]
        public string UrlAfterAction { get; set; }
    }
}