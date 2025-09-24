using System.Web.Mvc;
using AdvantShop.Web.Admin.Handlers.Settings.Users;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Web.Admin.Controllers.Settings
{
    public class SettingsTwoFactorAuthController : BaseAdminController
    {
        [HttpGet]
        public JsonResult GetQrCode(string email, string password, string moduleId)
        {
            return Json(new GetQrCodeHandler(email, password, moduleId).Execute());
        }
        
        [HttpGet]
        public JsonResult GetAuthActive(string email, string password, string moduleId)
        {
            return Json(new GetAuthHandler(email, password, moduleId).Execute());
        }
        
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SaveAuthActive(string email, string password, string moduleId, bool twoFactorAuthActive)
        {
            new SaveAuthHandler(email, password, moduleId, twoFactorAuthActive).Execute();
            return Json(true);
        }
    }
}