using System;
using System.Collections.Generic;
using System.IO;
using AdvantShop.Diagnostics;
using AdvantShop.App.Landing.Models.Pictures;
using System.Net;
using AdvantShop.Core.Services.Localization;

namespace AdvantShop.App.Landing.Handlers.Pictures
{
    public class GetBase64PictureByUrlHandler
    {
        private readonly string _url;
        private readonly List<string> _pictureExts = new List<string>() { ".jpg", ".jpeg", ".png", ".gif" };
        private readonly Dictionary<string, string> mimeTypes = new Dictionary<string, string>()
        {
            {".jpg", "image/jpeg" },
            {".jpeg", "image/jpeg" },
            {".png", "image/png" },
            {".gif", "image/gif" },
        };

        public GetBase64PictureByUrlHandler(string url)
        {
            _url = url;
        }

        public UploadPictureResult Execute()
        {
            try
            {
                if (_url.Contains("http://") || _url.Contains("https://"))
                {
                    var uri = new Uri(_url);
                    var ext = Path.GetExtension(uri.AbsolutePath).ToLower();

                    if (!_pictureExts.Contains(ext))
                        return new UploadPictureResult()
                        {
                            Error = LocalizationService.GetResource("Admin.Error.InvalidImageFormat"),
                            Result = false
                        };

                    byte[] buf;

                    var req = WebRequest.Create(_url) as HttpWebRequest;
                    req.ProtocolVersion = HttpVersion.Version10;

                    using (var response = req.GetResponse())
                    using (var stream = response.GetResponseStream())
                    using (var memoryStream = new MemoryStream())
                    {
                        stream.CopyTo(memoryStream);
                        buf = memoryStream.ToArray();
                    }

                    return new UploadPictureResult()
                    {
                        Result = true,
                        Picture = String.Format("data:{0};base64,{1}", mimeTypes[ext], Convert.ToBase64String(buf, 0, buf.Length)),
                        ProcessedPictures = null
                    };
                }

                return new UploadPictureResult()
                {
                    Error = LocalizationService.GetResource("Core.Helpers.ValidElement.IncorrectUrl")
                };
            }
            catch (WebException ex)
            {
                Debug.Log.Warn(ex);
                return new UploadPictureResult() { Error = ex.Message };
            }
            catch (Exception ex)
            {
                Debug.Log.Error("LandingPage module, GetBase64PictureByUrlHandler", ex);
                return new UploadPictureResult() { Error = ex.Message };
            }
        }
    }
}
