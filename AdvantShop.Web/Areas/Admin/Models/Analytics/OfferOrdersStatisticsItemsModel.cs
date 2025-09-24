using AdvantShop.Core.Services.Catalog;
using AdvantShop.Localization;
using System;

namespace AdvantShop.Web.Admin.Models.Analytics
{
    public class OfferOrdersStatisticsItemsModel
    {
        public int OrderId { get; set; }
        public string Number { get; set; }
        public bool IsPaid { get; set; }

        public float Sum { get; set; }
        public string SumFormatted
        {
            get
            {
                return PriceFormatService.FormatPrice(Sum, CurrencyValue, CurrencySymbol, CurrencyCode, IsCodeBefore);
            }
        }

        public string CurrencyCode { get; set; }
        public float CurrencyValue { get; set; }
        public string CurrencySymbol { get; set; }
        public bool IsCodeBefore { get; set; }

        public DateTime OrderDate { get; set; }
        public string OrderDateFormatted
        {
            get { return Culture.ConvertDate(OrderDate); }
        }

        public Guid CustomerId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Organization { get; set; }
        public string BuyerName { get; set; }
        public float OfferAmount { get; set; }
        public string Email { get; set; }
    }
}
