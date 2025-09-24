//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Web;
using AdvantShop.Core.UrlRewriter;

namespace AdvantShop.SEO
{
    public class Error404
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string UrlReferer { get; set; }
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
        public DateTime DateAdded { get; set; }

        public Error404()
        {
        }

        public Error404(HttpRequest request)
        {
            Url = request.RawUrl.TrimStart('/');
            
            var referer = request.GetUrlReferrer();
            UrlReferer = referer != null ? referer.AbsoluteUri : string.Empty;
            
            IpAddress = request.UserHostAddress;
            UserAgent = request.UserAgent;
        }

        public override int GetHashCode()
        {
            return (Url ?? string.Empty).GetHashCode() ^
                   (UrlReferer ?? string.Empty).GetHashCode() ^
                   (IpAddress ?? string.Empty).GetHashCode() ^
                   (UserAgent ?? string.Empty).GetHashCode();
        }
    }
}
