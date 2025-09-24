//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Linq;
using AdvantShop.Customers;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Saas;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Services.Crm.SalesFunnels;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Shipping;

namespace AdvantShop.Configuration
{
    public enum eOutOfStockAction
    {
        Preorder,
        Order,
        Lead,
        Cart
    }

    public enum TypeSortOrderShippings
    {
        [Localize("Core.Checkout.TypeSortOrderShippings.Default")]
        Default = 0,

        [Localize("Core.Checkout.TypeSortOrderShippings.AscByRate")]
        AscByRate = 1,

        [Localize("Core.Checkout.TypeSortOrderShippings.DescByRate")]
        DescByRate = 2
    }

    public enum EShoppingCartMode
    {
        [Localize("Core.Checkout.EShoppingCartMode.Compact")]
        Compact = 0,

        [Localize("Core.Checkout.EShoppingCartMode.Full")]
        Full = 1
    }

    public enum EStoreWorkMode
    {
        [Localize("Core.Checkout.EStoreWorkMode.AroundTheClock")]
        AroundTheClock = 0,

        [Localize("Core.Checkout.EStoreWorkMode.ByTime")]
        ByTime = 1
    }

    public enum EnTypeOfReceivingMethod
    {
        [Localize("Core.Checkout.EnTypeOfReceivingMethod.InHall")]
        InHall = 0,
        [Localize("Core.Checkout.EnTypeOfReceivingMethod.WithYou")]
        WithYou = 1,
    }

    public class SettingsCheckout
    {
        #region Settings

        public static bool TaxTypeByPaymentMethodType
        {
            get => Convert.ToBoolean(SettingProvider.Items["TaxTypeByPaymentMethodType"]);
            set => SettingProvider.Items["TaxTypeByPaymentMethodType"] = value.ToString();
        }

        public static bool AmountLimitation
        {
            get => Convert.ToBoolean(SettingProvider.Items["AmountLimitation"]);
            set => SettingProvider.Items["AmountLimitation"] = value.ToString();
        }

        public static eOutOfStockAction OutOfStockAction
        {
            get
            {
                var action = SettingProvider.Items["OutOfStockAction"].TryParseEnum<eOutOfStockAction>();
                return (action == eOutOfStockAction.Lead && SaasDataService.IsSaasEnabled && !SaasDataService.CurrentSaasData.HaveCrm)
                    ? eOutOfStockAction.Order : action;
            }
            set => SettingProvider.Items["OutOfStockAction"] = value.ToString();
        }

        public static int OutOfStockSalesFunnelId
        {
            get
            {
                var id = Convert.ToInt32(SettingProvider.Items["OutOfStockSalesFunnelId"]);

                var funnels = SalesFunnelService.GetList();
                if (funnels.Find(x => x.Id == id) == null)
                {
                    var funnel = funnels.FirstOrDefault();
                    if (funnel != null)
                        OutOfStockSalesFunnelId = id = funnel.Id;
                }
                return id;
            }
            set => SettingProvider.Items["OutOfStockSalesFunnelId"] = value.ToString();
        }



        public static bool DecrementProductsCount
        {
            get => Convert.ToBoolean(SettingProvider.Items["DecrementProductsCount"]);
            set => SettingProvider.Items["DecrementProductsCount"] = value.ToString();
        }

        public static bool ProceedToPayment
        {
            get => Convert.ToBoolean(SettingProvider.Items["ProceedToPayment"]);
            set => SettingProvider.Items["ProceedToPayment"] = value.ToString();
        }

        public static float MinimalOrderPriceForDefaultGroup
        {
            get => CustomerGroupService.GetMinimumOrderPriceByGroup(CustomerGroupService.DefaultCustomerGroup);
            set
            {
                var group = CustomerGroupService.GetCustomerGroup(CustomerGroupService.DefaultCustomerGroup);
                group.MinimumOrderPrice = value;
                CustomerGroupService.UpdateCustomerGroup(group);
            }
        }

