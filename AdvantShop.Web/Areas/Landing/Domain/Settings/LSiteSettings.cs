using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.App.Landing.Domain.Auth;
using AdvantShop.App.Landing.Domain.ColorSchemes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.Services.SEO;
using Newtonsoft.Json;

namespace AdvantShop.App.Landing.Domain.Settings
{
    /// <summary>
    /// Настройки сайта лендинга
    /// </summary>
    public static class LSiteSettings
    {
        private static LpSiteSettingsService _service = new LpSiteSettingsService();
        
        public static string BlockInHead
        {
            get { return _service.Get(LpService.CurrentSiteId, "SeoSettings.BlockInHead"); }
            set { _service.AddOrUpdate(LpService.CurrentSiteId, "SeoSettings.BlockInHead", value); }
        }
        
        public static string BlockInBodyBottom
        {
            get { return _service.Get(LpService.CurrentSiteId, "SeoSettings.BlockInBodyBottom"); }
            set { _service.AddOrUpdate(LpService.CurrentSiteId, "SeoSettings.BlockInBodyBottom", value); }
        }

        public static string SiteCss
        {
            get { return _service.Get(LpService.CurrentSiteId, "SeoSettings.SiteCss"); }
            set { _service.AddOrUpdate(LpService.CurrentSiteId, "SeoSettings.SiteCss", value); }
        }

        public static string Favicon
        {
            get { return _service.Get(LpService.CurrentSiteId, "SeoSettings.Favicon"); }
            set { _service.AddOrUpdate(LpService.CurrentSiteId, "SeoSettings.Favicon", value); }
        }

        public static string GetFaviconPath(int lpId)
        {
            return
                !string.IsNullOrEmpty(Favicon)
                    ? string.Format(LpFiles.LpSitePathRelative, lpId) + Favicon
                    : null;
        }

        public static string YandexCounterId
        {
            get { return _service.Get(LpService.CurrentSiteId, "SeoSettings.YandexCounterId"); }
            set { _service.AddOrUpdate(LpService.CurrentSiteId, "SeoSettings.YandexCounterId", value); }
        }

        public static string YandexCounterHtml
        {
            get { return _service.Get(LpService.CurrentSiteId, "SeoSettings.YandexCounterHtml"); }
            set { _service.AddOrUpdate(LpService.CurrentSiteId, "SeoSettings.YandexCounterHtml", value); }
        }

        public static string GoogleCounterId
        {
            get { return _service.Get(LpService.CurrentSiteId, "SeoSettings.GoogleCounterId"); }
            set { _service.AddOrUpdate(LpService.CurrentSiteId, "SeoSettings.GoogleCounterId", value); }
        }

        public static string GoogleTagManagerId
        {
            get { return _service.Get(LpService.CurrentSiteId, "SeoSettings.GoogleTagManagerId"); }
            set { _service.AddOrUpdate(LpService.CurrentSiteId, "SeoSettings.GoogleTagManagerId", value); }
        }

        public static bool UseHttpsForSitemap
        {
            get
            {
                var value = _service.Get(LpService.CurrentSiteId, "SeoSettings.UseHttpsForSitemap");
                if (string.IsNullOrEmpty(value))
                {
                    UseHttpsForSitemap = false;
                    return false;
                }
                return Convert.ToBoolean(value);
            }
            set { _service.AddOrUpdate(LpService.CurrentSiteId, "SeoSettings.UseHttpsForSitemap", value.ToString()); }
        }

        public static bool HideAdvantshopCopyright
        {
            get
            {
                if (CopyrightService.IsCopyrightLocked)
                    return false;
                var hideCopyright = _service.Get(LpService.CurrentSiteId, "SeoSettings.HideAdvantshopCopyright");
                return !string.IsNullOrEmpty(hideCopyright) && Convert.ToBoolean(hideCopyright);
            }
            set
            {
                if (CopyrightService.IsCopyrightLocked is false)
                {
                    _service.AddOrUpdate(LpService.CurrentSiteId, "SeoSettings.HideAdvantshopCopyright",
                        value.ToString());
                }
            }
        }

        #region Auth

        /// <summary>
        /// Спрашивать авторизацию для сайта
        /// </summary>
        public static bool RequireAuth
        {
            get { return _service.Get(LpService.CurrentSiteId, "AuthSettings.RequireAuth").TryParseBool(); }
            set { _service.AddOrUpdate(LpService.CurrentSiteId, "AuthSettings.RequireAuth", value.ToString()); }
        }

        public static string AuthRegUrl
        {
            get { return _service.Get(LpService.CurrentSiteId, "AuthSettings.AuthRegUrl"); }
            set { _service.AddOrUpdate(LpService.CurrentSiteId, "AuthSettings.AuthRegUrl", value); }
        }

