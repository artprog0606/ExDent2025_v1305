using System.Collections.Generic;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Models.Checkout;
using AdvantShop.Models.MyAccount;
using AdvantShop.Orders;

namespace AdvantShop.ViewModel.Checkout
{
    public class ThankYouPageViewModel
    {
        public EThankYouPageActionType ActionType { get; set; }

        public List<SocialNetworkGroup> SocialNetworks { get; set; }

        public string NameOfBlockProducts { get; set; }
        public List<int> ProductIds { get; set; }

        public OrderDetailsModel OrderDetails { get; set; }
    }
}