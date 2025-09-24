using System.Web;
using System.Web.Mvc;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.Services.Security;
using AdvantShop.Customers;
using AdvantShop.Web.Admin.Handlers.Shared.Search;
using AdvantShop.Web.Admin.Models.Catalog;
using AdvantShop.Web.Admin.ViewModels.Shared.Common;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Web.Admin.Controllers.Shared
{
    public partial class SearchController : BaseAdminController
    {
        [ChildActionOnly]
        public ActionResult SearchBlock()
        {
            var model = new SearchViewModel();

            if (!string.IsNullOrWhiteSpace(Request["search"]))
                model.Search = HttpUtility.HtmlEncode(Request["search"]);

            return PartialView(model);
        }

        [HttpPost]
        public ActionResult Index(string search)
        {
            search = HttpUtility.HtmlEncode(search);

            return RedirectToAction("Index", "Catalog", new { showMethod = ECatalogShowMethod.AllProducts, search = search});
        }

        [HttpGet]
        [AuthorizeRole(RoleAction.Catalog, RoleAction.Orders)]
        public JsonResult Autocomplete(string q, string type)
        {
            if (string.IsNullOrWhiteSpace(q))
                return Json(null);

            var model = new GetSearchAutocomplete(q, type).Execute();

            return Json(model);
        }

        [HttpGet]
        public JsonResult SearchImagesById(int objId, PhotoType type, int page = 0)
        {
            try
            {
                var result = new SearchImages(objId, type, page).Execute();
                return Json(result);
            }
            catch (BlException ex)
            {
                return JsonError(ex.Message);
            }
        }
        
        public JsonResult SearchImagesEnabled()
        {
            return Json(new {enabled = SettingsCatalog.ImageSearchEnabled});
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SetSearchImagesEnabled()
        {
            SettingsCatalog.ImageSearchEnabled = true;
            SettingsCatalog.ShowImageSearchEnabled = true;
            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult HideSearchImagesEnabled()
        {
            SettingsCatalog.ShowImageSearchEnabled = false;
            return JsonOk();
        }
    }
}
