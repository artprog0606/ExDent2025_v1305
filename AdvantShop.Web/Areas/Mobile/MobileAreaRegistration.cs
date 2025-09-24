using System.Web.Mvc;
using AdvantShop.Web.Infrastructure.Extensions;

namespace AdvantShop.Areas.Mobile
{
    public class MobileAreaRegistration : AreaRegistration
    {
        private const string Subdomain = "m";

        public override string AreaName => "Mobile";

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapMobileRoute(
                "Mobile_Home",
                Subdomain,
                "",
                new {controller = "Home", action = "Index"},
                new[] {"AdvantShop.Areas.Mobile.Controllers"}
            );

            context.MapMobileRoute(
                "Mobile_Product",
                Subdomain,
                url: "products/{url}",
                defaults: new {controller = "Product", action = "Index"},
                namespaces: new[] {"AdvantShop.Areas.Mobile.Controllers"}
            );

            context.MapMobileRoute(
                "Mobile_Category",
                Subdomain,
                url: "categories/{url}",
                defaults: new {controller = "Catalog", action = "Index"},
                namespaces: new[] {"AdvantShop.Areas.Mobile.Controllers"}
            );

            context.MapMobileRoute(
                "Mobile_CategoryTag",
                Subdomain,
                url: "categories/{url}/tag/{tagUrl}",
                defaults: new {controller = "Catalog", action = "Index"},
                namespaces: new[] {"AdvantShop.Areas.Mobile.Controllers"}
            );

            context.MapMobileRoute(
                "Mobile_CatalogRoot",
                Subdomain,
                url: "catalog",
                defaults: new {controller = "Catalog", action = "Index", CategoryId = 0},
                namespaces: new[] {"AdvantShop.Areas.Mobile.Controllers"}
            );

            context.MapMobileRoute(
                "Mobile_ProductList",
                Subdomain,
                url: "productlist/{type}/{list}",
                defaults: new
                {
                    controller = "Catalog", action = "ProductList", type = UrlParameter.Optional,
                    list = UrlParameter.Optional
                },
                namespaces: new[] {"AdvantShop.Areas.Mobile.Controllers"}
            );

            context.MapMobileRoute(
                "Mobile_ProductListTag",
                Subdomain,
                url: "productlist/{type}/tag/{tagUrl}/{list}",
                defaults: new
                {
                    controller = "Catalog", action = "ProductList", type = UrlParameter.Optional,
                    list = UrlParameter.Optional
                },
                namespaces: new[] {"AdvantShop.Areas.Mobile.Controllers"}
            );

            context.MapMobileRoute(
                "Mobile_Search",
                Subdomain,
                url: "search",
                defaults: new {controller = "Catalog", action = "Search"},
                namespaces: new[] {"AdvantShop.Areas.Mobile.Controllers"}
            );

            // context.MapMobileRoute(
            //     "Mobile_Cart",
            //     Subdomain,
            //     url: "cart",
            //     defaults: new {controller = "Cart", action = "Index"},
            //     namespaces: new[] {"AdvantShop.Areas.Mobile.Controllers"}
            // );

            context.MapMobileRoute(
                "Mobile_ChangeCity",
                Subdomain,
                url: "changecity",
                defaults: new {controller = "Home", action = "ChangeCity"},
                namespaces: new[] {"AdvantShop.Areas.Mobile.Controllers"}
            );

            context.MapRoute(
                "Mobile_Root",
                url: "mobile",
                defaults: new {controller = "Error", action = "NotFound", area = ""},
                namespaces: new[] {"AdvantShop.Controllers"}
            );

            context.MapRoute(
                "Mobile_Default",
                url: "mobile/{controller}/{action}/{id}",
                defaults: new {controller = "Home", action = "Index", id = UrlParameter.Optional},
                namespaces: new[] {"AdvantShop.Areas.Mobile.Controllers"}
            );
        }
    }
}