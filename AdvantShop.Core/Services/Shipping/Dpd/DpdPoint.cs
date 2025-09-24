using System.Runtime.Serialization;

namespace AdvantShop.Shipping.Dpd
{
    public class DpdPoint : BaseShippingPoint
    {
        [IgnoreDataMember]
        public string Services { get; set; }
        [IgnoreDataMember]
        public string ExtraServices;
    }
}