        public static float MinimalPriceCertificate
        {
            get
            {
                float minimalPriceCertificate = 0;
                float.TryParse(SettingProvider.Items["MinimalPriceCertificate"], out minimalPriceCertificate);
                return minimalPriceCertificate;
            }
            set => SettingProvider.Items["MinimalPriceCertificate"] = value.ToString("#0.00");
        }

        public static float MaximalPriceCertificate
        {
            get
            {
                float maximalPriceCertificate = 0;
                float.TryParse(SettingProvider.Items["MaximalPriceCertificate"], out maximalPriceCertificate);
                return maximalPriceCertificate;
            }
            set => SettingProvider.Items["MaximalPriceCertificate"] = value.ToString("#0.00");
        }

        public static bool EnableGiftCertificateService
        {
            get => Convert.ToBoolean(SettingProvider.Items["EnableGiftCertificateService"]);
            set => SettingProvider.Items["EnableGiftCertificateService"] = value.ToString();
        }

        public static bool DisplayPromoTextbox
        {
            get => Convert.ToBoolean(SettingProvider.Items["DisplayPromoTextbox"]);
            set => SettingProvider.Items["DisplayPromoTextbox"] = value.ToString();
        }


        public static bool EnableDiscountModule
        {
            get => Convert.ToBoolean(SettingProvider.Items["EnableDiscountModule"]);
            set => SettingProvider.Items["EnableDiscountModule"] = value.ToString();
        }

        public static bool PrintOrder_ShowStatusInfo
        {
            get => Convert.ToBoolean(SettingProvider.Items["PrintOrder_ShowStatusInfo"]);
            set => SettingProvider.Items["PrintOrder_ShowStatusInfo"] = value.ToString();
        }

        public static bool PrintOrder_ShowMap
        {
            get => Convert.ToBoolean(SettingProvider.Items["PrintOrder_ShowMap"]);
            set => SettingProvider.Items["PrintOrder_ShowMap"] = value.ToString();
        }

        public static string PrintOrder_MapType
        {
            get => SettingProvider.Items["PrintOrder_MapType"];
            set => SettingProvider.Items["PrintOrder_MapType"] = value;
        }

        public static string PrintOrderMapApiKey
        {
            get => SettingProvider.Items["PrintOrderMapApiKey"];
            set => SettingProvider.Items["PrintOrderMapApiKey"] = value;
        }

        public static string SuccessOrderScript
        {
            get => SettingProvider.Items["SuccessOrderScript"];
            set => SettingProvider.Items["SuccessOrderScript"] = value;
        }

        public static string OrderNumberFormat
        {
            get => SettingProvider.Items["OrderNumberFormat"];
            set => SettingProvider.Items["OrderNumberFormat"] = value;
        }

        public static bool ManagerConfirmed
        {
            get => Convert.ToBoolean(SettingProvider.Items["ManagerConfirmed"]);
            set => SettingProvider.Items["ManagerConfirmed"] = value.ToString();
        }

        public static bool MultiplyGiftsCount
        {
            get => Convert.ToBoolean(SettingProvider.Items["MultiplyGiftsCount"]);
            set => SettingProvider.Items["MultiplyGiftsCount"] = value.ToString();
        }

        public static bool DenyToByPreorderedProductsWithZerroAmount
        {
            get => Convert.ToBoolean(SettingProvider.Items["DenyToByPreorderedProductsWithZerroAmount"]);
            set => SettingProvider.Items["DenyToByPreorderedProductsWithZerroAmount"] = value.ToString();
        }

        public static TypeSortOrderShippings TypeSortOrderShippings
        {
            get => (TypeSortOrderShippings)SettingProvider.Items["TypeSortOrderShippings"].TryParseInt((int)TypeSortOrderShippings.Default);
            set => SettingProvider.Items["TypeSortOrderShippings"] = ((int)value).ToString();
        }

