using System.Web.Mvc;

namespace AdvantShop.App.Landing
{
    public class LandingAreaRegistration : AreaRegistration 
    {
        public override string AreaName
        {
            get { return "Landing"; }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                name: "LandingUser",
                url: "lp/user/{action}/{id}",
                defaults: new {controller = "LandingUser", action = "Auth", area = "Landing", id = UrlParameter.Optional},
                namespaces: new[] {"AdvantShop.App.Landing.Controllers"}
                );

            context.MapRoute(
                name: "Landing",
                url: "lp/{url}/{lpurl}",
                defaults: new {controller = "Landing", action = "Index", area = "Landing", lpurl = UrlParameter.Optional},
                namespaces: new[] {"AdvantShop.App.Landing.Controllers"}
                );

            context.MapRoute(
                name: "Landing_default",
                url: "Landing/{controller}/{action}/{id}",
                defaults: new {controller = "Landing", action = "Index", area = "Landing", id = UrlParameter.Optional},
                namespaces: new[] {"AdvantShop.App.Landing.Controllers"}
                );
        }
    }
}
