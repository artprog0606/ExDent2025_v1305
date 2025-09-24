using AdvantShop.Configuration;
using AdvantShop.Core.Services.FullSearch.Core;
using System.Collections.Generic;

namespace AdvantShop.Core.Services.FullSearch
{
    public class CategorySearcherAdmin : BaseSearcher<CategoryDocument>
    {
        public CategorySearcherAdmin(int hitsLimit) : base(hitsLimit, SettingsCatalog.SearchDeepInAdminPart)
        {
        }

        public static SearchResult Search(string searchTerm, int limit = 10000, string field = "")
        {
            using (var searcher = new CategorySearcherAdmin(limit))
            {
                var res = searcher.SearchItems(searchTerm, field);
                return res;
            }
        }

        protected override List<string> GetIgnoredFields()
        {
            return new List<string>() { "Tags", "Desc" };
        }
    }
}
