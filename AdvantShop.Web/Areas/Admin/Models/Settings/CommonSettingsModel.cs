using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Localization;

namespace AdvantShop.Web.Admin.Models.Settings
{
    public class CommonSettingsModel : IValidatableObject
    {
        // Common settings
        public string StoreUrl { get; set; }
        public string StoreName { get; set; }
        public string LogoImgSrc { get; set; }
        public string LogoBlogImgSrc { get; set; }
        public string LogoImgAlt { get; set; }
        public string FaviconSrc { get; set; }

        public int CountryId { get; set; }
        public List<SelectListItem> Countries { get; set; }

        public int RegionId { get; set; }
        public List<SelectListItem> Regions { get; set; }
        public bool HasRegions { get; set; }
        
        public string City { get; set; }

        public string Phone { get; set; }

        public string MobilePhone { get; set; }


        // Sales plan settings
        public float SalesPlan { get; set; }
        public float ProfitPlan { get; set; }

        public EnFeedbackAction FeedbackAction { get; set; }
        public List<SelectListItem> FeedbackActions { get; set; }
        
        public bool ShowUserAgreementText { get; set; }
        public bool AgreementDefaultChecked { get; set; }
        public string UserAgreementText { get; set; }
        public bool ShowUserAgreementForPromotionalNewsletter { get; set; }
        public string UserAgreementForPromotionalNewsletter { get; set; }
        public bool SetUserAgreementForPromotionalNewsletterChecked { get; set; }
        public bool ShowCookiesPolicyMessage { get; set; }
        public string CookiesPolicyMessage { get; set; }
        
        // Validation
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (StoreName.IsNullOrEmpty())
            {
                yield return new ValidationResult(LocalizationService.GetResource("Admin.Settings.Index.Error.ShopName"), new[] { "ShopName" });
            }

            if (StoreUrl.IsNullOrEmpty())
            {
                yield return new ValidationResult(LocalizationService.GetResource("Admin.Settings.Index.Error.SiteUrl"), new[] { "SiteUrl" });
            }
        }
    }
}
