using AdvantShop.Orders;
using System;

namespace AdvantShop.Core.Services.Webhook.Models
{
    public class ExternalWebhookOrderModel
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public float Sum { get; set; }

        public int ShippingMethodID { get; set; }
        public string ShippingMethodName { get; set; }
        public int ShippingTaxType { get; set; }
        public float ShippingCost { get; set; }

        public int PaymentMethodID { get; set; }
        public string PaymentMethodName { get; set; }
        public DateTime PaymentDate { get; set; }
        public float PaymentCost { get; set; }

        public int AffiliateID { get; set; }
        public float OrderDiscount { get; set; }
        public DateTime OrderDate { get; set; }

        public string GroupName { get; set; }
        public float GroupDiscount { get; set; }
        public string CustomerComment { get; set; }
        public string StatusComment { get; set; }
        public string AdditionalTechInfo { get; set; }
        public string AdminOrderComment { get; set; }
        public bool Decremented { get; set; }

        public float TaxCost { get; set; }
        public float SupplyTotal { get; set; }
        public int OrderStatusID { get; set; }
        public int ShippingContactID { get; set; }
        public int BillingContactID { get; set; }


        public string CertificateCode { get; set; }
        public float CertificatePrice { get; set; }
        public string CouponCode { get; set; }
        public int CouponType { get; set; }
        public float CouponValue { get; set; }

        public float BonusCost { get; set; }
        public float DiscountCost { get; set; }
        public int UseIn1C { get; set; }
        public DateTime ModifiedDate { get; set; }
        public Guid Code { get; set; }
        public bool ManagerConfirmed { get; set; }
        public int ManagerId { get; set; }
        public string PreviousStatus { get; set; }
        public int BonusCardNumber { get; set; }
        public int OrderSourceId { get; set; }
        public string CustomData { get; set; }
        public string TrackNumber { get; set; }
        public bool IsDraft { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string DeliveryTime { get; set; }
        public bool IsFromAdminArea { get; set; }
        public float OrderDiscountValue { get; set; }
        public int LeadId { get; set; }



        public static explicit operator ExternalWebhookOrderModel(Order order)
        {
            return new ExternalWebhookOrderModel
            {
                Id = order.OrderID,
                Number = order.Number,
                Sum = order.Sum
            };
        }
    }
}
