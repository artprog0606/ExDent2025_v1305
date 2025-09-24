using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.Controls;
using AdvantShop.Core.Services.IPTelephony;
using AdvantShop.Customers;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Settings.TelephonySettings;
using AdvantShop.Web.Admin.Models.Settings;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Filters;
using Newtonsoft.Json;
using RestSharp.Serialization.Json;

namespace AdvantShop.Web.Admin.Controllers.Settings
{
    [Auth(RoleAction.Settings)]
    public partial class SettingsTelephonyController : BaseAdminController
    {
        public ActionResult Index()
        {
            var model = new GetIPTelephonySettings().Execute();

            SetMetaInformation(T("Admin.Settings.Telephony.Title"));
            SetNgController(NgControllers.NgControllersTypes.SettingsTelephonyCtrl);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(IPTelephonySettingsModel model)
        {
            if (ModelState.IsValid)
            {
                var handler = new SaveIPTelephonySettings(model);
                try
                {
                    handler.Execute();
                    ShowMessage(NotifyType.Success, T("Admin.ChangesSuccessfullySaved"));
                }
                catch (BlException e)
                {
                    ModelState.AddModelError(e.Property, e.Message);
                }
            }
            ShowErrorMessages();

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

        #region Telphin

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult GetTelphinExtensions()
        {
            var extensions = new TelphinHandler().GetExtensions();
            return JsonOk(extensions);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddTelphinEvents(string extensionId)
        {
            try
            {
                var extensions = new TelphinHandler().AddEvents(extensionId);
                return JsonOk(extensions);
            }
            catch (BlException e)
            {
                return JsonError(e.Message);
            }
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteTelphinEvents(string extensionId)
        {
            try
            {
                var extensions = new TelphinHandler().DeleteEvents(extensionId);
                return JsonOk(extensions);
            }
            catch (BlException e)
            {
                return JsonError(e.Message);
            }
        }

        #endregion

        #region Phone Order Sources

        public JsonResult GetPhoneOrderSources()
        {
            return JsonOk(IPTelephonyService.PhoneOrderSources);
        }

        public JsonResult SavePhoneOrderSources(string phoneOrderSources)
        {
            try
            {
                IPTelephonyService.PhoneOrderSources = JsonConvert.DeserializeObject<Dictionary<long, int?>>(phoneOrderSources);
            }
            catch (Exception)
            {
                return JsonError();
            }
            return JsonOk();
        }

        #endregion
    }
}
