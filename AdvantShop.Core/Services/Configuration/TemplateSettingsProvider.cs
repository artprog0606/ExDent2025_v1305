//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Data;
using System.Data.SqlClient;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.SQL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;
using AdvantShop.Catalog;
using AdvantShop.CMS;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Services.CMS;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Design;
using AdvantShop.Diagnostics;
using AdvantShop.FilePath;
using AdvantShop.Helpers;

namespace AdvantShop.Configuration
{
    public class TemplateSettingsProvider
    {
        private const string TemplateFileConfigName = "template.config";

        public sealed class TemplateSettingIndexer
        {
            public string this[string name]
            {
                get => GetSettingValue(name);
                set => SetSettingValue(name, value);
            }
        }

        private static TemplateSettingIndexer _staticIndexer;
        public static TemplateSettingIndexer Items => _staticIndexer ?? (_staticIndexer = new TemplateSettingIndexer());

        #region Get/Set settings value

        public static string GetSettingValue(string key, string template = null)
        {
            var settings = GetAllTemplateSettings(template ?? SettingsDesign.Template);

            if (settings != null && settings.TryGetValue(key, out var value))
                return value;

            return null;
        }


        public static bool SetSettingValue(string name, string value, string template = null)
        {
            var tpl = template ?? SettingsDesign.Template;

            SQLDataAccess.ExecuteNonQuery(
                "IF (SELECT COUNT(*) FROM [Settings].[TemplateSettings] WHERE [Template] = @Template and [Name] = @Name) = 0" +
                    "BEGIN " +
                        "INSERT INTO [Settings].[TemplateSettings] ([Template], [Name], [Value]) VALUES (@Template, @Name, @Value) " +
                    "END " +
                "ELSE " +
                    "BEGIN " +
                        "UPDATE [Settings].[TemplateSettings] SET [Value] = @Value WHERE [Template] = @Template and [Name] = @Name " +
                    "END  ",
                CommandType.Text,
                new SqlParameter("@Template", tpl),
                new SqlParameter("@Name", name),
                new SqlParameter("@Value", value ?? (object)DBNull.Value));

            CacheManager.RemoveByPattern(CacheNames.GetTemplateSettings(tpl));

            return true;
        }

        public static void SaveSetting(TemplateSetting setting)
        {
            if (setting == null) return;

            switch (setting.Type)
            {
                case ETemplateSettingType.StaticBlockCheckbox:
                    var block = StaticBlockService.GetPagePartByKey(setting.Name);
                    if (block == null) return;
                            
                    block.Enabled = setting.Value.TryParseBool();
                    StaticBlockService.UpdatePagePart(block);
                    return;
                
                case ETemplateSettingType.TextBox:
                    if (setting.Value == null) setting.Value = "";
                    
                    SetSettingValue(setting.Name, setting.Value);
                    return;
                
                default:
                    if (setting.Value == null) return;
                    
                    SetSettingValue(setting.Name, setting.Value);
                    return;
            }
        }
        
        public static void SaveSetting(string name, string value, string template = null)
        {
            if (value == null)
                return;
            
            SetSettingValue(name, value, template);
        }

        #endregion

        #region Get/Set settings service

        public static Dictionary<string, string> GetAllTemplateSettings(string template)
        {
            var settings = CacheManager.Get(CacheNames.GetTemplateSettings(template), 60,
                () =>
                    SQLDataAccess.ExecuteReadDictionary<string, string>(
                        "SELECT [Name],[Value] FROM [Settings].[TemplateSettings] Where Template = @Template",
                        CommandType.Text, "Name", "Value",
                        new SqlParameter("@Template", template)));

            return settings;
        }


