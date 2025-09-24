//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Scheduler;
using AdvantShop.Core.Services.SEO;
using AdvantShop.Saas;

namespace AdvantShop.Configuration
{
    public class SettingsLandingPage
    {
        public static bool ActiveLandingPage
        {
            get =>
                Convert.ToBoolean(SettingProvider.Items["ActiveLandingPage"]) &&
                (!SaasDataService.IsSaasEnabled ||
                 (SaasDataService.IsSaasEnabled && SaasDataService.CurrentSaasData.LandingPage));
            set
            {
                SettingProvider.Items["ActiveLandingPage"] = value.ToString();
                JobActivationManager.SettingUpdated();
            }
        }

        public static string LandingPageCommonStatic
        {
            get => SettingProvider.Items["LandingPageCommonStatic"];
            set => SettingProvider.Items["LandingPageCommonStatic"] = value;
        }
        
        public static bool UseCrossSellLandingsInCheckout
        {
            get => Convert.ToBoolean(SettingProvider.Items["SettingsLandingPage.UseCrossSellLandingsInCheckout"]);
            set
            {
                SettingProvider.Items["SettingsLandingPage.UseCrossSellLandingsInCheckout"] = value.ToString();
                JobActivationManager.SettingUpdated();
            }
        }

        public static ECrossSellShowMode CrossSellShowMode
        {
            get
            {
                var showMode = ECrossSellShowMode.ProductNotInCart;
                Enum.TryParse(SettingProvider.Items["CrossSellShowMode"], out showMode);
                return showMode;
            }
            set => SettingProvider.Items["CrossSellShowMode"] = ((int)value).ToString();
        }

        public static string LastLandingblocksUpdate
        {
            get => SettingProvider.Items["LastLandingblocksUpdate"];
            set => SettingProvider.Items["LastLandingblocksUpdate"] = value;
        }

        public static string CopyrightHtmlContent
        {
            get
            {
                var setting = SettingProvider.Items["LandingCopyrightHtmlContent"];
                if (string.IsNullOrWhiteSpace(setting))
                {
                    setting = CopyrightService.UpdateCopyRight(ECopyrightType.Landing);
                }
                return setting;
            }
            set => SettingProvider.Items["LandingCopyrightHtmlContent"] = value;
        }
    }

    public enum ECrossSellShowMode
    {
        [Localize("AdvantShop.Core.SettingsLandingPage.ECrossSellShowMode.ProductNotInCart")]
        ProductNotInCart = 0,

        [Localize("AdvantShop.Core.SettingsLandingPage.ECrossSellShowMode.Always")]
        Always = 1
    }
}