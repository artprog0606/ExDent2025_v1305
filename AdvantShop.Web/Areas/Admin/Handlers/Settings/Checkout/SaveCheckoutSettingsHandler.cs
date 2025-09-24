using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Helpers;
using AdvantShop.Payment;
using AdvantShop.Web.Admin.Models.Settings.CheckoutSettings;
using Newtonsoft.Json;

namespace AdvantShop.Web.Admin.Handlers.Settings.Checkout
{
    public class SaveCheckoutSettingsHandler
    {
        private CheckoutSettingsModel _model;

        public SaveCheckoutSettingsHandler(CheckoutSettingsModel model)
        {
            _model = model;
        }

        public void Execute()
        {
            SettingsCheckout.BuyInOneClick = _model.BuyInOneClick;
            SettingsCheckout.BuyInOneClickDisableInCheckout = _model.BuyInOneClickDisableInCheckout;
            SettingsCheckout.BuyInOneClickFirstText = _model.BuyInOneClickFirstText;
            SettingsCheckout.BuyInOneClickButtonText = _model.BuyInOneClickButtonText;
            SettingsCheckout.BuyInOneClickCreateOrder = _model.BuyInOneClickAction == "order";
            SettingsCheckout.BuyInOneClickLinkText = _model.BuyInOneClickLinkText;
            SettingsCheckout.BuyInOneClickDefaultShippingMethod = _model.BuyInOneClickDefaultShippingMethod;
            SettingsCheckout.BuyInOneClickDefaultPaymentMethod = _model.BuyInOneClickDefaultPaymentMethod;
            SettingsCheckout.AmountLimitation = _model.AmountLimitation;
            SettingsCheckout.OutOfStockAction = _model.OutOfStockAction;
            SettingsCheckout.TypeSortOrderShippings = _model.TypeSortOrderShippings;
            SettingsCheckout.TaxTypeByPaymentMethodType = _model.TaxTypeByPaymentMethodType;

            if (_model.IsShowOutOfStockSalesFunnelId)
            {
                SettingsCheckout.OutOfStockSalesFunnelId = _model.OutOfStockSalesFunnelId;
            }
            
            if (_model.IsShowBuyInOneClickSalesFunnelId)
            {
                SettingsCrm.DefaultBuyInOneClickSalesFunnelId = _model.BuyInOneClickSalesFunnelId;
            }

            SettingsCheckout.DenyToByPreorderedProductsWithZerroAmount = _model.DenyToByPreorderedProductsWithZerroAmount;
            SettingsCheckout.ProceedToPayment = _model.ProceedToPayment;
            SettingsCheckout.MultiplyGiftsCount = _model.MultiplyGiftsCount;

            SettingsCheckout.PrintOrder_ShowStatusInfo = _model.PrintOrder_ShowStatusInfo;
            SettingsCheckout.PrintOrder_ShowMap = _model.PrintOrder_ShowMap;
            SettingsCheckout.PrintOrder_MapType = _model.PrintOrder_MapType;
            SettingsCheckout.PrintOrderMapApiKey = _model.PrintOrderMapApiKey;

            if (SettingsCheckout.EnableGiftCertificateService != _model.EnableGiftCertificateService)
            {
                var listMethod = PaymentService.GetAllPaymentMethods(true);
                var certificateMethod = listMethod.FirstOrDefault(x => x is IPaymentGiftCertificate);
                if (certificateMethod == null && _model.EnableGiftCertificateService)
                {
                    var paymentGiftCertificate = AdvantshopConfigService
                                                .GetDropdownPayments()
                                                .Select(model => model.Value)
                                                .Select(PaymentMethod.Create)
                                                .FirstOrDefault(method => method is IPaymentGiftCertificate) as IPaymentGiftCertificate;
                    if (paymentGiftCertificate is null)
                        paymentGiftCertificate =
                            AttachedModules.GetModules<Core.Modules.Interfaces.IPaymentMethod>()
                                           .Where(module => module != null)
                                           .Select(module => (Core.Modules.Interfaces.IPaymentMethod) Activator.CreateInstance(module))
                                           .Select(module => module.PaymentKey)
                                           .Select(PaymentMethod.Create)
                                           .FirstOrDefault(method => method is IPaymentGiftCertificate) as IPaymentGiftCertificate;
                    
                    if (paymentGiftCertificate != null)
                    {
                        certificateMethod = paymentGiftCertificate.GetDefaultPaymentCertificate();
                        PaymentService.AddPaymentMethod(certificateMethod);
                    }
                }
                else if (certificateMethod != null && !_model.EnableGiftCertificateService)
                {
                    // PaymentService.DeletePaymentMethod(certificateMethod.PaymentMethodId);
                    SettingsDesign.GiftSertificateVisibility = false;
                }
            }
            
            SettingsCheckout.EnableGiftCertificateService = _model.EnableGiftCertificateService;
            SettingsCheckout.DisplayPromoTextbox = _model.DisplayPromoTextbox;
            SettingsCheckout.MaximalPriceCertificate = _model.MaximalPriceCertificate;
            SettingsCheckout.MinimalPriceCertificate = _model.MinimalPriceCertificate;

            SettingsCheckout.MinimalOrderPriceForDefaultGroup = _model.MinimalOrderPriceForDefaultGroup;
            SettingsCheckout.ManagerConfirmed = _model.ManagerConfirmed;
            
            SettingsCheckout.SplitShippingByType = _model.SplitShippingByType;
            SettingsCheckout.DefaultShippingType = _model.DefaultShippingType;

            SettingsCheckout.SuccessOrderScript = _model.SuccessOrderScript;

            SettingsCheckout.OrderNumberFormat = _model.OrderNumberFormat.Trim().Replace("\t", "").Replace("\r", "").Replace("\n", "");

            #region checkout fields

            if (!Saas.SaasDataService.IsSaasEnabled || Saas.SaasDataService.CurrentSaasData.OrderAdditionFields)
            {
                SettingsCheckout.CustomerFirstNameField = _model.CustomerFirstNameField;
                SettingsCheckout.IsShowEmail = _model.IsShowEmail;
                SettingsCheckout.IsRequiredEmail = _model.IsRequiredEmail;
                SettingsCheckout.IsShowLastName = _model.IsShowLastName;
                SettingsCheckout.IsRequiredLastName = _model.IsRequiredLastName;
                SettingsCheckout.IsShowPatronymic = _model.IsShowPatronymic;
                SettingsCheckout.IsRequiredPatronymic = _model.IsRequiredPatronymic;
                SettingsCheckout.CustomerPhoneField = _model.CustomerPhoneField;
                SettingsCheckout.IsShowPhone = _model.IsShowPhone;
                SettingsCheckout.IsRequiredPhone = _model.IsRequiredPhone;

                SettingsCheckout.BirthDayFieldName = _model.BirthDayFieldName;
                SettingsCheckout.IsShowBirthDay = _model.IsShowBirthDay;
                SettingsCheckout.IsRequiredBirthDay = _model.IsRequiredBirthDay;
                SettingsCheckout.IsDisableEditingBirthDay = _model.IsDisableEditingBirthDay;

                // checkout
                SettingsCheckout.IsShowCountry = _model.IsShowCountry;
                SettingsCheckout.IsRequiredCountry = _model.IsRequiredCountry;
                SettingsCheckout.IsShowState = _model.IsShowState;
                SettingsCheckout.IsRequiredState = _model.IsRequiredState;
                SettingsCheckout.IsShowCity = _model.IsShowCity;
                SettingsCheckout.IsRequiredCity = _model.IsRequiredCity;
                SettingsCheckout.IsShowDistrict = _model.IsShowDistrict;
                SettingsCheckout.IsRequiredDistrict = _model.IsRequiredDistrict;
                SettingsCheckout.IsShowZip = _model.IsShowZip;
                SettingsCheckout.IsRequiredZip = _model.IsRequiredZip;
                SettingsCheckout.IsShowAddress = _model.IsShowAddress;
                SettingsCheckout.IsRequiredAddress = _model.IsRequiredAddress;
                SettingsCheckout.IsShowUserComment = _model.IsShowUserComment;
                SettingsCheckout.CustomShippingField1 = _model.CustomShippingField1;
                SettingsCheckout.IsShowCustomShippingField1 = _model.IsShowCustomShippingField1;
                SettingsCheckout.IsReqCustomShippingField1 = _model.IsReqCustomShippingField1;
                SettingsCheckout.CustomShippingField2 = _model.CustomShippingField2;
                SettingsCheckout.IsShowCustomShippingField2 = _model.IsShowCustomShippingField2;
                SettingsCheckout.IsReqCustomShippingField2 = _model.IsReqCustomShippingField2;
                SettingsCheckout.CustomShippingField3 = _model.CustomShippingField3;
                SettingsCheckout.IsShowCustomShippingField3 = _model.IsShowCustomShippingField3;
                SettingsCheckout.IsReqCustomShippingField3 = _model.IsReqCustomShippingField3;
                SettingsCheckout.IsShowFullAddress = _model.IsShowFullAddress;
                SettingsLandingPage.UseCrossSellLandingsInCheckout = _model.UseCrossSellLandingsInCheckout;
                SettingsCheckout.IsShowOrderRecipient = _model.IsShowOrderRecipient;
                SettingsCheckout.IsShowDontCallBack = _model.IsShowDontCallBack;

                // buy one click
                SettingsCheckout.BuyInOneClickName = _model.BuyInOneClickName;
                SettingsCheckout.IsShowBuyInOneClickName = _model.IsShowBuyInOneClickName;
                SettingsCheckout.IsRequiredBuyInOneClickName = _model.IsRequiredBuyInOneClickName;
                SettingsCheckout.BuyInOneClickEmail = _model.BuyInOneClickEmail;
                SettingsCheckout.IsShowBuyInOneClickEmail = _model.IsShowBuyInOneClickEmail;
                SettingsCheckout.IsRequiredBuyInOneClickEmail = _model.IsRequiredBuyInOneClickEmail;
                SettingsCheckout.BuyInOneClickPhone = _model.BuyInOneClickPhone;
                SettingsCheckout.IsShowBuyInOneClickPhone = _model.IsShowBuyInOneClickPhone;
                SettingsCheckout.IsRequiredBuyInOneClickPhone = _model.IsRequiredBuyInOneClickPhone;
                SettingsCheckout.BuyInOneClickComment = _model.BuyInOneClickComment;
                SettingsCheckout.IsShowBuyInOneClickComment = _model.IsShowBuyInOneClickComment;
                SettingsCheckout.IsRequiredBuyInOneClickComment = _model.IsRequiredBuyInOneClickComment;
                SettingsCheckout.ZipDisplayPlace = _model.ZipDisplayPlace;
                
                Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Settings_EditCheckoutFields);
            }
            #endregion