        public static bool ShowCartItemsInBilling
        {
            get => Convert.ToBoolean(SettingProvider.Items["ShowCartItemsInBilling"]);
            set => SettingProvider.Items["ShowCartItemsInBilling"] = value.ToString();
        }

        public static bool SplitShippingByType
        {
            get => Convert.ToBoolean(SettingProvider.Items["SplitShippingByType"]);
            set => SettingProvider.Items["SplitShippingByType"] = value.ToString();
        }

        public static EnTypeOfDelivery DefaultShippingType
        {
            get => SettingProvider.Items["DefaultShippingType"].TryParseEnum(EnTypeOfDelivery.SelfDelivery);
            set => SettingProvider.Items["DefaultShippingType"] = ((int)value).ToString();
        }

        public static bool ReviewOnlyPaidOrder
        {
            get => SettingProvider.Items["ReviewOnlyPaidOrder"].TryParseBool();
            set => SettingProvider.Items["ReviewOnlyPaidOrder"] = value.ToString();
        }

        public static bool AllowAddOrderReview
        {
            get => SettingProvider.Items["AllowAddOrderReview"].TryParseBool(true) ?? true;
            set => SettingProvider.Items["AllowAddOrderReview"] = value.ToString();
        }

        public static bool AllowEditAmount
        {
            get => SettingProvider.Items["AllowEditAmount"].TryParseBool(true) ?? true;
            set => SettingProvider.Items["AllowEditAmount"] = value.ToString();
        }

        public static EStoreWorkMode TypeStoreWorkMode
        {
            get => SettingProvider.Items["TypeStoreWorkMode"].TryParseEnum(EStoreWorkMode.AroundTheClock);
            set => SettingProvider.Items["TypeStoreWorkMode"] = ((int)value).ToString();
        }

        public static TimeSpan TimeZoneWorkingTime
        {
            get => SettingProvider.Items["TimeZoneWorkingTime"].TryParseTimeSpan(TimeZoneInfo.Local.BaseUtcOffset);
            set => SettingProvider.Items["TimeZoneWorkingTime"] = (value).ToString();
        }

        public static string NotAllowCheckoutText
        {
            get => SettingProvider.Items["NotAllowCheckoutText"] ?? LocalizationService.GetResource("Core.Checkout.NotAllowCheckoutText.DefaultText");
            set => SettingProvider.Items["NotAllowCheckoutText"] = value;
        }

        public static bool AllowUploadFiles
        {
            get => SettingProvider.Items["AllowUploadFilesAtCheckout"].TryParseBool(true) ?? false;
            set => SettingProvider.Items["AllowUploadFilesAtCheckout"] = value.ToString();
        }

        public static int CheckoutImageHeight
        {
            get => SettingProvider.Items["CheckoutImageHeight"].TryParseInt(1000);
            set => SettingProvider.Items["CheckoutImageHeight"] = value.ToString();
        }

        public static int CheckoutImageWidth
        {
            get => SettingProvider.Items["CheckoutImageWidth"].TryParseInt(1000);
            set => SettingProvider.Items["CheckoutImageWidth"] = value.ToString();
        }

        public static bool ShowReceivingMethod
        {
            get => SettingProvider.Items["ShowReceivingMethodAtCheckout"].TryParseBool(true) ?? false;
            set => SettingProvider.Items["ShowReceivingMethodAtCheckout"] = value.ToString();
        }

        public static bool ShowBriefDescriptionProductInCheckout
        {
            get => SettingProvider.Items["ShowBriefDescriptionProductInCheckout"].TryParseBool(true) ?? false;
            set => SettingProvider.Items["ShowBriefDescriptionProductInCheckout"] = value.ToString();
        }

        public static bool ShowCountDevices
        {
            get => SettingProvider.Items["ShowCountDevicesAtCheckout"].TryParseBool(true) ?? false;
            set => SettingProvider.Items["ShowCountDevicesAtCheckout"] = value.ToString();
        }

