using System.Web.Mvc;
using AdvantShop.Web.Admin.Handlers.Settings.Mails;

namespace AdvantShop.Web.Admin.Controllers.Settings
{
    public class SettingsAuthCallController : BaseAdminController
    {
        public JsonResult GetAuthCallMods(string moduleStringId)
        {
            return Json(new GetAuthCallModsHandler(moduleStringId).Execute());
        }
    }
}