using System;
using System.Collections.Generic;
using AdvantShop.Core.Services.Bonuses.Model.Enums;
using AdvantShop.Core.Services.Bonuses.Service;
using Newtonsoft.Json;

namespace AdvantShop.Core.Services.Bonuses.Model
{
    public class Purchase
    {
        public int Id { get; set; }
        public Guid CardId { get; set; }
        public DateTime CreateOn { get; set; }
        public DateTime CreateOnCut { get; set; }
        public decimal PurchaseAmount { get; set; }
        public decimal CashAmount { get; set; }

        /// <summary>
        /// Сколько бонусов списали во время операции
        /// </summary>
        public decimal BonusAmount { get; set; }

        /// <summary>
        /// Сумма начисляемых бонусов
        /// </summary>
        public decimal NewBonusAmount { get; set; }

        public string Comment { get; set; }

        /// <summary>
        /// Сколько осталось бонусов после операции
        /// </summary>
        public decimal BonusBalance { get; set; }
        public EPuchaseState Status { get; set; }
        public decimal PurchaseFullAmount { get; set; }
        public int? OrderId { get; set; }

        private List<Transaction> _transaction;
        [JsonIgnore]
        public List<Transaction> Transaction
        {
            get { return _transaction ?? (_transaction = TransactionService.GetByPurchase(Id)); }
            set { _transaction = value; }
        }

        private Card _card;
        [JsonIgnore]
        public Card Card
        {
            get { return _card ?? (_card = CardService.Get(CardId)); }
            set { _card = value; }
        }

    }
}
