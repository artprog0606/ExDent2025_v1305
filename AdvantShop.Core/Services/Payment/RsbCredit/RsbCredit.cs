using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Orders;
using AdvantShop.Taxes;

namespace AdvantShop.Payment
{
    [PaymentKey("RsbCredit")]
    public class RsbCredit : PaymentMethod, ICreditPaymentMethod
    {
        private const float MinOrderPrice = 3000;
        private const int DefFirstPayment = 10;

        public string PartnerId { get; set; }
        public float MinimumPrice { get; set; }
        public float? MaximumPrice { get; set; }
        public EnTypePresentationOfCreditInformation TypePresentationOfCreditInformation =>
            EnTypePresentationOfCreditInformation.FirstPayment;
        public float FirstPayment { get; set; }
        public bool ActiveCreditPayment => true;
        public bool ShowCreditButtonInProductCard => true;
        public string CreditButtonTextInProductCard => null;

        public decimal OrderSum { get; set; }
        

        public override ProcessType ProcessType
        {
            get { return ProcessType.Javascript; }
        }

        public override Dictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>
                           {
                               {RsbCreditTemplate.PartnerId, PartnerId},
                               {RsbCreditTemplate.MinimumPrice, MinimumPrice.ToInvariantString()},
                               {RsbCreditTemplate.MaximumPrice, MaximumPrice?.ToInvariantString()},
                               {RsbCreditTemplate.FirstPayment, FirstPayment.ToInvariantString()}
                           };
            }
            set
            {
                PartnerId = value.ElementOrDefault(RsbCreditTemplate.PartnerId);
                MinimumPrice = value.ElementOrDefault(RsbCreditTemplate.MinimumPrice).TryParseFloat(MinOrderPrice);
                MaximumPrice = value.ElementOrDefault(RsbCreditTemplate.MaximumPrice).TryParseFloat(true);
                FirstPayment = value.ElementOrDefault(RsbCreditTemplate.FirstPayment).TryParseFloat(DefFirstPayment);
            }
        }

        public float? GetFirstPayment(float finalPrice)
        {
            return finalPrice * FirstPayment / 100;
        }

        public (float AmountPyament, int NumberOfPayments) GetAmountAndNumberOfPayments(float finalPrice) => default;

        public override string ProcessJavascript(Order order)
        {
            var orderItemsCount = 0;
            var orderItemsParams = "";


            var paymentCurrency = PaymentCurrency ?? order.OrderCurrency;
            foreach (var item in order.GetOrderItemsForFiscal(paymentCurrency))
            {
                orderItemsCount++;
                orderItemsParams += string.Format("&TC_{0}={1}&TPr_{0}={2}&TName_{0}={3}",
                                            orderItemsCount, item.Amount.ToInvariantString(), item.Price.ToString("F2", CultureInfo.InvariantCulture), HttpUtility.UrlEncode(item.Name));
            }

            var shippingCost = order.ShippingCostWithDiscount.ConvertCurrency(order.OrderCurrency, paymentCurrency);

            if (shippingCost > 0)
            {
                orderItemsCount++;
                orderItemsParams += string.Format("&TC_{0}={1}&TPr_{0}={2}&TName_{0}={3}",
                                            orderItemsCount, 1, ((float)Math.Round(shippingCost)).ToString("F2", CultureInfo.InvariantCulture), HttpUtility.UrlEncode(LocalizationService.GetResource("Core.Payment.Receipt.ShippingName")));
            }

            var link = String.Format("https://www.onlinecredit.ru/sites/anketa/minipotreb.php?idTpl={0}&TTName={1}&Order={2}&TCount={3}{4}&UserName={5}&UserLastName={6}&UserMail={7}",
                                        PartnerId,
                                        SettingsMain.SiteUrlPlain,
                                        order.OrderID,
                                        orderItemsCount,
                                        orderItemsParams,
                                        order.OrderCustomer.FirstName,
                                        order.OrderCustomer.LastName,
                                        order.OrderCustomer.Email != "admin" ? order.OrderCustomer.Email : "");

            var sb = new StringBuilder();

            sb.Append("<script type=\"text/javascript\"> ");
            sb.AppendLine("function openrsbcredit() {{");
            sb.AppendFormat("window.open(\"{0}\", \"_blank\");\r\n", link);
            sb.AppendLine("}} ");
            sb.AppendLine("</script>");

            return sb.ToString();
        }

        public override string ProcessJavascriptButton(Order order)
        {
            return "openrsbcredit();";
        }
    }
}