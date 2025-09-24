
using System.Collections.Generic;
using AdvantShop.Orders;

namespace AdvantShop.ViewModel.PaymentReceipt
{
    public class BillUaViewModel
    {
        public string CompanyName { get; set; }
        public string CompanyCode { get; set; }
        public string Credit { get; set; }
        public string CompanyEssencials { get; set; }
        public string BuyerInfo { get; set; }
        public string BankCode { get; set; }
        public string BankName { get; set; }
        public string OrderNumber { get; set; }
        public string TotalCount { get; set; }
        public string Discount { get; set; }
        public string TaxSum { get; set; }
        public string TaxSumPartPrice { get; set; }
        public string Total { get; set; }
        public string TotalPartPrice { get; set; }
        //public string Tax { get; set; }
        public string ShippingCost { get; set; }
        public string PaymentCost { get; set; }

        public List<OrderItem> OrderItems { get; set; }
        public List<AdvantShop.Orders.GiftCertificate> OrderCertificates { get; set; }
        public OrderCurrency OrderCurrency { get; set; }
    }
}