using System;
using System.Web;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Diagnostics;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Web.Admin.Models.Catalog.Import;

namespace AdvantShop.Web.Admin.Handlers.Catalog.Colors
{
    public class UploadColorImagesArchiveFile
    {
        private readonly HttpPostedFileBase _file;
                
        public UploadColorImagesArchiveFile(HttpPostedFileBase file)
        {
            _file = file;           
        }

        public UploadFileResult Execute()
        {
            if (_file == null || string.IsNullOrEmpty(_file.FileName))
            {
                return new UploadFileResult { Result = false, Error = LocalizationService.GetResource("Admin.Import.Errors.FileNotFound") };
            }

            if (!FileHelpers.CheckFileExtensionByType(_file.FileName, EFileType.ZipArchive))
            {             
                return new UploadFileResult { Result = false, Error = LocalizationService.GetResource("Admin.Import.Errors.FileNotFound") };
            }

            FileHelpers.CreateDirectory(FoldersHelper.GetPathAbsolut(FolderType.ImageTemp));
            var fullPath = FoldersHelper.GetPathAbsolut(FolderType.ImageTemp, _file.FileName);

            try
            {                
                FileHelpers.DeleteFilesFromPath(FoldersHelper.GetPathAbsolut(FolderType.ImageTemp));
                _file.SaveAs(fullPath);
                var res = FileHelpers.UnZipFile(fullPath);
                
                if (!res)
                {
                    return new UploadFileResult { Result = false, FilePath = fullPath, Error = "Admin_ImportCsv_ErrorAtUnZip" };                    
                }
            }
            catch (Exception ex)
            {                
                Debug.Log.Error(ex);
                return new UploadFileResult { Result = false, FilePath = fullPath, Error = "Admin_ImportCsv_ErrorAtUploadFile" };
            }            

            FileHelpers.DeleteFile(fullPath);
            
            return new UploadFileResult { Result = true, FilePath = fullPath };
        }
    }
}
