using AdvantShop.Catalog;
using AdvantShop.Core.Modules;
using AdvantShop.Core.SQL2;
using AdvantShop.FullSearch;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdvantShop.Core.Services.FullSearch
{
    public interface ISearchService
    {
        SearchResult Find(string term);

        List<int> FindForPaging(string term, SqlPaging paging, ESortOrder eSort);
    }

    public class SearchService : ISearchService
    {
        public SearchResult Find(string term)
        {
            var type = AttachedModules.GetModules<IModuleProductSearchProvaider>().FirstOrDefault();
            if (type != null)
            {
                var instance = (IModuleProductSearchProvaider)Activator.CreateInstance(type, null);
                return instance.Find(term);
            }

            var defaultSearch = new LuceneProductSearch();
            return defaultSearch.Find(term);
        }

        public List<int> FindForPaging(string term, SqlPaging paging, ESortOrder eSort)
        {
            var resultSearch = Find(term);
            if (resultSearch.SearchResultItems != null)
            {
                var resultIds = resultSearch.SearchResultItems.Select(x => x.Id).ToList();

                paging.Inner_Join(
                    "(select item, sort from [Settings].[ParsingBySeperator]({0},'/') ) as dtt on Product.ProductId=convert(int, dtt.item)",
                    String.Join("/", resultIds));

                if (eSort == ESortOrder.NoSorting)
                {
                    paging.OrderBy("dtt.sort");
                }

                return resultIds;
            }
            return null;
        }
    }
}
