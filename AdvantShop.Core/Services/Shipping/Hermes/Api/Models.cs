using System;
using System.Collections.Generic;

namespace AdvantShop.Shipping.Hermes.Api
{
    public class ParamsWithSecuredToken
    {
        /// <summary>
        /// Токен клиента (обязательно)
        /// Client Secured Token (necessarily)
        /// </summary>
        public string SecuredToken { get; set; }
    }

    public class ParamsWithPublicToken
    {
        /// <summary>
        /// Токен клиента (обязательно)
        /// Client Secured Token (necessarily)
        /// </summary>
        public string PublicToken { get; set; }
    }

    public class BaseResponse
    {
        /// <summary>
        /// Успешный ли ответ
        /// Is response is success?
        /// </summary>
        public bool? IsSuccess { get; set; }

        /// <summary>
        /// Текст ошибок
        /// Error message
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Код ошибки
        /// Error code. Possible values include: 'Success', 'CommonError',
        /// 'BusinessUnitIsNotAvailable',
        /// 'BusinessUnitHasNotRequiredServices', 'ParcelshopIsNotAvailable',
        /// 'ParcelshopInRedirectState',
        /// 'ParcelPreadviceIsAlreadyInDelivery', 'ParcelPreadviceBarcodeRegexIsNotMatch',
        /// 'BusinessUnitHasNotConfiguredDistributionCenters',
        /// 'DistributionCenterIsNotAvailable',
        /// 'ParcelPreadviceIsNotAvailableToConfirm',
        /// 'ValidationCustomerAddressFailed',
        /// 'ValidationCustomerContactFailed', 'CalculationTermsFailed',
        /// 'PreadviceIsNotInDelivery', 'ParcelBarcodeIsNotFound',
        /// 'ParcelIsReceived', 'CourierHomeDeliveryIsNotAvailableInTheCity',
        /// 'ParcelContentWithoutName'
        /// </summary>
        public string ErrorCode { get; set; }

    }

    public class GetBusinessUnitsParams : ParamsWithSecuredToken { }

    public class GetBusinessUnitsResponse : BaseResponse
    {
        public List<BusinessUnit> BusinessUnits { get; set; }
    }

    public class BusinessUnit
    {
        /// <summary>
        /// Идентификатор бизнес-юнита
        /// Business unit id
        /// </summary>
        public long? BuId { get; set; }

        /// <summary>
        /// Код бизнес-юнита
        /// Business unit code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Краткое наименование
        /// Short name
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// Полное наименование
        /// Full name
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Наименование для уведомлений (sms, email, чаты, мессенджеры, боты)
        /// Notification name (sms, email, messengers, chats, bots)
        /// </summary>
        public string NotificationName { get; set; }

        /// <summary>
        /// Список поддерживаемых услуг
        /// Available services
        /// <para>
        /// Доставка в ПВЗ -
        /// DIRECT_DELIVERY
        /// </para>
        /// <para>
        /// Возврат из ПВЗ -
        /// DIRECT_RETURN
        /// </para>
        /// <para>
        /// Самопривоз клиентом в конечный ПВЗ для выдачи -
        /// DROP_OFF_TO_TARGET_PARCELSHOP
        /// </para>
        /// <para>
        /// Самопривоз клиентом в ПВЗ для дальнейшей доставки (транзит) -
        /// DROP_OFF_TO_TRANSIT_PARCELSHOP
        /// </para>
        /// <para>
        /// Возврат сопроводительных документов -
        /// ACCOMPANYING_DOCUMENTS_RETURN
        /// </para>
        /// <para>
        /// Клиент использует автогенерацию ШК через Hermes при передачи заказа -
        /// CLIENT_USE_AUTOGENERATE_BARCODE_ORDER_BY_HERMES
        /// </para>
        /// <para>
        /// Курьерская доставка получателю -
        /// DOOR_DELIVERY
        /// </para>
        /// </summary>
        public List<string> Services { get; set; }
    }

    public class GetDistributionCentersParams : ParamsWithSecuredToken
    {
        public string UnitClientCode { get; set; }
    }

    public class GetDistributionCentersResponse : BaseResponse
    {
        public List<DCModel> DcModels { get; set; }
    }

    public class DCModel
    {
        /// <summary>
        /// Идентификатор склада
        /// Stroage Id
        /// </summary>
        public long? DcId { get; set; }

        /// <summary>
        /// Код склада
        /// Stroage Code
        /// </summary>
        public string DcCode { get; set; }

        /// <summary>
        /// Наименвоание склада
        /// Storage Name
        /// </summary>
        public string DcName { get; set; }

        /// <summary>
        /// </summary>
        public string CityName { get; set; }

        /// <summary>
        /// </summary>
        public string RegionName { get; set; }

        /// <summary>
        /// </summary>
        public string DcStreet { get; set; }

        /// <summary>
        /// </summary>
        public string DcBuilding { get; set; }

        /// <summary>
        /// </summary>
        public string DcKorpus { get; set; }

