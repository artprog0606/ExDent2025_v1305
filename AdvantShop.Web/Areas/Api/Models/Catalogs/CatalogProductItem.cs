using System.Collections.Generic;
using System.Linq;
using AdvantShop.Areas.Api.Handlers.Products;
using AdvantShop.Areas.Api.Models.Products;
using AdvantShop.Areas.Api.Services;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Customers;
using AdvantShop.Orders;
using Newtonsoft.Json;

namespace AdvantShop.Areas.Api.Models.Catalogs
{
    public class CatalogProductItem
    {
        public int ProductId { get; }

        public int OfferId { get; }

        public string OfferArtNo { get; }

        public float AmountOffer { get; }

        public string UrlPath { get; }

        public string Name { get; }

        public string BriefDescription { get; }

        public string ArtNo { get; }

        public float Multiplicity { get; }

        public float Amount { get; }

        public float MinAmount { get; }

        public float? MaxAmount { get; }

        public bool AllowPreorder { get; }

        public bool Recomended { get; }

        public bool Sales { get; }

        public bool Bestseller { get; }

        public bool NewProduct { get; }

        public bool Favorite { get; }
        
        public bool Gifts { get; }

        public bool Enabled { get; }

        private string ColorsStr { get; set; }

        public List<ColorApi> Colors =>
            !string.IsNullOrEmpty(ColorsStr)
                ? JsonConvert.DeserializeObject<List<ProductColorModel>>(ColorsStr).Select(x => new ColorApi(x)).ToList()
                : null;

        public int? ColorId { get; }
        public int? SizeId { get; }
        public double Ratio { get; }
        public double? ManualRatio { get; }
        
        public int CommentsCount { get; }

        public float Price { get; }
        public float PriceWithDiscount { get; }
        public ProductDiscountApi Discount { get; }
        public float CurrencyValue { get; }
        public string PreparedPrice { get; }
        public string PreparedOldPrice { get; }
        
        public bool AddToCartFromList { get; }

        private bool MultiPrices { get; set; }

        [JsonIgnore]
        public ProductPhoto Photo { get; }

        public string PhotoSmall => Photo.ImageSrcSmall();

        public string PhotoMiddle => Photo.ImageSrcMiddle();
        
        public List<ProductPhotoApi> Photos { get; set; }
        
        public List<ProductMarker> Markers { get; set; }
        
        public List<PriceRuleAmountListItemApi> PriceRuleAmountList { get; }

        public CatalogProductItem(ProductItem product, bool showHtmlPrice, ShoppingCart wishList)
        {
            ProductId = product.ProductId;
            OfferId = product.OfferId;
            Enabled = product.Enabled;
            OfferArtNo = product.OfferArtNo;
            AmountOffer = product.AmountOffer;
            UrlPath = product.UrlPath;
            Name = product.Name;
            BriefDescription = !string.IsNullOrWhiteSpace(product.BriefDescription) ? product.BriefDescription : null;
            ArtNo = product.ArtNo;
            Multiplicity = product.Multiplicity;
            Amount = product.Amount;
            MinAmount = product.MinAmount;
            MaxAmount = product.MaxAmount > 0 ? product.MaxAmount : default;
            AllowPreorder = product.AllowPreorder;
            Recomended = product.Recomend;
            Sales = product.Sales;
            Bestseller = product.Bestseller;
            NewProduct = product.New;
            Favorite = wishList.Find(x => x.OfferId == product.OfferId || x.Offer.ProductId == product.ProductId) != null;
            Gifts = product.Gifts;
            ColorsStr = product.Colors;
            ColorId = product.ColorId > 0 ? product.ColorId : default(int?);
            SizeId = product.SizeId > 0 ? product.SizeId : default(int?);
            Ratio = product.Ratio;
            ManualRatio = product.ManualRatio;
            CommentsCount = product.Comments;
            MultiPrices = product.MultiPrices;
            
            Price = product.RoundedPrice;
            PriceWithDiscount = product.PriceWithDiscount;
            
            if (Price < PriceWithDiscount)
                Price = PriceWithDiscount;
            
            var discount = product.TotalDiscount;
            Discount = new ProductDiscountApi(discount);
            CurrencyValue = product.CurrencyValue;
            
            PreparedPrice = showHtmlPrice
                ? product.PreparedPrice
                : PriceFormatService.FormatPriceWithoutHtml(Price, PriceWithDiscount, discount, true, MultiPrices);
            
            PreparedOldPrice = showHtmlPrice
                ? product.PreparedPrice
                : Price.FormatPrice();

            Photo = product.Photo;

            Photos = PhotoService.GetPhotos<ProductPhoto>(ProductId, PhotoType.Product).Select(x => new ProductPhotoApi(x)).ToList();
            
            var p = new Product()
            {
                ProductId = ProductId,
                ArtNo = ArtNo,
                MinAmount = MinAmount,
                MaxAmount = MaxAmount,
                Multiplicity = Multiplicity,
                AllowPreOrder = AllowPreorder,
            };
            var amountByMultiplicity = product.AmountByMultiplicity;
            var amount = amountByMultiplicity > 0 ? product.Amount : 0;
            var amountMinToBuy = p.GetMinAmount();

            var isAvailableForPurchase = 
                ProductExtensions.IsAvailableForPurchase(amount, amountMinToBuy, Price, discount, p.AllowBuyOutOfStockProducts());

            AddToCartFromList = isAvailableForPurchase && 
                                ColorId == null && SizeId == null &&
                                !CustomOptionsService.DoesProductHaveCustomOptions(product.ProductId);

            if (SettingsPriceRules.ShowAmountsTableInCatalog && OfferId != 0)
            {
                PriceRuleAmountList =
                    PriceRuleService.GetPriceRuleAmountListItems(
                            OfferId,
                            ProductId,
                            CustomerContext.CurrentCustomer.CustomerGroup, 
                            Multiplicity, 
                            CurrencyValue,
                            new Discount(product.Discount, product.DiscountAmount),
                            product.DoNotApplyOtherDiscounts,
                            product.MainCategoryId)
                        ?.Select(x => new PriceRuleAmountListItemApi(x)).ToList();
            }
        }
    }
}