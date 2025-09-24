namespace AdvantShop.Models.Warehouse
{
    public class WarehouseInfoModel
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Address { get; set; }
        public string[] TimeOfWorkList { get; set; }
        public float? Longitude { get; set; }
        public float? Latitude { get; set; }
        public string AddressComment { get; set; }
    }
}