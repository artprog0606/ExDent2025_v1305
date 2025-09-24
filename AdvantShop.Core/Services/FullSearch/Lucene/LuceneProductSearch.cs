//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using AdvantShop.Core.Services.FullSearch;
using AdvantShop.Core.Services.Spellcheck;
using AdvantShop.Helpers;
using System.Linq;
using AdvantShop.Configuration;

namespace AdvantShop.FullSearch
{
    public class LuceneProductSearch : IProductSearchProvaider
    {
        private ISpellService _spellService;

        public LuceneProductSearch()
        {
            _spellService = new SpellService();
        }

        public SearchResult Find(string searchTerm)
        {
            var result = _find(searchTerm);
            var fixTerm = _spellService.Check(searchTerm);
            if (fixTerm != searchTerm)
            {
                var resultFixTerm = _find(fixTerm);
                result.SearchResultItems = result.SearchResultItems.Union(resultFixTerm.SearchResultItems)
                                                .GroupBy(x => new { x.Id })
                                                .Select(x => x.First())
                                                .OrderByDescending(x => x.Score)
                                                .ToList();
            }
            return result;
        }

        private SearchResult _find(string searchTerm)
        {
            var productIds = ProductSearcher.Search(searchTerm, minimizeSearchResults:SettingsCatalog.MinimizeSearchResults);

            var translitQ = StringHelper.TranslitToRusKeyboard(searchTerm);

            if (searchTerm != translitQ)
            {
                var translitProductIds = ProductSearcher.Search(translitQ, minimizeSearchResults: SettingsCatalog.MinimizeSearchResults);

                productIds.SearchResultItems = productIds.SearchResultItems
                                              .Union(translitProductIds.SearchResultItems)
                                              .GroupBy(x => new { x.Id })
                                              .Select(x => x.First())
                                              .ToList();              
            }
            return productIds;
        }

        public bool IsActive()
        {
            return true;
        }
    }
}