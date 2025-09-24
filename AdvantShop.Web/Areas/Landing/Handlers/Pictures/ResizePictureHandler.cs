using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Hosting;
using AdvantShop.Diagnostics;
using AdvantShop.App.Landing.Domain;
using AdvantShop.App.Landing.Models.Inplace;
using System.Drawing;
using AdvantShop.App.Landing.Models.Pictures;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Helpers;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Configuration;
using AdvantShop.FilePath;

namespace AdvantShop.App.Landing.Handlers.Pictures
{
    public class ResizePictureHandler
    {
        private readonly int _lpId;
        private readonly int _blockId;
        private readonly string _picture;
        private readonly List<PictureParameters> _parameters;
        private readonly int _maxWidth;
        private readonly int _maxHeight;
        private readonly List<string> _pictureExts = new List<string>() { ".jpg", ".jpeg", ".png", ".bmp", ".gif" };


        public ResizePictureHandler(int lpId, int blockId, string picture, int maxWidth, int maxHeight, List<PictureParameters> parameters = null)
        {
            _lpId = lpId;
            _blockId = blockId;
            _picture = picture;
            _parameters = parameters;
            _maxWidth = maxWidth;
            _maxHeight = maxHeight;
        }

        public UploadPictureResult Execute()
        {
            try
            {
                var lp = new LpService().Get(_lpId);

                var landingBlockFolder = HostingEnvironment.MapPath(string.Format(LpFiles.UploadPictureFolderLandingBlock, lp.LandingSiteId, _lpId, _blockId));
                var fileFullPath = landingBlockFolder + _picture;
                
                if (!Directory.Exists(landingBlockFolder) || !File.Exists(fileFullPath))
                    return new UploadPictureResult() { Error = "Не найден файл", Result = false };
                
                var ext = Path.GetExtension(_picture).ToLower();

                if (!_pictureExts.Contains(ext))
                    return new UploadPictureResult() { Error = LocalizationService.GetResource("Admin.Error.InvalidImageFormat"), Result = false };
                
                var fileFullPathTemp = FoldersHelper.GetPathAbsolut(FolderType.ImageTemp) + _picture;
                //File.Copy(fileFullPath, fileFullPathTemp);
                using (var lStream = new FileStream(fileFullPath, FileMode.Open, FileAccess.Read))
                {
                    using (var image = Image.FromStream(lStream))
                    {
                        FileHelpers.SaveResizePhotoFile(fileFullPathTemp, _maxWidth, _maxHeight, image);
                    }
                    
                    lStream.Close();
                }

                File.Delete(fileFullPath);
                
                File.Copy(fileFullPathTemp, fileFullPath);
                
                File.Delete(fileFullPathTemp);
                
                LpSiteService.UpdateModifiedDate(lp.LandingSiteId);

                return new UploadPictureResult()
                {
                    Result = true,
                    Picture = string.Format(LpFiles.UploadPictureFolderLandingBlockRelative + _picture, lp.LandingSiteId, _lpId, _blockId),
                };
            }
            catch (Exception ex)
            {
                Debug.Log.Error("LandingPage module, ResizePictureHandler", ex);
                return new UploadPictureResult() { Error = ex.Message };
            }
        }
    }
}