        /// <summary>
        /// </summary>
        public string DcOffice { get; set; }

        /// <summary>
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Признак склада по умолчанию
        /// Is Storage Default
        /// </summary>
        public bool? IsDefault { get; set; }
    }

    #region ParcelShop

    public class GetParcelShopsParams : ParamsWithPublicToken
    {
        public string BusinessUnitCode { get; set; }
    }

    public class GetParcelShopsResponse : BaseResponse
    {
        public List<ParcelShop> ParcelShops { get; set; }
    }

    public class ParcelShop
    {
        /// <summary>
        /// Код пункта выдачи
        /// Parcelshop code
        /// </summary>
        public string ParcelShopCode { get; set; }

        /// <summary>
        /// Максимальная масса посылки (в граммах)
        /// </summary>
        public int? MaxParcelWeight { get; set; }

        /// <summary>
        /// Максимальная сумма габаритов посылки (см)
        /// </summary>
        public int? MaxParcelOverallSize { get; set; }

        /// <summary>
        /// Максимальная стоимость посылки (руб.)
        /// </summary>
        public int? MaxParcelValue { get; set; }

        /// <summary>
        /// Наименование пункта выдачи
        /// Parcelshop name
        /// </summary>
        public string ParcelShopName { get; set; }

        /// <summary>
        /// Регион
        /// Region name
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// Город
        /// City
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Индекс
        /// Zipcode
        /// </summary>
        public string ZipCode { get; set; }

        /// <summary>
        /// Адрес
        /// Address
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Долгота
        /// </summary>
        public float? Longitude { get; set; }

        /// <summary>
        /// Широта
        /// </summary>
        public float? Latitude { get; set; }

        /// <summary>
        /// Рабочее время в упрощенном виде
        /// worktime in short format
        /// </summary>
        public string WorkTime { get; set; }

        /// <summary>
        /// Cсылка на приложение pschooser для отображения информации о ПВЗ
        /// External pschooser app url for parcelshop detail info
        /// </summary>
        public string AddressUrl { get; set; }

        /// <summary>
        /// Дополнительные данные адреса ПВЗ, описание пути
        /// Additional data for parcelshop, distance description, walk way and
        /// etc
        /// </summary>
        public string AddressInfo { get; set; }

        /// <summary>
        /// Список поддерживаемых услуг
        /// Available parcelshop services
        /// <para>
        /// Выдача отправлений в ПВЗ -
        /// HAND_OUT_IN_PARCEL_SHOP
        /// </para>
        /// <para>
        /// Возврат отправлений из ПВЗ -
        /// DIRECT_RETURN
        /// </para>
        /// <para>
        /// Прием клиентских возвратов в ПВЗ -
        /// CUSTOMER_RETURN_RECEPTION_IN_PARCEL_SHOP
        /// </para>
        /// <para>
        /// Оплата отправлений банковскими картами в ПВЗ -
        /// COD_VIA_BANK_CARD_IN_PARCEL_SHOP
        /// </para>
        /// <para>
        /// Возврат сопроводительных документов -
        /// ACCOMPANYING_DOCUMENTS_RETURN
        /// </para>
        /// <para>
        /// Самопривоз клиентом в транзитный ПВЗ для дальнейшей доставки -
        /// DROP_OFF_TO_TRANSIT_PARCELSHOP
        /// </para>
        /// </summary>
        public List<string> Services { get; set; }

        /// <summary>
        /// Станция метро
        /// Metro station
        /// </summary>
        public string MetroStation { get; set; }

        /// <summary>
        /// Станция ж/д
        /// Railway station
        /// </summary>
        public string RailwayStation { get; set; }

        /// <summary>
        /// Район
        /// District
        /// </summary>
        public string District { get; set; }

        /// <summary>
        /// Плановая дата закрытия
        /// Date of planned close datetime
        /// </summary>
        public DateTime? DateOfPlannedClose { get; set; }

        /// <summary>
        /// Юрлицо ПВЗ
        /// Parcelshop legal entity
        /// </summary>
        public string LegalEntityName { get; set; }

        /// <summary>
        /// Контактные телефоны
        /// Contact phone numbers
        /// </summary>
        public string ContactPhones { get; set; }

    }

    #endregion

    #region Parcel

    public class ParcelMeasurements
    {
        /// <summary>
        /// Высота в см
        /// Height in cm
        /// </summary>
        public int HeightInCentimeters { get; set; }

        /// <summary>
        /// Длина в см
        /// Length in cm
        /// </summary>
        public int LengthInCentimeters { get; set; }

        /// <summary>
        /// Ширина в см
        /// Width in cm
        /// </summary>
        public int WidthInCentimeters { get; set; }

        /// <summary>
        /// Вес, граммы, максимальное значение 30 кг.
        /// Weight in grams, max value is 30 kg
        /// </summary>
        public int WeightInGrams { get; set; }
    }

    public class Customer
    {

        /// <summary>
        /// Имя
        /// First name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Фамилия
        /// Last name
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Отчество
        /// Middle name
        /// </summary>
        public string MiddleName { get; set; }

