using System.Web.Mvc;
using AdvantShop.Core.Services.Partners;
using AdvantShop.FilePath;

namespace AdvantShop.Areas.Partners.Controllers
{
    public partial class ReportsController : BasePartnerController
    {
        public ActionResult ActReport(int id)
        {
            var report = ActReportService.GetActReport(id);
            if (report == null || report.PartnerId != PartnerContext.CurrentPartner.Id)
                return Error404();
            var filePath = FoldersHelper.GetPathAbsolut(FolderType.PartnerActReports, report.FileName);
            if (!System.IO.File.Exists(filePath))
                return Error404();

            return File(filePath, "application/octet-stream", 
                string.Format("act-report_{0}_{1}{2}", report.PeriodFrom.ToString("yyyyMMdd"), report.PeriodTo.ToString("yyyyMMdd"), System.IO.Path.GetExtension(report.FileName)));
        }

    }
}