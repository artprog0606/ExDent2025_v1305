//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Linq;
using System.Web;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using AdvantShop.Trial;
using System.IO;
using AdvantShop.Core.Services.Landing;
using AdvantShop.FilePath;
using AdvantShop.Core.Services.Diagnostics;
using AdvantShop.Core.Scheduler.Jobs;
using AdvantShop.Core.Services.Customers;
using AdvantShop.Repository;
using AdvantShop.Core.Services.Helpers;
using AdvantShop.Diagnostics;

namespace AdvantShop.Core.UrlRewriter
{
    public class HttpUrlRewrite : IHttpModule
    {
        public static readonly string AccessControlAllowOrigin = "Access-Control-Allow-Origin";
        public static readonly string AccessControlAllowMethods = "Access-Control-Allow-Methods";
        public static readonly string AccessControlAllowHeaders = "Access-Control-Allow-Headers";
        public static readonly string AccessControlAllowCredentials = "Access-Control-Allow-Credentials";
        public static readonly string AllowedCrossOriginResourceSharingDomain = ".advant.one";

        #region IHttpModule Members

        public void Dispose()
        {
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += OnBeginRequest;
        }

        #endregion

        private static void OnBeginRequest(object sender, EventArgs e)
        {
            var app = (HttpApplication)sender;
            string strCurrentUrl = app.Request.RawUrl.ToLower();
            app.StaticFile304();
            
            // Check cn
            if (AppServiceStartAction.state != PingDbState.NoError)
            {
                // Nothing here
                // just return
                return;
            }
            

            var ip = HttpContext.Current.TryGetIp();
            if (!ip.IsLocalIP() && UrlService.IsIpBanned(ip))
            {
                var customer = CustomerContext.CurrentCustomer;
                if (!customer.IsAdmin)
                {
                    app.RewriteTo403("Access denied from ip " + ip);
                    return;
                }
            }

            var origin = app.Context.Request.Headers.Get("Origin");
            if (!string.IsNullOrWhiteSpace(origin)
                && Uri.TryCreate(origin, UriKind.Absolute, out var uri)
                && uri.Host.EndsWith(AllowedCrossOriginResourceSharingDomain) 
                && app.Context.Request.Url.Host.EndsWith(AllowedCrossOriginResourceSharingDomain))
            {
                app.Context.Response.AddHeader(AccessControlAllowOrigin, origin);
                app.Context.Response.AddHeader(AccessControlAllowMethods, "*");
                app.Context.Response.AddHeader(AccessControlAllowHeaders, "*");
                app.Context.Response.AddHeader(AccessControlAllowCredentials, "true");
            }

            if (strCurrentUrl.Contains("adminv2/userfiles") || strCurrentUrl.Contains("adminv3/userfiles"))
            {
                app.Context.RewritePath(strCurrentUrl.Replace("adminv2/userfiles", "userfiles").Replace("adminv3/userfiles", "userfiles"));
                return;
            }

            if (UrlService.IsDebugUrl(strCurrentUrl) || strCurrentUrl.Contains("/signalr/hubs"))
                return;

            // Check original pictures
            if (strCurrentUrl.Contains("/pictures/product/original/"))
            {
                app.RewriteTo404();
                return;
            }

            if (strCurrentUrl.Contains("/content/price_temp"))
            {
                var customer = CustomerContext.CurrentCustomer;
                if (customer == null ||
                    !(customer.IsAdmin || customer.IsVirtual || TrialService.IsTrialEnabled ||
                      (customer.IsModerator &&
                       RoleActionService.GetCustomerRoleActionsByCustomerId(customer.Id).Any(x => x.Role == RoleAction.Orders || x.Role == RoleAction.Catalog))))
                {
                    app.RewriteTo404();
                    return;
                }
            }

            if (strCurrentUrl == "/favicon.ico" && !string.IsNullOrEmpty(SettingsMain.FaviconImageName))
            {
                app.Context.RewritePath(strCurrentUrl.Replace("favicon.ico", FoldersHelper.GetPathRelative(FolderType.Pictures, SettingsMain.FaviconImageName, false)));
                return;
            }

            var path = strCurrentUrl;
            if (!string.IsNullOrWhiteSpace(app.Request.ApplicationPath) && app.Request.ApplicationPath != "/") 
                path = path.Replace(app.Request.ApplicationPath.ToLower(), "");
            
            
            if (SettingsMain.IsTechDomainsReady)
            {
                if (app.Request.IsStrictForAdminTechDomain())
                {
                    // открылись на админку не по тех. домену админки
                    // и запрос не в списке исключений
                    if (!app.Request.IsAdminTechDomain() && !app.Request.IsForAnyTechDomain())
                    {
                        var buildTechDomainUrl = UrlRewriteExtensions.BuildTechDomainUrl(app.Request.Url,
                            SettingsMain.TechDomainAdminPanel);
                        // редиректим на тех. домен админки
                        app.Response.Redirect(buildTechDomainUrl);
                        return;
                    }
                }
                else
                {
                    // если по тех. домену админки идем не в админку
                    // и запрос не в списке исключений (js, картинки и тп)
                    if (app.Request.IsAdminTechDomain() && 
                        (!app.Request.IsForAnyTechDomain() || 
                         (CustomerContext.CurrentCustomer.IsBuyer && app.Request.Url.PathAndQuery.Contains("/pictures"))))
                    {
                        var buildTechDomainUrl = UrlRewriteExtensions.BuildTechDomainUrl(app.Request.Url,
                            SettingsMain.SiteUrl);
                        // редиректим на тех. домен клиентки
                        app.Response.Redirect(buildTechDomainUrl);
                        return;
                    }
                }  
            }

            if (strCurrentUrl.Contains("robots.txt"))
            {
                if (SettingsMain.IsTechDomain(app.Request.Url))
                {
                    app.Response.Clear();
                    app.Response.Write("User-agent: *\r\nDisallow: /");
                    app.Response.End();
                    return;
                }


                if (MobileHelper.IsMobileEnabled())
                {
                    UrlService.TryResponseFileFromDb("/areas/mobile/robots.txt", app.Response);
                    return;
                }

                if (LandingHelper.IsLandingDomain(app.Request.Url, out var lpSiteId))
                {
                    app.Context.RewritePath("pictures/landing/" + lpSiteId + "/robots.txt");
                    return;
                }
            }

            if (strCurrentUrl.Contains("/" + FoldersHelper.PhotoFoldersPath[FolderType.ApplicationTempData]))
            {
                var customer = CustomerContext.CurrentCustomer;
                
                if (customer == null || !(customer.IsAdmin || customer.IsVirtual || customer.IsModerator || TrialService.IsTrialEnabled))
                {
                    app.Response.Redirect(UrlService.GetUrl("error/NotFound"));
                    return;
                }
            }

            var index = path.IndexOf('?');
            var pathWithoutQuery = index > 0 ? path.Substring(0, index) : path;

            if (pathWithoutQuery.Contains("/content/attachments/"))
            {
                var customer = CustomerContext.CurrentCustomer;
                
                if (customer == null || !(customer.IsAdmin || customer.IsVirtual || customer.IsModerator || TrialService.IsTrialEnabled))
                {
                    app.Response.Redirect(UrlService.GetAdminUrl("login") + "?from=" + strCurrentUrl);
                    return;
                }
            }

            if (pathWithoutQuery.Contains("/" + FoldersHelper.PhotoFoldersPath[FolderType.TemplateDocx]))
            {
                var customer = CustomerContext.CurrentCustomer;
                
                if (customer == null || !(customer.IsAdmin || customer.IsVirtual || customer.IsModerator || TrialService.IsTrialEnabled))
                {
                    app.Response.Redirect(UrlService.GetAdminUrl("login") + "?from=" + strCurrentUrl);
                    return;
                }
            }

            if (pathWithoutQuery.Contains("/sitemap"))
            {
                if (LandingHelper.IsLandingDomain(app.Request.Url, out var lpSiteId))
                {
                    app.Context.RewritePath("pictures/landing/" + lpSiteId + pathWithoutQuery);
                    return;
                }
            }

            var extension = FileHelpers.GetExtensionWithoutException(pathWithoutQuery);

            if (!string.IsNullOrEmpty(extension) &&
                !UrlRewriteExtensions.AlowedExtensionsForTechDomainClosed.Contains(extension) &&
                app.Request.IsTechDomainClosed(pathWithoutQuery))
            {
                app.Response.Redirect(UrlService.GetUrl("errorext/techdomainclosed"));
                return;
            }

            if (string.IsNullOrEmpty(extension) && app.Request.IsAppBlock())
            {
                app.Context.RewritePath("~/app_block.htm");
                return;
            }

            if (!string.IsNullOrEmpty(extension) && UrlService.TryResponseFileFromDb(pathWithoutQuery, app.Response))
                return;

            if (UrlService.ExtentionNotToRedirect.Contains(extension) && !pathWithoutQuery.Contains("/robots.txt") && !pathWithoutQuery.Contains("/sitemap"))
            {
                if (UrlService.ExtentionOpenInBrowser.Contains(extension) && app.Request["OpenInBrowser"] == "true")
                {
                    app.Response.Clear();
                    app.Response.ContentType = "text/plain";
                    app.Response.AddHeader("content-disposition", "inline;filename=" + Path.GetFileName(app.Request.FilePath));
                    app.Response.WriteFile(System.Web.Hosting.HostingEnvironment.MapPath(app.Request.FilePath));
                    app.Response.End();
                }
                return;
            }
            
            if (!CountryService.IsAllowSiteBrowsingByIp(ip))
            {
                var customer = CustomerContext.CurrentCustomer;
                
                if (!customer.RegistredUser)
                {
                    app.RewriteTo403("Access denied from the country of the ip " + ip);
                    return;
                }
            }

            if (strCurrentUrl.Contains("/module/"))
            {
                app.Context.RewritePath(strCurrentUrl.Replace("/module/", "/"));
                return;
            }

            if (path.Contains("adv-admin.aspx") || pathWithoutQuery == "/admin")
            {
                if (!app.Request.QueryString["fcm"].IsNullOrEmpty())
                {
                    if (CustomerContext.CurrentCustomer.RegistredUser && !CustomerContext.CurrentCustomer.IsBuyer)
                    {
                        CustomerAdminPushNotificationService.UpdateFcmToken(CustomerContext.CustomerId, app.Request.QueryString["fcm"]);
                    }

                    CommonHelper.SetCookie("afcm", app.Request.QueryString["fcm"], httpOnly: true);
                }

                app.Response.Redirect(LandingHelper.IsLandingDomain(app.Request.Url, out _)
                    ? SettingsMain.SiteUrl + "/adminv2/login"
                    : UrlService.GetAdminUrl("login"));
            }

            var absoluteUri = (app.Request.Headers["x-forwarded-proto"] == "https"
                ? app.Request.Url.AbsoluteUri.Replace("http://", "https://")
                : app.Request.Url.AbsoluteUri).ToLower();

            var pathAndQuery = app.Request.Url.PathAndQuery;

            if (absoluteUri.EndsWith(pathAndQuery, StringComparison.Ordinal))
                absoluteUri = absoluteUri.Substring(0, absoluteUri.Length - pathAndQuery.Length) + strCurrentUrl;
            
            //301 redirect if need
            if (SettingsSEO.Enabled301Redirects && 
                !path.Contains("/admin/") && !path.Contains("/api/") && !path.Contains("paymentnotification") && 
                !DebugMode.IsDebugMode(eDebugMode.Redirects) && 
                !(app.Request.IsAjaxRequest() || app.Request.HttpMethod == "POST") && 
                app.Request.UserAgent != JobBeAlive.UserAgent &&
                !app.Request.IsTechDomainClosed())
            {
                var newUrl = UrlService.GetRedirect301(path.Trim('/'), absoluteUri.Trim('/'));
                if (newUrl.IsNotEmpty())
                {
                    var query = app.Request.Url.Query;
                    if (newUrl.EndsWith(query, StringComparison.OrdinalIgnoreCase))
                        newUrl = newUrl.Substring(0, newUrl.Length - query.Length) + query;

                    app.Response.RedirectPermanent(newUrl);
                    return;
                }

                var dirPath = path.Split("?").FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(dirPath) && dirPath != "/" && dirPath.EndsWith("/"))
                {
                    var url = UrlService.GetUrl(app.Request.Url.LocalPath);
                    app.Response.RedirectPermanent(url.Trim('/') + app.Request.Url.Query);
                    return;
                }

                // checking double slashes in path
                var requestUrl = (app.Request.ServerVariables["REQUEST_URI"] ?? "").ToLower();
                var rewriteUrl = (app.Request.ServerVariables["UNENCODED_URL"] ?? "").ToLower();

                if (rewriteUrl.Split('?')[0].Contains("//") && !requestUrl.Contains("//"))
                {
                    app.Response.RedirectPermanent(requestUrl);
                    return;
                }
                // checking double quotes in path
                if (requestUrl.Split('?')[0].Contains("\""))
                {
                    app.Response.RedirectPermanent(requestUrl.Replace("\"", ""));
                    return;
                }
            }