        public static bool ClearShoppingCartBeforeBuyByLink
        {
            get => SettingProvider.Items["ClearShoppingCartBeforeBuyByLink"].TryParseBool(true) ?? true;
            set => SettingProvider.Items["ClearShoppingCartBeforeBuyByLink"] = value.ToString();
        }

        public static bool HideShippingNotAvailableForWarehouse
        {
            get => SettingProvider.Items["HideShippingNotAvailableForWarehouse"].TryParseBool();
            set => SettingProvider.Items["HideShippingNotAvailableForWarehouse"] = value.ToString();
        }

        #endregion

        #region Buy one click

        public static bool BuyInOneClick
        {
            get => Convert.ToBoolean(SettingProvider.Items["BuyInOneClick"]);
            set => SettingProvider.Items["BuyInOneClick"] = value.ToString();
        }

        public static bool BuyInOneClickDisableInCheckout
        {
            get => Convert.ToBoolean(SettingProvider.Items["BuyInOneClick_DisableInCheckout"]);
            set => SettingProvider.Items["BuyInOneClick_DisableInCheckout"] = value.ToString();
        }
                
        public static string BuyInOneClickLinkText
        {
            get => SettingProvider.Items["BuyInOneClick_LinkText"];
            set => SettingProvider.Items["BuyInOneClick_LinkText"] = value;
        }

        public static string BuyInOneClickFirstText
        {
            get => SettingProvider.Items["BuyInOneClick_FirstText"];
            set => SettingProvider.Items["BuyInOneClick_FirstText"] = value;
        }

        public static string BuyInOneClickButtonText
        {
            get => SettingProvider.Items["BuyInOneClick_ButtonText"];
            set => SettingProvider.Items["BuyInOneClick_ButtonText"] = value;
        }

        public static bool BuyInOneClickCreateOrder
        {
            get => Convert.ToBoolean(SettingProvider.Items["BuyInOneClick_CreateOrder"]);
            set => SettingProvider.Items["BuyInOneClick_CreateOrder"] = value.ToString();
        }



        public static string BuyInOneClickName
        {
            get => SettingProvider.Items["BuyInOneClickName"];
            set => SettingProvider.Items["BuyInOneClickName"] = value;
        }

        public static bool IsShowBuyInOneClickName
        {
            get => Convert.ToBoolean(SettingProvider.Items["IsShowBuyInOneClickName"]);
            set => SettingProvider.Items["IsShowBuyInOneClickName"] = value.ToString();
        }

        public static bool IsRequiredBuyInOneClickName
        {
            get => Convert.ToBoolean(SettingProvider.Items["IsRequiredBuyInOneClickName"]);
            set => SettingProvider.Items["IsRequiredBuyInOneClickName"] = value.ToString();
        }

        public static string BuyInOneClickEmail
        {
            get => SettingProvider.Items["BuyInOneClickEmail"];
            set => SettingProvider.Items["BuyInOneClickEmail"] = value;
        }

        public static bool IsShowBuyInOneClickEmail
        {
            get => Convert.ToBoolean(SettingProvider.Items["IsShowBuyInOneClickEmail"]);
            set => SettingProvider.Items["IsShowBuyInOneClickEmail"] = value.ToString();
        }

        public static bool IsRequiredBuyInOneClickEmail
        {
            get => Convert.ToBoolean(SettingProvider.Items["IsRequiredBuyInOneClickEmail"]);
            set => SettingProvider.Items["IsRequiredBuyInOneClickEmail"] = value.ToString();
        }

        public static string BuyInOneClickPhone
        {
            get => SettingProvider.Items["BuyInOneClickPhone"];
            set => SettingProvider.Items["BuyInOneClickPhone"] = value;
        }

        public static bool IsShowBuyInOneClickPhone
        {
            get => Convert.ToBoolean(SettingProvider.Items["IsShowBuyInOneClickPhone"]);
            set => SettingProvider.Items["IsShowBuyInOneClickPhone"] = value.ToString();
        }

