using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AdvantShop.Shipping.FivePost.Api
{
    #region Token

    public class FivePostToken : FivePostError
    {
        public FivePostToken()
        {
            ExpireDate = DateTime.Now.AddHours(1);
        }

        public string Status { get; set; }

        [JsonProperty("jwt")]
        public string JwtToken { get; set; }

        private DateTime ExpireDate;

        public bool NeedUpdate
        {
            get
            {
                return DateTime.Now > ExpireDate.AddMinutes(-5)
                    || JwtToken.IsNullOrEmpty()
                    || Status != "ok";
            }
        }
    }

    public class FivePostGetTokenParams
    {
        public string Subject { get; set; }
        public string Audience { get; set; }
    }

    #endregion

    public class FivePostErrorList
    {
        public List<FivePostError> Errors { get; set; }
    }

    public class FivePostError
    {
        [JsonProperty("errorcode")]
        public string ErrorCode { get; set; }
        public string Message { get; set; }

        [JsonProperty("http_status_code")]
        public string Code { get; set; }

        public string ErrorMessage { get; set; }

        public string ErrorMsg
        {
            set => ErrorMessage = value;
        }

        public string errorСode
        {
            get => ErrorCode;
            set => ErrorCode = value;
        }

        public string Error { get; set; }

        public string code
        {
            set => Code = value;
        }

        public string FullError
            => StringHelper.AggregateStrings(", ", Code, ErrorCode, Message, ErrorMessage);
    }

    public class FivePostPagination
    {
        public int TotalPages { get; set; }

        public int TotalElements { get; set; }

        [JsonProperty("numberOfElements")]
        public int ElementsOnPage { get; set; }

        /// <summary>
        /// Не может быть больше 20
        /// </summary>
        [JsonProperty("size")]
        public int MaxPageSize { get; set; }
    }

    #region PickPoints

    public class FivePostPickPointParams
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; }

        public FivePostPickPointParams()
        {
            PageSize = 1000;
        }

        public FivePostPickPointParams(int pageSize, int pageNumber)
        {
            PageSize = pageSize;
            PageNumber = pageNumber;
        }
    }

    public class FivePostPickPointList : FivePostPagination
    {
        [JsonProperty("content")]
        public List<FivePostPickPoint> PickPoints { get; set; }
    }

    public class FivePostPickPoint
    {
        public string Id { get; set; }

        [JsonProperty("mdmCode")]
        public string InternalCode { get; set; }

        public string Name { get; set; }
        public string PartnerName { get; set; }
        public string MultiplaceDeliveryAllowed { get; set; }
        public EFivePostPickPointType Type { get; set; }

        [JsonProperty("additional")]
        public string Description { get; set; }

        [JsonProperty("cellLimits")]
        public FivePostWeightDimension WeightDimensionsLimit { get; set; }

        public bool ReturnAllowed { get; set; }
        public string TimeZone { get; set; }
        public string Phone { get; set; }

        [JsonProperty("cashAllowed")]
        public bool IsCash { get; set; }

        [JsonProperty("cardAllowed")]
        public bool IsCard { get; set; }

        [JsonProperty("localityFiasCode")]
        public string FiasCode { get; set; }

        [JsonProperty("rate")]
        public List<FivePostRate> RateList { get; set; }

        /// <summary>
        /// Возможны ситуации, когда в параметре deliverySl возвращается пустой массив или zone = null (и, соответственно, rateValue = 0). 
        /// Такие точки необходимо игнорировать, если отсутствующая информация критична для корректности работы с клиентами.
        /// </summary>
        [JsonProperty("deliverySL")]
        public List<FivePostPossibleDelivery> PossibleDeliveryList { get; set; }

        public FivePostLastMileWarehouse LastMileWarehouse { get; set; }
        public List<FivePostPickPointWorkHour> WorkHours { get; set; }
        public string FullAddress { get; set; }
        public string ShortAddress { get; set; }
        public FivePostAddress Address { get; set; }

        public FivePostRate CalculationRate { get; set; }

        public FivePostPossibleDelivery PossibleDelivery { get; set; }
    }

    public class FivePostPickPointWorkHour
    {
        public EFivePostDayOfWeek Day { get; set; }
        public TimeSpan OpensAt { get; set; }
        public TimeSpan CloseAt { get; set; }
        public string TimeZone { get; set; }
        public string TimeZoneOffset { get; set; }
    }

    public class FivePostWarehouseWorkHour
    {
        [JsonIgnore]
        public EFivePostDayOfWeek DayOfWeek { get; set; }
        public string DayOfWeekFormatted => DayOfWeek.Localize();

        public int DayNumber => (int)DayOfWeek;
        public TimeSpan? TimeFrom { get; set; }
        public TimeSpan? TimeTill { get; set; }
    }

    public class FivePostAddress
    {
        [JsonProperty("country")]
        public string CountryName { get; set; }

        public int ZipCode { get; set; }

        [JsonProperty("region")]
        public string RegionName { get; set; }
        public string RegionType { get; set; }
        public string ShortAddress { get; set; }

        [JsonProperty("city")]
        public string CityName { get; set; }
        public string CityType { get; set; }
        public string Street { get; set; }
        public string House { get; set; }
        public string Building { get; set; }
        public string MetroStation { get; set; }

        [JsonProperty("lat")]
        public float Latitude { get; set; }

        [JsonProperty("lng")]
        public float Longitude { get; set; }

        public override string ToString()
        {
            return StringHelper.AggregateStrings(", ", CountryName, RegionName, CityName, Street, House, Building);
        }
    }

    public class FivePostWeightDimension
    {
        [JsonProperty("maxCellWidth")]
        public int MaxWidthInMillimeters { get; set; }

        [JsonProperty("maxCellHeight")]
        public int MaxHeightInMillimeters { get; set; }

        [JsonProperty("maxCellLength")]
        public int MaxLengthInMillimeters { get; set; }

        [JsonProperty("maxWeight")]
        public long MaxWeightInMilligrams { get; set; }
    }

    public class FivePostPossibleDelivery
    {
        [JsonProperty("sl")]
        public int MaxDeliveryDays { get; set; }

        [JsonProperty("minimalSl")]
        public int MinDeliveryDays { get; set; }

        [JsonProperty("slCode")]
        public int Code { get; set; }
        public EFivePostDeliveryType Type => (EFivePostDeliveryType)Code;
    }

    public class FivePostRate
    {
        [JsonProperty("rateType")]
        public string Type { get; set; }

        /// <summary>
        /// Если zone=’NULL’ (пустое), значит, корректные данные по этой точке не успели внести
        /// </summary>
        [JsonProperty("zone")]
        public string ZoneName { get; set; }

        [JsonProperty("rateValue")]
        public string Value { get; set; }
        public string Vat { get; set; }

        // Стоимость 0 означает, что тарифный план для Партнера ещё не прогружен.  Бесплатной доставки на данный момент нет.
        [JsonProperty("rateValueWithVat")]
        public float ValueWithVat { get; set; }

        /// <summary>
        /// Величина надбавки за перевес с учетом НДС
        /// </summary>
        [JsonProperty("rateExtraValueWithVat")]
        public float ExtraValueWithVat { get; set; }

        /// <summary>
        /// Величина надбавки за перевес
        /// </summary>
        [JsonProperty("rateExtraValue")]
        public string ExtraValue { get; set; }

        [JsonProperty("rateCurrency")]
        public string CurrencyIso3 { get; set; }

        [JsonProperty("rateTypeCode")]
        public int TypeCode { get; set; }
    }

    public class FivePostLastMileWarehouse
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    #endregion

    #region Warehouses

    public class FivePostCreateUpdateWarehouseParams
    {
        public string WarehouseUUId { get; set; }

        /// <summary>
        /// Идентификатор склада в системе Партнера (Advantshop). Например: Romashka-1
        /// </summary>
        [JsonProperty("partnerLocationId")]
        public string UniqueName { get; set; }
        public string Name { get; set; }

        [JsonProperty("countryId")]
        public string CountryIso2 { get; set; }
        public string RegionCode { get; set; }
        public string FederalDistrict { get; set; }

        [JsonProperty("region")]
        public string RegionName { get; set; }

        [JsonProperty("index")]
        public string PostCode { get; set; }

        /// <summary>
        /// Наименование города с населенным пунктом. Например г. Санкт-Петербург
        /// </summary>
        [JsonProperty("city")]
        public string CityName { get; set; }
        public string Street { get; set; }

        [JsonProperty("houseNumber")]
        public string House { get; set; }
        public string Coordinates => string.Join(", ",
            Latitude.ToInvariantString(),
            Longitude.ToInvariantString());

        [JsonIgnore]
        public float Latitude { get; set; }
        [JsonIgnore]
        public float Longitude { get; set; }

        [JsonProperty("contactPhoneNumber")]
        public string Phone { get; set; }
        public string TimeZone { get; set; }     

        public List<FivePostWarehouseWorkHour> WorkingTime { get; set; }
    }

    public class FivePostWarehouseResult : FivePostError
    {
        public string Id { get; set; }

        [JsonProperty("timestamp")]
        public DateTime TimeStamp { get; set; }
        public int Status { get; set; }
        public string Path { get; set; }
    }

    public class FivePostGetWarehouseParams
    {
        public string Id { get; set; }
    }

    public class FivePostWarehouse : FivePostError
    {
        public string Id { get; set; }

        [JsonProperty("partnerLocationId")]
        public string InnerId { get; set; }
        public string Type { get; set; }

        [JsonProperty("status")]
        public EFivePostWarehouseStatus Active { get; set; }
        public string Name { get; set; }
        public string ContractorId { get; set; }
        public string FullAddress { get; set; }

        [JsonProperty("countryId")]
        public string CountryIso2 { get; set; }
        public string RegionCode { get; set; }

        [JsonProperty("index")]
        public string PostCode { get; set; }

        [JsonProperty("city")]
        public string CityName { get; set; }
        public string Street { get; set; }
        public string HouseNumber { get; set; }

        [JsonProperty("contactPhoneNumber")]
        public string Phone { get; set; }
        public string TimeZone { get; set; }
        public string TimeZoneOffset { get; set; }
        public string Coordinates { get; set; }

        private float _latitude;
        public float Latitude => _latitude != 0 ? _latitude : (_latitude = 
            Coordinates.Contains(",") ? Coordinates.Split(",")[0].TryParseFloat() : 0);
        private float _longitude;
        public float Longitude => _longitude != 0 ? _longitude : (_longitude =
            Coordinates.Contains(",") ? Coordinates.Split(",")[1].TryParseFloat() : 0);

        public List<FivePostPickPointWorkHour> WorkingTime { get; set; }

        public string Address => $"{CityName}, {Street} {HouseNumber}";
    }

    public class FivePostWarehouseList : FivePostPagination
    {
        [JsonProperty("content")]
        public List<FivePostWarehouse> Warehouses { get; set; }
    }

    #endregion

    #region Orders

    public class FivePostCreateOrderParams
    {
        [JsonProperty("partnerOrders")]
        public List<FivePostOrderDetails> OrderDetailsCollection => new List<FivePostOrderDetails> { OrderDetails };

        public FivePostOrderDetails OrderDetails { get; set; }
    }

    public class FivePostOrderDetails
    {
        [JsonProperty("senderOrderId")]
        public string OrderId { get; set; }

        [JsonProperty("clientOrderId")]
        public string TrackNumber { get; set; }

        [JsonProperty("clientName")]
        public string CustomerFIO { get; set; }

        [JsonProperty("clientEmail")]
        public string CustomerEmail { get; set; }

        [JsonProperty("clientPhone")]
        public long? CustomerStandartPhone { get; set; }

        /// <summary>
        /// Id склада указанное пользователем при создании склада (partnerLocationId)
        /// </summary>
        [JsonProperty("senderLocation")]
        public string WarehouseId { get; set; }

        [JsonProperty("receiverLocation")]
        public string PickPointId { get; set; }
        public EFivePostUndeliverableOption UndeliverableOption { get; set; }

        [JsonProperty("rateTypeCode")]
        public string RateType { get; set; }

        [JsonProperty("cost")]
        public FivePostOrderDetailsCost CostDetails { get; set; }

        public List<FivePostCargoDetails> Cargoes => new List<FivePostCargoDetails> { Cargo };

        [JsonIgnore]
        public FivePostCargoDetails Cargo { get; set; }
    }

    public class FivePostOrderDetailsCost
    {
        public float DeliveryCost { get; set; }

        [JsonProperty("deliveryCostCurrency")]
        public string DeliveryCurrensyIso3 => PaymentCurrencyIso3;

        [JsonProperty("paymentValue")]
        public float SumWithDelivery { get; set; }

        [JsonProperty("paymentCurrency")]
        public string PaymentCurrencyIso3 { get; set; }

        [JsonProperty("price")]
        public float InsurePrice { get; set; }

        [JsonProperty("priceCurrency")]
        public string InsureCurrensyIso3 => PaymentCurrencyIso3;

        public float PrepaymentSum { get; set; }
        public string PaymentType { get; set; }
    }

    public class FivePostCargoDetails
    {
        [JsonProperty("barcodes")]
        public List<FivePostBarcodeDetails> BarcodeDetails { get; set; }

        /// <summary>
        /// Id грузоместа, можно указать Barcode
        /// </summary>
        public string CargoId { get; set; }

        /// <summary>
        /// Высота в мм, должна быть больше 1
        /// </summary>
        [JsonProperty("height")]
        public int HeightInMillimeters { get; set; }

        /// <summary>
        /// Длина в мм, должна быть больше 1
        /// </summary>
        [JsonProperty("length")]
        public int LengthInMillimeters { get; set; }

        /// <summary>
        /// Ширина в мм, должна быть больше 1
        /// </summary>
        [JsonProperty("width")]
        public int WidthInMillimeters { get; set; }

        /// <summary>
        /// высота в мг, должна быть больше 1
        /// </summary>
        [JsonProperty("weight")]
        public long WeightInMilligrams { get; set; }

        [JsonProperty("price")]
        public float Sum { get; set; }

        [JsonProperty("currency")]
        public string CurrencyIso3 { get; set; }

        [JsonProperty("productValues")]
        public List<FivePostProduct> Products { get; set; }

        /// <summary>
        /// Приходит при получении ответа на создание заказа
        /// </summary>
        public string Barcode { get; set; }
    }

    public class FivePostCreateOrderResult : FivePostErrorList
    {
        public bool Created { get; set; }

        [JsonProperty("orderId")]
        public string FivePostOrderId { get; set; }

        [JsonProperty("senderOrderId")]
        public string OrderId { get; set; }

        public List<FivePostCargoDetails> Cargoes { get; set; }
    }

    public class FivePostDeleteOrderResult : FivePostError
    {
        public bool CanBeRetriedLater { get; set; }
    }

    public class FivePostBarcodeDetails
    {
        [JsonProperty("value")]
        public string Barcode { get; set; }
    }

    public class FivePostProduct
    {
        public string Name { get; set; }

        [JsonProperty("value")]
        public float Amount { get; set; }

        [JsonProperty("price")]
        public float PriceWithVat { get; set; }

        /// <summary>
        /// Ставка НДС в %. Возможные значения: 0, 10, 20, -1 (=без НДС)
        /// </summary>
        public float Vat { get; set; }

        [JsonProperty("vendorCode")]
        public string ArtNo { get; set; }
    }

    public class FivePostSyncOrderParams
    {
        [JsonProperty("senderOrderId")]
        public List<int> OrderIds { get; set; }
    }

    #endregion

    #region Statuses

    public class FivePostSyncStatusParams
    {
        [JsonProperty("senderOrderId")]
        public List<int> OrderIds { get; set; }
    }

    public class FivePostStatusResult
    {
        [JsonProperty("senderOrderId")]
        public int OrderId { get; set; }

        public EFivePostStatus Status { get; set; }
        public string ExecutionStatus { get; set; }
        public DateTime ChangeDate { get; set; }
    }

    #endregion

    #region enums

    [JsonConverter(typeof(StringEnumConverter))]
    public enum EFivePostPickPointType
    {
        [EnumMember(Value = "POSTAMAT")]
        Postamat = 0,

        [EnumMember(Value = "TOBACCO")]
        Tobacco = 1,

        [EnumMember(Value = "ISSUE_POINT")]
        Pvz = 2,
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum EFivePostDayOfWeek
    {
        [EnumMember(Value = "MON")]
        [Localize("Core.Shipping.FivePost.Monday")]
        Monday = 1,

        [EnumMember(Value = "TUE")]
        [Localize("Core.Shipping.FivePost.Tuesday")]
        Tuesday = 2,

        [EnumMember(Value = "WED")]
        [Localize("Core.Shipping.FivePost.Wednesday")]
        Wednesday = 3,

        [EnumMember(Value = "THU")]
        [Localize("Core.Shipping.FivePost.Thursday")]
        Thursday = 4,

        [EnumMember(Value = "FRI")]
        [Localize("Core.Shipping.FivePost.Friday")]
        Friday = 5,

        [EnumMember(Value = "SAT")]
        [Localize("Core.Shipping.FivePost.Saturday")]
        Saturday = 6,

        [EnumMember(Value = "SUN")]
        [Localize("Core.Shipping.FivePost.Sunday")]
        Sunday = 7,
    }

    public enum EFivePostWarehouseStatus
    {
        ACTIVE = 0
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum EFivePostUndeliverableOption
    {
        [EnumMember(Value = "RETURN")]
        [Localize("Core.Shipping.FivePost.ReturnToWarehouse")]
        ReturnToWarehouse,

        [EnumMember(Value = "UTILIZATION")]
        [Localize("Core.Shipping.FivePost.Utilization")]
        Utilization
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum EFivePostStatus
    {
        [EnumMember(Value = "NEW")]
        [Localize("Core.Shipping.FivePost.OrderStatus.New")]
        [StringName("Заказ был отправлен, но еще не был провалидирован системой OMS")]
        New = 0,

        [EnumMember(Value = "APPROVED")]
        [Localize("Core.Shipping.FivePost.OrderStatus.Approved")]
        [StringName("Заказ прошел валидацию в системе OMS и принят к исполнению")]
        Approved = 1,

        [EnumMember(Value = "REJECTED")]
        [Localize("Core.Shipping.FivePost.OrderStatus.Rejected")]
        [StringName("Заказ не прошел валидацию системой OMS")]
        Rejected = 2,

        [EnumMember(Value = "IN_PROCESS")]
        [Localize("Core.Shipping.FivePost.OrderStatus.InProgress")]
        [StringName("Заказ был передан в 5Пост")]
        InProgress = 3,

        [EnumMember(Value = "DONE")]
        [Localize("Core.Shipping.FivePost.OrderStatus.Done")]
        [StringName("Заказ выдан клиенту")]
        Done = 4,

        [EnumMember(Value = "INTERRUPTED")]
        [Localize("Core.Shipping.FivePost.OrderStatus.Interrupted")]
        [StringName("Исполнение заказа не может быть продолжено из-за невозможности обработки доставки")]
        Interrupted = 5,

        [EnumMember(Value = "CANCELLED")]
        [Localize("Core.Shipping.FivePost.OrderStatus.Cancelled")]
        [StringName("")]
        Cancelled = 6,

        [EnumMember(Value = "UNCLAIMED")]
        [Localize("Core.Shipping.FivePost.OrderStatus.Unclaimed")]
        [StringName("Клиент не забрал заказ из точки выдачи, срок хранения его истек и далее, согласно контракту, он будет возвращен или утилизирован.")]
        Unclaimed = 7,

        [EnumMember(Value = "PROBLEM")]
        [Localize("Core.Shipping.FivePost.OrderStatus.Problem")]
        [StringName("Заказ имеет проблемы")]
        Problem = 8,
    }

    public enum EFivePostDeliveryType
    {
        [StringName("Москва")] HubMoscowCity = 10,
        [StringName("Москва (ПВЗ)")] HubMoscowPoint = 11,
        [StringName("Москва (ПВЗ) +1 день")] HubMoscowPointPlus1 = 12,
        [StringName("Москва +1 день")] HubMoscowCityPlus1 = 13,
        [StringName("Москва (ПВЗ) +2 дня")] HubMoscowPointPlus2 = 14,
        [StringName("Санкт-Петербург")] HubSpbCity = 20,
        [StringName("Санкт-Петербург (ПВЗ)")] HubSpbPoint = 21,
        [StringName("Санкт-Петербург +1 день")] HubSpbCityPlus1 = 22,
        [StringName("Екатеринбург")] HubEkbCity = 30,
        [StringName("Екатеринбург (ПВЗ)")] HubEkbPoint = 31,
        [StringName("Екатеринбург +1 день")] HubEkbCityPlus1 = 32,
        [StringName("Казань")] HubKazanCity = 40,
        [StringName("Казань (ПВЗ)")] HubKazanPoint = 41,
        [StringName("Казань +1 день")] HubKazanCityPlus1 = 42,
        [StringName("Казань +2 дня")] HubKazanCityPlus2 = 43,
        [StringName("Ростов-на-Дону")] HubRostovOnDonCity = 50,
        [StringName("Ростов-на-Дону (ПВЗ)")] HubRostovOnDonPoint = 51,
        [StringName("Ростов-на-Дону (ПВЗ -3 дня)")] HubRostovOnDonPointMinus3 = 52,
        [StringName("Краснодар")] HubKrasnodarCity = 60,
        [StringName("Краснодар (ПВЗ)")] HubKrasnodarPoint = 61,
        [StringName("Самара")] HubSamaraCity = 70,
        [StringName("Самара (ПВЗ)")] HubSamaraPoint = 71,
        [StringName("Воронеж")] HubVoronezhCity = 80,
        [StringName("Воронеж (ПВЗ)")] HubVoronezhPoint = 81,
        [StringName("РЦ последней мили")] LmCity = 100,
        [StringName("РЦ последней мили (ПВЗ)")] LmPoint = 101,
        [StringName("Сберлогистика")] SL_SMM = 110,
        [StringName("Tastycoffee Казань")] Tastycoffee_Kazan_min = 120,
        [StringName("Челябинск")] HubChelyabinskCity = 130,
        [StringName("Челябинск (ПВЗ)")] HubChelyabinskPoint = 131,
        [StringName("Новосибирск")] HubNovosibirskCity = 140,
        [StringName("Новосибирск (ПВЗ)")] HubNovosibirskPoint = 141,
        [StringName("Мвидео")] SLMvideo = 203,
        [StringName("Казань +2 дня")] HubKazanCityPlus2_2 = 210,
        [StringName("Москва +2 дня")] HubMoscowCityPlus2 = 220,
        [StringName("Москва (ПВЗ) +3 дня")] HubMoscowPointPlus3 = 221,
        [StringName("Москва (ПВЗ) +4 дня")] HubMoscowPointPlus4 = 231,
        [StringName("Москва +3 дня)")] HubMoscowCityPlus3 = 240,
        [StringName("Москва (ПВЗ) +5 дней")] HubMoscowPointPlus5 = 241,
        [StringName("Москва +4 дня")] HubMoscowCityPlus4 = 250,
        [StringName("Санкт-Петербург (ПВЗ) +2 дня")] HubSpbPointPlus2 = 261,
        [StringName("Санкт-Петербург (ПВЗ) +3 дня")] HubSpbPointPlus3 = 271,
        [StringName("Екатеринбург (ПВЗ) +2 дня")] HubEkbPointPlus2 = 281,
        [StringName("Екатеринбург (ПВЗ) +3 дня")] HubEkbPointPlus3 = 291,
        [StringName("Санкт-Петербург (ПВЗ) +1 день")] HubSPBPointPlus1 = 301,
        [StringName("Екатеринбург (ПВЗ) +1 дня")] HubEkbPointPlus1 = 311,
        [StringName("РЦ последней мили +1 день")] LmCityPlus1 = 320,
        [StringName("РЦ последней мили (ПВЗ) +1 день")] LmPointPlus1 = 321,
        [StringName("РЦ последней мили +2 дня")] LmCityPlus2 = 330,
        [StringName("РЦ последней мили (ПВЗ) +2 дня")] LmPointPlus2 = 331,
    }

    [Obsolete]
    public enum EFivePostRateType
    {
        [StringName("HUB Moscow")] HubMoscow = 10,
        [StringName("Hub_Krasnodar")] HubKrasnodar = 15,
        [StringName("HUB_SPB")] HubSpb = 20,
        [StringName("Hub_Ekb")] HubEkb = 3,
        [StringName("Hub_Kazan")] HubKazan = 13,
        [StringName("Hub_Rostov_na_Donu")] HubRostovNaDonu = 14,
        [StringName("Hub_Chelyabinsk")] HubChelyabinsk = 11,
        [StringName("Hub_Novosibirsk")] HubNovosibirsk = 32,
        [StringName("Hub_Ryazav")] HubRyazav = 9,
        [StringName("Hub_Voronej")] HubVoronej = 16,
        [StringName("Hub_Kursk")] HubKursk = 19,
        [StringName("Hub_Nevinnomysk")] HubNevinnomysk = 21,
        [StringName("Hub_Petrozavodsk")] HubPetrozavodsk = 23,
        [StringName("Hub_Perm")] HubPerm = 24,
        [StringName("Hub_Samara")] HubSamara = 25,
        [StringName("Hub_Tyumen")] HubTyumen = 29,
        [StringName("Hub_Ufa")] HubUfa = 30,
        [StringName("Hub_Yaroslavl")] HubYaroslavl = 31,
        [StringName("Hub_Elabuga")] HubElabuga = 35
    }

    public enum EFivePostPaymentType
    {
        [StringName("CASH")] Cash = 0,
        [StringName("CASHLESS")] Cashless = 1,
        [StringName("PREPAYMENT")] Prepayment = 2,
    }

    #endregion
}
