using System;

namespace AdvantShop.Web.Admin.Models.Analytics
{
    public class AnalyticsModel
    {
        public string Type { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public bool? Paid { get; set; }
        public int? OrderStatus { get; set; }
        public bool UseShippingCost { get; set; }
        public string GroupFormatString { get; set; }
        public DateTime? DeliveryDateFrom { get; set; }
        public DateTime? DeliveryDateTo { get; set; }

        public void ValidateDates()
        {
            var now = DateTime.Now.Date;
            
            if (DateFrom == DateTime.MinValue)
                DateFrom = new DateTime(1900, 1, 1);

            DateTo =
                DateTo == DateTime.MinValue
                    ? new DateTime(now.Year, now.Month, now.Day, 23, 59, 59)
                    : new DateTime(DateTo.Year, DateTo.Month, DateTo.Day, 23, 59, 59);
            
            if (DeliveryDateFrom.HasValue && DeliveryDateFrom == DateTime.MinValue)
                DeliveryDateFrom = now.AddMonths(-1);

            if (DeliveryDateTo.HasValue)
                DateTo =
                    DeliveryDateTo == DateTime.MinValue
                        ? new DateTime(now.Year, now.Month, now.Day, 23, 59, 59)
                        : new DateTime(DeliveryDateTo.Value.Year, DeliveryDateTo.Value.Month, DeliveryDateTo.Value.Day, 23, 59, 59);
        }
    }
}
