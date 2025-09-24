//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using AdvantShop.Core.Services.Catalog;
using AdvantShop.Orders;
using AdvantShop.Payment;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Core.Services.Shipping.DDelivery;
using Newtonsoft.Json;
using AdvantShop.Core.Common.Extensions;

namespace AdvantShop.Shipping.DDelivery
{
    public class DDeliveryWidgetOption : BaseShippingOption
    {
        public DDeliveryCartWidgetObject WidgetConfigData { get; set; }

        public string PickpointId { get; set; }
        public string PickpointAddress { get; set; }
        public string PickpointAdditionalData { get; set; }

        private DDeliveryWidgetAdditionalDataObject _pickpointAdditionalDataObj;
        public DDeliveryWidgetAdditionalDataObject PickpointAdditionalDataObj
        {
            get
            {
                return _pickpointAdditionalDataObj ??
                       (_pickpointAdditionalDataObj =
                           !string.IsNullOrEmpty(PickpointAdditionalData)
                               ? JsonConvert.DeserializeObject<DDeliveryWidgetAdditionalDataObject>(
                                   PickpointAdditionalData)
                               : null);
            }
        }

        public float BasePrice { get; set; }
        public float PriceCash { get; set; }

        public DDeliveryWidgetOption()
        {
        }

        public DDeliveryWidgetOption(ShippingMethod method, float preCost)
            : base(method, preCost)
        {
            HideAddressBlock = true;
            Name = method.Name;
            ShippingType = method.ShippingType;
            IsAvailablePaymentCashOnDelivery = true;
        }

        public override string Id
        {
            get { return MethodId + "_" + MethodId.GetHashCode(); }
        }
       

        public override string TemplateName
        {
            get { return "DdeliveryWidgetOption.html"; }
        }

        public override void Update(BaseShippingOption option)
        {
            var opt = option as DDeliveryWidgetOption;
            if (opt != null && opt.MethodId == this.MethodId)
            {
                this.PickpointId = opt.PickpointId;
                this.PickpointAddress = opt.PickpointAddress;
                this.PickpointAdditionalData = opt.PickpointAdditionalData;
            }
        }

        public override OrderPickPoint GetOrderPickPoint()
        {
            return !string.IsNullOrEmpty(PickpointId)
                ? new OrderPickPoint
                {
                    PickPointId = PickpointId,
                    PickPointAddress = PickpointAddress ?? string.Empty,
                    AdditionalData = PickpointAdditionalData
                }
                : null;
        }

        public override bool ApplyPay(BasePaymentOption payOption)
        {
            if (payOption?.GetDetails()?.IsCashOnDeliveryPayment is true)
            {
                Rate = PriceCash;
                WidgetConfigData.NppOption = true;
            }
            else
            {
                Rate = BasePrice;
                WidgetConfigData.NppOption = false;
            }
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
