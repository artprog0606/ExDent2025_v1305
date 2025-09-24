using System;
using System.Collections.Generic;
using System.Web;
using AdvantShop.Catalog;
using Newtonsoft.Json;

namespace AdvantShop.Core.Services.Landing.Forms
{
    /// <summary>
    /// Форма блока
    /// </summary>
    [Serializable]
    public class LpForm : IConvertibleBlockModel
    {
        public int Id { get; set; }

        public int? BlockId { get; set; }

        public int LpId { get; set; }

        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string ButtonText { get; set; }

        public FormPostAction PostAction { get; set; }

        public bool? DontSendLeadId { get; set; }

        public string PostMessageText { get; set; }

        /// <summary>
        /// Если не null, то редирект на страницу лендинга, если null, то на PostMessageRedirectUrl
        /// </summary>
        public string PostMessageRedirectLpId { get; set; }
        public string PostMessageRedirectUrl { get; set; }

        public int? PayProductOfferId { get; set; }

        public string PayProductName
        {
            get
            {
                if (!PayProductOfferId.HasValue)
                    return "";

                var offer = OfferService.GetOffer(PayProductOfferId.Value);
                if (offer == null)
                    return "";

                return string.Format("{0} ({1})", offer.Product.Name, offer.ArtNo);
            }
        }

        public int? SalesFunnelId { get; set; }

        public string YaMetrikaEventName { get; set; }
        public string GaEventCategory { get; set; }
        public string GaEventAction { get; set; }

        private string FieldsJson { get; set; }
        
        private List<LpFormField> _fields;
        
        public List<LpFormField> Fields
        {
            get
            {
                if (_fields != null)
                    return _fields;

                return _fields = !string.IsNullOrEmpty(FieldsJson)
                    ? JsonConvert.DeserializeObject<List<LpFormField>>(FieldsJson)
                    : new List<LpFormField>();
            }
            set { _fields = value; }
        }

        public bool? ShowAgreement { get; set; }
        public string ModalClass { get; set; }
        public bool? AgreementDefaultChecked { get; set; }
        public string AgreementText { get; set; }
        
        [JsonProperty(PropertyName = "is_hidden")]
        public bool IsHidden { get; set; }

        public string EmailSubject { get; set; }
        public string EmailText { get; set; }

        public string OfferId { get; set; }
        public string OfferPrice { get; set; }

        private string OfferItemsJson { get; set; }

        private List<LpFormOfferItem> _offerItems;
        public List<LpFormOfferItem> OfferItems
        {
            get
            {
                if (_offerItems != null)
                    return _offerItems;

                return _offerItems = !string.IsNullOrEmpty(OfferItemsJson)
                    ? JsonConvert.DeserializeObject<List<LpFormOfferItem>>(OfferItemsJson)
                    : new List<LpFormOfferItem>();
            }
            set { _offerItems = value; }
        }
        
        public eInputTextPosition InputTextPosition { get; set; }

        public int? ActionUpsellLpId { get; set; }

        public bool? PostMessageRedirectShowMessage { get; set; }
        public int? PostMessageRedirectDelay { get; set; }

        public IConvertibleBlockModel ConvertToType(Type type)
        {
            throw new NotImplementedException();
        }

        public bool IsNull()
        {
            throw new NotImplementedException();
        }

        public IConvertibleBlockModel ConvertFromType(object obj, Type type)
        {
            throw new NotImplementedException();
        }

        public LpForm Clone()
        {
            return new LpForm()
            {
                Id = this.Id,
                BlockId = this.BlockId,
                LpId = LpId,
                Title = Title,
                SubTitle = SubTitle,
                ButtonText = ButtonText,
                PostAction = PostAction,
                DontSendLeadId = DontSendLeadId,
                PostMessageText = PostMessageText,
                PostMessageRedirectLpId = PostMessageRedirectLpId,
                PostMessageRedirectUrl = PostMessageRedirectUrl,
                PayProductOfferId = PayProductOfferId,
                SalesFunnelId = SalesFunnelId,
                YaMetrikaEventName = YaMetrikaEventName,
                GaEventCategory = GaEventCategory,
                GaEventAction = GaEventAction,
                FieldsJson = FieldsJson,
                Fields = Fields,
                ShowAgreement = ShowAgreement,
                AgreementText = AgreementText,
                IsHidden = IsHidden,
                EmailSubject = EmailSubject,
                EmailText = EmailText,
                OfferId = OfferId,
                OfferPrice = OfferPrice,
                OfferItemsJson = OfferItemsJson,
                _offerItems = _offerItems,
                OfferItems = OfferItems,
                InputTextPosition = InputTextPosition,
                ActionUpsellLpId = ActionUpsellLpId,
                PostMessageRedirectShowMessage = PostMessageRedirectShowMessage,
                PostMessageRedirectDelay = PostMessageRedirectDelay
            };
        }
    }

    /// <summary>
    /// Действие после отправки формы
    /// </summary>
    public enum FormPostAction
    {
        None = 0,
        ShowMessage = 1,
        RedrectToUrl = 2,
        RedirectToCheckout = 3,
        RedrectToUrlAndEmail = 4,
        AddToCartAndRedrectToUrl = 5,
    }

    public enum eInputTextPosition
    {
        Inside = 0,
        Outside = 1
    }

    public class LpFormOfferItem
    {
        [JsonProperty(PropertyName = "offerId")]
        public string OfferId { get; set; }

        [JsonProperty(PropertyName = "offerPrice")]
        public string OfferPrice { get; set; }

        [JsonProperty(PropertyName = "offerAmount")]
        public string OfferAmount { get; set; }
    }
}
