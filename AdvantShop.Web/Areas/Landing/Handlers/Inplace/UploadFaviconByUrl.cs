using System;
using System.IO;
using System.Web.Hosting;
using AdvantShop.App.Landing.Domain.Settings;
using AdvantShop.App.Landing.Models.Pictures;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;

namespace AdvantShop.App.Landing.Handlers.Pictures
{
    public class UploadFaviconByUrl
    {
        private readonly int _lpId;
        private readonly string _url;

        public UploadFaviconByUrl(int lpId, string url)
        {
            _lpId = lpId;
            _url = url;
        }

        public UploadPictureResult Execute()
        {
            try
            {
                var lp = new LpService().Get(_lpId);
                var lpFolder = HostingEnvironment.MapPath(string.Format(LpFiles.LpSitePath, lp.LandingSiteId));

                if (!Directory.Exists(lpFolder))
                    Directory.CreateDirectory(lpFolder);

                if (string.IsNullOrEmpty(_url) || !_url.Contains("http://") && !_url.Contains("https://"))
                    return new UploadPictureResult {Error = "Wrong url for favicon"};


                var fileName = _url.Substring(_url.LastIndexOf("/") + 1);
                var newFileName = fileName.FileNamePlusDate("favicon");

                if (!FileHelpers.CheckFileExtensionByType(fileName, EFileType.Favicon))
                    return new UploadPictureResult {Error = "Wrong url for favicon"};

                var photoFullName = lpFolder + newFileName;

                if (FileHelpers.DownloadRemoteImageFile(_url, photoFullName))
                {
                    FileHelpers.DeleteFile(lpFolder + LSiteSettings.Favicon);

                    LSiteSettings.Favicon = newFileName;

                    return new UploadPictureResult
                    {
                        Result = true,
                        Picture = LSiteSettings.GetFaviconPath(lp.LandingSiteId)
                    };
                }
                
                return new UploadPictureResult
                {
                    Result = false,
                    Error = "Wrong download url for landing image"
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
