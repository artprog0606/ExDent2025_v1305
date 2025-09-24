using System;
using System.IO;
using System.Linq;
using System.Web;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.ExportImport.ExportServices.ExportCustomers;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Statistic;
using AdvantShop.Web.Admin.Models.Customers.Export;

namespace AdvantShop.Web.Admin.Handlers.Customers.Export
{
    public class ExportCustomers
    {
        private const string StrFileName = "customers";
        private const string StrFileExt = ".csv";
        
        private readonly ExportCustomersSettings _settings;

        public ExportCustomers(ExportCustomersSettings settings)
        {
            _settings = settings;
        }

        public void Execute()
        {
            if (CommonStatistic.IsRun)
                return;
            
            var folderPath = FoldersHelper.GetPathAbsolut(FolderType.PriceTemp);
            FileHelpers.CreateDirectory(folderPath);

            foreach (var item in Directory.GetFiles(folderPath).Where(f => f.Contains(StrFileName)))
                FileHelpers.DeleteFile(item);

            var fileName = (StrFileName + StrFileExt).FileNamePlusDate();
            var fullPath = folderPath + fileName;
            FileHelpers.CreateDirectory(folderPath);

            
            var ctx = HttpContext.Current;
            var settings = (CustomersExportSettings) _settings;
            
            CommonStatistic.StartNew(() =>
                {
                    try
                    {
                        HttpContext.Current = ctx;
                        new CustomersCsvExportService(settings).Export(fullPath);
                    }
                    catch (Exception ex)
                    {
                        CommonStatistic.WriteLog(ex.Message);
                    }
                },
                "settingscustomers?tab=exportCustomers",
                "Экспорт покупателей",
                UrlService.GetUrl(FoldersHelper.PhotoFoldersPath[FolderType.PriceTemp] + fileName));
        }
    }
}
