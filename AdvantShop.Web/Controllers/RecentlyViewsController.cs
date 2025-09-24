using System.Web.Mvc;
using System.Web.SessionState;
using AdvantShop.Handlers.Common;

namespace AdvantShop.Controllers
{
    [SessionState(SessionStateBehavior.Disabled)]
    public partial class RecentlyViewsController : BaseClientController
    {
        [ChildActionOnly]
        public ActionResult RecentlyViewed(int productAmount = 3)
        {
            var model = new RecentlyViewedHandler(productAmount).Get();
            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult RecentlyViewedBlock(int productAmount = 10)
        {
            var model = new RecentlyViewedHandler(productAmount).Get();

            if (model == null)
                return new EmptyResult();

            return PartialView(model);
        }
    }
}