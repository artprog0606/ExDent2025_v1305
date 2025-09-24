using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AdvantShop.Catalog;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Web.Admin.Models.Catalog;
using AdvantShop.Web.Admin.ViewModels.Catalog;
using AdvantShop.Web.Infrastructure.Extensions;

namespace AdvantShop.Web.Admin.Handlers.Catalog
{
    public class GetCategoriesTree
    {
        private readonly string _id;
        private readonly int _categoryIdSelected;
        private readonly List<int> _excludeIds;
        private readonly List<int> _selectedIds;
        private readonly bool _showRoot;
        private readonly CategoriesTreeState _state;
        private readonly CategoriesTreeState _stateOriginal;
        private readonly CategoriesTreeCheckbox _checkbox;
        private List<int> allParentsIds = new List<int>();
        private UrlHelper _url;

        public GetCategoriesTree(CategoriesTree model)
        {
            _id = model.Id;
            _categoryIdSelected = model.CategoryIdSelected != null ? model.CategoryIdSelected.Value : -1;
            _excludeIds = model.ExcludeIds != null ? model.ExcludeIds.Split(',').Select(x => x.TryParseInt()).ToList() : new List<int>();
            _selectedIds = model.SelectedIds != null ? model.SelectedIds.Split(',').Select(x => x.TryParseInt()).ToList() : new List<int>();

            _showRoot = model.ShowRoot;
            _state = model.State;
            _stateOriginal = model.StateOriginal;
            _checkbox = model.Checkbox;

            _url = new UrlHelper(HttpContext.Current.Request.RequestContext);
        }

        public List<AdminCatalogTreeViewItem> Execute()
        {
            if (_showRoot)
            {
                var category = CategoryService.GetCategory(0);
                if (category != null)
                    return new List<AdminCatalogTreeViewItem>()
                    {
                        new AdminCatalogTreeViewItem()
                        {
                            id = "0",
                            parent = "#",
                            text = String.Format("<span class=\"jstree-advantshop-name\">{0}</span>", category.Name),
                            name = category.Name,
                            children = true,
                            state = new AdminCatalogTreeViewItemState()
                            {
                                opened = true,
                                selected = _categoryIdSelected == 0 || _selectedIds.Contains(0)
                                
                            },
                            li_attr = new Dictionary<string, string>() {
                                { "data-tree-id", "categoryItemId_" +  0}
                            },
                            a_attr =  new Dictionary<string, string>() {
                                { "href", "catalog"}
                            },
                        }
                    };
            }

            var categories = CategoryService.GetChildCategoriesByCategoryId(_id.TryParseInt(), false);
            int categoryOpen = 0;

            foreach (var item in _selectedIds)
            {
                allParentsIds.AddRange(CategoryService.GetParentCategories(item).Select(x => x.CategoryId));
            }


            if (categories.Any(x => x.CategoryId != _categoryIdSelected))
            {
                var category = CategoryService.GetCategory(_categoryIdSelected);
                if (category != null)
                {
                    int i = 20;
                    while (category.ParentCategoryId > 0 && i > 0)
                    {
                        if (categories.Any(x => x.CategoryId == category.ParentCategoryId))
                        {
                            categoryOpen = category.ParentCategoryId;
                            break;
                        }
                        category = CategoryService.GetCategory(category.ParentCategoryId);
                        i--;
                    }
                }
            }

            var result = categories.Where(x => !_excludeIds.Contains(x.CategoryId)).Select(x => new AdminCatalogTreeViewItem()
            {
                id = x.CategoryId.ToString(),
                parent = _id == "#" && x.ParentCategoryId == 0 ? "#" : x.ParentCategoryId.ToString(),
                text =
                    String.Format(
                        "<span class=\"jstree-advantshop-name\">{0}</span> <span class=\"jstree-advantshop-count\">" +
                        "<span uib-popover-html=\"'<span class=&quot;fs-xs&quot;>{3}: {1}<br />{4}: {2}</span><br/><span class=&quot;fs-min&quot;>{5}</span>' | sanitize\" popover-trigger=\"'mouseenter'\" popover-placement=\"auto right\" popover-append-to-body=\"true\">" +
                        "{1}/{2}</span></span>",
                        x.Name, x.ProductsCount, x.TotalProductsCount,
                        LocalizationService.GetResource("Admin.CategoriesTree.AvailableProductsCount"),
                        LocalizationService.GetResource("Admin.CategoriesTree.TotalProductsCount"),
                        x.HasChild ? LocalizationService.GetResource("Admin.CategoriesTree.ProductsCount.Subtext") : string.Empty),
                name = x.Name,
                children = x.HasChild,
                state = new AdminCatalogTreeViewItemState()
                {
                    opened = x.CategoryId == categoryOpen || allParentsIds.Contains(x.CategoryId),
                    selected = x.CategoryId == _categoryIdSelected || _selectedIds.Contains(x.CategoryId) || (_checkbox.Cascade == eTreeCascade.Down && _checkbox.TieSelection == false &&_stateOriginal != null && !_stateOriginal.Opened && _state.Selected),
                    enabled = x.Enabled
                },
                li_attr = new Dictionary<string, string>() {
                        { "data-tree-id", "categoryItemId_" +  x.CategoryId },
                        { "class", (x.Enabled ? "category-item-active": "category-item-inactive") + " " + (!x.HasChild ? "jstree-leaf": "") }
                },
                a_attr = new Dictionary<string, string>() {
                                { "href",_url.AbsoluteActionUrl("Index", "Catalog", new { categoryid = x.CategoryId})}
                            },
            }).ToList();

            return result;
        }
    }
}