        /// <summary>
        /// Массив телефонов получателя, обрабатываются только первые три
        /// элемента
        /// Phones array, processing up to 3 items
        /// 
        /// Обратите внимание, что первый элемент должен быть номером
        /// мобильного телефона
        /// Pay attention to set the first element as mobile phone number.
        /// </summary>
        public List<string> PhoneNumbers { get; set; }

        /// <summary>
        /// Электропочта получателя, e-mail
        /// Email
        /// </summary>
        public string EMail { get; set; }

        /// <summary>
        /// Адрес получателя до номера дома без деталей
        /// Address with district, street house without apartment or details
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Дополнительные данные адреса получателя: код домофона, этаж и др.
        /// Additional address info: door code, floor, etc
        /// </summary>
        public string AddressDetails { get; set; }

        /// <summary>
        /// Дополнительные данные адреса получателя: квартира
        /// Additional address info: apartment
        /// </summary>
        public string AddressApartment { get; set; }

        /// <summary>
        /// Город доставки
        /// City name
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Индекс точки доставки адреса получателя
        /// Zipcode
        /// </summary>
        public string ZipCode { get; set; }
    }

    public class ExtraParam
    {

        /// <summary>
        /// Наименование параметра
        /// Param name (key)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Значение параметра
        /// Param value
        /// </summary>
        public string Value { get; set; }
    }

    public class DeliveryTerm
    {

        /// <summary>
        /// Мин кол-во дней доставки
        /// Min delivery days count
        /// </summary>
        public int? DeliveryDaysMin { get; set; }

        /// <summary>
        /// Макс кол-во дней доставки
        /// Max delivery days count
        /// </summary>
        public int? DeliveryDaysMax { get; set; }

        /// <summary>
        /// Мин кол-во дней возврата
        /// Min return days count
        /// </summary>
        public int? ReturnDeliveryDaysMin { get; set; }

        /// <summary>
        /// Макс кол-во дней возврата
        /// Max return days count
        /// </summary>
        public int? ReturnDeliveryDaysMax { get; set; }

    }

    public class ParcelContentModel
    {

        /// <summary>
        /// Идентификатор товара в системе клиента
        /// Client item Id
        /// </summary>
        public string ClientItemId { get; set; }

        /// <summary>
        /// Тип товара
        /// Item type. Possible values include: 'DELIVERY', 'ITEM'
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Артикул товара
        /// Item number / item of good
        /// </summary>
        public string Article { get; set; }

        /// <summary>
        /// Код товара (реквизит введен постановлением Правительства РФ от
        /// 21.02.2019 № 174 - п. 5 ст. 4.7 Закона № 54-ФЗ)
        /// Code Number (the requisite was introduced by Decree of the
        /// Government of the Russian Federation of February 21, 2019 No. 174
        /// - paragraph 5 of Art. 4.7 of Law No. 54-ФЗ)
        /// </summary>
        public string CodeNumber { get; set; }

        /// <summary>
        /// Наименование товара
        /// Item name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Количество
        /// Quantity
        /// </summary>
        public int? Quantity { get; set; }

        /// <summary>
        /// Стоимость товара в рублях
        /// Item price in rubles
        /// </summary>
        public float? Price { get; set; }

        /// <summary>
        /// НДС в процентах, если не указан (VAT == null), то "Без НДС".
        /// Значение НДС передается в процентах, т.е. для НДС 20.5%
        /// необходимо передать значение 20.5
        /// VAT in precent, VAT == null means nds not included
        /// </summary>
        public float? Vat { get; set; }

    }

    #region CreateVsdParcel

    public class CreateVsdParcelParams : ParamsWithPublicToken
    {
        /// <summary>
        /// № фактуры предзаказа
        /// Preadvice act number
        /// </summary>
        public string ActNumber { get; set; }

        /// <summary>
        /// Дата фактуры предзаказа (ограничение даты: можно выбрать от -1
        /// месяца до текущей.)
        /// Preadvice vsd act date (date limit: you can choose from -1 month
        /// to current.)
        /// </summary>
        public DateTime VsdActDate { get; set; }

        /// <summary>
        /// Дата документа предзаказа (ограничение даты: можно выбрать от -1
        /// месяца до текущей.)
        /// Preadvice vsd document date (date limit: you can choose from -1
        /// month to current.)
        /// </summary>
        public DateTime VsdDocumentDate { get; set; }

        /// <summary>
        /// Код склада отгрузки, укажите пустое значение для подстановки
        /// склада по умолчанию
        /// получить список можно по методу /business-unit/get-dc
        /// Pickup distribution center code, set null to find default code or
        /// check available code by /business-unit/get-dc
        /// </summary>
        public string PickupDcCode { get; set; }

        /// <summary>
        /// Код склада возврата, укажите пустое значение для подстановки
        /// склада по умолчанию
        /// получить список можно по методу /business-unit/get-dc
        /// Return distribution center code, set null to find default code or
        /// check available code by /business-unit/get-dc
        /// </summary>
        public string ReturnDcCode { get; set; }

