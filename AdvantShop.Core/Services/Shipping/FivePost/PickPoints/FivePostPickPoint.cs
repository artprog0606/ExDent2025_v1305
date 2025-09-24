using AdvantShop.Shipping.FivePost.Api;
using System.Collections.Generic;

namespace AdvantShop.Shipping.FivePost.PickPoints
{
    public class FivePostPickPoint
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public EFivePostPickPointType Type { get; set; }
        public string Description { get; set; }
        public FivePostWeightDimension WeightDimensionsLimit { get; set; }
        public bool ReturnAllowed { get; set; }
        public bool IsCash { get; set; }
        public bool IsCard { get; set; }
        public string FiasCode { get; set; }
        public List<FivePostRate> RateList { get; set; }
        public List<FivePostPossibleDelivery> PossibleDeliveryList { get; set; }
        public string FullAddress { get; set; }
        public string RegionName { get; set; }
        public string CityName { get; set; }
        public float Lattitude { get; set; }
        public float Longitude { get; set; }
        public string Phone { get; set; }
        public string TimeWork { get; set; }
    }
}
