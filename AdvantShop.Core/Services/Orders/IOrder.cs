//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using AdvantShop.Taxes;

namespace AdvantShop.Orders
{
    public interface IOrder
    {
        int OrderID { get; set; }

        string Number { get; set; }

        Guid Code { get; set; }

        bool Payed { get; }
        
        bool IsDraft { get; set; }

        bool IsFromAdminArea { get; set; }

        string ShippingMethod { get; }
        string PaymentMethodName { get; }
        string ShippingMethodName { get; }

        string ArchivedPaymentName { get; set; }

        int ShippingMethodId { get; set; }

        int PaymentMethodId { get; set; }

        float OrderDiscount { get; set; }
        float OrderDiscountValue { get; set; }

        DateTime OrderDate { get; set; }

        DateTime? PaymentDate { get; set; }

        string CustomerComment { get; set; }

        string StatusComment { get; set; }

        string AdminOrderComment { get; set; }

        bool Decremented { get; set; }

        float ShippingCost { get; set; }

        float ShippingCostWithDiscount { get; }

        TaxType ShippingTaxType { get; set; }

        float PaymentCost { get; set; }

        float BonusCost { get; set; }

        float DiscountCost { get; set; }

        float TaxCost { get; set; }

        int OrderStatusId { get; set; }

        float Sum { get; set; }

        string GroupName { get; set; }

        float GroupDiscount { get; set; }

        OrderCertificate Certificate { get; set; }

        OrderCoupon Coupon { get; set; }

        DateTime ModifiedDate { get; set; }

        bool ManagerConfirmed { get; set; }

        int OrderSourceId { get; set; }

        DateTime? DeliveryDate { get; set; }

        string DeliveryTime { get; set; }

        string TrackNumber { get; set; }

        int? LeadId { get; set; }


        List<OrderItem> OrderItems { get; set; }

        OrderCurrency OrderCurrency { get; set; }
        
        IOrderCustomer GetOrderCustomer();

        IOrderStatus GetOrderStatus();
    }
}