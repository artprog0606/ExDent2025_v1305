using System;
using AdvantShop.Areas.Api.Models.Customers;
using AdvantShop.Core;
using AdvantShop.Core.Services.Bonuses;
using AdvantShop.Core.Services.Bonuses.Model;
using AdvantShop.Customers;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Customers
{
    public class GetCustomerBonuses : AbstractCommandHandler<BonusCardResponse>
    {
        private readonly Guid _id;
        private readonly long _cardNumber;
        private readonly bool _loadCustomer;
        private Card _bonusCard;
        private Customer _customer;

        
        public GetCustomerBonuses(Guid id)
        {
            _id = id;
            _loadCustomer = true;
        }
        
        public GetCustomerBonuses(Guid id, bool loadCustomer)
        {
            _id = id;
            _loadCustomer = loadCustomer;
        }

        public GetCustomerBonuses(long cardNumber)
        {
            _cardNumber = cardNumber;
            _loadCustomer = true;
        }

        protected override void Validate()
        {
            if (_cardNumber != 0)
            {
                _bonusCard = BonusSystemService.GetCard(_cardNumber);
                if (_bonusCard == null)
                    throw new BlException("Бонусная карта не найдена");
                
                if (_loadCustomer)
                    _customer = _bonusCard.Customer;
            }
            else
            {
                _customer = CustomerService.GetCustomer(_id);
                if (_customer == null)
                    throw new BlException("Пользователь не найден");

                if (_customer.BonusCardNumber == null)
                    throw new BlException("У покупателя нет бонусной карты");

                _bonusCard = BonusSystemService.GetCard(_customer.BonusCardNumber);
                if (_bonusCard == null)
                    throw new BlException("Бонусная карта не найдена");
            }
        }

        protected override BonusCardResponse Handle()
        {
            return new BonusCardResponse(_bonusCard, _loadCustomer ? _customer : null);
        }
    }
}