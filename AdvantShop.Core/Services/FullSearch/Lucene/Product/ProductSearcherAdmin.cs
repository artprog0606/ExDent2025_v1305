using System.Collections.Generic;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.FullSearch.Core;

namespace AdvantShop.Core.Services.FullSearch
{
    public class ProductSearcherAdmin : BaseSearcher<ProductDocument>
    {
        public ProductSearcherAdmin(int hitsLimit)
            : base(hitsLimit, new List<ESearchDeep> { SettingsCatalog.SearchDeepInAdminPart })
        {
        }

        public static SearchResult Search(string searchTerm, int limit = 10000, string field = "")
        {
            using (var searcher = new ProductSearcherAdmin(limit))
            {
                var res = searcher.SearchItems(searchTerm, field);
                return res;
            }
        }

        protected override List<string> GetIgnoredFields()
        {
            return new List<string>() { "Tags", "Desc", "MainCategoryName", "ParentCategoryName" };
        }
    }
}
