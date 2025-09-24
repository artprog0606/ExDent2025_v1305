using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common;
using AdvantShop.Core.Common.Attributes;
using Newtonsoft.Json;

namespace AdvantShop.Shipping.RussianPost.Api
{
    #region Common

    public class Address
    {
        /// <summary>
        /// Тип адреса
        /// </summary>
        [JsonProperty("address-type")]
        public EnAddressType AddressType { get; set; }

        /// <summary>
        /// Область, регион
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// Населенный пункт
        /// </summary>
        public string Place { get; set; }

        /// <summary>
        /// Район
        /// </summary>
        public string Area { get; set; }

        /// <summary>
        /// Название гостиницы
        /// </summary>
        public string Hotel { get; set; }

        /// <summary>
        /// Почтовый индекс
        /// </summary>
        public string Index { get; set; }

        /// <summary>
        /// Микрорайон
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Номер для а/я, войсковая часть, войсковая часть ЮЯ, полевая почта
        /// </summary>
        [JsonProperty("num-address-type")]
        public string NumAddressType { get; set; }

        /// <summary>
        /// Часть адреса: Улица
        /// </summary>
        public string Street { get; set; }

        /// <summary>
        /// Часть адреса: Номер здания
        /// </summary>
        public string House { get; set; }

        /// <summary>
        /// Часть здания: Строение
        /// </summary>
        public string Building { get; set; }

        /// <summary>
        /// Часть здания: Корпус
        /// </summary>
        public string Corpus { get; set; }

        /// <summary>
        /// Часть здания: Дробь
        /// </summary>
        public string Slash { get; set; }

        /// <summary>
        /// Часть здания: Литера
        /// </summary>
        public string Letter { get; set; }

        /// <summary>
        /// Часть здания: Номер помещения
        /// </summary>
        public string Room { get; set; }

        /// <summary>
        /// Часть здания: Офис
        /// </summary>
        public string Office { get; set; }

        /// <summary>
        /// Часть здания: Владение
        /// </summary>
        public string Vladenie { get; set; }
    }

    public class Dimension
    {
        /// <summary>
        /// Линейная высота (сантиметры)
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? Height { get; set; }

        /// <summary>
        /// Линейная длина (сантиметры)
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? Length { get; set; }

        /// <summary>
        /// Линейная ширина (сантиметры)
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? Width { get; set; }
    }

    public abstract class BaseOrderRussianPostResponse
    {
        /// <summary>
        /// Список ошибок
        /// </summary>
        [JsonProperty("errors")]
        public List<Error> Errors { get; set; }

        /// <summary>
        /// Список идентификаторов успешно обработанных сущностей
        /// </summary>
        [JsonProperty("result-ids")]
        public List<long> OrderIds { get; set; }
    }

    public class Error
    {
        /// <summary>
        /// Список кодов ошибок
        /// </summary>
        [JsonProperty("error-codes")]
        public List<ErrorCode> ErrorCodes { get; set; }

        /// <summary>
        /// Индекс в массиве
        /// </summary>
        [JsonProperty("position")]
        public long Position { get; set; }
    }

    public class ErrorCode
    {
        /// <summary>
        /// Код ошибки
        /// </summary>
        [JsonProperty("code")]
        public string Code { get; set; }

        /// <summary>
        /// Описание ошибки
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Описание ошибки
        /// </summary>
        [JsonProperty("details")]
        public string Details { get; set; }

        /// <summary>
        /// Индекс в массиве
        /// </summary>
        [JsonProperty("position")]
        public long Position { get; set; }
    }

    //[JsonConverter(typeof(StringEnumConverter))]
    //public enum EnMailCategory
    public class EnMailCategory : StringEnum<EnMailCategory>
    {
        public EnMailCategory(string value) : base(value)
        {
        }

        /// <summary>
        /// Простое
        /// <para>для писем</para>
        /// </summary>
        [Localize("Простое")]
        public static EnMailCategory Simple { get { return new EnMailCategory("SIMPLE"); } }

        /// <summary>
        /// Обыкновенное
        /// <para>для посылок</para>
        /// </summary>
        [Localize("Обыкновенное")]
        public static EnMailCategory Ordinary { get { return new EnMailCategory("ORDINARY"); } }

        /// <summary>
        /// Заказное
        /// <para>для писем</para>
        /// </summary>
        [Localize("Заказное")]
        public static EnMailCategory Ordered { get { return new EnMailCategory("ORDERED"); } }

        /// <summary>
        /// С объявленной ценностью
        /// <para>для посылок</para>
        /// </summary>
        [Localize("С объявленной ценностью")]
        public static EnMailCategory WithDeclaredValue { get { return new EnMailCategory("WITH_DECLARED_VALUE"); } }

        /// <summary>
        /// С объявленной ценностью и наложенным платежом
        /// <para>для посылок</para>
        /// </summary>
        [Localize("С объявленной ценностью и наложенным платежом")]
        public static EnMailCategory WithDeclaredValueAndCashOnDelivery { get { return new EnMailCategory("WITH_DECLARED_VALUE_AND_CASH_ON_DELIVERY"); } }

        /// <summary>
        /// С обязательным платежом
        /// <para>для еком</para>
        /// </summary>
        [Localize("С обязательным платежом")]
        public static EnMailCategory WithCompulsoryPayment { get { return new EnMailCategory("WITH_COMPULSORY_PAYMENT"); } }

        /// <summary>
        /// С объявленной ценностью и обязательным платежом
        /// <para>для еком</para>
        /// </summary>
        [Localize("С объявленной ценностью и обязательным платежом")]
        public static EnMailCategory WithDeclaredValueAndCompulsoryPayment { get { return new EnMailCategory("WITH_DECLARED_VALUE_AND_COMPULSORY_PAYMENT"); } }

        /// <summary>
        /// Комбинированное обыкновенное
        /// <para>для посылок</para>
        /// </summary>
        [Localize("Комбинированное обыкновенное")]
        public static EnMailCategory CombinedOrdinary { get { return new EnMailCategory("COMBINED_ORDINARY"); } }

        /// <summary>
        /// Комбинированное с объявленной ценностью
        /// <para>для посылок</para>
        /// </summary>
        [Localize("Комбинированное с объявленной ценностью")]
        public static EnMailCategory CombinedWithDeclaredValue { get { return new EnMailCategory("COMBINED_WITH_DECLARED_VALUE"); } }

        /// <summary>
        /// Комбинированное с объявленной ценностью и наложенным платежом
        /// <para>для посылок</para>
        /// </summary>
        [Localize("Комбинированное с объявленной ценностью и наложенным платежом")]
        public static EnMailCategory CombinedWithDeclaredValueAndCashOnDelivery { get { return new EnMailCategory("COMBINED_WITH_DECLARED_VALUE_AND_CASH_ON_DELIVERY"); } }
    }

    //[JsonConverter(typeof(StringEnumConverter))]
    //public enum EnMailType
    public class EnMailType : StringEnum<EnMailType>
    {
        public EnMailType(string value) : base(value)
        {
        }

        //[EnumMember(Value = "UNDEFINED")] None,

        /// <summary>
        /// Посылка "нестандартная"
        /// </summary>
        [Localize("Посылка \"нестандартная\"")]
        public static EnMailType PostalParcel { get { return new EnMailType("POSTAL_PARCEL"); } }

        /// <summary>
        /// Посылка "онлайн"
        /// </summary>
        [Localize("Посылка \"онлайн\"")]
        public static EnMailType OnlineParcel { get { return new EnMailType("ONLINE_PARCEL"); } }

        /// <summary>
        /// Курьер "онлайн"
        /// </summary>
        [Localize("Курьер \"онлайн\"")]
        public static EnMailType OnlineCourier { get { return new EnMailType("ONLINE_COURIER"); } }

        /// <summary>
        /// Отправление EMS
        /// </summary>
        [Localize("Отправление EMS")]
        public static EnMailType Ems { get { return new EnMailType("EMS"); } }

        /// <summary>
        /// EMS оптимальное
        /// </summary>
        [Localize("EMS оптимальное")]
        public static EnMailType EmsOptimal { get { return new EnMailType("EMS_OPTIMAL"); } }

        /// <summary>
        /// EMS РТ
        /// </summary>
        [Localize("EMS РТ")]
        public static EnMailType EmsRT { get { return new EnMailType("EMS_RT"); } }

        /// <summary>
        /// EMS тендер
        /// </summary>
        [Localize("EMS тендер")]
        public static EnMailType EmsTender { get { return new EnMailType("EMS_TENDER"); } }

        /// <summary>
        /// Письмо
        /// </summary>
        [Localize("Письмо")]
        public static EnMailType Letter { get { return new EnMailType("LETTER"); } }

        /// <summary>
        /// Письмо 1-го класса
        /// </summary>
        [Localize("Письмо 1-го класса")]
        public static EnMailType LetterClass1 { get { return new EnMailType("LETTER_CLASS_1"); } }

        /// <summary>
        /// Бандероль
        /// </summary>
        [Localize("Бандероль")]
        public static EnMailType Banderol { get { return new EnMailType("BANDEROL"); } }

        /// <summary>
        /// Бизнес курьер
        /// </summary>
        [Localize("Бизнес курьер")]
        public static EnMailType BusinessCourier { get { return new EnMailType("BUSINESS_COURIER"); } }

        /// <summary>
        /// Бизнес курьер экпресс
        /// </summary>
        [Localize("Бизнес курьер экпресс")]
        public static EnMailType BusinessCourierExpress { get { return new EnMailType("BUSINESS_COURIER_ES"); } }

        /// <summary>
        /// Посылка 1-го класса
        /// </summary>
        [Localize("Посылка 1-го класса")]
        public static EnMailType ParcelClass1 { get { return new EnMailType("PARCEL_CLASS_1"); } }

        /// <summary>
        /// Бандероль 1-го класса
        /// </summary>
        [Localize("Бандероль 1-го класса")]
        public static EnMailType BanderolClass1 { get { return new EnMailType("BANDEROL_CLASS_1"); } }

        /// <summary>
        /// ВГПО 1-го класса
        /// </summary>
        [Localize("ВГПО 1-го класса")]
        public static EnMailType VGPOClass1 { get { return new EnMailType("VGPO_CLASS_1"); } }

        /// <summary>
        /// Мелкий пакет
        /// </summary>
        [Localize("Мелкий пакет")]
        public static EnMailType SmallPacket { get { return new EnMailType("SMALL_PACKET"); } }

        /// <summary>
        /// Отправление ВСД (Возврат сопроводительныйх документов)
        /// </summary>
        [Localize("Отправление ВСД")]
        public static EnMailType VSD { get { return new EnMailType("VSD"); } }

        /// <summary>
        /// ECOM
        /// </summary>
        [Localize("ЕКОМ")]
        public static EnMailType ECOM { get { return new EnMailType("ECOM"); } }
    }

    //[JsonConverter(typeof(StringEnumConverter))]
    //public enum EnPaymentMethod
    public class EnPaymentMethod : StringEnum<EnPaymentMethod>
    {
        public EnPaymentMethod(string value) : base(value)
        {
        }

        /// <summary>
        /// Безналичный расчет
        /// </summary>
        public static EnPaymentMethod Cashless { get { return new EnPaymentMethod("CASHLESS"); } }

        /// <summary>
        /// Оплата марками
        /// </summary>
        public static EnPaymentMethod Stamp { get { return new EnPaymentMethod("STAMP"); } }

        /// <summary>
        /// Франкирование
        /// </summary>
        public static EnPaymentMethod Franking { get { return new EnPaymentMethod("FRANKING"); } }

        /// <summary>
        /// На франкировку
        /// </summary>
        public static EnPaymentMethod ToFranking { get { return new EnPaymentMethod("TO_FRANKING"); } }

        /// <summary>
        /// Знак онлайн оплаты
        /// </summary>
        public static EnPaymentMethod OnlinePaymentMark { get { return new EnPaymentMethod("ONLINE_PAYMENT_MARK"); } }
    }

    //[JsonConverter(typeof(StringEnumConverter))]
    //public enum EnAddressType
    public class EnAddressType : StringEnum<EnAddressType>
    {
        public EnAddressType(string value) : base(value)
        {
        }

