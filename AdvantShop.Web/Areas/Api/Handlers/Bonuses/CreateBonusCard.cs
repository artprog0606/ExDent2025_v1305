using AdvantShop.Areas.Api.Handlers.Customers;
using AdvantShop.Areas.Api.Models.Customers;
using AdvantShop.Core;
using AdvantShop.Core.Services.Bonuses;
using AdvantShop.Core.Services.Bonuses.Model;
using AdvantShop.Customers;
using AdvantShop.Web.Infrastructure.Handlers;
using System;

namespace AdvantShop.Areas.Api.Handlers.Bonuses
{
    public class CreateBonusCard : AbstractCommandHandler<BonusCardResponse>
    {
        private readonly Guid _customerId;
        private Customer _customer;

        public CreateBonusCard(Guid customerId)
        {
            _customerId = customerId;
        }

        protected override void Validate()
        {
            _customer = CustomerService.GetCustomer(_customerId);
            if (_customer == null)
                throw new BlException("Покупатель не найден");

            if (_customer.BonusCardNumber != null && BonusSystemService.GetCard(_customer.BonusCardNumber) != null)
                throw new BlException("У покупателя уже есть бонусная карта");
        }

        protected override BonusCardResponse Handle()
        {
            var card = new Card { CardId = _customer.Id };

            BonusSystemService.AddCard(card);

            return new GetCustomerBonuses(card.CardNumber).Execute();
        }
    }
}