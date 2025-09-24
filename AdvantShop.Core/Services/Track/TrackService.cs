using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Helpers;
using AdvantShop.Core.Services.SalesChannels;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Track.EventLog;

namespace AdvantShop.Track
{
    public class TrackService
    {
        private const string Url = "http://modules.advantshop.net/";

        public static void TrackEvent(ETrackEvent @event, string eventKeyPostfix = null, bool dontLogBotsUsers = false)
        {
            if (dontLogBotsUsers && BrowsersHelper.IsBot())
            {
                return;
            }

            var (sendOnce, onceDaily, eventKey) = ParseEventArgs(@event, eventKeyPostfix);
            if (eventKey.IsNullOrEmpty())
            {
                return;
            }

            var currentCustomer = Customers.CustomerContext.CurrentCustomer;
            if ((currentCustomer != null && currentCustomer.IsVirtual)
                || @event == ETrackEvent.None
                || (sendOnce && TrackEventIsCommitted(@event))
                || TrackEventLogService.IsEventLimitedPerHour(eventKey)
                || (onceDaily && TrackEventLogService.IsEventWasToday(eventKey)))
            {
                return;
            }

            SendEvent(eventKey);
            TrackEventLogService.AddEventLog(eventKey, HttpContext.Current);

            if (sendOnce)
            {
                SetTrackEventCommitted(eventKey);
            }
        }

        public static void TrackSalesChannel(ESalesChannelType channelType, bool active)
        {
            try
            {
                RequestHelper.MakeRequest<string>($"{Url}shop/setsaleschannel/{SettingsLic.LicKey}",
                    data: $"name={channelType}&active={active}",
                    method: ERequestMethod.POST,
                    contentType: ERequestContentType.FormUrlencoded,
                    timeoutSeconds: 1);
            }
            catch (Exception ex)
            {
                Debug.Log.Warn(ex);
            }
        }

        public static bool TrackEventIsCommitted(ETrackEvent @event)
        {
            // старый код, переход от куки к базе
            //var cookieValue = HttpContext.Current != null && HttpContext.Current.Request != null ? CommonHelper.GetCookieString("committedEvents") : string.Empty;
            //if (!SettingsTracking.TrackedEvents.Any() && cookieValue.IsNotEmpty())
            //{
            //    var cookieEvents = cookieValue.Split(new[] {'.'}, StringSplitOptions.RemoveEmptyEntries).Distinct().ToList();
            //    SettingsTracking.TrackedEvents = cookieEvents;
            //    CommonHelper.DeleteCookie("committedEvents");
            //}
            return TrackEventIsCommitted(@event.ToString());
        }

        public static bool TrackEventIsCommitted(string eventKey)
        {
            return SettingsTracking.TrackedEvents.Any(x => x == eventKey);
        }

        private static void SetTrackEventCommitted(string eventKey)
        {
            var trackedEvents = SettingsTracking.TrackedEvents;
            trackedEvents.Add(eventKey);
            SettingsTracking.TrackedEvents = trackedEvents;
        }

        private static (bool sendOnce, bool onceDaily, string eventKey) ParseEventArgs(ETrackEvent @event, string eventKeyPostfix)
        {
            var eventAttr = @event.GetAttribute<TrackEventAttribute>();

            string eventKey = null;
            var sendOnce = false;
            var onceDaily = false;
            string prefix = null;
            var delimiter = ".";

            if (eventAttr != null)
            {
                if (eventAttr.ShopMode.HasValue && !ModeConfigService.IsModeEnabled(eventAttr.ShopMode.Value))
                {
                    return (false, false, null);
                }

                eventKey = eventAttr.EventKey;
                sendOnce = eventAttr.SendOnce;
                //onceDaily = eventAttr.OnceDaily;
                if (eventAttr.Delimiter.IsNotEmpty())
                    delimiter = eventAttr.Delimiter;
                prefix = Trial.TrialService.IsTrialEnabled ? eventAttr.TrialPrefix : null;
            }

            if (eventKey.IsNullOrEmpty())
            {
                eventKey = @event.ToString();
            }

            eventKey = string.Join(delimiter, new List<string>
                {
                    prefix,
                    eventKey,
                    eventKeyPostfix
                }
                .Where(x => x.IsNotEmpty()));

            return (sendOnce, onceDaily, eventKey);
        }

        private static void SendEvent(string eventKey)
        {
            var url = $"{Url}Event/TrackEvent?licKey={SettingsLic.LicKey}&eventName={eventKey}";
            if (HttpContext.Current != null && HttpContext.Current?.Request != null &&
                HttpContext.Current.Request.UserHostAddress.IsNotEmpty())
                url += "&ip=" + HttpUtility.UrlEncode(HttpContext.Current.Request.UserHostAddress);

            try
            {
                var request = WebRequest.Create(url);
                request.Method = "GET";
                request.Timeout = 1000;
                _ = request.GetResponse();
            }
            catch (Exception ex)
            {
                Debug.Log.Warn(ex);
            }
        }
    }
}