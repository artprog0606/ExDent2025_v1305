using AdvantShop.Catalog;
using AdvantShop.Core.Services.Api;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Localization;

namespace AdvantShop.Areas.Api.Models.Products
{
    public class GetProductPriceResponse : IApiResponse
    {
        public float? OldPrice { get; set; }
        public string PreparedOldPrice => OldPrice?.FormatPrice();
        
        public float Price { get; set; }

        public string PreparedPrice => Price > 0
            ? Price.FormatPrice()
            : LocalizationService.GetResource("Core.Catalog.PriceFormat.ContactWithUs");

        public string Bonuses { get; set; }
        
        public ProductDiscountApi Discount { get; set; }
    }
}