        /// <summary>
        /// Опциональный срок хранения отправления в ПВЗ в днях
        /// Custom storage days for parcel
        /// </summary>
        public int? ParcelShopStorageDaysCount { get; set; }

        /// <summary>
        /// Код БЮ, список доступных БЮ можно получить через
        /// /business-unit/get
        /// Business unit code, check available codes by /business-unit/get
        /// </summary>
        public string BusinessUnitCode { get; set; }

        /// <summary>
        /// Баркод отправления
        /// если у бизнес-юнита включена автогенерация баркода, значение не
        /// учитывается и будет выдан числовой баркод
        /// Parcel preadvice barcode, if autogeneration barcode option is on,
        /// value is set automatically by counter in 14 digits format
        /// </summary>
        public string ParcelBarcode { get; set; }

        /// <summary>
        /// Уникальный номер отправления в системе клиента
        /// Unique client parcel number
        /// </summary>
        public string ClientParcelNumber { get; set; }

        /// <summary>
        /// Номер заказа в системе клиента
        /// Order number in client system
        /// </summary>
        public string ClientOrderNumber { get; set; }

        /// <summary>
        /// Точка доставки отправления - ПВЗ, проверьте доступность ПВЗ до его
        /// передачи через /parcel-shop/get-by-business-unit
        /// Delivery point - parcelshop code, check available parcelshops
        /// before set by /parcel-shop/get-by-business-unit
        /// </summary>
        public string ParcelShopCode { get; set; }

        /// <summary>
        /// Наложенный платеж отправления, максимальное значение 150000 руб.
        /// Preadvice COD, cash on delivery value in RUB
        /// </summary>
        public double CashOnDeliveryValue { get; set; }

        /// <summary>
        /// Страховая стоимость отправления, максимальное значение 150000 руб.
        /// Preadvice insurance value in RUB
        /// </summary>
        public double InsuranceValue { get; set; }

        /// <summary>
        /// Измерения отправления
        /// Preadvice measurements
        /// </summary>
        public ParcelMeasurements ParcelMeasurements { get; set; }

        /// <summary>
        /// Данные получателя
        /// Customer data
        /// </summary>
        public Customer Customer { get; set; }

        /// <summary>
        /// Хэш пинкода HMAC-SHA256 от клиента
        /// Client custom pin code hash gererated by client, HMAC-SHA256
        /// https://github.com/aspnet/Identity/blob/c7276ce2f76312ddd7fccad6e399da96b9f6fae1/src/Core/PasswordHasher.cs
        /// </summary>
        public string PincodeHash { get; set; }

        /// <summary>
        /// Опциональный список параметров в виде ключ-значение
        /// Optional extra params list as key - value
        /// </summary>
        public List<ExtraParam> ExtraParams { get; set; }
    }

    public class CreateVsdParcelResponse : BaseResponse
    {

        /// <summary>
        /// Баркод отправления, при выключенной опции автогенерации необходимо
        /// указывать баркод по правилам регулярного выражения клиента
        /// Parcel preadvice Barcode. If autogenerate barcode option is off
        /// set barcode by client regex rule
        /// </summary>
        public string ParcelBarcode { get; set; }

        /// <summary>
        /// Уникальный номер отправления в системе клиента, не должен
        /// повторяться
        /// Unique parcel number in client system
        /// </summary>
        public string ClientParcelNumber { get; set; }

        /// <summary>
        /// Номер заказа в системе клиента
        /// Order number in client system
        /// </summary>
        public string ClientOrderNumber { get; set; }

        /// <summary>
        /// Предварительная цена доставки
        /// Preliminary delivery price
        /// </summary>
        public double? ApproximatePrice { get; set; }

        /// <summary>
        /// Время доставки
        /// Date of delivery
        /// </summary>
        public DeliveryTerm DeliveryTerm { get; set; }
    }
    #endregion

    #region CreateStandardParcel

    public class CreateStandardParcelParams : ParamsWithPublicToken
    {

        /// <summary>
        /// Код склада отгрузки, укажите пустое значение для подстановки
        /// склада по умолчанию
        /// получить список можно по методу /business-unit/get-dc
        /// Pickup distribution center code, set null to find default code or
        /// check available code by /business-unit/get-dc
        /// </summary>
        public string PickupDcCode { get; set; }

        /// <summary>
        /// Код склада возврата, укажите пустое значение для подстановки
        /// склада по умолчанию
        /// получить список можно по методу /business-unit/get-dc
        /// Return distribution center code, set null to find default code or
        /// check available code by /business-unit/get-dc
        /// </summary>
        public string ReturnDcCode { get; set; }

        /// <summary>
        /// Опциональный срок хранения отправления в ПВЗ в днях
        /// Custom storage days for parcel
        /// </summary>
        public int? ParcelShopStorageDaysCount { get; set; }

