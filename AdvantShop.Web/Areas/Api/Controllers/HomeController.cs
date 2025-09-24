using System.Web.Mvc;
using AdvantShop.Areas.Api.Models.Home;
using AdvantShop.Configuration;

namespace AdvantShop.Areas.Api.Controllers
{
    public class HomeController : BaseApiController
    {
        // GET: Api/Home
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult Version()
        {
            return JsonOk(new VersionInfoModel
            {
                Version = SettingsGeneral.SiteVersionDev,
                PublicVersion = SettingsGeneral.SiteVersion,
                Release = SettingsGeneral.Release
            });
        }
    }
}