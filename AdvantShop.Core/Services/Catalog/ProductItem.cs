using System;
using System.Collections.Generic;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Customers;
using AdvantShop.Saas;

namespace AdvantShop.Core.Services.Catalog
{
    public class ProductItem
    {
        public int ProductId { get; }

        public int OfferId { get; }
        
        public string ArtNo { get; }

        public string OfferArtNo { get; }

        public string UrlPath { get; }

        public string Name { get; }

        public string BriefDescription { get; }

        public float Multiplicity { get; }

        public float AmountOffer { get; }
        
        public float Amount { get; }

        public float AmountByMultiplicity { get; }

        public float MinAmount { get; }

        public float? MaxAmount { get; }

        public bool AllowPreorder { get; }

        public bool Recomend { get; }

        public bool Sales { get; }

        public bool Bestseller { get; }

        public bool New { get; }

        public bool Gifts { get; }

        public bool Enabled { get; }

        public string Colors { get; }

        public List<ProductColorModel> ColorsList { get; }

        public int ColorId { get; }

        public int? SelectedColorId { get; }
        
        public int? PreSelectedColorId { get; }

        public int? SelectedSizeId { get; }

        public int SizeId { get; }

        public double Ratio { get; }

        public double? ManualRatio { get; }

        public int? RatioId { get; }

        public float BasePrice { get; }

        public float Discount { get; }
        public float DiscountAmount { get; }

        public int Comments { get; }

        public float CurrencyValue { get; }
        
        public bool DoNotApplyOtherDiscounts { get; }
        
        public int? MainCategoryId { get; }
        
        public bool MultiPrices { get; }

        public string BarCode { get; }


        public ProductPhoto Photo { get; }
        
        public string StartPhotoJson { get; }

        public string PhotoSmall { get; }

        public string PhotoMiddle { get; }

        public string PhotoBig { get; }

        public int PhotoId { get; }

        public int CountPhoto { get; }

        public string UnitDisplayName { get; set; }
        public string UnitName { get; set; }


        private List<ProductDiscount> ProductDiscounts { get; }
        private float DiscountByDatetime { get; }
        private CustomerGroup CustomerGroup { get; }
        
        private readonly OfferPriceRule _priceRule;
        public OfferPriceRule PriceRule => _priceRule;
        
        private readonly float? _priceByPriceRule;
        
        private float? _roundedPrice;
        public float RoundedPrice =>
            _roundedPrice ?? 
            (_roundedPrice = PriceService.RoundPrice(BasePrice, null, CurrencyValue)).Value;
        

        private Discount _totalDiscount;

        public Discount TotalDiscount
        {
            get
            {
                if (_totalDiscount != null)
                    return _totalDiscount;

                return _totalDiscount =
                    _priceRule == null || _priceRule.ApplyDiscounts
                        ? PriceService.GetFinalDiscount(_priceByPriceRule ?? RoundedPrice, Discount, DiscountAmount,
                                                        CurrencyValue, CustomerGroup, ProductId, DiscountByDatetime, 
                                                        ProductDiscounts, DoNotApplyOtherDiscounts, 
                                                        productMainCategoryId: MainCategoryId)
                        : new Discount(0, RoundedPrice - (_priceByPriceRule ?? 0));
            }
        }

        private float? _finalPrice;

        public float PriceWithDiscount =>
            _finalPrice ??
            (
                _finalPrice =
                    _priceRule == null || _priceRule.ApplyDiscounts
                        ? PriceService.GetFinalPrice(_priceByPriceRule ?? RoundedPrice, TotalDiscount)
                        : _priceByPriceRule ?? 0
            ).Value;

        private string _preparedPrice;

        public string PreparedPrice =>
            _preparedPrice ??
            (_preparedPrice =
                _priceRule == null || _priceRule.ApplyDiscounts
                    ? PriceFormatService.FormatPrice(_priceByPriceRule ?? RoundedPrice, PriceWithDiscount, TotalDiscount, true, true, MultiPrices, SettingsCatalog.ShowUnitsInCatalog ? UnitDisplayName : null)
                    : PriceFormatService.FormatPrice(RoundedPrice, PriceWithDiscount, TotalDiscount, true, true, MultiPrices, SettingsCatalog.ShowUnitsInCatalog ? UnitDisplayName : null));
        
