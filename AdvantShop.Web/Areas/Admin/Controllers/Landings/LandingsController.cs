using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AdvantShop.App.Landing.Domain.Auth;
using AdvantShop.App.Landing.Domain.Seo;
using AdvantShop.App.Landing.Domain.Settings;
using AdvantShop.App.Landing.Domain.Templates;
using AdvantShop.App.Landing.Handlers.Schemes;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.Services.Landing.Blocks;
using AdvantShop.Core.Services.Landing.Domains;
using AdvantShop.Core.Services.SalesChannels;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using AdvantShop.Saas;
using AdvantShop.Track;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Landings;
using AdvantShop.Web.Admin.Handlers.Landings.MobileApp;
using AdvantShop.Web.Admin.Models.Landings;
using AdvantShop.Web.Admin.ViewModels.Landings;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Extensions;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Web.Admin.Controllers.Landings
{
    public partial class FunnelsController : LandingsController { }

    [SaasFeature(ESaasProperty.HaveLandingFunnel)]
    [Auth(RoleAction.Landing), LogActivity]
    [SalesChannel(ESalesChannelType.Funnel)]
    public partial class LandingsController : BaseAdminController
    {
        #region Ctor

        private readonly LpService _lpService;
        private readonly LpSiteService _siteService;


        public LandingsController()
        {
            _siteService = new LpSiteService();
            _lpService = new LpService();
        }

        #endregion

        #region Landings list

        public ActionResult Index(LandingsAdminFilterModel model)
        {
            return RedirectToAction("Index", "Dashboard");

            //SetMetaInformation("Воронки");
            //SetNgController(NgControllers.NgControllersTypes.LandingsAdminCtrl);

            //model.ItemsPerPage = 20;

            //var result = new GetLandings(model).Execute();

            //return View(result);
        }

        public ActionResult GetLandings(LandingsAdminFilterModel model)
        {
            var data = new GetLandings(model).Execute();
            return PartialView("_Item", data);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteSiteLanding(int id)
        {
            _siteService.Delete(id);
            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteSiteLandings(LandingsAdminFilterModel command)
        {
            Command(command, (id, c) => _siteService.Delete(id));
            return JsonOk();
        }

        private void Command(LandingsAdminFilterModel command, Action<int, LandingsAdminFilterModel> func)
        {
            if (command.SelectMode == SelectModeCommand.None)
            {
                foreach (var id in command.Ids)
                    func(id, command);
            }
            else
            {
                var ids = new GetLandings(command).GetItemsIds();
                foreach (int id in ids)
                {
                    if (command.Ids == null || !command.Ids.Contains(id))
                        func(id, command);
                }
            }
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult Inplace(LandingSiteAdminItemModel model)
        {
            if (model.Name.IsNullOrEmpty())
                return JsonError("Укажите название воронки");
            
            var landing = _siteService.Get(model.Id);
            if (landing != null)
            {

                landing.Name = StringHelper.RemoveHTML(model.Name).Reduce(100);
                _siteService.Update(landing);
            }

            return JsonOk();
        }

        public JsonResult SetSiteEnabled(int id, bool enabled)
        {
            var site = _siteService.Get(id);
            if (site != null)
            {
                site.Enabled = enabled;
                _siteService.Update(site);
            }
            return JsonOk();
        }

        #endregion

        #region Site / Add

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult Add(LandingAdminIndexPostModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                return JsonError("Укажите название воронки");

            model.Name = model.Name.DefaultOrEmpty();

            try
            {
                var lp = new InstallTemplateHandler(model).Execute();
                if (lp == null)
                    return JsonError("Ошибка при создании воронки");
                
                return Json(new
                {
                    result = true,
                    url = Url.AbsoluteRouteUrl("Landing", new
                    {
                        url = _siteService.Get(lp.LandingSiteId).Url,
                        lpurl = lp.IsMain ? null : lp.Url,
                        inplace = "true"
                    })
                });
            }
            catch (BlException ex)
            {
                return JsonError(ex.Message);
            }
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddLandingSiteIsAllowed(AddingLpModel model)
        {
            try
            {
                new AddLanding(model).CreatingIsAllowed();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return JsonError();
            }
            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddLandingSite(AddingLpModel model)
        {
            return ProcessJsonResult(new AddLanding(model));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult CopyLandingSite(int landingSiteId)
        {
            TrackService.TrackEvent(ETrackEvent.Shop_Funnels_CopyFunnel);
            return ProcessJsonResult(new CreateCopyLandingSite(landingSiteId));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult CopyLandingPage(int landingPageId)
        {
            TrackService.TrackEvent(ETrackEvent.Shop_Funnels_CopyPage);
            return ProcessJsonResult(new CreateCopyLandingPage(landingPageId));
        }

        public ActionResult Site(int id)
        {
            var model = new GetLandingSite(id).Execute();
            if (model == null)
                return RedirectToAction("Index");

            SetMetaInformation($"{T("Admin.Controllers.Landings.Funnel")} \"" + model.Site.Name + "\"");
            SetNgController(NgControllers.NgControllersTypes.LandingSiteAdminCtrl);

            return View(model);
        }

        #endregion

        #region Create Funnel

        public ActionResult CreateFunnel(CreateFunnelModel model)
        {
            return RedirectToAction("CreateSite", "Dashboard");

            //SetMetaInformation(T("Admin.Landings.CreateFunnel.Title"));
            //SetNgController(NgControllers.NgControllersTypes.CreateFunnelCtrl);

            //model.FunnelTypes = new LpFunnelService().GetFunnelTypes();


            //return View(model);
        }

        public ActionResult FunnelDetails(FunnelDetailsModel model)
        {
            return RedirectToAction("CreateSite", "Dashboard");

            //var viewModel = new FunnelDetailsViewModel
            //{
            //    Category = model.Category,
            //    ProductId = model.ProductId,
            //    FunnelModel = model.Category == LpFunnelCategory.Default
            //        ? new LpFunnelModel() { Category = LpFunnelCategory.Default}
            //        : new LpFunnelService().GetFunnelTypes().FirstOrDefault(x => x.Category == model.Category),
            //    Templates = new GetLandingTemplates(model.Category).Execute()
            //};

            //if (viewModel.FunnelModel == null)
            //    return RedirectToAction("CreateFunnel");

            //TrackService.TrackEvent(ETrackEvent.Shop_Funnels_ClickFunnelType, model.Category.ToString());

            //SetNgController(NgControllers.NgControllersTypes.FunnelDetailsCtrl);
            //SetMetaInformation(T("Admin.Landings.CreateFunnel.Title") + " - " + model.Category.Localize());

            //return View(viewModel);
        }

        #endregion

        #region Site Pages

        public JsonResult GetLandingPages(SiteLandingsAdminFilterModel model)
        {
            return Json(new GetSiteLandings(model).Execute());
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteLandingPage(int id)
        {
            var lp = _lpService.Get(id);
            if (lp != null && lp.IsMain)
                return JsonError("Нельзя удалить главную страницу воронки");

            _lpService.Delete(id);

            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult LandingPageInplace(LandingAdminItemModel model)
        {
            var lp = _lpService.Get(model.Id);
            if (lp == null)
                return JsonError();

            //other pages
            var landings = _lpService.GetList(lp.LandingSiteId).Where(x => x.Id != lp.Id).ToList();
            
            if (landings.Any())
            {
                //change property isMain
                if (lp.IsMain != model.IsMain)
                {
                    var sibling = landings.FirstOrDefault(x => x.IsMain == model.IsMain);
                    if (sibling != null)
                    {
                        sibling.IsMain = !sibling.IsMain;
                        _lpService.Update(sibling);
                        
                        var blockService = new LpBlockService();
                        foreach (var block in blockService.GetList(model.IsMain ? sibling.Id : lp.Id).Where(x => x.ShowOnAllPagesTop || x.ShowOnAllPagesBottom))
                        {
                            block.LandingId = model.IsMain ? lp.Id : sibling.Id;
                            blockService.Update(block);
                        }
                    }

                    lp.IsMain = model.IsMain;
                    lp.Enabled = true;
                }

                //change property Enabled
                if (lp.Enabled != model.Enabled)
                {
                    if (lp.IsMain && !model.Enabled)
                    {
                        lp.Enabled = true;
                    }
                    else
                    {
                        lp.Enabled = model.Enabled;
                    }
                }
            }
            else
            {
                lp.Enabled = true;
                lp.IsMain = true;
            }

            _lpService.Update(lp);

            LpSiteService.UpdateModifiedDateByLandingId(lp.Id);

            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteLandingPages(SiteLandingsAdminFilterModel command)
        {
            PagesCommand(command, (id, c) =>
            {
                var lp = _lpService.Get(id);
                if (lp != null && !lp.IsMain)
                    _lpService.Delete(id);
            });
            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ActivateLandingPages(SiteLandingsAdminFilterModel command)
        {
            PagesCommand(command, (id, c) => SetLandingPageActive(id, true));
            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DisableLandingPages(SiteLandingsAdminFilterModel command)
        {
            PagesCommand(command, (id, c) => SetLandingPageActive(id, false));
            return JsonOk();
        }

        private void SetLandingPageActive(int id, bool active)
        {
            var lp = _lpService.Get(id);
            if (lp != null)
            {
                lp.Enabled = active;
                _lpService.Update(lp);
            }
        }

        private void PagesCommand(SiteLandingsAdminFilterModel command, Action<int, SiteLandingsAdminFilterModel> func)
        {
            if (command.SelectMode == SelectModeCommand.None)
            {
                foreach (var id in command.Ids)
                    func(id, command);
            }
            else
            {
                var ids = new GetSiteLandings(command).GetItemsIds();
                foreach (int id in ids)
                {
                    if (command.Ids == null || !command.Ids.Contains(id))
                        func(id, command);
                }
            }
        }

        #endregion

        #region Site settings 

        public ActionResult SiteSettings(int id)
        {
            return Redirect(Url.AbsoluteRouteUrl(new { controller = "Funnels", action = "Site", id }) + "?landingAdminTab=settings");
        }

        public JsonResult GetSiteSettings(int id)
        {
            var model = new GetLandingSiteSettings(id).Execute();
            return model != null ? JsonOk(model) : JsonError();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SaveSiteSettings(LandingAdminSiteSettings model)
        {
            return new SaveLandingSiteSettings(model).Execute() ? JsonOk() : JsonError();
        }

        #region Auth

        public JsonResult GetAuthOrderProducts(int landingSiteId)
        {
            LpService.CurrentSiteId = landingSiteId;
            var productIds = LSiteSettings.AuthOrderProductIds;
            var products = ProductService.GetAllProductsByIds(productIds).Select(x => new
            {
                x.ProductId,
                x.Name,
                x.ArtNo,
                Url = Url.AbsoluteActionUrl("Edit", "Product", new { id = x.ProductId })
            });

            var ids = products.Select(x => x.ProductId).ToList();
            if (!productIds.SequenceEqual(ids))
                LSiteSettings.AuthOrderProductIds = ids;

            return JsonOk(products);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteAuthOrderProduct(int landingSiteId, int productId)
        {
            LpService.CurrentSiteId = landingSiteId;
            var productIds = LSiteSettings.AuthOrderProductIds;
            productIds.RemoveAll(x => x == productId);
            LSiteSettings.AuthOrderProductIds = productIds;

            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddAuthOrderProducts(int landingSiteId, List<int> ids)
        {
            if (ids == null || ids.Count == 0)
                return JsonError();

            LpService.CurrentSiteId = landingSiteId;
            var productIds = LSiteSettings.AuthOrderProductIds;
            foreach (var id in ids.Where(x => !productIds.Contains(x)))
                productIds.Add(id);
            LSiteSettings.AuthOrderProductIds = productIds;
            LSiteSettings.AuthFilterRule = ELpAuthFilterRule.WithOrderAndProduct;
            LSiteSettings.AuthLeadSalesFunnelId = null;
            LSiteSettings.AuthLeadDealStatusId = null;

            return JsonOk();
        }

        #endregion

        #region Domains

        [HttpGet]
        public JsonResult GetSiteDomain(int id)
        {
            var site = _siteService.Get(id);
            return Json(new
            {
                domain = site != null ? site.DomainUrl : null,
                techdomain = site != null ? LpService.GetTechUrl(site.Url, "", true) : null
            });
        }

        public JsonResult GetSiteDomains(int siteId)
        {
            return Json(new LpDomainService().GetList(siteId).Where(x => !x.IsMain));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SaveSiteDomain(int id, string domain, bool? isAdditional)
        {
            return ProcessJsonResult(() => new SaveSiteDomain(id, domain, isAdditional).Execute());
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult RemoveSiteDomain(int id, string domain)
        {
            return ProcessJsonResult(() => new RemoveSiteDomain(id, domain).Execute());
        }

        
        public JsonResult GetFunnelDomains(int siteId)
        {
            var allDomains = new LpDomainService().GetList();
            var siteDomains = new LpDomainService().GetList(siteId);

            var domains = allDomains.Where(x => siteDomains.Find(d => d.Id == x.Id) == null).ToList();

            return Json(domains);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SaveReuseSiteDomain(int id, string reuseDomain)
        {
            return ProcessJsonResult(() => new SaveReuseSiteDomain(id, reuseDomain).Execute());
        }

        #endregion

        #region Sitemap

        public JsonResult GetSitemapInfo(int siteId)
        {
            return Json(new GetLpSitemapInfo(siteId).Execute());
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult GenerateSitemapXml(int siteId, bool useHttps)
        {
            return ProcessJsonResult(() => new LpSitemapGeneratorXml(siteId, useHttps).Execute());
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult GenerateSitemapHtml(int siteId, bool useHttps)
        {
            return ProcessJsonResult(() => new LpSitemapGeneratorHtml(siteId, useHttps).Execute());
        }

        #endregion

        #region LandingSite Products

        public JsonResult GetSiteProducts(SiteProductsFilterModel model)
        {
            return Json(new GetSiteProducts(model).Execute());
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddSiteProducts(int siteId, List<int> ids)
        {
            if (siteId == 0 || ids == null || ids.Count == 0)
                return JsonError();

            foreach (var productId in ids)
                _siteService.AddUpdateAdditionalSalesProduct(productId, siteId);

            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UnsetProductsFunnel(SiteProductsFilterModel model)
        {
            Command(model, (id, c) => _siteService.DeleteAdditionalSalesProduct(id, c.SiteId));
            return Json(true);
        }

        private void Command(SiteProductsFilterModel model, Action<int, SiteProductsFilterModel> func)
        {
            if (model.SelectMode == SelectModeCommand.None)
            {
                foreach (var id in model.Ids)
                    func(id, model);
            }
            else
            {
                var ids = new GetSiteProducts(model).GetItemsIds("Product.ProductId");
                foreach (int id in ids)
                {
                    if (model.Ids == null || !model.Ids.Contains(id))
                        func(id, model);
                }
            }
        }

        #endregion

        #region MobileApp

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UploadIcon(int objId)
        {
            var result = new UploadMobileAppIconHandler(objId).Execute();

            if (!result.Result)
                return JsonError(result.Error);

            new WebManifestHandler(objId).Execute();
            return JsonOk(new
            {
                picture = result.Picture,
                pictureId = result.PictureId
            });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UploadIconByLink(string fileLink, int objId)
        {
            var result = new UploadMobileAppIconByLinkHandler(objId, fileLink).Execute();
            if (!result.Result)
                return JsonError(result.Error);

            new WebManifestHandler(objId).Execute();
            return JsonOk(new
            {
                picture = result.Picture,
                pictureId = result.PictureId
            });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteIcon(int objId)
        {
            LpService.CurrentSiteId = objId;
            var landingPath = System.Web.Hosting.HostingEnvironment.MapPath(string.Format(LpFiles.LpSitePath, objId));
            FileHelpers.DeleteMobileAppIcons(LSiteSettings.MobileAppIconImageName, landingPath);

            LSiteSettings.MobileAppIconImageName = string.Empty;

            new WebManifestHandler(objId).Execute();
            return JsonOk(new { picture = string.Empty });
        }

        #endregion

        #endregion

        #region Track events

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult TrackCreateFunnelShow()
        {
            TrackService.TrackEvent(ETrackEvent.Landings_CreateFunnelShow);
            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]

        public JsonResult CreateFreeShippingFunnel_Step0()
        {
            TrackService.TrackEvent(ETrackEvent.Landings_CreateFreeShippingFunnel_Step0);
            return JsonOk();
        }
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult CreateFreeShippingFunnel_Step1()
        {
            TrackService.TrackEvent(ETrackEvent.Landings_CreateFreeShippingFunnel_Step1);
            return JsonOk();
        }
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult CreateFreeShippingFunnel_Step2()
        {
            TrackService.TrackEvent(ETrackEvent.Landings_CreateFreeShippingFunnel_Step2);
            return JsonOk();
        }
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult CreateFreeShippingFunnel_Step3()
        {
            TrackService.TrackEvent(ETrackEvent.Landings_CreateFreeShippingFunnel_Step3);
            return JsonOk();
        }
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult CreateFreeShippingFunnel_Step4()
        {
            TrackService.TrackEvent(ETrackEvent.Landings_CreateFreeShippingFunnel_Step4);
            return JsonOk();
        }
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult CreateFreeShippingFunnel_Step5()
        {
            TrackService.TrackEvent(ETrackEvent.Landings_CreateFreeShippingFunnel_Step5);
            return JsonOk();
        }
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult CreateFreeShippingFunnel_StepFinal()
        {
            TrackService.TrackEvent(ETrackEvent.Landings_CreateFreeShippingFunnel_StepFinal);
            return JsonOk();
        }


        #endregion

        #region Email Sequences, Leads, Orders, Booking

        public ActionResult _EmailSequences()
        {
            return PartialView("FunnelData/_Emails");
        }

        public JsonResult GetSiteTriggerEmails(int orderSourceId, DateTime dateFrom, DateTime dateTo)
        {
            return ProcessJsonResult(new GetSiteTriggerEmails(orderSourceId, dateFrom, dateTo));
        }

        public ActionResult _Leads()
        {
            return PartialView("FunnelData/_Leads");
        }

        public ActionResult _Orders()
        {
            return PartialView("FunnelData/_Orders");
        }

        public ActionResult _Bookings()
        {
            return PartialView("FunnelData/_Bookings");
        }

        public JsonResult GetFunnelStats(int orderSourceId)
        {
            var currentCustomer = CustomerContext.CurrentCustomer;
            var currentManager = ManagerService.GetManager(currentCustomer.Id);

            // статистика для админа без учета менеджера
            var managerId = currentCustomer.IsAdmin || currentManager == null ? (int?)null : currentManager.ManagerId;
            return Json(new
            {
                leadsCount = Core.Services.Crm.LeadService.GetNewLeadsCount(managerId, orderSourceId),
                ordersCount = Core.Services.Statistic.StatisticService.GetLastOrdersCount(managerId, orderSourceId),
                bookingsCount = Core.Services.Booking.BookingService.GetLastBookingCount(managerId: managerId, orderSourceId: orderSourceId)
            });
        }

        #endregion

        [HttpGet]
        public JsonResult GetLandingSitesList()
        {
            var list = _siteService.GetList().Select(x => new {label = x.Name, value = x.Id});
            return Json(list);
        }

        public JsonResult GetFunnelScheme(int siteId)
        {
            var result = new GetFunnelScheme(siteId).Execute();

            return Json(result);
        }
    }
}
