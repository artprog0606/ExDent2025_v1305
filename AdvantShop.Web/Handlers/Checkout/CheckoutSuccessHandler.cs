using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using AdvantShop.CMS;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Bonuses.Service;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Core.Services.SEO;
using AdvantShop.Customers;
using AdvantShop.Orders;
using AdvantShop.SEO;
using AdvantShop.ViewModel.Checkout;
using AdvantShop.Helpers;
using AdvantShop.Models.Checkout;

namespace AdvantShop.Handlers.Checkout
{
    public class CheckoutSuccessHandler
    {
        public CheckoutSuccess Get(Order order, bool isLanding, CheckoutShowMode ? showMode)
        {
            var model = new CheckoutSuccess { Order = order };

            model.IsEmptyLayout = model.IsLanding = isLanding || showMode == CheckoutShowMode.EmptyLayout;

            var sb = StaticBlockService.GetPagePartByKeyWithCache("OrderSuccessTop");
            var sbM = StaticBlockService.GetPagePartByKeyWithCache("MobileOrderSuccessTop");
            var mob = MobileHelper.IsMobileEnabled();
            if ((!mob && sb == null || sbM == null && mob) || (!CustomerContext.CurrentCustomer.IsAdmin && (!mob && !sb.Enabled || !sbM.Enabled && mob)))
            {
                model.OrderSuccessTopText = string.Empty;
            }
            else
            {
                model.OrderSuccessTopText = mob ? sbM.Content ?? string.Empty : sb.Content ?? string.Empty;
                model.OrderSuccessTopText = model.OrderSuccessTopText.Replace("#ORDER_ID#", order.Number);
            }

            if (SettingsCheckout.SuccessOrderScript.IsNotEmpty())
            {
                var orderScript = SettingsCheckout.SuccessOrderScript
                                                              .Replace("#ORDER_ID#", order.OrderID.ToString())
                                                              .Replace("#ORDER_SUM#", order.Sum.ToString("0.##"))
                                                              .Replace("#SHIPPING_SUM#", order.ShippingCost.ToString("0.##"))
                                                              .Replace("#ORDER_SUM_RAW#", order.Sum.ToInvariantString())
                                                              .Replace("#ORDER_CURRENCY_CODE#", order.OrderCurrency.CurrencyCode)
                                                              .Replace("#CUSTOMER_EMAIL#", HttpUtility.HtmlEncode(order.OrderCustomer.Email))
                                                              .Replace("#CUSTOMER_FIRSTNAME#", HttpUtility.HtmlEncode(order.OrderCustomer.FirstName))
                                                              .Replace("#CUSTOMER_LASTNAME#", HttpUtility.HtmlEncode(order.OrderCustomer.LastName))
                                                              .Replace("#CUSTOMER_PHONE#", HttpUtility.HtmlEncode(order.OrderCustomer.Phone))
                                                              .Replace("#CUSTOMER_ID#", order.OrderCustomer.CustomerID.ToString());

                var regex = new Regex("<<(.*)>>", RegexOptions.Singleline);
                var match = regex.Match(orderScript);
                var products = new StringBuilder();

                if (match.Groups.Count > 0 && match.Groups[1].Value.IsNotEmpty())
                {
                    var productLine = match.Groups[1].Value;
                    foreach (var item in order.OrderItems)
                    {
                        products.Append(
                            productLine.Replace("#PRODUCT_ARTNO#", HttpUtility.HtmlEncode(item.ArtNo))
                                       .Replace("#PRODUCT_NAME#", HttpUtility.HtmlEncode(item.Name))
                                       .Replace("#PRODUCT_PRICE#", item.Price.ToString("0.##"))
                                       .Replace("#PRODUCT_PRICE_RAW#", item.Price.ToInvariantString())
                                       .Replace("#PRODUCT_AMOUNT#", item.Amount.ToString("0.##")));
                    }

                    orderScript = orderScript.Replace("<<" + productLine + ">>", products.ToString());
                }
                model.SuccessScript = orderScript;
            }

            if (order.LpId.HasValue)
            {
                var lp = new LpService().Get(order.LpId.Value);
                if (lp != null)
                {
                    LpService.CurrentSiteId = lp.LandingSiteId;
                    LpService.CurrentLanding = lp;
                }
            }

            if (!model.IsLanding)
            {
                var tagManager = GoogleTagManagerContext.Current;
                tagManager.CreateTransaction(order);

                model.GoogleAnalyticsString =
                    new GoogleAnalyticsString(SettingsSEO.GoogleAnalyticsNumber, SettingsSEO.GoogleAnalyticsEnabled)
                        .GetForOrder(order);
            }

            if (order.BonusCardNumber != null)
            {
                float newBonusAmount = 0;

                var purchase = PurchaseService.GetByOrderId(order.OrderID);
                if (purchase != null)
                    newBonusAmount = (float) purchase.NewBonusAmount;

                var context = HttpContext.Current;
                if (context != null && context.Session["BonusesForNewCard"] != null)
                { 
                    newBonusAmount += Convert.ToSingle(context.Session["BonusesForNewCard"]);
                    context.Session.Remove("BonusesForNewCard");
                }

                model.NewBonusAmount = newBonusAmount;
            }

            var deferredMail = DeferredMailService.Get(order.OrderID, DeferredMailType.Order);
            if (deferredMail != null)
                DeferredMailService.SendMailByOrder(order, model.NewBonusAmount);

            return model;
        }
    }
}