            SettingsDesign.ShowClientId = _model.ShowClientId;
            SettingsDesign.EnableShoppingCartPopup = _model.EnableShoppingCartPopup;
            
            #region ThankYouPage
            SettingsThankYouPage.ActionType = _model.TYPageAction;
            var SocialNetworks = JsonConvert.DeserializeObject<List<SocialNetworkGroup>>(_model.TYSocialNetworksSerialized);
            foreach (var sn in SocialNetworks)
                sn.Link = StringHelper.CompileUrl(sn.Link);
            SettingsThankYouPage.SocialNetworks = SocialNetworks;

            SettingsThankYouPage.NameOfBlockProducts = _model.TYNameOfBlockProducts;
            SettingsThankYouPage.ShowReletedProducts = _model.TYShowReletedProducts;
            SettingsThankYouPage.ReletedProductsType = _model.TYReletedProductsType;

            SettingsThankYouPage.ShowProductsList = _model.TYShowProductsList;
            if (_model.TYProductsList.IsInt())
            {
                SettingsThankYouPage.ProductsListType = EProductOnMain.List;
                SettingsThankYouPage.ProductsListId = _model.TYProductsList.TryParseInt();
            }
            else
            {
                SettingsThankYouPage.ProductsListType = _model.TYProductsList.TryParseEnum<EProductOnMain>();
                SettingsThankYouPage.ProductsListId = null;
            }
            SettingsThankYouPage.ShowSelectedProducts = _model.TYShowSelectedProducts;