            if (!app.Context.Request.IsSecureConnection
                && TrialService.IsTrialEnabled
                && SettingsMain.IsTechDomain(app.Request.Url)
                && (!UrlService.IsOnSubdomain(app.Request.Url)
                    || UrlService.GetSubDomain(app.Request.Url)?.Length < 5))
            {
                var uriBuilder = new UriBuilder(app.Request.Url)
                {
                    Scheme = Uri.UriSchemeHttps,
                    Port = -1, // default port for scheme
                };

                app.Response.Redirect(uriBuilder.Uri.AbsoluteUri);
                return;
            }

            if (LandingHelper.IsLandingUrl(app, app.Request.Url, extension, out var rewriteLpUrl))
            {
                app.Context.RewritePath(rewriteLpUrl);
                return;
            }

            UrlService.ESocialType socialType;

            if (strCurrentUrl.Contains("/adminv3/") || strCurrentUrl.Contains("/adminv2/"))
            {
                SettingsDesign.IsMobileTemplate = MobileHelper.IsMobileAdmin();
            }
            else if (MobileHelper.IsMobileEnabled())
            {
                SettingsDesign.IsMobileTemplate = SettingsMobile.IsMobileTemplateActive;

                if (!SettingsDesign.IsMobileTemplate)
                {
                    app.Context.RewritePath("error/forbidden");
                    return;
                }
            }

            if ((socialType = UrlService.IsSocialUrl(absoluteUri)) != UrlService.ESocialType.none)
            {
                if ((socialType == UrlService.ESocialType.vk && !SettingsDesign.IsVkTemplateActive) ||
                    (socialType == UrlService.ESocialType.fb && !SettingsDesign.IsFbTemplateActive))
                {
                    app.Context.RewritePath("error/forbidden");
                    return;
                }
            }

            var modules = AttachedModules.GetModules<IModuleUrlRewrite>();
            foreach (var moduleType in modules)
            {
                var moduleObject = (IModuleUrlRewrite)Activator.CreateInstance(moduleType);
                var newUrl = path;
                if (moduleObject != null && moduleObject.RewritePath(path, ref newUrl))
                {
                    // for trial and virtual path
                    if (newUrl.StartsWith("/"))
                        newUrl = "~" + newUrl;
                    app.Context.RewritePath(newUrl);
                    return;
                }
            }
        }
    }
}