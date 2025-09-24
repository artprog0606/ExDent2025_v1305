using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Catalog;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Localization;
using AdvantShop.ExportImport;
using AdvantShop.Repository.Currencies;
using AdvantShop.Saas;
using Newtonsoft.Json;

namespace AdvantShop.Web.Admin.Models.Catalog.ExportFeeds
{
    public class ExportFeedSettingsYandexWebmasterModel : IValidatableObject
    {
        public ExportFeedSettingsYandexWebmasterModel(ExportFeedYandexWebmasterOptions exportFeedYandexOptions)
        {
            Currency = exportFeedYandexOptions.Currency;
            RemoveHtml = exportFeedYandexOptions.RemoveHtml;
            Delivery = exportFeedYandexOptions.Delivery;
            DeliveryCost = exportFeedYandexOptions.DeliveryCost;
           //GlobalDeliveryCost = exportFeedYandexOptions.GlobalDeliveryCost;
            //LocalDeliveryOption = exportFeedYandexOptions.LocalDeliveryOption;
            ExportProductProperties = exportFeedYandexOptions.ExportProductProperties;
            ExportPropertyDisplayedName = exportFeedYandexOptions.ExportPropertyDisplayedName;
            JoinPropertyValues = exportFeedYandexOptions.JoinPropertyValues;
            ExportOnlyUseInDetailsProperties = exportFeedYandexOptions.ExportOnlyUseInDetailsProperties;
            ExportOnlyUseInDetailsPropertiesExceptionIds = exportFeedYandexOptions.ExportOnlyUseInDetailsPropertiesExceptionIds;
            ProductPriceType = exportFeedYandexOptions.ProductPriceType;
            SalesNotes = exportFeedYandexOptions.SalesNotes;
            ShopName = exportFeedYandexOptions.ShopName;
            CompanyName = exportFeedYandexOptions.CompanyName;
            ColorSizeToName = exportFeedYandexOptions.ColorSizeToName;
            ProductDescriptionType = exportFeedYandexOptions.ProductDescriptionType;
            OfferIdType = exportFeedYandexOptions.OfferIdType;
            VendorCodeType = exportFeedYandexOptions.VendorCodeType;
            ExportNotAvailable = exportFeedYandexOptions.ExportNotAvailable;
            AllowPreOrderProducts = exportFeedYandexOptions.AllowPreOrderProducts;
            //Available = exportFeedYandexOptions.Available;
            ExportPurchasePrice = exportFeedYandexOptions.ExportPurchasePrice;
            ExportCount = exportFeedYandexOptions.ExportCount;
            ExportShopSku = exportFeedYandexOptions.ExportShopSku;
            ExportRelatedProducts = exportFeedYandexOptions.ExportRelatedProducts;

            ExportExpiry = exportFeedYandexOptions.ExportExpiry;
            ExportWarrantyDays = exportFeedYandexOptions.ExportWarrantyDays;
            ExportCommentWarranty = exportFeedYandexOptions.ExportCommentWarranty;
            ExportPeriodOfValidityDays = exportFeedYandexOptions.ExportPeriodOfValidityDays;
            ExportServiceLifeDays = exportFeedYandexOptions.ExportServiceLifeDays;
            ExportMarketTnVedCode = exportFeedYandexOptions.ExportMarketTnVedCode;
            ExportMarketStepQuantity = exportFeedYandexOptions.ExportMarketStepQuantity;
            ExportMarketMinQuantity = exportFeedYandexOptions.ExportMarketMinQuantity;
            ExportMarketCisRequired = exportFeedYandexOptions.ExportMarketCisRequired;
            
            ExportBarCode = exportFeedYandexOptions.ExportBarCode;
            ExportAllPhotos = exportFeedYandexOptions.ExportAllPhotos;
            TypeExportYandex = exportFeedYandexOptions.TypeExportYandex;
            NeedZip = exportFeedYandexOptions.NeedZip;
            OnlyMainOfferToExport = exportFeedYandexOptions.OnlyMainOfferToExport;
            ExportDimensions = exportFeedYandexOptions.ExportDimensions;
            Promos = new List<ExportFeedYandexWebmasterPromo>();
            NotExportAmountCount = exportFeedYandexOptions.NotExportAmountCount;
            DontExportProductsWithoutDimensionsAndWeight = exportFeedYandexOptions.DontExportProductsWithoutDimensionsAndWeight;
            PriceRuleId = exportFeedYandexOptions.PriceRuleId;
            PriceRuleIdForOldPrice = exportFeedYandexOptions.PriceRuleIdForOldPrice;

            PriceFrom = exportFeedYandexOptions.PriceFrom;
            PriceTo = exportFeedYandexOptions.PriceTo;

            ConsiderMultiplicityInPrice = exportFeedYandexOptions.ConsiderMultiplicityInPrice;
            
            WarehouseIds = exportFeedYandexOptions.WarehouseIds;

            try
            {
                if (!string.IsNullOrWhiteSpace(exportFeedYandexOptions.Promos))
                {
                    Promos =
                        JsonConvert.DeserializeObject<List<ExportFeedYandexWebmasterPromo>>(exportFeedYandexOptions.Promos);

                    PromosJson = exportFeedYandexOptions.Promos;


                    PromoCodesJson = JsonConvert.SerializeObject(Promos.Where(x => x.Type == YandexWebmasterPromoType.PromoCode));
                    FlashDiscountsJson = JsonConvert.SerializeObject(Promos.Where(x => x.Type == YandexWebmasterPromoType.Flash));
                    PromoGiftsJson = JsonConvert.SerializeObject(Promos.Where(x => x.Type == YandexWebmasterPromoType.Gift));
                    NPlusMJson = JsonConvert.SerializeObject(Promos.Where(x => x.Type == YandexWebmasterPromoType.NPlusM));
                }
            }
            finally
            {
                if (Promos == null)
                    Promos = new List<ExportFeedYandexWebmasterPromo>();
            }

            LocalDeliveryOption = new ExportFeedYandexWebmasterDeliveryCostOption();
            try
            {
                LocalDeliveryOption =
                    JsonConvert.DeserializeObject<ExportFeedYandexWebmasterDeliveryCostOption>(exportFeedYandexOptions.LocalDeliveryOption);
            }
            catch (Exception)
            {
            }
            finally
            {
                if (LocalDeliveryOption == null)
                    LocalDeliveryOption = new ExportFeedYandexWebmasterDeliveryCostOption();
            }

            GlobalDeliveryCost = new List<ExportFeedYandexWebmasterDeliveryCostOption>();
            try
            {
                if (!string.IsNullOrWhiteSpace(exportFeedYandexOptions.GlobalDeliveryCost))
                {
                    GlobalDeliveryCost =
                        JsonConvert.DeserializeObject<List<ExportFeedYandexWebmasterDeliveryCostOption>>(exportFeedYandexOptions.GlobalDeliveryCost);

                    GlobalDeliveryCostJson = exportFeedYandexOptions.GlobalDeliveryCost;
                }
            }
            finally
            {
                if (GlobalDeliveryCost == null)
                    GlobalDeliveryCost = new List<ExportFeedYandexWebmasterDeliveryCostOption>();
            }
        }

        
        public int ExportFeedId { get; set; }

