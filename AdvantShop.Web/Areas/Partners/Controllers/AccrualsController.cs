using System.Web.Mvc;
using AdvantShop.Areas.Partners.Handlers.Accruals;

namespace AdvantShop.Areas.Partners.Controllers
{
    public class AccrualsController : BasePartnerController
    {
        public ActionResult Index(int? page)
        {
            SetMetaInformation("Начисления - Личный кабинет партнера");

            var model = new GetAccrualsHandler(page).Get();
            if ((model.Pager.TotalPages < model.Pager.CurrentPage && model.Pager.CurrentPage > 1) || model.Pager.CurrentPage < 0)
                return Error404();

            return View(model);
        }
    }
}