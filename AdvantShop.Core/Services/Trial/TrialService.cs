//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Diagnostics;
using AdvantShop.Saas;
using Newtonsoft.Json;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Mails;
using RestSharp;
using AdvantShop.Core.Caching;
using AdvantShop.Track.EventLog;

namespace AdvantShop.Trial
{
    public class TrialService
    {
        private const string Url = "http://modules.advantshop.net/";

        private const string UrlTrialInfo = "http://modules.advantshop.net/Trial/GetParams/{0}";
        private const string UrlTrialCss = "http://modules.advantshop.net/Trial/GetCss/{0}";
        private const string UrlTrialEvents = "http://modules.advantshop.net/Event/LogEvent?licKey={0}&eventName={1}&eventParams={2}";

        private const string UrlTrialCounter = "http://modules.advantshop.net/Trial/GetCounter/{0}";
        private const string UrlHelpCounter = "http://modules.advantshop.net/Lic/GetHelpCounter/{0}";

        private static DateTime _lastUpdate;

        private static DateTime _trialTillCached = DateTime.MinValue;

        public static bool IsTrialEnabled => ModeConfigService.IsModeEnabled(ModeConfigService.Modes.TrialMode);

        public static int LeftDay => (TrialPeriodTill - DateTime.Now).Days + 1;

        public static string LeftDayString =>
            LeftDay + " " + Strings.Numerals(LeftDay,
                LocalizationService.GetResource("AdvantShop.Trial.TrialService.LeftDay0"),
                LocalizationService.GetResource("AdvantShop.TrialTrialService.LeftDay1"),
                LocalizationService.GetResource("AdvantShop.TrialTrialService.LeftDay2"),
                LocalizationService.GetResource("AdvantShop.TrialTrialService.LeftDay5"));

        public static DateTime TrialPeriodTill
        {
            get
            {
                if (DateTime.Now <= _lastUpdate.AddHours(1)) return _trialTillCached;
                
                try
                {
                    var request = WebRequest.Create(string.Format(UrlTrialInfo, SettingsLic.LicKey));
                    request.Method = "GET";

                    using (var dataStream = request.GetResponse().GetResponseStream())
                    {
                        using (var reader = new StreamReader(dataStream))
                        {
                            var responseFromServer = reader.ReadToEnd();

                            if (!string.IsNullOrEmpty(responseFromServer))
                            {
                                _trialTillCached = JsonConvert.DeserializeObject<DateTime>(responseFromServer);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log.Error(ex);
                }


                try
                {
                    var request = WebRequest.Create(string.Format(UrlTrialCss, SettingsLic.LicKey));
                    request.Method = "GET";

                    using (var dataStream = request.GetResponse().GetResponseStream())
                    {
                        using (var reader = new StreamReader(dataStream))
                        {
                            var responseFromServer = reader.ReadToEnd();
                            FilePath.FoldersHelper.SaveCSS(responseFromServer, FilePath.CssType.saas);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log.Error(ex);
                }

                _lastUpdate = DateTime.Now;
                return _trialTillCached;
            }
        }

        public static string TrialCounter
        {
            get
            {
                var counter = CacheManager.Get("trialCounter", 60 * 24, () =>
                {
                    var data = "";
                    try
                    {
                        var request = WebRequest.Create(string.Format(UrlTrialCounter, SettingsLic.LicKey));
                        request.Method = "GET";

                        using (var dataStream = request.GetResponse().GetResponseStream())
                        {
                            using (var reader = new StreamReader(dataStream))
                            {
                                var responseFromServer = reader.ReadToEnd();
                                if (!string.IsNullOrEmpty(responseFromServer))
                                {
                                    data = JsonConvert.DeserializeObject<KeyValuePair<DateTime, string>>(responseFromServer).Value;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.Log.Error(ex);
                    }
                    return data;
                });

                return counter;

            }
        }

        // ReSharper disable once UnusedMember.Global
        // used in views
        public static string HelpCounter
        {
            get
            {
                if (IsTrialEnabled) // для триала свой счетчик, этот не выводим, чтобы не было конфликта
                {
                    return "";
                }

                string counter = "";

                counter = CacheManager.Get<string>("helpCounter", 60 * 24, () =>
                {
                    string data = "";
                    try
                    {
                        var request = WebRequest.Create(string.Format(UrlHelpCounter, SettingsLic.LicKey));
                        request.Method = "GET";

                        using (var dataStream = request.GetResponse().GetResponseStream())
                        {
                            using (var reader = new StreamReader(dataStream))
                            {
                                var responseFromServer = reader.ReadToEnd();
                                if (!string.IsNullOrEmpty(responseFromServer))
                                {
                                    data = JsonConvert.DeserializeObject<KeyValuePair<DateTime, string>>(responseFromServer).Value;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.Log.Error(ex);
                    }
                    return data;
                });

                return counter;

            }
        }

        public static void TrackEvent(TrialEvents trialEvent, string eventParams)
        {
            if (!IsTrialEnabled && !SaasDataService.IsSaasEnabled)
                return;

            if (TrackEventLogService.IsEventLimitedPerHour(trialEvent.ToString()))
                return;

            var context = HttpContext.Current;
            try
            {
                new WebClient().DownloadString(string.Format(UrlTrialEvents, SettingsLic.LicKey, trialEvent.ToString(),
                    HttpUtility.UrlEncode(eventParams)));
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            TrackEventLogService.AddEventLog(trialEvent.ToString(), context);

            if (context != null)
            {
                if (context.Items["TrialEvents"] == null)
                    context.Items["TrialEvents"] = new List<KeyValuePair<TrialEvents, string>>();

                ((List<KeyValuePair<TrialEvents, string>>)context.Items["TrialEvents"]).Add(
                    new KeyValuePair<TrialEvents, string>(trialEvent, eventParams));
            }
        }

        public static void SendMessage(string to, MailTemplate tpl)
        {
            try
            {
                var client = new RestClient(Url);
                var request = new RestRequest($"Trial/SendMail/{SettingsLic.LicKey}", Method.POST) { Timeout = 3000 };
                request.AddJsonBody(new
                {
                    to,
                    subject = tpl.Subject,
                    body = tpl.Body
                });

                client.Execute(request);
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }
        }

        // ReSharper disable once UnusedMember.Global
        // used in admin views
        public static bool HasDemoProducts()
        {
            return ProductService.GetProductsCount("isdemo = 1") > 0;
        }

        public static void TrackVisitEvent()
        {
            if (HttpContext.Current?.Request != null && HttpContext.Current.Request.IsTechnicalHeader())
                return;

            var visitDate = SettingsTracking.AdminAreaVisitDate;
            // один раз в день
            if (visitDate.HasValue && visitDate.Value.Date == DateTime.Now.Date)
                return;

            SettingsTracking.AdminAreaVisitDate = DateTime.Now;
            // для всех сайтов, не только триалов
            Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Common_FirstVisitAdminAreaOfDay);

            if (!IsTrialEnabled || LeftDay < 0)
                return;

            if (!SettingsTracking.TrialCreatedDate.HasValue)
                SettingsTracking.TrialCreatedDate = DateTime.Now.Date;

            var day = (DateTime.Now.Date - SettingsTracking.TrialCreatedDate.Value.Date).TotalDays + 1;
            if (day > 0 && day <= 14)
                Track.TrackService.TrackEvent(Track.ETrackEvent.Trial_DailyVisit, day.ToString());
        }
    }
}