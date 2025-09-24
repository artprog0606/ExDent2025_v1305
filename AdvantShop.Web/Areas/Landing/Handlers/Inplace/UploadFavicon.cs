using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Hosting;
using AdvantShop.App.Landing.Domain;
using AdvantShop.App.Landing.Domain.Settings;
using AdvantShop.App.Landing.Models.Pictures;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;

namespace AdvantShop.App.Landing.Handlers.Inplace
{

    public class UploadFavicon
    {
        private readonly int _lpId;
        private readonly HttpPostedFileBase _file;
        private readonly List<string> _pictureExts = new List<string>() { ".ico", ".png", ".gif" };


        public UploadFavicon(int lpId, HttpPostedFileBase file)
        {
            _lpId = lpId;
            _file = file;
        }

        public UploadPictureResult Execute()
        {
            try
            {
                var lp = new LpService().Get(_lpId);

                var lpFolder = HostingEnvironment.MapPath(string.Format(LpFiles.LpSitePath, lp.LandingSiteId));

                if (!Directory.Exists(lpFolder))
                    Directory.CreateDirectory(lpFolder);

                var ext = Path.GetExtension(_file.FileName);
                if (!_pictureExts.Contains(ext))
                    return new UploadPictureResult() { Error = "wrong file", Result = false };

                if (!string.IsNullOrWhiteSpace(LSiteSettings.Favicon))
                    FileHelpers.DeleteFile(lpFolder + LSiteSettings.Favicon);
                
                var fileName = _file.FileName.FileNamePlusDate("favicon");
                
                _file.SaveAs(lpFolder + fileName);

                LSiteSettings.Favicon = fileName;

                return new UploadPictureResult()
                {
                    Result = true,
                    Picture = LSiteSettings.GetFaviconPath(lp.LandingSiteId)
                };
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                return new UploadPictureResult() { Error = ex.Message };
            }
        }
    }
}
