using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Catalog;
using AdvantShop.CMS;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.SalesChannels;
using AdvantShop.Core.Services.Taxes;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.FilePath;
using AdvantShop.Repository.Currencies;
using AdvantShop.Saas;
using AdvantShop.Taxes;

namespace AdvantShop.Web.Admin.Models.Catalog.Products
{
    public class AdminProductModel : IValidatableObject
    {
        public AdminProductModel()
        {
            BreadCrumbs = new List<BreadCrumbs>();

            var allCurrencies = CurrencyService.GetAllCurrencies(true);
            Currencies =
                allCurrencies
                    .Select(x => new SelectListItem() { Text = x.Name, Value = x.CurrencyId.ToString() })
                    .ToList();

            TabModules = new List<AdminProductTabItem>();
            //LandingLinks = new List<string>();

            Taxes =
                TaxService.GetTaxes().Where(x => x.Enabled)
                    .Select(x => new SelectListItem() { Text = x.Name, Value = x.TaxId.ToString() })
                    .ToList();

            PaymentMethodTypes = Enum.GetValues(typeof(ePaymentMethodType)).Cast<ePaymentMethodType>().Select(x => new SelectListItem()
            {
                Text = x.Localize(),
                Value = x.ConvertIntString()
            }).ToList();

            PaymentSubjectTypes = Enum.GetValues(typeof(ePaymentSubjectType)).Cast<ePaymentSubjectType>().Select(x => new SelectListItem()
            {
                Text = x.Localize(),
                Value = x.ConvertIntString()
            }).ToList();

            var sizeCharts = SizeChartService.GetAll().Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();
            sizeCharts.Insert(0, new SelectListItem() { Text = LocalizationService.GetResource("Core.Product.SizeCharts.NotSelected"), Value = string.Empty });
            SizeCharts = sizeCharts;

            Units =
                UnitService.GetList()
                          .Select(x => new SelectListItem() { Text = x.Name != x.DisplayName ? $"{x.Name} ({x.DisplayName})" : x.Name, Value = x.Id.ToString() })
                          .ToList();
            Units.Insert(0, new SelectListItem() { Text = LocalizationService.GetResource("Core.Product.Units.NotSelected"), Value = "" });

            UseOfferWeightAndDimensions = SettingsCatalog.EnableOfferWeightAndDimensions;
            UseOfferBarCode = SettingsCatalog.EnableOfferBarCode;

            var avito = SalesChannelService.GetByType(ESalesChannelType.Avito);
            ShowAvito = avito != null && avito.Enabled;
        }

        public List<BreadCrumbs> BreadCrumbs { get; set; }

        public int CurrencyId { get; set; }
        public Currency Currency { get; set; }
        public List<SelectListItem> Currencies { get; set; }


        public int ProductId { get; set; }
        public string ArtNo { get; set; }
        public string Name { get; set; }
        public string UrlPath { get; set; }
        public bool Enabled { get; set; }

        public string BriefDescription { get; set; }
        public string Description { get; set; }

        public bool Recomended { get; set; }
        public bool New { get; set; }
        public bool BestSeller { get; set; }
        public bool Sales { get; set; }


        public float Weight { get; set; }
        public float Length { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }


        public string Photo { get; set; }
        public string PhotoSrc()
        {
            return FoldersHelper.GetImageProductPath(ProductImageType.Small, Photo, false);
        }

        public float DiscountPercent { get; set; }
        public float DiscountAmount { get; set; }
        public DiscountType DiscountType { get; set; }


        public bool AllowPreOrder { get; set; }

        public int? SizeChartId { get; set; }
        public List<SelectListItem> SizeCharts { get; set; }
        public int? UnitId { get; set; }
        public string Unit { get; set; }
        public List<SelectListItem> Units { get; set; }
        public float? ShippingPrice { get; set; }
        public string YandexDeliveryDays { get; set; }

        public float? MinAmount { get; set; }
        public float? MaxAmount { get; set; }
        public float Multiplicity { get; set; }

        public int BrandId { get; set; }
        public Brand Brand { get; set; }

        public bool DefaultMeta { get; set; }
        public string SeoTitle { get; set; }
        public string SeoH1 { get; set; }
        public string SeoKeywords { get; set; }
        public string SeoDescription { get; set; }


        public int ReviewsCount { get; set; }
        public List<string> Tags { get; set; }

        public bool IsTagsVisible => !SaasDataService.IsSaasEnabled || SaasDataService.CurrentSaasData.HaveTags;

        public bool HasCustomOptions { get; set; }

        public List<AdminProductTabItem> TabModules { get; set; }


