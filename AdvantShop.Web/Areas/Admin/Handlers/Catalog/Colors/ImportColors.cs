using System.Web;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.ExportImport;
using AdvantShop.FilePath;
using AdvantShop.Helpers;

namespace AdvantShop.Web.Admin.Handlers.Catalog.Colors
{
    public class ImportColors
    {
        #region Ctor

        private readonly HttpPostedFileBase _file;
        private readonly ImportColorSettings _settings;

        public ImportColors(HttpPostedFileBase file, ImportColorSettings settings)
        {
            _file = file;
            _settings = settings;
        }

        #endregion

        public void Execute()
        {
            var filePath = FoldersHelper.GetPathAbsolut(FolderType.PriceTemp);
            var fileName = "colorsImport.csv";
            var fullFileName = filePath + fileName.FileNamePlusDate();

            FileHelpers.CreateDirectory(filePath);

            _file.SaveAs(fullFileName);

            new CsvImportColors(fullFileName, _settings).Process();
        }
    }
}