        /// <summary>
        /// Стандартный (улица, дом, квартира)
        /// </summary>
        public static EnAddressType Default { get { return new EnAddressType("DEFAULT"); } }

        /// <summary>
        /// Абонентский ящик
        /// </summary>
        public static EnAddressType PoBox { get { return new EnAddressType("PO_BOX"); } }

        /// <summary>
        /// До востребования
        /// </summary>
        public static EnAddressType Demand { get { return new EnAddressType("DEMAND"); } }

        /// <summary>
        /// Для военных частей
        /// </summary>
        public static EnAddressType Unit { get { return new EnAddressType("UNIT"); } }
    }

    //[JsonConverter(typeof(StringEnumConverter))]
    //public enum EnPrintType
    public class EnPrintType : StringEnum<EnPrintType>
    {
        public EnPrintType(string value) : base(value)
        {
        }

        /// <summary>
        /// А5, 14.8 x 21 см, лазерная и струйная печать
        /// </summary>
        public static EnPrintType Paper { get { return new EnPrintType("PAPER"); } }

        /// <summary>
        /// А6, 10 x 15 см, термопечать
        /// </summary>
        public static EnPrintType Thermo { get { return new EnPrintType("THERMO"); } }
    }

    #endregion

    #region Calculate

    public class CalculateOptions
    {
        /// <summary>
        /// Признак услуги проверки комплектности (Опционально)
        /// </summary>
        [JsonProperty("completeness-checking", NullValueHandling = NullValueHandling.Ignore)]
        public bool? CompletenessChecking { get; set; }

        /// <summary>
        /// Признак услуги проверки вложения (Опционально)
        /// </summary>
        [JsonProperty("contents-checking", NullValueHandling = NullValueHandling.Ignore)]
        public bool? ContentsChecking { get; set; }

        /// <summary>
        /// Отметка 'Курьер' (Опционально) для типа доставки EmsOptimal
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? Courier { get; set; }

        /// <summary>
        /// Объявленная ценность. Целое число копейки (Опционально) в сочитании с MailCategory WithDeclaredValue и WithDeclaredValueAndCashOnDelivery
        /// </summary>
        [JsonProperty("declared-value", NullValueHandling = NullValueHandling.Ignore)]
        public int? DeclaredValue { get; set; }

        /// <summary>
        /// Стоимость
        /// </summary>
        [JsonProperty("goods-value", NullValueHandling = NullValueHandling.Ignore)]
        public int? GoodValue { get; set; }

