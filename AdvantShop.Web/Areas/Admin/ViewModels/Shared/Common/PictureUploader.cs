using System.Collections.Generic;
using AdvantShop.Catalog;

namespace AdvantShop.Web.Admin.ViewModels.Shared.Common
{
    public class PictureUploader
    {
        public PhotoType PhotoType { get; set; }
        public int ObjId { get; set; }
        public string StartSrc { get; set; }
        public string UploadUrl { get; set; }
        public string DeleteUrl { get; set; }
        public string UploadByLinkUrl { get; set; }
        public string NgOnUpdateCallback { get; set; }
        public string HtmlAttributes { get; set; }
        public int? PictureId { get; set; }
        public HashSet<string> FileTypes { get; set; }
    }
}