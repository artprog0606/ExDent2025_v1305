namespace AdvantShop.Areas.Api.Model.Funnels
{
    public class SetMainDomainOnFunnelDto
    {
        public string Lickey { get; set; }
        public int FunnelId { get; set; }
        public string Domain { get; set; }
    }
}