            SettingsThankYouPage.ExcludedPaymentIds = JsonConvert.DeserializeObject<List<int>>(_model.TYExcludedPaymentIdsSerialized);
            #endregion

            #region CrossSell

            SettingsLandingPage.CrossSellShowMode = (ECrossSellShowMode)_model.CrossSellShowMode;

            #endregion

            SettingsCheckout.ShowCartItemsInBilling = _model.ShowCartItemsInBilling;
            SettingsCheckout.ShoppingCartMode = _model.ShoppingCartMode.TryParseEnum<EShoppingCartMode>();
            SettingsCheckout.AllowAddOrderReview = _model.AllowAddOrderReview;
            SettingsCheckout.ReviewOnlyPaidOrder = _model.ReviewOnlyPaidOrder;
            SettingsCheckout.AllowEditAmount = _model.AllowEditAmount;
            SettingsCheckout.TypeStoreWorkMode = _model.TypeStoreWorkMode;
            SettingsCheckout.AllowUploadFiles = _model.AllowUploadFiles;
            SettingsCheckout.CheckoutImageWidth = _model.CheckoutImageWidth;
            SettingsCheckout.CheckoutImageHeight = _model.CheckoutImageHeight;
            SettingsCheckout.ShowReceivingMethod = _model.ShowReceivingMethod;
            SettingsCheckout.ShowBriefDescriptionProductInCheckout = _model.ShowBriefDescriptionProductInCheckout;
            SettingsCheckout.ShowCountDevices = _model.ShowCountDevices;
            SettingsCheckout.ClearShoppingCartBeforeBuyByLink = _model.ClearShoppingCartBeforeBuyByLink;
            SettingsCheckout.HideShippingNotAvailableForWarehouse = _model.HideShippingNotAvailableForWarehouse;
            
            Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Settings_EditCheckoutSettings);
        }
    }
}
