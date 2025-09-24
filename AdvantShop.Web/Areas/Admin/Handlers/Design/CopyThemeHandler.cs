using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Design;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using System;
using System.IO;
using System.Text;
using System.Web.Hosting;
using System.Xml.Serialization;

namespace AdvantShop.Web.Admin.Handlers.Design
{
    public class CopyThemeHandler
    {
        private readonly Theme _theme;
        private readonly eDesign _design;
        private readonly string _newThemeTitle;
        private readonly string _themeCss;

        public CopyThemeHandler(Theme theme, eDesign design, string newThemeTitle, string themeCss)
        {
            _theme = theme;
            _design = design;
            _newThemeTitle = newThemeTitle;
            _themeCss = themeCss;
        }

        public string Execute()
        {
            var designFolderPath = SettingsDesign.Template != TemplateService.DefaultTemplateId
                ? HostingEnvironment.MapPath("~/Templates/" + SettingsDesign.Template + "/")
                : HostingEnvironment.MapPath("~/");

            var currentThemeFolderPath = string.Format("{0}design/{1}s/{2}", designFolderPath,
                _design, _theme.Name);

            var newThemeName = string.Format("{0}_custom_{1}", _theme.Name, DateTime.Now.Ticks);
            var newThemeFolderPath = string.Format("{0}design/{1}s/{2}", designFolderPath,
                _design, newThemeName);

            FileHelpers.CopyDirectory(currentThemeFolderPath, newThemeFolderPath, true, true);

            try
            {
                var newThemeConfigPath = string.Format("{0}/{1}.config", newThemeFolderPath, _design.ToString());
                if (File.Exists(newThemeConfigPath))
                {
                    Theme theme = null;
                    var serializer = new XmlSerializer(typeof(Theme));
                    using (var streamReader = new StreamReader(newThemeConfigPath))
                    {
                        theme = (Theme)serializer.Deserialize(streamReader);
                        streamReader.Close();
                    }

                    theme.Custom = true;
                    theme.Name = newThemeName;
                    theme.Names.ForEach(name => name.Value = _newThemeTitle);

                    using (var streamWriter = new StreamWriter(newThemeConfigPath, false, Encoding.UTF8))
                    {
                        var xmlNamespaces = new XmlSerializerNamespaces();
                        xmlNamespaces.Add("", "");
                        serializer.Serialize(streamWriter, theme, xmlNamespaces);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                return null;
            }

            switch (_design)
            {
                case eDesign.Theme:
                    SettingsDesign.ThemeInDb = newThemeName;
                    break;

                case eDesign.Background:
                    SettingsDesign.BackgroundInDb = newThemeName;
                    break;

                case eDesign.Color:
                    SettingsDesign.ColorSchemeInDb = newThemeName;
                    break;

                default:
                    break;
            }

            //обновляем css если его успели изменить
            var cssPath = string.Format("{0}/styles/styles.css", newThemeFolderPath);
            File.WriteAllText(cssPath, _themeCss, Encoding.UTF8);
            
            CacheManager.Clean();

            return newThemeName;
        }
    }
}