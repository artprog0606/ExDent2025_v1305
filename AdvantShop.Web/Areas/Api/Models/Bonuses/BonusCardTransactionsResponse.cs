using System;
using System.Collections.Generic;
using AdvantShop.Core.Services.Api;
using AdvantShop.Core.Services.Bonuses.Model;

namespace AdvantShop.Areas.Api.Models.Bonuses
{
    public sealed class BonusCardTransactionsResponse : List<BonusCardTransaction>, IApiResponse
    {
        public BonusCardTransactionsResponse(List<BonusCardTransaction> transactions)
        {
            if (transactions != null)
                this.AddRange(transactions);
        }
    }

    public sealed class BonusCardTransaction
    {
        public string Type { get; }
        public decimal Amount { get; }
        public string Basis { get; }
        public string DateTime { get; }
        
        public BonusCardTransaction(UnitedTransaction transaction)
        {
            Type =
            (
                (int)transaction.OperationType % 2 == 0
                    ? BonusCardTransactionType.Subtraction
                    : BonusCardTransactionType.Adding
            ).ToString();
            
            Amount = (int)transaction.OperationType % 2 == 0 ? -transaction.Amount : transaction.Amount;
            Basis = transaction.Basis;

            var date = System.DateTime.Now.Date;
            
            DateTime = transaction.CreateOn.Date == date
                ? $"Сегодня {transaction.CreateOn:H:mm}"
                : transaction.CreateOn.Date == date.AddDays(-1)
                    ? $"Вчера {transaction.CreateOn:H:mm}"
                    : transaction.CreateOn.ToString("d.MM.yyyy H:mm");
        }
    }

    public enum BonusCardTransactionType
    {
        Adding,
        Subtraction
    }
}