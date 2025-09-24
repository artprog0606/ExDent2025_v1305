using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.FullSearch;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Saas;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Catalog;
using AdvantShop.Web.Admin.Handlers.Catalog.Categories;
using AdvantShop.Web.Admin.Models.Catalog;
using AdvantShop.Web.Admin.Models.Catalog.Categories;
using AdvantShop.Web.Admin.ViewModels.Catalog;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Filters;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using AdvantShop.Core.Services.Catalog.Warehouses;

namespace AdvantShop.Web.Admin.Controllers.Catalog
{
    [Auth(RoleAction.Catalog)]
    public class CatalogController : BaseAdminController
    {
        private const int FilterFieldSearchLimit = 350;
        
        // GET: Admin/Catalog
        public ActionResult Index(CatalogFilterModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Search) && string.IsNullOrEmpty(model.From))
            {
                var product = ProductService.GetProduct(model.Search, true);
                if (product != null)
                    return RedirectToAction("Edit", "Product", new { id = product.ProductId });
            }

            SetMetaInformation(CategoryService.GetCategory(0).Name);
            SetNgController(NgControllers.NgControllersTypes.CatalogCtrl);

            var viewModel = new GetCatalogIndexHandler(model).Execute();
            if (viewModel == null)
                return Error404();

            return View(viewModel);
        }

        public JsonResult GetCatalog(CatalogFilterModel model)
        {
            return Json(new GetCatalog(model).Execute());
        }

        [ChildActionOnly]
        public ActionResult CatalogLeftMenu(string ngCallbackOnInit)
        {
            var model = new GetCatalogLeftMenu().Execute();
            model.NgCallbackOnInit = ngCallbackOnInit;

            return PartialView(model);
        }

        public JsonResult GetDataProducts()
        {
            return Json(new GetCatalogLeftMenu().Execute());
        }

        [ChildActionOnly]
        public ActionResult CatalogTreeView(AdminCatalogTreeView model)
        {
            return PartialView(model);
        }

        #region CategoriesTree

        [Auth(RoleAction.Catalog, RoleAction.Orders, RoleAction.Crm)]
        public JsonResult CategoriesTree(CategoriesTree model)
        {
            return Json(new GetCategoriesTree(model).Execute());
        }

        [Auth(RoleAction.Catalog, RoleAction.Orders, RoleAction.Crm)]
        public JsonResult GetSelectedCategoriesTree(List<CategoriesSelectedModel> categoriesSelected)
        {
            return Json(new GetSelectedCategoriesTree(categoriesSelected).Execute());
        }

        [Auth(RoleAction.Catalog, RoleAction.Orders, RoleAction.Crm)]
        public JsonResult CategoriesTreeBySearchRequest(string str)
        {
            return Json(new GetCategoriesTreeBySearchRequest(str).Execute());
        }

        #endregion

        #region Categories

        public JsonResult CategoryListJson(int categoryId, string categorySearch)
        {
            return Json(new GetCategoryList(categoryId, categorySearch).Execute());
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ChangeCategorySortOrder(int categoryId, int? prevCategoryId, int? nextCategoryId, int? parentCategoryId)
        {
            var result = new ChangeCategorySortOrder(categoryId, prevCategoryId, nextCategoryId, parentCategoryId).Execute();
            return Json(result);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ChangeParentCategory(int categoryId, int parentId)
        {
            var result = new ChangeParentCategory(categoryId, parentId).Execute();
            return Json(result);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteCategories(List<int> categoryIds)
        {
            foreach (var categoryId in categoryIds.Where(x => x != 0))
            {
                CategoryService.DeleteCategoryAndPhotos(categoryId);
            }
            CategoryService.RecalculateProductsCountManual();

            return JsonOk();
        }

        #endregion Categories

        #region Inplace

        [HttpPost]
        [ValidateJsonAntiForgeryToken]
        public JsonResult InplaceProduct(string price, string amount, CatalogProductModel model, int categoryId, ECatalogShowMethod showMethod)
        {
            var product = ProductService.GetProduct(model.ProductId);
            if (product == null)
                return JsonError("Товар не найден");

            if (model.Enabled && !product.Enabled && SaasDataService.IsSaasEnabled && ProductService.GetProductsCount("[Enabled] = 1") >= SaasDataService.CurrentSaasData.ProductsCount)
                return JsonError(T("Admin.Catalog.TariffLimitations"));

            var enabledChanged = product.Enabled != model.Enabled;
            var isAvailableChanged = false;

            product.Enabled = model.Enabled;
            product.ModifiedBy = CustomerContext.CustomerId.ToString();

            if (categoryId != 0 && showMethod == ECatalogShowMethod.Normal)
            {
                ProductService.UpdateProductLinkSort(product.ProductId, model.SortOrder, categoryId);
            }

            if (product.Offers.Count == 1)
            {
                model.Amount = Regex.Replace(amount, "\\s", "").TryParseFloat();
                
                var offerStocks = WarehouseStocksService.GetOfferStocks(product.Offers[0].OfferId);
                if (offerStocks.Count == 1)
                {
                    var offerStock = offerStocks.Single();
                    isAvailableChanged = offerStock.Quantity != model.Amount &&
                                         (offerStock.Quantity >= 0 && model.Amount <= 0 ||
                                          offerStock.Quantity <= 0 && model.Amount >= 0);
                    
                    offerStock.Quantity = model.Amount;
                    
                    if (offerStock.Quantity > 1000000)
                        offerStock.Quantity = 1000000;
                    
                    WarehouseStocksService.AddUpdateStocks(offerStock, trackChanges: true);
                }
                else if (offerStocks.Count == 0)
                {
                    var warehouses = WarehouseService.GetList();
                    if (warehouses.Count == 1)
                    {
                        var offerStock = new WarehouseStock
                        {
                            OfferId = product.Offers[0].OfferId,
                            Quantity = model.Amount,
                            WarehouseId = warehouses.Single().Id
                        };
                        if (offerStock.Quantity > 1000000)
                            offerStock.Quantity = 1000000;
                        
                        WarehouseStocksService.AddUpdateStocks(offerStock, trackChanges: true);

                        isAvailableChanged = true;
                    }
                }

                if (!string.IsNullOrEmpty(price))
                {
                    product.Offers[0].BasePrice = model.Price = Regex.Replace(price, "\\s", "").TryParseFloat();
                }
            }

            ProductService.UpdateProduct(product, true, true);

            var reloadCatalogTree = false;
            
            if (enabledChanged || isAvailableChanged)
            {
                CategoryService.RecalculateProductsCountInCategories(product.ProductId);
                reloadCatalogTree = true;
            }

            return Json(new { result = true, entity = model, reloadCatalogTree });
        }

        [HttpPost]
        [ValidateJsonAntiForgeryToken]
        public JsonResult DeleteProduct(int productId)
        {
            ProductService.DeleteProduct(productId, true);
            CategoryService.RecalculateProductsCountManual();
            CategoryService.CalculateHasProductsForAllWarehouseInAllCategories();

            return Json(new { result = true, reloadCatalogTree = true });
        }

        #endregion Inplace

        #region Comands

        private void Command(CatalogFilterModel command, Action<int, CatalogFilterModel> func)
        {
            var exceptions = new ConcurrentQueue<Exception>();

            if (command.SelectMode == SelectModeCommand.None)
            {
                Parallel.ForEach(command.Ids,
                    new ParallelOptions { MaxDegreeOfParallelism = SettingsMain.UseMultiThreads ? 10 : 1 }, (id) =>
                      {
                          try
                          {
                              func(id, command);
                          }
                          catch (Exception e)
                          {
                              exceptions.Enqueue(e);
                          }
                      });
            }
            else
            {
                var ids = new GetCatalog(command).GetItemsIds<int>("[Product].[ProductID]");

                Parallel.ForEach(ids,
                    new ParallelOptions { MaxDegreeOfParallelism = SettingsMain.UseMultiThreads ? 10 : 1 }, (id) =>
                      {
                          try
                          {
                              if (command.Ids == null || !command.Ids.Contains(id))
                                  func(id, command);
                          }
                          catch (Exception e)
                          {
                              exceptions.Enqueue(e);
                          }
                      });
            }

            if (exceptions.Any())
            {
                Debug.Log.Error(exceptions.AggregateString("<br/>^^^<br/>"));

                if (SettingsMain.UseMultiThreads)
                {
                    SettingsMain.UseMultiThreads = false;
                    Command(command, func);
                }
            }

            CategoryService.RecalculateProductsCountManual();
            CategoryService.CalculateHasProductsForAllWarehouseInAllCategories();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteProducts(CatalogFilterModel command)
        {
            Command(command, (id, c) => ProductService.DeleteProduct(id, false));
            ProductWriter.CreateIndexInTask();
            
            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteFromCategoryProducts(CatalogFilterModel command)
        {
            Command(command, (id, c) =>
            {
                if (c.CategoryId != null)
                {
                    CategoryService.DeleteCategoryAndLink(id, c.CategoryId.Value);
                    ProductService.SetProductHierarchicallyEnabled(id);
                }
            });

            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ActivateProducts(CatalogFilterModel command)
        {
            if (SaasDataService.IsSaasEnabled && ProductService.GetProductsCount("[Enabled] = 1") >= SaasDataService.CurrentSaasData.ProductsCount)
            {
                return JsonError(T("Admin.Catalog.TariffLimitations"));
            }
            Command(command, (id, c) => ProductService.SetActive(id, true));
            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DisableProducts(CatalogFilterModel command)
        {
            Command(command, (id, c) => ProductService.SetActive(id, false));
            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ChangeProductCategory(CatalogFilterModel command, List<int> newCategoryIds, bool removeFromCurrentCategories)
        {
            Command(command, (id, c) =>
            {
                if (removeFromCurrentCategories)
                {
                    foreach (var catId in ProductService.GetCategoriesIDsByProductId(id, false))
                        ProductService.DeleteProductLink(id, catId);
                }
                foreach (var categoryId in newCategoryIds)
                {
                    ProductService.AddProductLink(id, categoryId, 0, false, trackChanges: true);
                }
            });
            foreach (var categoryId in newCategoryIds)
                CategoryService.SetCategoryHierarchicallyEnabled(categoryId);
            
            CategoryService.CalculateHasProductsForAllWarehouseInAllCategories();
            ProductService.PreCalcProductParamsMassInBackground();
            CategoryService.ClearCategoryCache();

            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddPropertyToProducts(CatalogFilterModel command, int? selectedPropertyId, int? selectedPropertyValueId,
                                                string selectedPropertyName, string selectedPropertyValue)
        {
            var realPropertyId = 0;
            if (selectedPropertyId != null)
            {
                var property = PropertyService.GetPropertyById(selectedPropertyId.Value);
                if (property != null)
                    realPropertyId = property.PropertyId;
            }

            if (realPropertyId == 0 && !string.IsNullOrWhiteSpace(selectedPropertyName))
            {
                realPropertyId = PropertyService.AddProperty(new Property()
                {
                    Name = selectedPropertyName.Trim(),
                    UseInFilter = true,
                    UseInDetails = true,
                    Type = 1
                });
            }

            if (realPropertyId == 0)
                return JsonError();

            var realPropertyValueId = 0;
            if (selectedPropertyValueId != null)
            {
                var value = PropertyService.GetPropertyValueById(selectedPropertyValueId.Value);
                if (value != null)
                    realPropertyValueId = value.PropertyValueId;
            }

            if (realPropertyValueId == 0 && !string.IsNullOrWhiteSpace(selectedPropertyValue))
            {
                if (selectedPropertyId != null)
                {
                    var propValue = PropertyService.GetPropertyValueByName(realPropertyValueId, selectedPropertyValue.Trim());
                    if (propValue != null)
                        realPropertyValueId = propValue.PropertyValueId;
                }

                if (realPropertyValueId == 0)
                {
                    realPropertyValueId = PropertyService.AddPropertyValue(new PropertyValue()
                    {
                        PropertyId = realPropertyId,
                        Value = selectedPropertyValue.Trim()
                    });
                }
            }

            if (realPropertyValueId == 0)
                return JsonError();

            Command(command, (id, c) =>
            {
                var propValue = PropertyService.GetPropertyValuesByProductId(id)
                    .FirstOrDefault(x => x.PropertyValueId == realPropertyValueId);
                if (propValue == null)
                {
                    PropertyService.AddProductProperyValue(realPropertyValueId, id);
                }
            });

            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult RemovePropertyFromProducts(CatalogFilterModel command, int selectedPropertyValueId)
        {
            var propertyValue = PropertyService.GetPropertyValueById(selectedPropertyValueId);
            if (propertyValue == null)
                return JsonError();

            Command(command, (id, c) =>
            {
                PropertyService.DeleteProductPropertyValue(id, propertyValue.PropertyValueId);
            });

            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddTagsToProducts(CatalogFilterModel command, List<string> newTags)
        {
            if (SaasDataService.IsSaasEnabled && !SaasDataService.CurrentSaasData.HaveTags)
                return JsonError();

            foreach (var tag in newTags)
            {
                if (TagService.Get(tag) == null)
                {
                    TagService.Add(new Tag
                    {
                        Name = tag,
                        UrlPath = StringHelper.TransformUrl(StringHelper.Translit(tag)),
                        Enabled = true,
                        VisibilityForUsers = true
                    });
                }
            }

            Command(command, (id, c) =>
            {
                var product = ProductService.GetProduct(id);
                var prevTags = TagService.Gets(product.ProductId, ETagType.Product).Select(x => x.Name).ToList();

                product.Tags = prevTags.Concat(newTags.Where(x => !prevTags.Contains(x))).Select(x => new Tag
                {
                    Name = x,
                    UrlPath = StringHelper.TransformUrl(StringHelper.Translit(x)),
                    Enabled = true,
                    VisibilityForUsers = true
                }).ToList();

                ProductService.UpdateProduct(product, false);
            });

            ProductService.PreCalcProductParamsMassInBackground();
            Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Products_AddTagToProduct);

            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult RemoveTagsToProducts(CatalogFilterModel command, List<Tag> removeTags)
        {
            Command(command, (id, c) =>
            {
                var product = ProductService.GetProduct(id);
                
                foreach (var tag in removeTags.Where(tag => product.Tags.Any(x => x.Name == tag.Name)))
                {
                    product.Tags.RemoveAll(x => x.Name == tag.Name);
                }
                
                ProductService.UpdateProduct(product, false);
            });
            
            ProductService.PreCalcProductParamsMassInBackground();
                
            return JsonOk();
        }

        #endregion Comands

        public JsonResult GetPriceRangeForPaging(CatalogFilterModel command)
        {
            var handler = new GetCatalog(command);
            var price =
                handler.GetItemsIds<CatalogRangeModel>("Max(Price) as Max, Min(Price) as Min")
                    .FirstOrDefault();

            return Json(new { from = price != null ? price.Min : 0, to = price != null ? price.Max : 10000000 });
        }

        public JsonResult GetAmountRangeForPaging(CatalogFilterModel command)
        {
            var handler = new GetCatalog(command);
            var amounts =
                handler.GetItemsIds<float>("ISNULL((Select sum (Amount) from catalog.Offer where Offer.ProductID=Product.productID), 0) as Amount");

            var min = amounts != null ? amounts.Min() : 0;
            var max = amounts != null ? amounts.Max() : 10000000;

            return Json(new { from = min, to = max });
        }

        public JsonResult GetBrandList(string q, int page = 1, int? brandId = null)
        {
            var brands = BrandService.GetBrandsBySearch(500, page, q);
            if (brandId != null)
            {
                var b = BrandService.GetBrandById(brandId.Value);
                if (b != null)
                    brands.Add(b);
            }
            return Json(brands.Select(x => new { label = x.Name, value = x.BrandId }));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult RecalculateProductsCount()
        {
            CategoryService.RecalculateProductsCountManual();
            CategoryService.SetCategoryHierarchicallyEnabled(0);
            CategoryService.CalculateHasProductsForAllWarehouseInAllCategories();
            ProductService.PreCalcProductParamsMassInBackground();
            CategoryService.ClearCategoryCache();

            return JsonOk();
        }

        [HttpGet]
        [Auth(RoleAction.Catalog, RoleAction.Orders, RoleAction.Crm)]
        public JsonResult GetOffersCatalog(CatalogFilterModel model)
        {
            return Json(new GetCatalogOffers(model).Execute());
        }

        [HttpGet]
        [Auth(RoleAction.Catalog, RoleAction.Orders, RoleAction.Crm)]
        public JsonResult GetCatalogIds(CatalogFilterModel command)
        {
            return Json(new { ids = new GetCatalog(command).GetItemsIds<int>("[Product].[ProductID]") });
        }

        [HttpGet]
        [Auth(RoleAction.Catalog, RoleAction.Orders, RoleAction.Crm)]
        public JsonResult GetCatalogOfferIds(CatalogFilterModel command)
        {
            return Json(new { ids = new GetCatalogOffers(command).GetItemsIds<int>("[Offer].[OfferId]") });
        }

        [HttpGet]
        public JsonResult GetCategoryName(int categoryId)
        {
            var category = CategoryService.GetCategory(categoryId);
            return Json(new { name = (category != null ? category.Name : null) });
        }
        
        [HttpPost]
        public JsonResult GetCategoryNames(List<int> categoryIds)
        {
            var categories = new List<Category>();
            foreach (var categoryId in categoryIds)
            {
                var category = CategoryService.GetCategory(categoryId);
                if (category != null)
                    categories.Add(category);
            }
            return Json( categories.Select(x => new {categoryId = x.CategoryId, name = x.Name}));
        }

        public JsonResult GetColorList(string q, int page = 1, int? colorId = null)
        {
            var colors = ColorService.GetAllColorsByPaging(FilterFieldSearchLimit, page, q);
            if (colorId != null && colors.Any(x => x.ColorId == colorId) is false)
            {
                var c = ColorService.GetColor(colorId.Value);
                if (c != null)
                    colors.Add(c);
            }
            return Json(colors.Select(x => new { label = x.ColorName, value = x.ColorId }));
        }

        public JsonResult GetSizeList(string q, int page = 1, int? sizeId = null)
        {
            var sizes = SizeService.GetAllSizesByPaging(FilterFieldSearchLimit, page, q);
            if (sizeId != null && sizes.Any(x => x.SizeId == sizeId) is false)
            {
                var s = SizeService.GetSize(sizeId.Value);
                if (s != null)
                    sizes.Add(s);
            }
            return Json(sizes.Select(x => new { label = x.SizeName, value = x.SizeId }));
        }

        public JsonResult GetPropertyList(string q, int page = 1, int? propertyId = null)
        {
            var properties = PropertyService.GetAllPropertiesByPaging(FilterFieldSearchLimit, page, q);
            if (propertyId != null && properties.Any(x => x.PropertyId == propertyId) is false)
            {
                var p = PropertyService.GetPropertyById(propertyId.Value);
                if (p != null)
                    properties.Add(p);
            }
            
            return Json(properties.Select(x => new { label = x.Name, value = x.PropertyId }));
        }

        public JsonResult GetPropertyValueList(string q, int page = 1, int? propertyId = null, int? propertyValueId = null)
        {
            var propertyValues = PropertyService.GetAllPropertyValuesByPaging(FilterFieldSearchLimit, page, q, propertyId);
            // if (propertyValueId != null)
            // {
            //     var pv = PropertyService.GetPropertyValueById(propertyValueId.Value);
            //     if (pv != null)
            //         propertyValues.Add(pv);
            // }
            
            return Json(propertyValues.Select(x => new { label = x.Value, value = x.PropertyValueId }));
        }

        public JsonResult GetTags()
        {
            return Json(new
            {
                tags = TagService.GetAutocompleteTags().Select(x => new { value = x.Name })
            });
        }

        public JsonResult GetProductsTags(CatalogFilterModel command)
        {
            List<Tag> tags = new List<Tag>();
            
            Command(command, (id, c) =>
            {
                var product = ProductService.GetProduct(id);
                
                tags.AddRange(product.Tags.Where(tag => tags.All(x => x.Name != tag.Name)));
            });
            
            return Json(new
            {
                Tags = tags
            });
        }

        public JsonResult GetProductSelectvizrSettings()
        {
            var model = new AdminCatalog();
            return Json(new
            {
                IsTagsFilterVisible = model.IsTagsVisible,
                IsWarehouseFilterVisible = model.IsWarehouseFilterVisible
            });
        }
    }
}