        /// <summary>
        /// Get localized template settings
        /// </summary>
        /// <returns></returns>
        public static TemplateSettingBox GetTemplateSettingsBox()
        {
            var settingsBox = new TemplateSettingBox() { Settings = new List<TemplateSetting>() };
            
            var templateConfigFile =
                SettingsGeneral.AbsolutePath
                + (SettingsDesign.Template != TemplateService.DefaultTemplateId ? ("templates\\" + SettingsDesign.Template + "\\") : "App_Data\\")
                + TemplateFileConfigName;

            if (!File.Exists(templateConfigFile))
            {
                settingsBox.Message = LocalizationService.GetResource("Core.Configuration.TemplateSettings.ConfigNotExist");
                return settingsBox;
            }

            var allSettings = GetAllTemplateSettings(SettingsDesign.Template) ?? new Dictionary<string, string>();

            try
            {
                var settings = settingsBox.Settings;

                var doc = XDocument.Load(templateConfigFile);

                foreach (var elSection in doc.Root.Elements("SettingSection"))
                {
                    var sectionTitle = elSection.Attribute("Title").Value;
                    var sectionName = LocalizationService.GetResource("Core.Configuration.TemplateSettings_" + sectionTitle);
                    if (sectionName.Contains("Core.Configuration.TemplateSettings_"))
                        sectionName = sectionTitle;

                    if (elSection.Attribute("Hidden") != null && Convert.ToBoolean(elSection.Attribute("Hidden").Value))
                        continue;

                    foreach (var elSetting in elSection.Elements())
                    {
                        var name = elSetting.Attribute("Name").Value;
                        var type = elSetting.Attribute("Type") != null 
                            ? elSetting.Attribute("Type").Value.TryParseEnum<ETemplateSettingType>() 
                            : ETemplateSettingType.TextBox;
                        var value = elSetting.Element("Value") != null ? elSetting.Element("Value").Value : "";
                        if (type == ETemplateSettingType.StaticBlockCheckbox)
                        {
                            var block = StaticBlockService.GetPagePartByKey(name);
                            value = block != null
                                ? block.Enabled.ToLowerString()
                                : value;
                        }
                        else
                        {
                            value = allSettings.ContainsKey(name)
                                ? allSettings[name]
                                : value;
                        }

                        var setting = new TemplateSetting
                        {
                            Name = name,
                            Value = value,
                            Type = type,
                            SectionName = sectionName,
                            DataType = elSetting.Attribute("DataType") != null ? elSetting.Attribute("DataType").Value : "string",
                            IsAdditional = elSetting.Attribute("IsAdditional") != null && elSetting.Attribute("IsAdditional").Value.TryParseBool()
                        };

                        var titleEl = elSetting.Attribute("Title");

                        setting.Title = "";

                        if (titleEl != null)
                        {
                            setting.Title = LocalizationService.GetResource("Core.Configuration.TemplateSettings_" + titleEl.Value);
                            if (string.IsNullOrEmpty(setting.Title) || setting.Title.Contains("Core.Configuration.TemplateSettings_"))
                            {
                                setting.Title = titleEl.Value;
                            }
                        }
                        else
                        {
                            setting.Title = LocalizationService.GetResource("Core.Configuration.TemplateSettings_" + setting.Name);
                            if (string.IsNullOrEmpty(setting.Title) || setting.Title.Contains("Core.Configuration.TemplateSettings_"))
                            {
                                setting.Title = setting.Name;
                            }
                        }

                        if (elSetting.Attribute("Name") != null)
                        {
                            var resourceKey = "Core.Configuration.TemplateSettings_" + elSetting.Attribute("Name").Value +
                                              "_Description";

                            var localizedString = LocalizationService.GetResource(resourceKey);

                            setting.Description = localizedString.ToLower() != resourceKey.ToLower()
                                                    ? localizedString
                                                    : string.Empty;
                        }

                        var options = new List<TemplateOptionSetting>();
                        foreach (var elOption in elSetting.Elements("option"))
                        {
                            var titleValue = elOption.Attribute("Title").Value;
                            var title = LocalizationService.GetResource("Core.Configuration.TemplateSettings_" + titleValue);
                            if (string.IsNullOrEmpty(title) || title.Contains("Core.Configuration.TemplateSettings_"))
                                title = titleValue;

                            options.Add(new TemplateOptionSetting
                            {
                                Title = title,
                                Value = elOption.Attribute("Value").Value,
                                Image = elOption.Attribute("Image") != null ? elOption.Attribute("Image").Value : null
                            });
                        }
                        setting.Options = options;
                        settings.Add(setting);
                    }
                }
            }
            catch (Exception ex)
            {
                settingsBox.Message = LocalizationService.GetResource("Core.Configuration.TemplateSettings.ErrorReadConfig");
                Debug.Log.Error(ex);
            }

            return settingsBox;
        }
        
        public static TemplateSetting TryGetTemplateSetting(string name, List<TemplateSetting> settings) =>
            settings.Find(setting => setting.Name == name);
        
        public static string TryGetTemplateSettingValue(string name, List<TemplateSetting> settings) => 
            TryGetTemplateSetting(name, settings)?.Value;
        
        public static List<SelectListItem> GetTemplateOptions(string name, List<TemplateSetting> settings) =>
            TryGetTemplateSetting(name, settings)?.Options
                .Select(x => new SelectListItem()
                {
                    Text = x.Title,
                    Value = x.Value
                }).ToList() 
            ?? new List<SelectListItem>();
        
