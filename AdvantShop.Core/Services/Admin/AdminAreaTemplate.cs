using System.Web;
using AdvantShop.Configuration;

namespace AdvantShop.Core.Services.Admin
{
    public static class AdminAreaTemplate
    {
        public static string Current
        {
            get
            {
                if (HttpContext.Current == null)
                    return null;

                var area = HttpContext.Current.Items["AdminAreaTemplate"] != null
                    ? HttpContext.Current.Items["AdminAreaTemplate"] as string
                    : null;

                if (area != null)
                    return area;

                var url = HttpContext.Current.Request.RawUrl.ToLower();
                var index = url.IndexOf('?');
                var urlWhitoutQuery = index > 0 ? url.Substring(0, index) : url;

                if (urlWhitoutQuery.Contains("adminv3"))
                {
                    HttpContext.Current.Items["AdminAreaTemplate"] = area = "adminv3";
                }

                return area;
            }
        }

        public static bool IsAdminv3()
        {
            return Current == "adminv3";
        }

        public static string Template
        {
            get { return SettingProvider.Items["AdminAreaTemplate"] ?? "adminv3"; }
            set { SettingProvider.Items["AdminAreaTemplate"] = value; }
        }
    }
}
