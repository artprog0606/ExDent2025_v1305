using System;
using System.Linq;
using System.Web;
using AdvantShop.Catalog;
using AdvantShop.Orders;
using Newtonsoft.Json;

namespace AdvantShop.App.Landing.Domain.Trackers.YandexMetrika
{
    public class YaMetrikaService
    {
        public string YaMetrikaHeadScript(string counterId, string jsScript, bool collectip = true)
        {
            if (string.IsNullOrEmpty(counterId) || string.IsNullOrEmpty(jsScript))
                return null;

            return string.Format(
                "<script>window.yaCounterId=\"{0}\"; window.dataLayer = window.dataLayer || []; {1}</script>\n {2}\n",
                counterId,
                collectip ? "var yaParams={ip_adress: '" + System.Web.HttpContext.Current.Request.UserHostAddress + "'};" : "", 
                jsScript);
        }

        public string CheckoutFinalStepScript(string counterId, string jsScript, IOrder order)
        {
            if (string.IsNullOrEmpty(counterId) || string.IsNullOrEmpty(jsScript))
                return null;

            var orderParams = new
            {
                order_id = order.OrderID.ToString(),
                order_price = order.Sum,
                currency = order.OrderCurrency.CurrencyCode,
                exchange_rate = 1,
                goods = order.OrderItems.Select(orderItem => new
                {
                    id = HttpUtility.HtmlEncode(orderItem.ArtNo),
                    name = HttpUtility.HtmlEncode(orderItem.Name),
                    price = orderItem.Price,
                    quantity = orderItem.Amount > 1 ? (int)Math.Round(orderItem.Amount) : 1
                }).ToList()
            };
            
            var cookiePrefix = "tid_ya_" + order.OrderID;

            var script = @"
<script type=""text/javascript"">
    function writeOrderCookieLpYa(prefix) {
        var currentTime = Math.round(new Date().getTime() / 1000);

        var d = new Date();
        d.setTime(d.getTime() + (7 * 24 * 60 * 60 * 1000));
        var expires = 'expires=' + d.toGMTString();

        document.cookie = prefix + '=' + currentTime + '; ' + expires;
    }

    function checkOrderCookieLpYa(prefix) {
        var cname = '';
        cname = prefix + '=';
        var cookies = document.cookie.split(';');

        for (var i = 0; i < cookies.length; i++) {
            var ck = cookies[i].trim().toString();
            if (ck.indexOf(cname) === 0) {
                return ck.substring(cname.length).toString();
            }
        }
    }
    
    var hasCookie = checkOrderCookieLpYa('" + cookiePrefix + @"'); 
    if (typeof hasCookie === 'undefined') {
        (function yaMetrikaLpWatcher() {
            if (typeof(window.yaCounter" + counterId + @") != 'undefined') {
                var yaParams = " + JsonConvert.SerializeObject(orderParams) + @";
                yaCounter" + counterId + @".reachGoal('Order', yaParams);
                " + GetEcommerceScript(order) + @"
            } else {
                setTimeout(yaMetrikaLpWatcher, 1000);
            }
        })();
        writeOrderCookieLpYa('" + cookiePrefix + @"');
    }
</script>";

            return script;
        }
        
        private string GetEcommerceScript(IOrder order)
        {
            var actionField = new YandexMetrikaEcommerceActionField()
            {
                Id = order.Number,
                Coupon = order.Coupon?.Code
            };

            var products = order.OrderItems.Select(x => new YandexMetrikaEcommerceProduct()
            {
                Id = x.ArtNo,
                Name = x.Name,
                Price = x.Price,
                Category = x.ProductID != null ? GetCategory(x.ProductID.Value) : null,
                Brand = x.ProductID != null ? GetBrand(x.ProductID.Value) : null,
                Quantity = x.Amount > 1 ? (int)Math.Round(x.Amount) : 1
            });

            var result =
                string.Format(
                    "\n window.{2}.push({{\"ecommerce\": {{ \"purchase\": \n {{ \"actionField\": {0}, \n  \"products\":{1} }} \n }} \n }}); \n ",
                    JsonConvert.SerializeObject(actionField),
                    JsonConvert.SerializeObject(products),
                    "dataLayer"); // ContainerName

            return result;
        }


        private string GetCategory(int productId)
        {
            var categories = ProductService.GetCategoriesByProductId(productId);
            if (categories.Count > 5)
                categories = categories.Skip(categories.Count - 5).ToList();

            return String.Join("/", categories.Select(x => x.Name));
        }

        private string GetBrand(int productId)
        {
            var product = ProductService.GetProduct(productId);
            if (product == null || product.Brand == null)
                return null;

            return product.Brand.Name;
        }
    }
}