        public static List<string> GetHiddenSettings()
        {
            var hiddenSettings = new List<string>();

            var templateConfigFile =
                SettingsGeneral.AbsolutePath
                + (SettingsDesign.Template != TemplateService.DefaultTemplateId ? ("templates\\" + SettingsDesign.Template + "\\") : "App_Data\\")
                + TemplateFileConfigName;

            if (!File.Exists(templateConfigFile))
                return hiddenSettings;

            try
            {
                var doc = XDocument.Load(templateConfigFile);

                foreach (var settings in doc.Root.Elements("HiddenSettings"))
                    foreach (var setting in settings.Elements("HiddenSetting"))
                    {
                        var nameAttr = setting.Attribute("Name");
                        if (nameAttr != null && !string.IsNullOrEmpty(nameAttr.Value))
                        {
                            hiddenSettings.Add(nameAttr.Value);
                        }
                    }
            }
            catch (Exception ex)
            {
                Debug.Log.Warn(ex);
            }

            return hiddenSettings;
        }

        /// <summary>
        /// Set template settings from config if not exist
        /// </summary>
        /// <param name="template">Template name</param>
        /// <param name="setDefault">Rewrite setting value</param>
        /// <returns></returns>
        public static TemplateSettingBox SetTemplateSettings(string template = null, bool setDefault = false)
        {
            if (template == null)
                template = SettingsDesign.Template;

            var settingsBox = new TemplateSettingBox();

            var templateConfigFile = SettingsGeneral.AbsolutePath
                                     + (template != TemplateService.DefaultTemplateId
                                         ? "templates\\" + template + "\\"
                                         : "App_Data\\")
                                     + TemplateFileConfigName;

            if (!File.Exists(templateConfigFile))
            {
                settingsBox.Message = LocalizationService.GetResource("Core.Configuration.TemplateSettings.ConfigNotExist");
                return settingsBox;
            }

            try
            {
                var doc = XDocument.Load(templateConfigFile);

                foreach (var elSection in doc.Root.Elements("SettingSection"))
                {
                    foreach (var elSetting in elSection.Elements())
                    {
                        var name = elSetting.Attribute("Name")?.Value;
                        var value = elSetting.Element("Value") != null
                            ? elSetting.Element("Value")?.Value
                            : "";

                        var type = elSetting.Attribute("Type")?.Value;
                        if (!type.IsNullOrEmpty() && string.Equals(type, "CoreSetting", StringComparison.OrdinalIgnoreCase))
                        {
                            SettingProvider.Items[name] = value;
                        }
                        
                        if (setDefault || GetSettingValue(name, template).IsNullOrEmpty())
                            SetSettingValue(name, value, template);
                    }
                }

                InstallStaticBlocks(doc);

                InstallLogo(doc, template);
                InstallCarouselSlides(doc, template);

                AfterInstall();
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                settingsBox.Message = LocalizationService.GetResource("Core.Configuration.TemplateSettings.ErrorReadConfig");
            }

            return settingsBox;
        }

        private static void InstallStaticBlocks(XDocument doc)
        {
            var sbBlock = doc.Root.Element("StaticBlocks");
            if (sbBlock == null)
                return;

            foreach (var block in sbBlock.Elements("StaticBlock"))
            {
                var key = block.Attribute("Key");
                var name = block.Attribute("Name");

                if (key == null || name == null || 
                    string.IsNullOrWhiteSpace(key.Value) || string.IsNullOrWhiteSpace(name.Value) ||
                    StaticBlockService.GetPagePartByKey(key.Value) != null)
                {
                    continue;
                }

                var sb = new StaticBlock()
                {
                    Key = key.Value,
                    InnerName = name.Value,
                    Added = DateTime.Now,
                    Modified = DateTime.Now,
                    Content = block.Value ?? ""
                };

                var enabled = block.Attribute("Enabled");
                sb.Enabled = enabled == null || enabled.Value.TryParseBool();

                StaticBlockService.AddStaticBlock(sb);
            }
        }

