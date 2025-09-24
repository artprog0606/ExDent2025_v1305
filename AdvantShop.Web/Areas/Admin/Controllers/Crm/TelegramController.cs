using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Core.Services.Crm.SalesFunnels;
using AdvantShop.Core.Services.Crm.Telegram;
using AdvantShop.Core.Services.SalesChannels;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Web.Admin.Controllers.Crm
{
    [Auth(RoleAction.Telegram)]
    [SaasFeature(Saas.ESaasProperty.Telegram)]
    [SalesChannel(ESalesChannelType.Telegram)]
    public partial class TelegramController : BaseAdminController
    {
        private readonly TelegramApiService _apiService = new TelegramApiService();

        public ActionResult Index()
        {
            SetMetaInformation(T("Admin.Telegram.Index.Title"));

            return View();
        }

        public JsonResult GetSettings()
        {
            return Json(new
            {
                isActive = _apiService.IsActive(),
                token = SettingsTelegram.Token,
                botUser = SettingsTelegram.BotUser,
                salesFunnels = SalesFunnelService.GetList(),
                salesFunnelId = SettingsCrm.DefaultTelegramSalesFunnelId,
            });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SaveSettings(string token)
        {
            if (!_apiService.IsActive() && Core.Services.Crm.SocialNetworkService.IntegrationsLimitRiched())
                return JsonError(T("Admin.SettingsCrm.SocialNetworks.LimitRiched"));

            _apiService.DeActivate();

            SettingsTelegram.Token = token.DefaultOrEmpty();

            try
            {
                var user = _apiService.GetMe();
                if (user != null)
                    Track.TrackService.TrackEvent(Track.ETrackEvent.Shop_Telegram_BotConnected);
            }
            catch (Exception ex)
            {
                SettingsTelegram.Token = "";

                if (ex.Message.Contains("Invalid token format"))
                    return JsonError("Проверьте токен");

                if (ex.InnerException != null && ex.InnerException.Message != null && ex.InnerException.Message.Contains("HTTPS url must be provided"))
                    return JsonError("Для работы с Telegram необходим HTTPS");

                Debug.Log.Warn(ex);

                return JsonError("Не удалось сохранить токен");
            }

            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ChangeSaleFunnel(int id)
        {
            SettingsCrm.DefaultTelegramSalesFunnelId = id;
            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeActivate()
        {
            _apiService.DeActivate();
            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SendTelegramMessage(int? id, string message, string customerId, List<Guid> customerIds)
        {
            if (customerIds == null)
            {
                var result = _apiService.SendTelegramMessage(id ?? 0, message, customerId.TryParseGuid(true));
                return result ? JsonOk() : JsonError("Не удалось отправить сообщение");
            }

            var count = 0;

            foreach (var cid in customerIds)
            {
                var result = _apiService.SendTelegramMessage(id ?? 0, message, cid);
                count += Convert.ToInt32(result);
            }

            return count == customerIds.Count ? JsonOk() : JsonError("Сообщение отправлено с ошибками");
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteLink(Guid customerId)
        {
            new TelegramService().DeleteUser(customerId);
            return JsonOk();
        }
    }

    [Obsolete("use controller in integration area")]
    public class TelegramWebHookController : Controller
    {
        private readonly TelegramApiService _apiService = new TelegramApiService();

        [Obsolete("use action in integration area")]
        [HttpPost]
        public JsonResult Get()
        {
            var json = new StreamReader(HttpContext.Request.InputStream).ReadToEnd();

            _apiService.SaveMessage(json);

            return Json("ok");
        }
    }
}
