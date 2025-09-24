using AdvantShop.Core.Common;
using AdvantShop.Core.Common.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AdvantShop.Shipping.Pec.Api
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
        public List<ValidateField> Fields { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public long? Status { get; set; }
    }

    public class ValidateField
    {
        public string Key { get; set; }
        public List<string> Value { get; set; }
    }

    #endregion

    #region FindBranchesById

    public class BranchesFindByIdParams
    {
        public long Id { get; set; }
    }

    public class BranchesFindByIdResponse : BranchesFindByTitleResponse
    {

    }

    #endregion 

    #region FindBranchesByTitle

    public class BranchesFindByTitleParams
    {
        public string Title { get; set; }
        public bool? Exact { get; set; }
    }

    public class BranchesFindByTitleResponse
    {
        /// <summary>
        /// если найден один или несколько городов или филиалов
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Результаты поиска
        /// </summary>
        public List<BranchesFindByTitleItem> Items { get; set; }
    }

    public class BranchesFindByTitleItem
    {
        /// <summary>
        /// Название филиала
        /// </summary>
        public string BranchTitle { get; set; }

        /// <summary>
        /// Код филиала филиала
        /// </summary>
        public long BranchId { get; set; }

        /// <summary>
        /// Название города
        /// </summary>
        public string CityTitle { get; set; }

        /// <summary>
        /// Код города
        /// </summary>
        public long? CityId { get; set; }
    }

    #endregion

    #region Calculate

    public class CalculateParams
    {
        /// <summary>
        /// Код города отправителя
        /// </summary>
        public long SenderCityId { get; set; }

        /// <summary>
        /// Код города получателя
        /// </summary>
        public long ReceiverCityId { get; set; }

        ///// <summary>
        ///// Растентовка отправителя
        ///// </summary>
        //public bool IsOpenCarSender { get; set; }

        ///// <summary>
        ///// Тип доп. услуг отправителя
        ///// </summary>
        //public byte SenderDistanceType { get; set; }

        ///// <summary>
        ///// Необходим забор день в день
        ///// </summary>
        //public bool IsDayByDay { get; set; }

        ///// <summary>
        ///// Растентовка получателя
        ///// </summary>
        //public bool IsOpenCarReceiver { get; set; }

        ///// <summary>
        ///// Тип доп. услуг отправителя
        ///// </summary>
        //public long ReceiverDistanceType { get; set; }

        ///// <summary>
        ///// Признак гипермаркета
        ///// </summary>
        //public bool IsHyperMarket { get; set; }

        /// <summary>
        /// Расчетная дата
        /// </summary>
        [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd")]
        public DateTime CalcDate { get; set; }

        /// <summary>
        /// Страхование
        /// </summary>
        public bool IsInsurance { get; set; }

        /// <summary>
        /// Оценочная стоимость, руб
        /// </summary>
        public double? IsInsurancePrice { get; set; }

        /// <summary>
        /// Нужен забор
        /// </summary>
        public bool IsPickUp { get; set; }

        /// <summary>
        /// Нужна доставка
        /// </summary>
        public bool IsDelivery { get; set; }

        ///// <summary>
        ///// Погрузочно-разгрузочные работы при заборе
        ///// </summary>
        //public PickupDeliveryServices PickupServices { get; set; }

        ///// <summary>
        ///// Погрузочно-разгрузочные работы при доставке
        ///// </summary>
        //public PickupDeliveryServices DeliveryServices { get; set; }

        /// <summary>
        /// Для расчёта забора груза
        /// </summary>
        public CalculateDelivery Pickup { get; set; }

        /// <summary>
        /// Для расчёта доставки груза
        /// </summary>
        public CalculateDelivery Delivery { get; set; }

        /// <summary>
        /// Данные о грузах
        /// </summary>
        public List<CalculateCargo> Cargos { get; set; }

        /// <summary>
        /// Массив продуктов/тарифов для расчета [Array][Number]. Обязательно. В настоящее время допускается передача ОДНОГО Продукта/тарифа на расчет.
        /// </summary>
        public EnTransportingType[] TransportingTypes { get; set; }
    }

    public class CalculateCargo
    {
        /// <summary>
        /// Длина груза, м
        /// </summary>
        public double Length { get; set; }

        /// <summary>
        /// Ширина груза, м
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// Высота груза, м
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// Объем груза, м3
        /// </summary>
        public double Volume { get { return Math.Round(Length * Width * Height, 3); } }

        /// <summary>
        /// Максимальный габарит, м
        /// </summary>
        public double MaxSize { get { return Math.Max(Math.Max(Length, Width), Height); } }

        /// <summary>
        /// Защитная транспортировочная упаковка
        /// </summary>
        public bool IsHp { get; set; }

        ///// <summary>
        ///// Количество мест для пломбировки
        ///// </summary>
        //public long SealingPositionsCount { get; set; }

        /// <summary>
        /// Вес, кг
        /// </summary>
        public double Weight { get; set; }

        ///// <summary>
        ///// Негабаритный груз
        ///// </summary>
        //public bool OverSize { get; set; }
    }

    public class CalculateDelivery
    {
        /// <summary>
        /// Обязательный параметр для расчёта забора\доставки груза
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Необязательный параметр координат адреса (используется для точности вычислений)
        /// </summary>
        public Coordinates Coordinates { get; set; }
    }

    public class Coordinates
    {
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }

    public class PickupDeliveryServices
    {
        /// <summary>
        /// Расчитывать погрузочно-разгрузочные работы
        /// </summary>
        public bool IsLoading { get; set; }

        /// <summary>
        /// Поднять/спустить на этаж
        /// </summary>
        public long Floor { get; set; }

        /// <summary>
        /// Перенести груз (в метрах)
        /// </summary>
        public long CarryingDistance { get; set; }

        /// <summary>
        /// Подъем на лифте
        /// </summary>
        public bool IsElevator { get; set; }
    }

    public class CalculateResponse
    {
        /// <summary>
        /// Признак ошибок при расчетах
        /// </summary>
        public bool HasError { get; set; }

        /// <summary>
        /// Текст ошибки
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Результаты расчетов по видам перевозок
        /// </summary>
        public List<Transfer> Transfers { get; set; }

        /// <summary>
        /// Массив данных по времени приемки груза
        /// </summary>
        public List<DateTime> TransportingTimes { get; set; }

        /// <summary>
        /// Общие данных по срокам перевозок
        /// </summary>
        public List<CommonTerm> CommonTerms { get; set; }
    }

    public class CommonTerm
    {
        /// <summary>
        /// Вид перевозки
        /// </summary>
        public EnTransportingType TransportingType { get; set; }

        /// <summary>
        /// Филиал-отправитель
        /// </summary>
        public string BranchSender { get; set; }

        /// <summary>
        /// Филиал-получатель
        /// </summary>
        public string BranchReceiver { get; set; }

        /// <summary>
        /// Ориентировочные сроки перевозки, суток
        /// <para>из массива надо выбрать элемент в соответствии со временем сдачи груза на склад и Массивом данных по времени приемки груза transportingTimes</para>
        /// </summary>
        public List<string> Transporting { get; set; }

        /// <summary>
        /// Ориентировочные сроки перевозки с доставкой, суток
        /// </summary>
        public List<string> TransportingWithDelivery { get; set; }

        /// <summary>
        /// Срок перевозки с забором, суток
        /// </summary>
        public string TransportingWithPickup { get; set; }

        /// <summary>
        /// Срок перевозки с забором и доставкой, суток
        /// </summary>
        public string TransportingWithDeliveryWithPickup { get; set; }
    }

    public class Transfer
    {
        /// <summary>
        /// Вид перевозки
        /// </summary>
        public EnTransportingType TransportingType { get; set; }

        /// <summary>
        /// Признак ошибок при расчетах
        /// </summary>
        public bool HasError { get; set; }

        /// <summary>
        /// Текст ошибки
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Общая сумма по виду перевозки, руб.
        /// </summary>
        public long CostTotal { get; set; }

        ///// <summary>
        ///// Данные по услугам за вид перевозки
        ///// </summary>
        //public List<TransferService> Services { get; set; }
    }

    public class TransferService
    {
        /// <summary>
        /// Услуга
        /// </summary>
        public string ServiceType { get; set; }

        /// <summary>
        /// Город отправитель
        /// </summary>
        public string SenderCity { get; set; }

        /// <summary>
        /// Сумма за услугу, руб.
        /// </summary>
        public double Cost { get; set; }

        /// <summary>
        /// Расшифровка услуги
        /// </summary>
        public string Info { get; set; }

        /// <summary>
        /// Вложенные услуги
        /// </summary>
        public List<TransferService> Services { get; set; }
    }

    public class EnTransportingType : IntegerEnum<EnTransportingType>
    {
        public EnTransportingType(int value) : base(value)
        {
        }

        /// <summary>
        /// Автоперевозка
        /// </summary>
        [Localize("Автоперевозка")]
        public static EnTransportingType Car { get { return new EnTransportingType(1); } }

        /// <summary>
        /// Авиаперевозка
        /// </summary>
        [Localize("Авиаперевозка")]
        public static EnTransportingType Avia { get { return new EnTransportingType(2); } }

        /// <summary>
        /// Easyway
        /// </summary>
        [Localize("Easyway")]
        public static EnTransportingType Easyway { get { return new EnTransportingType(12); } }
    }

    #endregion

    #region AllBranches

    public class AllBranchesResponse
    {
        /// <summary>
        /// Список филиалов
        /// </summary>
        public List<Branch> Branches { get; set; }
    }

    /// <summary>
    /// Информация о филиале
    /// </summary>
    public class Branch
    {
        public string Id { get; set; }

        ///// <summary>
        ///// Название филиала
        ///// </summary>
        //public string Title { get; set; }

        /// <summary>
        /// Адрес филиала
        /// </summary>
        public string Address { get; set; }
        //public long? PostalCode { get; set; }

        ///// <summary>
        ///// Координаты GPS
        ///// </summary>
        //public string Coordinates { get; set; }

        /// <summary>
        /// Код филиала
        /// </summary>
        public long BitrixId { get; set; }

        //public string BranchCode { get; set; }
        //public string Timezone { get; set; }

        /// <summary>
        /// Список городов филиала
        /// </summary>
        public List<BranchCity> Cities { get; set; }

        /// <summary>
        /// Список отделений
        /// </summary>
        public List<Division> Divisions { get; set; }
    }

    public class BranchCity
    {
        /// <summary>
        /// Название города
        /// </summary>
        public string Title { get; set; }

        //public string PostalCode { get; set; }
        //public string CladrCode { get; set; }

        /// <summary>
        /// Код города
        /// </summary>
        public long? BitrixId { get; set; }

        ///// <summary>
        ///// идентификатор города
        ///// </summary>
        //public string CityId { get; set; }
        //public bool? IsKz { get; set; }

        ///// <summary>
        ///// тип населенного пункта
        ///// </summary>
        //public EnTypeCity CityStatus { get; set; }

        public List<string> Divisions { get; set; }
    }

    public class EnTypeCity : IntegerEnum<EnTypeCity>
    {
        public EnTypeCity(int value) : base(value)
        {
        }

        /// <summary>
        /// километраж
        /// </summary>
        public static EnTypeCity KilometricArea { get { return new EnTypeCity(0); } }

        /// <summary>
        /// филиал
        /// </summary>
        public static EnTypeCity Branch { get { return new EnTypeCity(1); } }

        /// <summary>
        /// отделение
        /// </summary>
        public static EnTypeCity Department { get { return new EnTypeCity(2); } }

        /// <summary>
        /// сателлит
        /// </summary>
        public static EnTypeCity Satellite { get { return new EnTypeCity(3); } }

    }

    public class Division
    {
        /// <summary>
        /// идентификатор отделения
        /// </summary>
        public string Id { get; set; }

        ///// <summary>
        ///// идентификатор города, которому принадлежит отделение
        ///// </summary>
        //public string CityId { get; set; }

        /// <summary>
        /// Название отделения
        /// </summary>
        public string Name { get; set; }
        //public long DivisionPriority { get; set; }
        //public bool IsAcceptanceOnly { get; set; }
        //public bool IsPartialDistributionAllowed { get; set; }
        //public bool IsKz { get; set; }

        /// <summary>
        /// Список складов
        /// </summary>
        public List<Warehouse> Warehouses { get; set; }
    }

    /// <summary>
    /// Информация о склад
    /// </summary>
    public class Warehouse
    {
        /// <summary>
        /// Идентификатор склада
        /// </summary>
        public string Id { get; set; }

        ///// <summary>
        ///// Идентификатор отделения
        ///// </summary>
        //public string DivisionId { get; set; }

        /// <summary>
        /// Название склада
        /// </summary>
        public string Name { get; set; }

        ///// <summary>
        ///// Название отделения
        ///// </summary>
        //public string DivisionName { get; set; }

        /// <summary>
        /// Адрес склада
        /// </summary>
        public string Address { get; set; }

        ///// <summary>
        ///// Адрес отделения
        ///// </summary>
        //public string AddressDivision { get; set; }

        /// <summary>
        /// Истина, если осуществляется только ПРИЁМ груза
        /// </summary>
        public bool IsAcceptanceOnly { get; set; }

        /// <summary>
        /// Истина, если склад выдает грузы
        /// </summary>
        public bool IsWarehouseGivesFreights { get; set; }

        /// <summary>
        /// Истин, если склад принимает грузы
        /// </summary>
        public bool IsWarehouseAcceptsFreights { get; set; }

        /// <summary>
        /// Истина, если за перевозку возможно взимание дополнительной платы
        /// </summary>
        public bool IsFreightSurcharge { get; set; }

        /// <summary>
        /// Координаты GPS
        /// </summary>
        public string Coordinates { get; set; }

        public string Email { get; set; }
        public string Telephone { get; set; }

        /// <summary>
        /// Имеются ограничения на параметры перевозимого груза
        /// </summary>
        public bool IsRestrictions { get; set; }

        public long PlacesCount { get; set; }

        /// <summary>
        /// Максимально допустимый вес
        /// </summary>
        public double MaxWeight { get; set; }

        /// <summary>
        /// Максимально допустимый объем
        /// </summary>
        public double MaxVolume { get; set; }

        /// <summary>
        /// Максимально допустимый вес одного грузоместа
        /// </summary>
        public double MaxWeightPerPlace { get; set; }

        /// <summary>
        /// Максимальный габарит груза
        /// </summary>
        public double MaxDimension { get; set; }

        /// <summary>
        /// Время работы склада
        /// </summary>
        public List<TimeOfWork> TimeOfWork { get; set; }

        /// <summary>
        /// Время работы отделения
        /// </summary>
        public List<TimeOfWork> DivisionTimeOfWork { get; set; }
    }

    public class TimeOfWork
    {
        /// <summary>
        /// Начало рабочего дня
        /// </summary>
        public string WorkFrom { get; set; }

        /// <summary>
        /// Конец рабочего дня
        /// </summary>
        public string WorkTo { get; set; }

        /// <summary>
        /// Начало обеденного перерыва
        /// </summary>
        public string DinnerFrom { get; set; }

        /// <summary>
        /// Конец обеденного перерыва
        /// </summary>
        public string DinnerTo { get; set; }

        /// <summary>
        /// День недели (1 - понедельник, 7 - воскресенье)
        /// </summary>
        public byte DayOfWeek { get; set; }
    }
    #endregion

    #region PreregistrationSubmit

    public class PreregistrationSubmitParams
    {
        ///// <summary>
        ///// Общие данные
        ///// </summary>
        //public PreregistrationSubmitParamsCommon Common { get; set; }

        /// <summary>
        /// Данные об отправителе
        /// </summary>
        public Sender Sender { get; set; }

        /// <summary>
        /// Данные о грузах
        /// </summary>
        public List<PreregistrationSubmitCargo> Cargos { get; set; }
    }

    public class PreregistrationSubmitCargo
    {
        /// <summary>
        /// бщие данные о грузе
        /// </summary>
        public CargoCommon Common { get; set; }

        /// <summary>
        /// Получатель 
        /// </summary>
        public Receiver Receiver { get; set; }

        /// <summary>
        /// Код получателя из справочника
        /// </summary>
        public string ReceiverCode { get; set; }

        /// <summary>
        /// Услуги
        /// </summary>
        public Services Services { get; set; }

        ///// <summary>
        ///// Биллинг, необязательное поле
        ///// </summary>
        //public Billing Billing { get; set; }
    }

    public class Billing
    {
        public List<BillingNumber> BillingNumbers { get; set; }
    }

    public class BillingNumber
    {
        public string BillingNo { get; set; }
        public long BillingSum { get; set; }
    }

    public class CargoCommon
    {
        ///// <summary>
        ///// Произвольное значение для синхронизации на стороне клиента, поле необязательно
        ///// </summary>
        //public string CustomerCorrelation { get; set; }

        /// <summary>
        /// Тип перевозки
        /// </summary>
        public EnTransportingTypeCargo Type { get; set; }

        /// <summary>
        /// Общий вес груза, кг, в данный момент поле необязательно
        /// </summary>
        public double Weight { get; set; }

        /// <summary>
        /// Общий объём груза, м3, поле необязательно
        /// </summary>
        public double Volume { get { return Math.Round(Length * Width * Height, 3); } }

        /// <summary>
        /// Ширина, м, поле необязательно
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// Длина, м, поле необязательно
        /// </summary>
        public double Length { get; set; }

        /// <summary>
        /// Высота, м [Number], поле необязательно
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// Объявленная стоимость товара
        /// </summary>
        public double DeclaredCost { get; set; }

        /// <summary>
        /// Есть комплект сопроводительных документов
        /// </summary>
        public bool AccompanyingDocuments { get; set; }

        ///// <summary>
        ///// Номера сопроводительных документов
        ///// </summary>
        //public string AccompanyingDocumentsNumbers { get; set; }

        /// <summary>
        /// Сумма сопроводительных документов поле обязательно, если accompanyingDocuments = true 
        /// </summary>
        public double? AccompanyingDocumentsSum { get; set; }

        /// <summary>
        /// Количество мест
        /// </summary>
        public byte PositionsCount { get; set; }

        ///// <summary>
        ///// Описание груза
        ///// </summary>
        //public string Description { get; set; }

        /// <summary>
        /// Номер заказа клиента, поле необязательно
        /// </summary>
        public string OrderNumber { get; set; }

        /// <summary>
        /// Форма оплаты (1 - Банк, 2 - Касса) [Number], поле необязательно, если значение не указано, равно "Банк" по умолчанию
        /// </summary>
        public EnPaymentForm PaymentForm { get; set; }

        ///// <summary>
        ///// Тип штрих-кодов, указанных для мест грузов заявки. Тип штрих-кода можно набирать символами любого регистра
        ///// </summary>
        //public string TypeClientBarcode { get; set; }

        ///// <summary>
        ///// приоритет
        ///// </summary>
        //public byte ClientPriority { get; set; }

        ///// <summary>
        ///// Штрих-коды мест груза
        ///// </summary>
        //public List<string> ClientPositionsBarcode { get; set; }

        public string CargoSourceSystemGUID { get { return "5ff31c56-2c7f-11eb-80ce-00155d4a0436"; } }
        public string ReferralID { get { return "Advantshop"; } }
    }

    public class EnPaymentForm : IntegerEnum<EnPaymentForm>
    {
        public EnPaymentForm(int value) : base(value)
        {
        }

        /// <summary>
        /// Банк
        /// </summary>
        [Localize("Банк")]
        public static EnPaymentForm Bank { get { return new EnPaymentForm(1); } }

        /// <summary>
        /// Касса
        /// </summary>
        [Localize("Касса")]
        public static EnPaymentForm Kassa { get { return new EnPaymentForm(2); } }
    }

    public class EnTransportingTypeCargo : IntegerEnum<EnTransportingTypeCargo>
    {
        public EnTransportingTypeCargo(int value) : base(value)
        {
        }

        /// <summary>
        /// Автоперевозка
        /// </summary>
        [Localize("Автоперевозка")]
        public static EnTransportingTypeCargo Car { get { return new EnTransportingTypeCargo(3); } }

        /// <summary>
        /// Авиаперевозка
        /// </summary>
        [Localize("Авиаперевозка")]
        public static EnTransportingTypeCargo Avia { get { return new EnTransportingTypeCargo(1); } }

        /// <summary>
        /// Easyway
        /// </summary>
        [Localize("Easyway")]
        public static EnTransportingTypeCargo Easyway { get { return new EnTransportingTypeCargo(12); } }
    }

    public class Sender
    {
        /// <summary>
        /// ИНН, поле необязательно
        /// </summary>
        public string Inn { get; set; }

        /// <summary>
        /// Город
        /// </summary>
        public string City { get; set; }

        /// <summary>
        ///  Форма собственности, поле необязательно
        /// </summary>
        public string Fs { get; set; }

        /// <summary>
        /// Наименование получателя/отправителя
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Телефон
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// добавочный номер (максимум 10 символов)
        /// </summary>
        public string PhoneAdditional { get; set; }

        /// <summary>
        /// Контактное лицо
        /// </summary>
        public string Person { get; set; }

        /// <summary>
        /// Идентификатор склада
        /// </summary>
        public string WarehouseId { get; set; }

        /// <summary>
        /// Документ удостоверяющий личность (для получателя от 50к руб обязталельно)
        /// </summary>
        public IdentityCard IdentityCard { get; set; }
    }

    public class Receiver: Sender
    {
        /// <summary>
        /// Комментарий
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Адрес склада, поле необязательно
        /// </summary>
        public string AddressStock { get; set; }

    }

    public class IdentityCard
    {
        /// <summary>
        /// тип документа
        /// </summary>
        public EnIdentityCardType Type { get; set; }

        /// <summary>
        /// Серия
        /// </summary>
        public string Series { get; set; }

        /// <summary>
        /// Номер
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// Дата
        /// </summary>
        public DateTime? Date { get; set; }

        /// <summary>
        /// служебное поле для других документов
        /// </summary>
        public string Note { get; set; }
    }

    public class EnIdentityCardType: IntegerEnum<EnIdentityCardType>
    {
        public EnIdentityCardType(int value) : base(value)
        {
        }

        /// <summary>
        /// Без предоставления документа
        /// </summary>
        public static EnIdentityCardType None { get { return new EnIdentityCardType(0); } }

        /// <summary>
        /// паспорт иностранного гражданина
        /// </summary>
        public static EnIdentityCardType Type1 { get { return new EnIdentityCardType(1); } }

        /// <summary>
        /// разрешенние на временное проживание
        /// </summary>
        public static EnIdentityCardType Type2 { get { return new EnIdentityCardType(2); } }

        /// <summary>
        /// водительское удостоверение
        /// </summary>
        public static EnIdentityCardType DriverLicense { get { return new EnIdentityCardType(3); } }

        /// <summary>
        /// вид на жительство
        /// </summary>
        public static EnIdentityCardType Type4 { get { return new EnIdentityCardType(4); } }

        /// <summary>
        /// заграничный паспорт
        /// </summary>
        public static EnIdentityCardType Type5 { get { return new EnIdentityCardType(5); } }

        /// <summary>
        /// удостоверение беженца
        /// </summary>
        public static EnIdentityCardType Type6 { get { return new EnIdentityCardType(6); } }

        /// <summary>
        /// временное удостоверение личности гражданина рф
        /// </summary>
        public static EnIdentityCardType Type7 { get { return new EnIdentityCardType(7); } }

        /// <summary>
        /// свидетельство о предоставлении временного убежища на территории рф
        /// </summary>
        public static EnIdentityCardType Type8 { get { return new EnIdentityCardType(8); } }

        /// <summary>
        /// паспорт моряка
        /// </summary>
        public static EnIdentityCardType Type9 { get { return new EnIdentityCardType(9); } }

        /// <summary>
        /// паспорт гражданина рф
        /// </summary>
        public static EnIdentityCardType PasportRF { get { return new EnIdentityCardType(10); } }

        /// <summary>
        /// свидетельство о рассмотрении ходатайства о признании беженцем
        /// </summary>
        public static EnIdentityCardType Type11 { get { return new EnIdentityCardType(11); } }

        /// <summary>
        /// военный  билет
        /// </summary>
        public static EnIdentityCardType Type12 { get { return new EnIdentityCardType(12); } }
    }

    public class Services
    {
        ///// <summary>
        ///// Email для бухгалтерских уведомлений, поле необязательно
        ///// </summary>
        //public string Email { get; set; }

        /// <summary>
        /// Перевозка
        /// </summary>
        public Transporting Transporting { get; set; }

        /// <summary>
        /// Защитная транспортировочная упаковка
        /// </summary>
        public Ing HardPacking { get; set; }

        /// <summary>
        /// Страхование
        /// </summary>
        public Insurance Insurance { get; set; }

        /// <summary>
        /// Пломбировка
        /// </summary>
        public Sealing Sealing { get; set; }

        /// <summary>
        /// Упаковка стреппинг-лентой
        /// </summary>
        public Strapping Strapping { get; set; }

        /// <summary>
        /// Возврат документов
        /// </summary>
        public Ing DocumentsReturning { get; set; }

        /// <summary>
        /// Доставка
        /// </summary>
        public ServicesDelivery Delivery { get; set; }

        ///// <summary>
        ///// Ответственное хранение, поле необязательно
        ///// </summary>
        //public Ing Storing { get; set; }

        /// <summary>
        /// Наложенный платеж
        /// </summary>
        public CashOnDelivery CashOnDelivery { get; set; }
    }

    public class CashOnDelivery: Ing
    {
        /// <summary>
        /// Общая стоимость заказа (сумма НП)
        /// </summary>
        public double CashOnDeliverySum { get; set; }

        /// <summary>
        /// Фактическая стоимость товара
        /// </summary>
        public double ActualCost { get; set; }

        /// <summary>
        /// Транспортно-экспедиционных услуги в НП
        /// <para>true— за услуги платит отправитель из суммы НП</para>
        /// <para>false— за услуги платит получатель сверх суммы НП</para>
        /// </summary>
        public bool IncludeTes { get; set; }

        ///// <summary>
        ///// Возможна частичная выдача, поле необязательно
        ///// </summary>
        //public bool? IsPartialDistributionAllowed { get; set; }

        ///// <summary>
        ///// Возможно вскрытие и внутритарный осмотр, поле необязательно
        ///// </summary>
        //public bool? IsOpenAndInspectAllowed { get; set; }

        /// <summary>
        /// Номер заказа клиента, поле необязательно
        /// </summary>
        public string OrderNumber { get; set; }

        /// <summary>
        /// ИНН отправителя (продавца) [String], поле необязательно
        /// </summary>
        public string SellerInn { get; set; }

        /// <summary>
        /// Наименование отправителя (продавца) [String], поле необязательно
        /// </summary>
        public string SellerTitle { get; set; }

        /// <summary>
        /// Телефон отправителя, поле необязательно
        /// </summary>
        public string SellerPhone { get; set; }

        ///// <summary>
        ///// Список дополнительных услуг, предоставляемых Грузотправителем
        ///// </summary>
        //public List<SellerService> SellerServices { get; set; }

        ///// <summary>
        ///// Частичная выдача груза
        ///// </summary>
        //public CashOnDeliverySpecification Specification { get; set; }
    }

    public class SellerService
    {
        public EnSellerServiceType Type { get; set; }
        public string RateVat { get; set; }
        public float SumIncludingVat { get; set; }
    }

    public class EnSellerServiceType: IntegerEnum<EnSellerServiceType>
    {
        public EnSellerServiceType(int value) : base(value)
        {
        }

        /// <summary>
        /// Доставка
        /// </summary>
        public static EnSellerServiceType Delivery { get { return new EnSellerServiceType(1); } }

        /// <summary>
        /// Курьерская доставка
        /// </summary>
        public static EnSellerServiceType Courier { get { return new EnSellerServiceType(2); } }

        /// <summary>
        /// Доставка и выдача на терминале
        /// </summary>
        public static EnSellerServiceType DeliveryAndPickUpAtTerminal { get { return new EnSellerServiceType(3); } }

        /// <summary>
        /// Доставка и выдача на ПВЗ
        /// </summary>
        public static EnSellerServiceType DeliveryAndPickUpAtPVZ { get { return new EnSellerServiceType(4); } }

        /// <summary>
        /// Подъем на этаж
        /// </summary>
        public static EnSellerServiceType AscentToFloor { get { return new EnSellerServiceType(5); } }

        /// <summary>
        /// Доставка интернет-магазина
        /// </summary>
        public static EnSellerServiceType DeliveryEShop { get { return new EnSellerServiceType(6); } }

        /// <summary>
        /// Погрузочно-разгрузочные работы интернет магазина
        /// </summary>
        public static EnSellerServiceType LoadingAndUnloadingWorksEShop { get { return new EnSellerServiceType(7); } }
    }

    public class CashOnDeliverySpecification
    {
        /// <summary>
        /// Брать сумму «доставки» при полном отказе получателя
        /// </summary>
        public bool TakeDeliveryZeroSum { get; set; }

        /// <summary>
        /// Обязательная сумма доставки. Обязательно если takeDeliveryZeroSum = true
        /// </summary>
        public double? AmountDeliveryMandatory { get; set; }

        /// <summary>
        /// состав спецификации
        /// </summary>
        public List<SpecificationElement> Specifications { get; set; }
    }

    public class SpecificationElement
    {
        /// <summary>
        /// Артикул 
        /// </summary>
        public string VendorCode { get; set; }

        /// <summary>
        /// Наименование позиции
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Количество
        /// </summary>
        public double Amount { get; set; }

        /// <summary>
        /// Комплект 
        /// </summary>
        public bool Kit { get; set; }

        /// <summary>
        /// Ставка НДС
        /// </summary>
        public string RateVat { get; set; }

        /// <summary>
        /// Объявленная ценность за ед., в т.ч. НДС, руб.
        /// </summary>
        public double ActualCostPerUnit { get; set; }

        /// <summary>
        /// К оплате с Грузополучателя за ед., в т.ч. НДС, руб.
        /// </summary>
        public double SumPerUnit { get; set; }

        /// <summary>
        /// Объявленная ценность всего, руб., в т.ч. НДС
        /// </summary>
        public double ActualCostTotal { get; set; }

        /// <summary>
        /// К оплате с Грузополучателя всего, руб., в т.ч. НДС, руб.
        /// </summary>
        public double SumTotal { get; set; }

        /// <summary>
        /// Примерка 
        /// </summary>
        public bool Fitting { get; set; }

        /// <summary>
        /// Вскрытие инд. упаковки
        /// </summary>
        public bool OpeningIndividualPacking { get; set; }
    }

    public class ServicesDelivery: Ing
    {
        ///// <summary>
        ///// Дата авизации
        ///// </summary>
        //public DateTime? AvisationDateTime { get; set; }

        ///// <summary>
        ///// Плановая дата доставки
        ///// </summary>
        //public DateTime? DateOfDelivery { get; set; }

        /// <summary>
        /// Плательщик
        /// </summary>
        public DeliveryPayer Payer { get; set; }

        ///// <summary>
        ///// еобходима погрузка силами «ПЭК», поле необязательно
        ///// </summary>
        //public bool? IsLoading { get; set; }

        ///// <summary>
        ///// Этаж, поле необязательно
        ///// </summary>
        //public long? Floor { get; set; }

        ///// <summary>
        ///// Есть лифт, поле необязательно
        ///// </summary>
        //public bool? IsElevator { get; set; }

        ///// <summary>
        ///// Метры переноски груза, поле необязательно
        ///// </summary>
        //public int? CarryingDistance { get; set; }
    }

    public class DeliveryPayer
    {
        public EnPaymentType Type { get; set; }

        ///// <summary>
        ///// Город оплаты за услугу, указывается только при type = 3 - третье лицо
        ///// </summary>
        //public string PaymentCity { get; set; }
    }

    public class EnPaymentType: IntegerEnum<EnPaymentType>
    {
        public EnPaymentType(int value) : base(value)
        {
        }

        /// <summary>
        /// отправитель
        /// </summary>
        public static EnPaymentType Seller { get { return new EnPaymentType(1); } }

        /// <summary>
        /// получатель
        /// </summary>
        public static EnPaymentType Receiver { get { return new EnPaymentType(2); } }

        /// <summary>
        /// третье лицо
        /// </summary>
        public static EnPaymentType Other { get { return new EnPaymentType(3); } }
    }

    public class Ing
    {
        /// <summary>
        /// Заказана ли услуга
        /// </summary>
        public bool Enabled { get; set; }
    }

    public class Insurance : Ing
    {
        /// <summary>
        /// Оценочная стоимость
        /// </summary>
        public float? Cost { get; set; }

        /// <summary>
        /// Плательщик
        /// </summary>
        public DeliveryPayer Payer { get; set; }
    }

    public class Sealing: Ing
    {
        public SealingPayer Payer { get; set; }
    }

    public class SealingPayer : DeliveryPayer
    {
        public Other Other { get; set; }
    }

    public class Other
    {
        public string Inn { get; set; }
        public string Title { get; set; }
        public string Phone { get; set; }
        public IdentityCard IdentityCard { get; set; }
    }

    public class Strapping: Ing
    {
        public DeliveryPayer Payer { get; set; }
    }

    public class Transporting
    {
        /// <summary>
        /// Плательщик 
        /// </summary>
        public DeliveryPayer Payer { get; set; }
    }

    public class PreregistrationSubmitParamsCommon
    {
        /// <summary>
        /// Планируемая дата сдачи груза, поле необязательно
        /// </summary>
        public DateTime? PlannedDate { get; set; }
    }

    public class PreregistrationSubmitResponse
    {
        /// <summary>
        /// Номер заявки
        /// </summary>
        public long DocumentId { get; set; }

        /// <summary>
        /// Информация о принятии данных грузов, описанных в заявках
        /// </summary>
        public List<PreregistrationSubmitResponseCargo> Cargos { get; set; }
    }

    public class PreregistrationSubmitResponseCargo
    {
        /// <summary>
        /// Код груза
        /// </summary>
        public string CargoCode { get; set; }

        /// <summary>
        /// Значение для штрих-кода в формате EAN-13
        /// </summary>
        public string Barcode { get; set; }

        /// <summary>
        /// Номер заказа
        /// </summary>
        public string OrderNumber { get; set; }

        /// <summary>
        /// Наименование склада приема груза, поле необязательно
        /// </summary>
        public string StockTitle { get; set; }

        /// <summary>
        /// Произвольное значение для синхронизации на стороне клиента
        /// </summary>
        public string CustomerCorrelation { get; set; }

        /// <summary>
        /// Информация о местах
        /// </summary>
        public List<Position> Positions { get; set; }

        /// <summary>
        /// Штрих-коды мест груза
        /// </summary>
        public List<string> ClientPositionsBarcode { get; set; }
    }

    public class Position
    {
        /// <summary>
        /// Значение для штрих-кода в формате CODE-128/EAN-128
        /// </summary>
        public string Barcode { get; set; }
    }
    #endregion

    #region GetCargosStatuses

    public class GetCargosStatusesParams
    {
        /// <summary>
        /// Коды грузов
        /// </summary>
        public string[] CargoCodes { get; set; }

        /// <summary>
        /// запросить приблизительные координаты груза, необязательное поле
        /// </summary>
        public bool? ReturnPosition { get; set; }
    }

    public class GetCargosStatusesResponse
    {
        /// <summary>
        /// Статусы грузов
        /// </summary>
        public List<CargoStatus> Cargos { get; set; }
    }

    public class CargoStatus
    {
        /// <summary>
        /// Общая информация о грузе
        /// </summary>
        public CargoInfo Info { get; set; }

        /// <summary>
        /// Параметры груза
        /// </summary>
        public CargoStatusCargo Cargo { get; set; }
    }

    public class CargoStatusCargo
    {
        /// <summary>
        /// Код груза
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Предварительный код груза
        /// </summary>
        public string PreliminaryCode { get; set; }
    }

    public class CargoInfo
    {
        /// <summary>
        /// Статус груза
        /// </summary>
        public string CargoStatus { get; set; }

        /// <summary>
        /// id статуса груза
        /// </summary>
        public long? CargoStatusId { get; set; }

        /// <summary>
        /// Дата и время планового прибытия
        /// </summary>
        public DateTime? ArrivalPlanDateTime { get; set; }
    }
    #endregion

    #region GetCargoStatusHistory

    public class GetCargoStatusHistoryParams
    {
        public string CargoCode { get; set; }
    }

    public class GetCargoStatusHistoryItem
    {
        public string Status { get; set; }
        public string Date { get; set; }
    }

    #endregion

    #region GetStatuses

    public class GetStatusesItem
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }

    #endregion

    #region CancellationCargos

    public class CancellationCargosItem
    {
        public string Code { get; set; }
        public bool Success { get; set; }
        public string Description { get; set; }
    }

    #endregion
}
