//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Shipping.Boxberry;
using AdvantShop.Orders;
using AdvantShop.Payment;
using Newtonsoft.Json;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Core.Common.Extensions;

namespace AdvantShop.Shipping.Boxberry
{
    public class BoxberryWidgetOption : BaseShippingOption
    {
        public Dictionary<string, object> WidgetConfigData { get; set; }

        public string PickpointId { get; set; }
        public string PickpointAddress { get; set; }
        public string PickpointAdditionalData { get; set; }
        public BoxberryObjectPoint PickpointAdditionalDataObj { get; set; }

        public float TotalOrderPrice { get; set; }
        public bool WithInsure { get; set; }
        public float BasePrice { get; set; }
        public float PriceCash { get; set; }

        public BoxberryWidgetOption()
        {
        }

        public BoxberryWidgetOption(ShippingMethod method, float preCost)
            : base(method, preCost)
        {
            HideAddressBlock = true;
            Name = method.Name;
            ShippingType = method.ShippingType;
        }

        public override string Id
        {
            get { return MethodId + "_" + MethodId.GetHashCode(); }
        }
        
        public override string TemplateName => "BoxberryWidgetOption.html";

        public override void Update(BaseShippingOption option)
        {
            var opt = option as BoxberryWidgetOption;
            if (opt != null && opt.Id == this.Id &&
                opt.WidgetConfigData != null && this.WidgetConfigData != null &&
                opt.WidgetConfigData.ContainsKey("custom_city") && this.WidgetConfigData.ContainsKey("custom_city") &&
                (string)opt.WidgetConfigData["custom_city"] == (string)this.WidgetConfigData["custom_city"])
            {
                this.PickpointId = opt.PickpointId;
                this.PickpointAddress = opt.PickpointAddress;
                this.PickpointAdditionalData = opt.PickpointAdditionalData;
                //this.Rate = opt.Rate;
                //this.DeliveryTime = opt.DeliveryTime;
                this.NameRate = opt.NameRate;

                if (!string.IsNullOrEmpty(opt.PickpointAdditionalData))
                {
                    PickpointAdditionalDataObj =
                        JsonConvert.DeserializeObject<BoxberryObjectPoint>(opt.PickpointAdditionalData);
                }

                this.SelectedPoint = new BoxberryPoint
                {
                    Id = opt.PickpointId,
                    Code = opt.PickpointId,
                    Address = opt.PickpointAddress,
                };
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
                if (!WidgetConfigData.ContainsKey("paysum"))
                    WidgetConfigData.Add("paysum", TotalOrderPrice);

                if (!WidgetConfigData.ContainsKey("ordersum"))
                    WidgetConfigData.Add("ordersum", TotalOrderPrice);
                else
                    WidgetConfigData["ordersum"] = TotalOrderPrice;
            }
            else
            {
                Rate = BasePrice;
                if (WidgetConfigData.ContainsKey("paysum"))
                    WidgetConfigData.Remove("paysum");

                if (!WidgetConfigData.ContainsKey("ordersum"))
                    WidgetConfigData.Add("ordersum", WithInsure ? TotalOrderPrice : 0f);
                else
                    WidgetConfigData["ordersum"] = WithInsure ? TotalOrderPrice : 0f;
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
                if (orderPickPoint.AdditionalData.IsNotEmpty())
                {
                    PickpointAdditionalData = orderPickPoint.AdditionalData;
                    PickpointAdditionalDataObj = JsonConvert.DeserializeObject<BoxberryObjectPoint>(orderPickPoint.AdditionalData);
                }
            }
        }
    }
}