        /// <summary>
        /// Код БЮ, список доступных БЮ можно получить через
        /// /business-unit/get
        /// Business unit code, check available codes by /business-unit/get
        /// </summary>
        public string BusinessUnitCode { get; set; }

        /// <summary>
        /// Баркод отправления
        /// если у бизнес-юнита включена автогенерация баркода, значение не
        /// учитывается и будет выдан числовой баркод
        /// Parcel preadvice barcode, if autogeneration barcode option is on,
        /// value is set automatically by counter in 14 digits format
        /// </summary>
        public string ParcelBarcode { get; set; }

        /// <summary>
        /// Уникальный номер отправления в системе клиента
        /// Unique client parcel number
        /// </summary>
        public string ClientParcelNumber { get; set; }

        /// <summary>
        /// Номер заказа в системе клиента
        /// Order number in client system
        /// </summary>
        public string ClientOrderNumber { get; set; }

        /// <summary>
        /// Точка доставки отправления - ПВЗ, проверьте доступность ПВЗ до его
        /// передачи через /parcel-shop/get-by-business-unit
        /// Delivery point - parcelshop code, check available parcelshops
        /// before set by /parcel-shop/get-by-business-unit
        /// </summary>
        public string ParcelShopCode { get; set; }

        /// <summary>
        /// Наложенный платеж отправления, максимальное значение 150000 руб.
        /// Preadvice COD, cash on delivery value in RUB
        /// </summary>
        public float CashOnDeliveryValue { get; set; }

        /// <summary>
        /// Страховая стоимость отправления, максимальное значение 150000 руб.
        /// Preadvice insurance value in RUB
        /// </summary>
        public float InsuranceValue { get; set; }

        /// <summary>
        /// Измерения отправления
        /// Preadvice measurements
        /// </summary>
        public ParcelMeasurements ParcelMeasurements { get; set; }

        /// <summary>
        /// Данные получателя
        /// Customer data
        /// </summary>
        public Customer Customer { get; set; }

        /// <summary>
        /// Хэш пинкода HMAC-SHA256 от клиента
        /// Client custom pin code hash gererated by client, HMAC-SHA256
        /// https://github.com/aspnet/Identity/blob/c7276ce2f76312ddd7fccad6e399da96b9f6fae1/src/Core/PasswordHasher.cs
        /// </summary>
        public string PincodeHash { get; set; }

        /// <summary>
        /// Опциональный список параметров в виде ключ-значение
        /// Optional extra params list as key - value
        /// </summary>
        public List<ExtraParam> ExtraParams { get; set; }

    }

    public class CreateStandardParcelResponse : BaseResponse
    {
        /// <summary>
        /// Баркод отправления, при выключенной опции автогенерации необходимо
        /// указывать баркод по правилам регулярного выражения клиента
        /// Parcel preadvice Barcode. If autogenerate barcode option is off
        /// set barcode by client regex rule
        /// </summary>
        public string ParcelBarcode { get; set; }

        /// <summary>
        /// Уникальный номер отправления в системе клиента, не должен
        /// повторяться
        /// Unique parcel number in client system
        /// </summary>
        public string ClientParcelNumber { get; set; }

        /// <summary>
        /// Номер заказа в системе клиента
        /// Order number in client system
        /// </summary>
        public string ClientOrderNumber { get; set; }

        /// <summary>
        /// Предварительная цена доставки
        /// Preliminary delivery price
        /// </summary>
        public double? ApproximatePrice { get; set; }

        /// <summary>
        /// Время доставки
        /// Date of delivery
        /// </summary>
        public DeliveryTerm DeliveryTerm { get; set; }
    }

    #endregion

    #region CreateDropParcel

    public class CreateDropParcelParams : ParamsWithPublicToken
    {

        /// <summary>
        /// Опциональный срок хранения отправления в ПВЗ в днях
        /// Custom storage days for parcel
        /// </summary>
        public int? ParcelShopStorageDaysCount { get; set; }

        /// <summary>
        /// Код БЮ, список доступных БЮ можно получить через
        /// /business-unit/get
        /// Business unit code, check available codes by /business-unit/get
        /// </summary>
        public string BusinessUnitCode { get; set; }

        /// <summary>
        /// Баркод отправления
        /// если у бизнес-юнита включена автогенерация баркода, значение не
        /// учитывается и будет выдан числовой баркод
        /// Parcel preadvice barcode, if autogeneration barcode option is on,
        /// value is set automatically by counter in 14 digits format
        /// </summary>
        public string ParcelBarcode { get; set; }

        /// <summary>
        /// Уникальный номер отправления в системе клиента
        /// Unique client parcel number
        /// </summary>
        public string ClientParcelNumber { get; set; }

        /// <summary>
        /// Номер заказа в системе клиента
        /// Order number in client system
        /// </summary>
        public string ClientOrderNumber { get; set; }

        /// <summary>
        /// Точка доставки отправления - ПВЗ, проверьте доступность ПВЗ до его
        /// передачи через /parcel-shop/get-by-business-unit
        /// Delivery point - parcelshop code, check available parcelshops
        /// before set by /parcel-shop/get-by-business-unit
        /// </summary>
        public string ParcelShopCode { get; set; }