        public static ELpAuthFilterRule AuthFilterRule
        {
            get { return (ELpAuthFilterRule)_service.Get(LpService.CurrentSiteId, "AuthSettings.AuthFilterRule").TryParseInt(); }
            set { _service.AddOrUpdate(LpService.CurrentSiteId, "AuthSettings.AuthFilterRule", ((int)value).ToString()); }
        }

        public static List<int> AuthOrderProductIds
        {
            get
            {
                var ids = _service.Get(LpService.CurrentSiteId, "AuthSettings.AuthOrderProductIds");
                return !string.IsNullOrEmpty(ids) ? ids.Split(',').Select(x => Convert.ToInt32(x)).ToList() : new List<int>();
            }
            set
            {
                var v = value != null ? String.Join(",", value.ToArray()) : null;
                _service.AddOrUpdate(LpService.CurrentSiteId, "AuthSettings.AuthOrderProductIds", v);
            }
        }

        public static int? AuthLeadSalesFunnelId
        {
            get { return _service.Get(LpService.CurrentSiteId, "AuthSettings.AuthLeadSalesFunnelId").TryParseInt(true); }
            set { _service.AddOrUpdate(LpService.CurrentSiteId, "AuthSettings.AuthLeadSalesFunnelId", value.ToString()); }
        }

        public static int? AuthLeadDealStatusId
        {
            get { return _service.Get(LpService.CurrentSiteId, "AuthSettings.AuthLeadDealStatusId").TryParseInt(true); }
            set { _service.AddOrUpdate(LpService.CurrentSiteId, "AuthSettings.AuthLeadDealStatusId", value.ToString()); }
        }

        #endregion

        #region MobileApp

        public static bool MobileAppActive
        {
            get { return _service.Get(LpService.CurrentSiteId, "MobileApp.Active").TryParseBool(); }
            set { _service.AddOrUpdate(LpService.CurrentSiteId, "MobileApp.Active", value.ToString()); }
        }

        public static string MobileAppManifestName
        {
            get { return _service.Get(LpService.CurrentSiteId, "MobileApp.ManifestName"); }
            set { _service.AddOrUpdate(LpService.CurrentSiteId, "MobileApp.ManifestName", value); }
        }


        public static string MobileAppName
        {
            get { return _service.Get(LpService.CurrentSiteId, "MobileApp.Name"); }
            set { _service.AddOrUpdate(LpService.CurrentSiteId, "MobileApp.Name", value); }
        }

        public static string MobileAppShortName
        {
            get { return _service.Get(LpService.CurrentSiteId, "MobileApp.ShortName"); }
            set { _service.AddOrUpdate(LpService.CurrentSiteId, "MobileApp.ShortName", value); }
        }

        public static string MobileAppIconImageName
        {
            get { return _service.Get(LpService.CurrentSiteId, "MobileApp.IconImageName"); }
            set { _service.AddOrUpdate(LpService.CurrentSiteId, "MobileApp.IconImageName", value); }
        }

        public static string GetMobileAppIconImagePath()
        {
            return
                !string.IsNullOrEmpty(MobileAppIconImageName)
                    ? Core.UrlRewriter.UrlService.GetUrl(string.Format(LpFiles.LpSitePathRelative, LpService.CurrentSiteId) + MobileAppIconImageName) 
                    : null;
        }

        public static string MobileAppAppleAppStoreLink
        {
            get { return _service.Get(LpService.CurrentSiteId, "MobileApp.AppleAppStoreLink"); }
            set { _service.AddOrUpdate(LpService.CurrentSiteId, "MobileApp.AppleAppStoreLink", value); }
        }

        public static string MobileAppGooglePlayMarket
        {
            get { return _service.Get(LpService.CurrentSiteId, "MobileApp.GooglePlayMarket"); }
            set { _service.AddOrUpdate(LpService.CurrentSiteId, "MobileApp.GooglePlayMarket", value); }
        }

        #endregion

        public static string FontMain
        {
            get { return _service.Get(LpService.CurrentSiteId, "SeoSettings.FontMain"); }
            set { _service.AddOrUpdate(LpService.CurrentSiteId, "SeoSettings.FontMain", value); }
        }


        public static string LineHeight
        {
            get { return _service.Get(LpService.CurrentSiteId, "SeoSettings.LineHeight"); }
            set { _service.AddOrUpdate(LpService.CurrentSiteId, "SeoSettings.LineHeight", value); }
        }

        public static List<LpColorScheme> ColorSchemes
        {
            get
            {
                var schemesJson = _service.Get(LpService.CurrentSiteId, "SeoSettings.ColorSchemes");

                if (!string.IsNullOrEmpty(schemesJson))
                    return JsonConvert.DeserializeObject<List<LpColorScheme>>(schemesJson);

                return GetDefaultSchemes();
            }
            set
            {
                _service.AddOrUpdate(LpService.CurrentSiteId, "SeoSettings.ColorSchemes", JsonConvert.SerializeObject(value ?? GetDefaultSchemes()));
            }
        }

