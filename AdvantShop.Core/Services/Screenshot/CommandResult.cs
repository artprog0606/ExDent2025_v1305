using Newtonsoft.Json;

namespace AdvantShop.Core.Services.Screenshot
{
    public class CommandResult<T>
    {
        [JsonProperty(PropertyName = "message", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Message { get; set; }

        [JsonProperty(PropertyName = "obj", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public T Obj { get; set; }
    }
}
