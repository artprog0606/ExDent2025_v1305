using System.IO;
using System.Web;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Helpers;
using AdvantShop.Web.Admin.Models.Catalog.Import;

namespace AdvantShop.Web.Admin.Handlers.Catalog.Import
{
    public class UploadCsvFileHandler
    {
        private readonly HttpPostedFileBase _file;
        private readonly string _outputFilePath;
               
        public UploadCsvFileHandler(HttpPostedFileBase file, string outputFilePath)
        {
            _file = file;
            _outputFilePath = outputFilePath;
            
            FileHelpers.DeleteFile(outputFilePath);
        }

        public UploadFileResult Execute()
        {
            if (_file == null || string.IsNullOrEmpty(_file.FileName))
            {
                return new UploadFileResult { Result = false, Error = LocalizationService.GetResource("Admin.Import.Errors.FileNotFound") };
            }

            _file.SaveAs(_outputFilePath);

            if (!File.Exists(_outputFilePath))
            {
                return new UploadFileResult { Result = false, Error = LocalizationService.GetResource("Admin.Import.Errors.FileNotFound") };
            }

            return new UploadFileResult { Result = true, FilePath = _outputFilePath };
        }
    }
}