        /// <summary>
        /// Идентификатор пункта выдачи заказов (ПВЗ) (Опционально)
        /// </summary>
        [JsonProperty("delivery-point-index", NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string DeliveryPointIndex { get; set; }

        /// <summary>
        /// Линейные размеры (Опционально) для негаборитной посылки (Посылка считается негабаритной, если сумма измерений трех сторон отправления превышает 120 см или одна из сторон отправления превышает 60 см.)
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Dimension Dimension { get; set; }

        /// <summary>
        /// Типоразмер (Опционально)
        /// </summary>
        [JsonProperty("dimension-type", NullValueHandling = NullValueHandling.Ignore)]
        public EnDimensionType DimensionType { get; set; }

        /// <summary>
        /// Категория вложения (Опционально)
        /// </summary>
        [JsonProperty("entries-type", NullValueHandling = NullValueHandling.Ignore)]
        public EnEntriesType EntriesType { get; set; }

        /// <summary>
        /// Отметка 'Осторожно/Хрупко' (Опционально) для посылки
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? Fragile { get; set; }

        /// <summary>
        /// Почтовый индекс объекта почтовой связи места приема (Опционально) указанный в договоре
        /// </summary>
        [JsonProperty("index-from", NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string IndexFrom { get; set; }

        /// <summary>
        /// Почтовый индекс объекта почтовой связи места назначения
        /// </summary>
        [JsonProperty("index-to")]
        public string IndexTo { get; set; }

        /// <summary>
        /// Опись вложения (Опционально)
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? Inventory { get; set; }

        /// <summary>
        /// Категория РПО
        /// </summary>
        [JsonProperty("mail-category")]
        public EnMailCategory MailCategory { get; set; }

        /// <summary>
        /// Вид РПО
        /// </summary>
        [JsonProperty("mail-type")]
        public EnMailType MailType { get; set; }

        /// <summary>
        /// Код страны назначения (Опционально)
        /// </summary>
        [JsonProperty("mail-direct")]
        public int? CountryCode { get; set; }

        /// <summary>
        /// Масса отправления в граммах
        /// </summary>
        [JsonProperty("mass")]
        public int Weight { get; set; }

        /// <summary>
        /// Способ оплаты уведомеления (Опционально)
        /// </summary>
        [JsonProperty("notice-payment-method", NullValueHandling = NullValueHandling.Ignore)]
        public EnPaymentMethod NoticePaymentMethod { get; set; }

        /// <summary>
        /// Способ оплаты (Опционально)
        /// </summary>
        [JsonProperty("payment-method", NullValueHandling = NullValueHandling.Ignore)]
        public EnPaymentMethod PaymentMethod { get; set; }

        /// <summary>
        /// Признак услуги SMS уведомления (Опционально)
        /// </summary>
        [JsonProperty("sms-notice-recipient", NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int? SmsNoticeRecipient { get; set; }

        /// <summary>
        /// Вид транспортировки (Опционально)
        /// </summary>
        [JsonProperty("transport-type", NullValueHandling = NullValueHandling.Ignore)]
        public EnTransportType TransportType { get; set; }

        /// <summary>
        /// Возврат сопроводительныйх документов (Опционально)
        /// </summary>
        [JsonProperty("vsd", NullValueHandling = NullValueHandling.Ignore)]
        public bool? ReturnAccompanyingDocuments { get; set; }

        /// <summary>
        /// Отметка 'С электронным уведомлением' (Опционально)
        /// </summary>
        [JsonProperty("with-electronic-notice", NullValueHandling = NullValueHandling.Ignore)]
        public bool? WithElectronicNotice { get; set; }

        /// <summary>
        /// Отметка 'С заказным уведомлением' (Опционально)
        /// </summary>
        [JsonProperty("with-order-of-notice", NullValueHandling = NullValueHandling.Ignore)]
        public bool? WithOrderOfNotice { get; set; }

        /// <summary>
        /// Отметка 'С простым уведомлением' (Опционально)
        /// </summary>
        [JsonProperty("with-simple-notice", NullValueHandling = NullValueHandling.Ignore)]
        public bool? WithSimpleNotice { get; set; }
    }

    public class EnDimensionType : StringEnum<EnDimensionType>, IComparable
    {
        // чтобы JsonConverter мог создавать экземпляр
        public EnDimensionType(string value) : base(value)
        {
        }

        /// <summary>
        /// до 260х170х80 мм
        /// </summary>
        public static EnDimensionType S { get { return new EnDimensionType("S"); } }

        /// <summary>
        /// до 300х200х150 мм
        /// </summary>
        public static EnDimensionType M { get { return new EnDimensionType("M"); } }

        /// <summary>
        /// до 400х270х180 мм
        /// </summary>
        public static EnDimensionType L { get { return new EnDimensionType("L"); } }

        /// <summary>
        /// 530х360х220 мм
        /// </summary>
        public static EnDimensionType XL { get { return new EnDimensionType("XL"); } }

        /// <summary>
        /// Негабарит (сумма сторон не более 1400 мм, сторона не более 600 мм)
        /// </summary>
        public static EnDimensionType Oversized { get { return new EnDimensionType("OVERSIZED"); } }

        public int GetSumDimension()
        {
            return GetSumDimension(this);
        }

        // возвращает максимально допустимую сумму сторон в мм
        // (необходимо для сравнения, какой тим меньше какой больше)
        public static int GetSumDimension(EnDimensionType dimensionType)
        {
            if (dimensionType == null)
                return 0;

            if (dimensionType == S)
                return 260 + 170 + 80;

            if (dimensionType == M)
                return 300 + 200 + 150;

            if (dimensionType == L)
                return 400 + 270 + 180;

            if (dimensionType == XL)
                return 530 + 360 + 220;

            if (dimensionType == Oversized)
                return 1400;

            return 0;
        }

        public static bool operator >(EnDimensionType a, EnDimensionType b)
        {
            return GetSumDimension(a) > GetSumDimension(b);
        }

        public static bool operator <(EnDimensionType a, EnDimensionType b)
        {
            return GetSumDimension(a) < GetSumDimension(b);
        }

        public static bool operator >=(EnDimensionType a, EnDimensionType b)
        {
            return a > b || a == b;
        }

        public static bool operator <=(EnDimensionType a, EnDimensionType b)
        {
            return a < b || a == b;
        }

        public static EnDimensionType GetDimensionType(Dimension dimension)
        {
            if (dimension == null)
                return null;

            if (dimension.Height == null ||
                dimension.Length == null ||
                dimension.Width == null)
                return null;

            var XYZ = new[] { dimension.Height * 10, dimension.Length * 10, dimension.Width * 10 }.OrderByDescending(x => x).ToArray();

            if (XYZ[0] <= 260 && XYZ[1] <= 170 && XYZ[2] <= 80)
                return S;

            if (XYZ[0] <= 300 && XYZ[1] <= 200 && XYZ[2] <= 150)
                return M;

            if (XYZ[0] <= 400 && XYZ[1] <= 270 && XYZ[2] <= 180)
                return L;

            if (XYZ[0] <= 530 && XYZ[1] <= 360 && XYZ[2] <= 220)
                return XL;

            if (XYZ.Sum() <= 1400 && XYZ.Max() <= 600)
                return Oversized;

            return null;
        }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            EnDimensionType otherEnDimensionType = obj as EnDimensionType;
            if (otherEnDimensionType != null)
            {
                if (this > otherEnDimensionType)
                    return 1;
                else if (this < otherEnDimensionType)
                    return -1;
                else
                    return 0;
            }
            else
                throw new ArgumentException("Object is not a EnDimensionType");
        }
    }

    public class CalculateResponse
    {
        /// <summary>
        /// Плата за Авиа-пересылку (коп) (Опционально)
        /// </summary>
        [JsonProperty("avia-rate")]
        public RateValues AviaRate { get; set; }

        /// <summary>
        /// Плата за "Проверку комплектности" (коп) (Опционально)
        /// </summary>
        [JsonProperty("completeness-checking-rate")]
        public RateValues CompletenessCheckingRate { get; set; }

        /// <summary>
        /// Плата за "Проверку вложений" (коп)
        /// </summary>
        [JsonProperty("contents-checking-rate")]
        public RateValues ContentsCheckingRate { get; set; }

        /// <summary>
        /// Примерные сроки доставки (Опционально)
        /// </summary>
        [JsonProperty("delivery-time")]
        public DeliveryTime DeliveryTime { get; set; }

        /// <summary>
        /// Надбавка за отметку 'Осторожно/Хрупкое' (Опционально)
        /// </summary>
        [JsonProperty("fragile-rate")]
        public RateValues FragileRate { get; set; }

        /// <summary>
        /// Плата за пересылку (коп) (Опционально)
        /// </summary>
        [JsonProperty("ground-rate")]
        public RateValues GroundRate { get; set; }

        /// <summary>
        /// Плата за объявленную ценность (коп) (Опционально)
        /// </summary>
        [JsonProperty("insurance-rate")]
        public RateValues InsuranceRate { get; set; }

        /// <summary>
        /// Плата за "Опись вложения" (коп) (Опционально)
        /// </summary>
        [JsonProperty("inventory-rate")]
        public RateValues InventoryRate { get; set; }

        /// <summary>
        /// Способ оплаты уведомеления (Опционально)
        /// </summary>
        [JsonProperty("notice-payment-method")]
        public EnPaymentMethod NoticePaymentMethod { get; set; }

        /// <summary>
        /// Надбавка за уведомление о вручении (Опционально)
        /// </summary>
        [JsonProperty("notice-rate")]
        public RateValues NoticeRate { get; set; }

        /// <summary>
        /// Надбавка за негабарит при весе более 10кг (Опционально)
        /// </summary>
        [JsonProperty("oversize-rate")]
        public RateValues OversizeRate { get; set; }

        /// <summary>
        /// Способ оплаты (Опционально)
        /// </summary>
        [JsonProperty("payment-method")]
        public EnPaymentMethod PaymentMethod { get; set; }

        /// <summary>
        /// Плата за "Пакет смс получателю" (коп) (Опционально)
        /// </summary>
        [JsonProperty("sms-notice-recipient-rate")]
        public RateValues SmsNoticeRecipientRate { get; set; }

        /// <summary>
        /// Плата всего без НДС (коп). Если ноль, значит осуществить пересылку с указанными параметрами нельзя
        /// </summary>
        [JsonProperty("total-rate")]
        public int TotalRateNoVat { get; set; }

        /// <summary>
        /// Всего НДС (коп)
        /// </summary>
        [JsonProperty("total-vat")]
        public int TotalVat { get; set; }

        /// <summary>
        /// Плата за "Возврат сопроводительных документов" (коп) (Опционально)
        /// </summary>
        [JsonProperty("vsd-rate")]
        public RateValues ReturnAccompanyingDocumentsRate { get; set; }
    }

    public class RateValues
    {
        /// <summary>
        /// Тариф без НДС (коп)
        /// </summary>
        public int Rate { get; set; }

        /// <summary>
        /// НДС (коп)
        /// </summary>
        public int Vat { get; set; }
    }

    public class DeliveryTime
    {
        /// <summary>
        /// Максимальное время доставки (дни)
        /// </summary>
        [JsonProperty("max-days")]
        public int MaxDays { get; set; }

        /// <summary>
        /// Минимальное время доставки (дни) (Опционально)
        /// </summary>
        [JsonProperty("min-days", NullValueHandling = NullValueHandling.Ignore)]
        public int? MinDays { get; set; }
    }
    #endregion

    #region CleanAddress

    public class CleanAddress
    {
        //Идентификатор записи
        public string Id { get; set; }

        /// <summary>
        /// Оригинальный адрес одной строкой
        /// </summary>
        [JsonProperty("original-address")]
        public string Address { get; set; }
    }

    public class CleanAddressResponse : Address
    {
        /// <summary>
        /// Идентификатор записи
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Оригинальные адрес одной строкой (который отправлялся для нормализации)
        /// </summary>
        [JsonProperty("original-address")]
        public string OriginalAddresss { get; set; }

        /// <summary>
        /// Код качества нормализации адреса
        /// </summary>
        [JsonProperty("quality-code")]
        public EnQualityCode QualityCode { get; set; }

        /// <summary>
        /// Код проверки нормализации адреса
        /// </summary>
        [JsonProperty("validation-code")]
        public EnValidationCode ValidationCode { get; set; }
    }

    //[JsonConverter(typeof(StringEnumConverter))]
    //public enum EnQualityCode
    public class EnQualityCode : StringEnum<EnQualityCode>
    {
        public EnQualityCode(string value) : base(value)
        {
        }

        /// <summary>
        /// Пригоден для почтовой рассылки
        /// </summary>
        public static EnQualityCode Good { get { return new EnQualityCode("GOOD"); } }

        /// <summary>
        /// До востребования
        /// </summary>
        public static EnQualityCode OnDemand { get { return new EnQualityCode("ON_DEMAND"); } }

        /// <summary>
        /// Абонентский ящик
        /// </summary>
        public static EnQualityCode PostalBox { get { return new EnQualityCode("POSTAL_BOX"); } }

        /// <summary>
        /// Не определен регион
        /// </summary>
        public static EnQualityCode UndefinedRegion { get { return new EnQualityCode("UNDEF_01"); } }

        /// <summary>
        /// Не определен город или населенный пункт
        /// </summary>
        public static EnQualityCode UndefinedCityOrPlace { get { return new EnQualityCode("UNDEF_02"); } }

        /// <summary>
        /// Не определена улица
        /// </summary>
        public static EnQualityCode UndefinedStreet { get { return new EnQualityCode("UNDEF_03"); } }

        /// <summary>
        /// Не определен номер дома
        /// </summary>
        public static EnQualityCode UndefinedHouse { get { return new EnQualityCode("UNDEF_04"); } }

        /// <summary>
        /// Не определена квартира/офис
        /// </summary>
        public static EnQualityCode UndefinedRoom { get { return new EnQualityCode("UNDEF_05"); } }

        /// <summary>
        /// Не определен
        /// </summary>
        public static EnQualityCode Undefined { get { return new EnQualityCode("UNDEF_06"); } }

        /// <summary>
        /// Иностранный адрес
        /// </summary>
        public static EnQualityCode NotRussia { get { return new EnQualityCode("UNDEF_07"); } }
    }

    //[JsonConverter(typeof(StringEnumConverter))]
    //public enum EnValidationCode
    public class EnValidationCode : StringEnum<EnValidationCode>
    {
        public EnValidationCode(string value) : base(value)
        {
        }

        /// <summary>
        /// Подтверждено контролером
        /// </summary>
        public static EnValidationCode ConfirmedManually { get { return new EnValidationCode("CONFIRMED_MANUALLY"); } }

        /// <summary>
        /// Уверенное распознавание
        /// </summary>
        public static EnValidationCode Validated { get { return new EnValidationCode("VALIDATED"); } }

        /// <summary>
        /// Распознан: адрес был перезаписан в справочнике
        /// </summary>
        public static EnValidationCode Overridden { get { return new EnValidationCode("OVERRIDDEN"); } }

        /// <summary>
        /// На проверку, неразобранные части
        /// </summary>
        public static EnValidationCode NotValidatedHasUnparsedParts { get { return new EnValidationCode("NOT_VALIDATED_HAS_UNPARSED_PARTS"); } }

        /// <summary>
        /// На проверку, предположение
        /// </summary>
        public static EnValidationCode NotValidatedHasAssumption { get { return new EnValidationCode("NOT_VALIDATED_HAS_ASSUMPTION"); } }

        /// <summary>
        /// На проверку, нет основных частей
        /// </summary>
        public static EnValidationCode NotValidatedHasNoMainPoints { get { return new EnValidationCode("NOT_VALIDATED_HAS_NO_MAIN_POINTS"); } }

        /// <summary>
        /// На проверку, предположение по улице
        /// </summary>
        public static EnValidationCode NotValidatedHasNumberStreetAssumption { get { return new EnValidationCode("NOT_VALIDATED_HAS_NUMBER_STREET_ASSUMPTION"); } }

        /// <summary>
        /// На проверку, нет в КЛАДР
        /// </summary>
        public static EnValidationCode NotValidatedHasNoKladrRecord { get { return new EnValidationCode("NOT_VALIDATED_HAS_NO_KLADR_RECORD"); } }

        /// <summary>
        /// На проверку, нет улицы или населенного пункта
        /// </summary>
        public static EnValidationCode NotValidatedHouseWithoutStreetOrNp { get { return new EnValidationCode("NOT_VALIDATED_HOUSE_WITHOUT_STREET_OR_NP"); } }

        /// <summary>
        /// На проверку, нет дома
        /// </summary>
        public static EnValidationCode NotValidatedHouseExtensionWithoutHouse { get { return new EnValidationCode("NOT_VALIDATED_HOUSE_EXTENSION_WITHOUT_HOUSE"); } }

        /// <summary>
        /// На проверку, неоднозначность
        /// </summary>
        public static EnValidationCode NotValidatedHasAmbi { get { return new EnValidationCode("NOT_VALIDATED_HAS_AMBI"); } }

        /// <summary>
        /// На проверку, большой номер дома
        /// </summary>
        public static EnValidationCode NotValidatedExcededHouseNumber { get { return new EnValidationCode("NOT_VALIDATED_EXCEDED_HOUSE_NUMBER"); } }

        /// <summary>
        /// На проверку, некорректный дом
        /// </summary>
        public static EnValidationCode NotValidatedIncorrectHouse { get { return new EnValidationCode("NOT_VALIDATED_INCORRECT_HOUSE"); } }

        /// <summary>
        /// На проверку, некорректное расширение дома
        /// </summary>
        public static EnValidationCode NotValidatedIncorrectHouseExtension { get { return new EnValidationCode("NOT_VALIDATED_INCORRECT_HOUSE_EXTENSION"); } }

        /// <summary>
        /// Иностранный адрес
        /// </summary>
        public static EnValidationCode NotValidatedForeign { get { return new EnValidationCode("NOT_VALIDATED_FOREIGN"); } }

        /// <summary>
        /// На проверку, не по справочнику
        /// </summary>
        public static EnValidationCode NotValidatedDictionary { get { return new EnValidationCode("NOT_VALIDATED_DICTIONARY"); } }
    }

    #endregion

    #region OrderRussianPost

    public abstract class BaseOrderRussianPost
    {
        /// <summary>
        /// Тип адреса
        /// </summary>
        [JsonProperty("address-type-to", NullValueHandling = NullValueHandling.Ignore)]
        public EnAddressType AddressTypeTo { get; set; }

        /// <summary>
        /// Район (Опционально)
        /// </summary>
        [JsonProperty("area-to", NullValueHandling = NullValueHandling.Ignore)]
        public string AreaTo { get; set; }

        /// <summary>
        /// Идентификатор подразделения (Опционально)
        /// </summary>
        [JsonProperty("branch-name", NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string BranchName { get; set; }

        /// <summary>
        /// Часть здания: Строение (Опционально)
        /// </summary>
        [JsonProperty("building-to", NullValueHandling = NullValueHandling.Ignore)]
        public string BuildingTo { get; set; }

        ///// <summary>
        ///// Комментарий (Опционально)
        ///// </summary>
        //[JsonProperty("comment", NullValueHandling = NullValueHandling.Ignore,
        //    DefaultValueHandling = DefaultValueHandling.Ignore)]
        //public string Comment { get; set; }

        /// <summary>
        /// Признак услуги проверки комплектности (Опционально)
        /// </summary>
        [JsonProperty("completeness-checking", NullValueHandling = NullValueHandling.Ignore)]
        public bool? CompletenessChecking { get; set; }

        /// <summary>
        /// К оплате с получателя (копейки) (Опционально)
        /// </summary>
        [JsonProperty("compulsory-payment", NullValueHandling = NullValueHandling.Ignore)]
        public long? CompulsoryPayment { get; set; }

        /// <summary>
        /// Часть здания: Корпус (Опционально)
        /// </summary>
        [JsonProperty("corpus-to", NullValueHandling = NullValueHandling.Ignore)]
        public string CorpusTo { get; set; }

        /// <summary>
        /// Таможенная декларация (для международных отправлений) (Опционально)
        /// </summary>
        [JsonProperty("customs-declaration", NullValueHandling = NullValueHandling.Ignore)]
        public CustomsDeclaration CustomsDeclaration { get; set; }

        /// <summary>
        /// Признак оплаты при получении (Опционально)
        /// </summary>
        [JsonProperty("delivery-with-cod", NullValueHandling = NullValueHandling.Ignore)]
        public bool? DeliveryWithCod { get; set; }

        /// <summary>
        /// Размеры (Опционально)
        /// </summary>
        [JsonProperty("dimension", NullValueHandling = NullValueHandling.Ignore)]
        public Dimension Dimension { get; set; }

        /// <summary>
        /// Типоразмер (Опционально)
        /// </summary>
        [JsonProperty("dimension-type", NullValueHandling = NullValueHandling.Ignore)]
        public EnDimensionType DimensionType { get; set; }

        /// <summary>
        /// Данные отправления ЕКОМ (Опционально)
        /// </summary>
        [JsonProperty("ecom-data", NullValueHandling = NullValueHandling.Ignore)]
        public EcomData EcomData { get; set; }

        /// <summary>
        /// Тип конверта (Опционально)
        /// </summary>
        [JsonProperty("envelope-type", NullValueHandling = NullValueHandling.Ignore)]
        public EnEnvelopeType EnvelopeType { get; set; }

        /// <summary>
        /// Фискальные данные (Опционально)
        /// </summary>
        [JsonProperty("fiscal-data", NullValueHandling = NullValueHandling.Ignore)]
        public FiscalData FiscalData { get; set; }

        /// <summary>
        /// Имя получателя
        /// </summary>
        [JsonProperty("given-name")]
        public string FirstName { get; set; }

        /// <summary>
        /// Товарное вложение РПО (Опционально)
        /// </summary>
        [JsonProperty("goods", NullValueHandling = NullValueHandling.Ignore)]
        public Goods Goods { get; set; }

        /// <summary>
        /// Название гостиницы (Опционально)
        /// </summary>
        [JsonProperty("hotel-to", NullValueHandling = NullValueHandling.Ignore)]
        public string HotelTo { get; set; }

        /// <summary>
        /// Номер здания (Опционально)
        /// </summary>
        [JsonProperty("house-to", NullValueHandling = NullValueHandling.Ignore)]
        public string HouseTo { get; set; }

        /// <summary>
        /// Почтовый индекс (Опционально)
        /// </summary>
        [JsonProperty("index-to", NullValueHandling = NullValueHandling.Ignore)]
        public int? IndexTo { get; set; }

        /// <summary>
        /// Сумма объявленной ценности (копейки) (Опционально)
        /// </summary>
        [JsonProperty("insr-value", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeclaredValue { get; set; }

        /// <summary>
        /// Часть здания: Литера (Опционально)
        /// </summary>
        [JsonProperty("letter-to", NullValueHandling = NullValueHandling.Ignore)]
        public string LetterTo { get; set; }

        /// <summary>
        /// Микрорайон (Опционально)
        /// </summary>
        [JsonProperty("location-to", NullValueHandling = NullValueHandling.Ignore)]
        public string LocationTo { get; set; }

        /// <summary>
        /// Категория РПО
        /// </summary>
        [JsonProperty("mail-category")]
        public EnMailCategory MailCategory { get; set; }

        /// <summary>
        /// Код страны
        /// </summary>
        [JsonProperty("mail-direct")]
        public int CountryCode { get; set; }

        /// <summary>
        /// Вид РПО
        /// </summary>
        [JsonProperty("mail-type")]
        public EnMailType MailType { get; set; }

        ///// <summary>
        ///// Отметка 'Ручной ввод адреса'
        ///// </summary>
        //[JsonProperty("manual-address-input")]
        //public bool ManualAddressInput { get; set; }

        /// <summary>
        /// Масса отправления в граммах
        /// </summary>
        [JsonProperty("mass")]
        public int Weight { get; set; }

        /// <summary>
        /// Отчество получателя (Опционально)
        /// </summary>
        [JsonProperty("middle-name", NullValueHandling = NullValueHandling.Ignore)]
        public string Patronymic { get; set; }

        /// <summary>
        /// Способ оплаты уведомления (Опционально)
        /// </summary>
        [JsonProperty("notice-payment-method", NullValueHandling = NullValueHandling.Ignore)]
        public EnPaymentMethod NoticePaymentMethod { get; set; }

        /// <summary>
        /// Номер для а/я, войсковая часть, войсковая часть ЮЯ, полевая почта (Опционально)
        /// </summary>
        [JsonProperty("num-address-type-to", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string NumAddressTypeTo { get; set; }

        /// <summary>
        /// Часть здания: Оффис (Опционально)
        /// </summary>
        [JsonProperty("office-to", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string OfficeTo { get; set; }

        /// <summary>
        /// Номер заказа. Внешний идентификатор заказа, который формируется отправителем
        /// </summary>
        [JsonProperty("order-num")]
        public string OrderNum { get; set; }

        /// <summary>
        /// Сумма наложенного платежа (копейки) (Опционально)
        /// </summary>
        [JsonProperty("payment", NullValueHandling = NullValueHandling.Ignore)]
        public long? CashOnDelivery { get; set; }

        /// <summary>
        /// Способ оплаты (Опционально)
        /// </summary>
        [JsonProperty("payment-method", NullValueHandling = NullValueHandling.Ignore)]
        public EnPaymentMethod PaymentMethod { get; set; }

        /// <summary>
        /// Населенный пункт
        /// </summary>
        [JsonProperty("place-to", NullValueHandling = NullValueHandling.Ignore)]
        public string PlaceTo { get; set; }

        /// <summary>
        /// Индекс места приема (Опционально) указанный в договоре
        /// </summary>
        [JsonProperty("postoffice-code", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string IndexFrom { get; set; }

        /// <summary>
        /// Необработанный адрес получателя (Опционально)
        /// </summary>
        [JsonProperty("raw-address", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string RawAddress { get; set; }

        /// <summary>
        /// Наименование получателя одной строкой (ФИО, наименование организации)
        /// </summary>
        [JsonProperty("recipient-name")]
        public string RecipientName { get; set; }

        /// <summary>
        /// Область, регион
        /// </summary>
        [JsonProperty("region-to", NullValueHandling = NullValueHandling.Ignore)]
        public string RegionTo { get; set; }

        /// <summary>
        /// Часть здания: Номер помещения (Опционально)
        /// </summary>
        [JsonProperty("room-to", NullValueHandling = NullValueHandling.Ignore)]
        public string RoomTo { get; set; }

        /// <summary>
        /// Часть здания: Дробь (Опционально)
        /// </summary>
        [JsonProperty("slash-to", NullValueHandling = NullValueHandling.Ignore)]
        public string SlashTo { get; set; }

        /// <summary>
        /// Признак услуги SMS уведомления (Опционально)
        /// </summary>
        [JsonProperty("sms-notice-recipient", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int? SmsNoticeRecipient { get; set; }

        /// <summary>
        /// Почтовый индекс (буквенно-цифровой)
        /// </summary>
        [JsonProperty("str-index-to", NullValueHandling = NullValueHandling.Ignore)]
        public string StrIndexTo { get; set; }

        /// <summary>
        /// Улица
        /// </summary>
        [JsonProperty("street-to", NullValueHandling = NullValueHandling.Ignore)]
        public string StreetTo { get; set; }

        /// <summary>
        /// Фамилия получателя
        /// </summary>
        [JsonProperty("surname")]
        public string LastName { get; set; }

        /// <summary>
        /// Телефон получателя (может быть обязательным для некоторых типов отправлений) (Опционально)
        /// </summary>
        [JsonProperty("tel-address", NullValueHandling = NullValueHandling.Ignore)]
        public long? Phone { get; set; }

        /// <summary>
        /// Возможный вид транспортировки (для международных отправлений) (Опционально)
        /// </summary>
        [JsonProperty("transport-type", NullValueHandling = NullValueHandling.Ignore)]
        public EnTransportType TransportType { get; set; }

        /// <summary>
        /// Часть здания: Владение (Опционально)
        /// </summary>
        [JsonProperty("vladenie-to", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string VladenieTo { get; set; }
    }

    public class OrderRussianPostAdd : BaseOrderRussianPost
    {
        /// <summary>
        /// Отметка 'Курьер' (Опционально) для типа доставки EmsOptimal
        /// </summary>
        [JsonProperty("courier", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Courier { get; set; }

        /// <summary>
        /// Лёгкий возврат (Опционально)
        /// </summary>
        [JsonProperty("easy-return", NullValueHandling = NullValueHandling.Ignore)]
        public bool? EasyReturn { get; set; }

        /// <summary>
        /// Отметка 'Осторожно/Хрупко' для посылки
        /// </summary>
        [JsonProperty("fragile", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Fragile { get; set; }

        /// <summary>
        /// Наличие описи вложения (Опционально)
        /// </summary>
        [JsonProperty("inventory", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Inventory { get; set; }

        /// <summary>
        /// Отметка "Возврату не подлежит" (Опционально)
        /// </summary>
        [JsonProperty("no-return", NullValueHandling = NullValueHandling.Ignore)]
        public bool? NoReturn { get; set; }

        /// <summary>
        /// Идентификатор временного интервала (Опционально)
        /// </summary>
        [JsonProperty("time-slot-id", NullValueHandling = NullValueHandling.Ignore)]
        public int? TimeSlotId { get; set; }

        /// <summary>
        /// Возврат сопроводительныйх документов (Опционально)
        /// </summary>
        [JsonProperty("vsd", NullValueHandling = NullValueHandling.Ignore)]
        public bool? ReturnAccompanyingDocuments { get; set; }

        /// <summary>
        /// Отметка 'С электронным уведомлением' (Опционально)
        /// </summary>
        [JsonProperty("with-electronic-notice", NullValueHandling = NullValueHandling.Ignore)]
        public bool? WithElectronicNotice { get; set; }

        /// <summary>
        /// Отметка 'С заказным уведомлением' (Опционально)
        /// </summary>
        [JsonProperty("with-order-of-notice", NullValueHandling = NullValueHandling.Ignore)]
        public bool? WithOrderOfNotice { get; set; }

        /// <summary>
        /// Отметка 'С простым уведомлением' (Опционально)
        /// </summary>
        [JsonProperty("with-simple-notice", NullValueHandling = NullValueHandling.Ignore)]
        public bool? WithSimpleNotice { get; set; }

        /// <summary>
        /// Отметка 'Без разряда' (Опционально)
        /// </summary>
        [JsonProperty("wo-mail-rank", NullValueHandling = NullValueHandling.Ignore)]
        public bool? WoMailRank { get; set; }
    }

    public class OrderRussianPost : BaseOrderRussianPost
    {
        /// <summary>
        /// Внешний идентификатор заказа (Опционально)
        /// </summary>
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public long? Id { get; set; }

        /// <summary>
        /// Адрес получателя скорректирован в процессе очистки (Опционально)
        /// </summary>
        [JsonProperty("address-changed", NullValueHandling = NullValueHandling.Ignore)]
        public bool? AddressChanged { get; set; }

        /// <summary>
        /// Авиа-сбор без НДС (для совместимости) (Опционально)
        /// </summary>
        [JsonProperty("avia-rate", NullValueHandling = NullValueHandling.Ignore)]
        public int? AviaRate { get; set; }

        /// <summary>
        /// Авиа-сбор с НДС (Опционально)
        /// </summary>
        [JsonProperty("avia-rate-with-vat", NullValueHandling = NullValueHandling.Ignore)]
        public int? AviaRateWithVat { get; set; }

        /// <summary>
        /// Авиа-сбор без НДС (Опционально)
        /// </summary>
        [JsonProperty("avia-rate-wo-vat", NullValueHandling = NullValueHandling.Ignore)]
        public int? AviaRateWoVat { get; set; }

        /// <summary>
        /// Штриховой почтовый идентификатор (Опционально)
        /// </summary>
        [JsonProperty("barcode", NullValueHandling = NullValueHandling.Ignore)]
        public string Barcode { get; set; }

        /// <summary>
        /// Надбавка за "Проверку комплектности" с НДС
        /// </summary>
        [JsonProperty("completeness-checking-rate-with-vat", NullValueHandling = NullValueHandling.Ignore)]
        public int? CompletenessCheckingRateWithVat { get; set; }

        /// <summary>
        /// Надбавка за "Проверку комплектности" без НДС (Опционально)
        /// </summary>
        [JsonProperty("completeness-checking-rate-wo-vat", NullValueHandling = NullValueHandling.Ignore)]
        public int? CompletenessCheckingRateWoVat { get; set; }

        /// <summary>
        /// Надбавка за "Проверку вложений" с НДС (Опционально)
        /// </summary>
        [JsonProperty("contents-checking-rate-with-vat", NullValueHandling = NullValueHandling.Ignore)]
        public int? ContentsCheckingRateWithVat { get; set; }

        /// <summary>
        /// Надбавка за "Проверку вложений" без НДС (Опционально)
        /// </summary>
        [JsonProperty("contents-checking-rate-wo-vat", NullValueHandling = NullValueHandling.Ignore)]
        public int? ContentsCheckingRateWoVat { get; set; }

        /// <summary>
        /// Примерные сроки доставки (Опционально)
        /// </summary>
        [JsonProperty("delivery-time", NullValueHandling = NullValueHandling.Ignore)]
        public DeliveryTime DeliveryTime { get; set; }

        /// <summary>
        /// Сбор за "Лёгкий возврат" с НДС
        /// </summary>
        [JsonProperty("easy-return-rate-with-vat", NullValueHandling = NullValueHandling.Ignore)]
        public int? EasyReturnRateWithVat { get; set; }

        /// <summary>
        /// Сбор за "Лёгкий возврат" без НДС
        /// </summary>
        [JsonProperty("easy-return-rate-wo-vat", NullValueHandling = NullValueHandling.Ignore)]
        public int? EasyReturnRateWoVat { get; set; }

        /// <summary>
        /// Надбавка за отметку 'Осторожно/Хрупкое' (Опционально)
        /// </summary>
        [JsonProperty("fragile-rate-with-vat", NullValueHandling = NullValueHandling.Ignore)]
        public int? FragileRateWithVat { get; set; }

        /// <summary>
        /// Надбавка за отметку 'Осторожно/Хрупкое' без НДС (Опционально)
        /// </summary>
        [JsonProperty("fragile-rate-wo-vat", NullValueHandling = NullValueHandling.Ignore)]
        public int? FragileRateNoVat { get; set; }

        /// <summary>
        /// Сбор за доставку наземно без НДС (для совместимости) (Опционально)
        /// </summary>
        [JsonProperty("ground-rate", NullValueHandling = NullValueHandling.Ignore)]
        public int? GroundRate { get; set; }

        /// <summary>
        /// Сбор за доставку наземно с НДС (Опционально)
        /// </summary>
        [JsonProperty("ground-rate-with-vat", NullValueHandling = NullValueHandling.Ignore)]
        public int? GroundRateWithVat { get; set; }

        /// <summary>
        /// Сбор за доставку наземно без НДС (Опционально)
        /// </summary>
        [JsonProperty("ground-rate-wo-vat", NullValueHandling = NullValueHandling.Ignore)]
        public int? GroundRateNoVat { get; set; }

        /// <summary>
        /// Плата за ОЦ без НДС (для совместимости) (Опционально)
        /// </summary>
        [JsonProperty("insr-rate", NullValueHandling = NullValueHandling.Ignore)]
        public int? InsuranceRate { get; set; }

        /// <summary>
        /// Плата за ОЦ с НДС (Опционально)
        /// </summary>
        [JsonProperty("insr-rate-with-vat", NullValueHandling = NullValueHandling.Ignore)]
        public int? InsuranceRateWithVat { get; set; }

        /// <summary>
        /// Плата за ОЦ без НДС (Опционально)
        /// </summary>
        [JsonProperty("insr-rate-wo-vat", NullValueHandling = NullValueHandling.Ignore)]
        public int? InsuranceRateNoVat { get; set; }

        /// <summary>
        /// Надбавка за "Опись вложения" с НДС
        /// </summary>
        [JsonProperty("inventory-rate-with-vat", NullValueHandling = NullValueHandling.Ignore)]
        public int? InventoryRateWithVat { get; set; }

        /// <summary>
        /// Надбавка за "Опись вложения" без НДС
        /// </summary>
        [JsonProperty("inventory-rate-wo-vat", NullValueHandling = NullValueHandling.Ignore)]
        public int? InventoryRateWoVat { get; set; }

        /// <summary>
        /// Заказ удален (Опционально)
        /// </summary>
        [JsonProperty("is-deleted", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsDeleted { get; set; }

        /// <summary>
        /// ШПИ связанного отправления (Опционально)
        /// </summary>
        [JsonProperty("linked-barcode", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string LinkedBarcode { get; set; }

        /// <summary>
        /// Разряд письма
        /// </summary>
        [JsonProperty("mail-rank")]
        public EnMailRank MailRank { get; set; }

        /// <summary>
        /// Почтовый сбор без НДС (для совместимости) (Опционально)
        /// </summary>
        [JsonProperty("mass-rate", NullValueHandling = NullValueHandling.Ignore)]
        public int? MassRate { get; set; }

        /// <summary>
        /// Почтовый сбор с НДС (Опционально)
        /// </summary>
        [JsonProperty("mass-rate-with-vat", NullValueHandling = NullValueHandling.Ignore)]
        public int? MassRateWithVat { get; set; }

        /// <summary>
        /// Почтовый сбор без НДС (Опционально)
        /// </summary>
        [JsonProperty("mass-rate-wo-vat", NullValueHandling = NullValueHandling.Ignore)]
        public int? MassRateNoVat { get; set; }

        /// <summary>
        /// Надбавка за уведомление о вручении с НДС (Опционально)
        /// </summary>
        [JsonProperty("notice-rate-with-vat", NullValueHandling = NullValueHandling.Ignore)]
        public int? NoticeRateWithVat { get; set; }

        /// <summary>
        /// Надбавка за уведомление о вручении без НДС (Опционально)
        /// </summary>
        [JsonProperty("notice-rate-wo-vat", NullValueHandling = NullValueHandling.Ignore)]
        public int? NoticeRateNoVat { get; set; }

        /// <summary>
        /// Надбавка за негабарит при весе более 10кг с НДС (Опционально)
        /// </summary>
        [JsonProperty("oversize-rate-with-vat", NullValueHandling = NullValueHandling.Ignore)]
        public int? OversizeRateWithVat { get; set; }

        /// <summary>
        /// Надбавка за негабарит при весе более 10кг без НДС (Опционально)
        /// </summary>
        [JsonProperty("oversize-rate-wo-vat", NullValueHandling = NullValueHandling.Ignore)]
        public int? OversizeRateNoVat { get; set; }

        /// <summary>
        /// Коды отметок внутренних и международных отправлений
        /// </summary>
        [JsonProperty("postmarks")]
        public List<EnPostmark> Postmarks { get; set; }

        /// <summary>
        /// Надбавка за 'Пакет смс получателю' с НДС (Опционально)
        /// </summary>
        [JsonProperty("sms-notice-recipient-rate-with-vat", NullValueHandling = NullValueHandling.Ignore)]
        public int? SmsNoticeRecipientRateWithVat { get; set; }

        /// <summary>
        /// Надбавка за 'Пакет смс получателю' без НДС (Опционально)
        /// </summary>
        [JsonProperty("sms-notice-recipient-rate-wo-vat", NullValueHandling = NullValueHandling.Ignore)]
        public int? SmsNoticeRecipientRateNoVat { get; set; }

        /// <summary>
        /// Плата всего без НДС (коп)
        /// </summary>
        [JsonProperty("total-rate-wo-vat")]
        public int TotalRateNoVat { get; set; }

        /// <summary>
        /// Всего НДС (коп)
        /// </summary>
        [JsonProperty("total-vat")]
        public int TotalVat { get; set; }

        /// <summary>
        /// Версия заказа
        /// </summary>
        [JsonProperty("version")]
        public long Version { get; set; }

        /// <summary>
        /// Надбавка за "Возврат сопроводительных документов" с НДС
        /// </summary>
        [JsonProperty("vsd-rate-with-vat", NullValueHandling = NullValueHandling.Ignore)]
        public int? VsdRateWithVat { get; set; }

        /// <summary>
        /// Надбавка за "Возврат сопроводительных документов" без НДС
        /// </summary>
        [JsonProperty("vsd-rate-wo-vat", NullValueHandling = NullValueHandling.Ignore)]
        public int? VsdRateWoVat { get; set; }
    }

    public class CustomsDeclaration
    {
        /// <summary>
        /// Сертификаты, сопровождающие отправление (Опционально)
        /// </summary>
        [JsonProperty("certificate-number", NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string CertificateNumber { get; set; }

        /// <summary>
        /// Код валюты ISO3
        /// </summary>
        [JsonProperty("currency")]
        public string Currency { get; set; }

        /// <summary>
        /// Список вложений
        /// </summary>
        [JsonProperty("customs-entries")]
        public List<CustomsEntry> CustomsEntries { get; set; }

        /// <summary>
        /// Категория вложения
        /// </summary>
        [JsonProperty("entries-type")]
        public EnEntriesType EntriesType { get; set; }

        /// <summary>
        /// Счет (номер счета-фактуры) (Опционально)
        /// </summary>
        [JsonProperty("invoice-number", NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string InvoiceNumber { get; set; }

        /// <summary>
        /// Лицензии, сопровождающие отправление (Опционально)
        /// </summary>
        [JsonProperty("license-number", NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string LicenseNumber { get; set; }

        /// <summary>
        /// Приложенные документы: сертификат (Опционально)
        /// </summary>
        [JsonProperty("with-certificate", NullValueHandling = NullValueHandling.Ignore)]
        public bool? WithCertificate { get; set; }

        /// <summary>
        /// Приложенные документы: счет-фактура (Опционально)
        /// </summary>
        [JsonProperty("with-invoice", NullValueHandling = NullValueHandling.Ignore)]
        public bool? WithInvoice { get; set; }

        /// <summary>
        /// Приложенные документы: лицензия (Опционально)
        /// </summary>
        [JsonProperty("with-license", NullValueHandling = NullValueHandling.Ignore)]
        public bool? WithLicense { get; set; }
    }

    public class CustomsEntry
    {
        /// <summary>
        /// Количество
        /// </summary>
        [JsonProperty("amount")]
        public int Amount { get; set; }

        /// <summary>
        /// Код страны происхождения (Опционально)
        /// </summary>
        [JsonProperty("country-code", NullValueHandling = NullValueHandling.Ignore)]
        public int? CountryCode { get; set; }

        /// <summary>
        /// Наименование товара
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Код ТНВЭД
        /// </summary>
        [JsonProperty("tnved-code")]
        public string TnvedCode { get; set; }

        /// <summary>
        /// Торговая марка
        /// </summary>
        [JsonProperty("trademark")]
        public string Brand { get; set; }

        /// <summary>
        /// Цена за единицу товара в копейках (вкл. НДС) (Опционально)
        /// </summary>
        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        public long? Value { get; set; }

        /// <summary>
        /// Вес товара (в граммах) (Опционально)
        /// </summary>
        [JsonProperty("weight", NullValueHandling = NullValueHandling.Ignore)]
        public int? Weight { get; set; }
    }

    public class EcomData
    {
        /// <summary>
        /// Идентификатор пункта выдачи заказов (Опционально)
        /// </summary>
        [JsonProperty("delivery-point-index", NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string DeliveryPointIndex { get; set; }

        /// <summary>
        /// Услуга по идентификации личности получателя в месте вручения (Опционально)
        /// </summary>
        [JsonProperty("identity-methods", NullValueHandling = NullValueHandling.Ignore)]
        public List<EnIdentityMethod> IdentityMethods { get; set; }

        /// <summary>
        /// Тип пункта выдачи (Опционально)
        /// <para>Не передается, только возвражается из ПР</para>
        /// </summary>
        [JsonProperty("delivery-point-type", NullValueHandling = NullValueHandling.Ignore)]
        public EnDeliveryPointType DeliveryPointType { get; set; }

        /// <summary>
        /// Сервисы ЕКОМ (Опционально)
        /// </summary>
        [JsonProperty("services", NullValueHandling = NullValueHandling.Ignore)]
        public List<EnEcomService> Services { get; set; }
    }

    public class FiscalData
    {
        /// <summary>
        /// Адрес электронной почты плательщика (Опционально)
        /// </summary>
        [JsonProperty("customer-email", NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string CustomerEmail { get; set; }

        /// <summary>
        /// ИНН юридического лица покупателя (Опционально)
        /// </summary>
        [JsonProperty("customer-inn", NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string CustomerInn { get; set; }

        /// <summary>
        /// Наименование юридического лица покупателя (Опционально)
        /// </summary>
        [JsonProperty("customer-name", NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string CustomerName { get; set; }

        /// <summary>
        /// Телефон плательщика (Опционально)
        /// </summary>
        [JsonProperty("customer-phone", NullValueHandling = NullValueHandling.Ignore)]
        public long? CustomerPhone { get; set; }

        /// <summary>
        /// Сумма предоплаты (копейки) (Опционально)
        /// <para>Только для передачи в ПР, не возвращается из ПР при запросе заказа</para>
        /// </summary>
        [JsonProperty("payment-amount", NullValueHandling = NullValueHandling.Ignore)]
        public long? PaymentAmount { get; set; }

        /// <summary>
        /// Средства, использованные при оплате (Опционально)
        /// <para>Не передается, только возвражается из ПР</para>
        /// </summary>
        [JsonProperty("fiscal-payments", NullValueHandling = NullValueHandling.Ignore)]
        public List<FiscalPayment> FiscalPayments { get; set; }
    }

    public class FiscalPayment
    {
        /// <summary>
        /// Сумма оплаты платежным средством,(копейки) (Опционально)
        /// </summary>
        [JsonProperty("payment-amount", NullValueHandling = NullValueHandling.Ignore)]
        public int? PaymentAmount { get; set; }

        /// <summary>
        /// Вид платежного средства (Опционально)
        /// </summary>
        [JsonProperty("payment-kind", NullValueHandling = NullValueHandling.Ignore)]
        public int? PaymentKind { get; set; }

        /// <summary>
        /// Тип платежного средства (Опционально)
        /// </summary>
        [JsonProperty("payment-type", NullValueHandling = NullValueHandling.Ignore)]
        public int? PaymentType { get; set; }
    }

    public class Goods
    {
        /// <summary>
        /// Список вложений
        /// </summary>
        [JsonProperty("items")]
        public List<GoodsItem> Items { get; set; }
    }

    public class GoodsItem
    {
        /// <summary>
        /// Категория товара (Опционально)
        /// <para>Не передается, только возвражается из ПР</para>
        /// </summary>
        [JsonProperty("category-feature", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string CategoryFeature { get; set; }

        /// <summary>
        /// Код (маркировка) товара (Опционально)
        /// </summary>
        [JsonProperty("code", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Code { get; set; }

        /// <summary>
        /// Код страны происхождения (Опционально)
        /// </summary>
        [JsonProperty("country-code", NullValueHandling = NullValueHandling.Ignore)]
        public int? CountryCode { get; set; }

        /// <summary>
        /// Номер таможенной декларации (Опционально)
        /// </summary>
        [JsonProperty("customs-declaration-number", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string CustomsDeclarationNumber { get; set; }

        /// <summary>
        /// Наименование товара
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Акциз (копейки) (Опционально)
        /// </summary>
        [JsonProperty("excise", NullValueHandling = NullValueHandling.Ignore)]
        public int? Excise { get; set; }

        /// <summary>
        /// Тип вложения (Опционально)
        /// </summary>
        [JsonProperty("goods-type", NullValueHandling = NullValueHandling.Ignore)]
        public EnGoodsType GoodsType { get; set; }

        /// <summary>
        /// Объявленная ценность (копейки) (Опционально)
        /// </summary>
        [JsonProperty("insr-value", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeclareValue { get; set; }

        /// <summary>
        /// Номенклатура (артикул) товара (Опционально)
        /// </summary>
        [JsonProperty("item-number", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string ArtNo { get; set; }

        /// <summary>
        /// Признак предмета расчета (Опционально)
        /// </summary>
        [JsonProperty("lineattr", NullValueHandling = NullValueHandling.Ignore)]
        public int? PaymentSubjectType { get; set; }

        /// <summary>
        /// Признак способа расчета (Опционально)
        /// </summary>
        [JsonProperty("payattr", NullValueHandling = NullValueHandling.Ignore)]
        public int? PaymentMethodType { get; set; }

        /// <summary>
        /// Количество товара
        /// </summary>
        [JsonProperty("quantity")]
        public long Quantity { get; set; }

        /// <summary>
        /// ИНН поставщика товара (Опционально)
        /// </summary>
        [JsonProperty("supplier-inn", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string SupplierInn { get; set; }

        /// <summary>
        /// Наименование поставщика товара (Опционально)
        /// </summary>
        [JsonProperty("supplier-name", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string SupplierName { get; set; }

        /// <summary>
        /// Телефон поставщика товара (Опционально)
        /// </summary>
        [JsonProperty("supplier-phone", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string SupplierPhone { get; set; }

        /// <summary>
        /// Цена за единицу товара в копейках (вкл. НДС) (Опционально)
        /// </summary>
        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        public long Value { get; set; }

        /// <summary>
        /// Ставка НДС (Опционально)
        /// <para>Без НДС(-1), 0, 10, 110, 20, 120</para>
        /// </summary>
        [JsonProperty("vat-rate", NullValueHandling = NullValueHandling.Ignore)]
        public int? VatRate { get; set; }

        /// <summary>
        /// Вес товара (в граммах) (Опционально)
        /// </summary>
        [JsonProperty("weight", NullValueHandling = NullValueHandling.Ignore)]
        public int? Weight { get; set; }
    }

    public class EnGoodsType : StringEnum<EnGoodsType>
    {
        public EnGoodsType(string value) : base(value)
        {
        }

        /// <summary>
        /// Товар
        /// </summary>
        public static EnGoodsType GOODS { get { return new EnGoodsType("GOODS"); } }

        /// <summary>
        /// Услуга
        /// </summary>
        public static EnGoodsType SERVICE { get { return new EnGoodsType("SERVICE"); } }
    }

    public class EnEcomService : StringEnum<EnEcomService>
    {
        public EnEcomService(string value) : base(value)
        {
        }

        /// <summary>
        /// Без сервиса
        /// </summary>
        public static EnEcomService WITHOUT_SERVICE { get { return new EnEcomService("WITHOUT_SERVICE"); } }

        /// <summary>
        /// Без вскрытия
        /// </summary>
        public static EnEcomService WITHOUT_OPENING { get { return new EnEcomService("WITHOUT_OPENING"); } }

        /// <summary>
        /// С проверкой вложения
        /// </summary>
        public static EnEcomService CONTENTS_CHECKING { get { return new EnEcomService("CONTENTS_CHECKING"); } }

        /// <summary>
        /// С примеркой
        /// </summary>
        public static EnEcomService WITH_FITTING { get { return new EnEcomService("WITH_FITTING"); } }

        /// <summary>
        /// Доставка курьером
        /// </summary>
        public static EnEcomService COURIER_DELIVERY { get { return new EnEcomService("COURIER_DELIVERY"); } }

        /// <summary>
        /// С частичным выкупом
        /// </summary>
        public static EnEcomService PARTIAL_REDEMPTION { get { return new EnEcomService("PARTIAL_REDEMPTION"); } }

        /// <summary>
        /// С проверкой работоспособности
        /// </summary>
        public static EnEcomService FUNCTIONALITY_CHECKING { get { return new EnEcomService("FUNCTIONALITY_CHECKING"); } }
    }

    public class EnIdentityMethod : StringEnum<EnIdentityMethod>
    {
        public EnIdentityMethod(string value) : base(value)
        {
        }

        /// <summary>
        /// без идентификации (доступно только для ПВЗ)
        /// </summary>
        public static EnIdentityMethod WITHOUT_IDENTIFICATION { get { return new EnIdentityMethod("WITHOUT_IDENTIFICATION"); } }

        /// <summary>
        /// с проверкой документа удостоверяющего личность (доступно только для ПВЗ)
        /// </summary>
        public static EnIdentityMethod IDENTITY_DOCUMENT { get { return new EnIdentityMethod("IDENTITY_DOCUMENT"); } }

        /// <summary>
        /// с проверкой пин-кода (доступно только для АПС, на данный момент ко всем отправлениям в АПС подключается проверка пин-кода автоматически)
        /// </summary>
        public static EnIdentityMethod PIN { get { return new EnIdentityMethod("PIN"); } }

        /// <summary>
        /// Номер заказа и ФИО (для отделений почтовой связи)
        /// </summary>
        public static EnIdentityMethod ORDER_NUM_AND_FIO { get { return new EnIdentityMethod("ORDER_NUM_AND_FIO"); } }

        /*
         * Услуга по идентификации личности получателя в месте вручения
         * Для партнерских ПВЗ:
         * "PIN" - с проверкой пин-кода
         * "PIN",  "IDENTITY_DOCUMENT" – с проверкой документа удостоверяющего личность + по пин-коду
         * Для отделений почтовой связи:
         * "WITHOUT_IDENTIFICATION" – без идентификации
         * "IDENTITY_DOCUMENT" – с проверкой документа, удостоверяющего личность 
         * Для почтоматов:
         * "PIN" - с проверкой пин-кода

         */
    }

    //[JsonConverter(typeof(StringEnumConverter))]
    //public enum EnEnvelopeType
    public class EnEnvelopeType : StringEnum<EnEnvelopeType>
    {
        public EnEnvelopeType(string value) : base(value)
        {
        }

        /// <summary>
        /// 229мм x 324мм
        /// </summary>
        public static EnEnvelopeType C4 { get { return new EnEnvelopeType("C4"); } }

        /// <summary>
        /// 162мм x 229мм
        /// </summary>
        public static EnEnvelopeType C5 { get { return new EnEnvelopeType("C5"); } }

        /// <summary>
        /// 110мм x 220мм
        /// </summary>
        public static EnEnvelopeType DL { get { return new EnEnvelopeType("DL"); } }

        /// <summary>
        /// 105мм x 148мм
        /// </summary>
        public static EnEnvelopeType A6 { get { return new EnEnvelopeType("A6"); } }

        /// <summary>
        /// 74мм x 105мм
        /// </summary>
        public static EnEnvelopeType A7 { get { return new EnEnvelopeType("A7"); } }
    }

    //[JsonConverter(typeof(StringEnumConverter))]
    //public enum EnEntriesType
    public class EnEntriesType : StringEnum<EnEntriesType>
    {
        public EnEntriesType(string value) : base(value)
        {
        }

        /// <summary>
        /// Подарок
        /// </summary>
        public static EnEntriesType Gift { get { return new EnEntriesType("GIFT"); } }

        /// <summary>
        /// Документы
        /// </summary>
        public static EnEntriesType Document { get { return new EnEntriesType("DOCUMENT"); } }

        /// <summary>
        /// Продажа товара
        /// </summary>
        public static EnEntriesType SaleOfGoods { get { return new EnEntriesType("SALE_OF_GOODS"); } }

        /// <summary>
        /// Коммерческий образец
        /// </summary>
        public static EnEntriesType CommercialSample { get { return new EnEntriesType("COMMERCIAL_SAMPLE"); } }

        /// <summary>
        /// Прочее
        /// </summary>
        public static EnEntriesType Other { get { return new EnEntriesType("OTHER"); } }
    }

    //[JsonConverter(typeof(StringEnumConverter))]
    //public enum EnMailRank
    public class EnMailRank : StringEnum<EnMailRank>
    {
        public EnMailRank(string value) : base(value)
        {
        }

        /// <summary>
        /// Без разряда
        /// </summary>
        public static EnMailRank WoRank { get { return new EnMailRank("WO_RANK"); } }

        /// <summary>
        /// Правительственное
        /// </summary>
        public static EnMailRank Governmental { get { return new EnMailRank("GOVERNMENTAL"); } }

        /// <summary>
        /// Воинское
        /// </summary>
        public static EnMailRank Military { get { return new EnMailRank("MILITARY"); } }

        /// <summary>
        /// Служебное
        /// </summary>
        public static EnMailRank Official { get { return new EnMailRank("OFFICIAL"); } }

        /// <summary>
        /// Судебное
        /// </summary>
        public static EnMailRank Judicial { get { return new EnMailRank("JUDICIAL"); } }

        /// <summary>
        /// Президентское
        /// </summary>
        public static EnMailRank Presidential { get { return new EnMailRank("PRESIDENTIAL"); } }

        /// <summary>
        /// Кредитное
        /// </summary>
        public static EnMailRank Credit { get { return new EnMailRank("CREDIT"); } }

        /// <summary>
        /// Административное
        /// </summary>
        public static EnMailRank Administrative { get { return new EnMailRank("ADMINISTRATIVE"); } }
    }

    //[JsonConverter(typeof(StringEnumConverter))]
    //public enum EnPostmark
    public class EnPostmark : StringEnum<EnPostmark>
    {
        public EnPostmark(string value) : base(value)
        {
        }

        /// <summary>
        /// Без отметки
        /// </summary>
        public static EnPostmark WithoutMark { get { return new EnPostmark("WITHOUT_MARK"); } }

        /// <summary>
        /// С простым уведомлением
        /// </summary>
        public static EnPostmark WithSimpleNotice { get { return new EnPostmark("WITH_SIMPLE_NOTICE"); } }

        /// <summary>
        /// С заказным уведомлением
        /// </summary>
        public static EnPostmark WithOrderOfNotice { get { return new EnPostmark("WITH_ORDER_OF_NOTICE"); } }

        /// <summary>
        /// С описью
        /// </summary>
        public static EnPostmark WithInventory { get { return new EnPostmark("WITH_INVENTORY"); } }

        /// <summary>
        /// Осторожно (Хрупкая)
        /// </summary>
        public static EnPostmark CautionFragile { get { return new EnPostmark("CAUTION_FRAGILE"); } }

        /// <summary>
        /// Тяжеловесная
        /// </summary>
        public static EnPostmark HeavyHanded { get { return new EnPostmark("HEAVY_HANDED"); } }

        /// <summary>
        /// Крупногабаритная (Громоздкая)
        /// </summary>
        public static EnPostmark LargeBulky { get { return new EnPostmark("LARGE_BULKY"); } }

        /// <summary>
        /// С доставкой (Доставка нарочным)
        /// </summary>
        public static EnPostmark WithDelivery { get { return new EnPostmark("WITH_DELIVERY"); } }

        /// <summary>
        /// Вручить в собственные руки
        /// </summary>
        public static EnPostmark AwardedInOwnHands { get { return new EnPostmark("AWARDED_IN_OWN_HANDS"); } }

        /// <summary>
        /// С документами
        /// </summary>
        public static EnPostmark WithDocuments { get { return new EnPostmark("WITH_DOCUMENTS"); } }

        /// <summary>
        /// С товарами
        /// </summary>
        public static EnPostmark WithGoods { get { return new EnPostmark("WITH_GOODS"); } }

        /// <summary>
        /// Возврату не подлежит
        /// </summary>
        public static EnPostmark NoReturn { get { return new EnPostmark("NO_RETURN"); } }

        /// <summary>
        /// Нестандартная
        /// </summary>
        public static EnPostmark Nonstandard { get { return new EnPostmark("NONSTANDARD"); } }

        /// <summary>
        /// 
        /// </summary>
        public static EnPostmark Border { get { return new EnPostmark("BORDER"); } }

        /// <summary>
        /// Застраховано
        /// </summary>
        public static EnPostmark Insured { get { return new EnPostmark("INSURED"); } }

        /// <summary>
        /// С электронным уведомлением
        /// </summary>
        public static EnPostmark WithElectronicNotification { get { return new EnPostmark("WITH_ELECTRONIC_NOTIFICATION"); } }

        /// <summary>
        /// Курьер бизнес-экспресс
        /// </summary>
        public static EnPostmark BusinessCourierExpress { get { return new EnPostmark("BUSINESS_COURIER_EXPRESS"); } }

        /// <summary>
        /// Нестандартная до 10 кг
        /// </summary>
        public static EnPostmark NonstandardUpto10kg { get { return new EnPostmark("NONSTANDARD_UPTO_10KG"); } }

        /// <summary>
        /// Нестандартная до 20 кг
        /// </summary>
        public static EnPostmark NonstandardUpto20kg { get { return new EnPostmark("NONSTANDARD_UPTO_20KG"); } }

        /// <summary>
        /// С наложенным платежом
        /// </summary>
        public static EnPostmark WithCashOnDelivery { get { return new EnPostmark("WITH_CASH_ON_DELIVERY"); } }

        /// <summary>
        /// Гарантия сохранности
        /// </summary>
        public static EnPostmark SafetyGuarantee { get { return new EnPostmark("SAFETY_GUARANTEE"); } }

        /// <summary>
        /// Заверительный пакет
        /// </summary>
        public static EnPostmark AssurePackage { get { return new EnPostmark("ASSURE_PACKAGE"); } }

        /// <summary>
        /// Доставка курьером
        /// </summary>
        public static EnPostmark CourierDelivery { get { return new EnPostmark("COURIER_DELIVERY"); } }

        /// <summary>
        /// Проверка комплектности
        /// </summary>
        public static EnPostmark CompletenessChecking { get { return new EnPostmark("COMPLETENESS_CHECKING"); } }

        /// <summary>
        /// Негабаритная
        /// </summary>
        public static EnPostmark Oversized { get { return new EnPostmark("OVERSIZED"); } }

        /// <summary>
        /// В упаковке Почты России
        /// </summary>
        public static EnPostmark RupostPackage { get { return new EnPostmark("RUPOST_PACKAGE"); } }

        /// <summary>
        /// Оплата при получении
        /// </summary>
        public static EnPostmark DeliveryWithCod { get { return new EnPostmark("DELIVERY_WITH_COD"); } }

        /// <summary>
        /// Возврат сопроводительных документов
        /// </summary>
        public static EnPostmark VSD { get { return new EnPostmark("VSD"); } }

        /// <summary>
        /// Легкий возврат
        /// </summary>
        public static EnPostmark EasyReturn { get { return new EnPostmark("EASY_RETURN"); } }
    }

    //[JsonConverter(typeof(StringEnumConverter))]
    //public enum EnTransportType
    public class EnTransportType : StringEnum<EnTransportType>
    {
        public EnTransportType(string value) : base(value)
        {
        }

        /// <summary>
        /// Используется для отправлений "EMS Оптимальное"
        /// </summary>
        [Localize("Стандарт")]
        public static EnTransportType Standard { get { return new EnTransportType("STANDARD"); } }

        /// <summary>
        /// Наземный
        /// </summary>
        [Localize("Наземный")]
        public static EnTransportType Surface { get { return new EnTransportType("SURFACE"); } }

        /// <summary>
        /// Авиа
        /// </summary>
        [Localize("Авиа")]
        public static EnTransportType Avia { get { return new EnTransportType("AVIA"); } }

        /// <summary>
        /// Комбинированный
        /// </summary>
        [Localize("Комбинированный")]
        public static EnTransportType Combined { get { return new EnTransportType("COMBINED"); } }

        /// <summary>
        /// Системой ускоренной почты
        /// </summary>
        [Localize("Системой ускоренной почты")]
        public static EnTransportType Express { get { return new EnTransportType("EXPRESS"); } }
    }

    public class OrderRussianPostAddResponse : BaseOrderRussianPostResponse
    {
    }

    public class OrderRussianPostDeleteResponse : BaseOrderRussianPostResponse
    {
    }

    #endregion

    #region AccountSettings

    public class AccountSettings
    {
        /// <summary>
        /// Признак администратора акаунта
        /// </summary>
        [JsonProperty("account-admin")]
        public bool AccountAdmin { get; set; }

        /// <summary>
        /// Список компаний
        /// </summary>
        [JsonProperty("accounts")]
        public List<Account> Accounts { get; set; }

        /// <summary>
        /// Тип адреса возврата (Опционально)
        /// </summary>
        [JsonProperty("address", NullValueHandling = NullValueHandling.Ignore)]
        public Address Address { get; set; }

        /// <summary>
        /// Внутренний идентификатор администратора
        /// </summary>
        [JsonProperty("admin-hid")]
        public string AdminHid { get; set; }

        /// <summary>
        /// Номер договора (Опционально)
        /// </summary>
        [JsonProperty("agreement-number", NullValueHandling = NullValueHandling.Ignore)]
        public string AgreementNumber { get; set; }

        /// <summary>
        /// Флаг доступа к API
        /// </summary>
        [JsonProperty("api_enabled")]
        public bool ApiEnabled { get; set; }

        /// <summary>
        /// API Gateway access token
        /// </summary>
        [JsonProperty("apig_access_token")]
        public string ApigAccessToken { get; set; }

        /// <summary>
        /// Все точки сдачи (Опционально)
        /// </summary>
        [JsonProperty("available-shipping-points")]
        public List<ShippingPoint> AvailableShippingPoints { get; set; }

        /// <summary>
        /// Признак блокировки
        /// </summary>
        [JsonProperty("blocked")]
        public bool Blocked { get; set; }

        /// <summary>
        /// Отправитель на посылке/название брэнда (Опционально)
        /// </summary>
        [JsonProperty("brand-name", NullValueHandling = NullValueHandling.Ignore)]
        public string BrandName { get; set; }

        /// <summary>
        /// Контактный email (Опционально)
        /// </summary>
        [JsonProperty("contact-email", NullValueHandling = NullValueHandling.Ignore)]
        public string ContactEmail { get; set; }

        /// <summary>
        /// Контактный телефон (Опционально)
        /// </summary>
        [JsonProperty("contact-phone", NullValueHandling = NullValueHandling.Ignore)]
        public long? ContactPhone { get; set; }

        /// <summary>
        /// Тип конверта по умолчанию (Опционально)
        /// </summary>
        [JsonProperty("def-envelope-type", NullValueHandling = NullValueHandling.Ignore)]
        public EnEnvelopeType DefEnvelopeType { get; set; }

        /// <summary>
        /// Способ оплаты по умолчанию (Опционально)
        /// </summary>
        [JsonProperty("def-payment-method", NullValueHandling = NullValueHandling.Ignore)]
        public EnPaymentMethod DefPaymentMethod { get; set; }

        /// <summary>
        /// ЕСПП (Единоая система почтовых переводов) код (Опционально)
        /// </summary>
        [JsonProperty("espp-code", NullValueHandling = NullValueHandling.Ignore)]
        public string EsppCode { get; set; }

        /// <summary>
        /// Внутренний идентификатор
        /// </summary>
        [JsonProperty("hid")]
        public string Hid { get; set; }

        /// <summary>
        /// Идентификатор компании (ЮЛ) (Опционально)
        /// </summary>
        [JsonProperty("legal-hid", NullValueHandling = NullValueHandling.Ignore)]
        public string LegalHid { get; set; }

        /// <summary>
        /// Разряд почтового отправления (Опционально)
        /// </summary>
        [JsonProperty("mail-rank", NullValueHandling = NullValueHandling.Ignore)]
        public EnMailRank MailRank { get; set; }

        /// <summary>
        /// Опции отправлений
        /// </summary>
        [JsonProperty("mailing-option")]
        public List<MailingOption> MailingOption { get; set; }

        /// <summary>
        /// ИНН организации (Опционально)
        /// </summary>
        [JsonProperty("org-inn", NullValueHandling = NullValueHandling.Ignore)]
        public string OrgInn { get; set; }

        /// <summary>
        /// КПП организации (Опционально)
        /// </summary>
        [JsonProperty("org-kpp", NullValueHandling = NullValueHandling.Ignore)]
        public string OrgKpp { get; set; }

        /// <summary>
        /// Наименование организации (Опционально)
        /// </summary>
        [JsonProperty("org-name", NullValueHandling = NullValueHandling.Ignore)]
        public string OrgName { get; set; }

        /// <summary>
        /// Планируемое количество отправлений в месяц (Опционально)
        /// </summary>
        [JsonProperty("planned-monthly-number", NullValueHandling = NullValueHandling.Ignore)]
        public long? PlannedMonthlyNumber { get; set; }

        /// <summary>
        /// Тип печати (Опционально)
        /// </summary>
        [JsonProperty("print-type", NullValueHandling = NullValueHandling.Ignore)]
        public EnPrintType PrintType { get; set; }

        /// <summary>
        /// Регионы обслуживания
        /// </summary>
        [JsonProperty("regions")]
        public List<string> Regions { get; set; }

        /// <summary>
        /// Наименования сабэкаунта как отправителя (Опционально)
        /// </summary>
        [JsonProperty("sender-name", NullValueHandling = NullValueHandling.Ignore)]
        public string SenderName { get; set; }

        /// <summary>
        /// Доступные точки сдачи (Опционально)
        /// </summary>
        [JsonProperty("shipping-points", NullValueHandling = NullValueHandling.Ignore)]
        public List<ShippingPoint> ShippingPoints { get; set; }
    }

    public class Account
    {
        /// <summary>
        /// Адрес возврата (Опционально)
        /// </summary>
        [JsonProperty("address", NullValueHandling = NullValueHandling.Ignore)]
        public string Address { get; set; }

        /// <summary>
        /// Признак блокировки
        /// </summary>
        [JsonProperty("blocked")]
        public bool Blocked { get; set; }

        /// <summary>
        /// Контактный email (Опционально)
        /// </summary>
        [JsonProperty("email", NullValueHandling = NullValueHandling.Ignore)]
        public string Email { get; set; }

        /// <summary>
        /// Признак администратора (Опционально)
        /// </summary>
        [JsonProperty("is-admin", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsAdmin { get; set; }

        /// <summary>
        /// Идентификатор компании (ЮЛ) (Опционально)
        /// </summary>
        [JsonProperty("legal-hid", NullValueHandling = NullValueHandling.Ignore)]
        public string LegalHid { get; set; }

        /// <summary>
        /// ИНН организации (Опционально)
        /// </summary>
        [JsonProperty("org-inn", NullValueHandling = NullValueHandling.Ignore)]
        public string OrgInn { get; set; }

        /// <summary>
        /// Наименование организации (Опционально)
        /// </summary>
        [JsonProperty("org-name", NullValueHandling = NullValueHandling.Ignore)]
        public string OrgName { get; set; }
    }

    public class ShippingPoint
    {
        /// <summary>
        /// ДТИ (дополнительный технический индекс). (Опционально)
        /// </summary>
        [Obsolete("Supported for back compatibility only. Use 'AdditionalOperatorPostcodes' instead.")]
        [JsonProperty("additional-operator-postcode", NullValueHandling = NullValueHandling.Ignore)]
        public string AdditionalOperatorPostcode { get; set; }

        /// <summary>
        /// Список ДТИ (дополнительный технический индекс) (Опционально)
        /// </summary>
        [JsonProperty("additional-operator-postcodes", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> AdditionalOperatorPostcodes { get; set; }

        /// <summary>
        /// Адрес возврата (Опционально)
        /// </summary>
        [JsonProperty("address", NullValueHandling = NullValueHandling.Ignore)]
        public Address Address { get; set; }

        /// <summary>
        /// Список доступных ДТИ (дополнительный технический индекс) (Опционально)
        /// </summary>
        [JsonProperty("available-additional-operator-postcodes", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> AvailableAdditionalOperatorPostcodes { get; set; }

        /// <summary>
        /// Разрешенные почтовые типы отправлений (Опционально)
        /// </summary>
        [JsonProperty("available-mail-types", NullValueHandling = NullValueHandling.Ignore)]
        public List<EnMailType> AvailableMailTypes { get; set; }

        /// <summary>
        /// Список доступных продуктов (Опционально)
        /// </summary>
        [JsonProperty("available-products", NullValueHandling = NullValueHandling.Ignore)]
        public List<AvailableProduct> AvailableProducts { get; set; }

        /// <summary>
        /// В документации не описано
        /// </summary>
        [JsonProperty("courier-call", NullValueHandling = NullValueHandling.Ignore)]
        public bool? CourierCall { get; set; }

        /// <summary>
        /// Признак активации (Опционально)
        /// </summary>
        [JsonProperty("enabled", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Enabled { get; set; }

        /// <summary>
        /// Индекс почтового отделения обслуживания
        /// </summary>
        [JsonProperty("operator-postcode")]
        public string OperatorIndex { get; set; }

        /// <summary>
        /// Адрес отделения почтовой связи (Опционально)
        /// </summary>
        [JsonProperty("ops-address", NullValueHandling = NullValueHandling.Ignore)]
        public string OpsAddress { get; set; }

        /// <summary>
        /// Номер абонентского ящика (Опционально)
        /// </summary>
        [JsonProperty("po-box", NullValueHandling = NullValueHandling.Ignore)]
        public string PoBox { get; set; }

        /// <summary>
        /// Тип адреса возврата (Опционально)
        /// </summary>
        [JsonProperty("return-address-type", NullValueHandling = NullValueHandling.Ignore)]
        public EnReturnAddressType ReturnAddressType { get; set; }
    }

    public class AvailableProduct
    {
        /// <summary>
        /// Категория почтового отправления
        /// </summary>
        [JsonProperty("mail-category")]
        public EnMailCategory MailCategory { get; set; }

        /// <summary>
        /// Тип почтового отправления
        /// </summary>
        [JsonProperty("mail-type")]
        public EnMailType MailType { get; set; }

        /// <summary>
        /// Тип продукта
        /// </summary>
        [JsonProperty("product-type")]
        public string ProductType { get; set; }

        /// <summary>
        /// Признак доступности услуги SMS уведомления получателя
        /// </summary>
        [JsonProperty("sms-notice-recipient-enabled")]
        public bool SmsNoticeRecipientEnabled { get; set; }
    }

    public class MailingOption
    {
        /// <summary>
        /// Свойство
        /// </summary>
        [JsonProperty("key")]
        public string Key { get; set; }

        /// <summary>
        /// Значение свойства
        /// </summary>
        [JsonProperty("value")]
        public List<MailingOptionValue> Value { get; set; }
    }

    public class MailingOptionValue
    {
        /// <summary>
        /// Свойство
        /// </summary>
        [JsonProperty("key")]
        public string Key { get; set; }

        /// <summary>
        /// Значение свойства
        /// </summary>
        [JsonProperty("value")]
        public string Value { get; set; }
    }

    //[JsonConverter(typeof(StringEnumConverter))]
    //public enum EnReturnAddressType
    public class EnReturnAddressType : StringEnum<EnReturnAddressType>
    {
        public EnReturnAddressType(string value) : base(value)
        {
        }

        /// <summary>
        /// По другому адресу
        /// </summary>
        public static EnReturnAddressType SenderAddress { get { return new EnReturnAddressType("SENDER_ADDRESS"); } }

        /// <summary>
        /// В ОПС обслуживания
        /// </summary>
        public static EnReturnAddressType PostofficeAddress { get { return new EnReturnAddressType("POSTOFFICE_ADDRESS"); } }
    }

    #endregion

    #region DeliveryPoint

    public class DeliveryPoint
    {
        /// <summary>
        /// Адрес ПВЗ
        /// </summary>
        [JsonProperty("address")]
        public Address Address { get; set; }

        //[JsonProperty("brand-id")]
        //public string BrandId { get; set; }

        /// <summary>
        /// Торговое наименование ПВЗ
        /// </summary>
        [JsonProperty("brand-name")]
        public string BrandName { get; set; }

        /// <summary>
        /// Возможность оплаты картой при получении
        /// </summary>
        [JsonProperty("card-payment")]
        public bool CardPayment { get; set; }

        /// <summary>
        /// Возможность оплаты наличными при получении
        /// </summary>
        [JsonProperty("cash-payment")]
        public bool CashPayment { get; set; }

        /// <summary>
        /// Признак вывода из эксплуатации объекта ПС
        /// </summary>
        [JsonProperty("closed")]
        public bool Closed { get; set; }

        /// <summary>
        /// Признак временного не функционирования объекта ПС
        /// </summary>
        [JsonProperty("temporary-closed")]
        public bool TemporaryClosed { get; set; }

        /// <summary>
        /// Опция Проверка вложений
        /// </summary>
        [JsonProperty("contents-checking")]
        public bool ContentsChecking { get; set; }

        /// <summary>
        /// Индекс ПВЗ
        /// </summary>
        [JsonProperty("delivery-point-index")]
        public string DeliveryPointIndex { get; set; }

        /// <summary>
        /// Тип пункта выдачи
        /// </summary>
        [JsonProperty("delivery-point-type")]
        public EnDeliveryPointType DeliveryPointType { get; set; }

        /// <summary>
        /// Опция Проверка работоспособности
        /// </summary>
        [JsonProperty("functionality-checking")]
        public bool FunctionalityChecking { get; set; }

        /// <summary>
        /// Подробное описание как добраться до ПВЗ
        /// </summary>
        [JsonProperty("getto")]
        public string GettoDescription { get; set; }

        /// <summary>
        /// Идентификатор объекта почтовой связи
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Широта
        /// </summary>
        [JsonProperty("latitude")]
        public float Latitude { get; set; }

        /// <summary>
        /// Долгота
        /// </summary>
        [JsonProperty("longitude")]
        public float Longitude { get; set; }

        ///// <summary>
        ///// Юридическое наименование ПВЗ (полное)
        ///// </summary>
        //[JsonProperty("legal-name")]
        //public string LegalName { get; set; }

        /// <summary>
        /// Юридическое наименование ПВЗ (сокращённое)
        /// </summary>
        [JsonProperty("legal-short-name")]
        public string LegalShortName { get; set; }

        //[JsonProperty("partial-redemption")]
        //public bool PartialRedemption { get; set; }

        [JsonProperty("type-post-office")]
        public EnTypePostOffice TypePostOffice { get; set; }

        /// <summary>
        /// примерка
        /// </summary>
        [JsonProperty("with-fitting")]
        public bool WithFitting { get; set; }

        /// <summary>
        /// Ограничение по весу в данном ПВЗ
        /// </summary>
        [JsonProperty("weight-limit")]
        [JsonConverter(typeof(FloatToIntegerConverter))]
        public int? WeightLimit { get; set; }

        [JsonProperty("dimension-limit")]
        public EnDimensionType DimensionLimit { get; set; }
        /// <summary>
        /// Время работы ПВЗ
        /// </summary>
        [JsonProperty("work-time")]
        [JsonConverter(typeof(ArrayStringToSingleLineConverter), "\n")]//для оптимизации памяти в кэше и чтобы не заниматься склейкой каждый раз при выводе
        public string WorkTime { get; set; }
    }

    public class EnDeliveryPointType : StringEnum<EnDeliveryPointType>
    {
        public EnDeliveryPointType(string value) : base(value)
        {
        }
        /// <summary>
        /// ПВЗ
        /// </summary>
        public static EnDeliveryPointType DeliveryPoint { get { return new EnDeliveryPointType("DELIVERY_POINT"); } }

        /// <summary>
        /// Почтомат
        /// </summary>
        public static EnDeliveryPointType PickupPoint { get { return new EnDeliveryPointType("PICKUP_POINT"); } }
    }

    public class EnTypePostOffice : StringEnum<EnTypePostOffice>
    {
        public EnTypePostOffice(string value) : base(value)
        {
        }

        /// <summary>
        /// ПВЗ
        /// </summary>
        public static EnTypePostOffice PVZ { get { return new EnTypePostOffice("PVZ"); } }

        /// <summary>
        /// Почтомат
        /// </summary>
        public static EnTypePostOffice POST_OFFICE { get { return new EnTypePostOffice("POST_OFFICE"); } }

        /// <summary>
        /// Почтовое отделение
        /// </summary>
        public static EnTypePostOffice GOPS { get { return new EnTypePostOffice("GOPS"); } }
    }

    #endregion

    #region Office

    public class Office
    {
        public Address Address { get; set; }
        // public AddressFias AddressFias { get; set; }
        public string BrandName { get; set; }
        // [JsonConverter(typeof(BoolConverter))]
        // public bool Ecom { get; set; }
        public EcomOptions EcomOptions { get; set; }
        // public List<Holiday> Holidays { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        // [JsonConverter(typeof(BoolConverter))]
        // public bool OnlineParcel { get; set;}
        public string Type { get; set; }
        public List<string> WorkTime { get; set; }
    }

    public class AddressFias
    {
        public Guid? AddGarCode { get; set; }
        public Guid? LocationGarCode { get; set; }
        public Guid? RegGarId { get; set; }
    }

    public class EcomOptions
    {
        public bool CardPayment { get; set; }
        public bool CashPayment { get; set; }
        public bool ContentsChecking { get; set; }
        public bool FunctionalityChecking { get; set; }
        public bool PartialRedemption { get; set; }
        public bool ReturnAvailable { get; set; }
        public int TypesizeId { get; set; }
        public string TypesizeVal { get; set; }
        public bool WithFitting { get; set; }
        public string Getto { get; set; }
        public float WeightLimit { get; set; }
    }

    public class Holiday
    {
        public DateTimeOffset? Df { get; set; }
        public DateTimeOffset? Ds { get; set; }
        public List<Work> Work { get; set; }
    }

    public class Work
    {
        public DateTimeOffset? Dt { get; set; }
        public long? Nm { get; set; }
        public string Fn { get; set; }
        public List<Rst> Rst { get; set; }
        public string St { get; set; }
    }
    
    public class Rst
    {
        public string Fn { get; set; }
        public string St { get; set; }
    }
    
    #endregion
}
