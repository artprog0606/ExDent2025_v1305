using System;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Partners;

namespace AdvantShop.Areas.Partners.Models.Accruals
{
    public class AccrualModel : BaseTransactionModel
    {
        public string Email { get; set; }
        public string EmailAnonimized { get { return Email.AnonimizeEmail(); } }
        public string Phone { get; set; }
        public string PhoneAnonimized { get { return Phone.AnonimizePhone(); } }
        public DateTime DateCreated { get; set; }

        public decimal Amount { get; set; }
        public string AmountFormatted { get { return PriceFormatService.FormatPrice((float)Amount, Currency); } }
    }
}