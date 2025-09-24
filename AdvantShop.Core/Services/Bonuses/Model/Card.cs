using System;
using System.Collections.Generic;
using AdvantShop.Core.Services.Bonuses.Service;
using AdvantShop.Customers;
using Newtonsoft.Json;

namespace AdvantShop.Core.Services.Bonuses.Model
{
    public class Card
    {
        /// <summary>
        /// Id карты (равен CustomerId)
        /// </summary>
        public Guid CardId { get; set; }

        /// <summary>
        /// Номер бонусной карты (то что видит пользователь)
        /// </summary>
        public long CardNumber { get; set; }

        public bool Blocked { get; set; }

        public int GradeId { get; set; }

        public bool ManualGrade { get; set; }

        public DateTime CreateOn { get; set; }

        private Grade _grade;
        [JsonIgnore]
        public Grade Grade
        {
            get => _grade ?? (_grade = GradeService.Get(GradeId));
            set => _grade = value;
        }

        private Customer _customer;
        [JsonIgnore]
        public Customer Customer
        {
            get => _customer ?? (_customer = CustomerService.GetCustomer(CardId));
            set => _customer = value;
        }

        private List<Bonus> _bonusesActual;
        [JsonIgnore]
        public List<Bonus> BonusesActual => _bonusesActual ?? (_bonusesActual = BonusService.Actual(CardId));

        /// <summary>
        /// Сумма всех бонусов
        /// </summary>
        public decimal BonusesTotalAmount => BonusService.ActualSum(CardId);

        private List<Transaction> _lasttransactions;
        [JsonIgnore]
        public List<Transaction> LastTransactions => _lasttransactions ?? (_lasttransactions = TransactionService.GetLast(CardId));

        private List<Purchase> _lastPurchases;
        [JsonIgnore]
        public List<Purchase> LastPurchases => _lastPurchases ?? (_lastPurchases = PurchaseService.GetLast(CardId));
    }
}
