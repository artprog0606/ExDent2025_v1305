using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AdvantShop.Shipping.Shiptor.Api
{
    public class PageParams
    {
        public PageParams()
        {
            Page = 1;
            PerPage = 10;
        }

        public int Page { get; set; }
        [JsonProperty("per_page")] public int PerPage { get; set; }
    }

    #region Settlement

    public class GetSettlementsParams : PageParams
    {
        /// <summary>
        /// Тип (Optional)
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string[] Types { get; set; }

        /// <summary>
        /// Уровень
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// Кладр родителя
        /// </summary>
        [JsonProperty("parent")]
        public string ParentKladrId { get; set; }

        /// <summary>
        /// Индетификатор страны
        /// </summary>
        [JsonProperty("country_code")]
        public string CountryIso2 { get; set; }
    }

    public class SimpleSuggestSettlementParams
    {
        /// <summary>
        /// Строка, по которой будет выполняться поиск
        /// </summary>
        [JsonProperty("query")]
        public string Query { get; set; }

        /// <summary>
        /// Код страны
        /// </summary>
        [JsonProperty("country_code", NullValueHandling = NullValueHandling.Ignore)]
        public string CountryIso2 { get; set; }
    }

    #endregion

    #region CalculateShipping

    public class CalculateShippingParams
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
        public float CashOnDelivery { get; set; }

        /// <summary>
        /// Объявленная ценность. Минимально допустимое значение - 10.
        /// </summary>
        [JsonProperty("declared_cost")]
        public float DeclaredCost { get; set; }

        /// <summary>
        /// Код страны для расчета (Optional)
        /// </summary>
        [JsonProperty("country_code", NullValueHandling = NullValueHandling.Ignore)]
        public string CountryIso2 { get; set; }

        /// <summary>
        /// Курьерская служба (Optional) Допустимые значения: "shiptor", "boxberry", "cdek", "dpd", "bpost", "pickpoint", "russian-post", "shiptor-one-day", "shiptor-oversize"
        /// </summary>
        [JsonProperty("courier", NullValueHandling = NullValueHandling.Ignore)]
        //public EnTypeCourier? Courier { get; set; }
        public string Courier { get; set; }

        /// <summary>
        /// Тип забора (Optional)
        /// </summary>
        [JsonProperty("pick_up_type", NullValueHandling = NullValueHandling.Ignore)]
        public EnExportType? ExportType { get; set; }

        /// <summary>
        /// Идентификатор КЛАДР населенного пункта отправителя (Optional)
        /// </summary>
        [JsonProperty("kladr_id_from", NullValueHandling = NullValueHandling.Ignore)]
        public string SettlementFromKladrId { get; set; }

        /// <summary>
        /// Идентификатор КЛАДР населенного пункта получателя
        /// </summary>
        [JsonProperty("kladr_id")]
        public string SettlementToKladrId { get; set; }

        /// <summary>
        /// Оплата картой (Optional)
        /// </summary>
        [JsonProperty("cashless_payment", NullValueHandling = NullValueHandling.Ignore)]
        public bool? CashlessPayment { get; set; }
    }

    public class SimpleCalculateParams
    {
        /// <summary>
        /// Длина в см, можно передать 0 и тогда значение будет взято из настроек габаритов по умолчанию
        /// </summary>
        public float Length { get; set; }

        /// <summary>
        /// Ширина в см, можно передать 0 и тогда значение будет взято из настроек габаритов по умолчанию
        /// </summary>
        public float Width { get; set; }

        /// <summary>
        /// Высота в см, можно передать 0 и тогда значение будет взято из настроек габаритов по умолчанию
        /// </summary>
        public float Height { get; set; }

        /// <summary>
        /// Вес в кг, можно передать 0 и тогда значение будет взято из настроек габаритов по умолчанию
        /// </summary>
        public float Weight { get; set; }

        /// <summary>
        /// Сумма наложенного платежа в руб, если отсутствует — передавайте 0
        /// </summary>
        [JsonProperty("cod")]
        public float CashOnDelivery { get; set; }

        /// <summary>
        /// Сумма оценочной стоимости в руб, если присутствует наложенный платеж, то оценочная стоимость должна быть равна ему, в противном случае сюда можно указывать любую сумму от 10 руб и больше.
        /// </summary>
        [JsonProperty("declared_cost")]
        public float DeclaredCost { get; set; }

        /// <summary>
        /// Можно ограничить возвращаемые методы доставки конкретной курьерской службой, указав ее код. Допустимые значения: "shiptor", "boxberry", "cdek", "dpd", "bpost", "pickpoint", "russian-post", "shiptor-one-day", "shiptor-oversize"
        /// </summary>
        [JsonProperty("courier", NullValueHandling = NullValueHandling.Ignore)]
        //public EnTypeCourier? Courier { get; set; }
        public string Courier { get; set; }

        /// <summary>
        /// Идентификатор КЛАДР населенного пункта отправителя (Optional)
        /// </summary>
        [JsonProperty("kladr_id_from", NullValueHandling = NullValueHandling.Ignore)]
        public string SettlementFromKladrId { get; set; }

        /// <summary>
        /// Идентификатор КЛАДР населенного пункта получателя
        /// </summary>
        [JsonProperty("kladr_id")]
        public string SettlementToKladrId { get; set; }

        /// <summary>
        /// Код страны для расчета (Optional)
        /// </summary>
        [JsonProperty("country_code", NullValueHandling = NullValueHandling.Ignore)]
        public string CountryIso2 { get; set; }

        public List<BasketItem> Basket { get; set; }
    }

    public class BasketItem
    {
        /// <summary>
        /// Артикул товара в магазине
        /// </summary>
        [JsonProperty("id")]
        public string ArtNo { get; set; }

        /// <summary>
        /// Название товара (Optional)
        /// </summary>
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        /// <summary>
        /// Стоимость 1 единицы товара
        /// </summary>
        [JsonProperty("price")]
        public float Price { get; set; }

        /// <summary>
        /// Количество товара в этой позиции
        /// </summary>
        [JsonProperty("quantity")]
        public int Amount { get; set; }

        /// <summary>
        /// Длина в см, можно передать null или 0, тогда габарит будет взят из настроек КЧ
        /// </summary>
        [JsonProperty("length")]
        public float? Length { get; set; }

        /// <summary>
        /// Ширина в см, можно передать null или 0, тогда габарит будет взят из настроек КЧ
        /// </summary>
        [JsonProperty("width")]
        public float? Width { get; set; }

        /// <summary>
        /// Высота в см, можно передать null или 0, тогда габарит будет взят из настроек КЧ
        /// </summary>
        [JsonProperty("height")]
        public float? Height { get; set; }

        /// <summary>
        /// Вес в кг, можно передать null или 0, тогда вес будет взять из настроек КЧ
        /// </summary>
        [JsonProperty("weight")]
        public float? Weight { get; set; }

    }

    //[JsonConverter(typeof(StringEnumConverter))]
    //public enum EnTypeCourier
    //{
    //    [EnumMember(Value = "shiptor")] Shiptor,
    //    [EnumMember(Value = "boxberry")] Boxberry,
    //    [EnumMember(Value = "cdek")] Cdek,
    //    [EnumMember(Value = "dpd")] Dpd,
    //    [EnumMember(Value = "iml")] Iml,
    //    [EnumMember(Value = "pec")] Pec,
    //    [EnumMember(Value = "bpost")] Bpost,
    //    [EnumMember(Value = "pickpoint")] PickPoint,
    //    [EnumMember(Value = "russian-post")] RussianPost,
    //    [EnumMember(Value = "shiptor-one-day")] ShiptorOneDay,
    //    [EnumMember(Value = "shiptor-oversize")] ShiptorOversize,
    //    [EnumMember(Value = "svyaznoy")] Svyaznoy,
    //}

    [JsonConverter(typeof(StringEnumConverter))]
    public enum EnTypeCategory
    {
        /// <summary>
        /// Пункт самовывоза — Пункт самовывоза
        /// </summary>
        [EnumMember(Value = "delivery-point-to-delivery-point")] DeliveryPointToDeliveryPoint,

        /// <summary>
        /// Дверь — Пункт самовывоза
        /// </summary>
        [EnumMember(Value = "door-to-delivery-point")] DoorToDeliveryPoint,

        /// <summary>
        /// Дверь — Дверь
        /// </summary>
        [EnumMember(Value = "door-to-door")] DoorToDoor,

        /// <summary>
        /// Пункт самовывоза — Дверь
        /// </summary>
        [EnumMember(Value = "delivery-point-to-door")] DeliveryPointToDoor,

        /// <summary>
        /// В пункты самовывоза, постоматы
        /// </summary>
        [EnumMember(Value = "delivery-point")] DeliveryPoint,

        /// <summary>
        /// До двери
        /// </summary>
        [EnumMember(Value = "to-door")] ToDoor,

        /// <summary>
        /// В отделение почты РФ
        /// </summary>
        [EnumMember(Value = "post-office")] PostOffice,
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum EnExportType
    {
        [EnumMember(Value = "courier")] Courier,
        [EnumMember(Value = "terminal")] Terminal,
        [EnumMember(Value = "independently-to-shiptor")] IndependentlyToShiptor,
        [EnumMember(Value = "from-provider")] FromProvider,
    }

    #endregion

    #region SetProduct

    public class SetProductParams
    {
        /// <summary>
        /// Товарный артикул магазина
        /// </summary>
        [JsonProperty("shop_article")]
        public string ShopArtNo { get; set; }

        /// <summary>
        /// артикул товара (Optional)
        /// </summary>
        [JsonProperty("article", NullValueHandling = NullValueHandling.Ignore)]
        public string ArtNo { get; set; }

        /// <summary>
        /// Название товара (Optional)
        /// </summary>
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        /// <summary>
        /// Название товара на английском (Optional)
        /// </summary>
        [JsonProperty("englishName", NullValueHandling = NullValueHandling.Ignore)]
        public string EnglishName { get; set; }

        /// <summary>
        /// Бренд товара (Optional)
        /// </summary>
        [JsonProperty("brand", NullValueHandling = NullValueHandling.Ignore)]
        public string Brand { get; set; }

        /// <summary>
        /// УРЛ товара на витрине (Optional)
        /// </summary>
        [JsonProperty("url", NullValueHandling = NullValueHandling.Ignore)]
        public string Url { get; set; }

        /// <summary>
        /// Длина в см (Optional)
        /// </summary>
        [JsonProperty("length", NullValueHandling = NullValueHandling.Ignore)]
        public float? Length { get; set; }

        /// <summary>
        /// Ширина в см (Optional)
        /// </summary>
        [JsonProperty("width", NullValueHandling = NullValueHandling.Ignore)]
        public float? Width { get; set; }

        /// <summary>
        /// Высота в см (Optional)
        /// </summary>
        [JsonProperty("height", NullValueHandling = NullValueHandling.Ignore)]
        public float? Height { get; set; }

        /// <summary>
        /// Вес в кг (Optional)
        /// </summary>
        [JsonProperty("weight", NullValueHandling = NullValueHandling.Ignore)]
        public float? Weight { get; set; }

        /// <summary>
        /// Розничная цена в руб (Optional)
        /// </summary>
        [JsonProperty("retail_price", NullValueHandling = NullValueHandling.Ignore)]
        public float? RetailPrice { get; set; }

        /// <summary>
        /// оптовая цена в руб (Optional)
        /// </summary>
        [JsonProperty("price", NullValueHandling = NullValueHandling.Ignore)]
        public float? SupplyPrice { get; set; }

        /// <summary>
        /// Признак хрупкого товара (Optional)
        /// </summary>
        [JsonProperty("fragile", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Fragile { get; set; }

        /// <summary>
        /// Признак опасного товара (Optional)
        /// </summary>
        [JsonProperty("danger", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Danger { get; set; }

        /// <summary>
        /// признак скоропортящегося товара (Optional)
        /// </summary>
        [JsonProperty("perishable", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Perishable { get; set; }

        /// <summary>
        /// признак потребности в упаковке товара (Optional)
        /// </summary>
        [JsonProperty("need_box", NullValueHandling = NullValueHandling.Ignore)]
        public bool? NeedBox { get; set; }

        /// <summary>
        /// Товар для взрослых (Optional)
        /// </summary>
        [JsonProperty("adult", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Adult { get; set; }

        /// <summary>
        /// штрихкод товара (Optional)
        /// </summary>
        [JsonProperty("barcode", NullValueHandling = NullValueHandling.Ignore)]
        public string Barcode { get; set; }

    }

    #endregion

    #region Package

    #region AddOrder

    public class AddOrderParams
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
        /// Уникальный идентификатор заказа в магазине
        /// </summary>
        [JsonProperty("external_id")]
        public string OrderNumber { get; set; }

        /// <summary>
        /// Сумма по товарам в заказе
        /// </summary>
        [JsonProperty("price")]
        public float ProductsPrice { get; set; }

        /// <summary>
        /// Стоимость доставки
        /// </summary>
        [JsonProperty("deliveryPrice")]
        public float ShippingCost { get; set; }

        /// <summary>
        /// Объявленная стоимость товаров в заказе
        /// </summary>
        [JsonProperty("declared_cost")]
        public float DeclaredCost { get; set; }

        /// <summary>
        /// Признак наложенного платежа
        /// </summary>
        [JsonProperty("cod")]
        public bool CashOnDelivery { get; set; }

        /// <summary>
        /// Тип оплаты заказа (Optional)
        /// </summary>
        [JsonProperty("payment", NullValueHandling = NullValueHandling.Ignore)]
        public EnPaymentType? PaymentType { get; set; }

        /// <summary>
        /// Флаг оплаченности заказа
        /// </summary>
        [JsonProperty("payed")]
        public bool IsPayed { get; set; }

        /// <summary>
        /// Данные об отправлении
        /// </summary>
        [JsonProperty("departure")]
        public AddOrderDeparture Departure { get; set; }

        /// <summary>
        /// Список продуктов
        /// </summary>
        [JsonProperty("products")]
        public List<AddOrderProduct> Products { get; set; }
    }

    public class AddOrderDeparture
    {
        /// <summary>
        /// Идентификатор метода доставки
        /// </summary>
        [JsonProperty("shipping_method")]
        public int ShippingMethodId { get; set; }

        /// <summary>
        /// Идентификатор выбранного ПВЗ (если выбран метод доставки с ПВЗ)
        /// </summary>
        [JsonProperty("delivery_point", NullValueHandling = NullValueHandling.Ignore)]
        public string DeliveryPointId { get; set; }

        /// <summary>
        /// Комментарий (Optional)
        /// </summary>
        [JsonProperty("comment", NullValueHandling = NullValueHandling.Ignore)]
        public string Comment { get; set; }

        /// <summary>
        /// Данные об адресе доставки
        /// </summary>
        [JsonProperty("address")]
        public AddOrderAddress Address { get; set; }
    }

    public class AddOrderAddress
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
        /// Отчество (Optional)
        /// </summary>
        [JsonProperty("patronymic", NullValueHandling = NullValueHandling.Ignore)]
        public string Patronymic { get; set; }

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
        /// Почтовый код, обязателен для Почты России
        /// </summary>
        [JsonProperty("postal_code", NullValueHandling = NullValueHandling.Ignore)]
        public string PostalCode { get; set; }

        /// <summary>
        /// Страна  Допустимые значения: "RU", "KZ", "BY"
        /// </summary>
        [JsonProperty("country")]
        public string CountryIso2 { get; set; }

        /// <summary>
        /// Название населеного пункта назначения.
        /// </summary>
        [JsonProperty("settlement")]
        public string Settlement { get; set; }

        /// <summary>
        /// Название улицы назначения
        /// </summary>
        [JsonProperty("street")]
        public string Street { get; set; }

        /// <summary>
        /// Номер дома
        /// </summary>
        [JsonProperty("house")]
        public string House { get; set; }

        /// <summary>
        /// Номер квартиры (Optional)
        /// </summary>
        [JsonProperty("apartment", NullValueHandling = NullValueHandling.Ignore)]
        public string Apartment { get; set; }

        /// <summary>
        /// КЛАДР код населенного пункта назначения
        /// </summary>
        [JsonProperty("kladr_id", NullValueHandling = NullValueHandling.Ignore)]
        public string KladrId { get; set; }
    }

    public class AddOrderProduct
    {
        /// <summary>
        /// Артикул товара в магазине
        /// </summary>
        [JsonProperty("id")]
        public string ArtNo { get; set; }

        /// <summary>
        /// Название товара (Optional)
        /// </summary>
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        /// <summary>
        /// Стоимость 1 единицы товара
        /// </summary>
        [JsonProperty("price")]
        public float Price { get; set; }

        /// <summary>
        /// Количество товара в этой позиции
        /// </summary>
        [JsonProperty("quantity")]
        public int Count { get; set; }

        /// <summary>
        /// Ндс, если не задано берётся из настрок аккаунта (допустимо null, 0, 10, 18)
        /// </summary>
        //[JsonProperty("vat")]
        //public int? Vat { get; set; }
    }

    /// <summary>
    /// Тип оплаты заказа
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum EnPaymentType
    {
        /// <summary>
        /// картой
        /// </summary>
        [EnumMember(Value = "card")] Card,
        /// <summary>
        /// наличными
        /// </summary>
        [EnumMember(Value = "cash")] Cash,
    }

    #endregion

    #region GetPackage

    public class GetPackageParams
    {
        /// <summary>
        /// Идентификационный номер посылки
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? Id { get; set; }
        /// <summary>
        /// Идентификационный номер посылки в магазине
        /// </summary>
        [JsonProperty("external_id", NullValueHandling = NullValueHandling.Ignore)]
        public string OrderNumber { get; set; }
    }

    #endregion

    #endregion

    #region Warehouse

    public class GetWarehouseParams
    {
        public int Id { get; set; }
    }

    #endregion
}
