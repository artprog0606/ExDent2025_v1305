using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AdvantShop.Core.Services.Payment.BePaid
{
    [JsonConverter(typeof(BePaidServiceResponseConverter))]
    public class BePaidServiceResponse<T>
        where T : class, new()
    {
        public BePaidServiceResponse(T result)
        {
            Result = result;
        }

        public BePaidServiceResponse(string error)
        {
            Error = error;
        }

        public T Result { get; set; }
        public string Error { get; set; }
    }

    #region Checkout

    public class CreateCheckoutResult
    {
        public CheckoutResult Checkout { get; set; }
    }

    public class CheckoutResult
    {
        public string Token { get; set; }
        public string RedirectUrl { get; set; }
    }

    public class CheckoutContainer
    {
        public Checkout Checkout { get; set; }
    }

    public class Checkout
    {
        public float Version
        {
            get { return 2.1f; }
        }

        public bool Test { get; set; }

        /// <summary>
        /// тип транзакции
        /// </summary>
        public TransactionType TransactionType { get; set; }

        /// <summary>
        /// Количество попыток для оплаты
        /// </summary>
        public int? Attempts { get; set; }

        public CheckoutOrder Order { get; set; }
        public CheckoutSettings Settings { get; set; }
        public CheckoutCustomer Customer { get; set; }
        public CheckoutPaymentMethod PaymentMethod { get; set; }
    }

    public class CheckoutPaymentMethod
    {
        public HashSet<string> Types { get; set; }
        public CheckoutEripPayment Erip { get; set; }
    }

    public class CheckoutEripPayment
    {
        /// <summary>
        /// 12 циферный номер заказа в системе торговца. Идентификационный номер заказа в системе для отслеживания оплаченного заказа после получения подтверждения
        /// </summary>
        public string OrderId { get; set; }

        /// <summary>
        /// номер заказа/услуги/договора, для оплаты через ЕРИП. Данное значение необходимо ввести покупателю, чтобы начать процесс оплаты заказа/услуги/договора через ЕРИП. Если будет прислан запрос на оплату с account_number, который уже есть в системе, то предыдущее требование на оплату получит статус expired
        /// <para>string(30)</para>
        /// </summary>
        public string AccountNumber { get; set; }

        /// <summary>
        /// код услуги ЕРИП, присвоенный bePaid. Передается, если у вас к одному магазину подключено несколько ЕРИП услуг
        /// <para>integer(8)</para>
        /// </summary>
        public int? ServiceNo { get; set; }

        /// <summary>
        /// массив строк информации, которая будет показана плательщику перед подтверждением оплаты через ЕРИП и описывающая за что происходит оплата. Например, Оплата по договору 123 за январь
        /// </summary>
        public List<string> ServiceInfo { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum TransactionType
    {
        [EnumMember(Value = "payment")] Payment,
        [EnumMember(Value = "authorization")] Authorization,
        [EnumMember(Value = "tokenization")] Tokenization
    }

    public class CheckoutCustomer
    {
        public string Address { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }

    public class CheckoutOrder
    {
        /// <summary>
        /// валюта в ISO-4217 формате, например USD
        /// </summary>
        public string Currency { get; set; }
        /// <summary>
        /// стоимость в минимальных денежных единицах. Например, $32.45 должна быть отправлена как 3245
        /// </summary>
        public long Amount { get; set; }

        /// <summary>
        /// описание заказа
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// id транзакции или заказа в вашей системе.
        /// </summary>
        public string TrackingId { get; set; }
    }

    public class CheckoutSettings
    {
        public string SuccessUrl { get; set; }
        public string DeclineUrl { get; set; }
        public string FailUrl { get; set; }
        public string CancelUrl { get; set; }
        public string NotificationUrl { get; set; }
        public string Language { get; set; }
        //public CustomerFields CustomerFields { get; set; }
    }

    public class CustomerFields
    {
        public List<string> Visible { get; set; }
        public List<string> ReadOnly { get; set; }
    }
    #endregion

    #region NotifyData

    public class NotifyData
    {
        public NotifyTransaction Transaction { get; set; }
    }

    public class NotifyTransaction
    {
        //public TransactionPayment Payment { get; set; }
        public string Id { get; set; }
        public string Uid { get; set; }
        public string Status { get; set; }
        public bool Test { get; set; }
        public string Currency { get; set; }
        public string TrackingId { get; set; }
        public string Type { get; set; }
        public string PaymentMethodType { get; set; }
    }

    //public class TransactionPayment
    //{
    //    public long GatewayId { get; set; }
    //    public string Status { get; set; }
    //}


    #endregion

    #region GatewayTransaction

    public class GatewayTransactions
    {
        public List<GatewayTransaction> Transactions { get; set; }
    }

    public class GatewayTransactionContainer
    {
        public GatewayTransaction Transaction { get; set; }
    }

    public class GatewayTransaction
    {
        public string Uid { get; set; }
        public string Status { get; set; }
        public long Amount { get; set; }
        public string Currency { get; set; }
        public string Type { get; set; }
        public string TrackingId { get; set; }
        public string Id { get; set; }
    }

    #endregion

    #region ApiTransaction

    public class ApiTransactionContainer
    {
        public ApiTransaction Transaction { get; set; }
    }

    public class ApiTransaction
    {
        public string Uid { get; set; }
        public string Status { get; set; }
        public long Amount { get; set; }
        public string Currency { get; set; }
        public string Type { get; set; }
        public string TrackingId { get; set; }
        public string Id { get; set; }
    }

    #endregion
}
