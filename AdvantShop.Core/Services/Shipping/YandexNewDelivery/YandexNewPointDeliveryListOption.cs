using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Orders;
using Newtonsoft.Json;

namespace AdvantShop.Shipping.ShippingYandexNewDelivery
{
    public class YandexNewPointDeliveryListOption : BaseShippingOption
    {
        public List<YandexNewPoint> ShippingPoints { get; set; }
        public YandexNewDeliveryAdditionalData PickpointAdditionalData { get; set; }
        public string PickpointCompany { get; set; }

        public YandexNewPointDeliveryListOption() { }
        public YandexNewPointDeliveryListOption(ShippingMethod method, float preCost)
            : base(method, preCost)
        {
        }

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
            get { return "YandexNewPointDeliveryListOption.html"; }
        }

        public override void Update(BaseShippingOption option)
        {
            var opt = option as YandexNewPointDeliveryListOption;

            if (opt != null && opt.Id == this.Id && opt.ShippingPoints != null)
            {
                this.SelectedPoint = opt.SelectedPoint != null && this.ShippingPoints != null ? this.ShippingPoints.FirstOrDefault(x => x.Id == opt.SelectedPoint.Id) : null;
                // this.SelectedPoint = this.SelectedPoint ?? opt.ShippingPoints.FirstOrDefault();
            }
        }

        public override OrderPickPoint GetOrderPickPoint()
        {
            return new OrderPickPoint
            {
                PickPointId = SelectedPoint != null ? SelectedPoint.Id : string.Empty,
                PickPointAddress = SelectedPoint != null 
                    ? string.Format("{0}{1}{2}",
                            PickpointCompany,
                            PickpointCompany.IsNotEmpty() && SelectedPoint.Address.IsNotEmpty() ? " " : string.Empty,
                            SelectedPoint.Address)
                    : string.Empty,
                AdditionalData = JsonConvert.SerializeObject(PickpointAdditionalData)
            };
        }
    }
}
