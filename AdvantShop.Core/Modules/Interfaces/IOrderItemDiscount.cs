using AdvantShop.Catalog;
using AdvantShop.Orders;

namespace AdvantShop.Core.Modules.Interfaces
{
    public interface IOrderItemDiscount
    {
        Discount GetOrderItemDiscount(IOrderItem orderItem);
    }
}