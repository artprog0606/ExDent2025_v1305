using AdvantShop.Shipping.LPost.Api;

namespace AdvantShop.Shipping.LPost.PickPoints
{
    public class LPostPickPoint
    {
        public LPostPickPoint()
        {
            City = string.Empty;
            Address = string.Empty;
        }
        
        public int Id { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string AddressDescription { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public bool Cash { get; set; }
        public bool Card { get; set; }
        public int RegionCode { get; set; }
        public string RegionName { get; set; }
        public bool IsCourier { get; set; }

    }
}