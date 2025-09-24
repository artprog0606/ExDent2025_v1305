using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using AdvantShop.ExportImport;
using AdvantShop.FilePath;
using AdvantShop.Web.Admin.Models.Catalog.Brands;
using CsvHelper;
using CsvHelper.Configuration;

namespace AdvantShop.Web.Admin.Handlers.Catalog.Brands
{
    public class ExportBrandsHandler
    {
        private readonly List<AdminBrandModel> _collection;
        private readonly string _fileName;

        public ExportBrandsHandler(AdminBrandsFilterResult filterResult, string fileName)
        {
            if (filterResult == null || filterResult.DataItems == null) return;

            _collection = filterResult.DataItems;
            _fileName = fileName;
        }

        public string Execute()
        {
            if (_collection == null)
                return string.Empty;

            var pathAbsolut = FoldersHelper.GetPathAbsolut(FolderType.PriceTemp);

            using (var writer = new CsvWriter(new StreamWriter(pathAbsolut + _fileName, false, Encoding.UTF8), CsvConstants.DefaultCsvConfiguration))
            {
                WriteHeader(writer);

                foreach (var brand in _collection)
                    WriteItem(writer, brand);
            }

            return pathAbsolut + _fileName;
        }

        private static void WriteHeader(IWriter writer)
        {
            writer.WriteField("название");
            writer.WriteField("описание");
            writer.WriteField("краткое описание");
            writer.WriteField("активность");
            writer.WriteField("синоним для URL запроса");
            writer.WriteField("сайт производителя");
            writer.WriteField("id страны");
            writer.WriteField("название страны");
            writer.WriteField("id страны производства");
            writer.WriteField("название страны производства");
            writer.WriteField("фото");

            writer.NextRecord();
        }

        private static void WriteItem(IWriter writer, AdminBrandModel brand)
        {
            writer.WriteField(brand.BrandName);
            writer.WriteField(brand.Description);
            writer.WriteField(brand.BriefDescription);
            writer.WriteField(brand.Enabled ? "+" : "-");
            writer.WriteField(brand.UrlPath);
            writer.WriteField(brand.BrandSiteUrl);
            writer.WriteField(brand.CountryId);
            writer.WriteField(brand.CountryName);
            writer.WriteField(brand.CountryOfManufactureId);
            writer.WriteField(brand.CountryOfManufactureName);
            writer.WriteField(brand.PhotoName);

            writer.NextRecord();
        }
    }
}
