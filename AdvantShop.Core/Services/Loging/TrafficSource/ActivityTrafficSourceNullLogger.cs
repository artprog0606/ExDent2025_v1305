using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Helpers;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using Newtonsoft.Json;

namespace AdvantShop.Core.Services.Loging.TrafficSource
{
    public class ActivityTrafficSourceNullLogger : ITrafficSourceLogger
    {
        private const string TrafficSourceCookieName = "advs";

        public virtual void LogTrafficSource()
        {
            var request = HttpContext.Current.Request;
            var referrer = request.GetUrlReferrer();

            // Логируем источник только если реферрер либо пустой, либо не является страницей сайта (то есть переход с внешнего источника)
            if (referrer == null ||
                referrer.Host.IndexOf(CommonHelper.GetParentDomain(), StringComparison.InvariantCultureIgnoreCase) == -1)
            {
                if (referrer != null || string.IsNullOrEmpty(CommonHelper.GetCookieString(TrafficSourceCookieName)))
                {
                    var source = GetSource(referrer, request);

                    source.Hash = GetHash(source);
                    source.CustomerId = null;
                    source.ShopId = null;


                    CommonHelper.SetCookie(TrafficSourceCookieName, JsonConvert.SerializeObject(source), new TimeSpan(30, 0, 0, 0), httpOnly: false, setOnMainDomain: true);
                }
            }
        }

        public virtual TrafficSource GetTrafficSourceByRule(Guid customerId)
        {
            var cookieValue = CommonHelper.GetCookieString(TrafficSourceCookieName);
            if (string.IsNullOrWhiteSpace(cookieValue))
                return null;

            var source = JsonConvert.DeserializeObject<TrafficSource>(HttpUtility.UrlDecode(cookieValue));

            source.ShopId = SettingsLic.LicKey;
            source.CustomerId = customerId;
            source.IP = HttpContext.Current.TryGetIp();

            if (string.IsNullOrEmpty(source.Hash) || source.Hash != GetHash(source))
            {
                Debug.Log.Warn("traffic source hash is wrong. hash = " + source.Hash + ", right hash = " + GetHash(source) + " cookie value: " + cookieValue);
                return null;
            }

            return source;
        }

        public virtual void ClearTrafficSources(Guid customerId)
        {
            CommonHelper.DeleteCookie(TrafficSourceCookieName);
        }

        public void LogOrderTafficSource(int objId, TrafficSourceType type, bool isFromAdminArea)
        {
            if (objId == 0)
                return;

            if (isFromAdminArea)
                return;

            var context = HttpContext.Current;
            if (context == null)
                return;

            var customerId = CustomerContext.CurrentCustomer.Id;
            
            Task.Run(() =>
            {
                try
                {
                    HttpContext.Current = context;
                    var source = GetTrafficSourceByRule(customerId);
                    if (source != null)
                        OrderTrafficSourceService.Add(objId, type, source);
                }
                catch (Exception ex)
                {
                    Debug.Log.Error(ex.Message, ex);
                }

                if (type != TrafficSourceType.Lead)
                {
                    ClearTrafficSources(customerId);
                }
            });
        }

        public List<TrafficSource> GetTrafficSources(Guid customerId)
        {
            throw new NotImplementedException();
        }

        public TrafficSource GetSource(Uri referrer, HttpRequest request)
        {
            var source = new TrafficSource()
            {
                ShopId = SettingsLic.LicKey,
                CustomerId = CustomerContext.CustomerId,

                CreateOn = DateTime.Now,
                Referrer = referrer != null ? referrer.ToString() : null,
                Url =
                    (UrlService.IsSecureConnection(request) ? "https://" : "http://") + request.Url.Authority +
                    request.RawUrl,

                utm_source = HttpUtility.HtmlEncode(request.QueryString["utm_source"]),
                utm_medium = HttpUtility.HtmlEncode(request.QueryString["utm_medium"]),
                utm_campaign = HttpUtility.HtmlEncode(request.QueryString["utm_campaign"]),
                utm_content = HttpUtility.HtmlEncode(request.QueryString["utm_content"]),
                utm_term = HttpUtility.HtmlEncode(request.QueryString["utm_term"]),
                IP = request.UserHostAddress
            };

            return source;
        }

        public string GetHash(TrafficSource source)
        {
            return ("adv" + source.ShopId + ":" + source.Referrer + ":" + source.Url + ":" +
                    source.utm_source + ":" + source.utm_medium + ":" + source.utm_campaign + ":" +
                    source.utm_content + ":" + source.utm_term).Md5(false);
        }
    }
}