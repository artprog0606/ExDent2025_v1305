using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.SessionState;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Services.Catalog.Warehouses;
using AdvantShop.Core.Services.Configuration;
using AdvantShop.Core.Services.SEO;
using AdvantShop.Core.Services.Statistic;
using AdvantShop.Customers;
using AdvantShop.Handlers.Search;
using AdvantShop.Models.Catalog;
using AdvantShop.Models.Search;
using AdvantShop.SEO;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Extensions;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Controllers
{
    [SessionState(SessionStateBehavior.Disabled)]
    public partial class SearchController : BaseClientController
    {
        [AccessByChannel(EProviderSetting.StoreActive)]
        public ActionResult Index(SearchCatalogModel model)
        {
            if (model.Page != null && model.Page < 1)
                return Error404();

            var viewModel = new SearchPagingHandler(model, false).Get();

            ModulesExecuter.Search(model.Q, viewModel.Pager.TotalItemsCount);

            if (model.Q.IsNotEmpty())
            {
                var url = Request.Url.ToString();
                url = url.Substring(url.LastIndexOf("/"), url.Length - url.LastIndexOf("/"));
                var cat = CategoryService.GetCategory(viewModel.Filter.CategoryId);
                var catName = "все категории";
                if (cat != null)
                    catName = cat.Name;
                StatisticService.AddSearchStatistic(url, model.Q, string.Format(T("Search.Index.SearchIn"),
                        catName,
                        viewModel.Filter.PriceFrom ?? 0,
                        viewModel.Filter.PriceTo == null ? "∞" : viewModel.Filter.PriceTo.ToString()),
                        viewModel.Pager.TotalItemsCount,
                        CustomerContext.CurrentCustomer.Id);
                WriteLog(model.Q, Request.Url.AbsoluteUri, ePageType.searchresults);
            }

            if ((viewModel.Pager.TotalPages < viewModel.Pager.CurrentPage && viewModel.Pager.CurrentPage > 1) ||
                viewModel.Pager.CurrentPage < 0)
            {
                return Error404();
            }
            
            var gtm = GoogleTagManagerContext.Current;
            if (gtm.Enabled)
            {
                gtm.PageType = ePageType.searchresults;
                gtm.ProdIds = new List<string>();
                if (viewModel.HasProducts)
                {
                    foreach (var item in viewModel.Products.Products)
                        gtm.ProdIds.Add(item.ArtNo);
                }
            }

            SetNgController(NgControllers.NgControllersTypes.CatalogCtrl);
            SetMetaInformation(new MetaInfo($"{SettingsMain.ShopName} - {T("Search.Index.SearchTitle")}"), model.Q, page: model.Page ?? 1);

            viewModel.SearchCatalogModel.Q = ClearSearch(viewModel.SearchCatalogModel.Q);
            return View(viewModel);
        }

        public JsonResult Filter(SearchCatalogModel categoryModel)
        {
            if (categoryModel.Page != null && categoryModel.Page < 1)
                return Json(null);

            var result = new GetSearchFilterHandler(categoryModel).Execute();
            return Json(result);
        }

        public JsonResult FilterProductCount(SearchCatalogModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Q) || (model.Page != null && model.Page < 1))
                return Json(null);

            var viewModel = new SearchPagingHandler(model, false).GetForFilterProductCount();

            return Json(viewModel.TotalItemsCount);
        }

        [HttpGet]
        public JsonResult Autocomplete(string q)
        {
            if (string.IsNullOrWhiteSpace(q))
                return Json(null);

            var handler = new SearchAutocompleteHandler(q, WarehouseContext.GetAvailableWarehouseIds());

            var categories =
                handler.GetCategories()
                    .Select(x => new
                    {
                        Name = x.Name,
                        Photo = x.Icon != null ? x.Icon.IconSrc() : "",
                        Url = Url.AbsoluteRouteUrl("Category", new {url = x.UrlPath}),
                        Template = "scripts\\_common\\autocompleter\\templates\\categories.html"
                    })
                    .ToList();
            
            var productsModel = handler.GetProducts();
            var products =
                productsModel.Products
                    .Select(x => new
                    {
                        Name = x.Name,
                        Photo = x.Photo.ImageSrcXSmall(),
                        Photo2x = x.Photo.ImageSrcSmall(),
                        Photo3x = x.Photo.ImageSrcMiddle(),
                        Amount = x.Amount,
                        Price = productsModel.HidePrice ? SettingsCatalog.TextInsteadOfPrice : x.PreparedPrice,
                        Rating = x.ManualRatio ?? x.Ratio,
                        Url = Url.AbsoluteRouteUrl("Product", new
                        {
                            url = x.UrlPath,
                            color = x.SelectedColorId,
                            size = x.SelectedSizeId
                        }),
                        Template = "scripts\\_common\\autocompleter\\templates\\products.html",
                        Gifts = x.Gifts
                    })
                    .ToList();

            return Json(new
            {
                Categories = new
                {
                    Title = T("Search.Autocomplete.Categories"),
                    Items = categories
                },
                Products = new
                {
                    Title = T("Search.Autocomplete.Products"),
                    Items = products
                },
                Empty = !products.Any() && !categories.Any()
            });
        }

        [ChildActionOnly]
        public ActionResult SearchBlock(SearchBlockModel search)
        {
            search = search ?? new SearchBlockModel();                        

            if (!string.IsNullOrEmpty(SettingsCatalog.SearchExample))
            {
                var examples = SettingsCatalog.SearchExample.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                if (examples.Length > 0)
                {
                    var example = examples[(new Random()).Next(0, examples.Length)];
                    example = example.TrimEnd('\r').Trim();
                    search.ExampleText = example;
                    search.ExampleLink = Url.AbsoluteRouteUrl("Search", new { q = example });
                }
            }

            search.Q = ClearSearch(search.Q);
            return PartialView("SearchBlock", search);
        }

        private static string ClearSearch(string q) =>
            WebUtility.UrlDecode(q)
                ?.Trim('{').Trim('}')
                .TrimStart('$');
    }
}