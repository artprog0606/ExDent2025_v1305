using System.Web.Mvc;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Customers;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Models.Marketing.LandingPages;
using AdvantShop.Web.Infrastructure.Controllers;

namespace AdvantShop.Web.Admin.Controllers.Marketing
{
    //[Auth(RoleAction.Marketing)]
    [SaasFeature(Saas.ESaasProperty.LandingPage)]
    public partial class LandingPagesController : BaseAdminController
    {
        public ActionResult Index()
        {
            var model = new LandingPageAdminModel
            {
                ActiveLandingPage = SettingsLandingPage.ActiveLandingPage,
                LandingPageCommonStatic = 
                    SettingsLandingPage.LandingPageCommonStatic ??
                    "<div class='plp-reasons'><div class='plp-subTitle'>" + T("Admin.Marketing.5reasons") + "</div><div class='r-container'>" +
                    "<div class='row center-xs'><div class='col-xs-3 cs-bg-3 reas-flex'>" +
                    "<div class='r-item'><div class='r-img'><img src='modules/productlandingpage/templates/lp/pictures/r1.png' alt='' /></div>" +
                    "<div class='r-info'><div class='r-title'>" + T("Admin.Marketing.MoneyBackGuarantee") + "</div>" +
                    "<div class='r-txt'>" + T("Admin.Marketing.RefundMoney") + "</div></div></div></div><div class='col-xs-3 cs-bg-3 reas-flex'>" +
                    "<div class='r-item'><div class='r-img'><img src='modules/productlandingpage/templates/lp/pictures/r2.png' alt='' /></div>" +
                    "<div class='r-info'><div class='r-title'>" + T("Admin.Marketing.DeliverWithoutPrepayment") + "</div>" +
                    "<div class='r-txt'>" + T("Admin.Marketing.DeliverToAnyRegion") + "</div></div></div></div>" +
                    "<div class='col-xs-3 cs-bg-3 reas-flex'><div class='r-item'><div class='r-img'><img src='modules/productlandingpage/templates/lp/pictures/r3.png' alt='' /></div>" +
                    "<div class='r-info'><div class='r-title'>" + T("Admin.Marketing.GiftAtPurchase") + "</div>" +
                    "<div class='r-txt'>" + T("Admin.Marketing.PoshGiftBag") + "</div></div></div></div>" +
                    "<div class='col-xs-3 cs-bg-3 reas-flex'><div class='r-item'><div class='r-img'><img src='modules/productlandingpage/templates/lp/pictures/r4.png' alt='' /></div>" +
                    "<div class='r-info'><div class='r-title'>" + T("Admin.Marketing.WhenBuyingLaptop") + "</div><div class='r-txt'>" + T("Admin.Marketing.StylishBag") + " <br /> " + T("Admin.Marketing.ForPresent") + "</div></div></div></div>" +
                    "<div class='col-xs-3 cs-bg-3 reas-flex'><div class='r-item'><div class='r-img'><img src='modules/productlandingpage/templates/lp/pictures/r5.png' alt='' /></div>" +
                    "<div class='r-info'><div class='r-title'>" + T("Admin.Marketing.ReturnWithoutProblem") + "</div>" +
                    "<div class='r-txt'>" + T("Admin.Marketing.ReturnWithin14Days") + "</div></div></div></div></div></div></div>"
            };

            SetMetaInformation("Landing Page");
            SetNgController(NgControllers.NgControllersTypes.LandingPagesCtrl);

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Index(LandingPageAdminModel model)
        {
            // fix for moving landing from module to engine
            SettingsLandingPage.LandingPageCommonStatic = model.LandingPageCommonStatic.DefaultOrEmpty().Replace("modules/productlandingpage/", "landings/");
            SettingsLandingPage.ActiveLandingPage = model.ActiveLandingPage;

            return RedirectToAction("Index");
        }
    }
}
