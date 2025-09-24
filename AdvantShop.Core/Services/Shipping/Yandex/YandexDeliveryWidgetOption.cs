using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Orders;
using AdvantShop.Payment;
using Newtonsoft.Json;

namespace AdvantShop.Shipping.Yandex
{
    public class YandexDeliveryWidgetOption : BaseShippingOption
    {
        public float BasePrice { get; set; }
        public float PriceCash { get; set; }
        public string PickpointId { get; set; }
        public string PickpointAddress { get; set; }
        [JsonIgnore]
        public List<YandexDeliveryShippingPoint> CurrentPoints { get; set; }
        public YandexDeliveryCalculateOption CalculateOption { get; set; }
        public Dictionary<string, object> WidgetConfigParams { get; set; }
        public int? PaymentCodCardId { get; set; }
        public bool CashOnDeliveryCardAvailable { get; set; }
        public bool CashOnDeliveryCashAvailable { get; set; }
        public override bool IsAvailablePaymentCashOnDelivery => CashOnDeliveryCardAvailable || CashOnDeliveryCashAvailable;

        public YandexDeliveryWidgetOption()
        {
            HideAddressBlock = true;
        }

        public YandexDeliveryWidgetOption(ShippingMethod method, float preCost)
            : base(method, preCost)
        {
            HideAddressBlock = true;
        }

        public override string TemplateName
        {
            get { return "YandexDeliveryWidgetOption.html"; }
        }

        public override void Update(BaseShippingOption option)
        {
            var opt = option as YandexDeliveryWidgetOption;

            if (opt != null && opt.Id == Id)
            {
                if (CurrentPoints == null || CurrentPoints.Any(x => x.Id == opt.PickpointId))
                {
                    PickpointId = opt.PickpointId;
                    PickpointAddress = opt.PickpointAddress;
                }
                else
                {
                    PickpointId = null;
                    PickpointAddress = null;
                }
            }
        }

        public override OrderPickPoint GetOrderPickPoint()
        {
            return !string.IsNullOrEmpty(PickpointId)
                ? new OrderPickPoint
                {
                    PickPointId = PickpointId,
                    PickPointAddress = PickpointAddress ?? string.Empty,
                    AdditionalData = JsonConvert.SerializeObject(CalculateOption)
                }
                : null;
        }

        public override bool AvailablePayment(BasePaymentOption payOption)
        {
            if (payOption.GetDetails()?.IsCashOnDeliveryPayment is true)
                return IsAvailablePaymentCashOnDelivery &&
                    (PaymentCodCardId != payOption.Id || CashOnDeliveryCardAvailable);

            return base.AvailablePayment(payOption);
        }

        public override bool ApplyPay(BasePaymentOption payOption)
        {
            if (payOption?.GetDetails()?.IsCashOnDeliveryPayment is true)
                Rate = PriceCash;
            else
                Rate = BasePrice;
            return true;
        }

        public override string GetDescriptionForPayment()
        {
            var diff = PriceCash - BasePrice;
            if (diff <= 0)
                return string.Empty;

            return string.Format("Стоимость доставки увеличится на {0}", diff.RoundPrice().FormatPrice());
        }

        public override void UpdateFromOrderPickPoint(OrderPickPoint orderPickPoint)
        {
            if (orderPickPoint.PickPointId.IsNotEmpty())
            {
                PickpointId = orderPickPoint.PickPointId;
                PickpointAddress = orderPickPoint.PickPointAddress;
            }
        }
    }
}
