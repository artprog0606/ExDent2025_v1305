using AdvantShop.Shipping.RussianPost.Api;

namespace AdvantShop.Shipping.RussianPost.PickPoints
{
    public class PickPointRussianPost
    {
        public PickPointRussianPost()
        {
            Region = string.Empty;
            Area = string.Empty;
            City = string.Empty;
            Address = string.Empty;
            BrandName = string.Empty;
            Type = string.Empty;
            WorkTime = string.Empty;
        }
        
        public int Id { get; set; }
        public EnTypePoint TypePoint { get; set; }
        public string Region { get; set; }
        public string Area { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string AddressDescription { get; set; }
        public string BrandName { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public bool? Cash { get; set; }
        public bool? Card { get; set; }
        public string Type { get; set; }
        public string WorkTime { get; set; }
        public float? WeightLimit { get; set; }
        public EnDimensionType DimensionLimit { get; set; }
    }

    public enum EnTypePoint
    {
        Ops,
        Aps,
    }
}