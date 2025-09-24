using System.Web.Mvc;

namespace AdvantShop.Areas.Partners
{
    public class PartnersAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get { return "Partners"; }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                name: "Partners_Home",
                url: "partners",
                defaults: new { controller = "Home", action = "Index", area = "Partners" },
                namespaces: new[] { "AdvantShop.Areas.Partners.Controllers" }
            );

            context.MapRoute(
                "Partners_Login",
                "partners/login",
                new { controller = "Account", action = "Login", area = "Partners" },
                namespaces: new[] { "AdvantShop.Areas.Partners.Controllers" }
            );

            context.MapRoute(
                "Partners_Default",
                "partners/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", area = "Partners", id = UrlParameter.Optional },
                namespaces: new[] { "AdvantShop.Areas.Partners.Controllers" }
            );
        }
    }
}