using System.Web;
using System.Web.Mvc;
using AdvantShop.Configuration;
using AdvantShop.Core.Controls;
using AdvantShop.Customers;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Settings.Tasks;
using AdvantShop.Web.Admin.Models.Settings;
using AdvantShop.Web.Infrastructure.Controllers;

namespace AdvantShop.Web.Admin.Controllers.Settings
{
    [Auth(RoleAction.Tasks)]
    public class SettingsTasksController : BaseAdminController
    {
        public ActionResult Index()
        {
            var model = new LoadSaveTasksSettings().Load();

            SetMetaInformation(T("Admin.Settings.Tasks.Title"));
            SetNgController(NgControllers.NgControllersTypes.SettingsTasksCtrl);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(TasksSettingsModel model)
        {
            if (ModelState.IsValid)
            {
                new LoadSaveTasksSettings(model).Save();
                ShowMessage(NotifyType.Success, T("Admin.Settings.SaveSuccess"));
            }
            else
            {
                foreach (var modelState in ViewData.ModelState.Values)
                    foreach (var error in modelState.Errors)
                    {
                        ShowMessage(NotifyType.Error, error.ErrorMessage);
                    }
            }


            var isMobile = SettingsDesign.IsMobileTemplate;

            var referrerParams = new System.Web.Routing.RouteValueDictionary();

            if (HttpContext.Request.UrlReferrer != null)
            {
                var nv = HttpUtility.ParseQueryString(HttpContext.Request.UrlReferrer.Query); //Преобразует урлхвост в NameValueCollection

                foreach (var nvKey in nv.AllKeys)
                {
                    if (referrerParams.ContainsKey(nvKey) is false)
                        referrerParams.Add(nvKey, nv.Get(nvKey));
                }
            }

            if (isMobile)
            {
                return RedirectToAction("Index", referrerParams);
            }
            else
            {
                return Index();
            }
        }
    }
}