        //public bool IsLandingEnabled { get; set; }
        //public List<string> LandingLinks { get; set; }
        //public string LandingProductDescription { get; set; }

        public bool ShowGoogleImageSearch { get; set; }
        public bool ShowImageSearchEnabled { get; set; }

        public string BarCode { get; set; }

        public bool AccrueBonuses { get; set; }

        public int? TaxId { get; set; }
        public List<SelectListItem> Taxes { get; set; }

        public ePaymentSubjectType PaymentSubjectType { get; set; }
        public List<SelectListItem> PaymentSubjectTypes { get; set; }

        public ePaymentMethodType PaymentMethodType { get; set; }
        public List<SelectListItem> PaymentMethodTypes { get; set; }

        public bool IsLandingFunnelsEnabled => SettingsLandingPage.ActiveLandingPage;

        public bool IsPropertyGroupsDropDownList
        { 
            get 
            {
                return SettingsCatalog.IsPropertyGroupsDropDownList;
            } 
        }


        public int AllSalesChannelCount { get; set; }
        public int ProductSalesChannelCount { get; set; }

        public bool HasFunnels { get; set; }

        public double Ratio { get; set; }
        public double? ManualRatio { get; set; }

        public bool UseOfferWeightAndDimensions { get; set; }
        public bool UseOfferBarCode { get; set; }
        public int SortPopular { get; set; }
        public bool Hidden { get; set; }

        public string PriceRules { get; set; }
        public bool IsDigital { get; set; }
        public string Comment { get; set; }
        public string DownloadLink { get; set; }
        
        public bool DoNotApplyOtherDiscounts { get; set; }

        public bool ShowAvito { get; set; }

        public bool ShowMarketplaceProductButton { get; set; }
        public bool AvailableWarehouses { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                yield return new ValidationResult(LocalizationService.GetResource("Admin.Category.AdminCategoryModel.Error.Name"), new[] { "Name" });
            }

            Name = Name.Trim();

            if (string.IsNullOrWhiteSpace(ArtNo))
            {
                yield return new ValidationResult(LocalizationService.GetResource("Admin.Category.AdminCategoryModel.Error.ArtNo"), new[] { "ArtNo" });
            }
            else
            {
                var tempId = ProductService.GetProductId(ArtNo);
                if (tempId != 0 && tempId != ProductId)
                {
                    yield return new ValidationResult(LocalizationService.GetResource("Admin.Category.AdminCategoryModel.Error.ArtNoDublicate"), new[] { "ArtNo" });
                }
            }

            if (Multiplicity <= 0)
            {
                Multiplicity = 1;
            }

            if (DiscountPercent < 0 || DiscountPercent > 100)
                yield return new ValidationResult(LocalizationService.GetResource("Admin.Category.AdminCategoryModel.Error.DiscountPercent"), new[] { "DiscountPercent" });

            if (DiscountAmount < 0)
                yield return new ValidationResult(LocalizationService.GetResource("Admin.Category.AdminCategoryModel.Error.DiscountAmount"), new[] { "DiscountAmount" });


            if (string.IsNullOrEmpty(UrlPath))
            {
                yield return new ValidationResult(LocalizationService.GetResource("Admin.Category.AdminCategoryModel.Error.Url"), new[] { "Url" });
            }
            else
            {
                // if new product or urlpath != previous urlpath
                if (ProductId == 0 || (UrlService.GetObjUrlFromDb(ParamType.Product, ProductId) != UrlPath))
                {
                    if (!UrlService.IsValidUrl(UrlPath, ParamType.Product))
                    {
                        UrlPath = UrlService.GetAvailableValidUrl(0, ParamType.Product, UrlPath);
                    }
                }
            }

            if (SeoTitle != null || SeoKeywords != null || SeoDescription != null || SeoH1 != null)
            {
                SeoTitle = SeoTitle ?? "";
                SeoKeywords = SeoKeywords ?? "";
                SeoDescription = SeoDescription ?? "";
                SeoH1 = SeoH1 ?? "";
            }

            if (BriefDescription.IsLongerThan(ProductService.MaxDescLength))
            {
                yield return new ValidationResult(LocalizationService.GetResourceFormat("Core.Product.DescriptionLengthLimit.ErrorFormat", null, ProductService.MaxDescLength), new[] { "BriefDescription" });
            }

            if (Description.IsLongerThan(ProductService.MaxDescLength))
            {
                yield return new ValidationResult(LocalizationService.GetResourceFormat("Core.Product.DescriptionLengthLimit.ErrorFormat", null, ProductService.MaxDescLength), new[] { "Description" });
            }
        }
    }
}
