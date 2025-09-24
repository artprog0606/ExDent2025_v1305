//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Orders;
using AdvantShop.Shipping.PointDelivery;
using Newtonsoft.Json;

namespace AdvantShop.Shipping.ShippingYandexNewDelivery
{
    public class YandexNewPointDeliveryMapOption : BaseShippingOption, IPointDeliveryMapOption
    {
        public YandexNewPointDeliveryMapOption()
        {
        }

        public YandexNewPointDeliveryMapOption(ShippingMethod method, float preCost)
            : base(method, preCost)
        {
            HideAddressBlock = true;
        }

        [JsonIgnore]
        public List<YandexNewPoint> CurrentPoints { get; set; }
        public MapParams MapParams { get; set; }
        public PointParams PointParams { get; set; }
        public int YaSelectedPoint { get; set; }
        public string PickpointId { get; set; }
        public string PickpointCompany { get; set; }
        public YandexNewDeliveryAdditionalData PickpointAdditionalData { get; set; }

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
            get { return "PointDeliveryMapOption.html"; }
        }

        public override void Update(BaseShippingOption option)
        {
            var opt = option as YandexNewPointDeliveryMapOption;
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
                 PickPointAddress = SelectedPoint != null 
                    ? string.Format("{0}{1}{2}",
                            PickpointCompany,
                            PickpointCompany.IsNotEmpty() && SelectedPoint.Address.IsNotEmpty() ? " " : string.Empty,
                            SelectedPoint.Address)
                    : null,
                 AdditionalData = JsonConvert.SerializeObject(PickpointAdditionalData)
             }
             : null;
        }
    }
}
