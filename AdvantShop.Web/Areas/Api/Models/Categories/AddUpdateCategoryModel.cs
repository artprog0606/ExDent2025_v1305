using AdvantShop.Core.Services.Api;
using Newtonsoft.Json;

namespace AdvantShop.Areas.Api.Models.Categories
{
    public class AddUpdateCategoryModel : CategoryModel
    {
    }

    public class AddUpdateCategoryResponse : ApiResponse
    {
        [JsonProperty("id")]
        public int Id { get; set; }
    }
}