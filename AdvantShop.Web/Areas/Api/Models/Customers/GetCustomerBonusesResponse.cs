using System;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Api;
using AdvantShop.Core.Services.Bonuses.Model;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Statistic;
using AdvantShop.Customers;
using Newtonsoft.Json;

namespace AdvantShop.Areas.Api.Models.Customers
{
    public sealed class BonusCardResponse : IApiResponse
    {
        public long CardId { get; }

        public decimal Amount { get; }

        public decimal Percent { get; }

        public bool IsBlocked { get; }

        public string GradeName { get; }

        public int GradeId { get; }
        
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public BonusCardCustomer Customer { get; }

        public BonusCardResponse(Card bonusCard, Customer customer)
        {
            CardId = bonusCard.CardNumber;
            Amount = bonusCard.BonusesTotalAmount;
            Percent = bonusCard.Grade.BonusPercent;
            GradeName = bonusCard.Grade.Name;
            GradeId = bonusCard.Grade.Id;
            IsBlocked = bonusCard.Blocked;

            if (customer != null)
                Customer = new BonusCardCustomer(customer);
        }
        
        public BonusCardResponse(Card bonusCard) : this(bonusCard, null)
        {
        }
    }

    public sealed class BonusCardCustomer : GetCustomerResponse
    {
        public BonusCardCustomer(Customer customer) : base(customer)
        {
            var count = StatisticService.GetCustomerOrdersCount(customer.Id);
            var sum = count > 0 ? StatisticService.GetCustomerOrdersSum(customer.Id) : 0;

            OrdersSum = sum.FormatPrice();
            OrdersCount = count;
            AverageCheck = (count > 0 ? (sum / count) : 0).FormatPrice();

            DurationOfWorkWithClient = customer.RegistrationDateTime.GetDurationString(DateTime.Now);
        }

        [JsonProperty(Order = 201)]
        public string OrdersSum { get; }
        
        [JsonProperty(Order = 201)]
        public int OrdersCount { get; }
        
        [JsonProperty(Order = 202)]
        public string AverageCheck { get; }
        
        [JsonProperty(Order = 203)]
        public string DurationOfWorkWithClient { get; }
    }
}