namespace AdvantShop.Areas.Partners.Models.Account
{
    public class ForgotPasswordModel
    {
        public string Email { get; set; }
        public string Hash { get; set; }
        public string CaptchaCode { get; set; }
        public string CaptchaSource { get; set; }
        public string NewPassword { get; set; }
        public string NewPasswordConfirm { get; set; }
    }
}