        public string Currency { get; set; }
        public bool RemoveHtml { get; set; }
        public bool Delivery { get; set; }
        public ExportFeedYandexWebmasterDeliveryCost DeliveryCost { get; set; }
        public bool ExportProductProperties { get; set; }
        public bool ExportPropertyDisplayedName { get; set; }
        public bool JoinPropertyValues { get; set; }
        
        public bool ExportOnlyUseInDetailsProperties { get; set; }
        public List<int> ExportOnlyUseInDetailsPropertiesExceptionIds { get; set; }

        public string ExportOnlyUseInDetailsPropertiesExceptionNames
        {
            get
            {
                if (ExportOnlyUseInDetailsPropertiesExceptionIds == null ||
                    ExportOnlyUseInDetailsPropertiesExceptionIds.Count == 0)
                    return null;

                var names = new List<string>();
                foreach (var id in ExportOnlyUseInDetailsPropertiesExceptionIds)
                {
                    var property = PropertyService.GetPropertyById(id);
                    if (property != null)
                        names.Add(property.Name);
                }
                return String.Join(", ", names);
            }
        }

        public EExportFeedYandexWebmasterPriceType ProductPriceType { get; set; }
        public string SalesNotes { get; set; }
        public string ShopName { get; set; }
        public string CompanyName { get; set; }
        public bool ColorSizeToName { get; set; }
        public string ProductDescriptionType { get; set; }
        public string OfferIdType { get; set; }
        public string VendorCodeType { get; set; }
        public bool ExportNotAvailable { get; set; }
        public bool AllowPreOrderProducts { get; set; }        
        public bool Available { get; set; }
        public bool ExportPurchasePrice { get; set; }
        public bool ExportCount { get; set; }
        public bool ExportShopSku { get; set; }
        
