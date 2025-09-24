using AdvantShop.Core.Services.Localization;
using System.Collections.Generic;

namespace AdvantShop.Web.Admin.Models.Settings.Partners
{
    public class PartnersSettingsModel
    {
        public float DefaultRewardPercent { get; set; }
        public int PayoutMinCustomersCount { get; set; }
        public decimal PayoutMinBalance { get; set; }
        public decimal PayoutCommissionNaturalPerson { get; set; }
        public bool AutoApplyPartnerCoupon { get; set; }
        public bool EnableCaptchaInRegistrationPartners { get; set; }
        public bool ActivatePartners { get; set; }
        public string EmailNotification { get; set; }
        public Dictionary<bool, string> ListTypesOption =>
            new Dictionary<bool, string>
            {
                {true, LocalizationService.GetResource("Admin.Settings.Partners.ActivatePartners.Immediately")},
                {false, LocalizationService.GetResource("Admin.Settings.Partners.ActivatePartners.ManualCheck")}
            };

        public bool SkipRegPasportStepForIndividualEntity { get; set; }
    }
}
