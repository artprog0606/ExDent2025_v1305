using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Helpers;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Core.Services.Taxes;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;
using AdvantShop.Repository.Currencies;
using AdvantShop.Taxes;

namespace AdvantShop.Core.Services.Payment.Rbkmoney2
{
    public class Rbkmoney2Service
    {
        private readonly string _apiKey;
        private readonly string _shopId;

        public Rbkmoney2Service(string apiKey, string shopId)
        {
            _apiKey = apiKey;
            _shopId = shopId;
        }

        public InvoiceCreatedResponse CreateInvoice(Order order, string orderDescription, Currency paymentCurrency, TaxElement tax)
        {
            if (_apiKey.IsNullOrEmpty() || _shopId.IsNullOrEmpty())
                return null;

            paymentCurrency = paymentCurrency ?? order.OrderCurrency;
            
            var invoice = new InvoiceModel
            {
                ShopId = _shopId,
                DueDate = DateTime.Now.AddMonths(3).ToString("yyyy-MM-ddTHH:mm:ssK"),
                Currency = paymentCurrency.Iso3,
                Product = orderDescription,
                Cart = 
                    order
                        .GetOrderItemsForFiscal(paymentCurrency, toIntegerAmount: true)
                        .Select(item =>
                            new CartItemModel
                            {
                                Product = item.Name.Reduce(1000),
                                Price = (int)(Math.Round(item.Price * 100, 0)),
                                Quantity = (int)item.Amount,
                                TaxMode = GetTaxMode(tax?.TaxType ?? item.TaxType, item.PaymentMethodType)
                            })
                        .ToList(),
                Metadata = new MetadataModel
                {
                    OrderId = order.OrderID
                }
            };

            if (order.OrderCertificates != null && order.OrderCertificates.Count > 0)
            {
                var certTax = TaxService.GetCertificateTax();
                invoice.Cart.AddRange(order.OrderCertificates
                    .ConvertCurrency(order.OrderCurrency, paymentCurrency)
                    .Select(x =>
                    new CartItemModel
                    {
                        Product = $"{LocalizationService.GetResource("Core.Payment.Receipt.GiftCertificateName")} {x.CertificateCode}",
                        Price = (int)(Math.Round(x.Sum * 100, 2)),
                        Quantity = 1,
                        TaxMode = GetTaxMode(tax?.TaxType ?? certTax?.TaxType, AdvantShop.Configuration.SettingsCertificates.PaymentMethodType)
                    }));
            }

            var orderShippingCostWithDiscount = 
                order.ShippingCostWithDiscount
                    .ConvertCurrency(order.OrderCurrency, paymentCurrency);
            if (orderShippingCostWithDiscount > 0)
            {
                invoice.Cart.Add(new CartItemModel()
                {
                    Product = LocalizationService.GetResource("Core.Payment.Receipt.ShippingName"),
                    Price = (int)(Math.Round(orderShippingCostWithDiscount * 100, 2)),
                    Quantity = 1,
                    TaxMode = GetTaxMode(tax?.TaxType ?? order.ShippingTaxType, order.ShippingPaymentMethodType)
                });
            }


            var headers = new Dictionary<string, string>
            {
                { "X-Request-ID", Guid.NewGuid().ToString().Replace("-", "") }, // [ 1 .. 32 ] characters
                { "Authorization", "Bearer " + _apiKey },
                { "Accept", "application/json" }
            };

            try
            {
                return RequestHelper.MakeRequest<InvoiceCreatedResponse>(
                    "https://api.rbk.money/v1/processing/invoices",
                    data: invoice, headers: headers,
                    contentType: ERequestContentType.TextJsonUtf8);
            }
            catch (BlException ex)
            {
                return new InvoiceCreatedResponse { Message = ex.Message };
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex.Message, ex);
            }
            return null;
        }


        public InvoiceModel GetInvoice(string invoiceId)
        {
            if (_apiKey.IsNullOrEmpty())
                return null;

            var headers = new Dictionary<string, string>
            {
                { "X-Request-ID", Guid.NewGuid().ToString().Replace("-", "") }, // [ 1 .. 32 ] characters
                { "Authorization", "Bearer " + _apiKey }
            };

            try
            {
                return RequestHelper.MakeRequest<InvoiceModel>(
                    "https://api.rbk.money/v1/processing/invoices/" + invoiceId,
                    headers: headers,
                    method: ERequestMethod.GET);
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex.Message, ex);
            }
            return null;
        }

        private TaxModeModel GetTaxMode(TaxType? taxType, ePaymentMethodType paymentMethodType)
        {
            if (!taxType.HasValue || taxType.Value == TaxType.VatWithout)
                return null;

            var taxMode = new TaxModeModel
            {
                Type = "InvoiceLineTaxVAT"
            };

            switch (taxType.Value)
            {
                case TaxType.Vat0:
                    taxMode.Rate = "0%";
                    break;
                case TaxType.Vat10:
                    if (AdvantShop.Configuration.SettingsCheckout.TaxTypeByPaymentMethodType &&
                        (paymentMethodType == ePaymentMethodType.full_prepayment ||
                         paymentMethodType == ePaymentMethodType.partial_prepayment ||
                         paymentMethodType == ePaymentMethodType.advance))
                        taxMode.Rate = "10/110";
                    else
                        taxMode.Rate = "10%";
                    break;
                case TaxType.Vat18:
                    if (AdvantShop.Configuration.SettingsCheckout.TaxTypeByPaymentMethodType &&
                        (paymentMethodType == ePaymentMethodType.full_prepayment ||
                         paymentMethodType == ePaymentMethodType.partial_prepayment ||
                         paymentMethodType == ePaymentMethodType.advance))
                        taxMode.Rate = "18/118";
                    else
                        taxMode.Rate = "18%";
                    break;
                case TaxType.Vat20:
                    if (AdvantShop.Configuration.SettingsCheckout.TaxTypeByPaymentMethodType &&
                        (paymentMethodType == ePaymentMethodType.full_prepayment ||
                         paymentMethodType == ePaymentMethodType.partial_prepayment ||
                         paymentMethodType == ePaymentMethodType.advance))
                        taxMode.Rate = "20/120";
                    else
                        taxMode.Rate = "20%";
                    break;
                case TaxType.Vat5:
                    if (AdvantShop.Configuration.SettingsCheckout.TaxTypeByPaymentMethodType &&
                        (paymentMethodType == ePaymentMethodType.full_prepayment ||
                         paymentMethodType == ePaymentMethodType.partial_prepayment ||
                         paymentMethodType == ePaymentMethodType.advance))
                        taxMode.Rate = "5/105";
                    else
                        taxMode.Rate = "5%";
                    break;
                case TaxType.Vat7:
                    if (AdvantShop.Configuration.SettingsCheckout.TaxTypeByPaymentMethodType &&
                        (paymentMethodType == ePaymentMethodType.full_prepayment ||
                         paymentMethodType == ePaymentMethodType.partial_prepayment ||
                         paymentMethodType == ePaymentMethodType.advance))
                        taxMode.Rate = "7/107";
                    else
                        taxMode.Rate = "7%";
                    break;
                default:
                    throw new NotImplementedException("taxType " + taxType.Value + " not implemented");
            };

            return taxMode;
        }

    }
}