using AdvantShop.Core.Services.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using AdvantShop.Helpers;

namespace AdvantShop.Core.Services.Files
{
    public class FileInDb
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string ContentType { get; set; }
        public string Charset { get; set; }
        public byte[] Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        public FileInDb()
        {

        }

        public FileInDb(HttpPostedFileBase file, string path = "/", string charset = null)
        {
            Name = file.FileName;
            Path = path + Name;

            using (var ms = new MemoryStream())
            {
                file.InputStream.CopyTo(ms);
                Content = ms.ToArray();
            }

            if (!new FileExtensionContentTypeHelper().TryGetContentType(file.FileName, out var contentType))
                contentType = "text/plain";

            ContentType = contentType;

            if (!string.IsNullOrEmpty(charset))
            {
                Charset = charset;
            }
            else if (file.ContentLength < 10_048_576 && contentType.StartsWith("text/"))
            {
                if (new EncodingDetector().TryDetect(Content, out var encoding) 
                    && encoding != null
                    && !string.IsNullOrEmpty(encoding.WebName))
                {
                    Charset = encoding.WebName.ToLower();
                }
            }
        }
    }
}
