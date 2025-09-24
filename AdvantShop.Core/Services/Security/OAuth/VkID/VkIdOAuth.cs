//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;

using System.IO;
using System.Net;
using System.Text;
using System.Web;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Crm.Vk;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Security.OpenAuth;
using Newtonsoft.Json;

namespace AdvantShop.Security.OAuth
{
    // docs: https://id.vk.com/about/business/go/docs/ru/vkid/latest/vk-id/connection/api-integration/api-description
    
    public sealed class VkIdOAuth
    {
        private const string CodeChallengeCookie = "v_cf";

        private string ClientId => SettingsOAuth.VkIdClientId;
        private string RedirectUri => HttpUtility.UrlEncode(StringHelper.ToPuny(UrlService.GetUrl("auth/vkid")));
        
        public string OpenDialog(string pageToRedirect)
        {
            var state = CustomerContext.CurrentCustomer.Id + "_" + pageToRedirect;
            
            var codeVerifier = Guid.NewGuid().ToString();
            var codeChallenge = codeVerifier.Sha256AndToBase64UrlEncode();
            
            CommonHelper.SetCookie(CodeChallengeCookie, codeVerifier, true);
            
            return 
                string.Format("https://id.vk.com/authorize?response_type=code&client_id={0}&redirect_uri={1}&scope={2}&state={3}&code_challenge={4}&code_challenge_method=S256",
                    ClientId,
                    RedirectUri,
                    "email phone",
                    state,
                    codeChallenge);
        }

        public bool Login()
        {
            try
            {
                var request = HttpContext.Current.Request;
                
                var exchangeCode = new ExchangeCode()
                {
                    code = request.QueryString["code"],
                    client_id = ClientId,
                    code_verifier = CommonHelper.GetCookieString(CodeChallengeCookie),
                    state = request.QueryString["state"],
                    device_id = request.QueryString["device_id"],
                    redirect_uri = RedirectUri,
                };
                
                var result = new VkApiService().GetAccessToken(exchangeCode);
                
                if (!result.IsSuccess)
                    return false;
                
                var userInfo = GetUserInfo(result.Value.access_token, ClientId);
                if (userInfo == null || userInfo.User == null)
                    return false;

                var user = userInfo.User;

                var customer = new Customer(CustomerGroupService.DefaultCustomerGroup)
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    EMail =
                        !string.IsNullOrEmpty(user.Email)
                            ? user.Email
                            : user.Id + "@temp.vk",
                    Phone = user.Phone,
                    StandardPhone = StringHelper.ConvertToStandardPhone(user.Phone),
                    BirthDay =
                        user.Birthday.IsNotEmpty() && DateTime.TryParse(user.Birthday, out var birthday)
                            ? birthday
                            : default(DateTime?),
                    Password = Guid.NewGuid().ToString()
                };
                
                OAuthService.AuthOrRegCustomer(customer, user.Id);
                
                CommonHelper.DeleteCookie(CodeChallengeCookie);
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                return false;
            }

            return true;
        }

        private static VkUserInfoResponse GetUserInfo(string accessToken, string clientId)
        {
            // docs: https://id.vk.com/about/business/go/docs/ru/vkid/latest/vk-id/connection/api-integration/api-description#Poluchenie-nemaskirovannyh-dannyh

            var request = WebRequest.Create("https://id.vk.com/oauth2/user_info");
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            
            var data = $"client_id={clientId}&access_token={accessToken}";
            
            var bytes = Encoding.UTF8.GetBytes(data);
            request.ContentLength = bytes.Length;

            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();
            }

            var responseContent = "";
            
            using (var response = request.GetResponse())
                using (var stream = response.GetResponseStream())
                    if (stream != null)
                        using (var reader = new StreamReader(stream))
                            responseContent = reader.ReadToEnd();

            if (responseContent.IsNotEmpty() && responseContent.Contains("error"))
            {
                Debug.Log.Error("VkIDOAuth.GetUserInfo " + responseContent);
            }

            return JsonConvert.DeserializeObject<VkUserInfoResponse>(responseContent);
        }
    }
}
