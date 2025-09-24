using System;
using System.Collections.Generic;
using AdvantShop.Customers;

namespace AdvantShop.Core.Services.ExportImport.ExportServices.ExportCustomers
{
    public class CustomersExportSettings 
    {
        public string Encoding { get; set; }
        
        public string ColumnSeparator {get;set;}

        public string PropertySeparator { get; set; }
        
        public List<string> SelectedExportFields { get; set; }
        
        public int? GroupId { get; set; }
        public DateTime? RegistrationDateFrom { get; set; }
        public DateTime? RegistrationDateTo { get; set; }
        public int? ManagerId { get; set; }
        public int? OrdersCountFrom { get; set; }
        public int? OrdersCountTo { get; set; }
        public float? OrderSumFrom { get; set; }
        public float? OrderSumTo { get; set; }
        public int? LastOrderFrom { get; set; }
        public int? LastOrderTo { get; set; }
        public DateTime? LastOrderDateTimeFrom { get; set; }
        public DateTime? LastOrderDateTimeTo { get; set; }
        public float? AverageCheckFrom { get; set; }
        public float? AverageCheckTo { get; set; }
        public string SocialType { get; set; }
        public bool? Subscription { get; set; }
        public bool? HasBonusCard { get; set; }
        public CustomerType? CustomerType { get; set; }
        public Dictionary<string, CustomersFieldExportSettings> CustomerFields { get; set; }
        
        public class CustomersFieldExportSettings
        {
            public string Value { get; set; }
            public string ValueExact { get; set; }
            public DateTime? DateFrom { get; set; }
            public DateTime? DateTo { get; set; }
            public string From { get; set; }
            public string To { get; set; }
        }
    }
}