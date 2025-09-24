using System;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Partners;
using AdvantShop.Localization;

namespace AdvantShop.Web.Admin.Models.Partners
{
    public class PartnerTransactionModel : BaseTransactionModel
    {
        private decimal Amount { get; set; }
        private decimal Balance { get; set; }
        private DateTime DateCreated { get; set; }

        public string Added
        {
            get { return Amount > 0 ? PriceFormatService.FormatPrice((float)Amount, Currency) : "-"; }
        }
        public string Subtracted
        {
            get { return Amount < 0 ? PriceFormatService.FormatPrice(-(float)Amount, Currency) : "-"; }
        }

        public string BalanceFormatted
        {
            get { return PriceFormatService.FormatPrice((float)Balance, Currency); }
        }
        public string Basis { get; set; }
        public Guid? CustomerId { get; set; }
        public string CustomerName { get; set; }
        public int? OrderId { get; set; }
        public string OrderNumber { get; set; }

        public string DateCreatedFormatted
        {
            get { return Culture.ConvertDate(DateCreated); }
        }
    }
}
