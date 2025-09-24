using System;
using System.Collections.Generic;
using AdvantShop.Core.Common;
using AdvantShop.Core.Common.Attributes;
using Newtonsoft.Json;

namespace AdvantShop.Shipping.PecEasyway.Api
{
    #region Base

    [JsonConverter(typeof(PecBaseResponseConverter))]
    public class PecBaseResponse<T>
        where T : class, new()
    {
        public PecBaseResponse(T result, ResponseError error)
        {
            Result = result;
            Error = error;
            Success = error == null;
        }

        public T Result { get; set; }
        public bool Success { get; set; }
        public ResponseError Error { get; set; }
    }

    public class ResponseError
    {
        public bool IsError { get; set; }

        public List<Error> Errors { get; set; }
    }

    public class Error
    {
        public long Code { get; set; }

        [JsonProperty("descr")]
        public string Description { get; set; }
    }

    #endregion

    #region CalculateDelivery

    public class CalculateDelivery
    {
        public EnDeliveryType DeliveryType { get; set; }
        public float Total { get; set; }
        public EstDeliveryTime EstDeliveryTime { get; set; }
    }

    public partial class EstDeliveryTime
    {
        public string Min { get; set; }
        public string Max { get; set; }
    }


    public class EnDeliveryType : IntegerEnum<EnDeliveryType>
    {
        public EnDeliveryType(int value) : base(value)
        {
        }

        /// <summary>
        /// Дверь-Дверь
        /// </summary>
        [Localize("Курьером")]
        public static EnDeliveryType Courier { get { return new EnDeliveryType(1); } }

        /// <summary>
        /// Дверь-Терминал
        /// </summary>
        [Localize("До терминала")]
        public static EnDeliveryType Terminal { get { return new EnDeliveryType(2); } }

        /// <summary>
        /// Дверь-Дверь (авиа)
        /// </summary>
        [Localize("Курьером (авиа)")]
        public static EnDeliveryType CourierAvia { get { return new EnDeliveryType(3); } }

        /// <summary>
        /// Дверь-Терминал (авиа)
        /// </summary>
        [Localize("До терминала (авиа)")]
        public static EnDeliveryType TerminalAvia { get { return new EnDeliveryType(4); } }

        /// <summary>
        /// Дверь-ПВЗ
        /// </summary>
        [Localize("До пункта выдачи")]
        public static EnDeliveryType PVZ { get { return new EnDeliveryType(5); } }
    }

    #endregion

    #region PickupPoints

    public class PickupPoint
    {
        public string City { get; set; }

        public string Region { get; set; }

        public string Address { get; set; }

        [JsonProperty("lat")]
        public float? Latitude { get; set; }

        [JsonProperty("lng")]
        public float? Longitude { get; set; }

        [JsonProperty("office")]
        public bool IsOffice { get; set; }

        public bool IsTerminal { get; set; }

        [JsonProperty("guid", NullValueHandling = NullValueHandling.Ignore)]
        public string Guid { get; set; }

        public string Partner { get; set; }

        public string Schedule { get; set; }

        public string Phone { get; set; }

        public string TripDescription { get; set; }
    }

    #endregion

    #region Order

    public class PecOrder
    {
        /// <summary>
        /// Идентификатор заказа в системе клиента
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Адрес отправления
        /// </summary>
        public string LocationFrom { get; set; }

        /// <summary>
        /// Адрес получения
        /// </summary>
        public string LocationTo { get; set; }

        /// <summary>
        /// Код ПВЗ, полученный методом getPickupPoints из поля guid
        /// </summary>
        public string PickupPointCode { get; set; }

        /// <summary>
        /// Количество грузомест
        /// </summary>
        public string CargoCount { get; set; }

        /// <summary>
        /// Вес в килограммах
        /// </summary>
        public float Weight { get; set; }

        /// <summary>
        /// Длина в сантиметрах
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Ширина в сантиметрах
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Высота в сантиметрах
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Оценочная стоимость
        /// </summary>
        public float AssessedCost { get; set; }

        /// <summary>
        /// Способ оплаты (0 - Безналичная, 1 - Наличными, 2 - предоплата (без наложенного платежа)
        /// </summary>
        public EnPaymentMethod PaymentMethod { get; set; }

        /// <summary>
        /// Тип доставки 
        /// </summary>
        public int DeliveryType { get; set; }

        /// <summary>
        /// Желаемый интервал доставки
        /// </summary>
        public Interval DeliveryInterval { get; set; }

        public Interval PickupInterval { get; set; }

        public Sender Sender { get; set; }

        /// <summary>
        /// Итого с клиента
        /// </summary>
        public float Total { get; set; }

