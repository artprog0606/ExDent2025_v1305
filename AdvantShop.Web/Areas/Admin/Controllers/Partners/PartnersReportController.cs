using System.Web.Mvc;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Partners;
using AdvantShop.Core.Services.SalesChannels;
using AdvantShop.Customers;
using AdvantShop.FilePath;
using AdvantShop.Saas;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.PartnersReport;
using AdvantShop.Web.Admin.Models.PartnersReport;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Extensions;
using AdvantShop.Web.Infrastructure.Filters;
using IOFile = System.IO.File;
using IOPath = System.IO.Path;

namespace AdvantShop.Web.Admin.Controllers.Partners
{
    [Auth(RoleAction.Partners)]
    [SaasFeature(ESaasProperty.Partners)]
    [SalesChannel(ESalesChannelType.Partners)]
    public partial class PartnersReportController : BaseAdminController
    {
        #region отчет по партнерам

        public ActionResult PartnersReport()
        {
            SetMetaInformation("Отчет по партнерам");
            SetNgController(NgControllers.NgControllersTypes.PartnersReportCtrl);

            return View();
        }

        public ActionResult GetPartnersReport(string from, string to)
        {
            var dateFrom = from.TryParseDateTime(isNullable: true);
            var dateTo = to.TryParseDateTime(isNullable: true);

            var filePath = PartnerReportService.GeneratePartnersReport(dateFrom, dateTo);
            return File(filePath, "application/octet-stream", IOPath.GetFileName(filePath));
        }

        #endregion

        #region отчет по выплатам партнерам

        public ActionResult PayoutReports()
        {
            SetMetaInformation("Отчеты по выплатам партнерам");
            SetNgController(NgControllers.NgControllersTypes.PartnersPayoutReportsCtrl);

            return View();
        }

        public JsonResult GetPayoutReports(PayoutReportsFilterModel model)
        {
            return Json(new GetPayoutReportsHandler(model).Execute());
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeletePayoutReport(int id)
        {
            PayoutReportService.DeletePayoutReport(id);
            return JsonOk();
        }

        // сформированный отчет по выплатам партнерам
        public ActionResult PayoutReport(int id)
        {
            var report = PayoutReportService.GetPayoutReport(id);
            if (report == null)
                return Error404();
            var filePath = FoldersHelper.GetPathAbsolut(FolderType.PartnerPayoutReports, report.FileName);
            if (!IOFile.Exists(filePath))
                return Error404();

            return File(filePath, "application/octet-stream", report.FileName);
        }

        // сформировать и получить отчет по выплатам партнерам
        [HttpPost, ValidateJsonAntiForgeryToken]
        public ActionResult GetPayoutsReport()
        {
            var report = PartnerReportService.GeneratePartnersPayoutReport();
            if (report == null)
                return JsonError();
            return JsonOk(new { url = Url.AbsoluteActionUrl("PayoutReport", new { id = report.Id }) });
        }

        #endregion
    }
}