        /// <summary>
        /// Наложенный платеж отправления, максимальное значение 150000 руб.
        /// Preadvice COD, cash on delivery value in RUB
        /// </summary>
        public double CashOnDeliveryValue { get; set; }

        /// <summary>
        /// Страховая стоимость отправления, максимальное значение 150000 руб.
        /// Preadvice insurance value in RUB
        /// </summary>
        public double InsuranceValue { get; set; }

        /// <summary>
        /// Измерения отправления
        /// Preadvice measurements
        /// </summary>
        public ParcelMeasurements ParcelMeasurements { get; set; }

        /// <summary>
        /// Данные получателя
        /// Customer data
        /// </summary>
        public Customer Customer { get; set; }

        /// <summary>
        /// Хэш пинкода HMAC-SHA256 от клиента
        /// Client custom pin code hash gererated by client, HMAC-SHA256
        /// https://github.com/aspnet/Identity/blob/c7276ce2f76312ddd7fccad6e399da96b9f6fae1/src/Core/PasswordHasher.cs
        /// </summary>
        public string PincodeHash { get; set; }

        /// <summary>
        /// Опциональный список параметров в виде ключ-значение
        /// Optional extra params list as key - value
        /// </summary>
        public List<ExtraParam> ExtraParams { get; set; }
    }

    public class CreateDropParcelResponse : BaseResponse
    {

        /// <summary>
        /// Баркод отправления, при выключенной опции автогенерации необходимо
        /// указывать баркод по правилам регулярного выражения клиента
        /// Parcel preadvice Barcode. If autogenerate barcode option is off
        /// set barcode by client regex rule
        /// </summary>
        public string ParcelBarcode { get; set; }

        /// <summary>
        /// Уникальный номер отправления в системе клиента, не должен
        /// повторяться
        /// Unique parcel number in client system
        /// </summary>
        public string ClientParcelNumber { get; set; }

        /// <summary>
        /// Номер заказа в системе клиента
        /// Order number in client system
        /// </summary>
        public string ClientOrderNumber { get; set; }

        /// <summary>
        /// Предварительная цена доставки
        /// Preliminary delivery price
        /// </summary>
        public double? ApproximatePrice { get; set; }

        /// <summary>
        /// Время доставки
        /// Date of delivery
        /// </summary>
        public DeliveryTerm DeliveryTerm { get; set; }
    }

    #endregion

    #region CreateHomeCourierParcel

    public class CreateHomeCourierParcelParams : ParamsWithPublicToken
    {

        /// <summary>
        /// Точка доставки отправления - ПВЗ, проверьте доступность ПВЗ до его
        /// передачи через /parcel-shop/get-by-business-unit
        /// Delivery point - parcelshop code, check available parcelshops
        /// before set by /parcel-shop/get-by-business-unit
        /// </summary>
        public string ParcelShopCode { get; set; }

        /// <summary>
        /// Код склада отгрузки, укажите пустое значение для подстановки
        /// склада по умолчанию
        /// получить список можно по методу /business-unit/get-dc
        /// Pickup distribution center code, set null to find default code or
        /// check available code by /business-unit/get-dc
        /// </summary>
        public string PickupDcCode { get; set; }

        /// <summary>
        /// Код склада возврата, укажите пустое значение для подстановки
        /// склада по умолчанию
        /// получить список можно по методу /business-unit/get-dc
        /// Return distribution center code, set null to find default code or
        /// check available code by /business-unit/get-dc
        /// </summary>
        public string ReturnDcCode { get; set; }

        /// <summary>
        /// Опциональный срок хранения отправления
        /// Custom storage days for parcel
        /// </summary>
        public int? ParcelShopStorageDaysCount { get; set; }

        /// <summary>
        /// Временной слот, формат времени ЧЧ:мм-ЧЧ:мм
        /// Time slot for customer delivery in format, default is HH:mm-:HH:mm
        /// </summary>
        public string TimeSlot { get; set; }

        /// <summary>
        /// Код БЮ, список доступных БЮ можно получить через
        /// /business-unit/get
        /// Business unit code, check available codes by /business-unit/get
        /// </summary>
        public string BusinessUnitCode { get; set; }

        /// <summary>
        /// Баркод отправления
        /// если у бизнес-юнита включена автогенерация баркода, значение не
        /// учитывается и будет выдан числовой баркод
        /// Parcel preadvice barcode, if autogeneration barcode option is on,
        /// value is set automatically by counter in 14 digits format
        /// </summary>
        public string ParcelBarcode { get; set; }

        /// <summary>
        /// Уникальный номер отправления в системе клиента
        /// Unique client parcel number
        /// </summary>
        public string ClientParcelNumber { get; set; }

        /// <summary>
        /// Номер заказа в системе клиента
        /// Order number in client system
        /// </summary>
        public string ClientOrderNumber { get; set; }

