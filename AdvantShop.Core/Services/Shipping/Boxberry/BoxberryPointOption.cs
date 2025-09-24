using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Orders;
using Newtonsoft.Json;
using AdvantShop.Payment;
using AdvantShop.Core.Common.Extensions;

namespace AdvantShop.Shipping.Boxberry
{
    public class BoxberryPointOption : BaseShippingOption
    {
        public BoxberryPointOption() { }

        public BoxberryPointOption(ShippingMethod method, float preCost)
            : base(method, preCost)
        {
            HideAddressBlock = true;
        }

        public List<BoxberryPoint> ShippingPoints { get; set; }

        public float BasePrice { get; set; }
        public float PriceCash { get; set; }
        public bool OnlyPrepaidOrders { get; set; }

        public override bool IsAvailablePaymentCashOnDelivery => SelectedPoint is BoxberryPoint selectedPoint
                                                                 && selectedPoint.OnlyPrepaidOrders == false;

        public override string TemplateName => "BoxberryOption.html";

        public override void Update(BaseShippingOption option)
        {
            var opt = option as BoxberryPointOption;
            if (opt != null && opt.Id == this.Id)
            {
                var selectedPoint = opt.SelectedPoint != null ? this.ShippingPoints.FirstOrDefault(x => x.Id == opt.SelectedPoint.Id) : null;
                this.SelectedPoint = selectedPoint;
                if (selectedPoint != null)
                {
                    this.Rate = selectedPoint.BasePrice;
                    this.BasePrice = selectedPoint.BasePrice;
                    this.PriceCash = selectedPoint.PriceCash;
                    this.OnlyPrepaidOrders = selectedPoint.OnlyPrepaidOrders;
                }
            }
        }

        public override OrderPickPoint GetOrderPickPoint()
        {
            return new OrderPickPoint
            {
                PickPointId = SelectedPoint?.Id,
                PickPointAddress = SelectedPoint != null ? SelectedPoint.Address : string.Empty,
                AdditionalData = SelectedPoint != null ? JsonConvert.SerializeObject(SelectedPoint) : null
            };
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
            if (orderPickPoint.AdditionalData.IsNotEmpty())
                SelectedPoint = JsonConvert.DeserializeObject<BoxberryPoint>(orderPickPoint.AdditionalData);
        }
    }
}