using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Core.Services.Crm.Ok;
using AdvantShop.Core.Services.Crm.Ok.OkMarket;
using AdvantShop.Core.Services.Crm.SalesFunnels;
using AdvantShop.Customers;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Shared.Ok;
using AdvantShop.Web.Admin.Models.Shared.Socials;
using AdvantShop.Web.Infrastructure.Filters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Core.Services.SalesChannels;
using AdvantShop.Diagnostics;

namespace AdvantShop.Web.Admin.Controllers.Crm
{
    [Auth(RoleAction.Ok)]
    [SaasFeature(Saas.ESaasProperty.Ok)]
    [SalesChannel(ESalesChannelType.Ok)]
    public class OkController : BaseAdminController
    {
        public ActionResult Index()
        {
            SetMetaInformation(T("Admin.Ok.Index.Title"));
            SetNgController(Infrastructure.Controllers.NgControllers.NgControllersTypes.OkMainCtrl);
            return View();
        }
        
        [HttpGet]
        public JsonResult GetOkSettings()
        {
            return Json(new
            {
                groupId = SettingsOk.GroupId,
                groupName = SettingsOk.GroupName,
                salesFunnels = SalesFunnelService.GetList(),
                salesFunnelId = SettingsCrm.DefaultOkSalesFunnelId,
                subscribeToMessages = SettingsOk.OkSubscribeToMessages
            });
        }
        
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ValidatePrimarySettings(string applicationPublicKey, string applicationAccessToken, string applicationSessionSecretKey, string groupSocialAccessToken)
        {
            if (!OkApiService.IsActive() && Core.Services.Crm.SocialNetworkService.IntegrationsLimitRiched())
                return JsonError(T("Admin.SettingsCrm.SocialNetworks.LimitRiched"));

            try
            {
                SettingsOk.ApplicationPublicKey = applicationPublicKey;
                SettingsOk.ApplicationAccessToken = applicationAccessToken;
                SettingsOk.ApplicationSessionSecretKey = applicationSessionSecretKey;
                SettingsOk.GroupSocialAccessToken = groupSocialAccessToken;

                var marketApiService = new OkMarketApiService();

                var subscribeResult = OkApiService.WebHookSubscribe();
                if (!subscribeResult)
                {
                    OkApiService.DeActivate();
                    marketApiService.DeActivate();
                    return JsonError("Неверный ключ доступа для работы с Bot API");
                }

                SettingsOk.OkSubscribeToMessages = true;

                var groupResponse = OkApiService.GetGroupInfo();
                if (groupResponse.ErrorMsg == null)
                {
                    SettingsOk.GroupId = groupResponse.GroupID.Split(':').Last();
                    SettingsOk.GroupName = groupResponse.Name;
                }
                else
                {
                    return JsonError(groupResponse.ErrorMsg);
                }

                var userGroupsResponse = marketApiService.GetUserGroups();
                if (!string.IsNullOrEmpty(userGroupsResponse.ErrorMsg))
                {
                    OkApiService.DeActivate();
                    marketApiService.DeActivate();
                    return JsonError(userGroupsResponse.ErrorMsg.Contains("application is not EXTERNAL", StringComparison.OrdinalIgnoreCase)
                        ? "Приложение не является внешним. Проверьте настойки приложения - добавлена ли платформа OAuth."
                        : "Неверные данные приложения");
                }
                var groups = marketApiService.GetGroupsInfo(new List<string> { SettingsOk.GroupId });
                if (groups.Count == 0)
                {
                    OkApiService.DeActivate();
                    marketApiService.DeActivate();
                    return JsonError("Не удалось получить данные о группе через приложение");
                }
                //test application permissions
                marketApiService = new OkMarketApiService();
                var testCatalogId = marketApiService.AddCatalog("Test");
                if (testCatalogId.HasValue)
                {
                    marketApiService.DeleteCatalog(testCatalogId.Value);
                }

                var testPhotoUrl = marketApiService.GetUploadUrl(out _);
                if (string.IsNullOrEmpty(testPhotoUrl) || !testCatalogId.HasValue)
                {
                    OkApiService.DeActivate();
                    marketApiService.DeActivate();
                    return JsonError("Не удалось настроить интеграцию: у приложения нет нужных разрешений");
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex.Message);
                return JsonError("Произошла ошибка");
            }

            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult RemoveBinding()
        {
            OkApiService.DeActivate();
            var marketApiService = new OkMarketApiService();
            marketApiService.DeActivate();
            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ChangeSaleFunnel(int id)
        {
            SettingsCrm.DefaultOkSalesFunnelId = id;
            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SendOkMessage(int id, string message, string customerId)
        {
            return ProcessJsonResult(() => OkApiService.SendOKMessage(id, message.Replace("\r\n", "<br>"), customerId.TryParseGuid()));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SendOkMessageByCustomers(SocialSendMessageModel model)
        {
            var sendedCount = new SendOkMessage(model).Execute();
            return JsonOk(sendedCount);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddOkUser(Guid customerId, string link)
        {
            try
            {
                var result = OkApiService.AddOkUserByLink(customerId, link);
                return result ? JsonOk() : JsonError();
            }
            catch (BlException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            return JsonError();
        }


        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteOkLink(Guid customerId)
        {
            var u = OkService.GetUser(customerId);
            if (u != null)
                OkService.DeleteUser(u.Id);
            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ToggleSubscriptionToMessages(bool subscribe)
        {
            SettingsOk.OkSubscribeToMessages = subscribe;
            
            var subscribeResult = subscribe ? OkApiService.WebHookSubscribe() : OkApiService.WebHookUnsubscribe();
            return subscribeResult
                ? JsonOk()
                : JsonError();
        }
    }

    [Obsolete("use controller in integration area")]
    public class OkWebHookController : Controller
    {
        [HttpPost]
        public JsonResult GetMessage()
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
