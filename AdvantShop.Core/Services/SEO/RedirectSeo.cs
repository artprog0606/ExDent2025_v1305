//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using AdvantShop.Localization;
using System;

namespace AdvantShop.SEO
{
    public class RedirectSeo
    {
        public int ID { get; set; }

        public string RedirectFrom { get; set; }

        public string RedirectTo { get; set; }

        public string ProductArtNo { get; set; }

        public DateTime Created { get; set; }

        public string CreatedFormatted
        {
            get { return Culture.ConvertDate(Created); }
        }

        public DateTime Edited { get; set; }

        public string EditedFormatted
        {
            get { return Culture.ConvertDate(Edited); }
        }
    }
}