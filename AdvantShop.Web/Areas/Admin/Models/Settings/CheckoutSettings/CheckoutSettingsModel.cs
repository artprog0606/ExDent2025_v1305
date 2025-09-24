using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Saas;
using AdvantShop.Shipping;
using AdvantShop.Web.Admin.ViewModels.Analytics;
using AdvantShop.Web.Admin.ViewModels.Catalog.Import;

namespace AdvantShop.Web.Admin.Models.Settings.CheckoutSettings
{
    public class CheckoutSettingsModel : IValidatableObject
    {
        public bool AmountLimitation { get; set; }

        public eOutOfStockAction OutOfStockAction { get; set; }
        public List<SelectListItem> OutOfStockActions { get; set; }
        
        public int OutOfStockSalesFunnelId { get; set; }
        public List<SelectListItem> OutOfStockSalesFunnels { get; set; }
        
        public bool IsShowOutOfStockSalesFunnelId
        {
            get { return !SaasDataService.IsSaasEnabled || SaasDataService.CurrentSaasData.HaveCrm; }
        }

        public TypeSortOrderShippings TypeSortOrderShippings { get; set; }
        public List<SelectListItem> TypesSortOrderShippings { get; set; }

        public bool TaxTypeByPaymentMethodType { get; set; }

        public float MinimalOrderPriceForDefaultGroup { get; set; }

        public bool ProceedToPayment { get; set; }

        public bool ManagerConfirmed { get; set; }
        public bool SplitShippingByType { get; set; }
        public EnTypeOfDelivery DefaultShippingType { get; set; }
        public List<SelectListItem> ShippingTypes { get; set; }

        [Obsolete("Not actual for 6.0 and grater")]
        public bool DenyToByPreorderedProductsWithZerroAmount { get; set; }

        public bool BuyInOneClick { get; set; }
        public bool BuyInOneClickDisableInCheckout { get; set; }
        public string BuyInOneClickLinkText { get; set; }
        public string BuyInOneClickFirstText { get; set; }
        public string BuyInOneClickButtonText { get; set; }

        public string BuyInOneClickAction { get; set; }
        public List<SelectListItem> BuyInOneClickActions { get; set; }

        public int BuyInOneClickDefaultShippingMethod { get; set; }
        public List<SelectListItem> ShippingMethods { get; set; }

        public int BuyInOneClickDefaultPaymentMethod { get; set; }
        public List<SelectListItem> PaymentMethods { get; set; }

        public bool EnableGiftCertificateService { get; set; }
        public bool DisplayPromoTextbox { get; set; }
        public float MaximalPriceCertificate { get; set; }
        public float MinimalPriceCertificate { get; set; }

        public bool MultiplyGiftsCount { get; set; }

        public bool PrintOrder_ShowStatusInfo { get; set; }
        public bool PrintOrder_ShowMap { get; set; }
        public string PrintOrder_MapType { get; set; }
        public List<SelectListItem> MapTypes { get; set; }

        public string OrderNumberFormat { get; set; }

        public int NextOrderNumber { get; set; }

        public string SuccessOrderScript { get; set; }

        public bool ShowClientId { get; set; }
        public bool EnableLogingInTariff { get; set; }

        public bool EnableShoppingCartPopup { get; set; }

        public bool ZipDisplayPlace { get; set; }


        public bool IsLandingFunnelsEnabled
        {
            get { return SettingsLandingPage.ActiveLandingPage; }
        }

        public bool UseCrossSellLandingsInCheckout { get; set; }

        public bool IsShowOrderRecipient { get; set; }

        public bool IsShowDontCallBack { get; set; }

        #region checkout fields

        // customer
        public string CustomerFirstNameField { get; set; }

        public bool IsShowLastName { get; set; }
        public bool IsRequiredLastName { get; set; }

        public bool IsShowPatronymic { get; set; }
        public bool IsRequiredPatronymic { get; set; }

        public string CustomerPhoneField { get; set; }
        public bool IsShowPhone { get; set; }
        public bool IsRequiredPhone { get; set; }

        public string BirthDayFieldName { get; set; }
        public bool IsShowBirthDay { get; set; }
        public bool IsRequiredBirthDay { get; set; }
        public bool IsDisableEditingBirthDay { get; set; }

        // checkout
        public bool IsShowCountry { get; set; }
        public bool IsRequiredCountry { get; set; }

        public bool IsShowState { get; set; }
        public bool IsRequiredState { get; set; }

        public bool IsShowCity { get; set; }
        public bool IsRequiredCity { get; set; }

        public bool IsShowDistrict { get; set; }
        public bool IsRequiredDistrict { get; set; }

        public bool IsShowZip { get; set; }
        public bool IsRequiredZip { get; set; }

        public bool IsShowAddress { get; set; }
        public bool IsRequiredAddress { get; set; }

        public bool IsShowFullAddress { get; set; }

        public bool IsShowUserComment { get; set; }

        public string CustomShippingField1 { get; set; }
        public bool IsShowCustomShippingField1 { get; set; }
        public bool IsReqCustomShippingField1 { get; set; }

