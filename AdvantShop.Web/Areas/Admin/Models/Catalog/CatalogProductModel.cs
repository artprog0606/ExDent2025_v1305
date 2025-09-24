using AdvantShop.Catalog;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Customers;
using AdvantShop.FilePath;
using AdvantShop.Repository.Currencies;

namespace AdvantShop.Web.Admin.Models.Catalog
{
    public interface IAdminCatalogGridProduct
    {
        int ProductId { get; set; }
    }
    
    public class CatalogProductModel : IAdminCatalogGridProduct
    {
        public int ProductId { get; set; }

        public string ArtNo { get; set; }

        public string Name { get; set; }

        public string ProductArtNo { get; set; }

        public string PhotoName { get; set; }

        public string PhotoSrc => FoldersHelper.GetImageProductPath(ProductImageType.Small, PhotoName, true);

        public float Price { get; set; }

        public string PriceString => PriceFormatService.FormatPrice(Price, CurrencyValue, CurrencyCode, CurrencyIso3, CurrencyIsCodeBefore);

        public float Amount { get; set; }

        public int OffersCount { get; set; }
        public int UsedWarehouses { get; set; }
        public int CountWarehouses { get; set; }

        public bool Enabled { get; set; }

        public string CurrencyCode { get; set; }
        public string CurrencyIso3 { get; set; }
        public float CurrencyValue { get; set; }
        public bool CurrencyIsCodeBefore { get; set; }

        public int SortOrder { get; set; }
        
        public int ColorId { get; set; }
        public int SizeId { get; set; }
    }

    public class CatalogProductModelBySelector : CatalogProductModel
    {
        public float Discount { get; set; }
        public float DiscountAmount { get; set; }
        public bool DoNotApplyOtherDiscounts { get; set; }
        public int? MainCategoryId { get; set; }
        
        private Currency RenderingCurrency => CurrencyService.BaseCurrency;
        
        private float? _roundedPrice;
        public float RoundedPrice =>
            _roundedPrice ?? 
            (_roundedPrice = PriceService.RoundPrice(Price, RenderingCurrency, CurrencyValue)).Value;

        private CustomerGroup _customerGroup;
        private CustomerGroup CustomerGroup =>
            _customerGroup ?? (_customerGroup = CustomerGroupService.GetDefaultCustomerGroup());
        
        private Discount _totalDiscount;
        public Discount TotalDiscount
        {
            get
            {
                if (_totalDiscount != null)
                    return _totalDiscount;

                return _totalDiscount =
                    PriceService.GetFinalDiscount(RoundedPrice, Discount, DiscountAmount,
                        CurrencyValue, CustomerGroup, ProductId, 
                        doNotApplyOtherDiscounts: DoNotApplyOtherDiscounts, 
                        renderingCurrency: RenderingCurrency,
                        productMainCategoryId: MainCategoryId);
            }
        }

        public float PriceWithDiscount => PriceService.GetFinalPrice(RoundedPrice, TotalDiscount);

        public new string PriceString => PriceWithDiscount.FormatPrice(RenderingCurrency);
    }

    public class CatalogOfferModel
    {
        public int OfferId { get; set; }
        public int ProductId { get; set; }
        public string ArtNo { get; set; }
        public string Name { get; set; }
        public string ColorName { get; set; }
        public string SizeName { get; set; }
        public string SizeNameForCategory { get; set; }
        public string SizeNameFormatted => SizeNameForCategory.IsNotEmpty() ? $"{SizeName} ({SizeNameForCategory})" : SizeName;
        public float Price { get; set; }
        public float Discount { get; set; }
        public float DiscountAmount { get; set; }
        public bool DoNotApplyOtherDiscounts { get; set; }
        public int? MainCategoryId { get; set; }
        public bool Enabled { get; set; }
        public float Amount { get; set; }

        public int OffersCount { get; set; }
        
        public string CurrencyCode { get; set; }
        public string CurrencyIso3 { get; set; }
        public float CurrencyValue { get; set; }
        public bool IsCodeBefore { get; set; }
        public bool Main { get; set; }
        public string PhotoName { get; set; }
        public string PhotoSrc => FoldersHelper.GetImageProductPath(ProductImageType.Small, PhotoName, true);

        private CustomerGroup _customerGroup;
        private CustomerGroup CustomerGroup =>
            _customerGroup ?? (_customerGroup = CustomerGroupService.GetDefaultCustomerGroup());

        private Currency RenderingCurrency => CurrencyService.BaseCurrency;
        
        private float? _roundedPrice;
        public float RoundedPrice =>
            _roundedPrice ?? 
            (_roundedPrice = PriceService.RoundPrice(Price, RenderingCurrency, CurrencyValue)).Value;
        
        private Discount _totalDiscount;

        public Discount TotalDiscount
        {
            get
            {
                if (_totalDiscount != null)
                    return _totalDiscount;

                return _totalDiscount =
                    PriceService.GetFinalDiscount(RoundedPrice, Discount, DiscountAmount,
                        CurrencyValue, CustomerGroup, ProductId, 
                        doNotApplyOtherDiscounts: DoNotApplyOtherDiscounts, 
                        renderingCurrency: RenderingCurrency,
                        productMainCategoryId: MainCategoryId);
            }
        }

        public float PriceWithDiscount =>
            PriceService.GetFinalPrice(RoundedPrice, TotalDiscount);

        public string PriceFormatted => 
            PriceWithDiscount.FormatPrice(RenderingCurrency);
    }
}