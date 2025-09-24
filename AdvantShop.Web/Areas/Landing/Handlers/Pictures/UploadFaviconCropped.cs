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
using AdvantShop.Helpers;
using System.Text.RegularExpressions;
using AdvantShop.App.Landing.Domain.Settings;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.Services.Localization;

namespace AdvantShop.App.Landing.Handlers.Pictures
{
    public class UploadFaviconCropped
    {
        private readonly int _lpId;
        private readonly int? _maxWidth;
        private readonly int? _maxHeight;
        private readonly List<string> _pictureExts = new List<string>() { ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".ico" };
        private readonly string _base64String;
        private readonly string _ext;

        private readonly Dictionary<string, string> mimeTypes = new Dictionary<string, string>()
        {
            {".jpg", "image/jpeg" },
            {".jpeg", "image/jpeg" },
            {".png", "image/png" },
            {".gif", "image/gif" },
            { ".ico", "image/x-icon" },
        };

        public UploadFaviconCropped(int lpId, string base64String, string ext, int? maxWidth = null, int? maxHeight = null)
        {
            _lpId = lpId;
            _maxWidth = maxWidth;
            _maxHeight = maxHeight;
            _base64String = base64String;
            _ext = ext.ToLower();
        }

        public UploadPictureResult Execute()
        {
            var lp = new LpService().Get(_lpId);
            var landingBlockFolder = HostingEnvironment.MapPath(string.Format(LpFiles.LpSitePath, lp.LandingSiteId));

            if (!Directory.Exists(landingBlockFolder))
                Directory.CreateDirectory(landingBlockFolder);

            if (!_pictureExts.Contains(_ext))
                return new UploadPictureResult() { Error = LocalizationService.GetResource("Admin.Error.InvalidImageFormat"), Result = false };


            var idPicture = Guid.NewGuid().ToString("N");
            var fileName = "favicon" + idPicture + _ext;

            try
            {
                var base64 = _base64String.Split(new[] { "base64," }, StringSplitOptions.None)[1];
                var bytes = Convert.FromBase64String(base64);

                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    using (Image image = Image.FromStream(ms, true))
                    {
                        FileHelpers.SaveResizePhotoFile(landingBlockFolder + fileName, _maxWidth.HasValue ? _maxWidth.Value : image.Width, _maxHeight.HasValue ? _maxHeight.Value : image.Height, image);
                    }
                }

                LSiteSettings.Favicon = fileName;

                return new UploadPictureResult()
                {
                    Result = true,
                    Picture = LSiteSettings.GetFaviconPath(lp.LandingSiteId)
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
