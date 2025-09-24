using System.Collections.Generic;
using AdvantShop.Catalog;
using AdvantShop.Web.Admin.Models.Catalog.Categories;

namespace AdvantShop.Web.Admin.Handlers.Catalog.Categories
{
    public class GetSelectedCategoriesTree
    {
        private readonly List<CategoriesSelectedModel> _categoriesSelected;

        public GetSelectedCategoriesTree(List<CategoriesSelectedModel> categoriesSelected)
        {
            _categoriesSelected = categoriesSelected;
        }

        public List<int> Execute()
        {
            var selectedIds = new List<int>();

            foreach (var categoryItemSelected in _categoriesSelected)
            {
                var category = CategoryService.GetCategory(categoryItemSelected.CategoryId);
                if (category != null)
                {
                    if (!selectedIds.Contains(category.CategoryId))
                        selectedIds.Add(category.CategoryId);

                    //категория не раскрыта, значит берем всех потомков
                    if (categoryItemSelected.Opened == false)
                    {
                        var subCategories = CategoryService.GetChildCategoriesByCategoryId(category.CategoryId, false);

                        if (subCategories != null && subCategories.Count > 0)
                            GetCategoryWithSubCategoriesIds(subCategories, selectedIds);
                    }
                }
            }

            return selectedIds;
        }

        private void GetCategoryWithSubCategoriesIds(List<Category> categories, List<int> result)
        {
            if (categories == null)
                return;

            foreach (var category in categories)
            {
                if (!result.Contains(category.CategoryId))
                    result.Add(category.CategoryId);

                if (category.HasChild)
                    GetCategoryWithSubCategoriesIds(CategoryService.GetChildCategoriesByCategoryId(category.CategoryId, false), result);
            }
        }
    }
}
