using System.Collections.Generic;
using AdvantShop.Web.Admin.Models.Catalog.Categories;
using Newtonsoft.Json;

namespace AdvantShop.Web.Admin.Models.Catalog.Products
{
    public class UploadPhotoResultModel
    {
        [JsonProperty("result")]
        public bool Result { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }

        public List<UploadPictureResult> PhotoResults { get; set; }
    }
}
