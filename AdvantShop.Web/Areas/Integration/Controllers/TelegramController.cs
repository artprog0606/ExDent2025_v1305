using System.IO;
using System.Web.Mvc;
using AdvantShop.Core.Services.Crm.Telegram;

namespace AdvantShop.Areas.Integration.Controllers
{
    public class TelegramController : Controller
    {
        private readonly TelegramApiService _apiService = new TelegramApiService();

        [HttpPost]
        public JsonResult Webhook()
        {
            var json = new StreamReader(HttpContext.Request.InputStream).ReadToEnd();

            _apiService.SaveMessage(json);

            return Json("ok");
        }
    }
}