        public bool ExportExpiry { get; set; }
        public bool ExportWarrantyDays { get; set; }
        public bool ExportCommentWarranty { get; set; }
        public bool ExportPeriodOfValidityDays { get; set; }
        public bool ExportServiceLifeDays { get; set; }
        public bool ExportMarketTnVedCode { get; set; }
        public bool ExportMarketStepQuantity { get; set; }
        public bool ExportMarketMinQuantity { get; set; }
        public bool ExportMarketCisRequired { get; set; }
        

        public bool ExportRelatedProducts { get; set; }
        public bool ExportAllPhotos { get; set; }
        public bool ExportBarCode { get; set; }

        public bool TypeExportYandex { get; set; }
        public bool NeedZip { get;  set; }
        public bool OnlyMainOfferToExport { get; set; }        

        public bool ExportDimensions { get; set; }
        
        public bool DontExportProductsWithoutDimensionsAndWeight { get; set; }

        public decimal? NotExportAmountCount { get; set; }

        public List<ExportFeedYandexWebmasterPromo> Promos { get; set; }

        public string PromosJson { get; set; }

        public string PromoCodesJson { get; set; }
        public string FlashDiscountsJson { get; set; }
        public string PromoGiftsJson { get; set; }
        public string NPlusMJson { get; set; }

        public bool PriceRulesEnabled => !SaasDataService.IsSaasEnabled || SaasDataService.CurrentSaasData.PriceTypes;
        public int? PriceRuleId { get; set; }
        public int? PriceRuleIdForOldPrice { get; set; }
        
        public decimal? PriceFrom { get; set; }
        public decimal? PriceTo { get; set; }

        public bool ConsiderMultiplicityInPrice { get; set; }
        
        public List<int> WarehouseIds { get; set; }

        private List<PriceRule> _priceRules;
        public List<PriceRule> PriceRules => _priceRules ?? (_priceRules = PriceRuleService.GetList());
        
        public List<SelectListItem> PriceRulesList
        {
            get
            {
                var list = new List<SelectListItem>() {new SelectListItem() {Text = "Не выбран"}};
                
                if (PriceRulesEnabled)
                {
                    list.AddRange(PriceRules.Select(x => new SelectListItem()
                        {Text = x.Name, Value = x.Id.ToString()}));
                }

                return list;
            }
        }

        public ExportFeedYandexWebmasterDeliveryCostOption LocalDeliveryOption { get; set; }

        public List<ExportFeedYandexWebmasterDeliveryCostOption> GlobalDeliveryCost { get; set; }

        public string GlobalDeliveryCostJson { get; set; }

        public Dictionary<ExportFeedYandexWebmasterDeliveryCost, string> DeliveryCostList
        {
            get
            {
                var deliveryCostList = new Dictionary<ExportFeedYandexWebmasterDeliveryCost, string>();
                foreach (ExportFeedYandexWebmasterDeliveryCost deliveryCost in Enum.GetValues(typeof(ExportFeedYandexWebmasterDeliveryCost)))
                {
                    deliveryCostList.Add(deliveryCost, deliveryCost.Localize());
                }
                return deliveryCostList;
            }
        }

        public Dictionary<string, string> Currencies
        {
            get
            {
                var currencyList = new Dictionary<string, string>();
                foreach (var item in CurrencyService.GetAllCurrencies().Where(item => ExportFeedYandexWebmaster.AvailableCurrencies.Contains(item.Iso3)).ToList())
                {
                    currencyList.Add(item.Iso3, item.Name);
                }
                return currencyList;
            }
        }

        public Dictionary<string, string> ProductDescriptionTypeList =>
            new Dictionary<string, string>
            {
                {"short", LocalizationService.GetResource("Admin.ExportFeed.Settings.BriefDescription")},
                {"full", LocalizationService.GetResource("Admin.ExportFeed.Settings.FullDescription")},
                {"none", LocalizationService.GetResource("Admin.ExportFeed.Settings.DontUseDescription")}
            };

        public Dictionary<string, string> OfferTypes =>
            new Dictionary<string, string>
            {
                {"id", LocalizationService.GetResource("Admin.ExportFeed.Settings.OfferId")},
                {"artno", LocalizationService.GetResource("Admin.ExportFeed.Settings.OfferSku")}
            };

