using AdvantShop.Handlers.Cart;
using AdvantShop.Web.Infrastructure.Filters;
using System.Web.Mvc;
using System.Web.SessionState;

namespace AdvantShop.Areas.Mobile.Controllers
{
    [SessionState(SessionStateBehavior.Disabled)]
    public class CartController : BaseMobileController
    {
        [HttpPost, ValidateJsonAntiForgeryToken]
        public ActionResult GetCart()
        {
            return PartialView("_SidebarCart", new GetCartHandler().Get());
        }
    }
}