        /// <summary>
        /// Идентификатор отправителя (в случае нескольких юр. лиц), необязательный
        /// </summary>
        public string SenderId { get; set; }

        /// <summary>
        /// true - в случае самостоятельной доставки груза на склад Easy Way, необязательный
        /// </summary>
        public bool? NoPickup { get; set; }

        /// <summary>
        /// Товарный состав, необязательный
        /// </summary>
        public List<Item> Items { get; set; }

        /// <summary>
        /// Характеристики грузовых мест
        /// </summary>
        public List<Place> Places { get; set; }

        /// <summary>
        /// Штрихкод сопроводительного документа
        /// </summary>
        public string ClientRelatedDocumentsBarcode { get; set; }

        /// <summary>
        /// Получатель
        /// </summary>
        public Recipient Recipient { get; set; }

        /// <summary>
        /// <para>"hardPack",                  // Жесткая упаковка</para>
        /// <para>"addPack1",                  // Дополнительная упаковка</para>
        /// <para>"addPack2",                  // Пузырьковая пленка</para>
        /// <para>"docReturn",                 // Возврат документов</para>
        /// <para>"loadUnload",                // Погрузочно-разгрузочные работы</para>
        /// <para>"recipientPayer",            // Оплата получателем</para>
        /// <para>"nightDelivery",             // Ночная доставка</para>
        /// <para>"partialDistribution"        // Частичная выдача</para>
        /// </summary>
        public List<string> Services { get; set; }
    }

    public class Interval
    {
        public DateTimeOffset Start { get; set; }

        public DateTimeOffset End { get; set; }
    }

    public class Item
    {
        /// <summary>
        /// Артикул
        /// </summary>
        public string Article { get; set; }

        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Количество
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Цена с НДС
        /// </summary>
        public float Price { get; set; }

        /// <summary>
        /// Сумма с НДС
        /// </summary>
        public float Sum { get; set; }

        /// <summary>
        /// // Ставка НДС (%) (none - без НДС)
        /// </summary>
        public string Vat { get; set; }

        /// <summary>
        /// Сумма НДС
        /// </summary>
        public float VatSum { get; set; }
    }

    public class Place
    {
        /// <summary>
        /// Длина (см)
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Ширина (см)
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Высота (см)
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Объём (м3)
        /// </summary>
        public float? Volume { get; set; }

        /// <summary>
        /// Вес (кг)
        /// </summary>
        public float Weight { get; set; }

        /// <summary>
        /// Штрихкод грузового места
        /// </summary>
        public string Barcode { get; set; }
    }

    public class Recipient
    {
        /// <summary>
        /// Заполняется, если получатель - юридическое лицо
        /// </summary>
        public Organization Organization { get; set; }

        /// <summary>
        /// ФИО получателя
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Телефон
        /// </summary>
        public string Phone { get; set; }
    }

    public class Organization
    {
        /// <summary>
        /// ИНН получателя
        /// </summary>
        public string Inn { get; set; }

        /// <summary>
        /// Наименование юридического лица
        /// </summary>
        public string Name { get; set; }
    }

    public class Sender
    {
        public string Inn { get; set; }

        public string Name { get; set; }

        public string ContactName { get; set; }

        public string ContactPhone { get; set; }
    }

    public enum EnPaymentMethod : int 
    {
        /// <summary>
        /// Безналичная
        /// </summary>
        NonCash = 0,

        /// <summary>
        /// Наличными
        /// </summary>
        Cash = 1,

        /// <summary>
        /// предоплата (без наложенного платежа)
        /// </summary>
        WithoutPayment = 2
    }

    public class CreatedOrderInfo
    {
        public CreatedOrderData Data { get; set; }
    }

    public class CreatedOrderData
    {
        public string Id { get; set; }
        public string Track { get; set; }
    }


    public class CancelOrdersInfo
    {
        public List<CancelOrderData> Data { get; set; }
    }

    public class CancelOrderData
    {
        public string Id { get; set; }

        public bool Cancel { get; set; }

        [JsonProperty("descr")]
        public string Description { get; set; }
    }

    public partial class OrdersStatuses
    {
        public string Id { get; set; }

        public string ClientId { get; set; }

        public DateTimeOffset? Date { get; set; }

        public string Status { get; set; }

        public DateTimeOffset? ArrivalPlanDateTime { get; set; }

        public DateTimeOffset? DateOrder { get; set; }

        public string Sender { get; set; }

        public string Receiver { get; set; }

        public string CarrierTrackNumber { get; set; }

        public string Address { get; set; }

        public string DeliveryType { get; set; }

        public string Phone { get; set; }

        public string StatusCode { get; set; }
    }
    #endregion
}
