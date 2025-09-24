using System.Collections.Generic;
using Newtonsoft.Json;

namespace AdvantShop.Web.Admin.Models.Orders.OrdersEdit
{
    public class OrderItemModel
    {
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public string ImageSrc { get; set; }
        public string ArtNo { get; set; }
        public string BarCode { get; set; }
        public string Name { get; set; }
        public string ProductLink { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public float Price { get; set; }
        public string PriceString => string.Format(Price%1 == 0 ? "{0:### ### ##0.##}" : "{0:### ### ##0.00##}", Price).Trim();

        public float Amount { get; set; }
        public bool Available { get; set; }
        public List<string> AvailableText { get; set; } = new List<string>();
        public List<string> StocksText { get; set; }
        public string Cost { get; set; }
        public string CustomOptions { get; set; }
        public bool ShowEditCustomOptions { get; set; }

        public float Length { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public float Weight { get; set; }
        public bool Enabled { get; set; }
        public int? ProductId { get; set; }
        public bool? MarkingRequiredValidation { get; set; }

        [JsonIgnore]
        public bool IsMarkingRequired { get; set; }
        
        public bool IsCustomPrice { get; set; }
        public float? BasePrice { get; set; }
        public float DiscountPercent { get; set; }
        public float DiscountAmount { get; set; }
        public string PriceWhenOrdering { get; set; }
        public string DiscountWhenOrdering { get; set; }
        
        public string UnitName { get; set; }
        public string MeasureName { get; set; }
        public string PaymentSubjectName { get; set; }
        public string PaymentMethodName { get; set; }
        
        public string DownloadLink { get; set; }
        public int CountWarehouses { get; set; }
    }
}
