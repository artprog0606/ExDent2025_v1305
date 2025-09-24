using AdvantShop.CDN;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace AdvantShop.Design
{
    public enum eDesign
    {
        Theme,
        Color,
        Background
    }

    public static class DesignService
    {
        private const int DesignCacheTime = 60;

        public static readonly Dictionary<eDesign, string> TypeAndPath = new Dictionary<eDesign, string>()
        {
            { eDesign.Background, "backgrounds" },
            { eDesign.Theme, "themes" },
            { eDesign.Color, "colors" },
        };

        public static string TemplatePath
        {
            get
            {
                return SettingsDesign.Template != TemplateService.DefaultTemplateId
                    ? ("Templates/" + SettingsDesign.Template + "/")
                    : "";
            }
        }

        public static List<Theme> GetDesigns(eDesign design, bool withCache = true)
        {
            if (withCache)
            {
                var cacheName = CacheNames.GetDesignCacheObjectName(design.ToString()) + SettingsDesign.Template;
                return CacheManager.Get(cacheName, DesignCacheTime, () => GetDesignsFromConfig(design));
            }

            return GetDesignsFromConfig(design, true);
        }

        private static List<Theme> GetDesignsFromConfig(eDesign design, bool fromDb = false)
        {
            var themes = new List<Theme>();
            var temp = new List<Theme>();

            var tmplPath = (!fromDb && SettingsDesign.Template != TemplateService.DefaultTemplateId) ||
                           (fromDb && SettingsDesign.TemplateInDb != TemplateService.DefaultTemplateId)
                ? "Templates\\" + (fromDb ? SettingsDesign.TemplateInDb : SettingsDesign.Template) + "\\"
                : "";

            var designPath = SettingsGeneral.AbsolutePath + tmplPath + "design\\" + TypeAndPath[design];

            if (Directory.Exists(designPath))
            {
                foreach (var configPath in Directory.GetDirectories(designPath))
                {
                    var themeName = configPath.Split('\\').Last();
                    var themeConfig = configPath + "\\" + design.ToString() + ".config";

                    try
                    {
                        Theme theme = null;

                        if (File.Exists(themeConfig))
                        {
                            using (var myReader = new StreamReader(themeConfig))
                            {
                                var mySerializer = new XmlSerializer(typeof(Theme));
                                theme = (Theme)mySerializer.Deserialize(myReader);

                                var themeTitle = theme.Names.Find(t => t.Lang == SettingsMain.Language);
                                theme.Title = themeTitle != null ? themeTitle.Value : themeName;
                                theme.Name = themeName;
                                theme.PreviewImage = theme.PreviewImage.IsNotEmpty() &&
                                                     File.Exists(configPath + "\\" + theme.PreviewImage)
                                    ? UrlService.GetAbsoluteLink(tmplPath + "design\\" + TypeAndPath[design] + "\\" +
                                                                 themeName + "\\" + theme.PreviewImage)
                                    : null;
                                myReader.Close();
                            }
                        }
                        else
                        {
                            theme = new Theme()
                            {
                                Title = themeName,
                                Name = themeName,
                            };
                        }

                        if (themeName != "_none")
                            temp.Add(theme);
                        else
                            themes.Add(theme);
                    }
                    catch (Exception ex)
                    {
                        Debug.Log.Error(ex);
                    }
                }
            }

            themes.AddRange(temp.OrderBy(t => t.Title));
            return themes.OrderBy(y => y.Order).ToList();
        }

        public static Theme GetCurrenDesign(eDesign design, bool fromDb = false)
        {
            var tmplPath = (!fromDb && SettingsDesign.Template != TemplateService.DefaultTemplateId) ||
                           (fromDb && SettingsDesign.TemplateInDb != TemplateService.DefaultTemplateId)
                ? "Templates\\" + (fromDb ? SettingsDesign.TemplateInDb : SettingsDesign.Template) + "\\"
                : "";

            string themeName = "_none";
            switch (design)
            {
                case eDesign.Color:
                    themeName = SettingsDesign.ColorScheme;
                    break;

                case eDesign.Theme:
                    themeName = SettingsDesign.Theme;
                    break;

                case eDesign.Background:
                    themeName = SettingsDesign.Background;
                    break;
            }

            var designPath = SettingsGeneral.AbsolutePath + tmplPath + "design\\" + TypeAndPath[design] + "\\" +
                             themeName;
            var themeConfig = designPath + "\\" + design.ToString() + ".config";
            Theme theme = null;

            try
            {
                if (File.Exists(themeConfig))
                {
                    using (var myReader = new StreamReader(themeConfig))
                    {
                        var mySerializer = new XmlSerializer(typeof(Theme));
                        theme = (Theme)mySerializer.Deserialize(myReader);

                        var themeTitle = theme.Names.Find(t => t.Lang == SettingsMain.Language);
                        theme.Title = themeTitle != null ? themeTitle.Value : themeName;
                        theme.Name = themeName;
                        theme.PreviewImage = theme.PreviewImage.IsNotEmpty() &&
                                             File.Exists(designPath + "\\" + theme.PreviewImage)
                            ? UrlService.GetAbsoluteLink(tmplPath + "design\\" + TypeAndPath[design] + "\\" +
                                                         themeName + "\\" + theme.PreviewImage)
                            : null;
                        myReader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            return theme;
        }

        public static string GetDesign(eDesign type, bool forMobileTemplate = false, string customValue = "")
        {
            var template = SettingsDesign.Template != TemplateService.DefaultTemplateId
                ? "templates/" + SettingsDesign.Template.ToLower() + "/"
                : "";

            if (forMobileTemplate && SettingsDesign.IsMobileTemplate && MobileHelper.IsMobileEnabled())
            {
                template = template + "areas/mobile/templates/" + SettingsDesign.MobileTemplate.ToLower() + "/";
            }

            switch (type)
            {
                case eDesign.Color:
                    return template + "design/colors/" + GetTypeDesign(TypeAndPath[type],
                        customValue.IsNotEmpty() ? customValue : SettingsDesign.ColorScheme);

                case eDesign.Theme:
                    return template + "design/themes/" + GetTypeDesign(TypeAndPath[type],
                        customValue.IsNotEmpty() ? customValue : SettingsDesign.Theme);

                case eDesign.Background:
                    return template + "design/backgrounds/" + GetTypeDesign(TypeAndPath[type],
                        customValue.IsNotEmpty() ? customValue : SettingsDesign.Background);

                default:
                    throw new Exception("Design type is undefined");
            }
        }

        public static string GetDesignUrlFile(eDesign type, bool forMobileTemplate = false, string customValue = "",
            bool useClassicMode = false)
        {
            string name = GetDesign(type, forMobileTemplate, customValue);
            string postfix = CDNService.UseCDN(eCDN.Design) ? ".cdn" : ".local";

            bool? isCustom = false;

            if (useClassicMode == false)
            {
                var cacheName = CacheNames.GetDesignCacheObjectName(type.ToString()) + "_isCustom_" +
                                SettingsDesign.Template;
                isCustom = CacheManager.Get(cacheName, DesignCacheTime, () =>
                {
                    var filePath = HttpContext.Current.Server.MapPath("~/" + name + $"/styles/styles{postfix}.css");
                    var design = GetCurrenDesign(type);
                    
                    return (design != null && design.Custom) || File.Exists(filePath) == false;
                });
            }
            
            return name +
                   $"/styles/styles{(useClassicMode || isCustom.Value ? "" : postfix)}.css";
        }

        private static string GetTypeDesign(string type, string currentType)
        {
            if (string.IsNullOrEmpty(currentType))
                currentType = "_none";

            if (!Demo.IsDemoEnabled || Customers.CustomerContext.CurrentCustomer.IsAdmin)
                return currentType;

            var styleCss = CommonHelper.GetCookieString(type);
            if (string.IsNullOrEmpty(styleCss))
            {
                CommonHelper.SetCookie(type, currentType);
                return currentType;
            }

            return styleCss.ToLower();
        }
    }
}