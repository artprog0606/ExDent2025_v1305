using System;
using System.Web.Mvc;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.SalesChannels;
using AdvantShop.Core.Services.Webhook;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.Saas;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Models.Services;
using AdvantShop.Web.Admin.ViewModels.Domains;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Web.Admin.Controllers.Shared
{
    public class ServiceController : BaseAdminController
    {
        public ServiceController()
        {
            var executionTime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm");
            ViewBag.ExecutionTime = executionTime;
            ViewBag.Hash = (SettingsLic.LicKey + executionTime + SettingsLic.ClientCode).Md5();
        }

        public ActionResult Tariffs()
        {
            SetMetaInformation(T("Admin.Service.Tariffs"));
            SetNgController(NgControllers.NgControllersTypes.TariffsCtrl);
            return View();
        }

        public ActionResult ChangeTariff()
        {
            SetMetaInformation(T("Admin.Service.ChangeTariff"));
            SetNgController(NgControllers.NgControllersTypes.TariffsCtrl);
            return View();
        }

        public ActionResult Domains(bool isOpenModalAddDomain)
        {
            Track.TrackService.TrackEvent(Track.ETrackEvent.Dashboard_DomenDone);

            SetMetaInformation(T("Admin.Service.Domain"));
            SetNgController(NgControllers.NgControllersTypes.TariffsCtrl);
            return View(new DomainsViewModel()
            {
                IsOpenAddDomainModal = isOpenModalAddDomain
            });
        }

        [Auth]
        public ActionResult DomainsManage()
        {
            SetMetaInformation(T("Admin.Service.DomainsManage"));
            SetNgController(NgControllers.NgControllersTypes.DomainsManageCtrl);
            return View();
        }

        public ActionResult DomainBinding()
        {
            return PartialView("_DomainBinding");
        }

        public ActionResult DomainBuy(int? funnelId = null)
        {
            SetMetaInformation(T("Admin.Service.Domain"));
            SetNgController(NgControllers.NgControllersTypes.TariffsCtrl);

            var model = new DomainBuyModel() { FunnelId = funnelId };

            return View(model);
        }

        public ActionResult SupportCenter()
        {
            //SettingsCongratulationsDashboard.SupportDone = true;
            Track.TrackService.TrackEvent(Track.ETrackEvent.Dashboard_SupportDone);

            SetMetaInformation(T("Admin.Service.SupportCenter"));
            SetNgController(NgControllers.NgControllersTypes.TariffsCtrl);
            return View();
        }

        public ActionResult Actions(string id)
        {
            ViewBag.ActionId = id;
            SetMetaInformation(T("Admin.Service.Tariffs"));
            SetNgController(NgControllers.NgControllersTypes.TariffsCtrl);
            return View();
        }

        [AdminAuth]
        public ActionResult GetFeature(string id, bool partial = false)
        {
            if (partial)
                return PartialView("~/areas/admin/views/service/GetFeature.cshtml", (object)id);

            SetMetaInformation(T("Admin.Service.Tariffs"));
            
            return View("~/areas/admin/views/service/GetFeature.cshtml", (object)id);
        }

        public ActionResult BuyTemplate(string id)
        {
            SetMetaInformation(T("Admin.Service.BuyTemplate"));
            SetNgController(NgControllers.NgControllersTypes.TariffsCtrl);
            return View((object)id);
        }

        public ActionResult RoleAccessIsDenied(RoleAction roleActionKey, bool partial = false)
        {
            if (partial)
                return PartialView("~/areas/admin/views/service/RoleAccessIsDenied.cshtml", (object)roleActionKey.Localize());

            return View("~/areas/admin/views/service/RoleAccessIsDenied.cshtml", (object)roleActionKey.Localize());
        }

        public JsonResult RoleAccessIsDeniedJson(RoleAction roleActionKey)
        {
            return JsonAccessDenied();
        }

        public ActionResult UnderConstruction()
        {
            return View("~/areas/admin/views/service/UnderConstruction.cshtml");
        }

        public ActionResult ResetSaasFromAdmin()
        {
            SaasDataService.GetSaasData(true);
            return Redirect(Request.GetUrlReferrer() != null ? Request.GetUrlReferrer().ToString() : "~/adminv2/");
        }


        [HttpPost, WebhookAuth(EWebhookType.None), ExcludeFilter(typeof(AdminAuthAttribute))]
        public JsonResult Reset(string apikey)
        {
            SaasDataService.GetSaasData(true);
            return Json(true);
        }

        public ActionResult GetSalesChannel(ESalesChannelType id, bool partial = false)
        {
            var channel = SalesChannelService.GetByType(id);

            if (partial)
                return PartialView("~/areas/admin/views/service/GetSalesChannel.cshtml", channel);

            SetMetaInformation(channel.Name);

            return View("~/areas/admin/views/service/GetSalesChannel.cshtml", channel);
        }
    }
}
