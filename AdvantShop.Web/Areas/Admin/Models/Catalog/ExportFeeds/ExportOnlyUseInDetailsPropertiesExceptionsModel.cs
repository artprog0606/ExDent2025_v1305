using System.Collections.Generic;
using Newtonsoft.Json;

namespace AdvantShop.Web.Admin.Models.Catalog.ExportFeeds
{
    public class ExportOnlyUseInDetailsPropertiesExceptionsModel
    {
        [JsonProperty("exceptionProperties")]
        public List<SelectItemModel<int>> ExceptionProperties { get; set; }
    }
}