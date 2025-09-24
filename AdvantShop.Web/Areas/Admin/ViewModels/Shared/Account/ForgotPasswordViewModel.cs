namespace AdvantShop.Web.Admin.ViewModels.Shared.Account
{
    public enum EForgotPasswordView
    {
        ForgotPassword,
        EmailSent,
        PasswordRecovery,
        RecoveryError,
        PasswordChanged
    }

    public class ForgotPasswordViewModel
    {
        public EForgotPasswordView View { get; set; }
        public string Email { get; set; }
        public string Hash { get; set; }
        public bool FirstVisit { get; set; }
        public bool ShowCaptcha { get; set; }
        public string MainSiteName { get; set; }

    }
}
