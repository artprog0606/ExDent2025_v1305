using System.Text.RegularExpressions;
using System.Web;
using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Helpers;
using AdvantShop.SEO;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Settings.Seo
{
    public class AddUpdate301RedirectHandler : AbstractCommandHandler
    {
        private readonly RedirectSeo _redirect;
        private readonly bool _editMode;

        public AddUpdate301RedirectHandler(RedirectSeo model, bool editMode)
        {
            var hasStar = model.RedirectFrom.EndsWith("*");
            
            model.RedirectFrom =
                model.RedirectFrom.IsNotEmpty()
                    ? ToPuny(HttpUtility.UrlDecode(model.RedirectFrom.Trim('*'))) + (hasStar ? "*" : "")
                    : null;
            model.RedirectTo = model.RedirectTo.IsNotEmpty() ? ToPuny(model.RedirectTo.Trim('*')) : "/";

            _redirect = new RedirectSeo
            {
                ID = model.ID,
                RedirectFrom = model.RedirectFrom,
                RedirectTo = model.RedirectTo,
                ProductArtNo = model.ProductArtNo.DefaultOrEmpty()
            };
            _editMode = editMode;
        }

        protected override void Validate()
        {
            if (_redirect.RedirectFrom.IsNullOrEmpty() || _redirect.RedirectFrom == "*")
                throw new BlException(T("Admin.SettingsSeo.Redirects.Errors.RedirectFromRequired"));

            RedirectSeo existRedirect;
            if ((existRedirect = RedirectSeoService.GetRedirectsSeoByRedirectFrom(_redirect.RedirectFrom)) != null && existRedirect.ID != _redirect.ID)
                throw new BlException(T("Admin.SettingsSeo.Redirects.Errors.RedirectFromExists"));

            if (_editMode && RedirectSeoService.GetRedirectSeoById(_redirect.ID) == null)
                throw new BlException(T("Admin.SettingsSeo.Redirects.Errors.RedirectNotFound"));

            if (RedirectSeoService.CheckOnSystemUrl(_redirect.RedirectFrom) || RedirectSeoService.CheckOnSystemUrl(_redirect.RedirectTo))
                throw new BlException(T("Admin.Js.Settings.AddEdit301RedCtrl.SystemUrl"));

            if (RedirectSeoService.IsToManyRedirects(_redirect))
                throw new BlException(T("Admin.Js.Settings.AddEdit301RedCtrl.ErrorToManyRed"));

            if (!_redirect.RedirectFrom.IsNullOrEmpty() && !_redirect.RedirectTo.IsNullOrEmpty() && _redirect.RedirectFrom.ToLower() == _redirect.RedirectTo.ToLower())
                throw new BlException(T("Admin.Js.Settings.AddEdit301RedCtrl.ErrorToManyRed"));
        }

        protected override void Handle()
        {
            if (_editMode)
                RedirectSeoService.UpdateRedirectSeo(_redirect);
            else
                RedirectSeoService.AddRedirectSeo(_redirect);
        }

        private string ToPuny(string urlpath)
        {
            if (string.IsNullOrWhiteSpace(urlpath))
                return "";

            if (urlpath == "/")
                return urlpath;
            
            urlpath = urlpath.Trim('/');

            if (new Regex("\\d+").IsMatch(urlpath))
                return urlpath;
            
            urlpath = StringHelper.ToPuny(urlpath);

            return urlpath.TrimEnd('/');
        }

    }
}

