using AdvantShop.Orders;
using AdvantShop.Shipping;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using AdvantShop.Shipping.ApiShip.Api;
using AdvantShop.Core.Common.Extensions;

namespace AdvantShop.Shipping.ApiShip
{
    public class ApiShipCourierOption : BaseShippingOption
    {
        public ApiShipCourierOption() { }
        public ApiShipCourierOption(ShippingMethod method, float preCost)
            : base(method, preCost)
        {
            HideAddressBlock = true;
            IsAvailablePaymentCashOnDelivery = true;
        }

        public override string TemplateName
        {
            get
            {
                return "ApiShipOption.html";
            }
        }

        public string ProviderKey {  get; set; }
        public string TariffId {  get; set; }
        public override OrderPickPoint GetOrderPickPoint()
        {
            return new OrderPickPoint
            {
                PickPointId = string.Empty,
                PickPointAddress = string.Empty,
                AdditionalData = JsonConvert.SerializeObject(new ApiShipPoint() {
                    Extra = new List<Extra>() { new Extra() { Key = "tariffId", Value = TariffId } },
                    ProviderKey = ProviderKey
                })
            };
        }
    }
}
