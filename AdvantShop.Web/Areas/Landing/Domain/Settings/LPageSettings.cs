using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.App.Landing.Domain.Auth;
using AdvantShop.App.Landing.Domain.Common;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Landing;

namespace AdvantShop.App.Landing.Domain.Settings
{
    /// <summary>
    /// Настройки страницы лендинга
    /// </summary>
    public static class LPageSettings
    {
        private static LpSettingsService _service = new LpSettingsService();
        
        public static string PageTitle
        {
            get { return _service.Get(LpService.CurrentLanding.Id, "SeoSettings.PageTitle"); }
            set { _service.AddOrUpdate(LpService.CurrentLanding.Id, "SeoSettings.PageTitle", value); }
        }

        public static string PageKeywords
        {
            get { return _service.Get(LpService.CurrentLanding.Id, "SeoSettings.PageKeywords"); }
            set { _service.AddOrUpdate(LpService.CurrentLanding.Id, "SeoSettings.PageKeywords", value); }
        }

        public static string PageDescription
        {
            get { return _service.Get(LpService.CurrentLanding.Id, "SeoSettings.PageDescription"); }
            set { _service.AddOrUpdate(LpService.CurrentLanding.Id, "SeoSettings.PageDescription", value); }
        }
        
        public static string PageCss
        {
            get { return _service.Get(LpService.CurrentLanding.Id, "SeoSettings.PageCss"); }
            set { _service.AddOrUpdate(LpService.CurrentLanding.Id, "SeoSettings.PageCss", value); }
        }

        public static string PageHeadHtml
        {
            get { return _service.Get(LpService.CurrentLanding.Id, "SeoSettings.PageHeadHtml"); }
            set { _service.AddOrUpdate(LpService.CurrentLanding.Id, "SeoSettings.PageHeadHtml", value); }
        }

        /// <summary>
        /// Отключить показ сквозных блоков для страницы
        /// </summary>
        public static bool DisableBlocksOnAllPages
        {
            get
            {
                var s = _service.Get(LpService.CurrentLanding.Id, "SeoSettings.DisableBlocksOnAllPages");
                return !string.IsNullOrEmpty(s) && Convert.ToBoolean(s);
            }
            set { _service.AddOrUpdate(LpService.CurrentLanding.Id, "SeoSettings.DisableBlocksOnAllPages", value.ToString()); }
        }


        public static bool ShowShoppingCart(int landingId)
        {
            var s = _service.Get(landingId, "SeoSettings.ShowShoppingCart");
            return !string.IsNullOrEmpty(s) && Convert.ToBoolean(s);
        }

        public static void SetShowShoppingCart(int landingId, bool value)
        {
            _service.AddOrUpdate(landingId, "SeoSettings.ShowShoppingCart", value.ToString());
        }

        public static ELpShoppingCartType GetShoppingCartType(int landingId)
        {
            return (ELpShoppingCartType)_service.Get(landingId, "SeoSettings.ShoppingCartType").TryParseInt();
        }

        public static void SetShoppingCartType(int landingId, ELpShoppingCartType shoppingCartType)
        {
            _service.AddOrUpdate(landingId, "SeoSettings.ShoppingCartType", ((int)shoppingCartType).ToString());
        }

        public static bool ShoppingCartHideShipping(int landingId)
        {
            var s = _service.Get(landingId, "SeoSettings.ShoppingCartHideShipping");
            return !string.IsNullOrEmpty(s) && Convert.ToBoolean(s);
        }

        public static void SetShoppingCartHideShipping(int landingId, bool value)
        {
            _service.AddOrUpdate(landingId, "SeoSettings.ShoppingCartHideShipping", value.ToString());
        }

        public static bool IgnoreActionParams(int landingId)
        {
            var s = _service.Get(landingId, "SeoSettings.IgnoreActionParams");
            return !string.IsNullOrEmpty(s) && Convert.ToBoolean(s);
        }

        public static void SetIgnoreActionParams(int landingId, bool value)
        {
            _service.AddOrUpdate(landingId, "SeoSettings.IgnoreActionParams", value.ToString());
        }

        #region Auth

        /// <summary>
        /// Разрешить доступ всем к странице
        /// </summary>
        public static bool AllowAccess
        {
            get { return _service.Get(LpService.CurrentLanding.Id, "SeoSettings.AllowAccess").TryParseBool(); }
            set { _service.AddOrUpdate(LpService.CurrentLanding.Id, "SeoSettings.AllowAccess", value.ToString()); }
        }

        /// <summary>
        /// Спрашивать авторизацию для определенной страницы
        /// </summary>
        public static bool RequireAuth
        {
            get { return _service.Get(LpService.CurrentLanding.Id, "SeoSettings.RequireAuth").TryParseBool(); }
            set { _service.AddOrUpdate(LpService.CurrentLanding.Id, "SeoSettings.RequireAuth", value.ToString()); }
        }

        public static string AuthRegUrl
        {
            get { return _service.Get(LpService.CurrentLanding.Id, "SeoSettings.AuthRegUrl"); }
            set { _service.AddOrUpdate(LpService.CurrentLanding.Id, "SeoSettings.AuthRegUrl", value); }
        }

        public static ELpAuthFilterRule AuthFilterRule
        {
            get { return (ELpAuthFilterRule)_service.Get(LpService.CurrentLanding.Id, "SeoSettings.AuthFilterRule").TryParseInt(); }
            set { _service.AddOrUpdate(LpService.CurrentLanding.Id, "SeoSettings.AuthFilterRule", ((int)value).ToString()); }
        }

        public static int? AuthOrderProductId
        {
            get { return _service.Get(LpService.CurrentLanding.Id, "SeoSettings.AuthOrderProductId").TryParseInt(true); }
            set { _service.AddOrUpdate(LpService.CurrentLanding.Id, "SeoSettings.AuthOrderProductId", value.ToString() ); }
        }

        public static List<int> AuthOrderProductIds
        {
            get
            {
                var ids = _service.Get(LpService.CurrentLanding.Id, "SeoSettings.AuthOrderProductIds");
                return !string.IsNullOrEmpty(ids) ? ids.Split(',').Select(x => Convert.ToInt32(x)).ToList() : new List<int>();
            }
            set
            {
                var v = value != null ? String.Join(",", value.ToArray()) : null;
                _service.AddOrUpdate(LpService.CurrentLanding.Id, "SeoSettings.AuthOrderProductIds", v);
            }
        }

        public static int? AuthLeadSalesFunnelId
        {
            get { return _service.Get(LpService.CurrentLanding.Id, "SeoSettings.AuthLeadSalesFunnelId").TryParseInt(true); }
            set { _service.AddOrUpdate(LpService.CurrentLanding.Id, "SeoSettings.AuthLeadSalesFunnelId", value.ToString()); }
        }

        public static int? AuthLeadDealStatusId
        {
            get { return _service.Get(LpService.CurrentLanding.Id, "SeoSettings.AuthLeadDealStatusId").TryParseInt(true); }
            set { _service.AddOrUpdate(LpService.CurrentLanding.Id, "SeoSettings.AuthLeadDealStatusId", value.ToString()); }
        }

        #endregion

    }
}
