using System.Collections.Generic;
using Newtonsoft.Json;

namespace AdvantShop.Core.Services.Payment.Rbkmoney2
{
    public enum EInvoiceStatus
    {
        None,
        Unpaid,     // Инвойс создан, но финансовые обязательства ещё не погашены
        Cancelled,  // Инвойс отменён с указанием причины, все обязательства по нему недействительны.
        Paid,       // Финансовые обязательства по инвойсу погашены, но товары или услуги плательщику ещё не предоставлены.
        Refunded,   // Все обязательства, как плательщика, так и мерчанта, погашены.
        Fulfilled   // Мерчанту не удалось выполнить свои обязательства, и финансовые обязательства плательщика были возмещены.
    }

    public class InvoiceModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("shopID")]
        public string ShopId { get; set; }

        /// <summary>
        /// Дата и время окончания действия инвойса, после наступления которых его уже невозможно будет оплатить, ISO 8601
        /// </summary>
        [JsonProperty("dueDate")]
        public string DueDate { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        /// <summary>
        /// Наименование предлагаемых товаров или услуг
        /// </summary>
        [JsonProperty("product")]
        public string Product { get; set; }

        [JsonProperty("cart")]
        public List<CartItemModel> Cart { get; set; }

        /// <summary>
        /// Метаданные, которые необходимо связать с инвойсом
        /// </summary>
        [JsonProperty("metadata")]
        public MetadataModel Metadata { get; set; }

        /// <summary>
        /// Статус инвойса
        /// </summary>
        [JsonProperty("status")]
        public EInvoiceStatus Status { get; set; }
    }

    public class CartItemModel
    {
        [JsonProperty("product")]
        public string Product { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        /// <summary>
        /// Цена предлагаемого товара или услуги, в минорных денежных единицах
        /// </summary>
        [JsonProperty("price")]
        public int Price { get; set; }

        /// <summary>
        /// Схема налогообложения предлагаемого товара или услуги. 
        /// Указывается, только если предлагаемый товар или услуга облагается налогом.
        /// </summary>
        [JsonProperty("taxMode", NullValueHandling = NullValueHandling.Ignore)]
        public TaxModeModel TaxMode { get; set; }
    }

    public class TaxModeModel
    {
        /// <summary>
        /// Тип схемы налогообложения
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// Ставка налога ("0%" "10%" "18%" "10/110" "18/118")
        /// </summary>
        [JsonProperty("rate")]
        public string Rate { get; set; }
    }

    public class MetadataModel
    {
        public int OrderId { get; set; }
    }
}