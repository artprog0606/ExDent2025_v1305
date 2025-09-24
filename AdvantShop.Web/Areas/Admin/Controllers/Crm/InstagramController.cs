using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.Services.Crm.Instagram;
using AdvantShop.Core.Services.Crm.SalesFunnels;
using AdvantShop.Core.Services.SalesChannels;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Models.Crm;
using AdvantShop.Web.Infrastructure.ActionResults;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Web.Admin.Controllers.Crm
{
    [Auth(RoleAction.Instagram)]
    [SaasFeature(Saas.ESaasProperty.Instagram)]
    [SalesChannel(ESalesChannelType.Instagram)]
    public partial class InstagramController : BaseAdminController
    {
        public ActionResult Index()
        {
            SetMetaInformation(T("Admin.Instagram.Index.Title"));

            return View();
        }

        #region Settings 

        public JsonResult GetSettings()
        {
            var isActive = Instagram.Instance.IsActive();

            return Json(new
            {
                isActive = isActive,
                login = SettingsInstagram.Login,
                password = SettingsInstagram.Password,
                userName = isActive ? SettingsInstagram.UserName : null,
                salesFunnels = SalesFunnelService.GetList(),
                salesFunnelId = SettingsCrm.DefaultInstagramSalesFunnelId,
                createLeadFromDirectMessages = SettingsInstagram.CreateLeadFromDirectMessages,
                createLeadFromComments = SettingsInstagram.CreateLeadFromComments,
            });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public ActionResult SaveLoginSettings(string login, string password)
        {
            if (!Instagram.Instance.IsActive() && SocialNetworkService.IntegrationsLimitRiched())
                return JsonError(T("Admin.SettingsCrm.SocialNetworks.LimitRiched"));

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
                return JsonError(T("Admin.Shared.EnterLoginAndPassword"));

            if (login[0] == '@')
                login = login.Substring(1, login.Length - 1);

            if (login.Contains("@"))
                login = login.Substring(0, login.IndexOf('@'));

            var authResult = Instagram.Instance.TryAuth(login, password);

            if (authResult is InstagramAuthChallengeRequiredResult)
            {
                var model = authResult as InstagramAuthChallengeRequiredResult;

                return Json(new CommandResult()
                {
                    Errors = new List<string>() {model.Error},
                    Obj = new { IsChallengeRequired = true, ApiPath = model.ApiPath}
                });
            }

            var result = authResult as InstagramAuthResult;
            if (result != null && !result.Result)
                return JsonError(T("Admin.Shared.CouldNotLogIn") + result.Error);
            

            Track.TrackService.TrackEvent(Track.ETrackEvent.Shop_Instagram_AccountConnected);

            return JsonOk();
        }
        

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeActivate()
        {
            Instagram.Instance.DeActivate();
            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SaveSettings(SaveInstagramSettingsModel model)
        {
            SettingsCrm.DefaultInstagramSalesFunnelId = model.Id;
            SettingsInstagram.CreateLeadFromDirectMessages = model.CreateLeadFromDirectMessages;
            SettingsInstagram.CreateLeadFromComments = model.CreateLeadFromComments;

            return JsonOk();
        }

        #endregion

        [HttpGet]
        public JsonResult GetCustomerMessages(Guid customerId)
        {
            if (!Instagram.Instance.IsActive())
                return Json(null);

            return Json(InstagramService.GetCustomerMessages(customerId));
        }


        [HttpPost, ValidateJsonAntiForgeryToken]
        public async Task<JsonResult> SendInstagramMessage(int? messageId, string message, string customerId)
        {
            var result = await Instagram.Instance.SendMessage(messageId ?? 0, customerId, message);
            return result ? JsonOk() : JsonError();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public async Task<JsonResult> AddInstagramUser(Guid customerId, string link)
        {
            try
            {
                var result = await Instagram.Instance.AddUserByLink(customerId, link);

                return result ? JsonOk() : JsonError();
            }
            catch (BlException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            return JsonError();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public async Task<JsonResult> RequireChallengeCode(string apiPath, int choiceMethod)
        {
            try
            {
                var result = await Instagram.Instance.RequireChallengeCode(apiPath, choiceMethod);
                return result ? JsonOk() : JsonError();
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                ModelState.AddModelError("", "Ошибка " + ex.Message);
            }
            return JsonError();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public async Task<JsonResult> SendChallengeCode(string apiPath, string code)
        {
            try
            {
                var result = await Instagram.Instance.SendChallengeCode(apiPath, code);
                return result ? JsonOk() : JsonError();
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                ModelState.AddModelError("", "Ошибка при отсылки кода " + ex.Message);
            }
            return JsonError();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteLink(Guid customerId)
        {
            var u = InstagramService.GetUserByCustomerId(customerId);
            if (u != null)
                InstagramService.DeleteUser(customerId);

            return JsonOk();
        }
    }
}