        public static List<LpColorScheme> GetSiteColorSchemes()
        {
            return ColorSchemes;
        }
        
        public static List<LpColorScheme> GetDefaultSchemes()
        {
            return new List<LpColorScheme>()
            {
                new LpColorScheme()
                {
                    Name = "Светлая",
                    Class = "color-scheme--light",
                    BackgroundColor = "rgb(255, 255, 255)",
                    BackgroundColorAlt = "rgba(242, 242, 242, 0.9)",

                    TitleColor = "rgb(0, 0, 0)",
                    TitleBold = "300",
                    SubTitleColor = "rgb(96, 96, 96)",
                    SubTitleBold = "300",
                    TextColor = "rgb(0, 0, 0)",
                    TextColorAlt = "rgb(0, 0, 0)",
                    TextBold = "300",

                    LinkColor = "rgb(23, 121, 250)",
                    LinkColorHover = "rgb(4, 89, 200)",
                    LinkColorActive = "rgb(23, 121, 250)",

                    ButtonTextColor = "rgb(255, 255, 255)",
                    ButtonTextColorHover = "rgb(255, 255, 255)",
                    ButtonTextColorActive = "rgb(255, 255, 255)",
                    ButtonBorderColor = "rgb(23, 121, 250)",
                    ButtonBorderWidth = "1px",
                    ButtonBorderRadius = "4px",
                    ButtonTextBold = "400",

                    ButtonBackgroundColor = "rgb(23, 121, 250)",
                    ButtonBackgroundColorHover = "rgb(55, 140, 251)",
                    ButtonBackgroundColorActive = "rgb(23, 121, 250)",

                    ButtonSecondaryTextColor = "rgb(0, 0, 0)",
                    ButtonSecondaryTextColorHover = "rgb(0, 0, 0)",
                    ButtonSecondaryTextColorActive = "rgb(0, 0, 0)",
                    ButtonSecondaryBorderColor = "rgb(218, 218, 218)",
                    ButtonSecondaryBorderWidth = "1px",
                    ButtonSecondaryBorderRadius = "4px",
                    ButtonSecondaryTextBold = "400",

                    ButtonSecondaryBackgroundColor = "rgb(255, 255, 255)",
                    ButtonSecondaryBackgroundColorHover = "rgba(0, 0, 0, 0.1)",
                    ButtonSecondaryBackgroundColorActive = "rgba(0, 0, 0, 0.2)",

                    DelimiterColor = "rgb(226, 226, 226)"
                },
                new LpColorScheme()
                {
                    Name = "Умеренная",
                    Class = "color-scheme--medium",
                    BackgroundColor = "rgb(248, 248, 248)",
                    BackgroundColorAlt = "rgb(255, 255, 255)",

                    TitleColor = "rgb(0, 0, 0)",
                    TitleBold = "300",
                    SubTitleColor = "rgb(96, 96, 96)",
                    SubTitleBold = "300",
                    TextColor = "rgb(0, 0, 0)",
                    TextColorAlt = "rgb(0, 0, 0)",
                    TextBold = "300",

                    LinkColor = "rgb(23, 121, 250)",
                    LinkColorHover = "rgb(4, 89, 200)",
                    LinkColorActive = "rgb(23, 121, 250)",

                    ButtonTextColor = "rgb(255, 255, 255)",
                    ButtonTextColorHover = "rgb(255, 255, 255)",
                    ButtonTextColorActive = "rgb(255, 255, 255)",
                    ButtonBorderColor = "rgb(23, 121, 250)",
                    ButtonBorderWidth = "1px",
                    ButtonBorderRadius = "4px",
                    ButtonTextBold = "400",

                    ButtonBackgroundColor = "rgb(23, 121, 250)",
                    ButtonBackgroundColorHover = "rgb(55, 140, 251)",
                    ButtonBackgroundColorActive = "rgb(23, 121, 250)",

                    ButtonSecondaryTextColor = "rgb(0, 0, 0)",
                    ButtonSecondaryTextColorHover = "rgb(0, 0, 0)",
                    ButtonSecondaryTextColorActive = "rgb(0, 0, 0)",
                    ButtonSecondaryBorderColor = "rgb(218, 218, 218)",
                    ButtonSecondaryBorderWidth = "1px",
                    ButtonSecondaryBorderRadius = "4px",
                    ButtonSecondaryTextBold = "400",

                    ButtonSecondaryBackgroundColor = "rgb(255, 255, 255)",
                    ButtonSecondaryBackgroundColorHover = "rgba(0, 0, 0, 0.1)",
                    ButtonSecondaryBackgroundColorActive = "rgba(0, 0, 0, 0.2)",

                    DelimiterColor = "rgb(205, 205, 205)"
                },
                new LpColorScheme()
                {
                    Name = "Темная",
                    Class = "color-scheme--dark",
                    BackgroundColor = "rgb(0, 0, 0)",
                    BackgroundColorAlt = "rgba(60, 60, 60, 1)",

                    TitleColor = "rgb(255, 255, 255)",
                    TitleBold = "300",
                    SubTitleColor = "rgb(232, 232, 232)",
                    SubTitleBold = "300",
                    TextColor = "rgb(255, 255, 255)",
                    TextColorAlt = "rgb(255, 255, 255)",
                    TextBold = "300",

                    LinkColor = "rgb(255, 255, 255)",
                    LinkColorHover = "rgb(204, 204, 204)",
                    LinkColorActive = "rgb(255, 255, 255)",

                    ButtonTextColor = "rgb(255, 255, 255)",
                    ButtonTextColorHover = "rgb(255, 255, 255)",
                    ButtonTextColorActive = "rgb(255, 255, 255)",
                    ButtonBorderColor = "rgb(23, 121, 250)",
                    ButtonBorderWidth = "1px",
                    ButtonBorderRadius = "4px",
                    ButtonTextBold = "400",

                    ButtonBackgroundColor = "rgb(23, 121, 250)",
                    ButtonBackgroundColorHover = "rgb(55, 140, 251)",
                    ButtonBackgroundColorActive = "rgb(23, 121, 250)",

                    ButtonSecondaryTextColor = "rgb(255, 255, 255)",
                    ButtonSecondaryTextColorHover = "rgb(255, 255, 255)",
                    ButtonSecondaryTextColorActive = "rgb(255, 255, 255)",
                    ButtonSecondaryBorderColor = "rgb(255, 255, 255)",
                    ButtonSecondaryBorderWidth = "1px",
                    ButtonSecondaryBorderRadius = "4px",
                    ButtonSecondaryTextBold = "400",

                    ButtonSecondaryBackgroundColor = "rgba(255, 255, 255, 0)",
                    ButtonSecondaryBackgroundColorHover = "rgba(255, 255, 255, 0.2)",
                    ButtonSecondaryBackgroundColorActive = "rgba(255, 255, 255, 0.3)",

                    DelimiterColor = "rgb(255, 255, 255)"
                }
            };
        }

