using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Partners;
using AdvantShop.Core.Services.SalesChannels;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.FilePath;
using AdvantShop.Saas;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Partners;
using AdvantShop.Web.Admin.Models;
using AdvantShop.Web.Admin.Models.Partners;
using AdvantShop.Web.Admin.ViewModels.Partners;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Web.Admin.Controllers.Partners
{
    [Auth(RoleAction.Partners)]
    [SaasFeature(ESaasProperty.Partners)]
    [SalesChannel(ESalesChannelType.Partners)]
    public partial class PartnersController : BaseAdminController
    {
        [ChildActionOnly]
        public ActionResult NavMenu()
        {
            return PartialView();
        }

        public ActionResult Index()
        {
            SetMetaInformation(T("Admin.Partners.Index.Title"));
            SetNgController(NgControllers.NgControllersTypes.PartnersCtrl);

            var model = new PartnersListViewModel();

            return View(model);
        }

        public JsonResult GetPartners(PartnersFilterModel model)
        {
            return Json(new GetPartnersPagingHandler(model).Execute());
        }

        #region AddEdit

        // Редирект на PopupAdd
        public ActionResult Add()
        {
            return Redirect(UrlService.GetAdminUrl("partners?partnerIdInfo="));
        }

        // Редирект на Popup
        public ActionResult Edit(int id)
        {
            return Redirect(UrlService.GetAdminUrl(UrlService.GetAdminUrl("partners?partnerIdInfo=" + id)));
        }

        [Auth(EAuthErrorType.PartialView, RoleAction.Partners)]
        [SaasFeature(ESaasProperty.Partners, partial: true)]
        public ActionResult PopupAdd()
        {
            var model = new PartnerEditModel()
            {
                Partner = new Partner
                {
                    RewardPercent = SettingsPartners.DefaultRewardPercent,
                    Enabled = true
                }
            };

            return PartialView("~/Areas/Admin/Views/Partners/AddEdit.cshtml", model);
        }

        [Auth(EAuthErrorType.PartialView, RoleAction.Partners)]
        [SaasFeature(ESaasProperty.Partners, partial: true)]
        public ActionResult Popup(int id)
        {
            var model = new GetPartnerHandler(id).Execute();
            if (model == null)
                return Error404(partial: true);

            return PartialView("~/Areas/Admin/Views/Partners/AddEdit.cshtml", model);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SavePopup(PartnerEditModel model)
        {
            return ProcessJsonResult(new AddUpdatePartnerHandler(model));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeletePartner(int id)
        {
            return ProcessJsonResult(new DeletePartnerHandler(id));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ChangePassword(int partnerId, string pass, string pass2)
        {
            return ProcessJsonResult(new ChangePasswordHandler(partnerId, pass, pass2));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UpdateAdminComment(int partnerId, string comment)
        {
            return ProcessJsonResult(new UpdateAdminCommentHandler(partnerId, comment));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeletePartners(List<int> ids, string selectMode)
        {
            if (selectMode == "all")
            {
                var partnerIds = PartnerService.GetPartnerIds(false);
                foreach (var id in partnerIds)
                {
                    PartnerService.DeletePartner(id);
                }
            }

            if (ids == null) return Json(false);
            foreach (var id in ids)
            {
                PartnerService.DeletePartner(id);
            }
            return Json(true);
        }

        #endregion

        #region View

        public ActionResult View(int id)
        {
            try
            {
                var model = new GetPartnerViewHandler(id).Execute();

                SetMetaInformation(T("Admin.Partners.View.Title") + " " + model.Partner.Name);
                SetNgController(NgControllers.NgControllersTypes.PartnerViewCtrl);

                return View(model);
            }
            catch (BlException)
            {
                return Error404();
            }
        }

        public JsonResult GetView(int id)
        {
            return ProcessJsonResult(new GetPartnerViewHandler(id));
        }

        #endregion

        #region Coupon

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddPartnerCoupon(int partnerId, int couponId)
        {
            return ProcessJsonResult(new AddPartnerCouponHandler(partnerId, couponId));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddPartnerCouponFromTpl(int partnerId, string couponCode)
        {
            return ProcessJsonResult(new AddPartnerCouponFromTplHandler(partnerId, couponCode));
        }

        public JsonResult CheckPartnerCouponTpl()
        {
            var couponTpl = CouponService.GetPartnersCouponTemplate();
            return couponTpl != null ? JsonOk() : JsonError();
        }

        #endregion

        #region Customers

        public JsonResult GetBindedCustomers(PartnerCustomersFilterModel model)
        {
            return Json(new GetBindedCustomersHandler(model).Execute());
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult BindCustomer(int partnerId, Guid customerId)
        {
            return ProcessJsonResult(new BindCustomerHandler(partnerId, customerId));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UnbindCustomer(Guid customerId)
        {
            return ProcessJsonResult(new UnbindCustomerHandler(customerId));
        }

        #endregion

        #region Balance

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddMoney(int partnerId, decimal amount, string basis)
        {
            return ProcessJsonResult(new ProcessMoneyHandler(partnerId, amount, basis));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SubtractMoney(int partnerId, decimal amount, string basis)
        {
            return ProcessJsonResult(new ProcessMoneyHandler(partnerId, -amount, basis));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult RewardPayout(int partnerId, decimal amount, string basis, DateTime rewardPeriodTo)
        {
            return ProcessJsonResult(new ProcessMoneyHandler(partnerId, -amount, basis, isRewardPayout: true, rewardPeriodTo: rewardPeriodTo));
        }

        public JsonResult GetRewardPayoutFormData(int partnerId, DateTime? rewardPeriodTo)
        {
            return ProcessJsonResult(new GetRewardPayoutDataHandler(partnerId, rewardPeriodTo));
        }

        #endregion

        #region Transactions

        public JsonResult GetTransactions(PartnerTransactionsFilterModel model)
        {
            return Json(new GetTransactionsHandler(model).Execute());
        }

        #endregion

        #region Act-Reports

        public JsonResult GetActReports(PartnerActReportsFilterModel model)
        {
            return Json(new GetActReportsHandler(model).Execute());
        }

        public ActionResult ActReport(int id)
        {
            var report = ActReportService.GetActReport(id);
            if (report == null)
                return Error404();
            var filePath = FoldersHelper.GetPathAbsolut(FolderType.PartnerActReports, report.FileName);
            if (!System.IO.File.Exists(filePath))
                return Error404();

            return File(filePath, "application/octet-stream", report.FileName);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult GenerateActReport(int partnerId, bool sendMail = false)
        {
            return ProcessJsonResult(() => PartnerReportService.GenerateActReport(partnerId, sendMail));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteActReport(int id)
        {
            ActReportService.DeleteActReport(id);
            return JsonOk();
        }

        #endregion

        public JsonResult GetTypes()
        {
            return Json(Enum.GetValues(typeof(EPartnerType))
                .Cast<EPartnerType>().Where(x => x != EPartnerType.None)
                .Select(x => new SelectItemModel<int>(x.Localize(), (int)x)));
        }
    }
}
