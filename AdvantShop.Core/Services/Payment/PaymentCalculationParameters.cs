using System;
using System.Collections.Generic;
using AdvantShop.Customers;
using AdvantShop.Orders;
using AdvantShop.Payment;
using AdvantShop.Shipping;

namespace AdvantShop.Core.Services.Payment
{
    [Serializable]
    public class PaymentCalculationParameters
    {
        public string Country { get; set; }
        public string Region { get; set; }
        public string District { get; set; }
        public string City { get; set; }
        // public string Address { get; set; }
        public string Zip { get; set; }
        public CustomerType? CustomerType { get; set; }
        public BaseShippingOption ShippingOption { get; set; }
        public BasePaymentOption PaymentOption { get; set; }
        public List<PreOrderItem> PreOrderItems { get; set; }
        
        /// <summary>
        /// Сумма всех товаров с учетом всех скидок
        /// </summary>
        public float ItemsTotalPriceWithDiscounts { get; set; }
        
        public GiftCertificate Certificate { get; set; }
        public Guid? BonusCardId { get; set; }
        public float BonusAmount { get; set; }
    }
}