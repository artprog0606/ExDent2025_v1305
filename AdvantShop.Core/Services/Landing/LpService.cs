using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Hosting;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.Services.Landing.Pictures;
using AdvantShop.Core.Services.Landing.Templates;
using AdvantShop.Core.SQL;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using AdvantShop.Saas;

namespace AdvantShop.Core.Services.Landing
{
    public class LpService
    {
        public static int CurrentSiteId
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    var id = HttpContext.Current.Items["Lp_CurrentLandingSiteId"];
                    if (id != null)
                        return Convert.ToInt32(id);

                    if (CurrentLanding != null)
                        return CurrentSiteId = CurrentLanding.LandingSiteId;
                }
                return 0;
            }
            set => HttpContext.Current.Items["Lp_CurrentLandingSiteId"] = value;
        }

        /// <summary>
        /// Используется для расчета ширины товаров и т.д с плавающей шириной
        /// </summary>
        public const int SiteWidthContent = 1159;

        public static Lp CurrentLanding
        {
            get => HttpContext.Current != null ? HttpContext.Current.Items["Lp_CurrentLanding"] as Lp : null;
            set => HttpContext.Current.Items["Lp_CurrentLanding"] = value;
        }

        /// <summary>
        /// Режим инплейса
        /// </summary>
        public static bool Inplace
        {
            get => HttpContext.Current != null && Convert.ToBoolean(HttpContext.Current.Items["Lp_Inplace"]);
            set => HttpContext.Current.Items["Lp_Inplace"] = value.ToString();
        }

        /// <summary>
        /// Режим конвертирования в html
        /// </summary>
        public static bool ConvertingToHtml
        {
            get => HttpContext.Current != null && Convert.ToBoolean(HttpContext.Current.Items["Lp_ConvertingToHtml"]);
            set => HttpContext.Current.Items["Lp_ConvertingToHtml"] = value.ToString();
        }

        public static bool HasAccess
        {
            get => HttpContext.Current != null && Convert.ToBoolean(HttpContext.Current.Items["Lp_HasAccess"]);
            set => HttpContext.Current.Items["Lp_HasAccess"] = value.ToString();
        }

        public static bool ShowShoppingCart
        {
            get => HttpContext.Current != null && Convert.ToBoolean(HttpContext.Current.Items["Lp_ShowShoppingCart"]);
            set => HttpContext.Current.Items["Lp_ShowShoppingCart"] = value.ToString();
        }

        public static int? IgnoredLpId
        {
            get =>
                HttpContext.Current != null
                    ? Convert.ToString(HttpContext.Current.Items["Lp_IgnoredLpId"]).TryParseInt(true)
                    : null;
            set => HttpContext.Current.Items["Lp_IgnoredLpId"] = value.ToString();
        }

        /// <summary>
        /// OrderId or LeadId
        /// </summary>
        public static int EntityId
        {
            get => HttpContext.Current != null ? Convert.ToInt32(HttpContext.Current.Items["Lp_EntityId"]) : 0;
            set => HttpContext.Current.Items["Lp_EntityId"] = value;
        }

        public static string OrderCode
        {
            get => HttpContext.Current != null ? HttpContext.Current.Items["Lp_OrderCode"] as string : null;
            set => HttpContext.Current.Items["Lp_OrderCode"] = value;
        }

        /// <summary>
        /// "order", "lead"
        /// </summary>
        public static string EntityType
        {
            get => HttpContext.Current != null ? Convert.ToString(HttpContext.Current.Items["Lp_EntityType"]) : "";
            set => HttpContext.Current.Items["Lp_EntityType"] = value;
        }

        /// <summary>
        /// mode=lp
        /// </summary>
        public static string Mode
        {
            get => HttpContext.Current != null ? Convert.ToString(HttpContext.Current.Items["Lp_Mode"]) : "";
            set => HttpContext.Current.Items["Lp_Mode"] = value;
        }

        /// <summary>
        /// Add prefix "code=" or "lid=" to url
        /// </summary>
        public static string GetUrlWithEntityPrefix(string url)
        {
            if (string.IsNullOrEmpty(url))
                return "";

            var prefix = EntityId != 0
                ? (!url.Contains('?') ? "?" : "&") +
                  (EntityType == "order"
                      ? "code=" + (!string.IsNullOrEmpty(OrderCode) ? OrderCode : EntityId.ToString())
                      : "lid=" + EntityId)
                : "";

            if (prefix != "" && !string.IsNullOrEmpty(Mode))
                prefix += "&mode=" + Mode;

            return url + prefix;
        }

        #region CRUD

        public int Add(Lp lp)
        {
            if (SaasDataService.IsSaasEnabled)
            {
                if (GetLandingPageCount() >= SaasDataService.CurrentSaasData.LandingFunnelPageCount)
                    throw new BlException("Количество страниц воронок по тарифному плану не может превышать " + SaasDataService.CurrentSaasData.LandingFunnelPageCount);
            }

            lp.Id = Convert.ToInt32(SQLDataAccess.ExecuteScalar(
                "INSERT INTO [CMS].[Landing] ([LandingSiteId],[Url],[Enabled],[Name],[Template],[CreatedDate],[IsMain],[PageType], [ProductId]) " +
                "VALUES (@LandingSiteId,@Url,@Enabled,@Name,@Template,GetDate(),@IsMain,@PageType,@ProductId); Select scope_identity(); ",
                CommandType.Text,
                new SqlParameter("@Url", lp.Url),
                new SqlParameter("@Name", lp.Name ?? string.Empty),
                new SqlParameter("@Template", lp.Template),
                new SqlParameter("@Enabled", lp.Enabled),
                new SqlParameter("@LandingSiteId", lp.LandingSiteId),
                new SqlParameter("@IsMain", lp.IsMain),
                new SqlParameter("@PageType", (int)lp.PageType),
                new SqlParameter("@ProductId", lp.ProductId ?? (object)DBNull.Value)
                ));

            CacheManager.RemoveByPattern(LpConstants.LpCachePrefix);

            return lp.Id;
        }

        public void Update(Lp lp)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Update [CMS].[Landing] Set [Url] = @Url, [Enabled] = @Enabled, [Name] = @Name, [Template] = @Template, [PageType] = @PageType, [IsMain] = @IsMain Where Id = @Id",
                CommandType.Text,
                new SqlParameter("@Url", lp.Url),
                new SqlParameter("@Enabled", lp.Enabled),
                new SqlParameter("@Name", lp.Name ?? string.Empty),
                new SqlParameter("@Template", lp.Template),
                new SqlParameter("@PageType", (int)lp.PageType),
                new SqlParameter("@IsMain", lp.IsMain),
                new SqlParameter("@Id", lp.Id)
                );

            CacheManager.RemoveByPattern(LpConstants.LpCachePrefix);
        }

        public void Delete(int id)
        {
            var lp = Get(id);
            if (lp == null)
                return;
            
            SQLDataAccess.ExecuteNonQuery(
                "Delete From [CMS].[Landing] Where Id = @Id",
                CommandType.Text,
                new SqlParameter("@Id", id));

            new RemoveLandingPicturesHandler(lp.Id, lp.LandingSiteId).Execute();

            if (lp.IsMain)
            {
                SQLDataAccess.ExecuteNonQuery(
                    "if Exists(Select * from [CMS].[Landing] Where LandingSiteId=@landingSiteId) " +
                    "Update [CMS].[Landing] Set IsMain=1 Where Id = (Select top(1) Id from [CMS].[Landing] Where LandingSiteId=@landingSiteId Order By IsMain desc)",
                    CommandType.Text,
                    new SqlParameter("@landingSiteId", lp.LandingSiteId));
            }

            CacheManager.RemoveByPattern(LpConstants.LpCachePrefix);
        }

        public List<Lp> GetList()
        {
            return SQLDataAccess.Query<Lp>("Select * From [CMS].[Landing]").ToList();
        }

        public int GetLandingPageCount()
        {
            return SQLDataAccess.ExecuteScalar<int>("Select Count(Id) From [CMS].[Landing]", CommandType.Text);
        }

        public List<Lp> GetList(int landingSiteId)
        {
            return SQLDataAccess.Query<Lp>(
                "Select lp.*, LandingSite.Url as SiteUrl " +
                "From [CMS].[Landing] as lp " +
                "Inner Join [CMS].[LandingSite] ON [LandingSite].[Id] = lp.[LandingSiteId] " +
                "Where LandingSiteId=@landingSiteId " +
                "Order By IsMain desc",
                new { landingSiteId }).ToList();
        }

        public Lp Get(int id)
        {
            return CacheManager.Get(LpConstants.LpCachePrefix + id, LpConstants.LpCacheTime,
                () =>
                    SQLDataAccess.Query<Lp>("Select * From [CMS].[Landing] Where Id = @id", new {id})
                        .FirstOrDefault());
        }

        public Lp Get(int siteId, string url)
        {
            return CacheManager.Get(LpConstants.LpCachePrefix + siteId + url, LpConstants.LpCacheTime,
                () => 
                    SQLDataAccess.Query<Lp>("Select top(1) * From [CMS].[Landing] Where LandingSiteId = @siteId and Url = @url", new {siteId, url})
                        .FirstOrDefault());
        }

        public Lp GetByMain(int siteId)
        {
            return CacheManager.Get(LpConstants.LpCachePrefix + siteId + "ismain", LpConstants.LpCacheTime,
                () =>
                    SQLDataAccess.Query<Lp>("Select top(1) * From [CMS].[Landing] Where LandingSiteId = @siteId and IsMain = 1", new { siteId })
                        .FirstOrDefault());
        }

        public Lp GetByName(string name)
        {
            return SQLDataAccess.Query<Lp>("Select top(1) * From [CMS].[Landing] Where LOWER(Name) = @name", new { name = name.ToLower() })
                        .FirstOrDefault();
        }

        public Lp GetThankYouPage(int id)
        {
            return SQLDataAccess.Query<Lp>(
                "Select top(1) * " +
                "From [CMS].[Landing] " +
                "Where LandingSiteId=(Select LandingSiteId From [CMS].[Landing] Where Id=@id) and PageType=@pageType",
                new {id, pageType = (int) LpTemplatePageType.ThankYouPage})
                .FirstOrDefault();
        }

        #endregion

        public string GetAvailableUrl(int siteId, string name)
        {
            name = name.ToLower();
            
            var transformUrl = !SettingsMain.EnableCyrillicUrl
                ? StringHelper.TransformUrl(StringHelper.Translit(name))
                : StringHelper.TransformUrl(name ?? "");
            
            if (transformUrl.Length > 150)
                transformUrl = transformUrl.Substring(0, 150);
            
            var url = transformUrl;
            var i = 1;

            while (Get(siteId, url) != null)
            {
                url = transformUrl;

                if (url.Length + i.ToString().Length >= 150)
                    url = url.Substring(0, url.Length - i.ToString().Length - 1);

                url += "-" + (i++);
            }

            return url;
        }

        public bool CanEdit(Customer customer)
        {
            return customer != null && customer.Enabled &&
                   (customer.IsAdmin || (customer.IsModerator && customer.HasRoleAction(RoleAction.Landing)));
        }

        public bool ReGenerateCss(int landingSiteId)
        {
            try
            {
                var wc = new WebClient();
                wc.Headers.Add(UrlRewriteExtensions.TechnicalHeaderName, SettingsLic.AdvId);

                var css = wc.DownloadString(UrlService.GetUrl("landing/landing/headcss/" + landingSiteId));

                var cssFile = HostingEnvironment.MapPath(string.Format(LpFiles.LpGeneratedCss, landingSiteId));
                var dir = HostingEnvironment.MapPath(string.Format(LpFiles.LpSitePath, landingSiteId));

                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                using (var w = new StreamWriter(cssFile))
                    w.Write(css ?? "");
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                return false;
            }
            return true;
        }

        public string GetLpLinkByMain(Lp lp)
        {
            var id = lp.Id;
            if (!lp.IsMain)
            {
                var mainLp = GetByMain(lp.LandingSiteId);
                if (mainLp != null)
                    id = mainLp.Id;
            }

            return GetLpLink(HttpContext.Current.Request.Url.Host, id);
        }

        public static string GetTechUrl(Lp lp)
        {
            var site = new LpSiteService().Get(lp.LandingSiteId);

            return site != null ? GetTechUrl(site.Url, lp.Url, lp.IsMain) : "";
        }

        public static string GetTechUrl(string siteUrl, string lpUrl, bool isMain, bool techAdmin = false)
        {
            var url = CommonHelper.IsLocalUrl() ? UrlService.GetUrl() : SettingsMain.SiteUrl + "/";

            return (SettingsMain.IsTechDomainsReady && techAdmin ? SettingsMain.TechDomainAdminPanel + "/" : url) 
                   + "lp/" + siteUrl + (isMain ? "" : "/" + lpUrl);
        }

        public string GetLpLink(int lpId)
        {
            return GetLpLink(HttpContext.Current.Request.Url.Host, lpId);
        }

        public string GetLpLink(string host, int lpId)
        {
            return GetLpLink(host, Get(lpId));
        }

        public string GetLpLink(string host, Lp lp)
        {
            if (lp == null) 
                return null;
            
            var site = new LpSiteService().Get(lp.LandingSiteId);
            if (site == null)
                return null;

            return
                !string.IsNullOrEmpty(site.DomainUrl) && host == StringHelper.ToPuny(site.DomainUrl)
                    ? (lp.IsMain ? UrlService.GetUrl() : UrlService.GetUrl(HttpUtility.UrlEncode(lp.Url)))
                    : UrlService.GetUrl(
                        $"lp/{HttpUtility.UrlEncode(site.Url)}{(lp.IsMain ? "" : "/" + HttpUtility.UrlEncode(lp.Url))}");
        }

        public string GetLpLinkRelative(int lpId)
        {
            var lp = Get(lpId);
            if (lp == null) 
                return null;
            
            var site = new LpSiteService().Get(lp.LandingSiteId);
            if (site == null)
                return null;

            return
                !string.IsNullOrEmpty(site.DomainUrl) &&
                HttpContext.Current.Request.Url.Host == StringHelper.ToPuny(site.DomainUrl)
                    ? HttpUtility.UrlEncode(lp.Url)
                    : $"lp/{HttpUtility.UrlEncode(site.Url)}{(lp.IsMain ? "" : "/" + HttpUtility.UrlEncode(lp.Url))}";
        }

        public static bool PreviewInAdmin => HttpContext.Current != null && Convert.ToBoolean(HttpContext.Current.Request["previewInAdmin"]);

        public bool CheckAuthByOrderAndProduct(Guid customerId, List<int> productIds)
        {
            var check = OrderService.IsCustomerHasPaidOrderWithProducts(customerId, productIds);
            return check;
        }

        public bool CheckAuthByLead(Guid customerId, int? salesFunnelId, int? dealStatusId)
        {
            if (salesFunnelId == null)
                return false;

            var customerLeads = LeadService.GetLeadsByCustomer(customerId).Where(x => x.SalesFunnelId == salesFunnelId).ToList();

            return dealStatusId != null
                ? customerLeads.Any(x => x.DealStatusId == dealStatusId.Value)
                : customerLeads.Any();
        }
    }
}
