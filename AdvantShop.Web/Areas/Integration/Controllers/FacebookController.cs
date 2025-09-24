using System.Web.Mvc;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Crm.Facebook;
using AdvantShop.Core.Services.Crm.Facebook.Models;
using AdvantShop.Diagnostics;
using Newtonsoft.Json;

namespace AdvantShop.Areas.Integration.Controllers
{
    public class FacebookController : Controller
    {
        private readonly FacebookApiService _fbService = new FacebookApiService();

        // Прохождение проверки
        [HttpGet]
        public ActionResult Index()
        {
            var mode = Request["hub.mode"];
            var token = Request["hub.verify_token"];
            var challenge = Request["hub.challenge"];

            Debug.Log.Info(
                string.Format("/facebookwebhook index hub.mode = {0}, hub.verify_token={1}, hub.challenge={2}",
                    mode, token, challenge));

            if (mode == "subscribe" && token != SettingsFacebook.VerifyToken)
            {
                Response.Status = "403 Forbidden";
                return new EmptyResult();
            }

            return Content(challenge);
        }

        // Получение входящих сообщений
        [HttpPost]
        public ActionResult Index(FbWebhookModel model)
        {
            Debug.Log.Info("/facebookwebhook " + JsonConvert.SerializeObject(model));

            if (model == null || model.@object != "page")
            {
                Response.Status = "403 Forbidden";
                return new EmptyResult();
            }

            _fbService.SaveWebHookMessage(model);

            return Content("EVENT_RECEIVED");
        }
    }
}