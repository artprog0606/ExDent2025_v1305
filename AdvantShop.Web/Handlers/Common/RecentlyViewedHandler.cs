using System.Web;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Customers;

namespace AdvantShop.Handlers.Common
{
    public class RecentlyViewedHandler
    {
        private readonly int _productAmount;

        public RecentlyViewedHandler(int productAmount)
        {
            _productAmount = productAmount;
        }

        public ProductViewModel Get()
        {
            if (Helpers.BrowsersHelper.IsBot())
                return null;

            var recentlyViewedItems = RecentlyViewService.LoadViewDataByCustomer(CustomerContext.CustomerId, _productAmount);

            return recentlyViewedItems != null && recentlyViewedItems.Count > 0
                ? new ProductViewModel(recentlyViewedItems)
                : null;
        }
    }
}