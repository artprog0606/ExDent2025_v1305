using System.Collections.Generic;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.FullSearch;
using AdvantShop.Helpers;

namespace AdvantShop.Handlers.Search
{
    public class SearchAutocompleteHandler
    {
        private readonly string _q;
        private readonly List<int> _warehouseIds;
        private readonly ISearchService _searchService;

        public SearchAutocompleteHandler(string q)
        {
            _q = q;
            _searchService = new SearchService();
        }

        public SearchAutocompleteHandler(string q, List<int> warehouseIds) : this(q)
        {
            _warehouseIds = warehouseIds;
        }

        public List<Category> GetCategories()
        {
            if (!SettingsCatalog.SearchByCategories)
                return new List<Category>();

            var categoryIds = CategorySearcher.Search(_q).SearchResultItems.Select(x => x.Id).ToList();
            
            var translitQ = StringHelper.TranslitToRusKeyboard(_q);
            var translitCategoryIds = CategorySearcher.Search(translitQ).SearchResultItems.Select(x => x.Id).ToList();

            var categories =
                categoryIds.Union(translitCategoryIds)
                    .Distinct()
                    .Select(CategoryService.GetCategory)
                    .Where(x =>
                        x != null
                        && x.Enabled
                        && x.ParentsEnabled 
                        && !x.Hidden
                        && (_warehouseIds == null ||
                            _warehouseIds.Any(warehouseId => CategoryService.GetCategoryWarehouseHasProducts(x.CategoryId, warehouseId)))
                        && (_warehouseIds == null && SettingsCatalog.ShowOnlyAvalible ? x.Available_Products_Count > 0 : true)
                        )
                    .Take(5)
                    .ToList();
            
            return categories;
        }

        public ProductViewModel GetProducts()
        {
            var productIds =
                _searchService.Find(_q)
                    .SearchResultItems
                    .Select(x => x.Id)
                    .Take(100)
                    .Aggregate("", (current, item) => current + (item + "/"));

            var products = ProductService.GetForAutoCompleteProducts(10, productIds, _warehouseIds);

            var offer = OfferService.GetOffer(_q);
            if (offer != null)
            {
                var p = products.Find(x => x.ProductId == offer.ProductId);
                if (p != null)
                {
                    p.BasePrice = offer.BasePrice;
                    p.Amount = offer.Amount;
                    p.SelectedSizeId = offer.SizeID;
                    p.SelectedColorId = offer.ColorID;
                    
                    if (offer.ColorID.HasValue && offer.Photo != null && offer.Photo.PhotoName.IsNotEmpty())
                        p.Photo = offer.Photo;
                }
            }

            return new ProductViewModel(products);
        }
    }
}