        private static void InstallLogo(XDocument doc, string template)
        {
            var isPreview = SettingsDesign.PreviewTemplate != null;

            if (!SettingsMain.IsDefaultLogo && !string.IsNullOrWhiteSpace(SettingsMain.LogoImageName) && !isPreview)
                return;
            
            try
            {
                var logoEl = doc.Root.Element("LogoPicture");
                if (logoEl == null)
                    return;

                var pathAttr = logoEl.Attribute("Path");
                if (pathAttr == null || string.IsNullOrWhiteSpace(pathAttr.Value))
                    return;

                var logoPath = SettingsGeneral.AbsolutePath + "templates\\" + template + "\\" + pathAttr.Value;

                if (!File.Exists(logoPath))
                    return;

                SettingsDesign.DefaultLogo = "templates/" + template + "/" + pathAttr.Value;

                if (isPreview)
                    return;

                //FileHelpers.DeleteFile(FoldersHelper.GetPathAbsolut(FolderType.Pictures, SettingsMain.LogoImageName));

                var logoName = pathAttr.Value.Split(new char[] {'\\', '/'}).LastOrDefault();

                var newName = logoName.FileNamePlusDate("logo");
                var newPath = FoldersHelper.GetPathAbsolut(FolderType.Pictures, newName);

                if (!File.Exists(newPath))
                    File.Copy(logoPath, newPath);

                SettingsMain.LogoImageName = newName;
                SettingsMain.IsDefaultLogo = true;

                try
                {
                    if (FileHelpers.CheckFileExtensionByType(newName, EFileType.Image))
                    {
                        var img = System.Drawing.Image.FromFile(logoPath, true);
                        SettingsMain.LogoImageWidth = img.Width;
                        SettingsMain.LogoImageHeight = img.Height;
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log.Error(ex);
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }
        }

        private static void InstallCarouselSlides(XDocument doc, string template)
        {
            var isPreview = SettingsDesign.PreviewTemplate != null;
            var allSlides = CarouselService.GetAllCarousels();

            if (!SettingsDesign.IsDefaultSlides && allSlides.Count > 0 && !isPreview)
                return;

            try
            {
                var carouselEl = doc.Root.Element("Carousel");
                if (carouselEl == null)
                    return;
                
                var slides = new List<CarouselSlide>();

                foreach (var slideEl in carouselEl.Elements("Slide"))
                {
                    var path = slideEl.Attribute("Path");
                    if (path == null || string.IsNullOrEmpty(path.Value))
                        continue;

                    var displayInMobile = slideEl.Attribute("DisplayInMobile");
                    var displayInOneColumn = slideEl.Attribute("DisplayInOneColumn");
                    var displayInTwoColumns = slideEl.Attribute("DisplayInTwoColumns");

                    slides.Add(new CarouselSlide()
                    {
                        Url = path.Value,
                        DisplayInMobile =
                            displayInMobile != null && !string.IsNullOrEmpty(displayInMobile.Value)
                                ? displayInMobile.Value.TryParseBool()
                                : true,

                        DisplayInOneColumn = displayInOneColumn != null && !string.IsNullOrEmpty(displayInOneColumn.Value)
                                ? displayInOneColumn.Value.TryParseBool()
                                : true,

                        DisplayInTwoColumns = displayInTwoColumns != null && !string.IsNullOrEmpty(displayInTwoColumns.Value)
                                ? displayInTwoColumns.Value.TryParseBool()
                                : true,
                    });
                }

                SettingsDesign.DefaultSlides = String.Join(";", slides.Select(x => x.Url));

                if (isPreview || slides.Count == 0)
                    return;

                var maxSortOrder = Int32.MinValue;

                foreach (var slide in allSlides)
                {
                    var existedSlide = slides.Find(x => x.Url == slide.Picture.PhotoName);

                    if (slide.Picture != null && existedSlide != null)
                    {
                        slide.Enabled = true;
                        slides.Remove(existedSlide);
                    }
                    else
                        slide.Enabled = false;

                    CarouselService.UpdateCarousel(slide);

                    if (maxSortOrder < slide.SortOrder)
                        maxSortOrder = slide.SortOrder;
                }

                for(var i = 0; i < slides.Count; i++)
                {
                    var carousel = new Carousel()
                    {
                        Url = "",
                        DisplayInMobile = slides[i].DisplayInMobile,
                        DisplayInOneColumn = slides[i].DisplayInOneColumn,
                        DisplayInTwoColumns = slides[i].DisplayInTwoColumns,
                        Enabled = true,
                        SortOrder = maxSortOrder + i*10
                    };
                    CarouselService.AddCarousel(carousel);

                    PhotoService.AddPhotoWithOrignName(new Photo(0, carousel.CarouselId, PhotoType.Carousel){PhotoName = slides[i].Url});
                }

                SettingsDesign.IsDefaultSlides = true;
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }
        }


        private static void AfterInstall()
        {
            var mobileBrowserColorVariantsSelected = SettingsMobile.BrowserColorVariantsSelected.TryParseEnum(SettingsMobile.eBrowserColorVariants.ColorScheme);
            if (mobileBrowserColorVariantsSelected == SettingsMobile.eBrowserColorVariants.ColorScheme)
            {
                var curColorScheme = DesignService.GetCurrenDesign(eDesign.Color);
                SettingsMobile.BrowserColor = curColorScheme.Color;
            }
            
            CommonHelper.DeleteCookie("mobile_viewmode");
        }

        private class CarouselSlide
        {
            public string Url { get; set; }
            public bool DisplayInMobile { get; set; }
            public bool DisplayInOneColumn { get; set; }
            public bool DisplayInTwoColumns { get; set; }
        }

        #endregion
    }
}