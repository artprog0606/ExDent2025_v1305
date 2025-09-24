using AdvantShop.Orders;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using AdvantShop.Shipping.ApiShip.Api;

namespace AdvantShop.Shipping.ApiShip
{
    public class ApiShipPointOption : BaseShippingOption
    {
        public ApiShipPointOption() { }
        public ApiShipPointOption(ShippingMethod method, float preCost)
            : base(method, preCost)
        {
            HideAddressBlock = true;
        }
        public override string TemplateName
        {
            get
            {
                return "ApiShipOption.html";
            }
        }
        public List<ApiShipShippingPoint> CurrentPoints { get; set; }
        public string CityTo {  get; set; }
        public string ProviderCode { get; set; }
        public string TariffId {  get; set; }

        public override void Update(BaseShippingOption option)
        {
            var opt = option as ApiShipPointOption;
            if (opt != null && opt.SelectedPoint != null)
            {
                this.SelectedPoint = opt.SelectedPoint;
                if (this.SelectedPoint != null)
                {                    
                    this.IsAvailablePaymentCashOnDelivery = this.SelectedPoint.AvailableCashOnDelivery.HasValue ? this.SelectedPoint.AvailableCashOnDelivery.Value : true;
                }
            }
        }

        public override OrderPickPoint GetOrderPickPoint()
        {
            ApiShipShippingPoint Point = SelectedPoint != null ? CurrentPoints.FirstOrDefault(x => x.Id.ToString() == SelectedPoint?.Id) : CurrentPoints.FirstOrDefault();
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
