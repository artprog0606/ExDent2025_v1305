using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Orders;
using AdvantShop.Payment;
using Newtonsoft.Json;

namespace AdvantShop.Shipping.PecEasyway
{
    public class PecEasywayListOption : BaseShippingOption
    {
        public float BasePrice { get; set; }
        public float PriceCash { get; set; }

        public List<PecEasywayPoint> ShippingPoints { get; set; }
        public PecEasywayCalculateOption CalculateOption { get; set; }

        public PecEasywayListOption()
        {
        }

        public PecEasywayListOption(ShippingMethod method, float preCost)
            : base(method, preCost)
        {
            HideAddressBlock = true;
        }

        public override string TemplateName
        {
            get { return "PecEasywayListOption.html"; }
        }

        public override void Update(BaseShippingOption option)
        {
            var opt = option as PecEasywayListOption;

            if (opt != null && opt.Id == this.Id && opt.ShippingPoints != null)
            {
                this.SelectedPoint = opt.SelectedPoint != null && this.ShippingPoints != null ? this.ShippingPoints.FirstOrDefault(x => x.Id == opt.SelectedPoint.Id) : null;
                this.SelectedPoint = this.SelectedPoint ?? opt.ShippingPoints?.FirstOrDefault();
            }
        }

        public override OrderPickPoint GetOrderPickPoint()
        {
            if (SelectedPoint == null) return null;
            var temp = new OrderPickPoint
            {
                PickPointId = SelectedPoint.Id,
                PickPointAddress = SelectedPoint.Address,
                AdditionalData = JsonConvert.SerializeObject(CalculateOption)
            };
            return temp;
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
}
