using System.Web.Mvc;
using AdvantShop.Areas.Api.Controllers;

namespace AdvantShop.Areas.Integration.Controllers
{
    public class HomeController : BaseApiController
    {
        // GET: Integration/Home
        public ActionResult Index()
        {
            return View();
        }
    }
}