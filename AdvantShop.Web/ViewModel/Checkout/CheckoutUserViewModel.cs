using System;
using System.Linq;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Core.Services.Auth;
using AdvantShop.Customers;
using AdvantShop.Orders;
using AdvantShop.Repository.Currencies;

namespace AdvantShop.ViewModel.Checkout
{
    public class CheckoutUserViewModel
    {
        public CheckoutUserViewModel()
        {
            SuggestionsModule = 
                AttachedModules.GetModules<ISuggestions>()
                                .Select(x => (ISuggestions) Activator.CreateInstance(x))
                                .FirstOrDefault();
        }

        public CheckoutData Data { get; set; }
        public Customer Customer { get; set; }
        public bool IsBonusSystemActive { get; set; }
        public float BonusPlus { get; set; }

        public Currency Currency { get; set; }
        public bool IsLanding { get; set; }
        public bool IsApi { get; set; }

        public ISuggestions SuggestionsModule { get; private set; }
        public bool IsLegalCustomer { get; set; }
        public bool IsPhysicalCustomer { get; set; }
        public EAuthMethod AuthMethod { get; set; }
    }
}