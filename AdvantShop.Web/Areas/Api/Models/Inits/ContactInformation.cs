using AdvantShop.Core.Services.Configuration.Settings;

namespace AdvantShop.Areas.Api.Models.Inits
{
    public class ContactInformation
    {
        public string Address { get; }
        public string AddressMap { get; }
        public string Phone { get; }
        public string Email { get; }
        public string WorkSchedule { get; }
        public string AboutCompany { get; set; }

        public ContactInformation()
        {
            Address = SettingsApiAuth.ContactAddress;
            AddressMap = SettingsApiAuth.ContactAddressMap;
            Phone = SettingsApiAuth.ContactPhone;
            Email = SettingsApiAuth.ContactEmail;
            WorkSchedule = SettingsApiAuth.ContactWorkSchedule;
            AboutCompany = SettingsApiAuth.ContactAboutCompany;
        }
    }
}