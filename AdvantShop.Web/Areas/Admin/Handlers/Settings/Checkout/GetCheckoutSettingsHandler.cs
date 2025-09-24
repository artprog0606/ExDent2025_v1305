using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Core.Services.Crm.SalesFunnels;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Customers;
using AdvantShop.Orders;
using AdvantShop.Payment;
using AdvantShop.Repository.Currencies;
using AdvantShop.Saas;
using AdvantShop.Shipping;
using AdvantShop.Web.Admin.Handlers.Analytics;
using AdvantShop.Web.Admin.Handlers.Catalog.Import;
using AdvantShop.Web.Admin.Models.Settings;
using AdvantShop.Web.Admin.Models.Settings.CheckoutSettings;
using AdvantShop.Web.Infrastructure.Handlers;
using Newtonsoft.Json;

namespace AdvantShop.Web.Admin.Handlers.Settings.Checkout
{
    public class GetCheckoutSettingsHandler : AbstractCommandHandler<CheckoutSettingsModel>
    {
        protected override CheckoutSettingsModel Handle()
        {
            List<SelectListItem> outOfStockActions = new List<SelectListItem>();

            //if (SettingsCheckout.OutOfStockAction == eOutOfStockAction.Preorder)
            //{
            //    outOfStockActions.Add(new SelectListItem() { Text = T("Admin.Settings.Checkout.CreatePreorder"), Value = eOutOfStockAction.Preorder.ToString() });
            //}

            outOfStockActions.Add(new SelectListItem() { Text = T("Admin.Settings.Checkout.CreateOrder"), Value = eOutOfStockAction.Order.ToString() });
            outOfStockActions.Add(new SelectListItem() { Text = T("Admin.Settings.Checkout.CreateLead"), Value = eOutOfStockAction.Lead.ToString(), Disabled = SaasDataService.IsSaasEnabled && !SaasDataService.CurrentSaasData.HaveCrm });
            outOfStockActions.Add(new SelectListItem() { Text = T("Admin.Settings.Checkout.AddToCart"), Value = eOutOfStockAction.Cart.ToString() });
            outOfStockActions.Add(new SelectListItem() { Text = T("Admin.Settings.Checkout.AddToCartPreOrder"), Value = eOutOfStockAction.Preorder.ToString() });

            var typesSortOrderShippings = Enum.GetValues(typeof(TypeSortOrderShippings))
                .Cast<TypeSortOrderShippings>()
                .Select(x => new SelectListItem() { Text = x.Localize(), Value = x.ToString() })
                .ToList();

            var shippingMethods = new List<SelectListItem> { new SelectListItem() { Text = "----", Value = "0" } };
            foreach (var shipping in ShippingMethodService.GetAllShippingMethods(true))
            {
                shippingMethods.Add(new SelectListItem() { Text = shipping.Name, Value = shipping.ShippingMethodId.ToString() });
            }


            var paymentMethods = new List<SelectListItem> { new SelectListItem() { Text = "----", Value = "0" } };
            foreach (var payment in PaymentService.GetAllPaymentMethods(true))
            {
                paymentMethods.Add(new SelectListItem() { Text = payment.Name, Value = payment.PaymentMethodId.ToString() });
            }


            var mapTypes = new List<SelectListItem>
            {
                //new SelectListItem() {Text = T("Admin.Settings.Checkout.GoogleMaps"), Value = "googlemap"},
                new SelectListItem() {Text = T("Admin.Settings.Checkout.YandexMaps"), Value = "yandexmap"}
            };

            var oneClickActions = new List<SelectListItem>
            {
                new SelectListItem() {Text = T("Admin.Settings.Checkout.CreateOrder"), Value = "order"},
                new SelectListItem()
                {
                    Text = T("Admin.Settings.Checkout.CreateLead"),
                    Value = "lead",
                    Disabled = SaasDataService.IsSaasEnabled && !SaasDataService.CurrentSaasData.HaveCrm
                }
            };

            var model = new CheckoutSettingsModel()
            {
                BuyInOneClick = SettingsCheckout.BuyInOneClick,
                BuyInOneClickDisableInCheckout = SettingsCheckout.BuyInOneClickDisableInCheckout,
                BuyInOneClickFirstText = SettingsCheckout.BuyInOneClickFirstText,
                BuyInOneClickButtonText = SettingsCheckout.BuyInOneClickButtonText,
                BuyInOneClickAction = SettingsCheckout.BuyInOneClickCreateOrder ? "order" : "lead",
                BuyInOneClickActions = oneClickActions,
                BuyInOneClickLinkText = SettingsCheckout.BuyInOneClickLinkText,

                BuyInOneClickSalesFunnelId = SettingsCrm.DefaultBuyInOneClickSalesFunnelId,
                BuyInOneClickSalesFunnels = SalesFunnelService.GetList().Count != 0 
                    ? SalesFunnelService.GetList().Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList()
                    : new List<SelectListItem>()
                    {
                        new SelectListItem()
                        {
                            Text = "Не выбрано",
                            Value = null
                        }
                    },

                BuyInOneClickDefaultShippingMethod = SettingsCheckout.BuyInOneClickDefaultShippingMethod,
                ShippingMethods = shippingMethods,
                BuyInOneClickDefaultPaymentMethod = SettingsCheckout.BuyInOneClickDefaultPaymentMethod,
                PaymentMethods = paymentMethods,

                AmountLimitation = SettingsCheckout.AmountLimitation,
                OutOfStockAction = SettingsCheckout.OutOfStockAction,
                OutOfStockActions = outOfStockActions,
                OutOfStockSalesFunnelId = SettingsCheckout.OutOfStockSalesFunnelId,
                OutOfStockSalesFunnels = SalesFunnelService.GetList().Count != 0 
                    ? SalesFunnelService.GetList().Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList()
                    : new List<SelectListItem>()
                    {
                        new SelectListItem()
                        {
                            Text = "Не выбрано",
                            Value = null
                        }
                    },

                TypeSortOrderShippings = SettingsCheckout.TypeSortOrderShippings,
                TypesSortOrderShippings = typesSortOrderShippings,

                TaxTypeByPaymentMethodType = SettingsCheckout.TaxTypeByPaymentMethodType,

                DenyToByPreorderedProductsWithZerroAmount = SettingsCheckout.DenyToByPreorderedProductsWithZerroAmount,
                ProceedToPayment = SettingsCheckout.ProceedToPayment,
                MultiplyGiftsCount = SettingsCheckout.MultiplyGiftsCount,

                PrintOrder_ShowStatusInfo = SettingsCheckout.PrintOrder_ShowStatusInfo,
                PrintOrder_ShowMap = SettingsCheckout.PrintOrder_ShowMap,
                PrintOrder_MapType = SettingsCheckout.PrintOrder_MapType,
                PrintOrderMapApiKey = SettingsCheckout.PrintOrderMapApiKey,
                MapTypes = mapTypes,

                EnableGiftCertificateService = SettingsCheckout.EnableGiftCertificateService,
                DisplayPromoTextbox = SettingsCheckout.DisplayPromoTextbox,
                MaximalPriceCertificate = SettingsCheckout.MaximalPriceCertificate,
                MinimalPriceCertificate = SettingsCheckout.MinimalPriceCertificate,

                MinimalOrderPriceForDefaultGroup = CustomerGroupService.GetMinimumOrderPriceByGroup(CustomerGroupService.DefaultCustomerGroup), //SettingsCheckout.MinimalOrderPriceForDefaultGroup,
                ManagerConfirmed = SettingsCheckout.ManagerConfirmed,
                
                SplitShippingByType = SettingsCheckout.SplitShippingByType,
                DefaultShippingType = SettingsCheckout.DefaultShippingType,

                OrderNumberFormat = SettingsCheckout.OrderNumberFormat,
                NextOrderNumber = OrderService.GetLastDbOrderId() + 1,
                SuccessOrderScript = SettingsCheckout.SuccessOrderScript,

                #region checkout fields

                CustomerFirstNameField = SettingsCheckout.CustomerFirstNameField,
                IsShowEmail = SettingsCheckout.IsShowEmail,
                IsRequiredEmail = SettingsCheckout.IsRequiredEmail,
                IsShowLastName = SettingsCheckout.IsShowLastName,
                IsRequiredLastName = SettingsCheckout.IsRequiredLastName,
                IsShowPatronymic = SettingsCheckout.IsShowPatronymic,
                IsRequiredPatronymic = SettingsCheckout.IsRequiredPatronymic,
                CustomerPhoneField = SettingsCheckout.CustomerPhoneField,
                IsShowPhone = SettingsCheckout.IsShowPhone,
                IsRequiredPhone = SettingsCheckout.IsRequiredPhone,

                BirthDayFieldName = SettingsCheckout.BirthDayFieldName,
                IsShowBirthDay = SettingsCheckout.IsShowBirthDay,
                IsRequiredBirthDay = SettingsCheckout.IsRequiredBirthDay,
                IsDisableEditingBirthDay = SettingsCheckout.IsDisableEditingBirthDay,

                // checkout
                IsShowCountry = SettingsCheckout.IsShowCountry,
                IsRequiredCountry = SettingsCheckout.IsRequiredCountry,
                IsShowState = SettingsCheckout.IsShowState,
                IsRequiredState = SettingsCheckout.IsRequiredState,
                IsShowCity = SettingsCheckout.IsShowCity,
                IsRequiredCity = SettingsCheckout.IsRequiredCity,
                IsShowDistrict = SettingsCheckout.IsShowDistrict,
                IsRequiredDistrict = SettingsCheckout.IsRequiredDistrict,
                IsShowZip = SettingsCheckout.IsShowZip,
                IsRequiredZip = SettingsCheckout.IsRequiredZip,
                IsShowAddress = SettingsCheckout.IsShowAddress,
                IsRequiredAddress = SettingsCheckout.IsRequiredAddress,
                IsShowUserComment = SettingsCheckout.IsShowUserComment,
                CustomShippingField1 = SettingsCheckout.CustomShippingField1,
                IsShowCustomShippingField1 = SettingsCheckout.IsShowCustomShippingField1,
                IsReqCustomShippingField1 = SettingsCheckout.IsReqCustomShippingField1,
                CustomShippingField2 = SettingsCheckout.CustomShippingField2,
                IsShowCustomShippingField2 = SettingsCheckout.IsShowCustomShippingField2,
                IsReqCustomShippingField2 = SettingsCheckout.IsReqCustomShippingField2,
                CustomShippingField3 = SettingsCheckout.CustomShippingField3,
                IsShowCustomShippingField3 = SettingsCheckout.IsShowCustomShippingField3,
                IsReqCustomShippingField3 = SettingsCheckout.IsReqCustomShippingField3,
                IsShowFullAddress = SettingsCheckout.IsShowFullAddress,
                UseCrossSellLandingsInCheckout = SettingsLandingPage.UseCrossSellLandingsInCheckout,
                IsShowOrderRecipient = SettingsCheckout.IsShowOrderRecipient,
                IsShowDontCallBack = SettingsCheckout.IsShowDontCallBack,

                // buy one click
                BuyInOneClickName = SettingsCheckout.BuyInOneClickName,
                IsShowBuyInOneClickName = SettingsCheckout.IsShowBuyInOneClickName,
                IsRequiredBuyInOneClickName = SettingsCheckout.IsRequiredBuyInOneClickName,
                BuyInOneClickEmail = SettingsCheckout.BuyInOneClickEmail,
                IsShowBuyInOneClickEmail = SettingsCheckout.IsShowBuyInOneClickEmail,
                IsRequiredBuyInOneClickEmail = SettingsCheckout.IsRequiredBuyInOneClickEmail,
                BuyInOneClickPhone = SettingsCheckout.BuyInOneClickPhone,
                IsShowBuyInOneClickPhone = SettingsCheckout.IsShowBuyInOneClickPhone,
                IsRequiredBuyInOneClickPhone = SettingsCheckout.IsRequiredBuyInOneClickPhone,
                BuyInOneClickComment = SettingsCheckout.BuyInOneClickComment,
                IsShowBuyInOneClickComment = SettingsCheckout.IsShowBuyInOneClickComment,
                IsRequiredBuyInOneClickComment = SettingsCheckout.IsRequiredBuyInOneClickComment,

                ZipDisplayPlace = SettingsCheckout.ZipDisplayPlace,

                #endregion

                ShowClientId = SettingsDesign.ShowClientId,
                EnableShoppingCartPopup = SettingsDesign.EnableShoppingCartPopup,
                EnableLogingInTariff = SaasDataService.IsSaasEnabled && !SaasDataService.CurrentSaasData.HaveCustomerLog,

                ShowCartItemsInBilling = SettingsCheckout.ShowCartItemsInBilling,
                AllowAddOrderReview = SettingsCheckout.AllowAddOrderReview,
                ReviewOnlyPaidOrder = SettingsCheckout.ReviewOnlyPaidOrder,
                AllowEditAmount = SettingsCheckout.AllowEditAmount,

                TypeStoreWorkMode = SettingsCheckout.TypeStoreWorkMode,
                TypeStoreWorkModeList = Enum.GetValues(typeof(EStoreWorkMode))
                                            .Cast<EStoreWorkMode>()
                                            .Select(x => new SelectListItem() { Text = x.Localize(), Value = x.ToString() })
                                            .ToList(),
                AllowUploadFiles = SettingsCheckout.AllowUploadFiles,
                ShowBriefDescriptionProductInCheckout = SettingsCheckout.ShowBriefDescriptionProductInCheckout,
                CheckoutImageHeight = SettingsCheckout.CheckoutImageHeight,
                CheckoutImageWidth = SettingsCheckout.CheckoutImageWidth,
                ShowReceivingMethod = SettingsCheckout.ShowReceivingMethod,
                ShowCountDevices = SettingsCheckout.ShowCountDevices,
                ClearShoppingCartBeforeBuyByLink = SettingsCheckout.ClearShoppingCartBeforeBuyByLink,
                HideShippingNotAvailableForWarehouse = SettingsCheckout.HideShippingNotAvailableForWarehouse,
            };

            #region ThankYouPage

            model.TYPageAction = SettingsThankYouPage.ActionType;
            model.TYPageActions = Enum.GetValues(typeof(EThankYouPageActionType)).Cast<EThankYouPageActionType>().Select(
                x => new SelectListItem { Text = x.Localize(), Value = x.ToString(), Selected = x == SettingsThankYouPage.ActionType }).ToList();

            model.TYSocialNetworks = SettingsThankYouPage.SocialNetworks;
            model.TYSocialNetworksSerialized = JsonConvert.SerializeObject(model.TYSocialNetworks);

            model.TYNameOfBlockProducts = SettingsThankYouPage.NameOfBlockProducts;
            model.TYShowReletedProducts = SettingsThankYouPage.ShowReletedProducts;
            model.TYReletedProductsType = SettingsThankYouPage.ReletedProductsType;
            model.TYReletedProductsTypes = new List<SelectListItem>
            {
                new SelectListItem { Text = SettingsCatalog.RelatedProductName, Value = RelatedType.Related.ToString() },
                new SelectListItem { Text = SettingsCatalog.AlternativeProductName, Value = RelatedType.Alternative.ToString() }
            };
            model.TYShowProductsList = SettingsThankYouPage.ShowProductsList;
            model.TYProductsLists = new List<SelectListItem>
            {
                new SelectListItem { Text = LocalizationService.GetResource("Не выбран"), Value = EProductOnMain.None.ToString() },
                new SelectListItem { Text = LocalizationService.GetResource("Catalog.ProductList.AllBestSellers"), Value = EProductOnMain.Best.ToString() },
                new SelectListItem { Text = LocalizationService.GetResource("Catalog.ProductList.AllNewProducts"), Value = EProductOnMain.New.ToString() },
                new SelectListItem { Text = LocalizationService.GetResource("Catalog.ProductList.AllSales"), Value = EProductOnMain.Sale.ToString() },
            };
            var productLists = ProductListService.GetList();
            model.TYProductsLists.AddRange(productLists.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }));
            if (SettingsThankYouPage.ProductsListId.HasValue)
                model.TYProductsList = model.TYProductsLists.Any(x => x.Value == SettingsThankYouPage.ProductsListId.ToString())
                    ? SettingsThankYouPage.ProductsListId.Value.ToString()
                    : EProductOnMain.None.ToString();
            else
                model.TYProductsList = SettingsThankYouPage.ProductsListType.ToString();
            model.TYShowSelectedProducts = SettingsThankYouPage.ShowSelectedProducts;

