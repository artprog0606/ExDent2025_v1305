//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Orders;
using AdvantShop.Payment;
using AdvantShop.Shipping.RussianPost.Api;
using Newtonsoft.Json;

namespace AdvantShop.Shipping.RussianPost
{
    public class RussianPostPointDeliveryMapOption : BaseShippingOption, PointDelivery.IPointDeliveryMapOption
    {
        public RussianPostPointDeliveryMapOption()
        {
        }

        public RussianPostPointDeliveryMapOption(ShippingMethod method, float preCost)
            : base(method, preCost)
        {
            HideAddressBlock = true;
        }
        public RussianPostCalculateOption CalculateOption { get; set; }
        public float BasePrice { get; set; }
        public float PriceCash { get; set; }
        public EnMailCategory CashMailCategory { get; set; }
        public EnMailCategory MailCategory { get; set; }

        [JsonIgnore]
        public List<RussianPostPoint> CurrentPoints { get; set; }
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
            var opt = option as RussianPostPointDeliveryMapOption;
            if (opt != null && opt.Id == this.Id)
            {
                if (this.CurrentPoints != null && this.CurrentPoints.Any(x => x.Id == opt.PickpointId))
                {
                    this.PickpointId = opt.PickpointId;
                    this.YaSelectedPoint = opt.YaSelectedPoint;
                    this.SelectedPoint = this.CurrentPoints.FirstOrDefault(x => x.Id == opt.PickpointId);
                    //if (this.SelectedPoint != null)
                    //    this.IsAvailableCashOnDelivery &=
                    //        this.CurrentPoints.Any(x => (x.Cash || x.Card) && x.Code == opt.PickpointId);
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
                 PickPointAddress = SelectedPoint != null ? SelectedPoint.Address : null,
                 AdditionalData = JsonConvert.SerializeObject(CalculateOption)
             }
             : null;
        }

        public override bool ApplyPay(BasePaymentOption payOption)
        {
            if (payOption?.GetDetails()?.IsCashOnDeliveryPayment is true)
            {
                Rate = PriceCash;
                CalculateOption.MailCategory = CashMailCategory;
            }
            else
            {
                Rate = BasePrice;
                CalculateOption.MailCategory = MailCategory;
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
                SelectedPoint = CurrentPoints?.FirstOrDefault(x => x.Id == orderPickPoint.PickPointId);
                if (SelectedPoint != null)
                    YaSelectedPoint = SelectedPoint.Id.GetHashCode();
            }
        }
    }
}
