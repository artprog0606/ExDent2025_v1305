namespace AdvantShop.Models.Warehouse
{
    public class CartStockInWarehousesModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool OutStock { get; set; }
        public string AvailableMessage { get; set; }
    }
}