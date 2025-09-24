using System;
using System.Linq;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;

namespace AdvantShop.ViewModel.Checkout
{
    public class CheckoutAddressViewModel
    {
        public CheckoutAddressViewModel()
        {
            SuggestionsModule = AttachedModules.GetModules<ISuggestions>().Select(x => (ISuggestions)Activator.CreateInstance(x)).FirstOrDefault();
        }

        public ISuggestions SuggestionsModule { get; private set; }
    }
}