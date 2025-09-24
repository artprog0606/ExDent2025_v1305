namespace AdvantShop.Core.Services.Catalog.Warehouses
{
    public class StockLabel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ClientName { get; set; }
        public float AmountUpTo { get; set; }
        public string Color { get; set; }
    }
}