        public static bool IsRequiredBuyInOneClickPhone
        {
            get => Convert.ToBoolean(SettingProvider.Items["IsRequiredBuyInOneClickPhone"]);
            set => SettingProvider.Items["IsRequiredBuyInOneClickPhone"] = value.ToString();
        }

        public static string BuyInOneClickComment
        {
            get => SettingProvider.Items["BuyInOneClickComment"];
            set => SettingProvider.Items["BuyInOneClickComment"] = value;
        }

        public static bool IsShowBuyInOneClickComment
        {
            get => Convert.ToBoolean(SettingProvider.Items["IsShowBuyInOneClickComment"]);
            set => SettingProvider.Items["IsShowBuyInOneClickComment"] = value.ToString();
        }

        public static bool IsRequiredBuyInOneClickComment
        {
            get => Convert.ToBoolean(SettingProvider.Items["IsRequiredBuyInOneClickComment"]);
            set => SettingProvider.Items["IsRequiredBuyInOneClickComment"] = value.ToString();
        }

        public static int BuyInOneClickDefaultShippingMethod
        {
            get => SettingProvider.Items["BuyInOneClickDefaultShippingMethod"].TryParseInt();
            set => SettingProvider.Items["BuyInOneClickDefaultShippingMethod"] = value.ToString();
        }

        public static int BuyInOneClickDefaultPaymentMethod
        {
            get => SettingProvider.Items["BuyInOneClickDefaultPaymentMethod"].TryParseInt();
            set => SettingProvider.Items["BuyInOneClickDefaultPaymentMethod"] = value.ToString();
        }

        #endregion

        #region Checkout Fields

        public static string CustomerFirstNameField
        {
            get => SettingProvider.Items["CustomerFirstNameField"];
            set => SettingProvider.Items["CustomerFirstNameField"] = value;
        }

        public static bool IsShowEmail
        {
            get
            {
                var isShow = SettingProvider.Items["IsShowEmail"];
                return isShow == null || Convert.ToBoolean(isShow);
            }
            set => SettingProvider.Items["IsShowEmail"] = value.ToString();
        }

        public static bool IsRequiredEmail
        {
            get
            {
                var isRequired = SettingProvider.Items["IsRequiredEmail"];
                return isRequired == null || Convert.ToBoolean(isRequired);
            }
            set => SettingProvider.Items["IsRequiredEmail"] = value.ToString();
        }

        public static bool IsShowLastName
        {
            get => Convert.ToBoolean(SettingProvider.Items["IsShowLastName"]);
            set => SettingProvider.Items["IsShowLastName"] = value.ToString();
        }

        public static bool IsRequiredLastName
        {
            get => Convert.ToBoolean(SettingProvider.Items["IsRequiredLastName"]);
            set => SettingProvider.Items["IsRequiredLastName"] = value.ToString();
        }

        public static bool IsShowPatronymic
        {
            get => Convert.ToBoolean(SettingProvider.Items["IsShowPatronymic"]);
            set => SettingProvider.Items["IsShowPatronymic"] = value.ToString();
        }

        public static bool IsRequiredPatronymic
        {
            get => Convert.ToBoolean(SettingProvider.Items["IsRequiredPatronymic"]);
            set => SettingProvider.Items["IsRequiredPatronymic"] = value.ToString();
        }

        public static string CustomerPhoneField
        {
            get => SettingProvider.Items["CustomerPhoneField"];
            set => SettingProvider.Items["CustomerPhoneField"] = value;
        }

        public static bool IsShowPhone
        {
            get => Convert.ToBoolean(SettingProvider.Items["IsShowPhone"]);
            set => SettingProvider.Items["IsShowPhone"] = value.ToString();
        }

        public static bool IsRequiredPhone
        {
            get => Convert.ToBoolean(SettingProvider.Items["IsRequiredPhone"]);
            set => SettingProvider.Items["IsRequiredPhone"] = value.ToString();
        }

