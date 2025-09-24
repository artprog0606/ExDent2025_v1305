namespace AdvantShop.Core.Services.Catalog
{
    public static class PriceRuleExtensions
    {
        /// <summary>
        /// Применять ли скидки
        /// </summary>
        public static bool ApplyDiscounts(this OfferPriceRule rule)
        {
            return rule == null || rule.ApplyDiscounts;
        }
    }
}