        /// <summary>
        /// Наложенный платеж отправления, максимальное значение 150000 руб.
        /// Preadvice COD, cash on delivery value in RUB
        /// </summary>
        public double CashOnDeliveryValue { get; set; }

        /// <summary>
        /// Страховая стоимость отправления, максимальное значение 150000 руб.
        /// Preadvice insurance value in RUB
        /// </summary>
        public double InsuranceValue { get; set; }

        /// <summary>
        /// Измерения отправления
        /// Preadvice measurements
        /// </summary>
        public ParcelMeasurements ParcelMeasurements { get; set; }

        /// <summary>
        /// Данные получателя
        /// Customer data
        /// </summary>
        public Customer Customer { get; set; }

        /// <summary>
        /// Хэш пинкода HMAC-SHA256 от клиента
        /// Client custom pin code hash gererated by client, HMAC-SHA256
        /// https://github.com/aspnet/Identity/blob/c7276ce2f76312ddd7fccad6e399da96b9f6fae1/src/Core/PasswordHasher.cs
        /// </summary>
        public string PincodeHash { get; set; }

        /// <summary>
        /// Опциональный список параметров в виде ключ-значение
        /// Optional extra params list as key - value
        /// </summary>
        public List<ExtraParam> ExtraParams { get; set; }

    }

    public class CreateHomeCourierParcelResponse : BaseResponse
    {

        /// <summary>
        /// Баркод отправления, при выключенной опции автогенерации необходимо
        /// указывать баркод по правилам регулярного выражения клиента
        /// Parcel preadvice Barcode. If autogenerate barcode option is off
        /// set barcode by client regex rule
        /// </summary>
        public string ParcelBarcode { get; set; }

        /// <summary>
        /// Уникальный номер отправления в системе клиента, не должен
        /// повторяться
        /// Unique parcel number in client system
        /// </summary>
        public string ClientParcelNumber { get; set; }

        /// <summary>
        /// Номер заказа в системе клиента
        /// Order number in client system
        /// </summary>
        public string ClientOrderNumber { get; set; }

        /// <summary>
        /// Предварительная цена доставки
        /// Preliminary delivery price
        /// </summary>
        public double? ApproximatePrice { get; set; }

        /// <summary>
        /// Время доставки
        /// Date of delivery
        /// </summary>
        public DeliveryTerm DeliveryTerm { get; set; }
    }

    #endregion

    #region DeleteParcel

    public class DeleteParcelParams : ParamsWithSecuredToken
    {
        /// <summary>
        /// Баркод предзаказа
        /// Preadvice barcode
        /// </summary>
        public string Barcode { get; set; }

    }

    public class DeleteParcelResponse : BaseResponse { }

    #endregion

    #region CreateOrUpdateParcelContent

    public class CreateOrUpdateParcelContentParams : ParamsWithSecuredToken
    {

        /// <summary>
        /// Баркод посылки
        /// Barcode
        /// </summary>
        public string Barcode { get; set; }

        /// <summary>
        /// Товары в посылке
        /// Items in a parcel
        /// </summary>
        public List<ParcelContentModel> ParcelItems { get; set; }
    }

    public class CreateOrUpdateParcelContentResponse : BaseResponse
    {
        /// <summary>
        /// </summary>
        public string Barcode { get; set; }
    }

    #endregion

    #region GetStatusesParcel

    public class GetStatusesParcelParams : ParamsWithSecuredToken
    {
        /// <summary>
        /// Баркод
        /// Barcode
        /// </summary>
        public string Barcode { get; set; }
    }

    public class GetStatusesParcelResponse : BaseResponse
    {
        /// <summary>
        /// Список статусов
        /// Statuses' list
        /// </summary>
        public List<ParcelStatus> Statuses { get; set; }
    }

    public class ParcelStatus
    {

        /// <summary>
        /// Код бизнес-юнита
        /// Business unit code
        /// </summary>
        public string BusinessUnitCode { get; set; }

        /// <summary>
        /// Баркод отправления
        /// Barcode
        /// </summary>
        public string ParcelBarcode { get; set; }

        /// <summary>
        /// Номер заказа
        /// Order number
        /// </summary>
        public string OrderNumber { get; set; }

        /// <summary>
        /// наименование статуса
        /// Status name
        /// </summary>
        public string StatusName { get; set; }

        /// <summary>
        /// Системное наименование статуса
        /// Status system name
        /// </summary>
        public string StatusSystemName { get; set; }

        /// <summary>
        /// Фактическая дата статуса
        /// Fact status datetime
        /// </summary>
        public DateTime? StatusTimestamp { get; set; }

        /// <summary>
        /// Дата удаления статуса
        /// Status delet datetime
        /// </summary>
        public DateTime? StatusDeleteTimestamp { get; set; }

        /// <summary>
        /// Дата поступления статуса
        /// Status change datetime
        /// </summary>
        public DateTime? StatusChangeTimestamp { get; set; }

        /// <summary>
        /// Причина отказа
        /// Refuse reason
        /// </summary>
        public string RefuseReasonName { get; set; }

