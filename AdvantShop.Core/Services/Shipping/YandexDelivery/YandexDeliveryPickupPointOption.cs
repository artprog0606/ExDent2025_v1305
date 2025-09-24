using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Orders;

namespace AdvantShop.Shipping.ShippingYandexDelivery
{
    public class YandexDeliveryPickupPointOption : BaseShippingOption
    {
        public string WidgetCodeYa { get; set; }
        public string Weight { get; set; }
        public string Dimensions { get; set; }
        public string Cost { get; set; }
        public string Amount { get; set; }

        public string PickpointId { get; set; }
        public string PickpointAddress { get; set; }
        public string PickpointAdditionalData { get; set; }
        public string TariffId { get; set; }

        public bool ShowAssessedValue { get; set; }

        public List<YandexDeliveryListItem> PickPoints { get; set; }

        public YandexDeliveryPickupPointOption()
        {
        }

        public YandexDeliveryPickupPointOption(ShippingMethod method, float preCost, List<YandexDeliveryListItem> items)
            : base(method, preCost)
        {
            HideAddressBlock = true;
            Name = LocalizationService.GetResource("Core.Services.Shipping.ParcelTerminalsDeliveryPoints");
            PickPoints = items;
            IsAvailablePaymentCashOnDelivery = true;
        }

        public override string TemplateName
        {
            get { return "YandexDeliveryPickupPointOption.html"; }
        }

        public override void Update(BaseShippingOption option)
        {
            var opt = option as YandexDeliveryPickupPointOption;
            if (opt != null && opt.Id == this.Id)
            {
                this.PickpointId = opt.PickpointId;
                this.PickpointAddress = opt.PickpointAddress;
                this.PickpointAdditionalData = opt.PickpointAdditionalData;
                this.TariffId = opt.TariffId;

                if (this.PickpointId.IsNotEmpty())
                {
                    IsAvailablePaymentCashOnDelivery &=
                        this.PickPoints.Any(pickPoint =>
                            pickPoint.PickupPoints != null && pickPoint.PickupPoints.Any(pickupPoint =>
                                pickupPoint.id.Equals(this.PickpointId, StringComparison.OrdinalIgnoreCase) &&
                                (pickupPoint.has_payment_card == "1" || pickupPoint.has_payment_cash == "1")));
                }
            }
        }

        public override OrderPickPoint GetOrderPickPoint()
        {
            return new OrderPickPoint
            {
                PickPointId = PickpointId,
                PickPointAddress = PickpointAddress ?? string.Empty,
                AdditionalData = PickpointAdditionalData
            };
        }
    }
}
 