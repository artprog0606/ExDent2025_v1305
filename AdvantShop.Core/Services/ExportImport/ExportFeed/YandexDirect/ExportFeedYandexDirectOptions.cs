using System;
using System.Collections.Generic;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using Newtonsoft.Json;

namespace AdvantShop.ExportImport
{
    [Serializable]
    public class ExportFeedYandexDirectOptions : IExportFeedFilterOptions
    {
        public bool ExportNotAvailable { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool AllowPreOrderProducts { get; set; } = true; // true, если отсутствует в json или равен null.
        public bool OnlyMainOfferToExport { get; set; }
        public int? PriceRuleId { get; set; }
        public int? PriceRuleIdForOldPrice { get; set; }
        public decimal? NotExportAmountCount { get; set; }
        public bool DontExportProductsWithoutDimensionsAndWeight { get; set; }
        public List<int> WarehouseIds { get; set; }


        [JsonProperty(PropertyName = "Currency")]
        public string Currency { get; set; }

        [JsonProperty(PropertyName = "RemoveHtml")]
        public bool RemoveHtml { get; set; }

        [JsonProperty(PropertyName = "Delivery")]
        public bool Delivery { get; set; }
        
        [JsonIgnore]
        private string _delivery;
        
        [JsonIgnore]
        public string DeliveryString => _delivery ?? (_delivery = Delivery.ToLowerString());

        [JsonProperty(PropertyName = "Pickup")]
        public bool Pickup { get; set; }
        
        [JsonIgnore]
        private string _pickup;
        
        [JsonIgnore]
        public string PickupString => _pickup ?? (_pickup = Pickup.ToLowerString());


        //[JsonProperty(PropertyName = "LocalDeliveryCost")]
        //public bool LocalDeliveryCost { get; set; }

        [JsonProperty(PropertyName = "DeliveryCost")]
        public ExportFeedYandexDirectDeliveryCost DeliveryCost { get; set; }

        [JsonProperty(PropertyName = "GlobalDeliveryCost")]
        public string GlobalDeliveryCost { get; set; }

        private string _localDeliveryOption;
        [JsonProperty(PropertyName = "LocalDeliveryOption")]
        public string LocalDeliveryOption
        {
            get { return _localDeliveryOption; }
            set
            {
                _localDeliveryOption = value;
                _localDeliveryOptionObject = null;
            }
        }

        private ExportFeedYandexDirectDeliveryCostOption _localDeliveryOptionObject;
        [JsonIgnore]
        public ExportFeedYandexDirectDeliveryCostOption LocalDeliveryOptionObject
        {
            get { return _localDeliveryOptionObject ?? (_localDeliveryOptionObject = GetLocalDeliveryOption(LocalDeliveryOption)); }
        }

        [JsonProperty(PropertyName = "ExportProductProperties")]
        public bool ExportProductProperties { get; set; }
        
        [JsonProperty(PropertyName = "ExportPropertyDisplayedName")]
        public bool ExportPropertyDisplayedName { get; set; }

        [JsonProperty(PropertyName = "JoinPropertyValues")]
        public bool JoinPropertyValues { get; set; }
        
        [JsonProperty(PropertyName = "ExportOnlyUseInDetailsProperties")]
        public bool ExportOnlyUseInDetailsProperties { get; set; }
        
        [JsonProperty(PropertyName = "ExportOnlyUseInDetailsPropertiesExceptionIds")]
        public List<int> ExportOnlyUseInDetailsPropertiesExceptionIds { get; set; }

        [Obsolete("use ProductPriceType")]
        [JsonProperty(PropertyName = "ExportProductDiscount")]
        public bool ExportProductDiscount
        {
            set => ProductPriceType = value
                    ? EExportFeedYandexDirectPriceType.Both
                    : EExportFeedYandexDirectPriceType.WithDiscount;
        }
        
        [JsonProperty(PropertyName = "ProductPriceType")]
        public EExportFeedYandexDirectPriceType ProductPriceType { get; set; }
        
        [JsonProperty(PropertyName = "SalesNotes")]
        public string SalesNotes { get; set; }

        [JsonIgnore] private string _salesNotesEncoded;
        [JsonIgnore] 
        public string SalesNotesEncoded => _salesNotesEncoded ?? 
                                           (_salesNotesEncoded = SalesNotes.XmlEncode().RemoveInvalidXmlChars());

        [JsonProperty(PropertyName = "ShopName")]
        public string ShopName { get; set; }

        [JsonProperty(PropertyName = "CompanyName")]
        public string CompanyName { get; set; }

        [JsonProperty(PropertyName = "ColorSizeToName")]
        public bool ColorSizeToName { get; set; }

        [JsonProperty(PropertyName = "ProductDescriptionType")]
        public string ProductDescriptionType { get; set; }

        [JsonProperty(PropertyName = "OfferIdType")]
        public string OfferIdType { get; set; }

        [JsonProperty(PropertyName = "VendorCodeType")]
        public string VendorCodeType { get; set; }

        [JsonProperty(PropertyName = "ExportPurchasePrice")]
        public bool ExportPurchasePrice { get; set; }

        [JsonProperty(PropertyName = "ExportCount")]
        public bool ExportCount { get; set; }

        [JsonProperty(PropertyName = "ExportShopSku")]
        public bool ExportShopSku { get; set; }

        [JsonProperty(PropertyName = "ExportManufacturer")]
        public bool ExportManufacturer { get; set; }


        [JsonProperty(PropertyName = "ExportBarCode")]
        public bool ExportBarCode { get; set; }

        [JsonProperty(PropertyName = "Store")]
        public bool Store { get; set; }

        [JsonIgnore]
        private string _store;
        
        [JsonIgnore]
        public string StoreString => _store ?? (_store = Store.ToLowerString());

        [JsonProperty(PropertyName = "ExportRelatedProducts")]
        public bool ExportRelatedProducts { get; set; }

        [JsonProperty(PropertyName = "ExportAllPhotos")]
        public bool ExportAllPhotos { get; set; }

        [JsonProperty(PropertyName = "TypeExportYandex")]
        public bool TypeExportYandex { get; set; }

        [JsonProperty(PropertyName = "NeedZip")]
        public bool NeedZip { get;  set; }

        [JsonProperty(PropertyName = "ExportDimensions")]
        public bool ExportDimensions { get; set; }

        [JsonProperty(PropertyName = "DontExportCurrency")]
        public bool DontExportCurrency { get; set; }

        [JsonProperty(PropertyName = "Promos")]
        public string Promos { get; set; }
        private static ExportFeedYandexDirectDeliveryCostOption GetLocalDeliveryOption(string localDeliveryOptionString)
        {
            var localDeliveryOption = new ExportFeedYandexDirectDeliveryCostOption();

            try
            {
                localDeliveryOption =
                    JsonConvert.DeserializeObject<ExportFeedYandexDirectDeliveryCostOption>(localDeliveryOptionString);
            }
            catch (Exception)
            {
            }
            finally
            {
                if (localDeliveryOption == null)
                    localDeliveryOption = new ExportFeedYandexDirectDeliveryCostOption();
            }
            return localDeliveryOption;
        }
        
        
        [JsonProperty(PropertyName = "ExportExpiry")]
        public bool ExportExpiry { get; set; }
        
        [JsonProperty(PropertyName = "ExportWarrantyDays")]
        public bool ExportWarrantyDays { get; set; }
        
        [JsonProperty(PropertyName = "ExportCommentWarranty")]
        public bool ExportCommentWarranty { get; set; }
        
        [JsonProperty(PropertyName = "ExportPeriodOfValidityDays")]
        public bool ExportPeriodOfValidityDays { get; set; }
        
        [JsonProperty(PropertyName = "ExportServiceLifeDays")]
        public bool ExportServiceLifeDays { get; set; }
        
        [JsonProperty(PropertyName = "ExportMarketTnVedCode")]
        public bool ExportMarketTnVedCode { get; set; }
        
        [JsonProperty(PropertyName = "ExportMarketStepQuantity")]
        public bool ExportMarketStepQuantity { get; set; }
        
        [JsonProperty(PropertyName = "ExportMarketMinQuantity")]
        public bool ExportMarketMinQuantity { get; set; }
        
        [JsonProperty(PropertyName = "ExportMarketCisRequired")]
        public bool ExportMarketCisRequired { get; set; }
        
        [JsonProperty(PropertyName = "PriceFrom")]
        public decimal? PriceFrom { get; set; }
        
        [JsonProperty(PropertyName = "PriceTo")]
        public decimal? PriceTo { get; set; }
        
        [JsonProperty(PropertyName = "ConsiderMultiplicityInPrice")]
        public bool ConsiderMultiplicityInPrice { get; set; }

        [JsonProperty(PropertyName = "ExportCollections")]
        public bool ExportCollections { get; set; }
    }

    public enum ExportFeedYandexDirectDeliveryCost
    {
        [Localize("Core.ExportImport.ExportFeedYandexDeliveryCost.None")]
        None = 0,

        [Localize("Core.ExportImport.ExportFeedYandexDeliveryCost.LocalDeliveryCost")]
        LocalDeliveryCost = 1,

        [Localize("Core.ExportImport.ExportFeedYandexDeliveryCost.GlobalDeliveryCost")]
        GlobalDeliveryCost = 2
    }

    public class ExportFeedYandexDirectDeliveryCostOption
    {
        public string Cost { get; set; }

        public string Days { get; set; }

        public string OrderBefore { get; set; }
    }

    public enum EExportFeedYandexDirectPriceType
    {
        [Localize("Core.ExportImport.ExportFeedYandexPriceType.WithDiscount")]
        WithDiscount,
        [Localize("Core.ExportImport.ExportFeedYandexPriceType.WithoutDiscount")]
        WithoutDiscount,
        [Localize("Core.ExportImport.ExportFeedYandexPriceType.Both")]
        Both
    }
}
