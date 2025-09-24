//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Orders;
using AdvantShop.Payment;
using Newtonsoft.Json;

namespace AdvantShop.Shipping.Sberlogistic
{
    public class SberlogisticMapOption : BaseShippingOption, PointDelivery.IPointDeliveryMapOption
    {
        public SberlogisticMapOption()
        {
        }

        public SberlogisticMapOption(ShippingMethod method, float preCost)
            : base(method, preCost)
        {
            HideAddressBlock = true;
        }

        public float BasePrice { get; set; }
        public float PriceCash { get; set; }
        public SberlogisticCalculateOption CalculateOption { get; set; }
        [JsonIgnore]
        public List<SberlogisticShippingPoint> CurrentPoints { get; set; }
        public PointDelivery.MapParams MapParams { get; set; }
        public PointDelivery.PointParams PointParams { get; set; }
        public int YaSelectedPoint { get; set; }
        public string PickpointId { get; set; }

        public override string TemplateName
        {
            get { return "PointDeliveryMapOption.html"; }
        }

        public override void Update(BaseShippingOption option)
        {
            var opt = option as SberlogisticMapOption;
            if (opt != null && opt.Id == this.Id)
            {
                if (this.CurrentPoints != null && this.CurrentPoints.Any(x => x.Id == opt.PickpointId))
                {
                    this.PickpointId = opt.PickpointId;
                    this.YaSelectedPoint = opt.YaSelectedPoint;
                    this.SelectedPoint = this.CurrentPoints.FirstOrDefault(x => x.Id == opt.PickpointId);
                }
                else
                {
                    this.PickpointId = null;
                }
            }
        }

        public override OrderPickPoint GetOrderPickPoint()
        {
            return !string.IsNullOrEmpty(PickpointId)
             ? new OrderPickPoint
             {
                 PickPointId = PickpointId,
                 PickPointAddress = SelectedPoint?.Address,
                 AdditionalData = JsonConvert.SerializeObject(CalculateOption)
             }
             : null;
        }

        public override bool ApplyPay(BasePaymentOption payOption)
        {
            if (payOption != null && payOption.GetDetails()?.IsCashOnDeliveryPayment is true)
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
