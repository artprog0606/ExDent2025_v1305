using System.Collections.Generic;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Orders;
using AdvantShop.Payment;
using Newtonsoft.Json;

namespace AdvantShop.Shipping.DeliveryByZones
{
    public class DeliveryByZonesOption : BaseShippingOption
    {
        public string MessageAtAddress { get; set; }
        public string ZoneDescription { get; set; }
        public decimal[] Point { get; set; }
        public decimal[][] BoundedBy { get; set; }
        public List<int> NotAvailablePayments { get; set; }
        public PointDelivery.MapParams MapParams { get; set; }
        public int? ZoneId { get; set; }
        public List<int> CheckWarehouses { get; set; }
        public float? MinimalOrderPrice { get; set; }

        public DeliveryByZonesOption() { }

        public DeliveryByZonesOption(ShippingMethod method, float preCost) : base(method, preCost) { }

        public override string TemplateName => "DeliveryByZonesOption.html";

        public override OptionValidationResult Validate()
        {
            var result = base.Validate();
            if (!result.IsValid)
                return result;

            if (!string.IsNullOrEmpty(this.MessageAtAddress))
            {
                result.IsValid = false;
                result.ErrorMessage = MessageAtAddress;
                return result;
            }
            
            if (ZoneId is null)
            {
                result.IsValid = false;
                result.ErrorMessage = LocalizationService.GetResource("Admin.ShippingMethods.DeliveryByZones.AddressIsOutsideZones");
                return result;
            }
            
            if (PreCost < MinimalOrderPrice)
            {
                result.IsValid = false;
                result.ErrorMessage =
                    LocalizationService.GetResourceFormat("Admin.ShippingMethods.DeliveryByZones.MinimalOrderPrice",
                        MinimalOrderPrice.Value.FormatPrice(),
                        (MinimalOrderPrice.Value - PreCost).FormatPrice());
                return result;
            }

            return result;
        }

        public override bool AvailablePayment(BasePaymentOption payOption)
        {
            if (NotAvailablePayments != null
                && NotAvailablePayments.Contains(payOption.Id))
                return false;
            
            return base.AvailablePayment(payOption);
        }

        public override OrderPickPoint GetOrderPickPoint()
        {
            if (!ZoneId.HasValue)
                return null;
            return new OrderPickPoint
            {
                PickPointId = ZoneId.ToString(),
                WarehouseIds =
                    CheckWarehouses?.Count > 0
                        ? CheckWarehouses
                        : null
            };
        }
    }
    
    public class FeatureCollection
    {
        [JsonProperty("type")]
        public string Type => "FeatureCollection";

        [JsonProperty("metadata")]
        public MetaData MetaData { get; set; }

        [JsonProperty("features")]
        public List<Feature> Features { get; set; }
    }

    public class MetaData
    {
        [JsonProperty("creator")]
        public string Creator { get; set; }
        
        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class Feature
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("type")]
        public string Type => "Feature";

        [JsonProperty("geometry")]
        public PointGeometry Geometry { get; set; }

        [JsonProperty("properties")]
        public PolygonProperties Properties { get; set; }
    }
    
    
    public class PointGeometry
    {
        [JsonProperty("type")]
        public string Type => "Polygon";

        [JsonProperty("coordinates")]
        public decimal[][,] Coordinates { get; set; }
    }

    public class PolygonProperties
    {
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("fill")]
        public string FillColor { get; set; }

        [JsonProperty("fill-opacity")]
        public float FillOpacity { get; set; }

        [JsonProperty("stroke")]
        public string StrokeColor { get; set; }

        [JsonProperty("stroke-width")]
        public string StrokeWidth { get; set; }

        [JsonProperty("stroke-opacity")]
        public float StrokeOpacity { get; set; }
    }
}