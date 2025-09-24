using System;
using System.Collections.Generic;
using System.Web;
using AdvantShop.Catalog;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Services.Landing.Pictures;
using Newtonsoft.Json;


namespace AdvantShop.Core.Services.Landing.Forms
{
    public enum LpButtonAction
    {
        Url,
        Section,
        Form,
        Checkout,
        CheckoutUpsell,
        ModalClose
    }

    public enum eLpButtonType
    {
        [StringName("lp-btn--primary")]
        Primary = 0,
        [StringName("lp-btn--secondary")]
        Secondary = 1,
        [StringName("lp-btn--link")]
        Link = 2
    }

    public enum eLpButtonSize
    {
        [StringName("lp-btn--xs")]
        XSmall = 0,
        [StringName("lp-btn--md")]
        Middle = 2
    }

    public class LpButtonGoals {
        public bool Enabled { get; set; }
        public string YaMetrikaEventName { get; set; }
        public string GaEventCategory { get; set; }
        public string GaEventAction { get; set; }
    }

    public class LpButton : IConvertibleBlockModel
    {
        public string Name { get; set; }

        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "action")]
        public string Action { get; set; }

        [JsonProperty(PropertyName = "action_url")]
        public string ActionUrl { get; set; }

        [JsonProperty(PropertyName = "action_url_lp_id")]
        public string ActionUrlLpId { get; set; }

        [JsonProperty(PropertyName = "action_section")]
        public string ActionSection { get; set; }

        [JsonProperty(PropertyName = "action_form")]
        public string ActionForm { get; set; }

        [JsonProperty(PropertyName = "action_offer_id")]
        public string ActionOfferId { get; set; }

        [JsonProperty(PropertyName = "action_offer_price")]
        public string ActionOfferPrice { get; set; }
        
        [JsonProperty(PropertyName = "action_shipping_price")]
        public string ActionShippingPrice { get; set; }

        [JsonProperty(PropertyName = "action_hide_shipping")]
        public bool? ActionHideShipping { get; set; }

        [JsonProperty(PropertyName = "action_upsell_lp_id")]
        public string ActionUpsellLpId { get; set; }

        [JsonProperty(PropertyName = "size")]
        public eLpButtonSize Size { get; set; }

        [JsonProperty(PropertyName = "type")]
        public eLpButtonType Type { get; set; }

        [JsonProperty(PropertyName = "additional_data")]
        public string AdditionalData { get; set; }

        [JsonProperty(PropertyName = "target_blank")]
        public bool TargetBlank { get; set; }

        public int BlockId { get; set; }

        [JsonProperty(PropertyName = "show_button")] 
        public bool? Show { get; set; }

        [JsonProperty(PropertyName = "callback")]
        public string Callback { get; set; }

        public LpButtonGoals Goals { get; set; }

        [JsonProperty(PropertyName = "use_many_offers")]
        public bool? UseManyOffers { get; set; }

        [JsonProperty(PropertyName = "action_offer_items")]
        public List<LpButtonOfferItem> ActionOfferItems { get; set; }

        [JsonProperty(PropertyName = "show_options")]
        public bool? ShowOptions { get; set; }

        [JsonProperty(PropertyName = "picture")]
        public PhotoModel Picture { get; set; }

        [JsonProperty(PropertyName = "discount")]
        public LpDiscount Discount { get; set; }

        [JsonProperty(PropertyName = "align")]
        public string Align { get; set; }
        public string ModalClass { get; set; }

        public IConvertibleBlockModel ConvertFromType(object obj, Type type)
        {
            throw new NotImplementedException();
        }

        public IConvertibleBlockModel ConvertToType(Type type)
        {
            throw new NotImplementedException();
        }

        public bool IsNull()
        {
            throw new NotImplementedException();
        }
    }


    public class LpButtonOfferItem
    {
        [JsonProperty(PropertyName = "offerId")]
        public string OfferId { get; set; }

        [JsonProperty(PropertyName = "offerPrice")]
        public string OfferPrice { get; set; }

        [JsonProperty(PropertyName = "offerAmount")]
        public string OfferAmount { get; set; }
    }

    public class LpDiscount
    {
        [JsonProperty(PropertyName = "amount")]
        public float Amount { get; set; }

        [JsonProperty(PropertyName = "type")]
        public DiscountType Type { get; set; }


        public static implicit operator Discount(LpDiscount discount)
        {
            discount = discount ?? new LpDiscount();
            return new Discount(discount.Type == DiscountType.Percent ? discount.Amount : 0,
                                discount.Type == DiscountType.Amount ? discount.Amount : 0, 
                                discount.Type);
        }
    }
}
