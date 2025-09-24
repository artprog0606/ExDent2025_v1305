using AdvantShop.Core.Common.Extensions;
using AdvantShop.Orders;

namespace AdvantShop.Shipping.ShippingYandexDelivery
{
    public class YandexDeliveryPickupWarehouseOption : YandexDeliveryOption
    {
        public string ToYDWarehouse { get; set; }
        public string AdditionalData { get; set; }

        public YandexDeliveryPickupWarehouseOption()
        {
        }

        public YandexDeliveryPickupWarehouseOption(ShippingMethod method, float preCost, YandexDeliveryListItem item)
            : base(method, preCost, item)
        {
            ToYDWarehouse = item.Settings.ToYDWarehouse;
            IsAvailablePaymentCashOnDelivery = true;
        }

        public override string Id
        {
            get { return MethodId + "_" + (Name + MethodId + TariffId).GetHashCode(); }
        }

        public override void Update(BaseShippingOption option)
        {
            var opt = option as YandexDeliveryPickupWarehouseOption;
            if (opt != null && opt.Id == this.Id)
            {
                this.AdditionalData = opt.AdditionalData;
            }
        }
        
        public override OrderPickPoint GetOrderPickPoint()
        {
            return new OrderPickPoint
            {
                AdditionalData = ToYDWarehouse.IsNotEmpty() ? "{\"to_ms_warehouse\": \"" + ToYDWarehouse + "\"}" : string.Empty
            };
        }

    }
}