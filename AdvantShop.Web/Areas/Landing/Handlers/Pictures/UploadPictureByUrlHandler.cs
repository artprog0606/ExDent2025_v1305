using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Hosting;
using AdvantShop.Diagnostics;
using AdvantShop.App.Landing.Domain;
using AdvantShop.App.Landing.Models.Inplace;
using System.Drawing;
using AdvantShop.Helpers;
using AdvantShop.App.Landing.Models.Pictures;
using System.Linq;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Configuration;
using AdvantShop.FilePath;

namespace AdvantShop.App.Landing.Handlers.Pictures
{
    public class UploadPictureByUrlHandler
    {
        private int _lpId;
        private int _blockId;
        private string _url;
        private List<PictureParameters> _parameters;
        private readonly int? _maxWidth;
        private readonly int? _maxHeight;

        public UploadPictureByUrlHandler(int lpId, int blockId, string url, int? maxWidth = null, int? maxHeight = null, List<PictureParameters> parameters = null)
        {
            _lpId = lpId;
            _blockId = blockId;
            _url = url;
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

                if (_url.Contains("http://") || _url.Contains("https://"))
                {
                    if (!FileHelpers.CheckFileExtensionByType(_url, EFileType.Image))
                        return new UploadPictureResult { Error = LocalizationService.GetResource("Admin.Error.InvalidImageFormat"), Result = false };

                    var idPicture = Guid.NewGuid().ToString("N");
                    var ext = Path.GetExtension(_url.Split('?').FirstOrDefault())?.ToLower();
                    var fileName = idPicture + ext;
                    var filePath = landingBlockFolder + fileName;
                    var processedPictures = new Dictionary<string, string>();

                    if (FileHelpers.DownloadRemoteImageFile(_url, filePath))
                    {
                        var fileFullPathTemp = FoldersHelper.GetPathAbsolut(FolderType.ImageTemp) + fileName;
                        using (Image image = Image.FromFile(filePath, true))
                        {
                            if (_maxWidth.HasValue || _maxHeight.HasValue)
                            {
                                FileHelpers.SaveResizePhotoFile(fileFullPathTemp, _maxWidth ?? image.Width, _maxHeight ?? image.Height, image);
                            }

                            if (_parameters != null)
                            {
                                foreach (var item in _parameters)
                                {
                                    var processedPicturesName = idPicture + "_" + item.Postfix + ext;
                                    FileHelpers.SaveResizePhotoFile(landingBlockFolder + processedPicturesName, item.MaxWidth, item.MaxHeight, image);
                                    processedPictures.Add(item.Postfix, string.Format(LpFiles.UploadPictureFolderLandingBlockRelative + processedPicturesName, lp.LandingSiteId, _lpId, _blockId));
                                }
                            }
                        }

                        File.Delete(filePath);
                        
                        File.Copy(fileFullPathTemp, filePath);
                        
                        File.Delete(fileFullPathTemp);
                        
                        LpSiteService.UpdateModifiedDate(lp.LandingSiteId);

                        return new UploadPictureResult
                        {
                            Result = true,
                            Picture = string.Format(LpFiles.UploadPictureFolderLandingBlockRelative + fileName, lp.LandingSiteId, _lpId, _blockId),
                            ProcessedPictures = processedPictures
                        };
                    }
                }

                return new UploadPictureResult
                {
                    Result = false,
                    Error = LocalizationService.GetResource("Core.Helpers.ValidElement.IncorrectUrl")
                };

            }
            catch (Exception ex)
            {
                Debug.Log.Error("LandingPage module, UploadPictureHandler", ex);
                return new UploadPictureResult { Error = ex.Message };
            }
        }
    }
}
