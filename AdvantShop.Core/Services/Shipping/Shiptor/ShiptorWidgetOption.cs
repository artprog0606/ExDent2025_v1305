//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;
using AdvantShop.Payment;
using AdvantShop.Core.UrlRewriter;
using Newtonsoft.Json;

namespace AdvantShop.Shipping.Shiptor
{
    public class ShiptorWidgetOption : BaseShippingOption
    {
        public ShiptorWidgetOption()
        {
            HideAddressBlock = true;
        }

        public ShiptorWidgetOption(ShippingMethod method, float preCost)
            : base(method, preCost)
        {
            HideAddressBlock = true;
            Name = method.Name;
        }

        public Dictionary<string, string> WidgetConfigData { get; set; }
        public Dictionary<string, object> WidgetConfigParams { get; set; }
        public string CurrentKladrId { get; set; }
        public string PickpointId { get; set; }
        public string PickpointCompany { get; set; }
        public string PickpointAddress { get; set; }
        public string PickpointAdditionalData { get; set; }
        public ShiptorEventWidgetData PickpointAdditionalDataObj { get; set; }

        public int? PaymentCodCardId { get; set; }
        public float BasePrice { get; set; }
        public float PriceCash { get; set; }
        public bool CashOnDeliveryCardAvailable { get; set; }

        private string PostfixName
        {
            get { return !string.IsNullOrEmpty(PickpointCompany) && (string.IsNullOrEmpty(base.Name) || !base.Name.EndsWith(string.Format(" ({0})", PickpointCompany))) ? string.Format(" ({0})", PickpointCompany) : string.Empty; }
        }

        public new string Name
        {
            get { return base.Name + PostfixName; }
            set { base.Name = !string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(PostfixName) && value.EndsWith(PostfixName) ? value.Replace(PostfixName, string.Empty) : value; }
        }

        public override string TemplateName
        {
            get { return "ShiptorWidgetOption.html"; }
        }

        public override void Update(BaseShippingOption option)
        {
            var opt = option as ShiptorWidgetOption;
            if (opt != null && opt.MethodId == this.MethodId)
            {
                this.PickpointId = opt.PickpointId;
                this.PickpointAddress = opt.PickpointAddress;
                this.PickpointAdditionalData = opt.PickpointAdditionalData;

                if (!string.IsNullOrEmpty(opt.PickpointAdditionalData))
                {
                    try
                    {
                        this.PickpointAdditionalDataObj =
                            JsonConvert.DeserializeObject<ShiptorEventWidgetData>(opt.PickpointAdditionalData);
                    }
                    catch (Exception ex)
                    {
                        Debug.Log.Warn(ex);
                    }
                }

                if (this.PickpointAdditionalDataObj != null && this.PickpointAdditionalDataObj.KladrId != CurrentKladrId)
                {
                    this.PickpointId = null;
                    this.PickpointAddress = null;
                    this.PickpointAdditionalData = null;
                    this.PickpointAdditionalDataObj = null;
                }
            }
        }

        public override OrderPickPoint GetOrderPickPoint()
        {
            return !string.IsNullOrEmpty(PickpointId)
                ? new OrderPickPoint
                {
                    PickPointId = PickpointId,
                    //PickPointAddress = PickpointAddress ?? string.Empty,
                    PickPointAddress =
                        string.Format("{0}{1}{2}",
                            PickpointCompany,
                            PickpointCompany.IsNotEmpty() && PickpointAddress.IsNotEmpty() ? " " : string.Empty,
                            PickpointAddress),
                    AdditionalData = PickpointAdditionalData
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

    public class WidgetConfigParamLocation
    {
        public string kladr_id { get; set; }
    }

    public class WidgetConfigParamDimensions
    {
        public float height { get; set; }
        public float width { get; set; }
        public float length { get; set; }
    }
}
