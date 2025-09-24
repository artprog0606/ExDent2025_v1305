using System;
using System.Collections.Generic;
using AdvantShop.Orders;

namespace AdvantShop.Core.Modules.Interfaces
{
    public interface IDigitalProductInfo
    {
        DigitalProductInfo GetDigitalProductInfoByOrderItem(OrderItem orderItem);
    }
    
    public interface IDigitalProductInfos
    {
        IList<DigitalProductInfo> GetDigitalProductInfosByOrderItem(OrderItem orderItem);
    }

    public class DigitalProductInfo
    {
        /// <summary>
        /// Ключ цифрового товара.
        /// </summary>
        public string Code { get; set; }
        
        /// <summary>
        /// Инструкция по активации ключа
        /// </summary>
        public string Slip { get; set; }
        
        /// <summary>
        /// Дата, до которой нужно активировать ключ
        /// </summary>
        public DateTime ActivateTill { get; set; }
    }
}