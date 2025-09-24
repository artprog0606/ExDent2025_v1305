using AdvantShop.Shipping.Yandex.Api;
using System.Collections.Generic;

namespace AdvantShop.Shipping.Yandex
{
    public class YandexDeliveryCalculateOption
    {
        public bool IsCourier { get; set; }
        /// <summary>
        /// Указанный покупателем интервал доставки
        /// </summary>
        public string IntervalFrom { get; set; }
        /// <summary>
        /// Указанный покупателем интервал доставки
        /// </summary>
        public string IntervalTo { get; set; }
    }
}
