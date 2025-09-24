using System;
using System.Collections.Generic;
using AdvantShop.Core.Common.Attributes;
using Newtonsoft.Json;

namespace AdvantShop.Core.Services.Shipping.PickPoint.Api
{
    public abstract class ObjectOfSession
    {
        public string SessionId { get; set; }
    }

    #region Authentication

    public class AuthenticationParams
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string Ikn { get; set; }
    }

    public class AuthenticationResponse
    {
        [PickPointErrorMessage]
        public string Error { get; set; }

        [PickPointErrorCode]
        public int ErrorCode { get; set; }

        public List<AuthenticationCity> Cities { get; set; }
        public bool Success { get; set; }
    }

    public class AuthenticationCity
    {
        public string Area { get; set; }
        public string AreaFiasId { get; set; }
        public string AreaKladrId { get; set; }
        public string AreaTypeFull { get; set; }
        public string City { get; set; }
        public string CityDistrict { get; set; }
        public string CityDistrictFiasId { get; set; }
        public string CityDistrictKladrId { get; set; }
        public string CityDistrictTypeFull { get; set; }
        public string CityFiasId { get; set; }
        public string CityKladrId { get; set; }
        public string CityTypeFull { get; set; }
        public string Country { get; set; }
        public double? GeoLat { get; set; }
        public double? GeoLon { get; set; }
        public int Id { get; set; }
        public string ObjectFiasId { get; set; }
        public string ObjectKladrId { get; set; }
        public int OwnerId { get; set; }
        public string Region { get; set; }
        public string RegionFiasId { get; set; }
        public string RegionKladrId { get; set; }
        public string RegionTypeFull { get; set; }
        public string Settlement { get; set; }
        public string SettlementFiasId { get; set; }
        public string SettlementKladrId { get; set; }
        public string SettlementTypeFull { get; set; }
    }

    #endregion Authentication

    #region Login

    public class LoginParams
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }

    public class LoginResponse : ObjectOfSession
    {
        [PickPointErrorCode]
        public int ErrorCode { get; set; }
        [PickPointErrorMessage]
        public string ErrorMessage { get; set; }
    }

    #endregion Login

    #region CreateShipment

    public class CreateShipmentParams : ObjectOfSession
    {
        public List<Shipment> Sendings { get; set; }

        public string Source { get { return "32"; } }
    }

    public class CreateShipmentResponse
    {
        public List<CreatedSending> CreatedSendings { get; set; }
        public List<RejectedSending> RejectedSendings { get; set; }
    }

    public class Shipment
    {
        /// <summary>
        /// Идентификатор запроса, используемый для ответа. Указывайте уникальное число (50 символов)
        /// </summary>
        [JsonProperty("EDTN")]
        public string IdOfRequest { get; set; }

        [JsonProperty("IKN")]
        public string Ikn { get; set; }

        /// <summary>
        /// Номер клиента в системе агрегатора (отражается в возвратных накладных от PickPoint), обязательное поле, если у ИКН проставлен флаг в CRM "Является агрегатором"
        /// </summary>
        public string ClientNumber { get; set; }

        /// <summary>
        /// Наименование клиента в системе агрегатора, необязательное поле. Следует заполнять только при регистрации КО переданный в запросе ИКН принадлежит клиенту-агрегатору
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// Наименование на русском для отображения на сайте в мониторинге PickPoint, необязательное поле
        /// </summary>
        public string TittleRus { get; set; }

        /// <summary>
        /// Наименование на английском для отображения на сайте в мониторинге PickPoint, необязательное поле
        /// </summary>
        public string TittleEng { get; set; }

        public InvoiceShipment Invoice { get; set; }
    }

    public class InvoiceShipment
    {
        /// <summary>
        /// Номер заказа магазина (50 символов)
        /// </summary>
        [JsonProperty("SenderCode")]
        public string OrderNumber { get; set; }

        /// <summary>
        /// Описание отправления, обязательное поле (200 символов)
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Имя получателя, обязательное поле (150 символов)
        /// </summary>
        public string RecipientName { get; set; }

        /// <summary>
        /// Номер постамата, обязательное поле (8 символов)
        /// </summary>
        public string PostamatNumber { get; set; }

        /// <summary>
        /// Номер ПТ клиента
        /// </summary>
        public string ClientPostamatNumber { get; set; }

        /// <summary>
        /// один номер телефона получателя, обязательное поле (100 символов)
        /// <para>Для России + 7 (далее 10 цифр без пробелов, скоробок, тире) пример: +79151234352</para>
        /// <para>Для Белоруссии +375 (далее 9 цифр без пробелов, скобок, тире) пример: +375786142536</para>
        /// </summary>
        public string MobilePhone { get; set; }

        /// <summary>
        /// Адрес электронной почты получателя (256 символов)
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Номер консультанта, строка 50 символов
        /// </summary>
        public string ConsultantNumber { get; set; }

        /// <summary>
        /// Тип услуги, обязательное поле 
        /// </summary>
        public PostageType PostageType { get; set; }

        /// <summary>
        /// Тип сдачи отправления, обязательное поле 
        /// </summary>
        public GettingType GettingType { get; set; }

        /// <summary>
        /// Тип оплаты, обязательное поле 
        /// </summary>
        public byte PayType { get { return 1; } }

        private float _sum;
        /// <summary>
        /// Сумма к оплате, обязательное поле (число, два знака после запятой)
        /// </summary>
        public float Sum { get {return (float)Math.Round(_sum, 2); } set { _sum = value; } }

        /// <summary>
        /// Сумма предоплаты, если платеж уже был внесен частично
        /// </summary>
        public float PrepaymentSum { get; set; }

        private float _insuareValue;
        /// <summary>
        /// Сумма страховки (число, два знака после запятой)
        /// </summary>
        public float InsuareValue { get {return (float)Math.Round(_insuareValue, 2); } set { _insuareValue = value; } }

        /// <summary>
        /// Ставка НДС по сервисному сбору 
        /// </summary>
        [JsonIgnore]
        public Vat DeliveryVat { get; set; }

        [JsonProperty("DeliveryVat")]
        public int? DeliveryVatForJson
        {
            get { return DeliveryVat == Vat.WithoutVat ? (int?)null : (int)DeliveryVat; }
        }

        /// <summary>
        /// Сумма сервисного сбора с НДС, если берется с физ. лица
        /// </summary>
        public float DeliveryFee { get; set; }

        /// <summary>
        /// Режим доставки 
        /// </summary>
        public DeliveryMode DeliveryMode { get; set; }

        ///// <summary>
        ///// Клиентский срок доставки
        ///// </summary>
        //public ClientDeliveryPeriod ClientDeliveryPeriod { get; set; }

        ///// <summary>
        ///// Клиентская дата доставки 
        ///// </summary>
        //public object ClientDeliveryDate { get; set; }

        /// <summary>
        /// Город сдачи отправлений в PickPoint
        /// </summary>
        public SenderCity SenderCity { get; set; }

        ///// <summary>
        ///// Адрес клиентского возврата
        ///// </summary>
        //public ReturnAddress ClientReturnAddress { get; set; }

        ///// <summary>
        ///// Адрес возврата невостребованного 
        ///// </summary>
        //public ReturnAddress UnclaimedReturnAddress { get; set; }

        public List<InvoicePlace> Places { get; set; }
    }

    public class InvoicePlace
    {
        private float _width;
        private float _height;
        private float _depth;
        private float _weight;

        /// <summary>
        /// Штрих код от PickPoint. Отправляйте поле пустым, в ответ будет ШК (50 символов)
        /// </summary>
        public string BarCode { get; set; }

        /// <summary>
        /// Клиентский штрих-код. Поле не обязательное. Можно не отправлять (255 символов)
        /// </summary>
        public string GCBarCode { get; set; }

        /// <summary>
        /// Ширина (число, два знака после запятой). Если не знаете точных габаритов, данное поле можно не передавать 
        /// </summary>
        public float Width {
            get
            {
                return (float)Math.Round(_width, 2);
            }
            set { _width = value; }
        }

        /// <summary>
        /// Высота (число, два знака после запятой). Если не знаете точных габаритов, данное поле можно не передавать 
        /// </summary>
        public float Height
        {
            get
            {
                return (float)Math.Round(_height, 2);
            }
            set { _height = value; }
        }

        /// <summary>
        /// лубина(число, два знака после запятой. Если не знаете точных габаритов, данное поле можно не передавать 
        /// </summary>
        public float Depth
        {
            get
            {
                return (float)Math.Round(_depth, 2);
            }
            set { _depth = value; }
        }

        /// <summary>
        /// Вес (число, два знака после запятой). Если не знаете точных габаритов, данное поле можно не передавать 
        /// </summary>
        public float Weight
        {
            get
            {
                return (float)Math.Round(_weight, 2);
            }
            set { _weight = value; }
        }

        /// <summary>
        /// Субвложимые
        /// </summary>
        public List<SubEnclose> SubEncloses { get; set; }
    }

    public class SubEnclose
    {
        /// <summary>
        /// Артикул товара(50 символов)
        /// </summary>
        public string ProductCode { get; set; }

        /// <summary>
        /// ШК товара(50 символов)
        /// </summary>
        public string GoodsCode { get; set; }

        /// <summary>
        /// Наименование товара(200 символов)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Стоимость ед. товара с НДС
        /// </summary>
        public float Price { get; set; }

        /// <summary>
        /// Кол-во ед. товара одного арт.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Ставка НДС по товару
        /// </summary>
        [JsonIgnore]
        public Vat Vat { get; set; }

        [JsonProperty("Vat")]
        public int? VatForJson
        {
            get { return Vat == Vat.WithoutVat ? (int?)null : (int)Vat; }
        }

        /// <summary>
        /// Описание товара 
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// код маркировки
        /// </summary>
        public int Upi { get; set; }

        /// <summary>
        /// ИНН принципала
        /// </summary>
        public string PrincipalINN { get; set; }

        /// <summary>
        /// Наименование юр.лица принципала
        /// </summary>
        public string PrincipalName { get; set; }

        /// <summary>
        /// Номер телефона принципала. Допускаются круглые скобки и тире.
        /// </summary>
        public string PrincipalPhoneNumber { get; set; }
    }

    public class SenderCity
    {
        /// <summary>
        /// Название города сдачи отправления
        /// </summary>
        public string CityName { get; set; }

        /// <summary>
        /// Название региона сдачи отправления
        /// </summary>
        public string RegionName { get; set; }
    }

    public class CreatedSending
    {
        /// <summary>
        /// Идентификатор запроса, используемый для ответа. Указывайте уникальное число (50 символов)
        /// </summary>
        [JsonProperty("EDTN")]
        public string IdOfRequest { get; set; }

        public string InvoiceNumber { get; set; }
        [JsonProperty("SenderCode")]
        public string OrderNumber { get; set; }

        public List<CreatedSendingPlace> Places { get; set; }
    }

    public class CreatedSendingPlace {
        public string Barcode { get; set; }
        public string GCBarCode { get; set; }
        public CellStorageType CellStorageType { get; set; }
    }

    public class RejectedSending
    {
        /// <summary>
        /// Идентификатор запроса, используемый для ответа. Указывайте уникальное число (50 символов)
        /// </summary>
        [JsonProperty("EDTN")]
        public string IdOfRequest { get; set; }
        [JsonProperty("SenderCode")]
        public string OrderNumber { get; set; }

        [PickPointErrorCode]
        public int ErrorCode { get; set; }

        [PickPointErrorMessage]
        public string ErrorMessage { get; set; }
    }

    public enum PostageType
    {
        /// <summary>
        /// Стандарт. Оплаченный заказ. При этом нужно передавать поле «Sum=0»
        /// </summary>
        Paid = 10001,

        /// <summary>
        /// Стандарт НП Отправление с наложенным платежом. При этом нужно передавать поле «Sum>0»
        /// </summary>
        Cod = 10003
    }

    public enum GettingType
    {
        /// <summary>
        /// «вызов курьера» -Наш курьер приедет к вам за отправлениями.
        /// </summary>
        [Localize("Вызов курьера")]
        Courier = 101,

        /// <summary>
        /// «в окне приема СЦ» - Вы привезете отправления в филиал PickPoint
        /// </summary>
        [Localize("В окне приема СЦ")]
        Bring = 102,

        /// <summary>
        /// «в окне приема ПТ валом»
        /// </summary>
        [Localize("В окне приема ПТ валом")]
        PTBatch = 103,

        /// <summary>
        /// «в окне приема ПТ» (самостоятельный развоз в нужный ПТ + при создании отправления у ПТ - С2С)
        /// </summary>
        [Localize("В окне приема ПТ")]
        PT = 104
    }

    public enum Vat
    {
        Vat0 = 0,
        Vat10 = 10,
        Vat20 = 20,
        WithoutVat = -1
    }

    public enum DeliveryMode
    {
        [Localize("Стандарт")]
        Standard = 1,
        [Localize("Приоритет")]
        Priority = 2
    }

    public enum CellStorageType
    {
        Standart = 0,
        Fridge = 1,
        Freezer = 2
    }

    #endregion

    #region CancelInvoice

    public class CancelInvoiceParams : ObjectOfSession
    {
        [JsonProperty("IKN")]
        public string Ikn { get; set; }

        /// <summary>
        /// Номер КО
        /// </summary>
        public string InvoiceNumber { get; set; }

        /// <summary>
        /// Номер отправления клиента
        /// </summary>
        public string GCInvoiceNumber { get; set; }
    }

    public class CancelInvoiceResponse
    {
        public bool Result { get; set; }
        [PickPointErrorCode] public int ErrorCode { get; set; }
        [PickPointErrorMessage] public string Error { get; set; }
    }

    #endregion

    #region CalcTariff

    public class CalcTariffParams : ObjectOfSession
    {
        /// <summary>
        /// Номер контракта
        /// </summary>
        [JsonProperty("IKN")]
        public string Ikn { get; set; }

        /// <summary>
        /// Номер отправления, не обязательное поле
        /// </summary>
        public string InvoiceNumber { get; set; }

        /// <summary>
        /// Город сдачи отправления
        /// </summary>
        public string FromCity { get; set; }

        /// <summary>
        /// Регион города сдачи отправления
        /// </summary>
        public string FromRegion { get; set; }

        /// <summary>
        /// Регион назначения
        /// </summary>
        public string ToRegion { get; set; }

        /*
         * Поля «PTNumber» и «ToCity» взаимоисключающие.
         * Если заполнены поля «PTNumber» и «ToCity», то будет обработан «PTNumber» если «PTNumber» не указан, то будет обработан «ToCity».
         * При заполненном параметре «ToCity», параметр «ToRegion» обязателен к заполнению.
         */

        /// <summary>
        /// Город назначения
        /// </summary>
        public string ToCity { get; set; }

        /// <summary>
        /// Пункт выдачи (назначения) отправления
        /// </summary>
        [JsonProperty("PTNumber")]
        public string PostamatNumber { get; set; }

        /// <summary>
        /// Вид приема, не обязательное поле 
        /// </summary>
        public GettingType? GettingType { get; set; }

        /// <summary>
        /// Количество мест, по умолчанию одно, не обязательное поле
        /// </summary>
        public int? EncloseCount { get; set; }

        /// <summary>
        /// Длина отправления, см
        /// </summary>
        public float Length { get; set; }

        /// <summary>
        /// Глубина отправления, см
        /// </summary>
        public float Depth { get; set; }

        /// <summary>
        /// Ширина отправления, см
        /// </summary>
        public float Width { get; set; }

        /// <summary>
        /// Вес отправления, не обязательное поле, по умолчанию 1кг
        /// </summary>
        public float? Weight { get; set; }
    }

    public class CalcTariffResponse
    {
        public List<CalcTariffService> Services { get; set; }

        /// <summary>
        /// Номер накладной
        /// </summary>
        public string InvoiceNumber { get; set; }

        /// <summary>
        /// Минимальный срок доставки (согласно режиму “Стандарт”)
        /// </summary>
        public int DPMin { get; set; }

        /// <summary>
        /// Минимальный срок доставки (согласно режиму “Приоритет”)
        /// </summary>
        public int DPMinPriority { get; set; }

        /// <summary>
        /// Максимальный срок доставки (согласно режиму “Стандарт”)
        /// </summary>
        public int DPMax { get; set; }

        /// <summary>
        /// Максимальный срок доставки (согласно режиму “Приоритет”)
        /// </summary>
        public int DPMaxPriority { get; set; }

        /// <summary>
        /// Зона
        /// </summary>
        public int Zone { get; set; }

        /// <summary>
        /// Код ошибки: 0 – нет ошибки, -1 - ошибка
        /// </summary>
        [PickPointErrorCode] public int ErrorCode { get; set; }

        /// <summary>
        /// Описание ошибки
        /// </summary>
        [PickPointErrorMessage] public string ErrorMessage { get; set; }
    }

    public class CalcTariffService
    {
        /// <summary>
        /// Наименование режима доставки
        /// </summary>
        public string DeliveryMode { get; set; }

        /// <summary>
        /// Наименование тарифа
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Стоимость доставки по тарифу
        /// </summary>
        public float Tariff { get; set; }

        /// <summary>
        /// НДС
        /// </summary>
        [JsonProperty("NDS")]
        public float Nds { get; set; }
    }

    #endregion

    #region ClientPostamatList

    public class ClientPostamatListParams : ObjectOfSession
    {
        [JsonProperty("IKN")]
        public string Ikn { get; set; }
    }

    public class ClientPostamat
    {
        public int Id { get; set; }
        //public int OwnerId { get; set; }
        public string Name { get; set; }

        public string Number { get; set; }

        //public int CitiId { get; set; }
        //public int CitiOwnerId { get; set; }
        public string CitiName { get; set; }
        public string Region { get; set; }
        public string CountryName { get; set; }
        public string CountryIso { get; set; }

        //public string ClientPTnumber { get; set; }
        //public string ClosingComment { get; set; }

        //[JsonConverter(typeof(DateFormatConverter), "dd.MM.yy")]
        //public DateTime ClosingDateFrom { get; set; }

        //[JsonConverter(typeof(DateFormatConverter), "dd.MM.yy")]
        //public DateTime ClosingDateTo { get; set; }

        //public string MovingComment { get; set; }

        //[JsonConverter(typeof(DateFormatConverter), "dd.MM.yy")]
        //public DateTime MovingDateFrom { get; set; }

        //[JsonConverter(typeof(DateFormatConverter), "dd.MM.yy")]
        //public DateTime MovingDateTo { get; set; }

        //public string FileI0 { get; set; }
        //public string FileI1 { get; set; }
        //public string FileI2 { get; set; }
        public string Metro { get; set; }
        public List<string> MetroArray { get; set; }
        public string IndoorPlace { get; set; }
        public string Address { get; set; }

        public string House { get; set; }

        /// <summary>
        /// максимальная сумма выдачи
        /// </summary>
        [JsonConverter(typeof(StringToNullableFloatConverter))] // вместо null присылают "Без ограничений"
        public float? AmountTo { get; set; }

        //public string PostCode { get; set; }
        //public string WorkTime { get; set; }
        public string WorkTimeSMS { get; set; }
        public string InDescription { get; set; }
        public string OutDescription { get; set; }
        public string TypeTitle { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public StatusPostamat Status { get; set; }
        public CashType Cash { get; set; }
        public CardType Card { get; set; }
        public bool PayPassAvailable { get; set; }

        [JsonConverter(typeof(StringToArrayConverter<float>), "x")]
        public float[] MaxSize { get; set; }

        //public string MaxBoxSize { get; set; }
        public float? MaxWeight { get; set; }
        //public bool WorkHourly { get; set; }
        //public bool Opening { get; set; }
        //public bool Returning { get; set; }
        public bool SelfDelivery { get; set; }
        //public bool Fitting { get; set; }
        //public string LocationType { get; set; }
        public string OwnerName { get; set; }
        public string Comment { get; set; }
        public string MapAllowed { get; set; }
        public string WidgetAllowed { get; set; }
        //public string HubProcessing { get; set; }
        //public string HubCity { get; set; }
        //public string HubRegion { get; set; }
        //public string HubAddress { get; set; }
    }

    public enum StatusPostamat
    {
        New = 1,
        Work = 2,
        Close = 3,
        Overloaded = 5
    }

    public enum CashType
    {
        No = 0,
        Yes = 1
    }

    public enum CardType
    {
        No = 0,
        Yes = 1,
        OnlyOnlinePay = 2
    }

    #endregion

    #region GetStates

    public class StateFromList
    {
        [JsonProperty("State")]
        public int Code { get; set; }

        [JsonProperty("StateText")]
        public string Name { get; set; }

        public List<VisualState> VisualStates { get; set; }
    }

    public class VisualState
    {
        [JsonProperty("ID")]
        public int Code { get; set; }

        [JsonProperty("Text")]
        public string Name { get; set; }
    }

    #endregion

    #region GetInvoicesChangeState

    public class GetInvoicesChangeStateParams : ObjectOfSession
    {
        [JsonConverter(typeof(DateFormatConverter), "dd.MM.yy HH:mm")]
        public DateTime DateFrom { get; set; }

        [JsonConverter(typeof(DateFormatConverter), "dd.MM.yy HH:mm")]
        public DateTime DateTo { get; set; }

        /// <summary>
        /// статус, если не указан, то возвращается по всем статусам
        /// </summary>
        public int? State { get; set; }

        /// <summary>
        /// вид отправления
        /// </summary>
        public PostageType? PostageType { get; set; }
    }

    public class InvoiceChangeState
    {
        /// <summary>
        /// Штрих-код вложимого
        /// </summary>
        public string BarCode { get; set; }

        /// <summary>
        /// дата изменения статуса
        /// </summary>
        [JsonConverter(typeof(DateFormatConverter), "dd.MM.yyyy HH:mm")]
        public DateTime ChangeDT { get; set; }

        /// <summary>
        /// комментарий
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// код статуса вложимого
        /// </summary>
        [JsonProperty("State")]
        public int? StateCode { get; set; }

        /// <summary>
        /// описание статуса
        /// </summary>
        public string StateMessage { get; set; }

        /// <summary>
        /// текстовый внешний статус вложимого
        /// </summary>
        public string VisualState { get; set; }

        /// <summary>
        /// код внешнего статуса вложимого
        /// </summary>
        public int? VisualStateCode { get; set; }

        /// <summary>
        /// Номер отправления PickPoint
        /// </summary>
        public string InvoiceNumber { get; set; }

        /// <summary>
        /// вид отправления
        /// </summary>
        public PostageType? PostageType { get; set; }

        /// <summary>
        /// Номер отправления магазина
        /// </summary>
        public string SenderInvoiceNumber { get; set; }

        /// <summary>
        /// Тип кладовки (1- СЦ Приема, 2 - Транзитный СЦ, 3 - СЦ доставки
        /// </summary>
        public int? SubState { get; set; }
    }

    #endregion

    #region GetZone

    public class GetZoneParams : ObjectOfSession
    {
        [JsonProperty("IKN")]
        public string Ikn { get; set; }
        public string FromCity { get; set; }

        /// <summary>
        /// Номер пункта выдачи
        /// </summary>
        [JsonProperty("ToPT")]
        public string ToPostamatNumber { get; set; }
    }

    public class GetZoneResponse
    {
        public List<Zone> Zones { get; set; }

        [PickPointErrorCode] public int ErrorCode { get; set; }
        [PickPointErrorMessage] public string Error { get; set; }
    }

    public class Zone
    {
        public string FromCity { get; set; }
        public string ToCity { get; set; }

        [JsonProperty("ToPT")]
        public string PostamatNumber { get; set; }

        [JsonProperty("Zone")]
        public int ZoneId { get; set; }
        public int DeliveryMin { get; set; }
        public int DeliveryMax { get; set; }
        public string DeliveryMode { get; set; }
        public float Koeff { get; set; }
    }

    #endregion

    #region GetCities

    public class CityFromList
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string NameEng { get; set; }

        [JsonProperty("Owner_Id")]
        public int OwnerId { get; set; }

        public string RegionName { get; set; }
    }

    #endregion

    #region MakeLabel

    public class MakeLabelParams : ObjectOfSession
    {
        public List<string> Invoices { get; set; }
    }

    #endregion MakeLabel

    #region MakeZebraLabel

    public class MakeZebraLabelParams : ObjectOfSession
    {
        public List<string> Invoices { get; set; }
    }

    #endregion MakeZebraLabel
}
