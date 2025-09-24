using System.Linq;
using AdvantShop.Areas.Api.Models.Catalogs;
using AdvantShop.Areas.Api.Models.Categories;
using AdvantShop.Areas.Api.Models.Search;
using AdvantShop.Areas.Api.Services;
using AdvantShop.Core;
using AdvantShop.Core.Services.Api;
using AdvantShop.Handlers.Search;
using AdvantShop.Models.Catalog;
using AdvantShop.Orders;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Search
{
    public class GetSearchApi : AbstractCommandHandler<SearchResponse>
    {
        private readonly SearchFilter _filter;

        public GetSearchApi(SearchFilter filter)
        {
            _filter = filter;
        }
        
        protected override void Validate()
        {
            if (_filter == null || string.IsNullOrWhiteSpace(_filter.Query))
                throw new BlException("Укажите поисковый запрос");
        }

        protected override SearchResponse Handle()
        {
            var response = new SearchResponse()
            {
                Pager = new ApiPagination()
            };
            
            var searchFilter = (SearchCatalogModel) _filter;

            var model = new SearchPagingHandler(searchFilter, false).Get();
            if (model != null)
            {
                if (model.Products?.Products != null)
                {
                    var wishList = ShoppingCartService.CurrentWishlist;
                        
                    response.Products = model.Products.Products
                        .Select(x => new CatalogProductItem(x, _filter.ShowHtmlPrice ?? false, wishList))
                        .ToList();
                }

                if (model.Pager != null)
                {
                    response.Pager = new ApiPagination()
                    {
                        TotalCount = model.Pager.TotalItemsCount,
                        TotalPageCount = model.Pager.TotalPages,
                        CurrentPage = model.Pager.CurrentPage,
                        Count = response.Products?.Count ?? 0
                    };
                }

                if (_filter.CategoryId == null && 
                    model.Categories?.Categories != null && model.Categories.Categories.Count > 0)
                {
                    response.Categories = model.Categories.Categories.Select(x => new GetCategoryResponse(x)).ToList();
                }
            }
            
            if (response.Products != null && response.Products.Count > 0)
            {
                new CatalogApiService().SetMarkers(response.Products);
            }

            return response;
        }
    }
}