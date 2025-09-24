using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AdvantShop.Core.Services.Payment.Mokka.Api
{

    public class BaseResponse
    {
        /// <summary>
        /// Код ответа.
        /// </summary>
        public int Status { get; set; }
        
        /// <summary>
        /// Короткое текстовое описание ответа.
        /// </summary>
        public string Message { get; set; }
    }

    #region Registration

    public class RegistrationParameters
    {
        /// <summary>
        /// URL для ответа от Мокка по решению для клиента.
        /// </summary>
        public string CallbackUrl { get; set; }
        
        /// <summary>
        /// URL для редиректа после нажатия на кнопку/ссылку в форме Мокка "Вернуться в интернет магазин"
        /// </summary>
        public string RedirectUrl { get; set; }

        /// <summary>
        /// Информация о заказе
        /// </summary>
        public CurrentOrderRegistration CurrentOrder { get; set; }
        
        /// <summary>
        /// Номер телефона клиента 10 цифр
        /// <remarks>Без кода страны</remarks>
        /// </summary>
        public string PrimaryPhone { get; set; }
        
        /// <summary>
        /// Email клиента
        /// </summary>
        public string PrimaryEmail { get; set; }
        
        /// <summary>
        /// Информация о клиенте
        /// </summary>
        public Person Person { get; set; }
        
        /// <summary>
        /// Дополнительная информация о клиенте
        /// </summary>
        public AdditionalDataRegistration AdditionalData { get; set; }
    }

    public class CurrentOrderRegistration
    {
        /// <summary>
        /// Уникальный номер заказа.
        /// <remarks>Не более 255 символов.</remarks>
        /// </summary>
        public string OrderId { get; set; }
    }

    public class Person
    {
        /// <summary>
        /// Имя клиента
        /// </summary>
        public string FirstName { get; set; }
        
        /// <summary>
        /// Фамилия клиента.
        /// </summary>
        public string Surname { get; set; }
        
        /// <summary>
        /// Отчество клиента
        /// </summary>
        public string Patronymic { get; set; }
        
        /// <summary>
        /// Дата рождения клиента
        /// </summary>
        [JsonConverter(typeof(DateFormatConverter), "dd.MM.yyyy")]
        public DateTime? BirthDate { get; set; }
    }

    public class AdditionalDataRegistration
    {
        /// <summary>
        /// Адрес, откуда клиент зашёл на сайт партнёра.
        /// </summary>
        public string PreviousUrl { get; set; }
        
        /// <summary>
        /// В каком канале открыта форма.
        /// <remarks>Возможные значения: mobile, app, desktop</remarks>
        /// </summary>
        public string Channel { get; set; }
        
        /// <summary>
        /// Является ли клиент повторным для партнёра.
        /// </summary>
        public bool? ReturningCustomer { get; set; }
        
        /// <summary>
        /// Является ли последним способом оплаты банковская карта.
        /// </summary>
        public bool? BankCard { get; set; }
        
        /// <summary>
        /// Количество заказов за последние 6 мес.
        /// </summary>
        public string LastOrders { get; set; }
        
        /// <summary>
        /// Совпадает ли текущий адрес доставки с предыдущим адресом доставки.
        /// </summary>
        public bool? SameAddress { get; set; }
    }
    
    public class RegistrationResponse : BaseResponse
    {
        /// <summary>
        /// Cсылка на сгенерированный iFrame.
        /// </summary>
        public string IframeUrl { get; set; }
    }

    #endregion Registration

    #region Registration Callbak

    public class RegistrationCallbak
    {
        public string OrderId { get; set; }
        public string Decision { get; set; }
        public float Amount { get; set; }
        public string MobilePhone { get; set; }
        public string Email { get; set; }
    }

    #endregion Registration Callbak

    #region Checkout

    public class CheckoutParameters
    {
        /// <summary>
        /// URL для ответа от Мокка по решению для клиента.
        /// </summary>
        public string CallbackUrl { get; set; }
        
        /// <summary>
        /// URL для редиректа после нажатия на кнопку/ссылку в форме Мокка "Вернуться в интернет магазин"
        /// </summary>
        public string RedirectUrl { get; set; }

        /// <summary>
        /// Информация о заказе
        /// </summary>
        public CurrentOrderCheckout CurrentOrder { get; set; }

        /// <summary>
        /// Номер телефона клиента 10 цифр
        /// <remarks>Без кода страны</remarks>
        /// </summary>
        public string PrimaryPhone { get; set; }

        /// <summary>
        /// Email клиента
        /// </summary>
        public string PrimaryEmail { get; set; }
        
        /// <summary>
        /// Информация о клиенте
        /// </summary>
        public Person Person { get; set; }
        
        /// <summary>
        /// Массив с информацией о заказе
        /// </summary>
        public List<CartItem> CartItems { get; set; }
        
        /// <summary>
        /// Флаг, который определяет будет ли отображена страница с результатом оформления в iFrame.
        /// <remarks>
        /// <para>По умолчанию - False.</para>
        /// <para>True - по успешному завершению оформления сразу происходит редирект по redirect_url.</para>
        /// <para>False - по успешному завершению оформления будет отображено окно с результатом.</para>
        /// </remarks>
        /// </summary>
        public bool? SkipResultPage { get; set; }
        
        /// <summary>
        /// Дополнительная информация о заказе, которую клиент вводит на сайте партнёра (не в форме Мокка).
        /// </summary>
        public DeliveryInfo DeliveryInfo { get; set; }
        
        /// <summary>
        /// Информация о клиенте.
        /// </summary>
        public AdditionalDataCheckout AdditionalData { get; set; }
    }

    public class DeliveryInfo
    {
        /// <summary>
        /// Имя клиента
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Фамилия клиента.
        /// </summary>
        public string Surname { get; set; }
        
        /// <summary>
        /// Отчество клиента
        /// </summary>
        public string Patronymic { get; set; }
        
        /// <summary>
        /// Способ доставки.
        /// </summary>
        public string Type { get; set; }
        
        /// <summary>
        /// Адрес доставки.
        /// </summary>
        public string Address { get; set; }
        
        /// <summary>
        /// Номер телефона клиента 10 цифр
        /// <remarks>Без кода страны</remarks>
        /// </summary>
        public string Phone { get; set; }
        
        /// <summary>
        /// Email клиента.
        /// </summary>
        public string Email { get; set; }
    }

    public class AdditionalDataCheckout
    {
        /// <summary>
        /// Адрес, откуда клиент зашёл на сайт партнёра.
        /// </summary>
        public string PreviousUrl { get; set; }
                
        /// <summary>
        /// В каком канале открыта форма.
        /// <remarks>Возможные значения: mobile, app, desktop</remarks>
        /// </summary>
        public string Channel { get; set; }
                
        /// <summary>
        /// Является ли клиент повторным для партнёра.
        /// </summary>
        public bool? ReturningCustomer { get; set; }
                
        /// <summary>
        /// Является ли последним способом оплаты банковская карта.
        /// </summary>
        public bool? BankCard { get; set; }
                
        /// <summary>
        /// Количество заказов за последние 6 мес.
        /// </summary>
        public string LastOrders { get; set; }
                
        /// <summary>
        /// Совпадает ли текущий адрес доставки с предыдущим адресом доставки.
        /// </summary>
        public bool? SameAddress { get; set; }
        
        /// <summary>
        /// Дополнительная информация о клиенте.
        /// </summary>
        public Client Client { get; set; }
        
        /// <summary>
        /// Дополнительная информация о заказе.
        /// </summary>
        public Order Order { get; set; }
        
        /// <summary>
        /// Дополнительная информация о доставке
        /// </summary>
        public Delivery Delivery { get; set; }
        
        /// <summary>
        /// Дополнительная информация о корзине
        /// </summary>
        public List<Purchase> Purchase { get; set; }
    }

    public class Client
    {
        /// <summary>
        /// Номер телефона клиента.
        /// </summary>
        public string Phone { get; set; }
        
        /// <summary>
        /// Email клиента.
        /// </summary>
        public string Email { get; set; }
        
        /// <summary>
        /// Имя или идентификатор клиента.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public string ClientId { get; set; }
        
        /// <summary>
        /// Дата регистрации клиента
        /// </summary>
        [JsonConverter(typeof(DateFormatConverter), "dd.MM.yyyy")]
        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public DateTime? RegistrationDate { get; set; }
        
        /// <summary>
        /// Дата последнего изменения данных клиента
        /// </summary>
        [JsonConverter(typeof(DateFormatConverter), "dd.MM.yyyy")]
        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public DateTime? DataChangeDate { get; set; }
        
        /// <summary>
        /// Число успешных покупок.
        /// </summary>
        public int? PurchasesVolume { get; set; }
        
        /// <summary>
        /// Общая сумма успешных покупок за всё время.
        /// </summary>
        public float? PurchasesSum { get; set; }
        
        /// <summary>
        /// Дата последней покупки клиента
        /// </summary>
        [JsonConverter(typeof(DateFormatConverter), "dd.MM.yyyy")]
        public DateTime? LastPurchaseDate { get; set; }
        
        /// <summary>
        /// Дата первой покупки клиента
        /// </summary>
        [JsonConverter(typeof(DateFormatConverter), "dd.MM.yyyy")]
        public DateTime? FirstPurchaseDate { get; set; }
        
        /// <summary>
        /// Дата рождения клиента
        /// </summary>
        [JsonConverter(typeof(DateFormatConverter), "dd.MM.yyyy")]
        public DateTime? Birthdate { get; set; }
        
        /// <summary>
        /// Пол клиента
        /// <remarks>Возможные значения: m, w.</remarks>
        /// </summary>
        public string Gender { get; set; }
    }

    public class Delivery
    {
        /// <summary>
        /// Способ доставки
        /// <remarks>Возможные значения: store pick-up, pick-up point, registered box, unregistered box, courier, shipping company</remarks>
        /// </summary>
        public string DeliveryKind { get; set; }
        
        /// <summary>
        /// Имя и фамилия получателя.
        /// </summary>
        public string ReceiverName { get; set; }
        
        /// <summary>
        /// Адрес доставки.
        /// </summary>
        public string DeliveryAddress { get; set; }
    }

    public class Order
    {
        /// <summary>
        /// Страна покупки.
        /// </summary>
        public string Country { get; set; }
        
        /// <summary>
        /// Валюта покупки.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public string Currency { get; set; }
        
        /// <summary>
        /// Стоимость покупки.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public float? OrderPrice { get; set; }
    }

    public class Purchase
    {
        /// <summary>
        /// Наименование позиции.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public string ProductName { get; set; }
        
        /// <summary>
        /// Количество.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public int? Number { get; set; }
        
        /// <summary>
        /// Стоимость SKU.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public float? ProductPrice { get; set; }
        
        /// <summary>
        /// Категория товара. Возможные значения: physical, digital, gift_card, discount, shipping_fee
        /// </summary>
        public string Category { get; set; }
        
        /// <summary>
        /// Путь категории.
        /// </summary>
        public string Breadcrumbs { get; set; }
        
        /// <summary>
        /// Размер скидки в %.
        /// </summary>
        public string Discount { get; set; }
        
        /// <summary>
        /// Глобальный идентификатор продукта.
        /// </summary>
        public string GlobalProductId { get; set; }
        
        /// <summary>
        /// Идентификатор продукта вендора.
        /// </summary>
        public string VendorProductId { get; set; }
    }

    public class CartItem
    {
        /// <summary>
        /// Складская учётная единица
        /// </summary>
        public string Sku { get; set; }
        
        /// <summary>
        /// Наименование товара.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Цена товара.
        /// </summary>
        public float? Price { get; set; }

        /// <summary>
        /// Цена товара со скидкой (если есть).
        /// </summary>
        public float? SalePrice { get; set; }
        
        /// <summary>
        /// Количество товара.
        /// </summary>
        public int? Quantity { get; set; }
        
        /// <summary>
        /// Единица измерения товара
        /// </summary>
        public string Unit { get; set; }
        
        /// <summary>
        /// Бренд товара.
        /// </summary>
        public string Brand { get; set; }
        
        /// <summary>
        /// Категория товара.
        /// </summary>
        public string Category { get; set; }
    }

    public class CurrentOrderCheckout
    {
        /// <summary>
        /// Уникальный номер заказа.
        /// <remarks>Не более 255 символов.</remarks>
        /// </summary>
        public string OrderId { get; set; }
        
        /// <summary>
        /// Срок, в течении которого заказ считается актуальным (срок холдирования средств).
        /// </summary>
        public DateTime? ValidTill { get; set; }
        
        /// <summary>
        /// Срок рассрочки в месяцах.
        /// </summary>
        public int? Term { get; set; }
        
        /// <summary>
        /// Сумма заказа в рублях.
        /// </summary>
        public string Amount { get; set; }
        
        /// <summary>
        /// Сумма уже внесённой клиентом предоплаты в рублях.
        /// </summary>
        public string PrepaymentAmount { get; set; }
    }
    
    
    public class CheckoutResponse : BaseResponse
    {
        /// <summary>
        /// Cсылка на сгенерированный iFrame.
        /// </summary>
        public string IframeUrl { get; set; }
    }

    #endregion Checkout

    #region Checkout Callback

    public class CheckoutCallback
    {
        /// <summary>
        /// Уникальный номер заказа
        /// </summary>
        public string OrderId { get; set; }
        
        /// <summary>
        /// Решение по выдаче рассрочки. При положительном решении - значение approved. При отрицательном решении - declined
        /// </summary>
        public string Decision { get; set; }
        
        /// <summary>
        /// Сумма к оплате частями в рублях.
        /// </summary>
        public float Amount { get; set; }
        
        /// <summary>
        /// Сумма предоплаты в рублях.
        /// </summary>
        public float PrepaymentAmount { get; set; }
        
        /// <summary>
        /// Полная сумма заказа, с учётом предоплаты.
        /// </summary>
        public float TotalAmount { get; set; }
        
        /// <summary>
        /// Срок рассрочки в месяцах.
        /// </summary>
        public int Term { get; set; }
        
        /// <summary>
        /// Объект, содержащий информацию о клиенте.
        /// </summary>
        public CallbackClient Client { get; set; }
        
        /// <summary>
        /// Объект, содержащий информацию о графике платежей.
        /// </summary>
        public List<CallbackSchedule> Schedule { get; set; }
    }

    public class CallbackClient
    {
        /// <summary>
        /// Номер телефона клиента 10 цифр (без кода страны).
        /// </summary>
        public string PrimaryPhone { get; set; }
        
        /// <summary>
        /// Email клиента.
        /// </summary>
        public string Email { get; set; }
        
        /// <summary>
        /// ФИО через пробел.
        /// </summary>
        public string FullName { get; set; }
        
        /// <summary>
        /// Имя клиента.
        /// </summary>
        public string FirstName { get; set; }
        
        /// <summary>
        /// Фамилия клиента.
        /// </summary>
        public string Surname { get; set; }
        
        /// <summary>
        /// Отчество клиента.
        /// </summary>
        public string Patronymic { get; set; }
    }

    public class CallbackSchedule
    {
        /// <summary>
        /// Дата платежа
        /// </summary>
        [JsonConverter(typeof(DateFormatConverter), "dd.MM.yyyy")]
        public DateTime? Date { get; set; }
        
        /// <summary>
        /// Сумма платежа в рублях.
        /// </summary>
        public float Amount { get; set; }
    }

    #endregion Checkout Callback

    #region Schedule

    public class ScheduleParameters
    {
        /// <summary>
        /// Сумма к оплате частями в рублях.
        /// </summary>
        public float Amount { get; set; }
    }

    public class ScheduleResponse : BaseResponse
    {
        /// <summary>
        /// Массив объектов, содержащий информацию о предварительных графиках платежей
        /// </summary>
        public List<PaymentSchedule> PaymentSchedule { get; set; }
    }

    public class PaymentSchedule
    {
        /// <summary>
        /// Полная сумма рассрочки с учётом переплаты.
        /// </summary>
        public float Total { get; set; }
        
        /// <summary>
        /// Приблизительная величина ежемесячного платежа с учётом переплаты.
        /// </summary>
        public float MonthlyPayment { get; set; }
        
        /// <summary>
        /// Величина ежемесячной переплаты в рублях.
        /// </summary>
        public float MonthlyOverpayment { get; set; }
        
        /// <summary>
        /// Срок рассрочки в месяцах.
        /// </summary>
        public int Term { get; set; }
        
        /// <summary>
        /// Объект, содержащий информацию о графике платежей.
        /// </summary>
        public List<PaymentDate> PaymentDates { get; set; }
    }

    public class PaymentDate
    {
        /// <summary>
        /// Дата платежа
        /// </summary>
        [JsonConverter(typeof(DateFormatConverter), "dd.MM.yyyy")]
        public DateTime? Date { get; set; }
        
        /// <summary>
        /// Сумма платежа в рублях.
        /// </summary>
        public float Amount { get; set; }
    }
    
    #endregion Schedule

    #region GetStatus

    public class StatusParameters
    {
        public string OrderId { get; set; }
    }

    public class StatusResponse : BaseResponse
    {
        /// <summary>
        /// Объект, содержащий информацию о заказе.
        /// </summary>
        public StatusOrder CurrentOrder { get; set; }
    }

    public class StatusOrder
    {
        /// <summary>
        /// Уникальный номер заказа.
        /// </summary>
        public string OrderId { get; set; }
        
        /// <summary>
        /// Флаг, отображающий статус актуальности заказа (холдирования средств).
        /// Для актуальных заказов - false. Становится true при наступлении срока valid_till.
        /// </summary>
        public bool Expired { get; set; }
        
        /// <summary>
        /// Информация по статусу заказа.
        /// <value>pending, hold, finished, canceled, declined, refunded</value>
        /// </summary>
        public string Status { get; set; }
        
        /// <summary>
        /// Информация по статусу лимита. Если лимит одобрен - approved. Если в лимите отказано - declined.
        /// </summary>
        public string Decision { get; set; }
        
        /// <summary>
        /// Сумма, оформленная клиентом для оплаты частями, в рублях.
        /// </summary>
        public float Amount { get; set; }
        
        /// <summary>
        /// Срок рассрочки в месяцах.
        /// </summary>
        public int Term { get; set; }
    }
    
    #endregion GetStatus

    #region Cancel

    public class CancelParameters
    {
        /// <summary>
        /// Уникальный номер заказа
        /// </summary>
        public string OrderId { get; set; }
    }

    public class CancelResponse : BaseResponse { }

    #endregion Cancel
    
    #region Finish

    public class FinishParameters
    {
        /// <summary>
        /// Уникальный номер заказа
        /// </summary>
        public string OrderId { get; set; }
        
        /// <summary>
        /// Сумма в рублях.
        /// </summary>
        public float Amount { get; set; }
        
        /// <summary>
        /// Номер фискального документа в системе партнёра (например, номер чека).
        /// </summary>
        public string CheckNumber { get; set; }
        
        /// <summary>
        /// Ссылка на чек на платформе ОФД
        /// </summary>
        public string CheckLink { get; set; }
    }
    
    public class FinishResponse: BaseResponse { }

    #endregion Finish

    #region Return

    public class ReturnParameters
    {
        /// <summary>
        /// Уникальный номер заказа
        /// </summary>
        public string OrderId { get; set; }
        
        /// <summary>
        /// Сумма возврата в рублях
        /// </summary>
        public float Amount { get; set; }
    }

    public class ReturnResponse : BaseResponse { }

    #endregion Return

    #region Limit

    public class LimitParameters
    {
        /// <summary>
        /// Объект, содержащий информацию о клиенте.
        /// </summary>
        public LimitClient Client { get; set; }
    }

    public class LimitClient
    {
        /// <summary>
        /// Номер телефона клиента 10 цифр (без кода страны).
        /// </summary>
        public string MobilePhone { get; set; }
    }

    public class LimitResponse : BaseResponse
    {
        public LimitClientInfo Client { get; set; }
    }
    
    public class LimitClientInfo
    {
        /// <summary>
        /// Номер телефона клиента 10 цифр (без кода страны).
        /// </summary>
        public string MobilePhone { get; set; }
        
        /// <summary>
        /// Лимит средств, доступных клиенту, в рублях.
        /// </summary>
        public float? LimitAmount { get; set; }
        
        /// <summary>
        /// Статус пользователя
        /// </summary>
        public string Status { get; set; }
    }
    
    #endregion
}