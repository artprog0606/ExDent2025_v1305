using AdvantShop.Catalog;
using AdvantShop.CMS;
using System.Linq;

namespace AdvantShop.Web.Admin.Handlers.Marketing.Analytics
{
    public class GetProductsDataHandler
    {
        public object Execute()
        {
            var products = ProductService.GetProductsCount();
            var categories = CategoryService.GetCategories();
            var brands = BrandService.GetBrandsCount();
            var reviews = ReviewService.GetReviewList();

            var model = new
            {
                products,                
                brands,
                categories = categories != null ? categories.Count() : 0,
                reviews = reviews != null ? reviews.Count() : 0
            };

            return model;
        }
    }
}
