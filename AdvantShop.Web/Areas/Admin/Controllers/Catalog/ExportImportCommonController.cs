using System.IO;
using System.Web.Mvc;
using AdvantShop.Catalog;
using AdvantShop.Saas;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Web.Admin.Controllers.Catalog
{
    //[Auth(RoleAction.Catalog)]
    [ExcludeFilter(typeof(CacheFilterAttribute))]
    public partial class ExportImportCommonController : BaseAdminController
    {        
        public JsonResult GetCommonStatistic()
        {
            return Json(Statistic.CommonStatistic.CurrentData);
        }
        
        public JsonResult InterruptProcess()
        {
            Statistic.CommonStatistic.Break();
            return Json(new { result = true });
        }

        public FileStreamResult GetLogFile()
        {
            FileInfo logFile = new FileInfo(Statistic.CommonStatistic.FileLog);
            if (logFile.Exists)
            {
                return File(logFile.OpenRead(), "text/plain", "logfile.txt");
            }

            return null;
        }
        
        public JsonResult GetSaasBlockInformation()
        {
            var currentSaasData = SaasDataService.CurrentSaasData;
            var productsCount = ProductService.GetProductsCount("[Enabled] = 1");

            return Json(new { productsInTariff = currentSaasData.ProductsCount, productsCount, isSaas = SaasDataService.IsSaasEnabled });
        }
    }
}
