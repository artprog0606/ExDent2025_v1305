using System.Web.Mvc;
using System.Web.SessionState;
using AdvantShop.Areas.Mobile.Handlers.Sidebar;

namespace AdvantShop.Areas.Mobile.Controllers
{
    [SessionState(SessionStateBehavior.Disabled)]
    public class SidebarController : BaseMobileController
    {
        [ChildActionOnly]
        public ActionResult Sidebar()
        {
            var model = new SidebarHandler().Get();

            return PartialView(model);
        }
    }
}