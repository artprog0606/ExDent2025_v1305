using System;
using System.Collections.Generic;

namespace AdvantShop.Web.Admin.ViewModels.Analytics
{
    public class ExportOrdersModel
    {
        public bool UseStatus { get; set; }
        
        public bool UseSource { get; set; }

        public bool UseDate { get; set; }

        public bool UseShippingDate { get; set; }

        public bool UsePaid { get; set; }

        public bool UseSum { get; set; }

        public bool UseShipping { get; set; }
        
        public bool UsePayment { get; set; }

        public bool UseCity { get; set; }
        
        public bool UseBonusCost { get; set; }

        public bool UseCouponCode { get; set; }
        public bool UseOrderItemsInString { get; set; } = true;


        public int Status { get; set; }
        
        public int Source { get; set; }

        public string Encoding { get; set; }

        public DateTime DateFrom { get; set; }

        public DateTime DateTo { get; set; }

        public DateTime ShippingDateFrom { get; set; }

        public DateTime ShippingDateTo { get; set; }


        public bool Paid { get; set; }

        public int Shipping { get; set; }
        
        public int Payment { get; set; }

        public float OrderSumFrom { get; set; }

        public float OrderSumTo { get; set; }

        public string City { get; set; }

        public string CouponCode { get; set; }

        public string CustomOptionOptionsSeparator { get; set; } = ":";

        public Dictionary<string, string> Encodings { get; set; }

        public Dictionary<int, string> OrderStatuses { get; set; }
        
        public Dictionary<int, string> OrderSources { get; set; }

        public Dictionary<int, string> Shippings { get; set; }

        public Dictionary<bool, string> PaidStatuses { get; set; }
        
        public Dictionary<int, string> Payments { get; set; }
    }
}
