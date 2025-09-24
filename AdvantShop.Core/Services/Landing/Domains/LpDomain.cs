namespace AdvantShop.Core.Services.Landing.Domains
{
    public class LpDomain
    {
        public int Id { get; set; }
        public int LandingSiteId { get; set; }
        public string DomainUrl { get; set; }
        public bool IsMain { get; set; }
    }
}
