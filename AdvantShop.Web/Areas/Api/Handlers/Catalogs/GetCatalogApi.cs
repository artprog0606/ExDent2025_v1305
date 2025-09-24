using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Areas.Api.Models.Catalogs;
using AdvantShop.Areas.Api.Models.Categories;
using AdvantShop.Areas.Api.Services;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Api;
using AdvantShop.Core.Services.Catalog.Warehouses;
using AdvantShop.Core.SQL;
using AdvantShop.Web.Infrastructure.Handlers;
using AdvantShop.Handlers.Catalog;
using AdvantShop.Orders;
using CategoryModel = AdvantShop.Models.Catalog.CategoryModel;

namespace AdvantShop.Areas.Api.Handlers.Catalogs
{
    public class GetCatalogApi : AbstractCommandHandler<CatalogResponse>
    {
        private readonly CatalogFilter _filter;
        private Category _category;
    
        public GetCatalogApi(CatalogFilter filter)
        {
            _filter = filter;
        }

        protected override void Load()
        {
            _category = _filter.CategoryId != null && _filter.CategoryId.Value >= 0
                ? CategoryService.GetCategory(_filter.CategoryId.Value)
                : !string.IsNullOrWhiteSpace(_filter.Url) ? CategoryService.GetCategory(_filter.Url) : null;
        }

        protected override void Validate()
        {
            if (_category == null)
                throw new BlException("Категория не найдена");
        }
    
        protected override CatalogResponse Handle()
        {
            var warehouseIds = WarehouseContext.GetAvailableWarehouseIds();
            
            var response = new CatalogResponse()
            {
                Category = new GetCategoryResponse(_category),
                SubCategories =
                    CategoryService.GetChildCategoriesByCategoryId(_category.CategoryId, warehouseIds: warehouseIds)
                        .Where(c => c.Enabled && c.ParentsEnabled && !c.Hidden)
                        .Select(x => new SubCategoryItem(x))
                        .ToList(),
                Pager = new ApiPagination(),
                Products = new List<CatalogProductItem>()
            };

            if ((_filter.WarehouseIds != null && _filter.WarehouseIds.Count > 0) ||
                (warehouseIds != null && warehouseIds.Count > 0))
            {
                foreach (var subCategory in response.SubCategories)
                {
                    subCategory.ProductsCount =
                        GetCountByCategoryAndWarehouses(subCategory.Id, _filter.WarehouseIds, warehouseIds);

                    subCategory.ProductsCountWithSubCategories =
                        GetCountByCategoryAndWarehouses(subCategory.Id, _filter.WarehouseIds, warehouseIds, true);
                }
            }

            if (_category.CategoryId == 0) 
                return response;
            
            var categoryModel = (CategoryModel) _filter;

            var paging = new CategoryProductPagingHandler(_category, false, categoryModel, false).GetForCatalog();
            if (paging != null)
            {
                if (paging.Products?.Products != null)
                {
                    var wishList = ShoppingCartService.CurrentWishlist;
                    
                    response.Products = paging.Products.Products
                        .Select(x => new CatalogProductItem(x, _filter.ShowHtmlPrice ?? false, wishList))
                        .ToList();
                }

                if (paging.Pager != null)
                {
                    response.Pager = new ApiPagination()
                    {
                        TotalCount = paging.Pager.TotalItemsCount,
                        TotalPageCount = paging.Pager.TotalPages,
                        CurrentPage = paging.Pager.CurrentPage,
                        Count = response.Products?.Count ?? 0
                    };
                }
            }

            if (response.Products != null && response.Products.Count > 0)
            {
                new CatalogApiService().SetMarkers(response.Products);
            }

            return response;
        }

        private int GetCountByCategoryAndWarehouses(int categoryId, List<int> filterWarehouseIds, List<int> currentWarehouseIds, bool inDepth = false)
        {
            var sqlParams = new List<SqlParameter>() { new SqlParameter("@CategoryId", categoryId) };

            var filterWarehouseParams =
                filterWarehouseIds != null && filterWarehouseIds.Count > 0
                    ? filterWarehouseIds.Select(x => new SqlParameter("@fw" + x, x)).ToList()
                    : null;
            if (filterWarehouseParams != null)
                sqlParams.AddRange(filterWarehouseParams);
            
            var currentWarehouseParams =
                currentWarehouseIds != null && currentWarehouseIds.Count > 0
                    ? currentWarehouseIds.Select(x => new SqlParameter("@cw" + x, x)).ToList()
                    : null;
            if (currentWarehouseParams != null)
                sqlParams.AddRange(currentWarehouseParams);
            
            
            var query = 
                "Select Count(Product.ProductId) " +
                "From Catalog.Product " +
                (
                    !inDepth 
                        ? "Inner Join [Catalog].[ProductCategories] ON [ProductCategories].[CategoryID] = @CategoryId and  ProductCategories.ProductId = Product.ProductId "
                        : ""
                ) +
                "Where Enabled = 1" +
                " AND CategoryEnabled = 1" +
                " AND Hidden = 0" +
                (
                    inDepth 
                        ? " AND Exists(" +
                          "Select 1 from Catalog.ProductCategories " +
                          "Inner Join [Settings].[GetChildCategoryByParent](@CategoryId) AS hCat ON hCat.id = ProductCategories.CategoryID and ProductCategories.ProductId = Product.ProductId)" 
                        : "" 
                ) +
                (
                    currentWarehouseParams != null
                        ? " and (Product.AllowPreOrder = 1 " +
                                "OR Exists(" +
                                    "Select 1 from [Catalog].[Offer] " + 
                                    "Inner Join [Catalog].[WarehouseStocks] on [Offer].[OfferID] = [WarehouseStocks].[OfferId] " + 
                                    "Where [WarehouseStocks].[WarehouseId] in (" + string.Join(",", currentWarehouseParams.Select(x => x.ParameterName)) + ") " + 
                                    "     AND Offer.ProductId = Product.ProductID " + 
                                    "     AND [WarehouseStocks].[Quantity] > 0)) "
                        : ""
                ) +
                (
                    filterWarehouseParams != null 
                        ? " AND Exists(Select 1 from [Catalog].[Offer] " +
                                      "Inner Join [Catalog].[WarehouseStocks] ON [Offer].[OfferID] = [WarehouseStocks].[OfferId] " +
                                      "Where [WarehouseStocks].[WarehouseId] IN (" + string.Join(",", filterWarehouseParams.Select(x => x.ParameterName)) + ") " +
                                      "     and Offer.ProductId = [Product].[ProductID] " +
                                      "     AND [WarehouseStocks].[Quantity] > 0)"
                        : ""
                );
            
            var cacheName = CacheNames.SQlPagingCountCacheName("GetCountByCategoryAndWarehouses", query, sqlParams.Select(p => p.ParameterName + p.Value).AggregateString(""));
                    
            var countByWarehouse = CacheManager.Get(cacheName, () => 
                SQLDataAccess.ExecuteScalar<int>(query, CommandType.Text, sqlParams.ToArray()));

            return countByWarehouse;
        }
    }
}