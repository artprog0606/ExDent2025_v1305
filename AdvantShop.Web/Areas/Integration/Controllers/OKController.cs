using System.IO;
using System.Web.Mvc;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Crm.Ok;

namespace AdvantShop.Areas.Integration.Controllers
{
    public class OKController : Controller
    {
        [HttpPost]
        public JsonResult Webhook()
        {
            if (!SettingsOk.OkSubscribeToMessages)
            {
                _ = OkApiService.WebHookUnsubscribe();
                return Json("OK");
            }

            var json = new StreamReader(HttpContext.Request.InputStream).ReadToEnd();

            OkApiService.SaveMessage(json);

            return Json("OK");
        }
    }
}