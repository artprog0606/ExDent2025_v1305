using System.Linq;
using AdvantShop.Areas.Api.Models.Catalogs;
using AdvantShop.Areas.Api.Models.Categories;
using AdvantShop.Areas.Api.Models.Search;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.Services.Catalog.Warehouses;
using AdvantShop.Web.Infrastructure.Handlers;
using AdvantShop.Handlers.Search;
using AdvantShop.Orders;

namespace AdvantShop.Areas.Api.Handlers.Search
{
    public sealed class GetSearchAutocompleteApi : AbstractCommandHandler<SearchAutocompleteResponse>
    {
        private readonly SearchAutocomplete _model;
    
        public GetSearchAutocompleteApi(SearchAutocomplete model)
        {
            _model = model;
        }

        protected override void Validate()
        {
            if (_model == null || string.IsNullOrWhiteSpace(_model.Query))
                throw new BlException("Укажите поисковый запрос");
        }

        protected override SearchAutocompleteResponse Handle()
        {
            var handler = new SearchAutocompleteHandler(_model.Query, WarehouseContext.GetAvailableWarehouseIds());

            var products =
                handler.GetProducts().Products
                    .Select(x => new CatalogProductItem(x, false, ShoppingCartService.CurrentWishlist))
                    .ToList();

            var categories =
                handler.GetCategories()
                    .Select(x => new GetCategoryResponse(x))
                    .ToList();

            return new SearchAutocompleteResponse(products, categories);
        }
    }
}