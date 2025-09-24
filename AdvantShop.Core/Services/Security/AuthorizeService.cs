//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Web;
using System.Web.Security;
using AdvantShop.Configuration;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Core.Services.Customers;
using AdvantShop.Core.Services.Partners;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;

namespace AdvantShop.Security
{
    public class AuthorizeService
    {
        private const string Splitter = ":";

        public static Customer GetAuthenticatedCustomer()
        {
            if (HttpContext.Current == null) return null;

            var formsCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (formsCookie != null)
            {
                try
                {
                    var formsAuthenticationTicket = FormsAuthentication.Decrypt(formsCookie.Value);
                    if (formsAuthenticationTicket != null)
                    {
                        var token = formsAuthenticationTicket.Name;
                        var words = token.Split(new[] { Splitter }, StringSplitOptions.RemoveEmptyEntries);
                        if (words.Length != 2) return null;
                        var isParsedToGuid = Guid.TryParse(words[0], out var customerId);
                        var passHash = words[1];

                        if (isParsedToGuid)
                            return customerId == Guid.Empty
                                ? null
                                : CustomerService.GetCustomerByIdAndPassword(customerId, passHash, true);

                        var email = words[0];
                        return string.IsNullOrWhiteSpace(email)
                            ? null
                            : CustomerService.GetCustomerByEmailAndPassword(email, passHash, true);
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log.Error(ex);
                }
            }

            return null;
        }

        public static bool SignIn(string email, string password, bool isHash, bool createPersistentCookie,
            out Customer customer, string adminFcmToken = null)
        {
            customer = null;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                return false;

            var isDebug = Secure.IsDebugAccount(email, password);
            if (isDebug)
            {
                SignInDebug(out customer);
                return true;
            }

            customer = CustomerService.GetCustomerByEmailAndPassword(email, password, isHash);
            if (customer == null || !customer.Enabled)
                return false;

            SignInCore(customer, createPersistentCookie);

            if (!string.IsNullOrWhiteSpace(adminFcmToken))
                CustomerAdminPushNotificationService.UpdateFcmToken(customer.Id, adminFcmToken);

            return true;
        }

        public static bool SignIn(string email, string password, bool isHash, bool createPersistentCookie, string adminFcmToken = null)
        {
            return SignIn(email, password, isHash, createPersistentCookie, out _, adminFcmToken);
        }

        public static bool SignInByPhone(long? phone, string password, bool isHash, bool createPersistentCookie,
            out Customer customer)
        {
            customer = null;

            if (!phone.HasValue || phone.Value == 0 || string.IsNullOrEmpty(password))
                return false;

            customer = CustomerService.GetCustomerByStandardPhoneAndPassword(phone.Value, password, isHash);
            if (customer == null)
                return false;

            SignInCore(customer, createPersistentCookie);

            return true;
        }

        public static bool SignInByPhone(long? phone, string password, bool isHash, bool createPersistentCookie)
        {
            return SignInByPhone(phone, password, isHash, createPersistentCookie, out _);
        }

        public static bool IsTwoFactorAuth(string email, string password)
        {
            var moduleId = SettingsManager.ActiveTwoFactorAuthModule;
            if (string.IsNullOrEmpty(moduleId) || moduleId == "-1")
                return false;
            var module = AttachedModules.GetModuleById(moduleId);
            if(module == null)
                return false;
            var customer = CustomerService.GetCustomerByEmailAndPassword(email, password, false);
            var moduleInstance = (ITwoFactorAuthentication)Activator.CreateInstance(module, null);
            if (customer != null && (customer.IsAdmin || customer.IsModerator)) return moduleInstance.HasUserEnabledAuthentication(customer.Id);

            return false;
        }
        
        public static bool IsTwoFactorAuthCodeValid(string email, string password, string code)
        {
            var moduleId = SettingsManager.ActiveTwoFactorAuthModule;
            if (string.IsNullOrEmpty(moduleId) || moduleId == "-1")
                return false;
            var module = AttachedModules.GetModuleById(moduleId);
            if(module == null)
                return false;
            var customer = CustomerService.GetCustomerByEmailAndPassword(email, password, false);
            var moduleInstance = (ITwoFactorAuthentication)Activator.CreateInstance(module, null);
            var key = moduleInstance.GetCodes(customer.Id, email).SecretKey;
            return moduleInstance.CheckCodeValid(key, code);

        }

        private static void SignInCore(Customer customer, bool createPersistentCookie)
        {
            var oldCustomerId = CustomerContext.CurrentCustomer.Id;

            Secure.AddUserLog(customer.EMail, true, customer.IsAdmin);
            ShoppingCartService.MergeShoppingCarts(oldCustomerId, customer.Id);
            CustomerContext.SetCustomerCookie(customer.Id);
            FormsAuthentication.SetAuthCookie(customer.Id + Splitter + customer.Password, createPersistentCookie);

            var domainSet = false;
            if (SettingsMain.IsTechDomainsReady &&
                !string.IsNullOrWhiteSpace(HttpContext.Current.Request.Headers["X-Forwarded-Host"]))
            {
                var formsCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
                if (formsCookie != null)
                {
                    var forwardedHostHeader = HttpContext.Current.Request.Headers["X-Forwarded-Host"].TrimEnd('/');
                    var forwardedHostPathHeader =
                        HttpContext.Current.Request.Headers["X-Forwarded-Host-Path"].TrimStart('/');
                    var forwardedProtoSecureHeader = HttpContext.Current.Request.Headers["X-Forwarded-Proto-Secure"];
            
                    if (!string.IsNullOrWhiteSpace(forwardedHostHeader))
                    {
                        formsCookie.Domain = "." + forwardedHostHeader;
                        domainSet = true;
                    }
            
                    if (!string.IsNullOrWhiteSpace(forwardedHostPathHeader))
                        formsCookie.Path = "/" + forwardedHostPathHeader;
            
                    if (forwardedProtoSecureHeader == "1")
                    {
                        formsCookie.SameSite = SameSiteMode.None;
                        formsCookie.Secure = true;
                    }
                }
            }
            
            if (!domainSet && SettingsMain.SetCookieOnMainDomain)
            {
                var formsCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
                if (formsCookie != null)
                    formsCookie.Domain = "." + SettingsMain.SiteUrlPlain;
            }
        }

        private static void SignInDebug(out Customer customer)
        {
            CustomerContext.IsDebug = true;

            customer = new Customer
            {
                CustomerRole = Role.Administrator,
                IsVirtual = true,
                Enabled = true
            };
            HttpContext.Current.Items["CustomerContext"] = customer;

            CustomerContext.SetDontDisturbByNotifyCookie(TimeSpan.FromDays(1));

            Secure.AddUserLog("sa", true, true);
        }

        public static void SignOut()
        {
            CustomerContext.IsDebug = false;
            CustomerContext.DeleteCustomerCookie();

            //удаляем куку после выхода из аккаунта чтобы не привязывались другие пользователи
            PartnerService.ClearReferralCookie();

            FormsAuthentication.SignOut();
            
            var domainSet = false;
            if (SettingsMain.IsTechDomainsReady &&
                !string.IsNullOrWhiteSpace(HttpContext.Current.Request.Headers["X-Forwarded-Host"]))
            {
                var formsCookie = HttpContext.Current.Response.Cookies[FormsAuthentication.FormsCookieName];
                if (formsCookie != null)
                {
                    var forwardedHostHeader = HttpContext.Current.Request.Headers["X-Forwarded-Host"].TrimEnd('/');
                    var forwardedHostPathHeader =
                        HttpContext.Current.Request.Headers["X-Forwarded-Host-Path"].TrimStart('/');
                    var forwardedProtoSecureHeader = HttpContext.Current.Request.Headers["X-Forwarded-Proto-Secure"];
            
                    if (!string.IsNullOrWhiteSpace(forwardedHostHeader))
                    {
                        formsCookie.Domain = "." + forwardedHostHeader;
                        domainSet = true;
                    }
            
                    if (!string.IsNullOrWhiteSpace(forwardedHostPathHeader))
                        formsCookie.Path = "/" + forwardedHostPathHeader;
            
                    if (forwardedProtoSecureHeader == "1")
                    {
                        formsCookie.SameSite = SameSiteMode.None;
                        formsCookie.Secure = true;
                    }
                    
                    HttpContext.Current.Response.Cookies.Add(formsCookie);
                }
            }
            
            if (!domainSet && SettingsMain.SetCookieOnMainDomain)
            {
                var formsCookie = HttpContext.Current.Response.Cookies[FormsAuthentication.FormsCookieName];
                if (formsCookie != null)
                {
                    formsCookie.Domain = "." + SettingsMain.SiteUrlPlain;
                    
                    HttpContext.Current.Response.Cookies.Add(formsCookie);
                }
            }
        }
    }
}