using System.Collections.Generic;
using AdvantShop.Orders;

namespace AdvantShop.Core.Modules.Interfaces
{
    public interface IIgnoreCheckoutShipping
    {
        List<ShoppingCartItem> GetIgnoreShippingCartItems();
    }
}
