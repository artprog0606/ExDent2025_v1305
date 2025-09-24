using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using Newtonsoft.Json;

namespace AdvantShop.Web.Infrastructure.Admin
{
    public class CookieTempDataProvider : ITempDataProvider
    {
        private const string CookieName = "td";

        public void SaveTempData(ControllerContext controllerContext, IDictionary<string, object> values)
        {
            if (values == null || values.Count == 0)
                return;

            var value = JsonConvert.SerializeObject(values);
            IssueCookie(controllerContext, value);
        }

        public IDictionary<string, object> LoadTempData(ControllerContext controllerContext)
        {
            var value = GetCookieValue(controllerContext);
            if (value == null)
                return null;

            ResetCookie(controllerContext);

            return JsonConvert.DeserializeObject<Dictionary<string, object>>(value);
        }

        private string GetCookieValue(ControllerContext controllerContext)
        {
            var c = controllerContext.HttpContext.Request.Cookies[CookieName];
            if (c != null)
                return StringHelper.DecodeFrom64(c.Value);
            
            return null;
        }

        private void IssueCookie(ControllerContext controllerContext, string value)
        {
            var c = new HttpCookie(CookieName, StringHelper.EncodeTo64(value))
            {
                HttpOnly = true,
                Path =
                    controllerContext.HttpContext.Request.ApplicationPath != null
                        ? controllerContext.HttpContext.Request.ApplicationPath.ToLower()
                        : "",
            };
            if (controllerContext.HttpContext.Request.IsSecureConnection)
            {
                c.Secure = true;
                c.SameSite = SameSiteMode.None;
            }

            if (value == null)
            {
                c.Expires = DateTime.Now.AddMonths(-1);
            }

            if (value != null || controllerContext.HttpContext.Request.Cookies[CookieName] != null)
            {
                try
                {
                    controllerContext.HttpContext.Response.Cookies.Add(c);
                }
                catch (Exception ex)
                {
                    Debug.Log.Warn(ex);
                }
                
            }
        }

        private void ResetCookie(ControllerContext controllerContext)
        {
            var cookie = new HttpCookie(CookieName)
            {
                Expires = DateTime.MinValue,
                Value = string.Empty,
                HttpOnly = true,
                Path =
                    controllerContext.HttpContext.Request.ApplicationPath != null
                        ? controllerContext.HttpContext.Request.ApplicationPath.ToLower()
                        : "",
            };
            if (controllerContext.RequestContext.HttpContext.Request.IsSecureConnection)
            {
                cookie.SameSite = SameSiteMode.None;
                cookie.Secure = true;
            }
            controllerContext.HttpContext.Response.Cookies.Add(cookie);
        }
    }
}
