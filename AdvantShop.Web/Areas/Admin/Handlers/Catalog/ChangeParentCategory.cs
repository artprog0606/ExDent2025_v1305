using AdvantShop.Catalog;
using AdvantShop.Customers;

namespace AdvantShop.Web.Admin.Handlers.Catalog
{
    public class ChangeParentCategory
    {
        private readonly int _categoryId;
        private readonly int _parentId;

        public ChangeParentCategory(int categoryId, int parentId)
        {
            _categoryId = categoryId;
            _parentId = parentId;
        }

        public bool Execute()
        {
            var category = CategoryService.GetCategory(_categoryId);
            var parentCategory = CategoryService.GetCategory(_parentId);

            if (category == null || parentCategory == null || 
                category.ParentCategoryId == _parentId || category.CategoryId == 0)
                return false;

            category.ParentCategoryId = _parentId;

            if (CustomerContext.CurrentCustomer != null)
                category.ModifiedBy = CustomerContext.CurrentCustomer.Id.ToString();

            CategoryService.UpdateCategory(category, true, trackChanges:true);

            return true;
        }
    }
}
