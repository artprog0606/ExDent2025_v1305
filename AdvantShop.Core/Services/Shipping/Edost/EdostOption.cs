namespace AdvantShop.Shipping.Edost
{
    public class EdostOption : BaseShippingOption
    {
        public int EdostId { get; private set; }
        public string EdostName { get; private set; }

        public EdostOption()
        {
        }

        public EdostOption(ShippingMethod method, float preCost, EdostTarif tarif)
            : base(method, preCost)
        {
            EdostId = tarif.Id;
            EdostName = tarif.Company + (string.IsNullOrWhiteSpace(tarif.Name) ? string.Empty : " (" + tarif.Name + ")");
            Rate = tarif.Price;
            DeliveryTime = tarif.Day;
            IconName = ShippingIcons.GetShippingIcon(method.ShippingType, method.IconFileName.PhotoName, EdostName);
        }
    }
}
