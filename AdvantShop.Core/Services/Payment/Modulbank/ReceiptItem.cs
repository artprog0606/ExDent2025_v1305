using Newtonsoft.Json;

namespace AdvantShop.Core.Services.Payment.Modulbank
{
    public class ReceiptItem
    {
        /// <summary>
        /// Наименование товара
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string Name { get; set; }

        /// <summary>
        /// Цена за единицу товара (рубли, в формате 10.99)
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public double Price { get; set; }

        /// <summary>
        /// Количество
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public double Quantity { get; set; }

        /// <summary>
        /// Система налогообложения.
        /// </summary>
        [JsonProperty("sno", Required = Required.Always)]
        public string Taxation { get; set; }

        /// <summary>
        /// Ставка НДС
        /// </summary>
        [JsonProperty("vat", Required = Required.Always)]
        public string Tax { get; set; }

        /// <summary>
        /// Метод платежа.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string PaymentMethod { get; set; }

        /// <summary>
        /// Предмет расчета.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string PaymentObject { get; set; }

    }
}
