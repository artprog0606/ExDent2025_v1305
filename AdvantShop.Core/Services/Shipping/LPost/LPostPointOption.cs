using System.Collections.Generic;
using AdvantShop.Orders;
using Newtonsoft.Json;
using AdvantShop.Payment;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Common.Extensions;
using System.Linq;

namespace AdvantShop.Shipping.LPost
{
    public class LPostPointOption : BaseShippingOption
    {
        public LPostPointOption() { }

        public LPostPointOption(ShippingMethod method) : base(method) { }

        public LPostPointOption(ShippingMethod method, float preCost)
            : base(method, preCost) 
        {
            HideAddressBlock = true;
        }

        //public override string Id
        //{
        //    get { return MethodId + "_" + MethodId.GetHashCode(); }
        //}

        public float BasePrice { get; set; }

        public float PriceCash { get; set; }

        public List<LPostPoint> ShippingPoints { get; set; }

        public override string TemplateName
        {
            get { return "LPostPointOption.html"; }
        }

        public override OrderPickPoint GetOrderPickPoint()
        {
            return SelectedPoint is null
                ? null
                : new OrderPickPoint
                {
                    PickPointId = SelectedPoint.Id,
                    PickPointAddress = SelectedPoint.Address,
                    AdditionalData = JsonConvert.SerializeObject(SelectedPoint)
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
            if (orderPickPoint.PickPointId.IsNotEmpty())
                SelectedPoint = ShippingPoints?.FirstOrDefault(x => x.Id == orderPickPoint.PickPointId);
        }
    }
}
