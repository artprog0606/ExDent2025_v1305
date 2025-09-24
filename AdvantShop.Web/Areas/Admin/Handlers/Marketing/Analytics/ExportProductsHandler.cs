using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using AdvantShop.Catalog;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.SQL;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.ExportImport;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Statistic;
using AdvantShop.Web.Admin.ViewModels.Analytics;
using CsvHelper;
using CsvHelper.Configuration;

namespace AdvantShop.Web.Admin.Handlers.Marketing.Analytics
{
    public class ExportProductsHandler
    {
        private readonly ExportProductsModel _settings;
        private readonly string _strFilePath;
        private string _strFullPath;
        private const string StrFileName = "StatisticsProducts";
        private const string StrFileExt = ".csv";
        protected string ExtStrFileName;

        public ExportProductsHandler(ExportProductsModel settings)
        {
            _settings = settings;

            _strFilePath = FoldersHelper.GetPathAbsolut(FolderType.PriceTemp);
            FileHelpers.CreateDirectory(_strFilePath);
        }

        public void Execute()
        {
            if (CommonStatistic.IsRun)
            {
                return;
            }

            foreach (var item in Directory.GetFiles(_strFilePath).Where(f => f.Contains(StrFileName)))
            {
                FileHelpers.DeleteFile(item);
            }


            ExtStrFileName = (StrFileName + StrFileExt).FileNamePlusDate();
            _strFullPath = _strFilePath + ExtStrFileName;
            FileHelpers.CreateDirectory(_strFilePath);

            CommonStatistic.StartNew(() =>
                {
                    try
                    {
                        ExportProducts(_strFullPath);
                    }
                    catch (Exception ex)
                    {
                        CommonStatistic.WriteLog(ex.Message);
                    }
                },
                "analytics?analyticsReportTab=exportProducts",
                "Выгрузка товаров",
                UrlService.GetUrl(FoldersHelper.PhotoFoldersPath[FolderType.PriceTemp] + ExtStrFileName));
        }

