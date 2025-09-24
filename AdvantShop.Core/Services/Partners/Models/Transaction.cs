using System;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Catalog;

namespace AdvantShop.Core.Services.Partners
{
    public class Transaction
    {
        public int Id { get; set; }
        public int PartnerId { get; set; }
        public decimal Balance { get; set; }
        public decimal Amount { get; set; }
        public string Basis { get; set; }
        public Guid? CustomerId { get; set; }
        public int? OrderId { get; set; }
        public DateTime DateCreated { get; set; }
        public bool IsRewardPayout { get; set; }
        public DateTime? RewardPeriodTo { get; set; }
        public string DetailsJson { get; set; }

        public TransactionCurrency Currency { get; set; }

        public decimal RoundedBaseAmount
        {
            get
            {
                if (Currency == null)
                    return Amount;
                return (decimal)PriceService.SimpleRoundPrice((float)Amount * Currency.CurrencyValue, Currency);
            }
        }
    }
}
