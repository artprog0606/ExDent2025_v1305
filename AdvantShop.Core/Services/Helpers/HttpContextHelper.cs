using System.Web;

namespace AdvantShop.Core.Services.Helpers
{
    public static class HttpContextHelper
    {
        public static string TryGetIp(this HttpContext context)
        {
            try
            {
                if (context == null)
                    return null;
                
                var ip = context.Request.Headers["X-1Gb-Client-IP"]
                         ?? context.Request.Headers["X-Real-IP"]
                         ?? context.Request.Headers["X-Forwarded-For"]
                         ?? context.Request.UserHostAddress;
                
                return ip;
            }
            catch
            {
                // ignored
            }
            return null;
        }
        
        public static bool TryGetRequest(this HttpContext context, out HttpRequest request)
        {
            request = null;
            
            try
            {
                if (context == null)
                    return false;
                
                request = context.Request;

                return true;
            }
            catch
            {
                // ignored
            }
            return false;
        }
    }
}