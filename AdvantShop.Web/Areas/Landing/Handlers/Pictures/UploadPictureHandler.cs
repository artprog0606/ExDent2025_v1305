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

namespace AdvantShop.App.Landing.Handlers.Pictures
{
    public class UploadPictureHandler
    {
        private readonly int _lpId;
        private readonly int _blockId;
        private readonly HttpPostedFile _file;
        private readonly List<PictureParameters> _parameters;
        private readonly int? _maxWidth;
        private readonly int? _maxHeight;
        private readonly List<string> _pictureExts = new List<string>() { ".jpg", ".jpeg", ".png", ".bmp", ".gif" };


        public UploadPictureHandler(int lpId, int blockId, HttpPostedFile file, int? maxWidth = null, int? maxHeight = null, List<PictureParameters> parameters = null)
        {
            _lpId = lpId;
            _blockId = blockId;
            _file = file;
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

                if (!Directory.Exists(landingBlockFolder))
                    Directory.CreateDirectory(landingBlockFolder);

                var ext = Path.GetExtension(_file.FileName).ToLower();

                if (!_pictureExts.Contains(ext))
                    return new UploadPictureResult() { Error = LocalizationService.GetResource("Admin.Error.InvalidImageFormat"), Result = false };

                var idPicture = Guid.NewGuid().ToString("N");
                var fileName = idPicture + ext;
                var processedPictures = new Dictionary<string, string>();

                using (Image image = Image.FromStream(_file.InputStream, true))
                {
                    FileHelpers.SaveResizePhotoFile(landingBlockFolder + fileName, _maxWidth.HasValue ?  _maxWidth.Value : image.Width, _maxHeight.HasValue ? _maxHeight.Value : image.Height, image);

                    if (_parameters != null)
                    {
                        string processedPicturesName;

                        foreach (var item in _parameters)
                        {
                            processedPicturesName = idPicture + "_" + item.Postfix + ext;
                            FileHelpers.SaveResizePhotoFile(landingBlockFolder + processedPicturesName, item.MaxWidth, item.MaxHeight, image);
                            processedPictures.Add(item.Postfix, string.Format(LpFiles.UploadPictureFolderLandingBlockRelative + processedPicturesName, lp.LandingSiteId, _lpId, _blockId));
                        }
                    }
                }

                LpSiteService.UpdateModifiedDate(lp.LandingSiteId);

                return new UploadPictureResult()
                {
                    Result = true,
                    Picture = string.Format(LpFiles.UploadPictureFolderLandingBlockRelative + fileName, lp.LandingSiteId, _lpId, _blockId),
                    ProcessedPictures = processedPictures
                };
            }
            catch (Exception ex)
            {
                Debug.Log.Error("LandingPage module, UploadPictureHandler", ex);
                return new UploadPictureResult() { Error = ex.Message };
            }
        }
    }
}
