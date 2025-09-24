using System;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Localization;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Models.Landings
{
    public class LandingsAdminFilterModel : BaseFilterModel
    {
        public bool? Enabled { get; set; }
        public string CreatedDateFrom { get; set; }
        public string CreatedDateTo { get; set; }
    }

    public class SiteLandingsAdminFilterModel : BaseFilterModel
    {
        public int SiteId { get; set; }
    }

    public class LandingsFilterResult : FilterResult<LandingSiteAdminItemModel>
    {

    }


    public partial class LandingSiteAdminItemModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Enabled { get; set; }

        public string Url
        {
            get
            {
                return LpService.GetTechUrl(LandingSiteUrl, "", true);
            }
        }

        public string DomainUrl { get; set; }
        public string LandingSiteUrl { get; set; }

        public int LandingsCount { get; set; }

        public string LandingsCountStr
        {
            get
            {
                return LandingsCount + " " + Strings.Numerals(LandingsCount, "страниц", "страница", "страницы", "страниц");
            }
        }


        public DateTime CreatedDate { get; set; }
        public string CreatedDateFormatted { get { return Culture.ConvertDate(CreatedDate); } }
    }


    public partial class LandingAdminItemModel
    {
        public int Id { get; set; }
        public int LandingSiteId { get; set; }
        public string Name { get; set; }
        public bool Enabled { get; set; }

        public string DomainUrl { get; set; }
        public string LpUrl { get; set; }
        public string SiteUrl { get; set; }

        public string TechUrl
        {
            get
            {
                return LpService.GetTechUrl(SiteUrl, LpUrl, IsMain, true);
            }
        }

        public string UrlWithDomain
        {
            get
            {
                return !string.IsNullOrEmpty(DomainUrl) ? "http://" + DomainUrl + "/" + (!IsMain ? LpUrl : "") : null;
            }
        }

        public string Template { get; set; }

        public bool IsMain { get; set; }

        public DateTime CreatedDate { get; set; }
        public string CreatedDateFormatted { get { return Culture.ConvertDate(CreatedDate); } }
    }
}
