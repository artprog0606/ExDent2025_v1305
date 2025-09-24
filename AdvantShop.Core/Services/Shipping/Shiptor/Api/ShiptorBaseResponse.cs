using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AdvantShop.Shipping.Shiptor.Api
{
    [JsonConverter(typeof(ShiptorBaseResponseConverter))]
    public class ShiptorBaseResponse<T>
        where T : class, new()
    {
        public ShiptorBaseResponse(T result, ResponseError error)
        {
            Result = result;
            Error = error;
        }

        public T Result { get; set; }
        public ResponseError Error { get; set; }
    }

    public class ResponseError
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public string Data { get; set; }
    }

    public class PageResponse
    {
        public int Count { get; set; }
        public int Page { get; set; }
        [JsonProperty("per_page")] public int PerPage { get; set; }
        public int Pages { get; set; }
    }

    #region Settlement

    public class GetSettlementsResponse : PageResponse
    {
        public List<Settlement> Settlements { get; set; }
    }

    #endregion

    #region CalculateShipping

    public class CalculateShippingResponse
    {
        public CalculateShippingParams Request { get; set; }
        public List<CalculateMethod> Methods { get; set; }
    }

    public class CalculateMethod
    {
        public string Status { get; set; }
        public ShippingMethod Method { get; set; }
        public CalculateMethodCost Cost { get; set; }
        public string Days { get; set; }
        public int Priority { get; set; }
    }

    public class CalculateMethodCost
    {
        public List<ServiceCost> Services { get; set; }
        public TotalCalculateMethodCost Total { get; set; }
    }

    public class ServiceCost
    {
        public string Service { get; set; }
        public float Sum { get; set; }

        [JsonProperty("currency")]
        public string CurrencyIso3 { get; set; }
        public string Readable { get; set; }
    }

    //[JsonConverter(typeof(StringEnumConverter))]
    //public enum EnServiceCostType
    //{
    //    /// <summary>
    //    /// Доставка
    //    /// </summary>
    //    [EnumMember(Value = "shipping")] Shipping,

    //    /// <summary>
    //    /// Комиссия за наложенный платеж
    //    /// </summary>
    //    [EnumMember(Value = "cod")] CashOnDelivery,
        
    //    /// <summary>
    //    /// Комиссия за страховку
    //    /// </summary>
    //    [EnumMember(Value = "cost_declaring")] DeclaredCost,
    //}

    public class TotalCalculateMethodCost
    {
        public float Sum { get; set; }

        [JsonProperty("currency")]
        public string CurrencyIso3 { get; set; }
        public string Readable { get; set; }
    }

    public class SimpleCalculateResponse
    {
        public SimpleCalculateParams Request { get; set; }
        public List<CalculateMethod> Methods { get; set; }
    }

    #endregion

    #region SetProduct

    public class SetProductResponse
    {
        public SetProductResult Result { get; set; }
        public int Id { get; set; }
    }

    public class SetProductResult : SetProductParams
    {
        public int Id { get; set; }
    }

    #endregion

    #region Package

    #region addPackage

    public class AddOrderResponse : AddOrderParams
    {
        public int Id { get; set; }
    }

    #endregion

    #region GetPackage

    public class GetPackageResponse
    {
        public int Id { get; set; }

        /// <summary>
        /// Уникальный идентификатор заказа в магазине
        /// </summary>
        [JsonProperty("external_id")]
        public string OrderNumber { get; set; }
        public string Status { get; set; }
        [JsonProperty("tracking_number")] public string TrackingNumber { get; set; }
        [JsonProperty("external_tracking_number")] public string ExternalTrackingNumber { get; set; }
        [JsonProperty("created_at")] public DateTime CreatedAt { get; set; }
        
        /// <summary>
        /// Длина, см
        /// </summary>
        public float Length { get; set; }

        /// <summary>
        /// Ширина, см
        /// </summary>
        public float Width { get; set; }

        /// <summary>
        /// Высота, см
        /// </summary>
        public float Height { get; set; }

        /// <summary>
        /// Вес, кг
        /// </summary>
        public float Weight { get; set; }

        /// <summary>
        /// Сумма наложенного платежа
        /// </summary>
        [JsonProperty("cod")]
        public float? CashOnDelivery { get; set; }

        /// <summary>
        /// Объявленная ценность. Минимально допустимое значение - 10.
        /// </summary>
        [JsonProperty("declared_cost")]
        public float? DeclaredCost { get; set; }

        [JsonProperty("label_url")] public string LabelUrl { get; set; }
        public List<PackagePhoto> Photo { get; set; }
        public PackageCost Cost { get; set; }

        /// <summary>
        /// Данные об отправлении
        /// </summary>
        [JsonProperty("departure")]
        public ReturnPackageDeparture Departure { get; set; }
        //[JsonProperty("pick_up")] public EnExportType? PickUp { get; set; }
        public PackageShipment Shipment { get; set; }
        public List<PackageCheckpoint> Checkpoints { get; set; }
        public List<PackageHistory> History { get; set; }
        public List<PackageOrder> Orders { get; set; }
        public List<PackageProblem> Problems { get; set; }
    }

    public class ReturnPackageDeparture
    {
        /// <summary>
        /// ID способа доставки
        /// </summary>
        [JsonProperty("shipping_method")]
        public ShippingMethod ShippingMethod { get; set; }

        /// <summary>
        /// ID пункта самовывоза
        /// </summary>
        // [JsonProperty("delivery_point")]
        // public int? DeliveryPointId { get; set; }

        /// <summary>
        /// Оплата картой
        /// </summary>
        [JsonProperty("cashless_payment")]
        public bool? IsCashlessPayment { get; set; }

        /// <summary>
        /// Комментарий
        /// </summary>
        [JsonProperty("comment")]
        public string Comment { get; set; }

        /// <summary>
        /// Данные об адресе доставки
        /// </summary>
        [JsonProperty("address")]
        public ReturnDepartureAddress Address { get; set; }
    }

    public class ReturnDepartureAddress
    {
        /// <summary>
        /// Имя
        /// </summary>
        [JsonProperty("name")]
        public string FistName { get; set; }

        /// <summary>
        /// Фамилия
        /// </summary>
        [JsonProperty("surname")]
        public string LastName { get; set; }

        /// <summary>
        /// Отчество
        /// </summary>
        [JsonProperty("patronymic")]
        public string Patronymic { get; set; }

        /// <summary>
        /// Ф.И.О или название организации
        /// </summary>
        [JsonProperty("receiver")]
        public string Receiver { get; set; }

        /// <summary>
        /// Адрес электронной почты
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; }

        /// <summary>
        /// Номер телефона в международном формате (+7…)
        /// </summary>
        [JsonProperty("phone")]
        public string Phone { get; set; }

        /// <summary>
        /// Почтовый индекс
        /// </summary>
        [JsonProperty("postal_code")]
        public string PostalCode { get; set; }

        /// <summary>
        /// Страна
        /// </summary>
        [JsonProperty("country_code")]
        public string CountryIso2 { get; set; }

        /// <summary>
        /// Область. Можно оставить пустой, если передан kladr_id.
        /// </summary>
        [JsonProperty("administrative_area")]
        public string AdministrativeArea { get; set; }

        /// <summary>
        /// Населенный пункт. Можно оставить пустым, если передан kladr_id.
        /// </summary>
        [JsonProperty("settlement")]
        public string Settlement { get; set; }

        /// <summary>
        /// Улица
        /// </summary>
        [JsonProperty("street")]
        public string Street { get; set; }

        /// <summary>
        /// Дом
        /// </summary>
        [JsonProperty("house")]
        public string House { get; set; }

        /// <summary>
        /// Квартира
        /// </summary>
        [JsonProperty("apartment")]
        public string Apartment { get; set; }

        /// <summary>
        /// Код КЛАДР населенного пункта, можно получить из справочника населенных пунктов.
        /// </summary>
        [JsonProperty("kladr_id")]
        public string KladrId { get; set; }
    }

    public class PackagePhoto
    {
        public string Default { get; set; }
        public string Medium { get; set; }
        public string Mini { get; set; }
    }

    public class PackageCost
    {
        [JsonProperty("shipping_cost")] public float ShippingCost { get; set; }
        [JsonProperty("cod_service_cost")] public int CodServiceCost { get; set; }
        [JsonProperty("compensation_service_cost")] public float CompensationServiceCost { get; set; }
        [JsonProperty("total_cost")] public float TotalCost { get; set; }
    }

    public class PackageShipment
    {
        public int Id { get; set; }
        [JsonProperty("pickup_date")] public DateTime PickupDate { get; set; }
        [JsonProperty("pickup_time")] public string PickupTime { get; set; }
        public bool Confirmed { get; set; }
        public PackageShipmentCourier Courier { get; set; }
        public PackageShipmentAddress Address { get; set; }
    }

    public class PackageShipmentAddress : ReturnDepartureAddress
    {
        /// <summary>
        /// Адрес одной строкой
        /// </summary>
        [JsonProperty("address_line_1")]
        public string Address { get; set; }

        /// <summary>
        /// Адрес 2. Для иностранных адресов максимальное значение до 50 символов.
        /// </summary>
        [JsonProperty("address_line_2")]
        public string Address2 { get; set; }
    }

    public class PackageShipmentCourier
    {
        //public EnTypeCourier Slug { get; set; }
        public string Slug { get; set; }
        public string Name { get; set; }
    }

    public class PackageCheckpoint
    {
        public DateTime Date { get; set; }
        public string Message { get; set; }
        public string Details { get; set; }
    }

    public class PackageHistory
    {
        public DateTime Date { get; set; }
        public string Event { get; set; }
        public string Description { get; set; }
    }

    public class PackageOrderLine
    {
        public float Sum { get; set; }
        public string Service { get; set; }
        public int Package { get; set; }
        public EnExportType? Pickup { get; set; }
    }

    public class PackageOrder
    {
        public int Transaction { get; set; }
        //public List<PackageOrderLine> Lines { get; set; }
        public PackageOrderLine Lines { get; set; }
    }

    public class PackageProblem
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [JsonProperty("created_at")] public DateTime CreatedAt { get; set; }
        [JsonProperty("resolved_at")] public DateTime? ResolvedAt { get; set; }
    }

    #endregion

    #endregion
}
