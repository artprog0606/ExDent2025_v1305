using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.SessionState;
using AdvantShop.Areas.Mobile.Handlers.Footer;
using AdvantShop.CMS;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Handlers.Common;

namespace AdvantShop.Areas.Mobile.Controllers
{
    [SessionState(SessionStateBehavior.Disabled)]
    public class CommonController : BaseMobileController
    {
        [ChildActionOnly]
        public ActionResult BreadCrumbs(List<BreadCrumbs> breadCrumbs)
        {
            if (breadCrumbs == null || breadCrumbs.Count <= 2)
                return new EmptyResult();

            return PartialView(breadCrumbs);
        }
        
        [ChildActionOnly]
        public ActionResult BottomPanel()
        {
            if (!SettingsMobile.ShowBottomPanel)
                return new EmptyResult();

            return PartialView();
        }

        [ChildActionOnly]
        public ActionResult Footer()
        {
            var model = new FooterHandler().Get();
            return PartialView("_Footer", model);
        }

        [ChildActionOnly]
        public ActionResult TopPanel()
        {
            return PartialView(new TopPanelHandler().Get());
        }
    }
}
