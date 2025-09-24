using AdvantShop.Customers;
using AdvantShop.Security.OpenAuth;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace AdvantShop.Security.OAuth
{
    public class PassportAdvantService
    {
        protected static readonly HttpClient _httpClient = new HttpClient();

        public const string url = "https://passport.advantshop.net";

        public static async Task<OAuthUserInfo> GetUserData(string code, string state)
        {
            var urlprocees = string.Format(url + "/OAuth/GetUserData?code={0}&state={1}", code, state);
            HttpResponseMessage response = await _httpClient.GetAsync(urlprocees).ConfigureAwait(false);
            string jsonString = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<OAuthUserInfo>(jsonString);
            return result;
        }

        public static string GetLoginPage(string pageToRedirect, string advClientId)
        {
            return string.Format(url + "/OAuth/LoginShop/?pageToRedirect={0}&advClientId={1}", pageToRedirect, advClientId);
        }

        public static string GetLoginRequest(string pageToRedirect, string provider, string advClientId)
        {
            return string.Format(url + "/OAuth/LoginRequestShop/?pageToRedirect={0}&provider={1}&advClientId={2}", pageToRedirect, provider, advClientId);
        }

        public static async Task<bool> Login(string code, string state)
        {
            var userData = await GetUserData(code, state);
            if (userData == null) return false;

            OAuthService.AuthOrRegCustomer(new Customer(CustomerGroupService.DefaultCustomerGroup)
            {
                FirstName = userData.FirstName,
                LastName = userData.LastName,
                EMail = !string.IsNullOrEmpty(userData.Email) ? userData.Email : userData.UserDataId + "@temp.fb",
                Password = Guid.NewGuid().ToString()
            }, userData.UserDataId);

            return true;
        }
    }

    public class OAuthUserInfo
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string UserDataId { get; set; }
    }
}