            model.TYExcludedPaymentIdsSerialized = JsonConvert.SerializeObject(SettingsThankYouPage.ExcludedPaymentIds);
            model.TYPaymentMethods = PaymentService.GetAllPaymentMethods(false).Select(x =>
            new SelectListItem
            {
                Text = x.Name,
                Value = x.PaymentMethodId.ToString(),
                Disabled = !x.Enabled
            }).ToList();

            #endregion

            model.ExportOrdersModel = new GetExportOrdersModel().Execute();
            model.ImportOrdersModel = new GetImportOrdersModel().Execute();

            model.ShoppingCartMode = SettingsCheckout.ShoppingCartMode.ToString();
            model.ShoppingCartModeList = Enum.GetValues(typeof(EShoppingCartMode)).Cast<EShoppingCartMode>().Select(
                x => new SelectListItem { Text = x.Localize(), Value = x.ToString(), Selected = x.ToString() == model.ShoppingCartMode }).ToList();

            model.ShippingTypes = Enum.GetValues(typeof(EnTypeOfDelivery)).Cast<EnTypeOfDelivery>().Select(
                x => new SelectListItem { Text = x.Localize(), Value = x.ToString(), Selected = x == model.DefaultShippingType }).ToList();

            #region CrossSell

            model.CrossSellShowMode = (int)SettingsLandingPage.CrossSellShowMode;
            model.CrossSellShowModes = Enum.GetValues(typeof(ECrossSellShowMode)).Cast<ECrossSellShowMode>().Select(
                x => new SelectListItem
                {
                    Text = x.Localize(),
                    Value = ((int) x).ToString(),
                    Selected = (int) x == model.CrossSellShowMode
                }).ToList();

            #endregion
            
            return model;
        }
    }
}
