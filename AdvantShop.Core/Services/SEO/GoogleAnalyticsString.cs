//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Customers;
using AdvantShop.Orders;

namespace AdvantShop.SEO
{
    public class GoogleAnalyticsItem
    {
        public string OrderId { get; set; }
        public string Sku { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Price { get; set; }
        public string Quantity { get; set; }
    }

    public class GoogleAnalyticsTrans
    {
        public string OrderId { get; set; }
        public string Affiliation { get; set; }
        public string Total { get; set; }
        public string Tax { get; set; }
        public string Shipping { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string CurrencyCode { get; set; }
    }

    /// <summary>
    /// GoogleAnalyticsString
    /// </summary>
    public class GoogleAnalyticsString
    {
        public GoogleAnalyticsString(string gaId, bool enabled)
        {
            GaId = gaId;
            Enabled = enabled;
        }

        public string GaId { get; private set; }
        public bool Enabled { get; private set; }
        
        private const string GoogleAnalytics =
            "<script>\n " +

            "(function(i,s,o,g,r,a,m){{i['GoogleAnalyticsObject']=r;i[r]=i[r]||function(){{ " +
            "(i[r].q=i[r].q||[]).push(arguments)}},i[r].l=1*new Date();a=s.createElement(o), " +
            "m=s.getElementsByTagName(o)[0];a.async=1;a.src=g;m.parentNode.insertBefore(a,m) " +
            "}})(window,document,'script','//www.google-analytics.com/analytics.js','ga'); \n" +

            "ga('create', '{0}', 'auto'); \n" +
            "{1}" + 
            "ga('send', 'pageview'); \n" +
            "/* Accurate bounce rate by time */ \n" +
            "if (!document.referrer ||  document.referrer.split('/')[2].indexOf(location.hostname) != 0) \n" +
            "setTimeout(function() \n" +
            "        {{ \n" +
            "            ga('send', 'event', 'New visitor', location.pathname); \n" +
            "        }}, 15000); \n" +
            "</script> \n";

        private const string GaAddTransaction =
            "ga('ecommerce:addTransaction', {{ " +
                "'id': '{0}', " +
                "'affiliation': '{1}', " +
                "'revenue': '{2}', " +
                "'shipping': '{3}', " +
                "'tax': '{4}'{5}" +
            "}}); ";

        private const string GaAddItems =
            "ga('ecommerce:addItem', {{ " +
                "'id': '{0}', " +
                "'name': '{1}', " +
                "'sku': '{2}', " +
                "'category': '{3}', " +
                "'price': '{4}', " +
                "'quantity': '{5}' " +
            "}}); \r\n";


        public string GetGoogleAnalyticsString(bool useDemogrReports = true)
        {
            if (!Enabled || GaId.IsNullOrEmpty())
                return string.Empty;

            var gaWithPrefix = (GaId.Contains("ua-", StringComparison.InvariantCultureIgnoreCase)
                ? ""
                : "UA-") + GaId;

            return string.Format(GoogleAnalytics, gaWithPrefix,
                (useDemogrReports ? "ga('require', 'displayfeatures');\n" : "") +
                "ga('set', '&uid', '" + CustomerContext.CurrentCustomer.Id + "');\n"
            );
        }
        
        public string GetForOrder(Order order, string storeName = null)
        {
            if (SettingsSEO.GoogleAnalyticsSendOrderOnStatusChanged)
                return string.Empty;

            var trans = new GoogleAnalyticsTrans
            {
                OrderId = order.OrderID.ToString(),
                Affiliation = storeName ?? SettingsMain.ShopName,
                Total = (order.Sum - order.ShippingCostWithDiscount).ToString("F2", CultureInfo.InvariantCulture),
                Shipping = order.ShippingCostWithDiscount.ToString("F2", CultureInfo.InvariantCulture),
                City = order.OrderCustomer.City,
                State = string.Empty,
                Country = order.OrderCustomer.Country,
                CurrencyCode = order.OrderCurrency.CurrencyCode
            };
            var items = GetListItemForGoogleAnalytics(order.OrderItems, order.OrderID).ToList();

            return GetEComerce(trans, items);
        }

        private string GetEComerce(GoogleAnalyticsTrans trans, List<GoogleAnalyticsItem> items)
        {
            if (!Enabled || GaId.IsNullOrEmpty() || trans == null || items.Count < 1)
                return string.Empty;

            var sb = new StringBuilder();
            sb.AppendLine("<script> ");

            sb.AppendLine(
@"function writeOrderCookie(prefix) {
    var currentTime = Math.round(new Date().getTime() / 1000);

    //set expiration time for new cookie
    var d = new Date();
    d.setTime(d.getTime() + (7 * 24 * 60 * 60 * 1000));
    var expires = 'expires=' + d.toGMTString();

    document.cookie = prefix + '=' + currentTime + '; ' + expires;
}

function checkOrderCookie(prefix) {
    var cname = '';
    cname = prefix + '=';
    var cookies = document.cookie.split(';');
  
    for (var i = 0; i < cookies.length; i++) {
        var ck = cookies[i].trim().toString();
        if (ck.indexOf(cname) === 0) {
            return ck.substring(cname.length).toString();
        }
    }
}");

            sb.AppendLine("var hasCookie = checkOrderCookie('tid_" + trans.OrderId + "'); ");
            sb.AppendLine("if (typeof hasCookie === 'undefined') { ");

            sb.AppendLine("  ga('require', 'ecommerce', 'ecommerce.js');");
            sb.AppendFormat(GaAddTransaction, trans.OrderId, HttpUtility.HtmlEncode(trans.Affiliation), trans.Total, trans.Shipping, trans.Tax, 
                GoogleAnalyticsService.IsAvailableCurrency(trans.CurrencyCode) ? string.Format(", 'currency': '{0}'", trans.CurrencyCode) : null);

            foreach (var item in items)
            {
                sb.AppendFormat(GaAddItems, item.OrderId, HttpUtility.HtmlEncode(item.Name),
                    HttpUtility.HtmlEncode(item.Sku), HttpUtility.HtmlEncode(item.Category), item.Price, item.Quantity);
            }

            sb.AppendLine("  ga('ecommerce:send');");

            sb.AppendLine("  writeOrderCookie('tid_" + trans.OrderId + "'); ");
            sb.AppendLine("}");
            sb.AppendLine("</script>");

            return sb.ToString();
        }

        private static IEnumerable<GoogleAnalyticsItem> GetListItemForGoogleAnalytics(IEnumerable<OrderItem> orderItems, int orderid)
        {
            return from item in orderItems
                   where item.ProductID != null
                   let categoryId = ProductService.GetFirstCategoryIdByProductId((int)item.ProductID)
                   select new GoogleAnalyticsItem
                   {
                       OrderId = orderid.ToString(),
                       Sku = item.ArtNo,
                       Name = item.Name,
                       Category = categoryId != -1 ? CategoryService.GetCategory(categoryId).Name : "",
                       Price = item.Price.ToString("F2", CultureInfo.InvariantCulture),
                       Quantity = item.Amount.ToString()
                   };
        }
    }
}