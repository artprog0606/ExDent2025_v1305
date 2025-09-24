//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Shipping.Grastin;
using AdvantShop.Orders;
using AdvantShop.Payment;
using AdvantShop.Core.UrlRewriter;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Extensions;

namespace AdvantShop.Shipping.Grastin
{
    public class GrastinPointDeliveryMapOption : BaseShippingOption, PointDelivery.IPointDeliveryMapOption
    {
        public GrastinPointDeliveryMapOption()
        {
        }

        public GrastinPointDeliveryMapOption(ShippingMethod method, float preCost)
            : base(method, preCost)
        {
            HideAddressBlock = true;
        }

        public PointDelivery.MapParams MapParams { get; set; }
        public PointDelivery.PointParams PointParams { get; set; }
        public int YaSelectedPoint { get; set; }

        private string _pickpointId;
        public string PickpointId
        {
            get
            {
                return _pickpointId;
            }
            set
            {
                _pickpointId = value;
                _pickpointAdditionalDataObj = null;
            }
        }

        private GrastinEventWidgetData _pickpointAdditionalDataObj;
        [JsonIgnore]
        public GrastinEventWidgetData PickpointAdditionalDataObj
        {
            get
            {
                if (_pickpointAdditionalDataObj == null && !string.IsNullOrEmpty(PickpointId) && PickpointId.Contains("#"))
                {
                    _pickpointAdditionalDataObj = new GrastinEventWidgetData
                    {
                        CityFrom = CityFrom,
                        CityTo = CityTo,
                        PickPointId = PickpointId.Split('#')[1],
                        DeliveryType = EnDeliveryType.PickPoint
                    };
                    _pickpointAdditionalDataObj.Partner = (EnPartner)Enum.Parse(typeof(EnPartner), PickpointId.Split('#')[0], true);
                }


                return _pickpointAdditionalDataObj;
            }
        }

        [JsonIgnore]
        public List<GrastinPoint> GrastinPoints { get; set; }

        [JsonIgnore]
        public List<GrastinPoint> BoxberryPoints { get; set; }
        [JsonIgnore]
        public List<GrastinPoint> CdekPoints { get; set; }
        [JsonIgnore]
        public bool ShowDrivingDescriptionPoint { get; set; }
        public bool IsAvailableCashOnDelivery { get; set; }
        public override bool IsAvailablePaymentPickPoint { get { return PickpointAdditionalDataObj != null && PickpointAdditionalDataObj.DeliveryType == EnDeliveryType.PickPoint && IsAvailableCashOnDelivery; } }
        public float BasePrice { get; set; }
        public float PriceCash { get; set; }

        public string CityTo { get; set; }
        public string CityFrom { get; set; }

        public override string TemplateName
        {
            get { return "PointDeliveryMapOption.html"; }
        }

        public override void Update(BaseShippingOption option)
        {
            var opt = option as GrastinPointDeliveryMapOption;
            if (opt != null && opt.Id == this.Id && opt.CityTo == this.CityTo && (GrastinPoints != null || BoxberryPoints != null || CdekPoints != null))
            {
                this.PickpointId = opt.PickpointId;
                this.YaSelectedPoint = opt.YaSelectedPoint;
                //this.PickpointAdditionalDataObj = opt.PickpointAdditionalDataObj;

                if (PickpointAdditionalDataObj != null)
                {
                    if ((PickpointAdditionalDataObj.Partner == EnPartner.Grastin || PickpointAdditionalDataObj.Partner == EnPartner.Partner) &&
                        GrastinPoints != null)
                    {
                        SelectedPoint = GrastinPoints.FirstOrDefault(x => x.Id == opt.PickpointAdditionalDataObj.PickPointId);
                    }
                    else if (PickpointAdditionalDataObj.Partner == EnPartner.Boxberry && BoxberryPoints != null)
                    {
                        SelectedPoint = BoxberryPoints.FirstOrDefault(x => x.Id == opt.PickpointAdditionalDataObj.PickPointId);
                        //if (boxberryPoint != null)
                        //    IsAvailableCashOnDelivery = boxberryPoint.FullPrePayment;
                    }
                    else if (PickpointAdditionalDataObj.Partner == EnPartner.Cdek && CdekPoints != null)
                    {
                        SelectedPoint = CdekPoints.FirstOrDefault(x => x.Id == opt.PickpointAdditionalDataObj.PickPointId);
                        //if (cdekPoint != null)
                        //    IsAvailableCashOnDelivery = cdekPoint.FullPrePayment;
                    }

                    HideAddressBlock = PickpointAdditionalDataObj == null ||
                                       (PickpointAdditionalDataObj.DeliveryType == EnDeliveryType.PickPoint &&
                                        PickpointAdditionalDataObj.Partner != EnPartner.RussianPost);
                }
            }
        }

        public override OrderPickPoint GetOrderPickPoint()
        {
            return !string.IsNullOrEmpty(PickpointId) && PickpointAdditionalDataObj != null
                ? new OrderPickPoint
                {
                    PickPointId = PickpointAdditionalDataObj.PickPointId,
                    PickPointAddress = SelectedPoint != null ?  SelectedPoint.Address : null,
                    AdditionalData = JsonConvert.SerializeObject(PickpointAdditionalDataObj)
                }
                : null;
        }

        public override bool ApplyPay(BasePaymentOption payOption)
        {
            if (payOption?.GetDetails()?.IsPickPointPayment is true)
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
            {
                var pickpointAdditionalDataObj = JsonConvert.DeserializeObject<GrastinEventWidgetData>(orderPickPoint.AdditionalData);
                if (orderPickPoint.PickPointId.IsNotEmpty())
                    PickpointId = pickpointAdditionalDataObj.Partner.ToString() + "#" + orderPickPoint.PickPointId;
                if (pickpointAdditionalDataObj != null && pickpointAdditionalDataObj.PickPointId.IsNotEmpty())
                    if ((pickpointAdditionalDataObj.Partner == EnPartner.Grastin || pickpointAdditionalDataObj.Partner == EnPartner.Partner) &&
                        GrastinPoints != null)
                    {
                        SelectedPoint = GrastinPoints.FirstOrDefault(x => x.Id == pickpointAdditionalDataObj.PickPointId);
                        if (SelectedPoint != null)
                        {
                            YaSelectedPoint = SelectedPoint.Id.GetHashCode();
                        }
                    }
                    else if (pickpointAdditionalDataObj.Partner == EnPartner.Boxberry && BoxberryPoints != null)
                    {
                        SelectedPoint = BoxberryPoints.FirstOrDefault(x => x.Id == pickpointAdditionalDataObj.PickPointId);
                        if (SelectedPoint != null)
                        {
                            YaSelectedPoint = SelectedPoint.Id.GetHashCode();
                        }
                    }
                    else if (pickpointAdditionalDataObj.Partner == EnPartner.Cdek && CdekPoints != null)
                    {
                        SelectedPoint = CdekPoints.FirstOrDefault(x => x.Id == pickpointAdditionalDataObj.PickPointId);
                        if (SelectedPoint != null)
                        {
                            YaSelectedPoint = SelectedPoint.Id.GetHashCode();
                        }
                    }
            }
        }

    }
}
