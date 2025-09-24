using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Core.Services.ChangeHistories;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;

namespace AdvantShop.Web.Admin.Handlers.Catalog.Categories
{
    public class AddCategoryList
    {
        private readonly int _categoryId;
        private readonly List<string> _categories;

        public AddCategoryList(int categoryId, List<string> categories)
        {
            _categoryId = categoryId;
            _categories = categories;
        }

        public void Execute()
        {
            var childs = CategoryService.GetChildCategoriesByCategoryId(_categoryId, false);
            var sortOrder = childs != null && childs.Count > 0 ? childs.Max(x => x.SortOrder) + 10 : 10;

            var parentCategory = CategoryService.GetCategory(_categoryId);

            foreach (var categoryName in _categories)
            {
                if (string.IsNullOrWhiteSpace(categoryName))
                    continue;
                
                var category = new Category
                {
                    Name = categoryName.Trim(),
                    UrlPath = UrlService.GetAvailableValidUrl(0, ParamType.Category, categoryName.Trim()),
                    ParentCategoryId = _categoryId,
                    Enabled = true,
                    DisplayStyle = ECategoryDisplayStyle.Tile,
                    Sorting = ESortOrder.NoSorting,
                    SortOrder = sortOrder,
                    ModifiedBy = CustomerContext.CurrentCustomer.Id.ToString(),
                    ParentsEnabled = _categoryId == 0 || parentCategory.Enabled && parentCategory.ParentsEnabled
                };

                try
                {
                    category.CategoryId = CategoryService.AddCategory(category, true, true, trackChanges: true);
                }
                catch (Exception ex)
                {
                    Debug.Log.Error(ex.Message + " at AddCategoryList", ex);
                }

                sortOrder += 10;
            }

            Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Categories_CategoriesListCreated);
        }
    }
}
