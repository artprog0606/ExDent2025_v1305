using System.Collections.Generic;
using AdvantShop.Areas.Api.Handlers.Products;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Localization;

namespace AdvantShop.Areas.Api.Models.Products
{
    public class ProductOffer
    {
        public int OfferId { get; set; }
        
        public string ArtNo { get; set; }
        
        public bool IsMain { get; set; }
        
        public int? ColorId { get; set; }
        
        public int? SizeId { get; set; }
        
        public float? OldPrice { get; set; }
        
        public string PreparedOldPrice { get; set; }
        
        public float Price { get; set; }
        
        public string PreparedPrice { get; set; }
        
        public string Bonuses { get; set; }
        
        public float Amount { get;set; }
        
        public bool IsAvailable { get; set; }
        
        public string AvailableText { get; set; }

        public float Weight { get; set; }
        
        public float Height { get; set; }

        public float Length { get; set; }

        public float Width { get; set; }
        
        public bool AddToCart { get; }

        public ProductOffer(Offer offer, Product product, IList<CustomOption> customOptions,
                                List<SelectedCustomOptionApi> options, float amount)
        {
            OfferId = offer.OfferId;
            ArtNo = offer.ArtNo;
            ColorId = offer.ColorID;
            SizeId = offer.SizeID;
            IsMain = offer.Main;
            Amount = offer.Amount;

            Weight = offer.GetWeight();
            Width = offer.GetWidth();
            Length = offer.GetLength();
            Height = offer.GetHeight();

            var prices = 
                new GetProductPriceApi(product, offer, customOptions, options, amount).Execute();
            
            OldPrice = prices.OldPrice;
            PreparedOldPrice = prices.PreparedOldPrice;
            Price = prices.Price;
            PreparedPrice = prices.PreparedPrice;
            Bonuses = prices.Bonuses;

            var amountByMultiplicity = offer.GetAmountByMultiplicity(product.Multiplicity);
            var allowBuyOutOfStockProducts = product.AllowBuyOutOfStockProducts();
            
            var isAvailable = amountByMultiplicity > 0;
            
            IsAvailable = isAvailable || allowBuyOutOfStockProducts;
            AvailableText = string.Format("{0}{1}",
                IsAvailable
                    ? LocalizationService.GetResource("Product.Available")
                    : LocalizationService.GetResource("Product.NotAvailable"),
                isAvailable && SettingsCatalog.ShowStockAvailability
                    ? string.Format(" ({0}{1})",
                        amountByMultiplicity,
                        (product.Unit?.DisplayName).IsNotEmpty() ? " " + product.Unit?.DisplayName : "")
                    : string.Empty);

            var isAvailableForPurchase =
                offer.IsAvailableForPurchase(amount, amountByMultiplicity, Price, (Discount) prices.Discount, allowBuyOutOfStockProducts);

            AddToCart = isAvailableForPurchase && IsAvailable;
        }
    }
}