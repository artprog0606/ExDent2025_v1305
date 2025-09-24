using System.Globalization;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Localization;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Web.Infrastructure.Admin;
using CsvHelper;
using System.IO;
using System.Text;
using AdvantShop.ExportImport;

namespace AdvantShop.Web.Admin.Handlers.Marketing.Analytics.Reports
{
    public class ExportProductOrdersStatistics
    {
        private readonly FilterResult<ProductOrdersStatisticsItemsModel> _productStatisticsList;
        private string _filePath;
        public ExportProductOrdersStatistics(FilterResult<ProductOrdersStatisticsItemsModel> productStaticsList, string fileName) 
        { 
            _productStatisticsList = productStaticsList; 
            var filePath = $"{FoldersHelper.GetPathAbsolut(FolderType.ApplicationTempData)}export\\reports\\";
            FileHelpers.CreateDirectory(filePath);
            _filePath = filePath + fileName.FileNamePlusDate();
        }

        public string Execute()
        {
            if (_productStatisticsList == null || _productStatisticsList.DataItems == null)
                return string.Empty;

            using (var writer = new CsvWriter(new StreamWriter(_filePath, false, Encoding.UTF8), CsvConstants.DefaultCsvConfiguration))
            {
                WriteHeader(writer);

                foreach (var item in _productStatisticsList.DataItems)
                {
                    WriteItem(writer, item);
                }
            }
            return _filePath;
        }

        private void WriteHeader(CsvWriter writer)
        {
            writer.WriteField(LocalizationService.GetResource("Admin.Js.ProductReport.NumberOfOrder"));
            writer.WriteField(LocalizationService.GetResource("Admin.Js.ProductReport.Customer"));
            writer.WriteField("Email");
            writer.WriteField(LocalizationService.GetResource("Admin.Js.ProductReport.Paid"));
            writer.WriteField(LocalizationService.GetResource("Admin.Js.ProductReport.Date"));
            writer.WriteField(LocalizationService.GetResource("Admin.Js.ProductReport.QuantityOfProducts"));

            writer.NextRecord();
        }

        private void WriteItem(CsvWriter writer, ProductOrdersStatisticsItemsModel customer)
        {
            writer.WriteField(customer.OrderId);
            writer.WriteField(customer.BuyerName);
            writer.WriteField(customer.Email);
            writer.WriteField(customer.IsPaid);
            writer.WriteField(customer.OrderDateFormatted);
            writer.WriteField(customer.ProductAmount);

            writer.NextRecord();
        }
    }
}
