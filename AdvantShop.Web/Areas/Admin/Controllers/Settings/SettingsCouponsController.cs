using System.Web.Mvc;
using AdvantShop.Customers;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Infrastructure.Controllers;

namespace AdvantShop.Web.Admin.Controllers.Settings
{
    [Auth(EAuthKeysComparer.And, RoleAction.CouponsAndDiscounts, RoleAction.Settings)]
    public class SettingsCouponsController : BaseAdminController
    {
        public ActionResult Index()
        {
            SetMetaInformation(T("Admin.Settings.SettingsCoupons.Title"));
            SetNgController(NgControllers.NgControllersTypes.SettingsCouponsCtrl);

            return View();
        }
    }
}
