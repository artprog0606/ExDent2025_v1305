using System.Web.Mvc;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Security.OAuth;
using AdvantShop.Web.Infrastructure.Extensions;

namespace AdvantShop.Controllers
{
    public sealed class AuthController : BaseClientController
    {
        public ActionResult LoginVkId(string pageToRedirect)
        {
            return Redirect(new VkIdOAuth().OpenDialog(pageToRedirect));
        }

        public ActionResult VkId()
        {
            var failUrl = Url.AbsoluteRouteUrl("Login");

            if (!SettingsOAuth.VkIdActive)
            {
                Debug.Log.Info("Авторизация через VK ID не активна");
                return Redirect(failUrl);
            }

            var state = Request.QueryString["state"];
            if (state.IsNullOrEmpty())
            {
                Debug.Log.Info($"AuthController.VkId {Request?.Url?.Query}");
                return Redirect(failUrl);
            }

            var arr = state.Split('_');
            if (arr.Length != 2)
            {
                Debug.Log.Info($"AuthController.VkId {Request?.Url?.Query}");
                return Redirect(failUrl);
            }

            var customerId = arr[0];
            var redirectUrl = arr[1];

            if (customerId != CustomerContext.CustomerId.ToString())
                return Redirect(failUrl);

            var result = new VkIdOAuth().Login();

            return Redirect(
                result
                    ? UrlService.GetAbsoluteLink(redirectUrl)
                    : failUrl);
        }
    }
}