        public static bool IsShowCountry
        {
            get => Convert.ToBoolean(SettingProvider.Items["IsShowCountry"]);
            set => SettingProvider.Items["IsShowCountry"] = value.ToString();
        }

        public static bool IsRequiredCountry
        {
            get => Convert.ToBoolean(SettingProvider.Items["IsRequiredCountry"]);
            set => SettingProvider.Items["IsRequiredCountry"] = value.ToString();
        }

        public static bool IsShowState
        {
            get => Convert.ToBoolean(SettingProvider.Items["IsShowState"]);
            set => SettingProvider.Items["IsShowState"] = value.ToString();
        }

        public static bool IsRequiredState
        {
            get => Convert.ToBoolean(SettingProvider.Items["IsRequiredState"]);
            set => SettingProvider.Items["IsRequiredState"] = value.ToString();
        }

        public static bool IsShowCity
        {
            get => Convert.ToBoolean(SettingProvider.Items["IsShowCity"]);
            set => SettingProvider.Items["IsShowCity"] = value.ToString();
        }

        public static bool IsRequiredCity
        {
            get => Convert.ToBoolean(SettingProvider.Items["IsRequiredCity"]);
            set => SettingProvider.Items["IsRequiredCity"] = value.ToString();
        }

        public static bool IsShowDistrict
        {
            get => Convert.ToBoolean(SettingProvider.Items["IsShowDistrict"]);
            set => SettingProvider.Items["IsShowDistrict"] = value.ToString();
        }

        public static bool IsRequiredDistrict
        {
            get => Convert.ToBoolean(SettingProvider.Items["IsRequiredDistrict"]);
            set => SettingProvider.Items["IsRequiredDistrict"] = value.ToString();
        }

        public static bool IsShowZip
        {
            get => Convert.ToBoolean(SettingProvider.Items["IsShowZip"]);
            set => SettingProvider.Items["IsShowZip"] = value.ToString();
        }

        public static bool IsRequiredZip
        {
            get => Convert.ToBoolean(SettingProvider.Items["IsRequiredZip"]);
            set => SettingProvider.Items["IsRequiredZip"] = value.ToString();
        }

        public static bool IsShowAddress
        {
            get => Convert.ToBoolean(SettingProvider.Items["IsShowAddress"]);
            set => SettingProvider.Items["IsShowAddress"] = value.ToString();
        }

        public static bool IsRequiredAddress
        {
            get => Convert.ToBoolean(SettingProvider.Items["IsRequiredAddress"]);
            set => SettingProvider.Items["IsRequiredAddress"] = value.ToString();
        }

        public static bool IsShowUserAgreementText => IsShowUserAgreementTextValue && 
                                                      (CustomerContext.CurrentCustomer == null || !CustomerContext.CurrentCustomer.RegistredUser);

        public static bool IsShowUserAgreementTextValue
        {
            get
            {
                var isShow = SettingProvider.Items["IsShowUserAgreementText"];
                return isShow == null || Convert.ToBoolean(isShow);
            }
            set => SettingProvider.Items["IsShowUserAgreementText"] = value.ToString();
        }


        public static bool AgreementDefaultChecked
        {
            get
            {
                var isShow = SettingProvider.Items["AgreementDefaultChecked"];
                return isShow == null ? false : Convert.ToBoolean(isShow);
            }
            set => SettingProvider.Items["AgreementDefaultChecked"] = value.ToString();
        }

        public static string UserAgreementText
        {
            get => SettingProvider.Items["UserAgreementText"];
            set => SettingProvider.Items["UserAgreementText"] = value;
        }

        public static bool IsShowUserComment
        {
            get => Convert.ToBoolean(SettingProvider.Items["IsShowUserComment"]);
            set => SettingProvider.Items["IsShowUserComment"] = value.ToString();
        }

        public static bool IsShowFullAddress
        {
            get => Convert.ToBoolean(SettingProvider.Items["IsShowFullAddress"]);
            set => SettingProvider.Items["IsShowFullAddress"] = value.ToString();
        }

