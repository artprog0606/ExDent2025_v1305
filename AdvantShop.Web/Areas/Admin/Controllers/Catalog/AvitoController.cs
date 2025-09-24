using System.Web.Mvc;
using System.Collections.Generic;
using AdvantShop.ExportImport;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Web.Admin.Controllers.Catalog
{
    public class AvitoController : Controller
    {
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult GetProperties(int productId)
        {
            return Json(new { result = true, obj = ExportFeedAvitoService.GetProductProperties(productId) });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SaveProperties(int productId, List<ExportFeedAvitoProductProperty> properties)
        {
            ExportFeedAvitoService.DeleteProductProperties(productId);
            if (properties == null || properties.Count == 0)
                return Json(new {result = true});

            return Json(new { result = ExportFeedAvitoService.AddProductProperties(properties) });
        }
    }
}
