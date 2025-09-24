//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Collections.Generic;
using System.Runtime.Serialization;
using AdvantShop.Core.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AdvantShop.Core.Services.Payment.Kupivkredit
{
    public partial class TinkoffCreateOrder
    {
        /// <summary>
        /// Идентификатор магазина
        /// </summary>
        public string ShopId { get; set; }

        /// <summary>
        /// Идентификатор витрины(сайта)
        /// </summary>
        public string ShowcaseId { get; set; }

        /// <summary>
        /// Общая сумма заказа
        /// </summary>
        public double Sum { get; set; }

        /// <summary>
        /// Список товаров
        /// </summary>
        public List<Item> Items { get; set; }

        /// <summary>
        /// Идентификатор заказа в системе партнера
        /// </summary>
        public string OrderNumber { get; set; }

        /// <summary>
        /// Идентификатор кредитного продукта(кредит/рассрочка)
        /// </summary>
        public string PromoCode { get; set; }

        /// <summary>
        /// Доступные флоу для демо заявки
        /// </summary>
        public EnDemoFlow? DemoFlow { get; set; }

        /// <summary>
        /// URL для возврата в случае неуспешного завершения заявки, если не указан то будет использован URL из настроек магазина
        /// </summary>
        public string FailUrl { get; set; }

        /// <summary>
        /// URL для возврата в случае успешного завершения заявки, если не указан то будет использован URL из настроек магазина
        /// </summary>
        public string SuccessUrl { get; set; }

        /// <summary>
        /// Данные клиента для предзаполнения формы
        /// </summary>
        public Values Values { get; set; }
    }

    public partial class Item
    {
        /// <summary>
        /// Название товарной позиции
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Артикул
        /// </summary>
        public string VendorCode { get; set; }

        /// <summary>
        /// Количество единиц товара в позиции
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Цена одной единицы товара
        /// </summary>
        public double Price { get; set; }

        /// <summary>
        /// Категория товара
        /// </summary>
        public string Category { get; set; }
    }

    public partial class Values
    {
        /// <summary>
        /// Контакт
        /// </summary>
        public Contact Contact { get; set; }
    }

    public partial class Contact
    {
        /// <summary>
        /// ФИО клиента
        /// </summary>
        public Fio Fio { get; set; }

        /// <summary>
        /// Телефон клиента(10 цифр, без +7)
        /// </summary>
        public string MobilePhone { get; set; }

        /// <summary>
        /// Email клиента
        /// </summary>
        public string Email { get; set; }
    }

    public partial class Fio
    {
        /// <summary>
        /// Фамилия клиента
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Имя клиента
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Отчество клиента
        /// </summary>
        public string MiddleName { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum EnDemoFlow
    {
        [EnumMember(Value = "sms")] Sms,
        [EnumMember(Value = "appointment")] Appointment,
        [EnumMember(Value = "reject")] Reject,
    }

    public class TinkoffCreateOrderResponse
    {
        /// <summary>
        /// ID заявки в TCB
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Ссылка на заявку в TCB
        /// </summary>
        public string Link { get; set; }
    }

    public class TinkoffOrderInfo
    {
        /// <summary>
        /// Идентификатор заказа
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Текущий статус заявки
        /// </summary>
        public EnOrderStatus Status { get; set; }

        /// <summary>
        /// Дата и время создания заявки
        /// </summary>
        public string CreatedAt { get; set; }

        /// <summary>
        /// Флаг показывающий реальная ли это заявка или заявка созданная в демо-режиме
        /// </summary>
        public bool Demo { get; set; }

        /// <summary>
        /// Флаг показывающий подтверждена ли данная заявка
        /// </summary>
        public bool Committed { get; set; }

        /// <summary>
        /// Сумма первого платежа по кредиту
        /// </summary>
        public double FirstPayment { get; set; }

        /// <summary>
        /// Сумма заказа
        /// </summary>
        public double OrderAmount { get; set; }

        /// <summary>
        /// Сумма выдаваемого клиенту кредита
        /// </summary>
        public double CreditAmount { get; set; }

        /// <summary>
        /// Тип продукта
        /// </summary>
        public EnTypeProduct Product { get; set; }

        /// <summary>
        /// Количество месяцев, на которое оформляется кредит
        /// </summary>
        public long Term { get; set; }

        /// <summary>
        /// Сумма ежемесячного платежа
        /// </summary>
        public double MonthlyPayment { get; set; }

        /// <summary>
        /// Имя клиента
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Фамилия клиента
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Отчество клиента
        /// </summary>
        public string MiddleName { get; set; }

        /// <summary>
        /// Телефон клиента
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Номер договора
        /// </summary>
        public string LoanNumber { get; set; }
    }


    public class EnOrderStatus : StringEnum<EnOrderStatus>
    {
        public EnOrderStatus(string value) : base(value) { }

        public static EnOrderStatus Inprogress { get { return new EnOrderStatus("inprogress"); } }

        /// <summary>
        /// Договор подписан клиентом через СМС или на встрече с представителем банка. Вы можете выдать товар клиенту или оказать купленную услугу
        /// </summary>
        public static EnOrderStatus Signed { get { return new EnOrderStatus("signed"); } }

        public static EnOrderStatus Issued { get { return new EnOrderStatus("issued"); } }

        /// <summary>
        /// Заявка отменена. Клиент по какой-то причине отменил заказ
        /// </summary>
        public static EnOrderStatus Canceled { get { return new EnOrderStatus("canceled"); } }

        public static EnOrderStatus New { get { return new EnOrderStatus("new"); } }

        /// <summary>
        /// По заявке отказ. Вы можете связаться с клиентом и предложить альтернативные способы оплаты - кредитная карта, наличные
        /// </summary>
        public static EnOrderStatus Rejected { get { return new EnOrderStatus("rejected"); } }

        /// <summary>
        /// Заявка одобрена. Клиенту остается подписать документы по СМС или на встрече с представителем банка
        /// </summary>
        public static EnOrderStatus Approved { get { return new EnOrderStatus("approved"); } }
    }

    public class EnTypeProduct : StringEnum<EnTypeProduct>
    {
        public EnTypeProduct(string value) : base(value) { }

        public static EnTypeProduct Credit { get { return new EnTypeProduct("credit"); } }
        public static EnTypeProduct InstallmentCredit { get { return new EnTypeProduct("installment_credit"); } }
    }

    public class TinkoffNotifyData
    {
        public string Id { get; set; }
        public EnOrderStatus Status { get; set; }
        public string CreatedAt { get; set; }
        public bool Demo { get; set; }
        public bool Committed { get; set; }
        public double FirstPayment { get; set; }
        public double OrderAmount { get; set; }
        public double CreditAmount { get; set; }
        public EnTypeProduct Product { get; set; }
        public int Term { get; set; }
        public double MonthlyPayment { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Phone { get; set; }
    }
}
