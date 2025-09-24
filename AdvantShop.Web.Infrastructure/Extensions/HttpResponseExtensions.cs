using System.Web;

namespace AdvantShop.Web.Infrastructure.Extensions
{
    public static class HttpResponseExtensions
    {
        public static bool AddHeaderIfNotExists(this HttpResponseBase responseBase, string key, string value)
        {
            if (responseBase.Headers[key] != null)
                return false;

            responseBase.AddHeader(key, value);
            return true;
        }

        public static void AddHeaderRemovePrevious(this HttpResponseBase responseBase, string key, string value)
        {
            if (responseBase.Headers[key] != null) responseBase.Headers.Remove(key);

            responseBase.AddHeader(key, value);
        }
    }
}