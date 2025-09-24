using System;
using System.Collections.Generic;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Web.Infrastructure.Admin;
using Newtonsoft.Json;

namespace AdvantShop.Web.Admin.Models.Orders
{
    public enum OrdersPreFilterType
    {
        [Localize("Admin.Models.Orders.OrdersPreFilterType.None")]
        None,
        [Localize("Admin.Models.Orders.OrdersPreFilterType.New")]
        New,
        [Localize("Admin.Models.Orders.OrdersPreFilterType.Paid")]
        Paid,
        [Localize("Admin.Models.Orders.OrdersPreFilterType.NotPaid")]
        NotPaid,
        [Localize("Admin.Models.Orders.OrdersPreFilterType.Drafts")]
        Drafts,
    }

    public class OrdersFilterModel : BaseFilterModel
    {
        public string Filter { get; set; }
        
        public string Status { get; set; }

        public string Number { get; set; }
        
        public int OrderStatusId { get; set; }
        
        public float PriceFrom { get; set; }

        public float PriceTo { get; set; }

        public string ProductNameArtNo { get; set; }

        public string BuyerName { get; set; }

        public string BuyerPhone { get; set; }

        public string BuyerEmail { get; set; }

        public string BuyerZip { get; set; }

        public string BuyerApartment { get; set; }

        public string BuyerCity { get; set; }

        public string BuyerStreet { get; set; }

        public string BuyerHouse { get; set; }

        public string BuyerStructure { get; set; }
        
        public string PaymentMethod { get; set; }

        public string ShippingMethod { get; set; }

        public bool? IsPaid { get; set; }

        public bool IsDraft { get; set; }

        public int? ManagerId { get; set; }

        public string OrderDateFrom { get; set; }

        public string OrderDateTo { get; set; }

        public int? OrderSourceId { get; set; }

        public string CouponCode { get; set; }

        public string DeliveryDateFrom { get; set; }

        public string DeliveryDateTo { get; set; }

        public OrdersPreFilterType FilterBy { get; set; }


        public Guid? CustomerId { get; set; }


        public int? StatusId { get; set; }
        
        public string EnableHiding { get; set; }

        private OrdersFilterHidingFields _fields;

        public OrdersFilterHidingFields LoadFields
        {
            get
            {
                if (string.IsNullOrWhiteSpace(EnableHiding))
                    return _fields = new OrdersFilterHidingFields();
                
                try
                {
                    _fields = JsonConvert.DeserializeObject<OrdersFilterHidingFields>(EnableHiding);
                }
                catch
                {
                }
                return _fields;
            }
        }

        public string GroupName { get; set; }
    }

    public class OrdersFilterHidingFields
    {
        public bool? OrderItems { get; set; }
        public bool? AdminOrderComment { get; set; } 
    }
}
