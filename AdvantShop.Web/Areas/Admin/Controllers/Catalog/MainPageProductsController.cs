using System;
using System.Collections.Generic;
using System.Web.Mvc;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Customers;
using AdvantShop.SEO;
using AdvantShop.Web.Admin.Models.Catalog;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Filters;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.SalesChannels;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Catalog.MainPageProducts;
using AdvantShop.Web.Admin.Models.Catalog.MainPageProducts;
using AdvantShop.Web.Admin.ViewModels.Catalog.MainPageProducts;

namespace AdvantShop.Web.Admin.Controllers.Catalog
{
    [Auth(RoleAction.Catalog)]
    [SalesChannel(ESalesChannelType.Store)]
    public partial class MainPageProductsController : BaseAdminController
    {
        public ActionResult Index(EProductOnMain type = EProductOnMain.Best)
        {
            if (type == EProductOnMain.None)
                return Error404();

            var model = new MainPageProductsViewModel(){Type = type};

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


        public JsonResult GetCatalog(CatalogFilterModel model, EProductOnMain type)
        {
            return Json(new GetMainPageProducts(model, type).Execute());
        }

        #region Edit list

        [HttpGet]
        public JsonResult GetMainPageList(EProductOnMain type) =>
            Json(new GetMainPageListHandler(type).Execute());

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UpdateMainPageList(MainPageProductsModel model) =>
            ProcessJsonResult(new UpdateMainPageListHandler(model));

        #endregion

        #region Inplace

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult InplaceProduct(MainPageProductModel model, EProductOnMain type)
        {
            if (model.ProductId != 0 && type != EProductOnMain.None)
            {
                ProductOnMain.UpdateProductByType(model.ProductId, model.SortOrder, type);
            }
            
            var product = ProductService.GetProduct(model.ProductId);
            if (product == null)
                return JsonError("Товар не найден");
            
            var enabledChanged = product.Enabled != model.Enabled;

            product.Enabled = model.Enabled;
            product.ModifiedBy = CustomerContext.CustomerId.ToString();
            
            ProductService.UpdateProduct(product, true, true);

            var reloadCatalogTree = false;
            if (enabledChanged)
            {
                CategoryService.RecalculateProductsCountInCategories(product.ProductId);
                reloadCatalogTree = true;
            }

            return Json(new { result = true, entity = model, reloadCatalogTree });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteFromList(EProductOnMain type, int productId)
        {
            ProductOnMain.DeleteProductByType(productId, type);
            return JsonOk();
        }

        #endregion

        #region Commands

        private void Command(CatalogFilterModel command, EProductOnMain type, Action<int, EProductOnMain, CatalogFilterModel> func)
        {
            if (command.SelectMode == SelectModeCommand.None)
            {
                foreach (var id in command.Ids)
                    func(id, type, command);
            }
            else
            {
                var ids = new GetMainPageProducts(command, type).GetItemsIds("[Product].[ProductID]");
                foreach (int id in ids)
                {
                    if (command.Ids == null || !command.Ids.Contains(id))
                        func(id, type, command);
                }
            }
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteProductsFromList(CatalogFilterModel command, EProductOnMain type)
        {
            Command(command, type, (id, t, c) => ProductOnMain.DeleteProductByType(id, t));
            return Json(true);
        }

        #endregion


        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddProducts(EProductOnMain type, List<int> ids)
        {
            if (type == EProductOnMain.None || ids == null || ids.Count == 0)
                return Json(new {result = false});

            foreach (var id in ids)
                ProductOnMain.AddProductByType(id, type);
            
            return Json(new { result = true });
        }

    }
}