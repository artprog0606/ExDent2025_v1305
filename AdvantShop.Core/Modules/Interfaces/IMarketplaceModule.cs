namespace AdvantShop.Core.Modules.Interfaces
{
    public interface IMarketplaceModule
    {
        ModuleRoute GetProductButtonRoute(int productId, int? offerId = null, bool isAdminArea = false);
        bool IsHaveProductButton(int productId, bool isAdminArea = false);
    }
}