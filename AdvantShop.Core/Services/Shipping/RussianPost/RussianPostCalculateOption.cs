using AdvantShop.Shipping.RussianPost.Api;

namespace AdvantShop.Shipping.RussianPost
{
    public class RussianPostCalculateOption
    {
        public string IndexFrom { get; set; }
        public string IndexTo { get; set; }
        public int? CountryCode { get; set; }
        public EnMailType MailType { get; set; }
        public EnMailCategory MailCategory { get; set; }
        public EnTransportType TransportType { get; set; }
        public bool ToOps { get; set; }
    }
}
