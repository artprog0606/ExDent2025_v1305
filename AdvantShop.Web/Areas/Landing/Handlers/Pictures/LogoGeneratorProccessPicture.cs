using System;
using System.Drawing;
using System.IO;
using System.Web.Hosting;
using AdvantShop.App.Landing.Domain;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;

namespace AdvantShop.App.Landing.Handlers.Pictures
{
    public class LogoGeneratorProccessPictureResult
    {
        public string ImgSource { get; set; }
    }

    public class LogoGeneratorProccessPicture
    {
        private readonly string _fileExtension;
        private readonly string _dataUrl;
        private readonly int _lpId;
        private readonly int _blockId;

        public LogoGeneratorProccessPicture(string fileExtension, string dataUrl, int lpId, int blockId)
        {
            _fileExtension = fileExtension;
            _dataUrl = dataUrl;
            _lpId = lpId;
            _blockId = blockId;
        }

        public object Execute()
        {
            try
            {
                var lp = new LpService().Get(_lpId);
                var landingBlockFolder = HostingEnvironment.MapPath(string.Format(LpFiles.UploadPictureFolderLandingBlock, lp.LandingSiteId, _lpId, _blockId));

                if (landingBlockFolder != null && !Directory.Exists(landingBlockFolder))
                    Directory.CreateDirectory(landingBlockFolder);

                if (!FileHelpers.CheckFileExtensionByType(_fileExtension, EFileType.Image))
                    return new LogoGeneratorProccessPictureResult();
                
                var base64 = _dataUrl.Split(new[] {"base64,"}, StringSplitOptions.None)[1];
                var bytes = Convert.FromBase64String(base64);

                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    using (Image image = Image.FromStream(ms, true))
                    {
                        var newFile = "logo_generated".FileNamePlusDate() + FileHelpers.GetExtension(_fileExtension);

                        FileHelpers.SaveResizePhotoFile(landingBlockFolder + newFile, 1200, 500, image);

                        LpSiteService.UpdateModifiedDate(lp.LandingSiteId);

                        return new LogoGeneratorProccessPictureResult()
                        {
                            ImgSource = string.Format(LpFiles.UploadPictureFolderLandingBlockRelative + newFile, lp.LandingSiteId, _lpId, _blockId),
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }
            return new LogoGeneratorProccessPictureResult();
        }
    }
}