        public static List<LpFont> GetDefaultFonts()
        {
            return new List<LpFont>()
            {
                new LpFont()
                {
                    Name = "Open Sans",
                    FontFamily = "'OpenSans'"
                },
                new LpFont()
                {
                    Name = "Roboto",
                    FontFamily = "'Roboto'"
                },
                new LpFont()
                {
                    Name = "Playfair Display",
                    FontFamily = "'Playfair Display'"
                },
                new LpFont()
                {
                    Name = "Cormorant",
                    FontFamily = "'Cormorant'"
                },
                new LpFont()
                {
                    Name = "Montserrat",
                    FontFamily = "'Montserrat'"
                },
                new LpFont()
                {
                    Name = "Merriweather",
                    FontFamily = "'Merriweather'"
                },
                new LpFont()
                {
                    Name = "Ubuntu",
                    FontFamily = "'Ubuntu'"
                },
                new LpFont()
                {
                    Name = "Alegreya Sans SC",
                    FontFamily = "'Alegreya Sans SC'"
                },
                new LpFont()
                {
                    Name = "Spectral SC",
                    FontFamily = "'Spectral SC'"
                },
                new LpFont()
                {
                    Name = "Georgia",
                    FontFamily = "'Georgia'",
                },
                new LpFont()
                {
                    Name = "Arial",
                    FontFamily = "'Arial'",
                },
                new LpFont()
                {
                    Name = "Tahoma",
                    FontFamily = "'Tahoma'",
                },
                new LpFont()
                {
                    Name = "Roboto Condensed",
                    FontFamily = "'Roboto Condensed'"
                },
                new LpFont()
                {
                    Name = "Roboto Slab",
                    FontFamily = "'Roboto Slab'"
                },
                new LpFont()
                {
                    Name = "Rubik",
                    FontFamily = "'Rubik'"
                },
                new LpFont()
                {
                    Name = "IBM Plex Sans",
                    FontFamily = "'IBM Plex Sans'"
                },
                new LpFont()
                {
                    Name = "Scada",
                    FontFamily = "'Scada'"
                },
                new LpFont()
                {
                    Name = "Inter",
                    FontFamily = "'Inter'"
                },
                new LpFont()
                {
                    Name = "PT Sans",
                    FontFamily = "'PT Sans'"
                },
                new LpFont()
                {
                    Name = "PT Serif",
                    FontFamily = "'PT Serif'"
                },
                new LpFont()
                {
                    Name = "Oswald",
                    FontFamily = "'Oswald'"
                }
            };
        }
    }
}
