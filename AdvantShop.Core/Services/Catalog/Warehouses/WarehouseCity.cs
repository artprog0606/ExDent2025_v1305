namespace AdvantShop.Core.Services.Catalog.Warehouses
{
    public sealed class WarehouseCity
    {
        public int WarehouseId { get; set; }
        public int CityId { get; set; }
        public int SortOrder { get; set; }
    }

    public sealed class CityOfWarehouse
    {
        public int CityId { get; set; }
        public string CityName { get; set; }
    }
    
    public sealed class WarehouseByCity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}