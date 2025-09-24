using AdvantShop.Core.Services.Partners;

namespace AdvantShop.Areas.Partners.ViewModels.Account
{
    public class RegistrationViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string City { get; set; }
        public EPartnerType PartnerType { get; set; }
        public string Password { get; set; }
        public string PasswordConfirm { get; set; }

        public bool Agree { get; set; }
    }
}