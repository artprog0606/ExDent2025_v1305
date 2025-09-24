using AdvantShop.DownloadableContent;
using System.Collections.Generic;

namespace AdvantShop.Web.Admin.Models.Dashboard
{
    public class CreateTemplateModel : DownloadableContentObject
    {
        public List<string> Colors { get; set; }

        public string Mode { get; set; }
    }
}