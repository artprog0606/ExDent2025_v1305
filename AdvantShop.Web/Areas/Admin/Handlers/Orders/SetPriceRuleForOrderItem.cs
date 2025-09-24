using AdvantShop.Catalog;
using AdvantShop.Orders;
using System.Linq;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Saas;

namespace AdvantShop.Web.Admin.Handlers.Orders
{
    public class SetPriceRuleForOrderItem
    {
        private readonly Order _order;
        private readonly OrderItem _orderItem;

        public SetPriceRuleForOrderItem(Order order, OrderItem orderItem)
        {
            _order = order;
            _orderItem = orderItem;
        }

        public void Execute()
        {
            if (_orderItem.ProductID == null  || _orderItem.IsCustomPrice)
                return;
            
            if (SaasDataService.IsSaasEnabled && !SaasDataService.CurrentSaasData.PriceTypes)
                return;

            var offer = OfferService.GetOffer(_orderItem.ArtNo);
            if (offer == null || offer.ProductId != _orderItem.ProductID)
                return;

            var priceRulesByOffer = PriceRuleService.GetOfferPriceRules(offer.OfferId).Where(x => x.PriceByRule != null).ToList();
            if (priceRulesByOffer.Count == 0)
                return;
            
            _orderItem.Price = OrderItemPriceService.CalculateFinalPrice(_orderItem, _order, out _, out _);
        }
    }
}
