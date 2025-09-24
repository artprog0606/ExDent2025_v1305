using System.Web.Mvc;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.SalesChannels;
using AdvantShop.Customers;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Extensions;
using AdvantShop.Web.Admin.Models.Catalog.ProductLists;
using AdvantShop.Web.Admin.ViewModels.Catalog.MainPageProducts;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Web.Admin.Controllers.Catalog
{
    [Auth(RoleAction.Catalog)]
    [SalesChannel(ESalesChannelType.Store)]
    public partial class MainPageProductsStoreController : BaseAdminController
    {
        public ActionResult Index(EProductOnMain type = EProductOnMain.Best, int? listId = null)
        {
            if (type == EProductOnMain.None)
                return Error404();

            var model = new MainPageProductsViewModel() {Type = type, ListId = listId.HasValue ? listId.Value : 0 };

            switch (type)
            {
                case EProductOnMain.Best:
                    model.Title = T("Admin.MainPageProducts.BestTitle");
                    break;
                case EProductOnMain.New:
                    model.Title = T("Admin.MainPageProducts.NewTitle");
                    break;
                case EProductOnMain.Sale:
                    model.Title = T("Admin.MainPageProducts.SalesTitle");
                    break;
            }

            SetMetaInformation(model.Title);
            SetNgController(NgControllers.NgControllersTypes.MainPageProductsCtrl);

            return View(model);
        }

        public JsonResult GetItemByType(EProductOnMain type, int? id)
        {
            var model = new ProductListsMenuItem() {Type = type};
            var urlHelper = new UrlHelper(HttpContext.Request.RequestContext);

            switch (type)
            {
                case EProductOnMain.Best:
                    model.Name = T("Admin.MainPageProducts.BestTitle");
                    model.Enabled = SettingsCatalog.BestEnabled;
                    model.ShuffleList = SettingsCatalog.ShuffleBestOnMainPage;
                    model.ShowOnMainPage = SettingsCatalog.ShowBestOnMainPage;
                    model.Link = urlHelper.AbsoluteClientRouteUrl("ProductList", new { type });
                    break;

                case EProductOnMain.New:
                    model.Name = T("Admin.MainPageProducts.NewTitle");
                    model.Enabled = SettingsCatalog.NewEnabled;
                    model.DisplayLatestProductsInNewOnMainPage = SettingsCatalog.DisplayLatestProductsInNewOnMainPage;
                    model.ShuffleList = SettingsCatalog.ShuffleNewOnMainPage;
                    model.ShowOnMainPage = SettingsCatalog.ShowNewOnMainPage;
                    model.Link = urlHelper.AbsoluteClientRouteUrl("ProductList", new { type });
                    break;
                case EProductOnMain.Sale:
                    model.Name = T("Admin.MainPageProducts.SalesTitle");
                    model.Enabled = SettingsCatalog.SalesEnabled;
                    model.ShuffleList = SettingsCatalog.ShuffleSalesOnMainPage;
                    model.ShowOnMainPage = SettingsCatalog.ShowSalesOnMainPage;
                    model.Link = urlHelper.AbsoluteClientRouteUrl("ProductList", new { type });
                    break;
                case EProductOnMain.List:
                    var list = id != null ? ProductListService.Get(id.Value) : null;
                    if (list != null)
                    {
                        model.Id = list.Id;
                        model.Name = list.Name;
                        model.Enabled = list.Enabled;
                        model.ShowOnMainPage = list.ShowOnMainPage;
                        model.SortOrder = list.SortOrder;
                        model.ShuffleList = list.ShuffleList;
                        model.ShowOnMainPage = list.ShowOnMainPage;
                        model.Link = urlHelper.AbsoluteClientRouteUrl("ProductList", new { type, list = list.UrlPath });
                    }
                    break;
            }

            return Json(model);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ChangeEnabled(EProductOnMain type, bool enabled, int? id)
        {
            switch (type)
            {
                case EProductOnMain.Best:
                    SettingsCatalog.BestEnabled = enabled;
                    break;

                case EProductOnMain.New:
                    SettingsCatalog.NewEnabled = enabled;
                    break;

                case EProductOnMain.Sale:
                    SettingsCatalog.SalesEnabled = enabled;
                    break;

                case EProductOnMain.List:
                    var list = id != null ? ProductListService.Get(id.Value) : null;
                    if (list == null)
                        return JsonError();

                    list.Enabled = enabled;
                    ProductListService.Update(list);
                    break;
            }

            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ChangeDisplayLatestProductsInNewOnMainPageEnabled(bool enabled)
        {
            SettingsCatalog.DisplayLatestProductsInNewOnMainPage = enabled;
            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ChangeShuffleList(EProductOnMain type, bool shuffleList, int? id)
        {
            switch (type)
            {
                case EProductOnMain.Best:
                    SettingsCatalog.ShuffleBestOnMainPage = shuffleList;
                    break;

                case EProductOnMain.New:
                    SettingsCatalog.ShuffleNewOnMainPage = shuffleList;
                    break;

                case EProductOnMain.Sale:
                    SettingsCatalog.ShuffleSalesOnMainPage = shuffleList;
                    break;

                case EProductOnMain.List:
                    var list = id != null ? ProductListService.Get(id.Value) : null;
                    if (list == null)
                        return JsonError();

                    list.ShuffleList = shuffleList;
                    ProductListService.Update(list);
                    break;
            }

            return JsonOk();
        }
        
        public JsonResult ChangeShowOnMainPage(EProductOnMain type, bool showOnMainPage, int? id)
        {
            switch (type)
            {
                case EProductOnMain.Best:
                    SettingsCatalog.ShowBestOnMainPage = showOnMainPage;
                    break;
                
                case EProductOnMain.New:
                    SettingsCatalog.ShowNewOnMainPage = showOnMainPage;
                    break;
                
                case EProductOnMain.Sale:
                    SettingsCatalog.ShowSalesOnMainPage = showOnMainPage;
                    break;
                
                case EProductOnMain.List:
                    var list = id != null ? ProductListService.Get(id.Value) : null;
                    if (list == null)
                        return JsonError();

                    list.ShowOnMainPage = showOnMainPage;
                    ProductListService.Update(list);
                    break;
            }

            return JsonOk();
        }
    }
}