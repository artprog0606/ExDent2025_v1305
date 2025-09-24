namespace AdvantShop.Core.Modules.Interfaces
{
    public interface IIgnoreProductDoNotApplyOtherDiscounts
    {
        bool Ignore(int productId);
    }
}