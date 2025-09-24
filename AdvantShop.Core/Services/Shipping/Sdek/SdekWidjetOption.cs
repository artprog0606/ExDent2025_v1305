using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Orders;
using AdvantShop.Payment;
using Newtonsoft.Json;

namespace AdvantShop.Shipping.Sdek
{
    public class SdekWidjetOption : BaseShippingOption
    {
        public float BasePrice { get; set; }
        public float PriceCash { get; set; }
        public string TariffId { get; set; }
        public string PickpointId { get; set; }
        public string PickpointAddress { get; set; }
        public string PickpointAdditionalData { get; set; }
        public bool IgnoreCurrentPoints { get; set; }
        public List<SdekShippingPoint> CurrentPoints { get; set; }
        public Dictionary<string, object> WidgetConfigParams { get; set; }
        public SdekCalculateOption CalculateOption { get; set; }

        public SdekWidjetOption()
        {
        }

        public SdekWidjetOption(ShippingMethod method, float preCost)
            : base(method, preCost)
        {
            HideAddressBlock = true;
            IsAvailablePaymentCashOnDelivery = true;
        }

        public override string TemplateName
        {
            get { return "SdekWidjetOption.html"; }
        }

        public override void Update(BaseShippingOption option)
        {
            var opt = option as SdekWidjetOption;
            if (opt != null && opt.Id == this.Id)
            {
                if (this.IgnoreCurrentPoints || this.CurrentPoints == null || this.CurrentPoints.Any(x => x.Id == opt.PickpointId))
                {
                    this.PickpointId = opt.PickpointId;
                    this.PickpointAddress = opt.PickpointAddress;
                    //this.PickpointAdditionalData = opt.PickpointAdditionalData;
                }
                else
                {
                    this.PickpointId = null;
                    this.PickpointAddress = null;
                    //this.PickpointAdditionalData = null;
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
