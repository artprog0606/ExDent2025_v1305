using AdvantShop.Core.Common.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;

namespace AdvantShop.Shipping.LPost.Api
{
    public class LPostError
    {
        [JsonProperty("errorMessage")]
        public string Error { get; set; }
    }

    public class LPostMethod
    {
        public EMethod Method { get; set; }
        public string Name { get; set; }
        public int Version { get; set; }
    }

    public class LPostToken : LPostError
    {
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("valid_till", ItemConverterType = typeof(JavaScriptDateTimeConverter))]
        public DateTime ValidTill { get; set; }

        public bool NeedUpdate {
            get
            {
                return DateTime.Now > ValidTill
#if DEBUG
                    .AddHours(1) // Добавляю час, т.к. ответ приходит в Московском времени
#endif
                    || Token.IsNullOrEmpty()
                    || Error.IsNotEmpty(); 
            }
        }
    }

    #region ReceivePoint

    [JsonConverter(typeof(LPostTextJsonConverter<LPostReceivePointList, LPostReceivePoint>))]
    public class LPostReceivePointList : LPostError
    {
        public List<LPostReceivePoint> ReceivePoints { get; set; }

        public LPostReceivePointList(List<LPostReceivePoint> receivePoints)
        {
            ReceivePoints = receivePoints;
        }
        public LPostReceivePointList() { }
    }

    public class LPostReceivePoint //: LPostError
    {
        [JsonProperty("ID_Sklad")]
        public int WarehouseId { get; set; }

        [JsonProperty("ID_Region")]
        public int RegionCode { get; set; }

        public string City { get; set; }
        public string Address { get; set; }

        [JsonProperty("Shedule")]
        public string WorkTime { get; set; }

        [JsonProperty("Break")]
        public string BreakTime { get; set; }
    }

    #endregion

    #region PickPoint

    [JsonConverter(typeof(LPostTextJsonConverter<LPostPickPointList, LPostPickPoint>))]
    public class LPostPickPointList : LPostError
    {
        public List<LPostPickPoint> PickPoints { get; set; }

        public LPostPickPointList(List<LPostPickPoint> pickPoints)
        {
            PickPoints = pickPoints;
        }
        public LPostPickPointList() { }
    }

    public class LPostPickPoint
    {
        [JsonProperty("ID_PickupPoint")]
        public int PickPointId { get; set; }

        [JsonProperty("Zone")]
        public List<LPostZone> ZoneList {get; set;}
        public float Latitude { get; set; }
        public float Longitude { get; set; }

        [JsonProperty("CityName")]
        public string City { get; set; }

        [JsonProperty("ID_Region")]
        public int RegionCode { get; set; }
        public int DayLogistic { get; set; }
        public EPickPointCourier IsCourier { get; set; }
        public EPickPointCash IsCash { get; set; }
        public EPickPointCard IsCard { get; set; }
        public string Address { get; set; }

        [JsonProperty("PickupDop")]
        public string AddressDescription { get; set; }

        public string Metro { get; set; }
        public int NumberOfFittingRooms { get; set; }

        [JsonProperty("Photo")]
        public List<LPostPhoto> Photos { get; set; }
    }

    public class PickPointParams
    {
        [JsonProperty("ID_Regions")]
        public List<int> RegionCodeList { get; set; }
        [DefaultValue(-1)]
        [JsonProperty("isCourier")]
        public EPickPointCourier IsCourier { get; set; }
        public string CityName { get; set; }

        public override string ToString()
        {
            return
                (RegionCodeList != null ? string.Join(",", RegionCodeList) : "")
                + IsCourier.ToString();
        }
    }
    public class LPostZone
    {
        [JsonProperty("ZoneNumber")]
        public int Id { get; set; }
        public string WKT { get; set; }
    }

    public class LPostPhoto
    {
        [JsonProperty("Photo")]
        public string PhotoUrl { get; set; }
    }

    public class PickUpWorkHours
    {
        public string Day { get; set; }

        [JsonProperty("From", ItemConverterType = typeof(JavaScriptDateTimeConverter))]
        public DateTime WorkFrom { get; set; }

        [JsonProperty("To", ItemConverterType = typeof(JavaScriptDateTimeConverter))]
        public DateTime WorkTo { get; set; }
    }

    #endregion

    #region AddressPoints

    [JsonConverter(typeof(LPostTextJsonConverter<LPostAddressPointList, LPostAddressPoint>))]
    public class LPostAddressPointList : LPostError
    {
        public List<LPostAddressPoint> AddressPoints { get; set; }

        public LPostAddressPointList(List<LPostAddressPoint> addressPoints)
        {
            AddressPoints = addressPoints;
        }
        public LPostAddressPointList() { }
    }

    public class LPostAddressPoint
    {
        [JsonProperty("ID_PartnerWarehouse")]
        public int WarehouseId { get; set; }
        public string Address { get; set; }
    }

    #endregion

    #region DeliveryCost

    [JsonConverter(typeof(LPostTextJsonConverter<LPostDeliveryCostList, LPostDeliveryCost>))]
    public class LPostDeliveryCostList : LPostError
    {
        public LPostDeliveryCost DeliveryCost { get; set; }

        public LPostDeliveryCostList(List<LPostDeliveryCost> deliveryCostList)
        {
            if (deliveryCostList != null && deliveryCostList.Count > 0)
                DeliveryCost = deliveryCostList[0];
        }
        public LPostDeliveryCostList() { }
    }

    public class LPostDeliveryCost
    {
        public float SumCost { get; set; }
        public float DeliveryCost { get; set; }
        public float ServicesCost { get; set; }
        public float OptionsCost { get; set; }
        public int DayLogistic { get; set; }
        public DateTime DateClose { get; set; }
        [JsonProperty("PossibleDelivDates")]
        public List<DeliveryDate> DeliveryDates 
        { 
            get => SortDeliveryDates;
            set => SortDeliveryDates = value?
                .OrderBy(x => x.DateDelive)
                .ToList();
        }

        private List<DeliveryDate> SortDeliveryDates;

        public string DeliveryTime
        {
            get
            {
                return SortDeliveryDates != null && SortDeliveryDates.Count > 0
                    ? $"От {(SortDeliveryDates[0].DateDelive - DateTime.Now).Days} " +
                    $"до {(SortDeliveryDates[DeliveryDates.Count - 1].DateDelive - DateTime.Now).Days} дн."
                    : string.Empty;
            }
        }
    }

    public class LPostOptionParams
    {
        [JsonProperty("ID_Sklad")]
        public int WarehouseId { get; set; }

        [JsonProperty("ID_PartnerWarehouse")]
        public int PartnerWarehouseId { get; set; }

        [JsonProperty("ID_PickupPoint")]
        public int PickPointId { get; set; }

        [DefaultValue(-1)]
        public EOrderType OrderType { get; set; } = EOrderType.Delivery;
        public string LinkedOrder { get; set; }
        [JsonProperty("isExchange")]
        public bool IsExchange { get; set; }

        [JsonProperty("isFastDelivery")]
        public bool IsFastDelivery { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }

        [JsonProperty("isNotExactAddress")]
        public int IsNotExactAddress { get; set; }
        public string Address { get; set; }
        public DateTime DateShipment { get; set; }
        public int Weight { get; set; }
        public int Volume { get; set; }
        public decimal SumPayment { get; set; }
        public decimal Value { get; set; }

        [JsonProperty("Options")]
        public List<LPostDopOption> DopOptions { get; set; }
    }

    public class LPostDopOption
    {
        public bool Fitting { get; set; }
        public bool ReturnDocuments { get; set; }
    }

    public class DeliveryDate
    {
        public DateTime DateDelive { get; set; }
        public List<Interval> Intervals { get; set; } 
    }

    public class Interval
    {
        public TimeSpan TimeFrom { get; set; }
        public TimeSpan TimeTo { get; set; }
    }

    #endregion

    public enum EMethod
    {
        Auth,
        GetReceivePoints,
        GetPickupPoints,
        GetAddressPoints,
        GetServicesCalc
    }

    public enum EPickPointCourier
    {
        SelfDelivery = 0,
        Courier = 1
    }

    public enum EPickPointCash
    {
        NoCash = 0,
        HaveCash = 1
    }

    public enum EPickPointCard
    {
        NoCard = 0,
        HaveCard = 1
    }

    public enum EOrderType
    {
        Delivery = 1,
        Take = 2
    }

}
