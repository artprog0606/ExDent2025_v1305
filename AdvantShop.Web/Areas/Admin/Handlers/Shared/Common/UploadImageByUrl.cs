using System;
using System.IO;
using System.Linq;
using System.Web;
using AdvantShop.Diagnostics;
using AdvantShop.FilePath;
using AdvantShop.Helpers;

namespace AdvantShop.Web.Admin.Handlers.Shared.Common
{
    public class UploadImageByUrl
    {
        private readonly string _url;

        public UploadImageByUrl(string url)
        {
            _url = url;
        }

        public string Execute()
        {
            if (string.IsNullOrWhiteSpace(_url))
                return null;
            
            try
            {
                if (_url.Contains("http://") || _url.Contains("https://"))
                {
                    var uri = new Uri(_url);
                    var photoname = uri.PathAndQuery.Trim('/').Replace("/", "-").Split('?')[0];
                    photoname = HttpUtility.UrlDecode(Path.GetInvalidFileNameChars().Aggregate(photoname, (current, c) => current.Replace(c.ToString(), "")));

                    var filePath = FoldersHelper.GetPathAbsolut(FolderType.ImageTemp, photoname);
                    
                    if (FileHelpers.DownloadRemoteImageFile(_url, filePath))
                    {
                        return FoldersHelper.GetPath(FolderType.ImageTemp, photoname, false);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error("UploadImageByUrl", ex);
            }

            return null;
        }
    }
}
