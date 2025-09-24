using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using AdvantShop.Core.Common;
using AdvantShop.Core.Common.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AdvantShop.Shipping.OzonRocket.Api
{
    [JsonConverter(typeof(ErrorConverter))]
    public class Error
    {
        /// <summary>
        /// Сообщение об ошибке.
        /// </summary>
        public string Message { get; set; }
        
        /// <summary>
        /// Код ошибки.
        /// </summary>
        public string ErrorCode { get; set; }
        
        /// <summary>
        /// Дополнительная информация об ошибке.
        /// </summary>
        public Dictionary<string, string[]> Arguments { get; set; }
        public object Extensions { get; set; }
        public int HttpStatusCode { get; set; }
    }
    
    #region OAuthData

    public class OAuthData
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [JsonProperty("expires_in")]
        public long? ExpiresIn { get; set; }
        [JsonProperty("token_type")]
        public string TokenType { get; set; }
        public string Scope { get; set; }
    }

    #endregion

    #region DeliveryVariants

    public class GetDeliveryVariantsParams
    {
        public string CityName { get; set; }
        public PayloadIncludes PayloadIncludes { get; set; }
        public Pagination Pagination { get; set; }
    }

    public class Pagination
    {
        /// <summary>
        /// Количество записей на странице
        /// </summary>
        public int Size { get; set; }
        
        /// <summary>
        /// Номер страницы
        /// </summary>
        public string Token { get; set; }
    }

    public class PayloadIncludes
    {
        /// <summary>
        /// Добавить в ответ часы работы пункта выдачи
        /// </summary>
        public bool IncludeWorkingHours { get; set; }
        
        /// <summary>
        /// Добавить в ответ почтовый индекс пункта выдачи
        /// </summary>
        public bool IncludePostalCode { get; set; }
    }

    public class DeliveryVariants
    {
        /// <summary>
        /// Список способов доставки.
        /// </summary>
        public List<DeliveryVariant> Data { get; set; }
    }

    public class DeliveryVariantsPart : DeliveryVariants
    {
        /// <summary>
        /// Общее количество способов доставки.
        /// </summary>
        public int TotalCount { get; set; }
        
        /// <summary>
        /// Номер следующей страницы.
        /// </summary>
        public string NextPageToken { get; set; }
    }

    public class DeliveryVariant
    {
        public long Id { get; set; }
        public long ObjectTypeId { get; set; }
        public string ObjectTypeName { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string Region { get; set; }
        public string Settlement { get; set; }
        public string Streets { get; set; }
        public string Placement { get; set; }
        [Obsolete]
        public bool? Enabled { get; set; }
        [Obsolete]
        public long CityId { get; set; }
        public Guid FiasGuid { get; set; }
        public Guid FiasGuidControl { get; set; }
        [Obsolete]
        public long AddressElementId { get; set; }
        public bool FittingShoesAvailable { get; set; }
        public bool FittingClothesAvailable { get; set; }
        public bool CardPaymentAvailable { get; set; }
        public string HowToGet { get; set; }
        public string Phone { get; set; }
        [Obsolete]
        public string ContractorName { get; set; }
        [Obsolete]
        public long ContractorId { get; set; }
        [Obsolete]
        public string StateName { get; set; }
        public int? MinWeight { get; set; }
        public int? MaxWeight { get; set; }
        public int? MinPrice { get; set; }
        public int? MaxPrice { get; set; }
        public int? RestrictionWidth { get; set; }
        public int? RestrictionLength { get; set; }
        public int? RestrictionHeight { get; set; }
        public float? Lat { get; set; }
        public float? Long { get; set; }
        public bool ReturnAvailable { get; set; }
        public bool PartialGiveOutAvailable { get; set; }
        public bool DangerousAvailable { get; set; }
        public bool IsCashForbidden { get; set; }
        [Obsolete]
        public string Code { get; set; }
        public bool WifiAvailable { get; set; }
        public bool LegalEntityNotAvailable { get; set; }
        [Obsolete]
        public DateTimeOffset? DateOpen { get; set; }
        [Obsolete]
        public DateTimeOffset? DateClose { get; set; }
        public bool IsRestrictionAccess { get; set; }
        public string RestrictionAccessMessage { get; set; }
        public string UtcOffsetStr { get; set; }
        public bool IsPartialPrepaymentForbidden { get; set; }
        [Obsolete]
        public bool IsGeozoneAvailable { get; set; }
        public int? PostalCode { get; set; }
        public List<WorkingHour> WorkingHours { get; set; }
    }

    public class WorkingHour
    {
        public DateTimeOffset Date { get; set; }
        public List<Period> Periods { get; set; }
    }

    public class Period
    {
        public TimePeriod Min { get; set; }
        public TimePeriod Max { get; set; }
    }

    public class TimePeriod
    {
        public int Hours { get; set; }
        public int Minutes { get; set; }
    }

    #endregion

    #region GetDeliveryVariantsByIds

    public class GetDeliveryVariantsByIds
    {
        public List<long> Ids { get; set; }
    }

    #endregion

    #region GetDeliveryVariantsByAddress
    
    public class DeliveryType : StringEnum<DeliveryType>
    {
        public DeliveryType(string value) : base(value)
        {
        }

        /// <summary>
        /// доставка курьером
        /// </summary>
        [Localize("Курьер")]
        public static DeliveryType Courier { get { return new DeliveryType("Courier"); } }

        /// <summary>
        /// самовывоз
        /// </summary>
        [Localize("ПВЗ")]
        public static DeliveryType PickPoint { get { return new DeliveryType("PickPoint"); } }

        /// <summary>
        /// постамат
        /// </summary>
        [Localize("Постамат")]
        public static DeliveryType Postamat { get { return new DeliveryType("Postamat"); } }
    }

    public class GetDeliveryVariantsByAddressParams
    {
        /// <summary>
        /// Способ доставки
        /// </summary>
        public DeliveryType DeliveryType { get; set; }
        
        /// <summary>
        /// Фильтр для способов доставки по признакам
        /// </summary>
        public FilterByAddress Filter { get; set; }
        
        /// <summary>
        /// Адрес доставки
        /// <remarks>Как минимум, нужно указать населённый пункт. Для областных населённых пунктов также указывается область и район.</remarks>
        /// </summary>
        public string Address { get; set; }
        
        /// <summary>
        /// Радиус поиска
        /// <remarks>актуально для типов доставки Постамат и ПВЗ</remarks>
        /// </summary>
        public long? Radius { get; set; }
        
        /// <summary>
        /// Информация о грузовом месте (отправлении)
        /// </summary>
        public List<PackageByAddress> Packages { get; set; }
    }

    public class FilterByAddress
    {
        /// <summary>
        /// Признак возможности принимать платёж наличными средствами
        /// </summary>
        public bool? IsCashAvailable { get; set; }
        
        /// <summary>
        /// Признак возможности принимать платёж банковской картой
        /// </summary>
        public bool? IsPaymentCardAvailable { get; set; }
        public bool? IsAnyPaymentAvailable { get; set; }
    }

    public class PackageByAddress
    {
        /// <summary>
        /// Количество одинаковых коробок.
        /// </summary>
        public int? Count { get; set; }
        
        /// <summary>
        /// Информация о габаритах.
        /// </summary>
        public DimensionsPackageByAddress Dimensions { get; set; }
        
        /// <summary>
        /// Общая стоимость содержимого коробки в рублях.
        /// </summary>
        public float? Price { get; set; }
        
        /// <summary>
        /// объявленная ценность содержимого коробки
        /// </summary>
        public float? EstimatedPrice { get; set; }
    }

    public class DimensionsPackageByAddress
    {
        /// <summary>
        /// Вес в граммах
        /// </summary>
        public int Weight { get; set; }
        
        /// <summary>
        /// Длина в миллиметрах
        /// </summary>
        public int Length { get; set; }
        
        /// <summary>
        /// Высота в миллиметрах
        /// </summary>
        public int Height { get; set; }
        
        /// <summary>
        /// Ширина в миллиметрах
        /// </summary>
        public int Width { get; set; }
    }
    
    #endregion

    #region DeliveryCalculate

    public class GetDeliveryCalculateParams
    {
        /// <summary>
        /// Идентификатор способа доставки
        /// </summary>
        public long? DeliveryVariantId { get; set; }
        
        /// <summary>
        /// Вес отправления в граммах
        /// </summary>
        public float? Weight { get; set; }
        
        /// <summary>
        /// Идентификатор места передачи отправления
        /// </summary>
        public long? FromPlaceId { get; set; }
        
        /// <summary>
        /// Объявленная ценность отправления
        /// </summary>
        public float? EstimatedPrice { get; set; }
    }

    public class DeliveryCalculate
    {
        public float Amount { get; set; }
    }

    #endregion

    #region DeliveryCalculateInformation

    public class GetDeliveryCalculateInformationParams
    {
        /// <summary>
        /// Идентификатор места отправления
        /// </summary>
        public long? FromPlaceId { get; set; }
        
        /// <summary>
        /// Адрес
        /// <remarks>Как минимум, нужно указать населённый пункт. Для областных населённых пунктов также указывается область и район.</remarks>
        /// </summary>
        public string DestinationAddress { get; set; }
        
        /// <summary>
        /// Массив с информацией по отправлениям 
        /// </summary>
        public List<PackageByAddress> Packages { get; set; }
    }
    
    public class DeliveryCalculateInformation
    {
        /// <summary>
        /// Информация о доставке
        /// </summary>
        public List<DeliveryInfo> DeliveryInfos { get; set; }
    }
    
    public class DeliveryInfo
    {
        /// <summary>
        /// Способ доставки
        /// </summary>
        public DeliveryType DeliveryType { get; set; }
        
        /// <summary>
        /// Стоимость доставки
        /// </summary>
        public float? Price { get; set; }
        
        /// <summary>
        /// Состав стоимости доставки
        /// </summary>
        public List<PricePosition> PricePositions { get; set; }
        
        /// <summary>
        /// Предполагаемый срок доставки
        /// </summary>
        public long? DeliveryTermInDays { get; set; }
    }

    public class PricePosition
    {
        /// <summary>
        /// Тип услуги
        /// </summary>
        public string Type { get; set; }
        
        /// <summary>
        /// Стоимость услуги
        /// </summary>
        public float? Amount { get; set; }
    }

    #endregion

    #region DeliveryTime

    public class GetDeliveryTimeParams
    {
        
        /// <summary>
        /// Идентификатор места передачи отправления
        /// </summary>
        public long? FromPlaceId { get; set; }
        
        /// <summary>
        /// Идентификатор способа доставки
        /// </summary>
        public long? DeliveryVariantId { get; set; }
    }

    public class DeliveryTime
    {
        /// <summary>
        /// Предполагаемый срок доставки в днях. Минимум — 1 день.
        /// <remarks>Если в ответе возвращается «0», значит нет статистики по маршруту</remarks>
        /// </summary>
        public int Days { get; set; }
    }

    #endregion

    #region DeliveryFromPlaces

    public class DeliveryFromPlaces
    {
        public List<DropOffPlace> Places { get; set; }
    }

    public class DropOffPlace
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string UtcOffset { get; set; }
    }

    #endregion

    #region DeliveryPickupPlaces

    public class DeliveryPickupPlaces
    {
        public List<PickupPlace> Places { get; set; }
    }
    
    public class PickupPlace
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string Storage { get; set; }
    }

    #endregion

    #region NewOrder

    public class NewOrderBase
    {
        public Buyer Buyer { get; set; }
        public Buyer Recipient { get; set; }
        public FirstMileTransfer FirstMileTransfer { get; set; }
        public Payment Payment { get; set; }
        
        /// <summary>
        /// Способ доставки
        /// </summary>
        public DeliveryInformation DeliveryInformation { get; set; }
        
        /// <summary>
        /// Информация по отправлениям
        /// <remarks>Допускается не более 200 отправлений</remarks>
        /// </summary>
        public List<Package> Packages { get; set; }
        public List<OrderLine> OrderLines { get; set; }
        public string Comment { get; set; }
        
        /// <summary>
        /// Частичная выдача
        /// </summary>
        public bool AllowPartialDelivery { get; set; }
        
        /// <summary>
        /// Вскрывать отправление до оплаты
        /// </summary>
        public bool AllowUncovering { get; set; }
        
        public List<OrderAttribute> OrderAttributes { get; set; }
    }

    public class NewOrder : NewOrderBase
    {
        public string OrderNumber { get; set; }
    }

    public class UpdateOrder : NewOrderBase
    {
        public long OrderId { get; set; }
    }

    public class NewDraftOrder : NewOrder { }

    public class Buyer
    {
        public string Name { get; set; }
        public TypeBuyer Type { get; set; }
        public string LegalName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }

    public class TypeBuyer : StringEnum<TypeBuyer>
    {
        public TypeBuyer(string value) : base(value)
        {
        }

        /// <summary>
        /// физическое лицо
        /// </summary>
        public static TypeBuyer NaturalPerson { get { return new TypeBuyer("NaturalPerson"); } }

        /// <summary>
        /// юридическое лицо
        /// </summary>
        public static TypeBuyer LegalPerson { get { return new TypeBuyer("LegalPerson"); } }
    }

    public class DeliveryInformation
    {
        public string DeliveryVariantId { get; set; }
        public string Address { get; set; }
        
        /// <summary>
        /// Идентификатор периода доставки
        /// </summary>
        public string TimeSlotId { get; set; }
        public DesiredDeliveryTimeInterval DesiredDeliveryTimeInterval { get; set; }
        public string AdditionalAddress { get; set; }
    }

    public class DesiredDeliveryTimeInterval
    {
        public DateTimeOffset? From { get; set; }
        public DateTimeOffset? To { get; set; }
    }

    public class FirstMileTransfer
    {
        public EnFirstMileTransferType Type { get; set; }
        public string FromPlaceId { get; set; }
        
        [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd")]
        public DateTimeOffset? PickupPlannedDate { get; set; }
        public string PickupPlannedInterval { get; set; }
        public string PickupPlaceId { get; set; }    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum EnFirstMileTransferType
    {
        /// <summary>
        /// принципал везёт заказы на склад Ozon самостоятельно
        /// </summary>
        [EnumMember(Value = "DropOff")]DropOff,
        
        /// <summary>
        /// необходим забор заказов у принципала силами Ozon
        /// <remarks>Можно указывать только при наличии одного склада у принципала</remarks>
        /// </summary>
        [EnumMember(Value = "PickUp")]PickUp
    }

    public class OrderAttribute
    {
        public string ContractorShortName { get; set; }
        public bool? ReturnOfShippingDocuments { get; set; }
    }

    public class OrderLine
    {
        public string ArticleNumber { get; set; }
        public string Name { get; set; }
        public float SellingPrice { get; set; }
        public float EstimatedPrice { get; set; }
        public int Quantity { get; set; }
        public Vat Vat { get; set; }
        public Attributes Attributes { get; set; }
        
        /// <summary>
        /// Идентификатор грузового места
        /// <remarks>resideInPackages = packageNumber</remarks>
        /// </summary>
        public List<string> ResideInPackages { get; set; }
        
        /// <summary>
        /// ИНН поставщика
        /// </summary>
        public string SupplierTin { get; set; }
    }

    public class Attributes
    {
        /// <summary>
        /// Признак опасного груза
        /// </summary>
        public bool? IsDangerous { get; set; }
    }

    public class Vat
    {
        public float Rate { get; set; }
        public float Sum { get; set; }
    }

    public class Package
    {
        /// <summary>
        /// Номер грузоместа
        /// </summary>
        public string PackageNumber { get; set; }
        
        /// <summary>
        /// Информация о габаритах
        /// </summary>
        public DimensionsPackage Dimensions { get; set; }
        
        /// <summary>
        /// Штрихкод грузоместа
        /// </summary>
        public string BarCode { get; set; }
    }

    public class DimensionsPackage
    {
        /// <summary>
        /// Вес в граммах
        /// </summary>
        public int Weight { get; set; }
        
        /// <summary>
        /// Длина в миллиметрах
        /// </summary>
        public int Length { get; set; }
        
        /// <summary>
        /// Высота в миллиметрах
        /// </summary>
        public int Height { get; set; }
        
        /// <summary>
        /// Ширина в миллиметрах
        /// </summary>
        public int Width { get; set; }
    }

    public class Payment
    {
        /// <summary>
        /// Тип оплаты
        /// </summary>
        public EnPaymentType Type { get; set; }
        
        /// <summary>
        /// Сумма предварительной оплаты
        /// </summary>
        public float PrepaymentAmount { get; set; }
        
        /// <summary>
        /// Сумма наложенного платежа, который необходимо получить от клиента в момент выдачи заказа
        /// </summary>
        public float RecipientPaymentAmount { get; set; }
        
        /// <summary>
        /// Стоимость доставки
        /// </summary>
        public float DeliveryPrice { get; set; }
        
        /// <summary>
        /// НДС доставки. Обязательно, если стоимость доставки облагается налогом
        /// </summary>
        public Vat DeliveryVat { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum EnPaymentType
    {
        /// <summary>
        /// полная предварительная оплата
        /// </summary>
        [EnumMember(Value = "FullPrepayment")]FullPrepayment,
        
        /// <summary>
        /// постоплата
        /// </summary>
        [EnumMember(Value = "Postpay")]Postpay
    }
    
    public class CreatedOrder
    {
        public long Id { get; set; }
        public string OrderNumber { get; set; }
        public string LogisticOrderNumber { get; set; }
        public List<CreatedOrderPackage> Packages { get; set; }
    }
     
    public class UpdatedOrder : CreatedOrder { }
   
    public class CreatedDraftOrder : CreatedOrder { }
    
    public class ConvertedDraftToOrder : CreatedOrder { }

    public class CreatedOrderPackage
    {
        public string PackageNumber { get; set; }
        public string PostingNumber { get; set; }
        public long PostingId { get; set; }
        public string BarCode { get; set; }
    }
    
    #endregion

    #region CancellationOrders

    public class CancellationOrders
    {
        public List<long> Ids { get; set; }
    }

    public class CancellationOrdersResult
    {
        public List<ResponseCancellationOrderResult> Responses { get; set; }
    }

    public class ResponseCancellationOrderResult
    {
        public List<ErrorCancellationOrder> Errors { get; set; }
        public long Id { get; set; }
        public bool Success { get; set; }
        public ResponseCancellationOrder Response { get; set; }
    }

    public class ErrorCancellationOrder
    {
        public string Message { get; set; }
        public string Code { get; set; }
        public string PropertyName { get; set; }
    }

    public class ResponseCancellationOrder
    {
        public string NewStatus { get; set; }
        public string OldStatus { get; set; }
    }
    
    #endregion

    #region TrackingByPostingNumber

    public class TrackingByPostingNumberParams
    {
        public string PostingNumber { get; set; }
    }

    public class TrackingByPostingNumber
    {
        public TrackingHeader TrackingHeader { get; set; }
        public List<TrackingByPostingNumberItem> Items { get; set; }
    }

    public class TrackingByPostingNumberItem
    {
        /// <summary>
        /// Идентификатор события
        /// </summary>
        public int EventId { get; set; }
        
        /// <summary>
        /// Идентификатор места
        /// </summary>
        public string PlaceId { get; set; }
        
        /// <summary>
        /// Наименование места
        /// </summary>
        public string PlaceName { get; set; }
        
        /// <summary>
        /// Дата и время события
        /// </summary>
        public DateTimeOffset? Moment { get; set; }
        
        /// <summary>
        /// Тип действия с отправлением
        /// </summary>
        public string Action { get; set; }
        
        /// <summary>
        /// Идентификатор перевозки
        /// </summary>
        public string CarrierId { get; set; }
        
        /// <summary>
        /// Название перевозки
        /// </summary>
        public string CarrierName { get; set; }
    }

    public class TrackingHeader
    {
        /// <summary>
        /// Согласованная с клиентом дата доставки. Только при доставке курьером
        /// </summary>
        public ApprovedDeliveryTime ApprovedDeliveryTime { get; set; }
        
        /// <summary>
        /// Ожидаемое время прибытия в город получателя
        /// </summary>
        public DateTimeOffset? EstimateTimeArrival { get; set; }
        
        /// <summary>
        /// Количество дней хранения в пункте выдачи
        /// </summary>
        public int? StorageDays { get; set; }
        
        /// <summary>
        /// Срок хранения — дата, до которой отправление будет храниться в пункте выдачи
        /// </summary>
        public DateTimeOffset? StorageExpirationDate { get; set; }
    }

    public class ApprovedDeliveryTime
    {
        /// <summary>
        /// Дата доставки
        /// </summary>
        public DateTimeOffset? Date { get; set; }
        
        /// <summary>
        /// Время начала доставки
        /// </summary>
        public DateTimeOffset? TimeFrom { get; set; }
        
        /// <summary>
        /// Время конца доставки
        /// </summary>
        public DateTimeOffset? TimeTo { get; set; }
    }
    
    #endregion

    #region TrackingByPostingNumbersOrIds

    public class TrackingByPostingNumbersOrIdsParams
    {
        /// <summary>
        /// postingNumber или postingId
        /// </summary>
        public List<string> Articles { get; set; }
    }

    public class TrackingByPostingNumbersOrIds
    {
        public List<TrackingItem> Items { get; set; }
    }

    public class TrackingItem
    {
        public string PostingNumber { get; set; }
        public string PostingBarcode { get; set; }
        public List<TrackingEvent> Events { get; set; }
    }
   
    public class TrackingEvent
    {
        /// <summary>
        /// Идентификатор события
        /// </summary>
        public int? EventId { get; set; }
        
        /// <summary>
        /// Идентификатор места
        /// </summary>
        public long PlaceId { get; set; }
        
        /// <summary>
        /// Наименование места
        /// </summary>
        public string PlaceName { get; set; }
        
        /// <summary>
        /// Дата и время события
        /// </summary>
        public DateTimeOffset? Moment { get; set; }
        
        /// <summary>
        /// Тип действия с отправлением
        /// </summary>
        public string Action { get; set; }
        
        /// <summary>
        /// Идентификатор перевозки
        /// </summary>
        public long? CarrierId { get; set; }
        
        /// <summary>
        /// Название перевозки
        /// </summary>
        public string CarrierName { get; set; }
    } 
    #endregion
}