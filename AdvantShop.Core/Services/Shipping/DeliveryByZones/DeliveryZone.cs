using System.Collections.Generic;

namespace AdvantShop.Shipping.DeliveryByZones
{
    public class DeliveryZone
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float Rate { get; set; }
        public float? CostFree { get; set; }
        public float? MinimalOrderPrice { get; set; }
        public string ZeroPriceMessage { get; set; }
        public string DeliveryTime { get; set; }
        public decimal[][,] Coordinates { get; set; }
        public string FillColor { get; set; }
        public float FillOpacity { get; set; }
        public string StrokeColor { get; set; }
        public string StrokeWidth { get; set; }
        public float StrokeOpacity { get; set; }
        public List<int> NotAvailablePayments { get; set; }
        public List<int> CheckWarehouses { get; set; }
    }
}