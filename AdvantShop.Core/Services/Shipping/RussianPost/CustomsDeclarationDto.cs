namespace AdvantShop.Shipping.RussianPost
{
    public class CustomsDeclarationDto
    {
        public int OrderItemId { get; set; }
        public int? ProductId { get; set; }
        public string Name { get; set; }
        public string ArtNo { get; set; }
        public string CountryCode { get; set; }
        public string TnvedCode { get; set; }

    }
}