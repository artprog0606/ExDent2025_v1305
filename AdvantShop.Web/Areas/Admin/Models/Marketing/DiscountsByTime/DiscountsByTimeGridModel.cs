using Newtonsoft.Json;
using System;

namespace AdvantShop.Web.Admin.Models.Marketing.DiscountsByTime
{
    public class DiscountsByTimeGridModel
    {
        public int Id { get; set; }
        public bool Enabled { get; set; }
        public DateTime TimeFrom { get; set; }
        public DateTime TimeTo { get; set; }
        public string Time => $"{TimeFrom:HH:mm}-{TimeTo:HH:mm}";
        public float Discount { get; set; }
        public int SortOrder { get; set; }
    }
}