        public string CustomShippingField2 { get; set; }
        public bool IsShowCustomShippingField2 { get; set; }
        public bool IsReqCustomShippingField2 { get; set; }

        public string CustomShippingField3 { get; set; }
        public bool IsShowCustomShippingField3 { get; set; }
        public bool IsReqCustomShippingField3 { get; set; }

        // buy one click
        public string BuyInOneClickName { get; set; }
        public bool IsShowBuyInOneClickName { get; set; }
        public bool IsRequiredBuyInOneClickName { get; set; }

        public string BuyInOneClickEmail { get; set; }
        public bool IsShowBuyInOneClickEmail { get; set; }
        public bool IsRequiredBuyInOneClickEmail { get; set; }


        public string BuyInOneClickPhone { get; set; }
        public bool IsShowBuyInOneClickPhone { get; set; }
        public bool IsRequiredBuyInOneClickPhone { get; set; }

        public string BuyInOneClickComment { get; set; }
        public bool IsShowBuyInOneClickComment { get; set; }
        public bool IsRequiredBuyInOneClickComment { get; set; }
        public int BuyInOneClickSalesFunnelId { get; set; }

        public bool IsShowBuyInOneClickSalesFunnelId
        {
            get { return !SaasDataService.IsSaasEnabled || SaasDataService.CurrentSaasData.HaveCrm; }
        }

        public List<SelectListItem> BuyInOneClickSalesFunnels { get; set; }

        public bool IsShowEmail { get; set; }
        public bool IsRequiredEmail { get; set; }

        #endregion


        #region ThankYouPage

        public EThankYouPageActionType TYPageAction { get; set; }
        public List<SelectListItem> TYPageActions { get; set; }

        public List<SocialNetworkGroup> TYSocialNetworks { get; set; }
        public string TYSocialNetworksSerialized { get; set; }

        public string TYNameOfBlockProducts { get; set; }
        public bool TYShowReletedProducts { get; set; }
        public RelatedType TYReletedProductsType { get; set; }
        public List<SelectListItem> TYReletedProductsTypes { get; set; }

        public bool TYShowProductsList { get; set; }
        public string TYProductsList { get; set; }
        public List<SelectListItem> TYProductsLists { get; set; }

        public bool TYShowSelectedProducts { get; set; }

        public string TYExcludedPaymentIdsSerialized { get; set; }
        public List<SelectListItem> TYPaymentMethods { get; set; }

        #endregion

        public ExportOrdersModel ExportOrdersModel { get; set; }
        public ImportOrdersModel ImportOrdersModel { get; set; }

        public int CrossSellShowMode { get; set; }
        public List<SelectListItem> CrossSellShowModes { get; set; }
        public string PrintOrderMapApiKey { get; set; }


        public bool ShowCartItemsInBilling { get; set; }

        public string ShoppingCartMode { get; set; }
        public List<SelectListItem> ShoppingCartModeList { get; set; }

        public bool ReviewOnlyPaidOrder { get; set; }
        public bool AllowAddOrderReview { get; set; }
        public bool AllowEditAmount { get; set; }

        public EStoreWorkMode TypeStoreWorkMode { get; set; }
        public List<SelectListItem> TypeStoreWorkModeList { get; set; }
        public bool AllowUploadFiles { get; set; }
        public bool ShowBriefDescriptionProductInCheckout { get; set; }
        public int CheckoutImageHeight { get; set; }
        public int CheckoutImageWidth { get; set; }
        public bool ShowReceivingMethod { get; set; }
        public bool ShowCountDevices { get; set; }
        public bool ClearShoppingCartBeforeBuyByLink { get; set; }
        public bool HideShippingNotAvailableForWarehouse { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (MinimalOrderPriceForDefaultGroup < 0)
            {
                yield return new ValidationResult("Введите корректную минимальную сумму заказа");
            }

            if (MinimalPriceCertificate <= 0)
            {
                yield return new ValidationResult("Введите корректную минимальную сумму сертификата");
            }

            if (string.IsNullOrWhiteSpace(OrderNumberFormat) || (!OrderNumberFormat.Contains("#NUMBER#") && !ContainsRandomFormat(OrderNumberFormat)))
            {
                yield return new ValidationResult("Неправильный формат номера заказа");
            }
        }

        private bool ContainsRandomFormat(string format)
        {
            var startStr = "#R";
            var endStr = "R#";
            var currentIndex = 0;
            int startStrIndex, endStrIndex;

            while (currentIndex < format.Length - 1 &&
                   (startStrIndex = format.IndexOf(startStr, currentIndex, StringComparison.Ordinal)) > -1 &&
                   (endStrIndex = format.IndexOf(endStr, startStrIndex, StringComparison.Ordinal)) > -1)
            {
                var countR = endStrIndex - startStrIndex;
                var valid = format
                    .Substring(startStrIndex + 1, countR)
                    .All(c => c == 'R');

                if (valid && countR >= 3)
                    return true;

                currentIndex = startStrIndex + 1;
            }
            return false;
        }
    }
}
