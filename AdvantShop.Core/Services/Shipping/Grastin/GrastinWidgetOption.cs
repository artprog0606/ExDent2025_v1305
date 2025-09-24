//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Shipping.Grastin;
using AdvantShop.Orders;
using AdvantShop.Payment;
using AdvantShop.Core.UrlRewriter;
using Newtonsoft.Json;
using AdvantShop.Diagnostics;
using AdvantShop.Core.Common.Extensions;

namespace AdvantShop.Shipping.Grastin
{
    public class GrastinWidgetOption : BaseShippingOption
    {
        private readonly bool _updateRateAndTime;
        public Dictionary<string, string> WidgetConfigData { get; set; }

        public string PickpointId { get; set; }
        public string PickpointAddress { get; set; }
        public string PickpointAdditionalData { get; set; }
        public GrastinEventWidgetData PickpointAdditionalDataObj { get; set; }
        public bool IsAvailableCashOnDelivery { get; set; }
        public override bool IsAvailablePaymentCashOnDelivery { get { return PickpointAdditionalDataObj != null && PickpointAdditionalDataObj.DeliveryType == EnDeliveryType.Courier && IsAvailableCashOnDelivery; } }
        public override bool IsAvailablePaymentPickPoint { get { return PickpointAdditionalDataObj != null && PickpointAdditionalDataObj.DeliveryType == EnDeliveryType.PickPoint && IsAvailableCashOnDelivery; } }
        public float BasePrice { get; set; }
        public float PriceCash { get; set; }

        public GrastinWidgetOption()
        {
        }

        public GrastinWidgetOption(bool updateRateAndTime)
        {
            _updateRateAndTime = updateRateAndTime;
        }

        public GrastinWidgetOption(ShippingMethod method, float preCost)
            : base(method, preCost)
        {
            HideAddressBlock = true;
            Name = method.Name;
        }

        public GrastinWidgetOption(ShippingMethod method, float preCost, bool updateRateAndTime)
            : base(method, preCost)
        {
            HideAddressBlock = true;
            Name = method.Name;
            _updateRateAndTime = updateRateAndTime;
        }

        public override string Id
        {
            get { return MethodId + "_" + MethodId.GetHashCode(); }
        }

        private string PostfixName
        {
            get { return !string.IsNullOrEmpty(NameRate) && (string.IsNullOrEmpty(base.Name) || !base.Name.EndsWith(string.Format(" ({0})", NameRate))) ? string.Format(" ({0})", NameRate) : string.Empty; }
        }

        public new string Name
        {
            get { return base.Name + PostfixName; }
            set { base.Name = !string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(PostfixName) && value.EndsWith(PostfixName) ? value.Replace(PostfixName, string.Empty) : value; }
        }

        public override string TemplateName
        {
            get { return "GrastinWidgetOption.html"; }
        }

        public override void Update(BaseShippingOption option)
        {
            var opt = option as GrastinWidgetOption;
            if (opt != null && opt.MethodId == this.MethodId)
            {
                this.PickpointId = opt.PickpointId;
                this.PickpointAddress = opt.PickpointAddress;
                this.PickpointAdditionalData = opt.PickpointAdditionalData;
                if (_updateRateAndTime)
                {
                    this.Rate = opt.Rate;
                    this.BasePrice = opt.Rate;
                    this.PriceCash = opt.Rate;
                    this.DeliveryTime = opt.DeliveryTime;
                }

                this.NameRate = opt.NameRate;

                if (!string.IsNullOrEmpty(opt.PickpointAdditionalData))
                {
                    try
                    {
                        PickpointAdditionalDataObj =
                            JsonConvert.DeserializeObject<GrastinEventWidgetData>(opt.PickpointAdditionalData);
                    }
                    catch (Exception ex)
                    {
                        Debug.Log.Warn(ex);
                    }

                    HideAddressBlock = PickpointAdditionalDataObj == null ||
                                       (PickpointAdditionalDataObj.DeliveryType == EnDeliveryType.PickPoint &&
                                        PickpointAdditionalDataObj.Partner != EnPartner.RussianPost);
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
                    AdditionalData = PickpointAdditionalData
                }
                : null;
        }

        public override bool ApplyPay(BasePaymentOption payOption)
        {
            var paymentDetails = payOption?.GetDetails();
            if (paymentDetails?.IsCashOnDeliveryPayment is true || paymentDetails?.IsPickPointPayment is true)
                Rate = PriceCash;
            else
            {
                Rate = BasePrice;
            }
            return true;
        }

        public override string GetDescriptionForPayment()
        {
            var diff = PriceCash - BasePrice;
            if (diff <= 0)
                return string.Empty;

            return string.Format("Стоимость доставки увеличится на {0}", diff.FormatPrice());
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
