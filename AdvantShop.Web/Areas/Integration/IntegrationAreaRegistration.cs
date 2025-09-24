using System.Web.Mvc;
using System.Web.Routing;

namespace AdvantShop.Areas.Integration
{
    public class IntegrationAreaRegistration : AreaRegistration
    {
        public override string AreaName => "Integration";

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                name: "Integration_Default",
                url: "integration/{controller}/{action}/{id}",
                defaults: new {controller = "Home", action = "Index", id = UrlParameter.Optional},
                namespaces: new[] {"AdvantShop.Areas.Integration.Controllers"}
            );
        }
    }
}