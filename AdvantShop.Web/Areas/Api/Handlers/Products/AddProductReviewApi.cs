using AdvantShop.Areas.Api.Models.Products;
using AdvantShop.Catalog;
using AdvantShop.CMS;
using AdvantShop.Core;
using AdvantShop.Handlers.Reviews;
using AdvantShop.Models.Review;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Products
{
    public class AddProductReviewApi : AbstractCommandHandler<AddProductReviewResponse>
    {
        private readonly int _id;
        private readonly AddReviewModel _review;

        public AddProductReviewApi(int id, AddReviewModel review)
        {
            _id = id;
            _review = review;
        }

        protected override void Validate()
        {
            if (!ProductService.IsExists( _id))
                throw new BlException("Товар не найден");
            
            if (_review.Rating != null && (_review.Rating <= 0 || _review.Rating > 5))
                throw new BlException("Рейтинг должен быть от 1 до 5");
        }

        protected override AddProductReviewResponse Handle()
        {
            var result = new AddReview(new ReviewModel()
            {
                EntityId = _id,
                EntityType = (int) EntityType.Product,
                ParentId = _review.ParentId,
                Text = _review.Text,
                Name = _review.Name,
                Email = _review.Email,
                Agreement = _review.Agreement,
                File = _review.Files,
                Rating = _review.Rating ?? 0
            }).Execute();

            if (result == null || result.Review == null)
                throw new BlException("Ошибка при добавлении");

            if (_review.Rating != null)
                RatingService.Vote(_id, _review.Rating.Value);

            return new AddProductReviewResponse() {Id = result.Review.ReviewId};
        }
    }
}