        public static bool ZipDisplayPlace
        {
            get => Convert.ToBoolean(SettingProvider.Items["ZipDisplayPlace"]);
            set => SettingProvider.Items["ZipDisplayPlace"] = value.ToString();
        }

        public static string BirthDayFieldName
        {
            get => SettingProvider.Items["BirthDayFieldName"];
            set => SettingProvider.Items["BirthDayFieldName"] = value;
        }

        public static bool IsShowBirthDay
        {
            get => Convert.ToBoolean(SettingProvider.Items["IsShowBirthDay"]);
            set => SettingProvider.Items["IsShowBirthDay"] = value.ToString();
        }

        public static bool IsRequiredBirthDay
        {
            get => Convert.ToBoolean(SettingProvider.Items["IsRequiredBirthDay"]);
            set => SettingProvider.Items["IsRequiredBirthDay"] = value.ToString();
        }

        public static bool IsDisableEditingBirthDay
        {
            get => Convert.ToBoolean(SettingProvider.Items["IsDisableEditingBirthDay"]);
            set => SettingProvider.Items["IsDisableEditingBirthDay"] = value.ToString();
        }

        public static bool IsShowOrderRecipient
        {
            get => SettingProvider.Items["IsShowOrderRecipient"].TryParseBool();
            set => SettingProvider.Items["IsShowOrderRecipient"] = value.ToString();
        }

        public static bool IsShowDontCallBack
        {
            get => SettingProvider.Items["IsShowDontCallBack"].TryParseBool();
            set => SettingProvider.Items["IsShowDontCallBack"] = value.ToString();
        }

        #region Custom shipping fields

        public static bool IsShowCustomShippingField1
        {
            get => Convert.ToBoolean(SettingProvider.Items["IsShowCustomShippingField1"]);
            set => SettingProvider.Items["IsShowCustomShippingField1"] = value.ToString();
        }

        public static bool IsReqCustomShippingField1
        {
            get => Convert.ToBoolean(SettingProvider.Items["IsReqCustomShippingField1"]);
            set => SettingProvider.Items["IsReqCustomShippingField1"] = value.ToString();
        }

        public static string CustomShippingField1
        {
            get => SettingProvider.Items["CustomShippingField1"];
            set => SettingProvider.Items["CustomShippingField1"] = value;
        }


        public static bool IsShowCustomShippingField2
        {
            get => Convert.ToBoolean(SettingProvider.Items["IsShowCustomShippingField2"]);
            set => SettingProvider.Items["IsShowCustomShippingField2"] = value.ToString();
        }

        public static bool IsReqCustomShippingField2
        {
            get => Convert.ToBoolean(SettingProvider.Items["IsReqCustomShippingField2"]);
            set => SettingProvider.Items["IsReqCustomShippingField2"] = value.ToString();
        }

        public static string CustomShippingField2
        {
            get => SettingProvider.Items["CustomShippingField2"];
            set => SettingProvider.Items["CustomShippingField2"] = value;
        }


        public static bool IsShowCustomShippingField3
        {
            get => Convert.ToBoolean(SettingProvider.Items["IsShowCustomShippingField3"]);
            set => SettingProvider.Items["IsShowCustomShippingField3"] = value.ToString();
        }

        public static bool IsReqCustomShippingField3
        {
            get => Convert.ToBoolean(SettingProvider.Items["IsReqCustomShippingField3"]);
            set => SettingProvider.Items["IsReqCustomShippingField3"] = value.ToString();
        }

        public static string CustomShippingField3
        {
            get => SettingProvider.Items["CustomShippingField3"];
            set => SettingProvider.Items["CustomShippingField3"] = value;
        }

        public static EShoppingCartMode ShoppingCartMode
        {
            get => SettingProvider.Items["ShoppingCartMode"].TryParseEnum<EShoppingCartMode>();
            set => SettingProvider.Items["ShoppingCartMode"] = value.ToString();
        }

        #endregion

        #endregion
    }
}