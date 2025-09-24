using AdvantShop.Core.Services.Bonuses;
using AdvantShop.Customers;
using AdvantShop.Orders;
using AdvantShop.ViewModel.Checkout;
using AdvantShop.Repository.Currencies;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Auth;

namespace AdvantShop.Handlers.Checkout
{
    public class CheckoutUserHandler
    {
        private readonly bool? _isLanding;
        private readonly bool? _isApi;

        public CheckoutUserHandler(bool? isLanding, bool? isApi)
        {
            _isLanding = isLanding;
            _isApi = isApi;
        }

        public CheckoutUserViewModel Execute()
        {
            var current = MyCheckout.Factory(CustomerContext.CustomerId);

            var model = new CheckoutUserViewModel()
            {
                Customer = CustomerContext.CurrentCustomer,
                Data = current.Data,
                Currency = CurrencyService.CurrentCurrency,
                IsLanding = _isLanding != null && _isLanding.Value,
                IsApi = _isApi != null && _isApi.Value,
                IsLegalCustomer = SettingsCustomers.IsRegistrationAsLegalEntity,
                IsPhysicalCustomer = SettingsCustomers.IsRegistrationAsPhysicalEntity,
                AuthMethod = SettingsOAuth.AuthMethod == 
                    EAuthMethod.Code && !SettingsOAuth.AuthByCodeActive
                        ? EAuthMethod.Email
                        : SettingsOAuth.AuthMethod,
            };

            if (BonusSystem.IsActive)
            {
                model.IsBonusSystemActive = true;
                model.BonusPlus = BonusSystem.BonusesForNewCard != 0
                    ? BonusSystem.BonusesForNewCard
                    : BonusSystemService.GetBonusCost(current.Cart).BonusPlus;
            }

            return model;
        }

    }
}