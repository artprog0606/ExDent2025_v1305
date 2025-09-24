using AdvantShop.Catalog;
using AdvantShop.Customers;

namespace AdvantShop.Web.Admin.Handlers.Catalog.Categories
{
    public class SetActiveCategoryHandler
    {
        private readonly int _categoryId;
        private readonly bool _active;


        public SetActiveCategoryHandler(int categoryId, bool active)
        {
            _categoryId = categoryId;
            _active = active;
        }

        public bool Execute()
        {
            var category = CategoryService.GetCategory(_categoryId);

            if (category == null)
                return false;
            
            category.Enabled = _active;

            if (CustomerContext.CurrentCustomer != null)
                category.ModifiedBy = CustomerContext.CurrentCustomer.Id.ToString();

            CategoryService.UpdateCategory(category, true, trackChanges:true);

            return true;
        }
    }
}
