using AdvantShop.Catalog;
using AdvantShop.Core.Common.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AdvantShop.ExportImport
{
    [Serializable()]
    public class ExportFeedYandexPromo
    {
        [JsonProperty(PropertyName = "PromoID")]
        public Guid PromoID { get; set; }

        [JsonProperty(PropertyName = "Type")]
        public YandexPromoType Type { get; set; }

        [JsonProperty(PropertyName = "Name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "Description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "PromoUrl")]
        public string PromoUrl { get; set; }

        [JsonProperty(PropertyName = "StartDate")]
        public DateTime? StartDate { get; set; }

        [JsonProperty(PropertyName = "ExpirationDate")]
        public DateTime? ExpirationDate { get; set; }

        #region FlashDiscount/Gifts/NPluSM
        [JsonProperty(PropertyName = "ProductIDs")]
        public List<int> ProductIDs { get; set; }

        [JsonProperty(PropertyName = "RequiredQuantity")]
        public int RequiredQuantity { get; set; }
        
        [JsonProperty(PropertyName = "FreeQuantity")]
        public int FreeQuantity { get; set; }

        [JsonProperty(PropertyName = "GiftID")]
        public int GiftID { get; set; }

        [JsonProperty(PropertyName = "CategoryIDs")]
        public List<int> CategoryIDs { get; set; }

        #endregion

        #region PromoCode
        [JsonProperty(PropertyName = "CouponId")]
        public int CouponId { get; set; }
        #endregion
        
    }

    public enum YandexPromoType
    {
        [Localize("Core.ExportImport.ExportFeed.YandexPromoType.PromoCode")]
        PromoCode,
        [Localize("Core.ExportImport.ExportFeed.YandexPromoType.Gift")]
        Gift,
        [Localize("Core.ExportImport.ExportFeed.YandexPromoType.FlashDiscount")]
        Flash,
        [Localize("Core.ExportImport.ExportFeed.YandexPromoType.NPlusM")]
        NPlusM
    }
}
