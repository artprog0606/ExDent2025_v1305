using AdvantShop.Core.Common;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace AdvantShop.Shipping.Yandex.Api
{
    [JsonConverter(typeof(YandexErrorConverter))]
    public class ErrorResponse
    {
        public string Message { get; set; }
    }

    public class Error
    {
        public string Message { get; set; }
    }

    #region Calculate

    public class TariffType : StringEnum<TariffType>
    {
        public TariffType(string value) : base(value)
        {
        }

        /// <summary>
        /// доставка до двери в интервал
        /// </summary>
        [Localize("AdvantShop.Core.Shipping.YandexApi.Courier")]
        public static TariffType TimeInterval { get { return new TariffType("time_interval"); } }
        /// <summary>
        /// доствка до ПВЗ
        /// </summary>
        [Localize("AdvantShop.Core.Shipping.YandexApi.OrderPickupPoint")]
        public static TariffType SelfPickup { get { return new TariffType("self_pickup"); } }
    }
    /// <summary>
    /// Для настроек
    /// </summary>
    public class DeliveryType : StringEnum<DeliveryType>
    {
        public DeliveryType(string value) : base(value)
        {
        }
        /// <summary>
        /// доставка до двери в интервал
        /// </summary>
        [Localize("AdvantShop.Core.Shipping.YandexApi.Courier")]
        public static DeliveryType Courier { get { return new DeliveryType("Courier"); } }
        /// <summary>
        /// доствка до ПВЗ
        /// </summary>
        [Localize("AdvantShop.Core.Shipping.YandexApi.OrderPickupPoint")]
        public static DeliveryType PVZ { get { return new DeliveryType("PVZ"); } }
        /// <summary>
        /// доствка до постамата
        /// </summary>
        [Localize("AdvantShop.Core.Shipping.YandexApi.Postamat")]
        public static DeliveryType Postamat { get { return new DeliveryType("postamat"); } }
    }

    public class PaymentMethodType : StringEnum<PaymentMethodType>
    {
        public PaymentMethodType(string value) : base(value)
        {
        }

        /// <summary>
        /// уже оплачено
        /// </summary>
        public static PaymentMethodType AlreadyPaid { get { return new PaymentMethodType("already_paid"); } }
        /// <summary>
        /// оплата картой при получении
        /// </summary>
        public static PaymentMethodType CardOnReceipt { get { return new PaymentMethodType("card_on_receipt"); } }
        /// <summary>
        /// оплата наличными при получении
        /// </summary>
        public static PaymentMethodType CashOnReceipt { get { return new PaymentMethodType("cash_on_receipt"); } }
        /// <summary>
        /// безналичный
        /// </summary>
        public static PaymentMethodType Cashless { get { return new PaymentMethodType("cashless"); } }
    }

    public class Location
    {
        /// <summary>
        /// Адрес точки.
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// Id (ПВЗ или постамата доставки)/(склада отправки), зарегистрированного в платформе.
        /// </summary>
        public string PlatformStationId { get; set; }
    }

    public class CalculateParams
    {
        /// <summary>
        /// Cумма к оплате с получателя в копейках.
        /// </summary>
        public int ClientPrice { get; set; }
        /// <summary>
        /// Точка доставки.
        /// </summary>
        public Location Destination { get; set; }
        /// <summary>
        /// Cпособ оплаты товаров.
        /// </summary>
        public PaymentMethodType PaymentMethod { get; set; }
        /// <summary>
        /// Точка отправки.
        /// </summary>
        public Location Source { get; set; }
        /// <summary>
        /// Тариф доставки.
        /// </summary>
        public TariffType Tariff { get; set; }
        /// <summary>
        /// Суммарная оценочная стоимость посылок в копейках.
        /// </summary>
        public int TotalAssessedPrice { get; set; }
        /// <summary>
        /// Cуммарный вес посылки в граммах.
        /// </summary>
        public int TotalWeight { get; set; }
        /// <summary>
        /// Информация о местах в заказе.
        /// </summary>
        public List<CalculatePlace> Places { get; set; }
    }

    public class CalculateResponse
    {
        public bool Error { get; set; }
        /// <summary>
        /// Стоимость за услугу доставки и страхование посылки в руб (с НДС). В случае указания способа оплаты already_paid не заполняется.
        /// </summary>
        public string Pricing { get; set; }
        /// <summary>
        /// Размер комисиии за прием наложенного платежа в %. Заполняется в случае указания способа оплаты отлиного от already_paid.
        /// </summary>
        public string PricingCommissionOnDeliveryPayment { get; set; }
        /// <summary>
        /// Размер комисиии за прием наложенного платежа в руб (с НДС). Заполняется в случае указания способа оплаты отлиного от already_paid.
        /// </summary>
        public string PricingCommissionOnDeliveryPaymentAmount { get; set; }
        /// <summary>
        /// Суммарная стоимость доставки с учетом дополнительных услуг (с НДС).
        /// </summary>
        public string PricingTotal { get; set; }
        /// <summary>
        /// Сообщение при ошибке запроса
        /// </summary>
        public string Message { get; set; }
    }

    public class DeliveryIntervalParams
    {
        /// <summary>
        /// id станции (склада) отгрузки
        /// </summary>
        public string StationId { get; set; }
        /// <summary>
        /// Полный конечный адрес доставки.
        /// </summary>
        public string FullAddress { get; set; }
        /// <summary>
        /// GeoID конечного адреса
        /// </summary>
        public int? GeoId { get; set; }
        /// <summary>
        /// id пвз в логплатформе
        /// </summary>
        public string SelfPickupId { get; set; }
        /// <summary>
        /// Формат в котором время нужно отправить интервалы доставки (true - unix, false - utc)
        /// </summary>
        //public bool SendUnix => true;
    }

    public class DeliveryInterval 
    {
        public List<IntervalOffer> Offers { get; set; }
    }

    public class IntervalOffer
    {
        /// <summary>
        /// UTC время предлагаемого начала доставки
        /// </summary>
        public string From { get; set; }
        /// <summary>
        /// UTC время предлагаемого начала доставки
        /// </summary>
        public string To { get; set; }
    }

    public class CalculatePlace
    {
        public PhysicalDims PhysicalDims { get; set; }
    }

    #endregion

    #region PickPoint

    public class PickPointType : StringEnum<PickPointType>
    {
        public PickPointType(string value) : base(value)
        {
        }

        /// <summary>
        /// пункт выдачи заказов
        /// </summary>
        public static PickPointType PickupPoint { get { return new PickPointType("pickup_point"); } }
        /// <summary>
        /// постомат
        /// </summary>
        public static PickPointType Terminal { get { return new PickPointType("terminal"); } }
        /// <summary>
        /// почтовое отделение
        /// </summary>
        public static PickPointType PostOffice { get { return new PickPointType("post_office"); } }
        /// <summary>
        /// сортировочный центр
        /// </summary>
        public static PickPointType SortingCenter { get { return new PickPointType("sorting_center"); } }
    }

    public class PickPointParams
    {
        public int? GeoId { get; set; }
        /// <summary>
        /// Возможность отгрузки заказов в точку (самопривоз)
        /// </summary>
        public bool AvailableForDropoff { get; set; }
        public Сoordinates Latitude { get; set; }
        public Сoordinates Longitude { get; set; }
        /// <summary>
        /// Cпособ оплаты товаров.
        /// </summary>
        public PaymentMethodType PaymentMethod { get; set; }
        /// <summary>
        /// Идентификаторы точек получения заказа
        /// </summary>
        public List<string> PickupPointIds { get; set; }
        public PickPointType Type { get; set; }

    }

    public class Сoordinates
    {
        /// <summary>
        /// Нижняя граница интервала
        /// </summary>
        public float From { get; set; }
        /// <summary>
        /// Верхняя граница интервала
        /// </summary>
        public float To { get; set; }
    }

    #endregion

    #region GetPickPoint
    
    public class PickPointResponse
    {
        public List<PickupPoint> Points { get; set; }
    }

    public class PickupPoint
    {
        /// <summary>
        /// Адрес пункта выдачи
        /// </summary>
        public Address Address { get; set; }
        /// <summary>
        /// Контакты пункта выдачи
        /// </summary>
        public Contact Contact { get; set; }
        /// <summary>
        /// Идентификатор точки забора заказа. Должен использоваться при получении вариантов доставки в качестве конечной точки.
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Название точки забора заказа.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Дополнительные указания по тому, как добраться до точки получения заказа.
        /// </summary>
        public string Instruction { get; set; }
        /// <summary>
        /// методы оплаты заказа при получении.
        /// </summary>
        public List<PaymentMethodType> PaymentMethods { get; set; }
        /// <summary>
        /// Положение на карте
        /// </summary>
        public GeoLocation Position { get; set; }
        /// <summary>
        /// Расписание работы
        /// </summary>
        public Schedule Schedule { get; set; }
        /// <summary>
        /// Тип пункта выдачи
        /// </summary>
        public PickPointType Type { get; set; }
        /// <summary>
        /// Признак Почты России
        /// </summary>
        public bool IsPostOffice { get; set; }

    }

    public class Address
    {
        /// <summary>
        /// идентификатор населённого пункта
        /// </summary>
        [JsonProperty("geoId")]
        public int GeoId { get; set; }
        /// <summary>
        /// Комментарий
        /// </summary>
        public string Comment { get; set; }
        /// <summary>
        /// Полный адрес
        /// </summary>
        public string FullAddress { get; set; }
        /// <summary>
        /// Номер квартиры или офиса
        /// </summary>
        public string Room { get; set; }
    }

    public class Contact
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Partonymic { get; set; }
        public string Phone { get; set; }
    }

    public class Schedule
    {
        public List<Restriction> Restrictions { get; set; }
        /// <summary>
        /// Часовая зона; смещение в часах относительно всемирного координированного времени UTC.
        /// </summary>
        public int TimeZone { get; set; }
    }

    public class Restriction
    {
        /// <summary>
        /// Номера дней недели, к которым применяется правило. 1 - понедельник, 2 - вторник, ..., 7 - воскресенье.
        /// </summary>
        public List<int> Days { get; set; }
        /// <summary>
        /// Время работы, от
        /// </summary>
        public WorkTime TimeFrom { get; set; }
        /// <summary>
        /// Время работы, до
        /// </summary>
        public WorkTime TimeTo { get; set; }

    }

    public class WorkTime
    {
        public int Hours { get; set; }
        public int Minutes { get; set; }
    }

    public class GeoLocation
    {
        public float? Latitude { get; set; }
        public float? Longitude { get; set; }
    }

    public class GetGeoIdResponse
    {
        public List<AddressResponse> Variants { get; set; }
    }

    public class AddressResponse
    {
        /// <summary>
        /// Вариант адреса
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// Идентификатор населенного пункта (geo_id)
        /// </summary>
        public int GeoId { get; set; }
    }

    #endregion

    #region Create order

    public class LastMilePolicyType : StringEnum<LastMilePolicyType>
    {
        public LastMilePolicyType(string value) : base(value)
        {
        }

        /// <summary>
        /// доставка до двери в интервал
        /// </summary>
        public static LastMilePolicyType TimeInterval { get { return new LastMilePolicyType("time_interval"); } }
        /// <summary>
        /// По требованию
        /// </summary>
        public static LastMilePolicyType OnDemand { get { return new LastMilePolicyType("on_demand"); } }
        /// <summary>
        /// Экспресс
        /// </summary>
        public static LastMilePolicyType Express { get { return new LastMilePolicyType("express"); } }
        /// <summary>
        /// доствка до ПВЗ
        /// </summary>
        public static LastMilePolicyType SelfPickup { get { return new LastMilePolicyType("self_pickup"); } }
    }

    public class DestinationType : StringEnum<DestinationType>
    {
        public DestinationType(string value) : base(value)
        {
        }

        /// <summary>
        /// Для доставки до двери
        /// </summary>
        public static DestinationType CustomLocation { get { return new DestinationType("custom_location"); } }
        /// <summary>
        /// для доставки до ПВЗ
        /// </summary>
        public static DestinationType PlatformStation { get { return new DestinationType("platform_station"); } }
    }

    public class CancelOrderStatusType : StringEnum<CancelOrderStatusType>
    {
        public CancelOrderStatusType(string value) : base(value)
        {
        }

        /// <summary>
        /// отмена заявки инициирована в платформе
        /// </summary>
        public static CancelOrderStatusType Created { get { return new CancelOrderStatusType("CREATED"); } }
        /// <summary>
        /// отмена заявки успешно выполнена
        /// </summary>
        public static CancelOrderStatusType Success { get { return new CancelOrderStatusType("SUCCESS"); } }
        /// <summary>
        /// запрос завершился с ошибкой
        /// </summary>
        public static CancelOrderStatusType Error { get { return new CancelOrderStatusType("ERROR"); } }
    }

    public class CreateOrderParams
    {
        public BillingInfo BillingInfo { get; set; }
        /// <summary>
        /// Место назначения, обязательное
        /// </summary>
        public Destination Destination { get; set; }
        public Info Info { get; set; }
        /// <summary>
        /// Информация о предметах в заказе.
        /// </summary>
        public List<Item> Items { get; set; }
        /// <summary>
        /// Для доставки до двери в указанный интервал - "time_interval", для доставки до пункта выдачи - "self_pickup"
        /// </summary>
        public LastMilePolicyType LastMilePolicy { get; set; }
        /// <summary>
        /// Разрешен ли частичный выкуп
        /// </summary>
        public bool ParticularItemsRefuse { get; set; }
        /// <summary>
        /// Информация о местах в заказе, обязательное
        /// </summary>
        public List<Place> Places { get; set; }
        public RecipientInfo RecipientInfo { get; set; }
        /// <summary>
        /// Место отправления, обязательное
        /// </summary>
        public Source Source { get; set; }
    }

    public class BillingDetails
    {
        /// <summary>
        /// Оценная цена за единицу товара (передается в копейках), обязательное
        /// </summary>
        public int AssessedUnitPrice { get; set; }
        /// <summary>
        /// ИНН
        /// </summary>
        public string INN { get; set; }
        /// <summary>
        /// Код НДС. Дефолтное значение 20%.
        /// </summary>
        public int NDS { get; set; }
        /// <summary>
        /// Цена за единицу товара (передается в копейках), обязательное
        /// </summary>
        public int UnitPrice { get; set; }
    }

    public class BillingInfo
    {
        /// <summary>
        /// Сумма которую нужно взять с получателя за доставку
        /// </summary>
        public int DeliveryCost { get; set; }
        /// <summary>
        /// метод оплаты, обязательное
        /// </summary>
        public PaymentMethodType PaymentMethod { get; set; }
    }

    public class CustomLocation
    {
        /// <summary>
        /// Детали адреса
        /// </summary>
        public Details Details { get; set; }
        public float? Latitude { get; set; }
        public float? Longitude { get; set; }
    }

    public class Destination
    {
        /// <summary>
        /// Информация о произвольной точке. Точка может быть задана либо своими координатами - пара (latitude, longitude), либо адресом
        /// </summary>
        public CustomLocation CustomLocation { get; set; }
        /// <summary>
        /// Интервал необходимо передавать либо в UTC, либо в UNIX timestamp. При доставке до пункта выдачи, необходимо указывать только дату
        /// </summary>
        public Interval Interval { get; set; }
        /// <summary>
        /// Интервал необходимо передавать либо в UTC, либо в UNIX timestamp. При доставке до пункта выдачи, необходимо указывать только дату
        /// </summary>
        public IntervalUtc IntervalUtc { get; set; }
        public PlatformStation PlatformStation { get; set; }
        /// <summary>
        /// Тип целевой точки. Для доставки до двери - custom_location (2), для доставки до ПВЗ - platform_station (1)
        /// </summary>
        public DestinationType Type { get; set; }
    }

    public class Details
    {
        public string Comment { get; set; }
        public string FullAddress { get; set; }
        public string Room { get; set; }
    }

    public class Info
    {
        /// <summary>
        /// Опциональный коментарий.
        /// </summary>
        public string Comment { get; set; }
        /// <summary>
        /// Идентификатор заказа у отправителя, обязательное
        /// </summary>
        public string OperatorRequestId { get; set; }
        /// <summary>
        /// Передаем "advantshop" для статистики наших доставок
        /// </summary>
        public string ReferralSource { get; set; }
    }

    public class Interval
    {
        public long From { get; set; }
        public long To { get; set; }
    }

    public class IntervalUtc
    {
        public string From { get; set; }
        public string To { get; set; }
    }

    public class Item
    {
        /// <summary>
        /// Артикул, обязательное
        /// </summary>
        public string Article { get; set; }
        public BillingDetails BillingDetails { get; set; }
        /// <summary>
        /// Количество, обязательное
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// Код для маркировки
        /// </summary>
        public string MarkingCode { get; set; }
        /// <summary>
        /// Название, обязательное
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Указываются либо габариты (dx, dy, dz), либо Объем в см3 (predefined_volume).
        /// </summary>
        public PhysicalDims PhysicalDims { get; set; }
        /// <summary>
        /// Штрихкод коробки, к которой относится товар, обязательное
        /// </summary>
        public string PlaceBarcode { get; set; }
        /// <summary>
        /// Уникальный идентификатор
        /// </summary>
        public string Uin { get; set; }
        /// <summary>
        /// Штрихкод товарa, обязательное
        /// </summary>
        public string Barcode { get; set; }
    }

    public class PhysicalDims
    {
        /// <summary>
        /// Длина, сантиметры
        /// </summary>
        public int Dx { get; set; }
        /// <summary>
        /// Высота, сантиметры
        /// </summary>
        public int Dy { get; set; }
        /// <summary>
        /// Ширина, сантиметры
        /// </summary>
        public int Dz { get; set; }
        /// <summary>
        /// Объем в см3
        /// </summary>
        public int PredefinedVolume { get; set; }
        /// <summary>
        /// Вес брутто, граммы, обязательное
        /// </summary>
        public int WeightGross { get; set; }
    }

    public class Place
    {
        /// <summary>
        /// Штрихкод коробки, обязательное
        /// </summary>
        public string Barcode { get; set; }
        /// <summary>
        /// Описание коробки
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Указываются либо габариты (dx, dy, dz), либо Объем в см3 (predefined_volume), обязательное
        /// </summary>
        public PhysicalDims PhysicalDims { get; set; }
    }

    public class PlatformStation
    {
        /// <summary>
        /// Идентификатор станции в Логистической платформе (например, склад отгрузки или ПВЗ).
        /// </summary>
        public string PlatformId { get; set; }
    }

    public class RecipientInfo
    {
        /// <summary>
        /// Адрес электронной почты
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Имя, обязательное
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// Фамилия
        /// </summary>
        public string LastName { get; set; }
        /// <summary>
        /// Отчество
        /// </summary>
        public string Partonymic { get; set; }
        /// <summary>
        /// Номер телефона, обязательное
        /// </summary>
        public string Phone { get; set; }
    }

    public class Source
    {
        public PlatformStation PlatformStation { get; set; }
    }

    public class CreateOrderResponse
    {
        public List<OrderOffer> Offers { get; set; }
    }

    public class OrderOffer
    {
        /// <summary>
        /// UTC Timestamp окончания действия предложения маршрутного листа в Гринвиче.
        /// </summary>
        public string ExpiresAt { get; set; }
        public OfferDetails OfferDetails { get; set; }
        /// <summary>
        /// Идентификатор предложения маршрутного листа (оффера).
        /// </summary>
        public string OfferId { get; set; }
        /// <summary>
        /// Стоимость
        /// </summary>
        public string Pricing { get; set; }
    }

    public class OrderDeliveryInterval
    {
        /// <summary>
        /// Верхняя граница временного интервала доставки, UTC Timestamp в Гринвиче.
        /// </summary>
        public string Max { get; set; }
        /// <summary>
        /// Нижняя граница временного интервала доставки, UTC Timestamp в Гринвиче.
        /// </summary>
        public string Min { get; set; }
        /// <summary>
        /// Для доставки до двери в указанный интервал - "time_interval", для доставки до пункта выдачи - "self_pickup"
        /// </summary>
        public LastMilePolicyType Policy { get; set; }
    }

    public class OfferDetails
    {
        public OrderDeliveryInterval DeliveryInterval { get; set; }
    }

    public class ConfirmOrderResponse
    {
        /// <summary>
        /// Идентификатор только что созданного заказа
        /// </summary>
        public string RequestId { get; set; }
    }

    public class CancelOrderResponse
    {
        /// <summary>
        /// Комментарий к результату выполнения запроса
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Статус отмены заявки
        /// </summary>
        public CancelOrderStatusType Status { get; set; }

    }

    #endregion

    #region GetStatuses

    public class StatusesResponse
    {
        public List<StateHistory> StateHistory { get; set; }
    }

    public class StateHistory
    {
        public long Timestamp { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
    }

    #endregion

}
