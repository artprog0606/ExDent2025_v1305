using AdvantShop.Handlers.Shipings;
using AdvantShop.SEO;
using System.Web.Mvc;

namespace AdvantShop.Controllers
{
    public class ShippingsController : BaseClientController
    {
        // GET
        public ActionResult Index()
        {
            SetMetaInformation(new MetaInfo(T("Common.ShippingsPage.Meta.Title")));
            return View(new GetShippingZonesPageHandler().Execute());
        }
    }
}