        private void ExportProducts(string strFullPath)
        {
            var sqlParameters = new List<SqlParameter>();

            List<int> categories = null;
            if (_settings.ExportProductsType == EExportProductsType.Categories && _settings.SelectedCategories?.Count > 0)
            {
                categories = new List<int>(_settings.SelectedCategories);
                foreach (var categoryId in _settings.SelectedCategories)
                    categories.AddRange(CategoryService.GetChildIDsHierarchical(categoryId));
            }
            var exportOneProduct = _settings.ExportProductsType == EExportProductsType.OneProduct && _settings.OfferId.HasValue;

            string cmd = string.Empty;
            if (!exportOneProduct)
                cmd = string.Format(
                        "SELECT [ProductId], [ArtNo], [Name] " +
                        ",(SELECT ISNULL(SUM([Amount]),0) FROM [Order].[OrderItems] INNER JOIN [Order].[Order] ON [OrderItems].[OrderID] = [Order].[OrderID] WHERE [PaymentDate] IS NOT NULL{0}{1} AND [OrderItems].[ProductID]=[Product].[ProductId]) AS Count " +
                        ",(SELECT ISNULL(SUM([Amount]*[Price]),0) FROM [Order].[OrderItems] INNER JOIN [Order].[Order] ON [OrderItems].[OrderID] = [Order].[OrderID] WHERE [PaymentDate] IS NOT NULL{0}{1} AND [OrderItems].[ProductID]=[Product].[ProductId]) AS Sum " +
                        "FROM [Catalog].[Product] " + 
                        "WHERE [Product].ProductId in (Select ProductId FROM [Order].[OrderItems] INNER JOIN [Order].[Order] ON [OrderItems].[OrderID] = [Order].[OrderID] WHERE [PaymentDate] IS NOT NULL{0}{1}) " +
                        "{2}",
                        _settings.DateFrom != DateTime.MinValue 
                            ? " AND [OrderDate] >= @DateFrom" 
                            : string.Empty,
                        _settings.DateTo != DateTime.MinValue 
                            ? " AND [OrderDate] <= @DateTo" 
                            : string.Empty,
                        categories?.Count > 0 
                            ? "AND [ProductID] in (select ProductCategories.ProductId from [Catalog].[ProductCategories] Where [ProductCategories].[CategoryID] in (" +
                          string.Join(",", categories.Distinct()) + "))" 
                            : string.Empty);
            else
                cmd = string.Format("SELECT [Product].[ProductId], [Offer].[ArtNo], [Name] " +
                                    ",(SELECT ISNULL(SUM([Amount]),0) FROM [Order].[OrderItems] INNER JOIN [Order].[Order] ON [OrderItems].[OrderID] = [Order].[OrderID] WHERE [PaymentDate] IS NOT NULL{0}{1} AND [OrderItems].[ArtNo]=[Offer].[ArtNo]) AS Count " +
                                    ",(SELECT ISNULL(SUM([Amount]*[Price]),0) FROM [Order].[OrderItems] INNER JOIN [Order].[Order] ON [OrderItems].[OrderID] = [Order].[OrderID] WHERE [PaymentDate] IS NOT NULL{0}{1} AND [OrderItems].[ArtNo]=[Offer].[ArtNo]) AS Sum " +
                                    "FROM [Catalog].[Product] " +
                                    "LEFT JOIN [Catalog].[Offer] ON [Offer].ProductID = [Product].ProductId " +
                                    "WHERE [Product].ProductId in (Select ProductId FROM [Order].[OrderItems] INNER JOIN [Order].[Order] ON [OrderItems].[OrderID] = [Order].[OrderID] WHERE [PaymentDate] IS NOT NULL) " +
                                    "AND [Offer].[OfferID]=@OfferId",
                    _settings.DateFrom != DateTime.MinValue ? " AND [OrderDate] >= @DateFrom" : "",
                    _settings.DateTo != DateTime.MinValue ? " AND [OrderDate] <= @DateTo" : "");

            if (exportOneProduct)
                sqlParameters.Add(new SqlParameter("@OfferId", _settings.OfferId.Value));
            if (_settings.DateFrom != DateTime.MinValue)
                sqlParameters.Add(new SqlParameter("@DateFrom", _settings.DateFrom));
            if (_settings.DateTo != DateTime.MinValue)
                sqlParameters.Add(new SqlParameter("@DateTo", _settings.DateTo));

            var data = SQLDataAccess.ExecuteTable(cmd, CommandType.Text, sqlParameters.ToArray());

            CommonStatistic.TotalRow = data.Rows.Count;

            using (var writer = InitWriter(strFullPath, _settings.Encoding, _settings.ColumnSeparator))
            {
                var columns = new[]
                {
                    LocalizationService.GetResource("Admin.ExportField.ArtNo"),
                    LocalizationService.GetResource("Admin.ExportField.ProductName"),
                    LocalizationService.GetResource("Admin.ExportField.ProductSoldAmount"),
                    LocalizationService.GetResource("Admin.ExportField.Sum")
                };

                foreach (var item in columns)
                    writer.WriteField(item);

                writer.NextRecord();

                for (int row = 0; row < data.Rows.Count; row++)
                {
                    if (!CommonStatistic.IsRun || CommonStatistic.IsBreaking) return;

                    writer.WriteField(data.Rows[row]["ArtNo"]);
                    writer.WriteField(data.Rows[row]["Name"]);
                    writer.WriteField(SQLDataHelper.GetInt(data.Rows[row]["Count"]).ToString());
                    writer.WriteField(SQLDataHelper.GetFloat(data.Rows[row]["Sum"]).ToString("F2"));

                    writer.NextRecord();

                    CommonStatistic.RowPosition++;
                }
            }
        }


        private CsvWriter InitWriter(string strFullPath, string csvEnconing, string csvSeparator)
        {
            var csvConfiguration = CsvConstants.DefaultCsvConfiguration;
            csvConfiguration.Delimiter = csvSeparator;
            var writer = new CsvWriter(new StreamWriter(strFullPath, false, Encoding.GetEncoding(csvEnconing)), csvConfiguration);
            return writer;
        }
    }
}
