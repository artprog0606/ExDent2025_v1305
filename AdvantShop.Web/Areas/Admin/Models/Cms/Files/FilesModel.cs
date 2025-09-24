using System;
using AdvantShop.Core.UrlRewriter;
using Newtonsoft.Json;

namespace AdvantShop.Web.Admin.Models.Cms.Files
{
    public class FilesModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        
        [JsonIgnore]
        public byte[] Content { get; set; }
        
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }


        public string CreatedDateFormatted => CreatedDate.ToString("dd.MM.yyyy HH:mm:ss");
        public string ModifiedDateFormatted => ModifiedDate.ToString("dd.MM.yyyy HH:mm:ss");

        public string FileSizeString
        {
            get
            {

                string[] sizes = { "B", "KB", "MB", "GB" };
                double len = Content != null ? Content.Length : 0;
                int order = 0;
                while (len >= 1024 && ++order < sizes.Length)
                {
                    len = len / 1024;
                }
                return string.Format("{0:0.##} {1}", len, sizes[order]);
            }
        }

        public string DownLoadLink => UrlService.GetUrl((Path ?? "").TrimStart('/'));
    }
}
