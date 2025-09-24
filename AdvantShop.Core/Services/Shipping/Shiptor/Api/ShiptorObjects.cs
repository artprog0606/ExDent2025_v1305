using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AdvantShop.Shipping.Shiptor.Api
{
    public class Package
    {
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
        /// Собирать со склада (только фулфилмент)
        /// </summary>
        [JsonProperty("is_fulfilment")]
        public bool? IsFulfilment { get; set; }

        /// <summary>
        /// Не собирать посылку (только фулфилмент)
        /// </summary>
        [JsonProperty("no_gather")]
        public bool? NoGather { get; set; }

        /// <summary>
        /// Объявленная ценность. Минимально допустимое значение - 10.
        /// </summary>
        [JsonProperty("declared_cost")]
        public float? DeclaredCost { get; set; }

        /// <summary>
        /// Уникальный идентификатор заказа в магазине
        /// </summary>
        [JsonProperty("external_id")]
        public string OrderNumber { get; set; }
        /// <summary>
        /// Данные об отправлении
        /// </summary>
        [JsonProperty("departure")]
        public PackageDeparture Departure { get; set; }

        /// <summary>
        /// Список продуктов
        /// </summary>
        [JsonProperty("products")]
        public List<PackageProduct> Products { get; set; }

        /// <summary>
        /// Массив фотографий посылки, закодированных в base64
        /// </summary>
        [JsonProperty("photos")]
        public string[] Photos { get; set; }

        /// <summary>
        /// Список услуг
        /// </summary>
        [JsonProperty("services")]
        public List<PackageService> Services { get; set; }

        /// <summary>
        /// Список дополнительных услуг
        /// </summary>
        [JsonProperty("additional_service")]
        public List<EnPackageService> AdditionalService { get; set; }
    }

    public class PackageDeparture
    {
        /// <summary>
        /// ID способа доставки
        /// </summary>
        [JsonProperty("shipping_method")]
        public int ShippingMethodId { get; set; }

        /// <summary>
        /// ID пункта самовывоза
        /// </summary>
        [JsonProperty("delivery_point")]
        public int? DeliveryPointId { get; set; }

        /// <summary>
        /// Рекомендованное время доставки (для доставки Shiptor)
        /// </summary>
        [JsonProperty("delivery_time")]
        public int? DeliveryTimeId { get; set; }

        /// <summary>
        /// Рекомендованное время доставки в виде строки (для доставки Shiptor)
        /// </summary>
        [JsonProperty("delivery_time_string")]
        public string DeliveryTimeString { get; set; }

        /// <summary>
        /// Желаемая дата отправки груза со склада в формате Y-m-d (для доставки Shiptor)
        /// </summary>
        [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd")]
        [JsonProperty("delayed_delivery_at")]
        public DateTime? DelayedDeliveryAt { get; set; }

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
        public DepartureAddress Address { get; set; }
    }

    public class DepartureAddress
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
        [JsonProperty("country")]
        public string Country { get; set; }

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
        /// Адрес одной строкой
        /// </summary>
        [JsonProperty("address_line_1")]
        public string Address { get; set; }

        /// <summary>
        /// Адрес 2. Для иностранных адресов максимальное значение до 50 символов.
        /// </summary>
        [JsonProperty("address_line_2")]
        public string Address2 { get; set; }

        /// <summary>
        /// Код КЛАДР населенного пункта, можно получить из справочника населенных пунктов.
        /// </summary>
        [JsonProperty("kladr_id")]
        public string KladrId { get; set; }
    }

    public class PackageProduct
    {
        /// <summary>
        /// Артикул в магазине
        /// </summary>
        [JsonProperty("shopArticle")]
        public string ArtNo { get; set; }

        /// <summary>
        /// Название на английском
        /// </summary>
        [JsonProperty("englishName")]
        public string EnglishName { get; set; }

        /// <summary>
        /// Количество товаров
        /// </summary>
        [JsonProperty("count")]
        public float Count { get; set; }

        /// <summary>
        /// Цена продажи, обязательно если товар ранее не существовал
        /// </summary>
        [JsonProperty("price")]
        public float Price { get; set; }

        /// <summary>
        /// Ндс, если не задано берётся из настрок аккаунта (допустимо null, 0, 10, 18)
        /// </summary>
        [JsonProperty("vat")]
        public int? Vat { get; set; }
    }

    public class PackageService
    {
        /// <summary>
        /// Артикул в магазине
        /// </summary>
        [JsonProperty("shopArticle")]
        public string ArtNo { get; set; }

        /// <summary>
        /// Количество оказаных услуг
        /// </summary>
        [JsonProperty("count")]
        public float Count { get; set; }

        /// <summary>
        /// Цена услуги
        /// </summary>
        [JsonProperty("price")]
        public float Price { get; set; }

        /// <summary>
        /// Ндс, если не задано берётся из настрок аккаунта (допустимо null, 0, 10, 18)
        /// </summary>
        [JsonProperty("vat")]
        public int? Vat { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum EnPackageService
    {
        [EnumMember(Value = "additional-pack")] AdditionalPack,
        [EnumMember(Value = "express-gathering")] ExpressGathering,
        [EnumMember(Value = "partial-pay-out")] PartialPayOut,
        [EnumMember(Value = "package-insurance")] PackageInsurance,
    }

    public class Settlement
    {
        [JsonProperty("kladr_id")]
        public string KladrId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }

        [JsonProperty("type_short")]
        public string TypeShort { get; set; }
        public List<Settlement> Parents { get; set; }

    }

    public partial class SuggestSettlement
    {
        [JsonProperty("kladr_id")]
        public string KladrId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("short_readable")]
        public string ShortReadable { get; set; }

        [JsonProperty("type_short")]
        public string TypeShort { get; set; }

        [JsonProperty("administrative_area")]
        public string AdministrativeArea { get; set; }

        [JsonProperty("readable_parents")]
        public string ReadableParents { get; set; }

        [JsonProperty("country")]
        public SuggestCountry Country { get; set; }
    }

    public partial class SuggestCountry
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }
    }
    public class ShippingMethod
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public EnTypeCategory? Category { get; set; }
        public string Group { get; set; }
        //public EnTypeCourier? Courier { get; set; }
        public string Courier { get; set; }
        public string Comment { get; set; }
        public string Description { get; set; }
    }

    public class PackageStatus
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        // [JsonProperty("group")]
        // public PackageStatusGroup Group { get; set; }
    }

    public partial class PackageStatusGroup
    {
        [JsonProperty("standard")]
        public string Standard { get; set; }

        [JsonProperty("through")]
        public string Through { get; set; }

        [JsonProperty("fulfilment")]
        public string Fulfilment { get; set; }

        [JsonProperty("all")]
        public string All { get; set; }
    }
}
