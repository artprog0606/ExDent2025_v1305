using Newtonsoft.Json;

namespace AdvantShop.Web.Admin.Models.Tasks
{
    public class TaskCopyResult
    {
        [JsonProperty("result")]
        public bool Result { get; set; }

        [JsonProperty("taskId")]
        public int TaskId { get; set; }
    }
}
