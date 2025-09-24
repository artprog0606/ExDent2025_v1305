using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Localization;
using AdvantShop.ExportImport;
using AdvantShop.Repository.Currencies;
using AdvantShop.Saas;

namespace AdvantShop.Web.Admin.Models.Catalog.ExportFeeds
{
    public class ExportFeedSettingsAvitoModel : IValidatableObject
    {
        public ExportFeedSettingsAvitoModel(ExportFeedAvitoOptions options)
        {
            Currency = options.Currency;
            PublicationDateOffset = options.PublicationDateOffset;
            DurationOfPublicationInDays = options.DurationOfPublicationInDays;
            PaidPublicationOption = options.PaidPublicationOption;
            PaidServices = options.PaidServices;
            ManagerName = options.ManagerName;
            Phone = options.Phone;
            Address = options.Address;
            EmailMessages = options.EmailMessages;
            ExportNotAvailable = options.ExportNotAvailable;
            ProductDescriptionType = options.ProductDescriptionType;
            DefaultAvitoCategory = options.DefaultAvitoCategory;
            UnloadProperties = options.UnloadProperties;
            IsActiveAboveAdditionalDescription = options.IsActiveAboveAdditionalDescription;
            IsActiveBelowAdditionalDescription = options.IsActiveBelowAdditionalDescription;
            AboveAdditionalDescription = options.AboveAdditionalDescription;
            BelowAdditionalDescription = options.BelowAdditionalDescription;
            NotExportColorSize = options.NotExportColorSize;
            ExportMode = options.ExportMode;
            PriceRuleId = options.PriceRuleId;
        }

        public string Currency { get; set; }
        public int PublicationDateOffset { get; set; }
        public int DurationOfPublicationInDays { get; set; }
        public EPaidPublicationOption PaidPublicationOption { get; set; }
        public EPaidServices PaidServices { get; set; }
        public string ManagerName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public bool EmailMessages { get; set; }
        public bool ExportNotAvailable { get; set; }
        public string ProductDescriptionType { get; set; }
        public string DefaultAvitoCategory { get; set; }
        public bool UnloadProperties { get; set; }
        public bool IsActiveAboveAdditionalDescription { get; set; }
        public bool IsActiveBelowAdditionalDescription { get; set; }
        public string AboveAdditionalDescription { get; set; }
        public string BelowAdditionalDescription { get; set; }
        public bool NotExportColorSize { get; set; }
        public EAvitoExportMode ExportMode { get; set; }
        public int? PriceRuleId { get; set; }
        public bool PriceRulesEnabled => !SaasDataService.IsSaasEnabled || SaasDataService.CurrentSaasData.PriceTypes;
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

        public List<SelectItemModel<int>> ExportModes => 
            Enum.GetValues(typeof(EAvitoExportMode)).Cast<EAvitoExportMode>()
                .Select(x => new SelectItemModel<int>(x.Localize(), (int) x))
                .ToList();

        public Dictionary<string, string> ListPaidPublicationOption =>
            new Dictionary<string, string> {
                { EPaidPublicationOption.Package.StrName(),EPaidPublicationOption.Package.Localize()},
                { EPaidPublicationOption.PackageSingle.StrName(),EPaidPublicationOption.PackageSingle.Localize() },
                { EPaidPublicationOption.Single.StrName(),EPaidPublicationOption.Single.Localize() }
            };

        public Dictionary<string, string> ListPaidServices =>
            new Dictionary<string, string> {
                { EPaidServices.Free.StrName(),EPaidServices.Free.Localize()},
                { EPaidServices.Highlight.StrName(),EPaidServices.Highlight.Localize() },
                { EPaidServices.Premium.StrName(),EPaidServices.Premium.Localize() },
                { EPaidServices.PushUp.StrName(),EPaidServices.PushUp.Localize() },
                { EPaidServices.QuickSale.StrName(),EPaidServices.QuickSale.Localize() },
                { EPaidServices.TurboSale.StrName(),EPaidServices.TurboSale.Localize() },
                { EPaidServices.VIP.StrName(),EPaidServices.VIP.Localize() }
            };

        public Dictionary<string, string> ProductDescriptionTypeList =>
            new Dictionary<string, string> {
                { "short", LocalizationService.GetResource("Admin.ExportFeed.Settings.BriefDescription") },
                { "full", LocalizationService.GetResource("Admin.ExportFeed.Settings.FullDescription") }
                //,{ "none", LocalizationService.GetResource("Admin.ExportFeed.Settings.DontUseDescription") }
            };

        public Dictionary<string, string> Currencies
        {
            get
            {
                var currencyList = new Dictionary<string, string>();
                foreach (var item in CurrencyService.GetAllCurrencies()
                             .Where(item => ExportFeedAvito.AvailableCurrencies.Contains(item.Iso3)).ToList())
                {
                    currencyList.Add(item.Iso3, item.Name);
                }

                return currencyList;
            }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(ManagerName))
            {
                yield return new ValidationResult(LocalizationService.GetResource("Admin.Category.AdminCategoryModel.Error.Name"), new[] { "ShopName" });
            }
            if (string.IsNullOrEmpty(Phone))
            {
                yield return new ValidationResult(LocalizationService.GetResource("Admin.Category.AdminCategoryModel.Error.Name"), new[] { "CompanyName" });
            }
        }
    }
}
