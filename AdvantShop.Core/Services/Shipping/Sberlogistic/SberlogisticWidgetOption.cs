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

namespace AdvantShop.Shipping.Sberlogistic
{
    public class SberlogisticWidgetOption : BaseShippingOption
    {
        public List<BaseShippingPoint> ShippingPoints { get; set; }
        public SberlogisticCalculateOption CalculateOption { get; set; }

        public SberlogisticWidgetOption()
        {
            HideAddressBlock = true;
        }

        public SberlogisticWidgetOption(ShippingMethod method, float preCost)
            : base(method, preCost)
        {
            HideAddressBlock = true;
            Name = method.Name;
        }
        public string CurrentKladrId { get; set; }
        public string PickpointId { get; set; }
        public string PickpointCompany { get; set; }
        public string PickpointAddress { get; set; }
        public string PickpointAdditionalData { get; set; }
        public SberlogisticEventWidgetData PickpointAdditionalDataObj { get; set; }

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

        public Dictionary<string, string> WidgetConfigData { get; set; }
        public Dictionary<string, object> WidgetConfigParams { get; set; }

        public override string TemplateName
        {
            get { return "SberlogisticWidjetOption.html"; }
        }


        public override void Update(BaseShippingOption option)
        {
            var opt = option as SberlogisticWidgetOption;
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
                            JsonConvert.DeserializeObject<SberlogisticEventWidgetData>(opt.PickpointAdditionalData);
                    }
                    catch (Exception ex)
                    {
                        Debug.Log.Warn(ex);
                    }
                }

                if (this.PickpointAdditionalDataObj != null && !CurrentKladrId.Contains(this.PickpointAdditionalDataObj.KladrId))
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
                    PickPointAddress =
                        string.Format("{0}{1}{2}",
                            PickpointCompany,
                            PickpointCompany.IsNotEmpty() && PickpointAddress.IsNotEmpty() ? " " : string.Empty,
                            PickpointAddress),
                    AdditionalData = PickpointAdditionalData
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