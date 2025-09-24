using System.Collections.Generic;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.ExportImport;
using AdvantShop.Web.Admin.Models.Catalog;
using AdvantShop.Web.Admin.ViewModels.Catalog;

namespace AdvantShop.Web.Admin.Handlers.Catalog.ExportFeeds
{
    public class GetCategoriesTree
    {
        private readonly string _id;
        private readonly List<ExportFeedSelectedCategory> _selectedCategories;
        private readonly List<int> _excludeIds;
        private readonly bool _showRoot;
        private readonly CategoriesTreeState _state;
        private readonly CategoriesTreeState _stateOriginal;

        public GetCategoriesTree(CategoriesTree model, int exportFeedId)
        {
            _id = model.Id;
            _selectedCategories = ExportFeedService.GetExportFeedCategoriesId(exportFeedId);
            _excludeIds = model.ExcludeIds != null
                ? model.ExcludeIds.Split(',').Select(x => x.TryParseInt()).ToList()
                : new List<int>();

            _showRoot = model.ShowRoot;
            _state = model.State;
            _stateOriginal = model.StateOriginal;
        }

        public List<AdminCatalogTreeViewItem> Execute()
        {
            if (_showRoot)
            {
                var category = CategoryService.GetCategory(0);
                if (category != null)
                    return new List<AdminCatalogTreeViewItem>
                    {
                        new AdminCatalogTreeViewItem
                        {
                            id = "0",
                            parent = "#",
                            text = $"<span class=\"jstree-advantshop-name\">{category.Name}</span>",
                            name = category.Name,
                            children = true,
                            state = new AdminCatalogTreeViewItemState
                            {
                                opened = true,
                                selected = _selectedCategories.Any(item => item.CategoryId == 0)
                            }
                        }
                    };
            }

            var selectedCategoryIds = _selectedCategories.Select(x => x.CategoryId).ToList();
            var childCategories = CategoryService.GetChildCategoriesByCategoryId(_id.TryParseInt(), false);
            var categoryOpen = new List<int> { 0 };

            //Если ничего не выбрано - нет смысла тянуть данные
            if (selectedCategoryIds.Count > 0)
            {
                foreach (var childCategory in childCategories)
                {
                    //Если итерируемая дочерняя категория - выбрана и открыта, открываем ее
                    if (_selectedCategories.Any(item => item.CategoryId == childCategory.CategoryId && item.Opened))
                    {
                        categoryOpen.Add(childCategory.CategoryId);
                        continue;
                    }

                    //подтягиваем всех детей включая итерируемую дочернюю категорию
                    var childIdsHierarchical =
                        CategoryService.GetChildIDsHierarchical(childCategory.CategoryId).ToList();
                    //удаляем итерируемую дочернию категорию из списка
                    childIdsHierarchical.Remove(childCategory.CategoryId);

                    //Если в списке дочерних есть выбранная - раскрываем итерируемую дочернюю категорию
                    if (childIdsHierarchical.Any(id => selectedCategoryIds.Contains(id)))
                        categoryOpen.Add(childCategory.CategoryId);
                }
            }

            var result =
                childCategories
                    .Where(x => !_excludeIds.Contains(x.CategoryId))
                    .Select(x => new AdminCatalogTreeViewItem
                    {
                        id = x.CategoryId.ToString(),
                        parent = _id == "#" && x.ParentCategoryId == 0
                            ? "#"
                            : x.ParentCategoryId.ToString(),
                        text =
                            $"<span class=\"jstree-advantshop-name\">{x.Name}</span> <span class=\"jstree-advantshop-count\">{x.ProductsCount}/{x.TotalProductsCount}</span>",
                        name = x.Name,
                        children = x.HasChild,
                        state = new AdminCatalogTreeViewItemState
                        {
                            opened = categoryOpen.Contains(x.CategoryId),
                            selected = _selectedCategories.Any(item => item.CategoryId == x.CategoryId) 
                                       || _stateOriginal != null && !_stateOriginal.Opened && _state.Selected
                        },
                        li_attr = new Dictionary<string, string>
                        {
                            {
                                "class", !x.HasChild
                                    ? "jstree-leaf"
                                    : ""
                            }
                        }
                    }).ToList();

            return result;
        }
    }
}
