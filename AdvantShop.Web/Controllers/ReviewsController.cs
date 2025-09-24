using System.Web.Mvc;
using System.Web.SessionState;
using AdvantShop.CMS;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Customers;
using AdvantShop.Handlers.Reviews;
using AdvantShop.Models.Review;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Controllers
{
    [SessionState(SessionStateBehavior.Disabled)]
    public partial class ReviewsController : BaseClientController
    {
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult Add(ReviewModel review)
        {
            try
            {
                return Json(new AddReview(review).Execute());
            }
            catch (BlException e)
            {
                ModelState.AddModelError(e.Property, e.Message);
                return JsonError();
            }
        }

        public JsonResult Delete(int reviewId)
        {
            var customer = CustomerContext.CurrentCustomer;
            if (customer.IsAdmin || (customer.IsModerator && customer.HasRoleAction(RoleAction.Catalog)))
            {
                if (reviewId == 0)
                    return Json(false);

                ReviewService.DeleteReview(reviewId);
                return Json(true);
            }

            return Json(false);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult LikeVote(int? reviewId, bool vote)
        {
            if (!reviewId.HasValue || ReviewService.GetReview(reviewId.Value) == null)
                return Json(new { error = true, errors = "Отзыв не найден." });
            if (SettingsCatalog.ReviewsVoiteOnlyRegisteredUsers && !CustomerContext.CurrentCustomer.RegistredUser)
                return Json(new { error = true, errors = "Голосовать могут только зарегистрированные пользователи." });

            ReviewService.AddVote(reviewId.Value, vote);

            var reviewItem = ReviewService.GetReview(reviewId.Value);

            return Json(new
            {
                error = false,
                likeData = new
                {
                    Likes = reviewItem.LikesCount,
                    Dislikes = reviewItem.DislikesCount,
                    RatioByLikes = reviewItem.RatioByLikes,
                }
            });
        }

    }
}