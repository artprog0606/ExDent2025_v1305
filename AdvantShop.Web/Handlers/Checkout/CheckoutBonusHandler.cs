using AdvantShop.Core.Services.Bonuses;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Customers;
using AdvantShop.Orders;
using AdvantShop.ViewModel.Checkout;

namespace AdvantShop.Handlers.Checkout
{
    public class CheckoutBonusHandler
    {
        public CheckoutBonusViewModel Execute()
        {
            if (!BonusSystem.IsActive)
                return null;

            var current = MyCheckout.Factory(CustomerContext.CustomerId);
            var bonusCard = BonusSystemService.GetCard(CustomerContext.CurrentCustomer.Id);

            var appliedBonuses = bonusCard == null ? 0 : (float)bonusCard.BonusesTotalAmount.SimpleRoundPrice();

            if (current.Data.SelectShipping != null || current.AvailableShippingOptions().Count == 0)
                appliedBonuses = BonusSystemService.GetBonusCost(current.Cart, current.Data.SelectShipping?.FinalRate ?? 0, current.Data.Bonus.AppliedBonuses, current.Data.User.WantRegist || current.Data.User.WantBonusCard).BonusPrice;

            if (appliedBonuses != current.Data.Bonus.AppliedBonuses)
                current.Data.Bonus.AppliedBonuses = appliedBonuses;

            current.Update();
            return new CheckoutBonusViewModel()
            {
                AppliedBonuses = appliedBonuses,
                HasCard = bonusCard != null,
                BonusPlus = BonusSystemService.GetBonusCost(current.Cart).BonusPlus,
                AllowSpecifyBonusAmount = BonusSystem.AllowSpecifyBonusAmount,
                ProhibitAccrualAndSubstractBonuses = BonusSystem.ProhibitAccrualAndSubstractBonuses
            };
        }
    }
}