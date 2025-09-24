using AdvantShop.Configuration;
using AdvantShop.Core.Services.FullSearch.Core;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;

namespace AdvantShop.Core.Services.FullSearch
{
    public class CategorySearcher : BaseSearcher<CategoryDocument>
    {
        public CategorySearcher(int hitsLimit, ESearchDeep deepLimit, string path)
            : base(hitsLimit, deepLimit, path)
        {
        }
        
        public CategorySearcher(int hitsLimit, ESearchDeep deepLimit)
            : base(hitsLimit, deepLimit)
        {
        }

        protected override BooleanQuery ProcessCondition(BooleanQuery bq)
        {
            var conditionParser = new QueryParser(CurrentVersion, nameof(CategoryDocument.Enabled), _analyzer);
            var conditionQuery = ParseQuery(true.ToString(), conditionParser);
            bq.Add(conditionQuery, Occur.MUST);

            conditionParser = new QueryParser(CurrentVersion, nameof(CategoryDocument.Hidden), _analyzer);
            conditionQuery = ParseQuery(false.ToString(), conditionParser);
            bq.Add(conditionQuery, Occur.MUST);

            return bq;
        }

        public static SearchResult Search(string searchTerm, string field = "")
        {
            using (var searcher = new CategorySearcher(SettingsCatalog.SearchMaxItems, SettingsCatalog.SearchDeep))
            {
                return searcher.SearchItems(searchTerm, field);
            }
        }
    }
}
