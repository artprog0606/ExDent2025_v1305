using AdvantShop.Core.Common.Extensions;
using AdvantShop.Shipping;
using AdvantShop.Shipping.ApiShip;

namespace AdvantShop.Core.Services.Shipping.ApiShip
{
    public class ApiShipParamsSendOrder
    {
        public ApiShipParamsSendOrder()
        { }
        public ApiShipParamsSendOrder(ShippingMethod method)
        {
            Name = method.Params.ElementOrDefault(ApiShipTemplate.Name);
            ApiKey = method.Params.ElementOrDefault(ApiShipTemplate.ApiKey);
            SenderName = method.Params.ElementOrDefault(ApiShipTemplate.SenderName);
            SenderPhone = method.Params.ElementOrDefault(ApiShipTemplate.SenderPhone);
            SenderAddress = method.Params.ElementOrDefault(ApiShipTemplate.SenderAddress);
            ReturnAddress = method.Params.ElementOrDefault(ApiShipTemplate.ReturnAddress);
            SendedCountry = int.Parse(method.Params.ElementOrDefault(ApiShipTemplate.SendedCountry) ?? "0");
            ReturnCountry = int.Parse(method.Params.ElementOrDefault(ApiShipTemplate.ReturnCountry) ?? "0");
       }

        public string Name { get; }
        public string ApiKey { get; }
        public string SenderName { get; }
        public string SenderPhone { get; }
        public string SenderAddress { get; }
        public string ReturnAddress { get; }
        public int SendedCountry { get; }
        public int ReturnCountry { get; }
    }
}