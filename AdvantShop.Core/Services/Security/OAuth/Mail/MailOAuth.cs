//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Security.OAuth.Mail;
using AdvantShop.Security.OpenAuth;
using Newtonsoft.Json;

namespace AdvantShop.Security.OAuth
{
    // docs: https://oauth.mail.ru/docs

    public class MailOAuth
    {
        public static string OpenDialog(string pageToRedirect)
        {
            return OAuth.OpenAuthDialog("https://oauth.mail.ru/login",
                SettingsOAuth.MailAppId,
                pageToRedirect,
                "userinfo",
                "mail");
        }

        public static bool Login(string code, string pageToRedirect)
        {
            var tokenResponse = GetTokenPostRequest<MailOAuthToken>(
               "https://oauth.mail.ru/token",
               code,
               SettingsOAuth.MailAppId,
               SettingsOAuth.MailClientSecret);

            if (tokenResponse == null)
                return false;

            var user = GetUserData(tokenResponse.AccessToken);
            if (user == null)
                return false;

            var c = new Customer(CustomerGroupService.DefaultCustomerGroup)
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                EMail = user.Email,
                Password = Guid.NewGuid().ToString()
            };

            if (!string.IsNullOrEmpty(user.Birthday))
            {
                var dt = user.Birthday.TryParseDateTime();
                if (dt != DateTime.MinValue)
                    c.BirthDay = dt;
            }

            OAuthService.AuthOrRegCustomer(c, user.Id);

            return true;
        }

        private static MailUserData GetUserData(string token)
        {
            try
            {
                var str = new WebClient().DownloadString($"https://oauth.mail.ru/userinfo?access_token={token}");
                return JsonConvert.DeserializeObject<MailUserData>(str);
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }
            return null;
        }

        public static T GetTokenPostRequest<T>(string url, string code, string clientId, string clientSecret) where T : class, new()
        {
            var dataStr =
                string.Format(
                    "grant_type=authorization_code&code={0}&client_id={1}&client_secret={2}&redirect_uri={3}",
                    code,
                    clientId,
                    clientSecret,
                    StringHelper.ToPuny(UrlService.GetUrl("login")));

            var responseResult = "";

            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                var data = Encoding.UTF8.GetBytes(dataStr);

                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;
                request.KeepAlive = false;
                request.ServicePoint.ConnectionLimit = 1;
                
                request.Headers.Clear();
                var ua = HttpContext.Current != null && !string.IsNullOrEmpty(HttpContext.Current.Request.UserAgent)
                    ? HttpContext.Current.Request.UserAgent
                    : "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.159 Safari/537.36";
                request.UserAgent = ua;

                using (var stream = request.GetRequestStream())
                    stream.Write(data, 0, data.Length);

                var response = request.GetResponse();
                responseResult = new StreamReader(response.GetResponseStream()).ReadToEnd();
            }
            catch (WebException ex)
            {
                var result = "";

                using (var eResponse = ex.Response)
                {
                    if (eResponse != null)
                    {
                        using (var eStream = eResponse.GetResponseStream())
                        {
                            if (eStream != null)
                                using (var reader = new StreamReader(eStream))
                                {
                                    result = reader.ReadToEnd();
                                    Debug.Log.Error(result);
                                }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(
                    string.Format("InnerException: {0}; StackTrace: {1}; url: {2}; data: {3};",
                        ex.InnerException, ex.StackTrace, url, dataStr));
            }

            return responseResult != "" ? JsonConvert.DeserializeObject<T>(responseResult) : new T();
        }
    }
}
