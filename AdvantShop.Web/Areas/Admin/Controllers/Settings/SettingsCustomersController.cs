using System.Web.Mvc;
using AdvantShop.Core.Controls;
using AdvantShop.Customers;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Settings.Customers;
using AdvantShop.Web.Admin.Models.Settings;
using AdvantShop.Web.Infrastructure.Controllers;

namespace AdvantShop.Web.Admin.Controllers.Settings
{
    [Auth(EAuthKeysComparer.And, RoleAction.Customers, RoleAction.Settings)]
    public class SettingsCustomersController : BaseAdminController
    {
        public ActionResult Index()
        {
            SetMetaInformation(T("Admin.Settings.Customers.Title"));
            SetNgController(NgControllers.NgControllersTypes.SettingsCustomersCtrl);

            var model = new GetCustomersSettingsHandler().Execute();
            return View("Index", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Index(CustomersSettingsModel model)
        {
            if (ModelState.IsValid)
            {
                new SaveCustomersSettingsHandler(model).Execute();
                ShowMessage(NotifyType.Success, T("Admin.Settings.SaveSuccess"));
            }
            else
            {
                ShowErrorMessages();
            }

            return Index();
        }
    }
}
