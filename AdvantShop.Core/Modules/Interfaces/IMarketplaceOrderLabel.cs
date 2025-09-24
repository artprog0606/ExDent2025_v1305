namespace AdvantShop.Core.Modules.Interfaces
{
    public interface IMarketplaceOrderLabel
    {
        (byte[] Label, string ContentType) GetOrderLabel(int orderId);
        /// <returns>Доступность этикетки заказа</returns>
        bool IsAvailableLabel(int orderId);
    }
}
