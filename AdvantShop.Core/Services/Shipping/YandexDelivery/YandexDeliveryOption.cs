using System;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Localization;

namespace AdvantShop.Shipping.ShippingYandexDelivery
{
    public class YandexDeliveryOption : BaseShippingOption
    {
        public string Direction { get; set; }
        public new string DeliveryId { get; set; }
        public string TariffId { get; set; }

        public YandexDeliveryOption()
        {
        }

        public override string Id
        {
            get { return MethodId + "_" + (Name + MethodId + TariffId).GetHashCode(); }
        }

        public YandexDeliveryOption(ShippingMethod method, float preCost, YandexDeliveryListItem item)
            : base(method, preCost)
        {
            Name = item.Delivery.name.ToLower().Contains("почта") ? item.Delivery.name + " (" + item.TariffName + ")" :  LocalizationService.GetResourceFormat("Core.Services.Shipping.ByCourier", item.Delivery.name);
            Rate = item.CostWithRules;
            DeliveryTime = PrepareDeliveryTime(item.Days);
            DeliveryId = item.Delivery.id;
            Direction = item.Direction;
            TariffId = item.TariffId;

            IsAvailablePaymentCashOnDelivery = true;
        }

        private string PrepareDeliveryTime(string time)
        {
            if (string.IsNullOrWhiteSpace(time) || ExtraDeliveryTime == 0)
                return time;

            time = time.Replace(" - ", "-");

            var arr = time.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (arr.Length == 2)
            {
                var days = arr[0].Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);

                if (days.Length == 2 && days[0].TryParseInt() != 0)
                {
                    return string.Format("{0}-{1} {2}", days[0].TryParseInt() + ExtraDeliveryTime, days[1].TryParseInt() + ExtraDeliveryTime, arr[1]);
                }

                if (days.Length == 1 && days[0].TryParseInt() != 0)
                {
                    return string.Format("{0} {1}", days[0].TryParseInt() + ExtraDeliveryTime, arr[1]);
                }
            }

            if (arr.Length == 1)
            {
                var days = arr[0].Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);

                if (days.Length == 2 && days[0].TryParseInt() != 0)
                {
                    return string.Format("{0}-{1} дн.", days[0].TryParseInt() + ExtraDeliveryTime, days[1].TryParseInt() + ExtraDeliveryTime);
                }

                if (days.Length == 1 && days[0].TryParseInt() != 0)
                {
                    return string.Format("{0} дн.", days[0].TryParseInt() + ExtraDeliveryTime);
                }
            }

            return time;
        }
    }
}