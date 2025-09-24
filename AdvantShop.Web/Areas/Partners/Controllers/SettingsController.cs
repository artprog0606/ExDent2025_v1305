using System;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Areas.Partners.Models.Settings;
using AdvantShop.Areas.Partners.ViewModels.Settings;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Controls;
using AdvantShop.Core.Services.Partners;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Areas.Partners.Controllers
{
    public class SettingsController : BasePartnerController
    {
        public ActionResult Index()
        {
            SetMetaInformation("Настройки - Личный кабинет партнера");
            SetNgController(NgControllers.NgControllersTypes.PartnerSettingsCtrl);

            var partner = PartnerContext.CurrentPartner;
            var model = new SettingsViewModel
            {
                Partner = partner
            };
            foreach (EPartnerMessageType messageType in Enum.GetValues(typeof(EPartnerMessageType))
                .Cast<EPartnerMessageType>().Where(x => x != EPartnerMessageType.None))
            {
                model.SendMessages[messageType.ToString()] = partner.SendMessages.HasFlag(messageType);
            }

            return View(model);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SaveCommonInfo(CommonSettingsModel model)
        {
            if (ModelState.IsValid)
            {
                var partner = PartnerContext.CurrentPartner;
                partner.Name = model.Name;
                partner.Phone = model.Phone;
                partner.City = model.City;

                partner.SendMessages = EPartnerMessageType.None;
                foreach (var messageType in model.SendMessages.Keys.Where(key => model.SendMessages[key]).ToList())
                    partner.SendMessages |= messageType.TryParseEnum<EPartnerMessageType>();

                PartnerService.UpdatePartner(partner);

                return JsonOk();
            }

            return JsonError();
        }

        [ChildActionOnly]
        public ActionResult ChangePassword()
        {
            return PartialView(new ChangePasswordModel());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var partner = PartnerContext.CurrentPartner;
                PartnerService.ChangePassword(partner.Id, model.NewPassword, false);
                PartnerAuthService.SignIn(partner.Email, model.NewPassword, false);
                ShowMessage(NotifyType.Success, "Изменения сохранены");

                return RedirectToAction("Index");
            }

            ShowErrorMessages();

            return new RedirectResult(Url.Action("Index") + "?tab=changepassword");
        }
    }
}