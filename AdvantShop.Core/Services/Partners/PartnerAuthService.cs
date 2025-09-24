using System;
using System.Web;
using System.Web.Security;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;

namespace AdvantShop.Core.Services.Partners
{
    public class PartnerAuthService
    {
        private const string Spliter = "::";
        private const string AuthCookieName = "Advantshop.Partner.AUTH";

        public static Partner GetAuthenticatedPartner()
        {
            if (HttpContext.Current == null)
                return null;

            var formsCookie = HttpContext.Current.Request.Cookies[AuthCookieName];
            if (formsCookie != null)
            {
                try
                {
                    var authTicket = FormsAuthentication.Decrypt(formsCookie.Value);
                    if (authTicket != null)
                    {
                        var token = authTicket.Name;
                        var words = token.Split(new[] { Spliter }, StringSplitOptions.RemoveEmptyEntries);
                        if (words.Length != 2) return null;
                        var email = words[0];
                        var passHash = words[1];
                        return string.IsNullOrEmpty(email)
                            ? null
                            : PartnerService.GetPartner(email, passHash, true);
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log.Error(ex);
                }
            }
            return null;
        }

        public static bool SignIn(string email, string password, bool isHash, out Partner partner)
        {
            partner = null;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                return false;

            partner = PartnerService.GetPartner(email, password, isHash);
            if (partner == null)
                return false;

            FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(1, email + Spliter + partner.Password, DateTime.Now, DateTime.Now.AddMonths(3), true, string.Empty, AuthCookieName);
            var ticketEncrypted = FormsAuthentication.Encrypt(authTicket);

            CommonHelper.SetCookie(AuthCookieName, ticketEncrypted, httpOnly: true);

            return true;
        }

        public static bool SignIn(string email, string password, bool isHash)
        {
            Partner partner;
            return SignIn(email, password, isHash, out partner);
        }

        public static void SignOut()
        {
            CommonHelper.DeleteCookie(AuthCookieName);
        }

    }
}