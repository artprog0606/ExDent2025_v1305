namespace AdvantShop.Web.Admin.Models.Home
{
    public class AdvReferralModel
    {
        public string ReferralLink { get; set; }
        public string PartnerAccountLink { get; set; }
        public AdvReferralAuthModel Auth { get; set; }
    }

    public class AdvReferralAuthModel
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }
}
