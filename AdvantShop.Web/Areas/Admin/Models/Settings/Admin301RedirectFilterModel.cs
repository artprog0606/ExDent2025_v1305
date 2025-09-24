using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Repository;
using System;

namespace AdvantShop.Web.Admin.Models.Settings
{
    public class Admin301RedirectFilterModel : BaseFilterModel
    {
        public string ID { get; set; }

        public string RedirectFrom { get; set; }

        public string RedirectTo { get; set; }

        public string ProductArtNo { get; set; }

        public DateTime? CreatedFrom { get; set; }

        public DateTime? CreatedTo { get; set; }

        public DateTime? EditedFrom { get; set; }

        public DateTime? EditedTo { get; set; }
    }
}