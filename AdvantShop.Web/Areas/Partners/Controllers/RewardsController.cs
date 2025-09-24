using System.Web.Mvc;
using AdvantShop.Areas.Partners.Handlers.Rewards;
using AdvantShop.Core.Services.Partners;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Areas.Partners.Controllers
{
    public class RewardsController : BasePartnerController
    {
        public ActionResult Index(int? page)
        {
            SetMetaInformation("Выплаты - Личный кабинет партнера");
            SetNgController(NgControllers.NgControllersTypes.PartnerRewardsCtrl);

            var model = new GetRewardsHandler(page).Get();
            if ((model.Pager.TotalPages < model.Pager.CurrentPage && model.Pager.CurrentPage > 1) || model.Pager.CurrentPage < 0)
                return Error404();

            return View(model);
        }

        [HttpPost]
        [ValidateJsonAntiForgeryToken]
        public JsonResult SaveNaturalPersonPaymentData(int? paymentTypeId, string paymentAccountNumber)
        {
            var partner = PartnerContext.CurrentPartner;
            
            if (partner.NaturalPerson == null)
                partner.NaturalPerson = new NaturalPerson();
            
            partner.NaturalPerson.PaymentTypeId = paymentTypeId;
            partner.NaturalPerson.PaymentAccountNumber = paymentAccountNumber;
            
            PartnerService.UpdatePartner(partner);

            return JsonOk();
        }
    }
}