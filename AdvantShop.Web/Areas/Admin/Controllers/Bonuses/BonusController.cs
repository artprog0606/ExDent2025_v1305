using System.Web.Mvc;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Configuration;
using AdvantShop.Core.UrlRewriter;

namespace AdvantShop.Web.Admin.Controllers.Bonuses
{
    public class BonusController : BaseAdminController
    {
        
        public ActionResult Index()
        {
            var isMobile = SettingsDesign.IsMobileTemplate;

            if(isMobile)
            {
                SetNgController(NgControllers.NgControllersTypes.ExportFeedsCtrl);
                return View();
            } 
            else
            {
                return Redirect(UrlService.GetAdminUrl("cards"));
            }
        }
    }
}
