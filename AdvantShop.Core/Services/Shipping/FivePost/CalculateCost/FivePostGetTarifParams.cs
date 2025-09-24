using AdvantShop.Shipping.FivePost.Api;
using System.Collections.Generic;

namespace AdvantShop.Core.Services.Shipping.FivePost.CalculateCost
{
    public class FivePostGetTarifParams
    {
        public float Weight { get; set; }
        public List<FivePostRate> RateList { get; set; }
        public List<FivePostPossibleDelivery> PossibleDeliveryList { get; set; }
        public List<int> ActiveTarifTypes { get; set; }
        public Dictionary<string, int?> WarehouseDeliveryTypeReference { get; set; }
        public Dictionary<int, List<EFivePostDeliveryType>> RateDeliverySLReference { get; set; }
    }

    public class FivePostGetTarifResult
    {
        public FivePostRate Rate { get; set; }
        public FivePostPossibleDelivery PossibleDelivery { get; set; }
        public string WarehouseId { get; set; }
    }
}
