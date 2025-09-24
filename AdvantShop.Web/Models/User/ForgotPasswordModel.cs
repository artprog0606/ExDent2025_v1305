namespace AdvantShop.Models.User
{
    public class ForgotPasswordModel
    {
        public string View { get; set; }

        public string Email { get; set; }

        public string RecoveryCode { get; set; }

        public bool ShowCaptcha { get; set; }

        public int? LpId { get; set; }
    }
}