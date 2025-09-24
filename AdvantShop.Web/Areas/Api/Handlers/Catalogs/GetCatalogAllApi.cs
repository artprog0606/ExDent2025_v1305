using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Areas.Api.Models.Catalogs;
using AdvantShop.Areas.Api.Services;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Core.Services.Api;
using AdvantShop.Core.Services.Catalog.Warehouses;
using AdvantShop.Web.Infrastructure.Handlers;
using AdvantShop.Handlers.Catalog;
using AdvantShop.Orders;
using CategoryModel = AdvantShop.Models.Catalog.CategoryModel;

namespace AdvantShop.Areas.Api.Handlers.Catalogs
{
    public class GetCatalogAllApi : AbstractCommandHandler<CatalogAllResponse>
    {
        private readonly CatalogAllFilter _filter;
        private readonly CatalogApiService _catalogApiService; 
    
        public GetCatalogAllApi(CatalogAllFilter filter)
        {
            _filter = filter;
            _catalogApiService = new CatalogApiService();
        }
    
        protected override CatalogAllResponse Handle()
        {
            var response = new CatalogAllResponse()
            {
                Categories =
                    CategoryService.GetChildCategoriesByCategoryId(
                            0, 
                            warehouseIds: WarehouseContext.GetAvailableWarehouseIds())
                        .Where(c => c.Enabled && c.ParentsEnabled && !c.Hidden)
                        .Select(x => new CategoryItem(x))
                        .ToList()
            };

            foreach (var categoryItem in response.Categories)
            {
                var category = CategoryService.GetCategory(categoryItem.Id);
                var categoryModel = new CategoryModel()
                {
                    CategoryId = categoryItem.Id,
                    Page = 1,
                    Sort = _filter.Sorting,
                    SearchQuery = _filter.Search,
                    Warehouse = _filter.WarehouseIds != null ? string.Join(",", _filter.WarehouseIds) : null
                };
                var productsPerPage = _filter.ProductsLimit != null && _filter.ProductsLimit.Value > 0
                    ? _filter.ProductsLimit.Value
                    : SettingsCatalog.ProductsPerPage;
                
                var paging = new CategoryProductPagingHandler(category, false, categoryModel, false, productsPerPage).GetForCatalog();
                if (paging != null)
                {
                    if (paging.Products?.Products != null)
                    {
                        var wishList = ShoppingCartService.CurrentWishlist;
                        
                        categoryItem.Products = paging.Products.Products
                            .Select(x => new CatalogProductItem(x, _filter.ShowHtmlPrice ?? false, wishList))
                            .ToList();

                        _catalogApiService.SetMarkers(categoryItem.Products);
                    }

                    if (paging.Pager != null)
                    {
                        categoryItem.ProductsTotalCount = paging.Pager.TotalItemsCount;
                        categoryItem.ProductsTotalPageCount = paging.Pager.TotalPages;
                    }
                }
            }

            return response;
        }
    }
}