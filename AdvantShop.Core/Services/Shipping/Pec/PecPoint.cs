namespace AdvantShop.Shipping.Pec
{
    public class PecPoint : BaseShippingPoint
    {

        [Newtonsoft.Json.JsonIgnore]
        public double? MaxVolume { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public double? MaxDimension { get; set; }
    }
}