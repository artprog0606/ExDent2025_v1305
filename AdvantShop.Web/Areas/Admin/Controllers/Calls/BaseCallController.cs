using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Crm.BusinessProcesses;
using AdvantShop.Core.Services.IPTelephony;
using AdvantShop.Diagnostics;
using AdvantShop.Web.Admin.Handlers.IPTelephony;
using AdvantShop.Web.Admin.Handlers.Shared.AdminNotifications;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Extensions;

namespace AdvantShop.Web.Admin.Controllers.Calls
{
    public class BaseCallController : BaseController
    {
        protected static void SendIncomingCallNotify(Call call)
        {
            var cacheName = string.Format("Telephony_NotifySent_{0}", call.CallId);
            if (CacheManager.Get<bool>(cacheName))
                return;

            var handler = new IPTelephonyHandler(call);
            var notificationsHandler = new AdminNotificationsHandler();
            foreach (var notice in handler.GetWebNotifications())
            {
                notificationsHandler.NotifyAllCustomers(notice);
            }
            foreach (var notice in handler.GetToasterNotifications())
            {
                notificationsHandler.NotifyByToaster(notice);
            }

            CacheManager.Insert(cacheName, true, 0.1);  // 6 seconds
        }

        protected static void SendCallEndedNotify(Call call)
        {
            var notificationsHandler = new AdminNotificationsHandler();
            notificationsHandler.CloseToaster(call.CallId);
        }

        protected void ProcessMissedCall(Call call)
        {
            var cacheName = $"Telephony_CallProcessed_{call.CallId}";
            if (CacheManager.Get<bool>(cacheName))
                return;

            BizProcessExecuter.CallMissed(call);
            var emailMissingCall = SettingsMail.EmailForMissedCall;
            if (emailMissingCall.IsNotEmpty())
            {
                var mail = new Mails.MissedCallMailTemplate(call.SrcNum);
                Core.Services.Mails.MailService.SendMailNow(emailMissingCall, mail);
            }
            CacheManager.Insert(cacheName, true, 0.1);  // 6 seconds
        }

        protected void LogNotification()
        {
            if (SettingsTelephony.LogNotifications && HttpContext != null && HttpContext.Request != null)
                Debug.Log.Info(HttpContext.Request.GetRequestRawData());
        }
    }
}