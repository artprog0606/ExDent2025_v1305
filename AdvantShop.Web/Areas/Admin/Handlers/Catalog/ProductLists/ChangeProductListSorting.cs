using System.Linq;
using AdvantShop.Catalog;

namespace AdvantShop.Web.Admin.Handlers.Catalog.ProductLists
{
    public class ChangeProductListSorting
    {
        private readonly int _id;
        private readonly int? _prevId;
        private readonly int? _nextId;

        public ChangeProductListSorting(int id, int? prevId, int? nextId)
        {
            _id = id;
            _prevId = prevId;
            _nextId = nextId;
        }

        public bool Execute()
        {
            var list = ProductListService.Get(_id);
            if (list == null)
                return false;

            ProductList prevList = null, nextList = null;

            if (_prevId != null)
                prevList = ProductListService.Get(_prevId.Value);

            if (_nextId != null)
                nextList = ProductListService.Get(_nextId.Value);

            if (prevList == null && nextList == null)
                return false;

            if (prevList != null && nextList != null)
            {
                if (nextList.SortOrder - prevList.SortOrder > 1)
                {
                    list.SortOrder = prevList.SortOrder + 1;
                    ProductListService.UpdateSortOrder(list.Id, list.SortOrder);
                }
                else
                {
                    UpdateSortOrderForAllGroups(list, prevList, nextList);
                }
            }
            else
            {
                UpdateSortOrderForAllGroups(list, prevList, nextList);
            }

            return true;
        }

        private void UpdateSortOrderForAllGroups(ProductList list, ProductList prevList, ProductList nextList)
        {
            var lists =
                ProductListService.GetList()
                    .Where(x => x.Id != list.Id)
                    .OrderBy(x => x.SortOrder)
                    .ToList();

            if (prevList != null)
            {
                var index = lists.FindIndex(x => x.Id == prevList.Id);
                lists.Insert(index + 1, list);
            }
            else if (nextList != null)
            {
                var index = lists.FindIndex(x => x.Id == nextList.Id);
                lists.Insert(index > 0 ? index - 1 : 0, list);
            }

            for (int i = 0; i < lists.Count; i++)
            {
                lists[i].SortOrder = i * 10 + 10;
                ProductListService.UpdateSortOrder(lists[i].Id, lists[i].SortOrder);
            }
        }
    }
}
