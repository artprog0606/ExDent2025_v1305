using System.Collections.Generic;
using System.Linq;
using AdvantShop.Catalog;

namespace AdvantShop.Web.Admin.Handlers.Catalog
{
    public class GetCategoriesTreeBySearchRequest
    {
        private readonly string _str;

        public GetCategoriesTreeBySearchRequest(string str)
        {
            _str = str;
        }

        public List<int> Execute()
        {
            var list = new List<int>();

            var parentIds = CategoryService.GetParentCategoriesIdsByChildName(_str);

            foreach (var id in parentIds)
            {
                var ids = CategoryService.GetParentCategoryIds(id);

                list.AddRange(ids);
            }

            return list.Distinct().ToList();
        }
    }
}
