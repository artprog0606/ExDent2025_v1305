using AdvantShop.Orders;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using AdvantShop.Shipping.ApiShip.Api;
using System;
using AdvantShop.Shipping.PointDelivery;

namespace AdvantShop.Shipping.ApiShip
{
    public class ApiShipPointDeliveryMapOption : BaseShippingOption, IPointDeliveryMapOption
    {
        public ApiShipPointDeliveryMapOption() { }

        public ApiShipPointDeliveryMapOption(ShippingMethod method, float preCost)
            : base(method, preCost)
        {
        }

        public override string TemplateName
        {
            get
            {
                return "PointDeliveryMapOption.html";
            }
        }

        public string PickpointId { get; set; }
        public string ProviderCode { get; set; }
        public string TariffId {  get; set; }
        public int YaSelectedPoint { get; set; }
        [JsonIgnore]
        public List<ApiShipShippingPoint> CurrentPoints { get; set; }
        public string CityTo {  get; set; }
        public MapParams MapParams { get; set; }
        public PointParams PointParams { get; set; }

        public override void Update(BaseShippingOption option)
        {

            var opt = option as ApiShipPointDeliveryMapOption;

            if (opt != null && opt.Id == this.Id)
            {
                if (this.CityTo == opt.CityTo && this.CurrentPoints != null && this.CurrentPoints.Any(x => x.Id == opt.YaSelectedPoint.ToString()))
                {
                    this.PickpointId = opt.YaSelectedPoint.ToString();
                    this.YaSelectedPoint = opt.YaSelectedPoint;
                    this.SelectedPoint = this.CurrentPoints.FirstOrDefault(x => x.Id == opt.YaSelectedPoint.ToString());
                    if (this.SelectedPoint != null)
                        this.IsAvailablePaymentCashOnDelivery = this.SelectedPoint.AvailableCashOnDelivery.HasValue ? this.SelectedPoint.AvailableCashOnDelivery.Value : true;
                }
                else
                {
                    this.PickpointId = null;
                }
            }
        }

        public override OrderPickPoint GetOrderPickPoint()
        {
            ApiShipShippingPoint Point = SelectedPoint != null 
                ? (ApiShipShippingPoint)SelectedPoint 
                : CurrentPoints != null && CurrentPoints.Count > 0 && SelectedPoint != null && SelectedPoint.Id != null
                    ? CurrentPoints.FirstOrDefault(x => x.Id.ToString() == SelectedPoint?.Id) 
                    : CurrentPoints != null && CurrentPoints.Count > 0 
                        ? CurrentPoints[0]
                        : new ApiShipShippingPoint();
            Point.Extra = new List<Extra>() { new Extra() { Key = "tariffId", Value = TariffId } };
            Point.ProviderKey = ProviderCode;
            return new OrderPickPoint
            {
                PickPointId = Point.Id.ToString(),
                PickPointAddress = Point.Address,
                AdditionalData = JsonConvert.SerializeObject(Point)
            };
        }
    }
}
