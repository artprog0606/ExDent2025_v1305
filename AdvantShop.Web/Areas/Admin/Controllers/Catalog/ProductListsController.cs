using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.SEO;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Extensions;
using AdvantShop.Web.Admin.Handlers.Catalog;
using AdvantShop.Web.Admin.Handlers.Catalog.ProductLists;
using AdvantShop.Web.Admin.Models.Catalog;
using AdvantShop.Web.Admin.Models.Catalog.ProductLists;
using AdvantShop.Web.Admin.ViewModels.Catalog.ProductLists;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Web.Admin.Controllers.Catalog
{
    [Auth(RoleAction.Catalog)]
    public class ProductListsController : BaseAdminController
    {
        #region Product lists

        public ActionResult Index()
        {
            SetMetaInformation(T("Admin.ProductLists.Index.Title"));
            SetNgController(NgControllers.NgControllersTypes.ProductListsCtrl);

            return View();
        }

        public JsonResult GetProductLists(ProductListsFilterModel model)
        {
            return Json(new GetProductListsHandler(model).Execute());
        }

        public JsonResult GetProductListsMenu()
        {
            //var items = new List<ProductListsMenuItem>()
            //{
            //    new ProductListsMenuItem()
            //    {
            //        Name = T("Admin.MainPageProducts.BestTitle"),
            //        Type = EProductOnMain.Best
            //    },
            //    new ProductListsMenuItem()
            //    {
            //        Name = T("Admin.MainPageProducts.NewTitle"),
            //        Type = EProductOnMain.New
            //    },
            //    new ProductListsMenuItem()
            //    {
            //        Name = T("Admin.MainPageProducts.SalesTitle"),
            //        Type = EProductOnMain.Sale
            //    }
            //};

            var items = ProductListService.GetList().Select(x => new ProductListsMenuItem()
            {
                Id = x.Id,
                Name = x.Name,
                Enabled = x.Enabled,
                SortOrder = x.SortOrder,
                Type = EProductOnMain.List,
            });

            return Json(items);
        }

        public JsonResult GetProductList(int id)
        {
            return Json(ProductListService.Get(id));
        }
        
        [HttpGet]
        public JsonResult GetProductListUrl() =>
            Json(new UrlHelper(HttpContext.Request.RequestContext).AbsoluteClientRouteUrl("ProductList",
                new { type = EProductOnMain.List }).TrimEnd('/') + "/");

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddProductList(ProductList list)
        {
            try
            {
                return JsonOk(new AddProductListHandler(list).Execute());
            }
            catch (BlException e)
            {
                ModelState.AddModelError(e.Property, e.Message);
                return JsonError();
            }
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UpdateProductList(ProductList list)
        {
            try
            {
                return JsonOk(new UpdateProductListHandler(list).Execute());
            }
            catch (BlException e)
            {
                ModelState.AddModelError(e.Property, e.Message);
                return JsonError();
            }
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteProductList(int id)
        {
            ProductListService.Delete(id);
            return JsonOk();
        }

        private void ListsCommand(ProductListsFilterModel command, Action<int, ProductListsFilterModel> func)
        {
            if (command.SelectMode == SelectModeCommand.None)
            {
                foreach (var id in command.Ids)
                    func(id, command);
            }
            else
            {
                var ids = new GetProductListsHandler(command).GetItemsIds("Id");
                foreach (int id in ids)
                {
                    if (command.Ids == null || !command.Ids.Contains(id))
                        func(id, command);
                }
            }
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteProductLists(ProductListsFilterModel command)
        {
            ListsCommand(command, (id, c) => ProductListService.Delete(id));
            return JsonOk();
        }

        public JsonResult ChangeProductListsSorting(int id, int? prevId, int? nextId)
        {
            var result = new ChangeProductListSorting(id, prevId, nextId).Execute();
            return Json(new {result = result});
        }

        #endregion

        #region Products of list

        public ActionResult Products(int id)
        {
            var list = ProductListService.Get(id);
            if (list == null)
                return RedirectToAction("Index");

            var model = new PropductsByListViewModel() {ListId = list.Id, ListName = list.Name};

            SetMetaInformation(T("Admin.ProductLists.Index.Title"));
            SetNgController(NgControllers.NgControllersTypes.ProductListsCtrl);

            return View(model);
        }

        public JsonResult GetProductsByList(CatalogFilterModel model)
        {
            var handler = new GetProductsByList(model);
            var result = handler.Execute();

            return Json(result);
        }


        private void Command(CatalogFilterModel command, Action<int, CatalogFilterModel> func)
        {
            if (command.SelectMode == SelectModeCommand.None)
            {
                foreach (var id in command.Ids)
                    func(id, command);
            }
            else
            {
                var ids = new GetProductsByList(command).GetItemsIds("[Product].[ProductID]");
                foreach (int id in ids)
                {
                    if (command.Ids == null || !command.Ids.Contains(id))
                        func(id, command);
                }
            }
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteProductsFromList(CatalogFilterModel command)
        {
            Command(command, (id, c) =>
            {
                if (command.ListId != null)
                    ProductListService.DeleteProduct(command.ListId.Value, id);
            });
            return Json(true);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteFromList(int listId, int productId)
        {
            ProductListService.DeleteProduct(listId, productId);
            return Json(true);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddProducts(CatalogFilterModel model)
        {
            var listId = model.ListId ?? 0;

            if (ProductListService.Get(listId) == null)
                return Json(new {result = false});

            if (model.SelectMode == SelectModeCommand.None)
            {
                foreach (var id in model.Ids.Where(id => !ProductListService.IsExistsProduct(listId, id)))
                {
                    ProductListService.AddProduct(listId, id, 0);
                }
            }
            else
            {
                if (model.CategoryId == 0)
                    model.ShowMethod = ECatalogShowMethod.AllProducts;

                var handler = new GetCatalog(model);
                var ids = handler.GetItemsIds<int>("[Product].[ProductID]");

                foreach (var id in ids.Where(id => (model.Ids == null || !model.Ids.Contains(id)) &&
                                                   ProductListService.IsExistsProduct(listId, id)))
                {
                    ProductListService.AddProduct(listId, id, 0);
                }
            }

            return Json(new {result = true});
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult InplaceProduct(ProductListItemModel model)
        {
            ProductListService.UpdateProduct(model.ListId, model.ProductId, model.SortOrder);
            
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

        #endregion
    }
}
