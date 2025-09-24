using System.Web.Mvc;
using AdvantShop.Web.Admin.Handlers.Design;
using AdvantShop.Web.Infrastructure.Controllers;

namespace AdvantShop.Web.Admin.Controllers.Shared
{
     public partial class StoreController : BaseAdminController
    {
         public ActionResult Index()
         {
            SetMetaInformation("Мои сайты");
            SetNgController(NgControllers.NgControllersTypes.DesignCtrl);

            var model = new DesignHandler(Request["stringid"]).Execute();

            return View(model);
         }

         public ActionResult Domains()
         {
             SetMetaInformation("Мои сайты");
             SetNgController(NgControllers.NgControllersTypes.StorePageCtrl);

            return View();
         }
    }
}
