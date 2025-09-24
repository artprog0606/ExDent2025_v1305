namespace AdvantShop.Areas.Partners.ViewModels.Account
{
    public class ForgotPasswordViewModel
    {
        public bool ShowRecovery { get; set; }
        public string Email { get; set; }
        public string Hash { get; set; }
        public bool ShowCaptcha { get; set; }
    }
}
