using System;
using System.Globalization;
using System.Web;
using System.Web.Mvc;
using AdvantShop.Core.Common;

namespace AdvantShop.Web.Infrastructure.Filters
{
    public class CacheFilterAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Gets or sets the cache duration in seconds. The default is 1 second.
        /// </summary>
        /// <value>The cache duration in seconds.</value>
        public int Duration { get; set; }

        public int LastModifiedPeriod { get; set; }

        public HttpCacheability Cacheability { get; set; }

        public CacheFilterAttribute()
        {
            Duration = 1;
            Cacheability = HttpCacheability.Private;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.IsChildAction || Duration <= 0 || filterContext.HttpContext.Request.IsAjaxRequest())
                return;

            var ifModifiedSinceHeader = filterContext.HttpContext.Request.Headers["If-Modified-Since"] ?? string.Empty;
            DateTime ifModifiedSince;
            if (LastModifiedPeriod == 0 &&
                DateTime.TryParseExact(ifModifiedSinceHeader, "R", CultureInfo.InvariantCulture, DateTimeStyles.None, out ifModifiedSince) &&
                (int)(ifModifiedSince.ToLocalTime() - DataModificationFlag.LastModified).TotalSeconds >= 0)
            {
                filterContext.HttpContext.Response.SuppressContent = true;
                filterContext.HttpContext.Response.StatusCode = 304;
                filterContext.Result = new EmptyResult();
            }
            else
            {

                filterContext.HttpContext.Response.Cache.SetCacheability(Cacheability);
                filterContext.HttpContext.Response.Cache.SetExpires(DateTime.Now.AddSeconds(Duration));
                filterContext.HttpContext.Response.Cache.SetMaxAge(TimeSpan.FromSeconds(Duration));
                if (LastModifiedPeriod != 0)
                {
                    filterContext.HttpContext.Response.Cache.SetLastModified(DateTime.Now.AddSeconds(-LastModifiedPeriod));
                }
                else
                {
                    filterContext.HttpContext.Response.Cache.SetLastModified(DataModificationFlag.LastModified);
                }
                //cache.AppendCacheExtension("must-revalidate, proxy-revalidate");
            }
        }
    }
}
