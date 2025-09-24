using System;
using AdvantShop.Diagnostics;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Web.Admin.Models.Catalog.Import;

namespace AdvantShop.Web.Admin.Handlers.Catalog.Colors
{
    public class DeleteColorImagesArchiveFile
    {
        public DeleteColorImagesArchiveFile()
        {
            FileHelpers.CreateDirectory(FoldersHelper.GetPathAbsolut(FolderType.ImageTemp));
        }

        public UploadFileResult Execute()
        {
            try
            {
                FileHelpers.DeleteFilesFromPath(FoldersHelper.GetPathAbsolut(FolderType.ImageTemp));
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                return new UploadFileResult { Result = false, Error = "Admin_ImportCsv_ErrorAtUploadFile" };
            }

            return new UploadFileResult { Result = true };
        }
    }
}