        public Dictionary<string, string> VendorCodeTypes =>
            new Dictionary<string, string>()
            {
                {"productArtno", LocalizationService.GetResource("Admin.ExportFeed.Settings.ProductArtNo")},
                {"offerArtno", LocalizationService.GetResource("Admin.ExportFeed.Settings.OfferArtNo")},
            };

        public Dictionary<string, string> DeliveryCostTypes =>
            new Dictionary<string, string> {
                {ExportFeedYandexDeliveryCost.None.ToString(),ExportFeedYandexDeliveryCost.None.Localize()},
                {ExportFeedYandexDeliveryCost.GlobalDeliveryCost.ToString(),ExportFeedYandexDeliveryCost.GlobalDeliveryCost.Localize() },
                {ExportFeedYandexDeliveryCost.LocalDeliveryCost.ToString(),ExportFeedYandexDeliveryCost.LocalDeliveryCost.Localize() },
            };


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(ShopName))
            {
                yield return new ValidationResult(LocalizationService.GetResource("Admin.Category.AdminCategoryModel.Error.Name"), new[] { "ShopName" });
            }
            if (string.IsNullOrEmpty(CompanyName))
            {
                yield return new ValidationResult(LocalizationService.GetResource("Admin.Category.AdminCategoryModel.Error.Name"), new[] { "CompanyName" });
            }
        }
        
        public Dictionary<EExportFeedYandexWebmasterPriceType, string> ProductPriceTypeList =>
            Enum.GetValues(typeof(EExportFeedYandexWebmasterPriceType)).Cast<EExportFeedYandexWebmasterPriceType>().ToDictionary(priceType => priceType, priceType => priceType.Localize());
    }
    
    public class ExportFeedSettingsYandexWebmasterPromoModel : IValidatableObject
    {
        public Guid? PromoID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PromoUrl { get; set; }
        public string Type { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? ExpirationDate { get; set; }

        public List<int> ProductIDs { get; set; }
        public int RequiredQuantity { get; set; }
        public int FreeQuantity { get; set; }
        public int GiftID { get; set; }
        public List<int> CategoryIDs { get; set; }

        #region PromoCode

        public int CouponId { get; set; }
        private Coupon _coupon { get; set; }
        public Coupon Coupon
        {
            get
            {
                if (_coupon != null)
                {
                    return _coupon;
                }
                else
                {
                    _coupon = CouponService.GetCoupon(CouponId);
                    return _coupon;
                }
            }
        }

        #endregion

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var type = Type.TryParseEnum<YandexWebmasterPromoType>();
            if (StartDate != null && ExpirationDate != null)
            {
                var delta = ExpirationDate - StartDate;
                if (type == YandexWebmasterPromoType.Flash && delta.Value.Days > 7)
                {
                    yield return new ValidationResult("Указанная длительность акции больше 7 дней.");
                }
                if (delta.Value < new TimeSpan(0,1,0))
                {
                    yield return new ValidationResult("Указанная длительность акции недопустима.");
                }
            }
            else if ((StartDate != null && ExpirationDate == null) || (StartDate == null && ExpirationDate != null))
            {
                yield return new ValidationResult("Необходимо указать обе даты.");
            }

            if (type == YandexWebmasterPromoType.PromoCode)
            {
                if (CouponId <= 0)
                    yield return new ValidationResult("Неверный код купона.");
            }
            else
            {
                if(ProductIDs == null || ProductIDs.Count == 0)
                {
                    if(type == YandexWebmasterPromoType.NPlusM )
                    {
                        if (CategoryIDs == null || CategoryIDs.Count == 0)
                        {
                            yield return new ValidationResult("Выберите по крайней мере один продукт или категорию.");
                        }
                    }
                    else
                    {
                        yield return new ValidationResult("Выберите по крайней мере один продукт.");
                    }

                }
                if (type == YandexWebmasterPromoType.Gift)
                {
                    if (RequiredQuantity <= 0 || RequiredQuantity > 24)
                    {
                        yield return new ValidationResult("Количество товаров, которое нужно приобрести должно быть от 1 до 24.");
                    }
                    if (GiftID == 0)
                    {
                        yield return new ValidationResult("Выберите продукт в качестве подарка.");
                    }
                }
                if (type == YandexWebmasterPromoType.NPlusM)
                {
                    if(RequiredQuantity <= 0 || RequiredQuantity > 24 || FreeQuantity <= 0 || FreeQuantity > 24)
                    {
                        yield return new ValidationResult("Можно добавить от 1 до 24 товаров за полную цену/бонусных товаров.");
                    }
                }
            }
        }
    }
}
