using System.Collections.Generic;

namespace AdvantShop.Web.Admin.Models.Dashboard
{
    public class SiteTemplatesModel
    {
        public List<SiteTemplateModel> LpTemplates { get; set; }
        public List<SiteTemplateModel> Templates { get; set; }
        public int TemplatesTotal { get; set; }
    }

    public class SiteTemplateModel
    {
        public int Id { get; set; }
        public string StringId { get; set; }
        public string Name { get; set; }
        public string Picture { get; set; }
        public float Price { get; set; }
        public string PriceStr { get; set; }

        public string EditLink { get; set; }

        public string DemoLink { get; set; }

        public bool IsPopular { get; set; }

        public string Version { get; set; }
        public bool Active { get; set; }


    }
}