        private bool? _allowAddProductToCart;

        public bool AllowAddProductToCart
        {
            get =>
                _allowAddProductToCart ??
                (_allowAddProductToCart =
                    OfferService.GetCountProductOffers(ProductId) <= 1
                    && !CustomOptionsService.DoesProductHaveRequiredCustomOptions(ProductId)).Value;
            
            set => _allowAddProductToCart = value;
        }
        
        public float GetMinAmount(ProductItem product) => product.GetMinAmount();

        public List<string> Labels { get; set; }

        public ProductItem(ProductModel product, float discountByDatetime, List<ProductDiscount> productDiscounts) 
                    : this(product, 
                        CustomerContext.CurrentCustomer.CustomerGroup, 
                        discountByDatetime, 
                        productDiscounts,
                        priceRulesActive: !SaasDataService.IsSaasEnabled || SaasDataService.CurrentSaasData.PriceTypes,
                        null)
        {
            
        }

        public ProductItem(ProductModel product, CustomerGroup customerGroup, 
                            float discountByDatetime, List<ProductDiscount> productDiscounts,
                            bool priceRulesActive, List<string> labels)
        {
            ProductId = product.ProductId;
            OfferId = product.OfferId;
            OfferArtNo = product.OfferArtNo;
            AmountOffer = product.AmountOffer;
            UrlPath = product.UrlPath;
            Name = product.Name;
            BriefDescription = product.BriefDescription;
            ArtNo = product.ArtNo;
            Multiplicity = product.Multiplicity;
            Amount = product.Amount;
            AmountByMultiplicity = product.AmountByMultiplicity;
            MinAmount = product.MinAmount;
            MaxAmount = product.MaxAmount;
            AllowPreorder = product.AllowPreorder;
            Recomend = product.Recomend;
            Sales = product.Sales;
            Bestseller = product.Bestseller;
            New = product.New;
            Gifts = product.Gifts;
            Enabled = product.Enabled;
            Colors = product.Colors;
            ColorsList = product.ColorsList;
            ColorId = product.ColorId;
            SizeId = product.SizeId;
            SelectedColorId = product.SelectedColorId;
            PreSelectedColorId = product.PreSelectedColorId;
            SelectedSizeId = product.SelectedSizeId;
            Ratio = product.Ratio;
            ManualRatio = product.ManualRatio;
            RatioId = product.RatioId;
            Comments = product.Comments;
            BarCode = product.BarCode;
            
            BasePrice = product.BasePrice;
            Discount = product.Discount;
            DiscountAmount = product.DiscountAmount;
            CurrencyValue = product.CurrencyValue;
            DoNotApplyOtherDiscounts = product.DoNotApplyOtherDiscounts;
            MultiPrices = product.MultiPrices;
            MainCategoryId = product.MainCategoryId;
            
            Photo = product.Photo;
            StartPhotoJson = product.StartPhotoJson;
            PhotoSmall = product.PhotoSmall;
            PhotoMiddle = product.PhotoMiddle;
            PhotoBig = product.PhotoBig;
            PhotoId = product.PhotoId;
            CountPhoto = product.CountPhoto;
            
            CustomerGroup = customerGroup;
            DiscountByDatetime = discountByDatetime;
            ProductDiscounts = productDiscounts;
            UnitDisplayName = product.UnitDisplayName;
            UnitName = product.UnitName;
            
            if (product.AllowAddToCartInCatalog != null)
                AllowAddProductToCart = product.AllowAddToCartInCatalog.Value;
            
            Labels = labels;

            if (priceRulesActive)
            {
                var amount = MinAmount == 0
                    ? Multiplicity
                    : Multiplicity > MinAmount ? Multiplicity : MinAmount;

                _priceRule = PriceRuleService.GetPriceRule(OfferId, amount, CustomerGroup.CustomerGroupId);

                if (_priceRule?.PriceByRule != null)
                    _priceByPriceRule = _priceRule.PriceByRule.Value.RoundPrice(CurrencyValue);
            }
        }
    }
}