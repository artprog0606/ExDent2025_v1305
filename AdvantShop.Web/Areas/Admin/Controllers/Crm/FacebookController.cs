using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Core.Services.Crm.Facebook;
using AdvantShop.Core.Services.Crm.Facebook.Models;
using AdvantShop.Core.Services.Crm.SalesFunnels;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Models.Crm;
using AdvantShop.Web.Infrastructure.Filters;
using Newtonsoft.Json;

namespace AdvantShop.Web.Admin.Controllers.Crm
{
    [Auth(RoleAction.Settings, RoleAction.Customers)]
    public partial class FacebookController : BaseAdminController
    {
        private readonly FacebookApiService _fbService = new FacebookApiService();

        #region Settings 

        public JsonResult GetSettings()
        {
            var isActive = _fbService.IsActive();

            return Json(new
            {
                isActive = isActive,
                clientId = SettingsFacebook.ClientId,
                groupId = SettingsFacebook.GroupId,
                groupName = SettingsFacebook.GroupName,
                salesFunnels = SalesFunnelService.GetList(),
                salesFunnelId = SettingsCrm.DefaultFacebookSalesFunnelId,
                verifyToken = SettingsFacebook.VerifyToken,
                verifyUrl = UrlService.GetUrl("integration/facebook"),
                createLeadFromMessages = SettingsFacebook.CreateLeadFromMessages,
                createLeadFromComments = SettingsFacebook.CreateLeadFromComments
            });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public ActionResult SaveAuthUser(string clientId, string clientSecret, string code, string redirectUrl)
        {
            var result = _fbService.GetUserAuthTokenByCode(clientId, clientSecret, code, redirectUrl);
            if (!result)
                return JsonError(T("Admin.Shared.AuthorisationError"));

            var groupTokens = _fbService.GetUserGroupTokens();
            if (groupTokens == null)
                return JsonError(T("Admin.Shared.ErrorFetchingGroups"));

            return JsonOk(groupTokens.Select(x => new
            {
                x.AccessToken,
                x.Id,
                x.Name,
                x.Permissions
            }));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public ActionResult SaveGroupToken(FacebookGroupItemAccessToken group)
        {
            if (!_fbService.IsActive() && Core.Services.Crm.SocialNetworkService.IntegrationsLimitRiched())
                return JsonError(T("Admin.SettingsCrm.SocialNetworks.LimitRiched"));

            if (group == null ||
                string.IsNullOrWhiteSpace(group.AccessToken) ||
                string.IsNullOrWhiteSpace(group.Id) ||
                string.IsNullOrWhiteSpace(group.Name))
            {
                return JsonError();
            }

            SettingsFacebook.GroupId = group.Id;
            SettingsFacebook.GroupName = group.Name;
            SettingsFacebook.GroupToken = group.AccessToken;
            SettingsFacebook.GroupPerms = String.Join(",", group.Permissions);

            Task.Run(() => _fbService.GetLastMessages());

            Track.TrackService.TrackEvent(Track.ETrackEvent.Shop_Facebook_GroupConnected);

            return Json(new
            {
                Id = SettingsFacebook.GroupId,
                Name = SettingsFacebook.GroupName,
                IsActive = _fbService.IsActive()
            });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteGroup()
        {
            _fbService.DeActivate();
            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SaveSettings(SaveFbSettingsModel model)
        {
            SettingsCrm.DefaultFacebookSalesFunnelId = model.Id;
            SettingsFacebook.CreateLeadFromMessages = model.CreateLeadFromMessages;
            SettingsFacebook.CreateLeadFromComments = model.CreateLeadFromComments;

            return JsonOk();
        }

        #endregion

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SendFacebookMessage(int id, string message)
        {
            var result = _fbService.SendMessage(id, message.Replace("\r\n", "<br>"));
            return result ? JsonOk() : JsonError();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddFacebookUser(Guid customerId, string link)
        {
            try
            {
                var facebookService = new FacebookApiService();
                var result = facebookService.AddFacebookUserByLink(customerId, link);

                return result ? JsonOk() : JsonError();
            }
            catch (BlException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            return JsonError();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteLink(Guid customerId)
        {
            var u = FacebookService.GetUser(customerId);
            if (u != null)
                FacebookService.DeleteUser(customerId);

            return JsonOk();
        }
    }

    #region Facebook webhook

    [Obsolete("use controller in integration area")]
    public class FacebookWebHookController : Controller
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

    #endregion
}
