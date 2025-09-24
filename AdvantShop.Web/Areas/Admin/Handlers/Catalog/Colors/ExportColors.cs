using System;
using System.Globalization;
using System.IO;
using System.Text;
using AdvantShop.Catalog;
using AdvantShop.Diagnostics;
using AdvantShop.ExportImport;
using AdvantShop.FilePath;
using CsvHelper.Configuration;

namespace AdvantShop.Web.Admin.Handlers.Catalog.Colors
{
    public class ExportColors
    {
        private readonly string _fileName;

        public ExportColors(string fileName)
        {
            _fileName = fileName;
        }

        public string Execute()
        {
            var fileDirectory = FoldersHelper.GetPathAbsolut(FolderType.PriceTemp);
            if (File.Exists(fileDirectory + _fileName))
                File.Delete(fileDirectory + _fileName);

            if (!Directory.Exists(fileDirectory))
            {
                Directory.CreateDirectory(fileDirectory);
            }

            try
            {
                using (
                    var csvWriter = new CsvHelper.CsvWriter(new StreamWriter(fileDirectory + _fileName, false, Encoding.UTF8),
                        CsvConstants.DefaultCsvConfiguration))
                {
                    foreach (var item in new[] {"Name", "Code", "Photo", "SortOrder"})
                        csvWriter.WriteField(item);

                    csvWriter.NextRecord();

                    foreach (var color in ColorService.GetAllColors())
                    {
                        csvWriter.WriteField(color.ColorName);
                        csvWriter.WriteField(color.ColorCode);
                        csvWriter.WriteField(color.IconFileName.PhotoName);
                        csvWriter.WriteField(color.SortOrder);

                        csvWriter.NextRecord();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                return null;
            }

            return fileDirectory + _fileName;
        }
    }
}
