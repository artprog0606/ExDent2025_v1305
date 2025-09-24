using AdvantShop.News;
using AdvantShop.Web.Admin.Models.Cms.News;

namespace AdvantShop.Web.Admin.Handlers.Cms.News
{
    public class DeleteNewsPicture
    {
        private readonly int _newsId;

        public DeleteNewsPicture(int? newsId)
        {
            _newsId = newsId != null ? newsId.Value : -1;
        }

        public UploadNewsPictureResult Execute()
        {
            NewsService.DeleteNewsImage(_newsId);
            return new UploadNewsPictureResult() { Result = true, Picture = "../images/nophoto_small.png" };
        }
    }
}
