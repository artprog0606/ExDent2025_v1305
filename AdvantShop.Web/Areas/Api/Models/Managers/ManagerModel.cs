using Newtonsoft.Json;

namespace AdvantShop.Areas.Api.Models.Managers
{
    public class ManagerModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}