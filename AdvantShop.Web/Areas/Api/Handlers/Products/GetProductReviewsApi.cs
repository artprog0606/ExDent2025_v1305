using System.Linq;
using AdvantShop.Areas.Api.Models.Products;
using AdvantShop.Catalog;
using AdvantShop.CMS;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Products
{
    public class GetProductReviewsApi : AbstractCommandHandler<GetProductReviewsResponse>
    {
        private readonly int _id;
        
        public GetProductReviewsApi(int id)
        {
            _id = id;
        }

        protected override void Validate()
        {
            if (!ProductService.IsExists(_id))
                throw new BlException("Товар не найден");
        }

        protected override GetProductReviewsResponse Handle()
        {
            var reviews = ReviewService.GetReviews(_id, EntityType.Product)
                .Where(x => x.Checked || !SettingsCatalog.ModerateReviews)
                .Select(x => new ProductReviewApi(x))
                .ToList();
            return new GetProductReviewsResponse(reviews);
        }
    }
}