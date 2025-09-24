using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.Services.Landing.Templates;
using AdvantShop.Core.Services.SalesChannels;
using AdvantShop.Tools.QrCode;
using AdvantShop.Web.Admin.Handlers.Dashboard;
using AdvantShop.Web.Admin.Handlers.Home;
using AdvantShop.Web.Admin.ViewModels.Home;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Filters;
using System.IO;
using System;
using System.Web.Mvc;
using AdvantShop.Core;
using AdvantShop.Customers;
using AdvantShop.Saas;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.ViewModels.Shared.Dashboard;

namespace AdvantShop.Web.Admin.Controllers.Shared
{
    [Auth(RoleAction.Store, RoleAction.Landing)]
    public partial class DashboardController : BaseAdminController
    {
        public ActionResult Index()
        {
            SetMetaInformation(T("Admin.Shared.Dashboard.CreateFunnel.MySites"));
            SetNgController(NgControllers.NgControllersTypes.DashboardSitesCtrl);

            var isSaas = SaasDataService.IsSaasEnabled;
            var viewModel = new CommonDashboardInfoViewModel
            {
                IsSaas = isSaas
            };
            
            if (isSaas)
            {
                viewModel.AllowedSitesCount = SaasDataService.CurrentSaasData.LandingFunnelCount;
                if (!SaasDataService.CurrentSaasData.DisableStore)
                    viewModel.AllowedSitesCount++;
                viewModel.CurrentSitesCount = new LpSiteService().GetList().Count;
                if (SettingsMain.StoreActive)
                    viewModel.CurrentSitesCount++;
            }

            return View(viewModel);
        }

        public ActionResult SettingStoreDashboard()
        {
            SetNgController(NgControllers.NgControllersTypes.DashboardSitesCtrl);

            return View();
        }

        public JsonResult GetDashBoard()
        {
            return Json(new GetDashboard().Execute());
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteSite(int id, DashboardSiteItemType type)
        {
            switch (type)
            {
                case DashboardSiteItemType.Funnel:
                    {
                        var service = new LpSiteService();
                        var funnel = service.Get(id);
                        if (funnel != null)
                        {
                            service.Delete(id);
                            return JsonOk();
                        }
                        break;
                    }

                case DashboardSiteItemType.Store:
                    {
                        var store = SalesChannelService.GetByType(ESalesChannelType.Store);
                        store.Enabled = false;
                        return JsonOk();
                    }
            }

            return JsonError(T("Admin.Shared.Dashboard.CreateFunnel.FailedToDeleteSite"));
        }

        public ActionResult CreateSite(string mode)
        {
            SetMetaInformation(T("Admin.Shared.Dashboard.CreateFunnel.CreatingNewSite"));
            SetNgController(NgControllers.NgControllersTypes.CreateSiteCtrl);

            var model = new GetCreateSiteModel(mode).Execute();

            return View(model);
        }

        public ActionResult Preview()
        {
            SetMetaInformation(T("Admin.Shared.Dashboard.CreateFunnel.CreatingNewSite"));
            SetNgController(NgControllers.NgControllersTypes.CreateSiteCtrl);

            return View();
        }

        public ActionResult TrialFirstSession()
        {

            if (SalesChannelService.IsFirstTimeCreateStore())
            {
                SetMetaInformation(T("Admin.Shared.Dashboard.CreateFunnel.CreatingNewSite"));
                SetNgController(NgControllers.NgControllersTypes.CreateSiteCtrl);

                return View();
            }

            return RedirectToAction("Index", "Home");
        }


        public JsonResult GetSiteTemplates(LpSiteCategory category)
        {
            return Json(new GetSiteTemplates(category).Execute());
        }

        public ActionResult CreateTemplate(string id, string mode)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("CreateSite", new {mode});

            var model = new GetCreateTemplateModel(id).Execute();
            if (model == null)
                return Error404();

            SetMetaInformation(T("Admin.Shared.Dashboard.CreateFunnel.CreatingNewSite"));
            SetNgController(NgControllers.NgControllersTypes.CreateSiteCtrl);

            model.Mode = mode;

            return View(model);
        }

        public ActionResult CreateFunnel(string id)
        {
            var model = new GetCreateFunnelModel(id).Execute();
            if (model == null)
                return Error404();

            SetMetaInformation(T("Admin.Shared.Dashboard.CreateFunnel.CreatingNewSite"));
            SetNgController(NgControllers.NgControllersTypes.CreateSiteCtrl);

            return View(model);
        }

        //[HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult CreateScreenShots()
        {
            new CreateDashboardScreenShots().Execute();
            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ChangeMainUrl(string url)
        {
            if (!string.IsNullOrEmpty(url))
                SettingsMain.SiteUrl = url;

            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ChangeEnabled(int id, DashboardSiteItemType type, bool enabled)
        {
            return ProcessJsonResult(() =>
            {
                switch (type)
                {
                    case DashboardSiteItemType.Funnel:
                        var service = new LpSiteService();
                        var funnel = service.Get(id);
                        if (funnel == null)
                            throw new BlException(T("Admin.Shared.Dashboard.CreateFunnel.FunnelNotFound"));

                        funnel.Enabled = enabled;

                        service.Update(funnel);
                        break;

                    case DashboardSiteItemType.Store:
                        var store = SalesChannelService.GetByType(ESalesChannelType.Store);
                        store.Enabled = enabled;
                        break;

                    default:
                        throw new BlException(T("Admin.Shared.Dashboard.CreateFunnel.TypeNotSupported"));
                }
            });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult GenerateQrCode(string text)
        {
            if (text.IsNullOrEmpty())
                return JsonError(T("Admin.Shared.Dashboard.CreateFunnel.LinkTextMissing"));
            var stream = QrCodeService.GetQrCode(text);
            if (stream == null)
                return JsonError(T("Admin.Shared.Dashboard.CreateFunnel.FailedGenerateQRCode"));
            if (stream is MemoryStream memoryStream)
                return JsonOk(Convert.ToBase64String(memoryStream.ToArray()));

            var bytes = new byte[(int)stream.Length];

            stream.Seek(0, SeekOrigin.Begin);
            stream.Read(bytes, 0, (int)stream.Length);
            return JsonOk(Convert.ToBase64String(bytes));
        }
    }
}