        /// <summary>
        /// Системное наименование причины
        /// System refuse reason
        /// </summary>
        public string RefuseReasonSystemName { get; set; }

        /// <summary>
        /// Признак текущего статуса
        /// Is current parcel status
        /// </summary>
        public bool? Current { get; set; }

        /// <summary>
        /// Типа оплаты, будет указан, если статус RECEIVED
        /// Payment Type, exist if status is RECEIVED. Possible values
        /// include: 'CARD_IN_DELIVERY_OFFICE', 'CASH_IN_DELIVERY_OFFICE'
        /// </summary>
        public string PaymentType { get; set; }

        /// <summary>
        /// Срок хранения отправления, начинается после приема в ПВЗ, статус
        /// ARRIVED_AT_PARCELSHOP
        /// Parcel storage datetime, starts after ARRIVED_AT_PARCELSHOP status
        /// is set
        /// </summary>
        public DateTime? StorageDateTime { get; set; }
    }

    #endregion

    #endregion

    #region Calculate

    public class TransitPoint
    {
        /// <summary>
        /// Тип точки
        /// Point type. Possible values include: 'ClientDistributionCenter',
        /// 'Terminal', 'Parcelshop', 'Home'
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Код точки
        /// Point code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Адрес точки
        /// </summary>
        public string Address { get; set; }
    }

    public class CalculationResult
    {
        /// <summary>
        /// Стоимость доставки
        /// </summary>
        public float? DeliveryPrice { get; set; }

        /// <summary>
        /// Стоимость оплаты картой или наличными
        /// </summary>
        public float? PaymentPrice { get; set; }

        /// <summary>
        /// Стоимость страховки
        /// </summary>
        public float? InsurancePrice { get; set; }

        /// <summary>
        /// Общая стоимость
        /// </summary>
        public float? TotalPrice { get; private set; }
    }

    public class GetPriceCalculationByPointParams : ParamsWithSecuredToken
    {
        /// <summary>
        /// Идентификатор запроса для сопоставления ответа
        /// Request Id to define response by request
        /// </summary>
        public string RequestId { get; set; }

        /// <summary>
        /// Продукт расчета
        /// Calculation product. Possible values include:
        /// 'ParcelShopDelivery', 'ParcelShopReturn',
        /// 'ParcelShopCustomerReturnPaidByCustomer',
        /// 'ParcelShopCashOnDelivery',
        /// 'ParcelShopCustomerReturnPaidByClient',
        /// 'ParcelShopCardOnDelivery', 'ParcelShopCardOnCustomerReturn',
        /// 'ParcelShopCashOnCustomerReturn', 'ParcelInsurance',
        /// 'DropOffToTargetParcelShop', 'DropOffToTransitParcelShop',
        /// 'ParcelInsuranceDelivery', 'ParcelInsuranceReturn',
        /// 'CourierDelivery', 'CourierReturn', 'InsuranceCourierDelivery',
        /// 'InsuranceCourierReturn'
        /// </summary>
        public string Product { get; set; }

        /// <summary>
        /// Код БЮ
        /// Business unit code
        /// </summary>
        public string BusinessUnitCode { get; set; }

        /// <summary>
        /// исходная точка
        /// source point
        /// </summary>
        public TransitPoint SourcePoint { get; set; }

        /// <summary>
        /// конечная точка
        /// destination point
        /// </summary>
        public TransitPoint DestinationPoint { get; set; }

        /// <summary>
        /// Наложенный платеж отправления, максимальное значение 150000 руб.
        /// Preadvice COD, cash on delivery value in RUB, max value is 150000
        /// </summary>
        public float? CashOnDeliveryValue { get; set; }

        /// <summary>
        /// Страховая стоимость отправления, максимальное значение 150000 руб.
        /// Preadvice insurance value in RUB, max value is 150000
        /// </summary>
        public float? InsuranceValue { get; set; }

        /// <summary>
        /// Измерения отправления
        /// Preadvice measurements
        /// </summary>
        public ParcelMeasurements ParcelMeasurements { get; set; }
    }

    public class GetPriceCalculationByPointResponse : BaseResponse
    {
        /// <summary>
        /// Идентификатор запроса для сопоставления ответа
        /// Request Id to define response by request
        /// </summary>
        public string RequestId { get; set; }

        /// <summary>
        /// Баркод, по которому рассчитывалась предварительная цена
        /// Barcode
        /// </summary>
        public string Barcode { get; set; }

        /// <summary>
        /// Предварительная цена доставки
        /// Preliminary delivery price
        /// </summary>
        public float? ApproximatePrice { get; set; }

        /// <summary>
        /// Результаты расчетов
        /// Calculation results
        /// </summary>
        public CalculationResult CalculationResult { get; set; }

        /// <summary>
        /// Расчет по объемному весу
        /// Calculated by volume weight
        /// </summary>
        public bool? CalculatedByVolumeWeight { get; set; }
    }

    #endregion
}
