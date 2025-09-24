using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Net;
using System.Threading;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Localization;
using Newtonsoft.Json;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.DownloadableContent;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.SalesChannels;

namespace AdvantShop.Design
{
    public class TemplateService
    {
        public const string DefaultTemplateId = "_default";
        public const string TemplateCacheKey = "TemplateCacheKey_";

        private const string RequestUrlGetTemplates = "http://modules.advantshop.net/DownloadableContent/GetDlcs?id={0}&dlctype=Template&storeversion={1}";
        private const string RequestUrlGetTemplateArchive = "http://modules.advantshop.net/DownloadableContent/GetDlc?lickey={0}&dlcId={1}&storeversion={2}&forpreview={3}";

        public static DownloadableContentBox GetTemplates()
        {
            var templatesFromServer = GetTemplatesFromRemoteServer();

            var store = SalesChannelService.GetByType(ESalesChannelType.Store);
            var isStoreEnabled = store != null && store.Enabled;

            templatesFromServer.Items.Insert(0, new DownloadableContentObject()
            {
                StringId = DefaultTemplateId,
                Name = LocalizationService.GetResource("Core.Design.Template.DefaultTemplate"),
                IsInstall = isStoreEnabled, //SettingsDesign.Template == DefaultTemplateId,
                Active = true,
                Icon = "http://advantshop.net/demo-standart.jpg",
                OnlineDemoLink = "http://advantshop.net/demo-standart",
                Description = LocalizationService.GetResource("Core.Design.Template.DefaultTemplateDescription"),
                SortOrder = 70
            });

            if (Directory.Exists(SettingsGeneral.AbsolutePath + "Templates"))
            {
                foreach (var templateFolder in Directory.GetDirectories(SettingsGeneral.AbsolutePath + "Templates"))
                {
                    if (!File.Exists(templateFolder + "\\template.config"))
                        continue;

                    var stringId = templateFolder.Split('\\').Last();
                    var curTemplate = templatesFromServer.Items.Find(t => t.StringId.ToLower() == stringId.ToLower());

                    if (curTemplate != null)
                    {
                        var templateFromDb = DownloadableContentService.GetOne(stringId);

                        curTemplate.IsInstall = !isStoreEnabled ? false : templateFromDb?.IsInstall ?? false;
                        curTemplate.CurrentVersion = templateFromDb != null ? templateFromDb.Version : LocalizationService.GetResource("Core.Design.Template.InDebug");
                        curTemplate.IsCustomVersion = File.Exists(System.Web.Hosting.HostingEnvironment.MapPath("~/Templates/" + stringId + "/custom.txt"));
                    }
                    else
                    {
                        var templateFromDb = DownloadableContentService.GetOne(stringId);

                        var version = templateFromDb != null
                            ? templateFromDb.Version
                            : LocalizationService.GetResource("Core.Design.Template.InDebug");

                        templatesFromServer.Items.Insert(0, new DownloadableContentObject
                        {
                            StringId = stringId,
                            Name = stringId,
                            IsInstall = templateFromDb?.IsInstall ?? false,
                            Icon = $"../Templates/{stringId}/preview.jpg",
                            Active = true,
                            Price = 0,
                            CurrentVersion = version,
                            Version = version,
                            IsLocalVersion = true,
                            IsCustomVersion = File.Exists(System.Web.Hosting.HostingEnvironment.MapPath("~/Templates/" + stringId + "/custom.txt"))
                        });
                    }
                }
            }

            var resultTemplateBox = new DownloadableContentBox()
            {
                Message = templatesFromServer.Message,
                Items = new List<DownloadableContentObject>()
            };

            if (isStoreEnabled)
            {
                var current = templatesFromServer.Items.FirstOrDefault(t => t.StringId == SettingsDesign.Template);
                if (current != null)
                    resultTemplateBox.Items.Add(current);
                
                resultTemplateBox.Items.AddRange(templatesFromServer.Items.Where(t => t.StringId != SettingsDesign.Template));
            }
            else
            {
                resultTemplateBox.Items.AddRange(templatesFromServer.Items.OrderBy(x => x.SortOrder));
            }

            return resultTemplateBox;
        }

        private static DownloadableContentBox GetTemplatesFromRemoteServer()
        {
            var box = CacheManager.Get(TemplateCacheKey + "remote", 15, () =>
            {
                var templateBox = new DownloadableContentBox() {Items = new List<DownloadableContentObject>()};

                try
                {
                    var request = WebRequest.Create(string.Format(RequestUrlGetTemplates, SettingsLic.LicKey, SettingsGeneral.SiteVersionDev));
                    request.Method = "GET";

                    using (var dataStream = request.GetResponse().GetResponseStream())
                    using (var reader = new StreamReader(dataStream))
                    {
                        var responseFromServer = reader.ReadToEnd();
                        if (!string.IsNullOrEmpty(responseFromServer))
                        {
                            templateBox = JsonConvert.DeserializeObject<DownloadableContentBox>(responseFromServer);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log.Error(ex);
                }

                return templateBox;
            });

            return box.DeepCloneJson();
        }


        public static bool IsExistTemplate(string templateId)
        {
            if (SettingsDesign.Template == DefaultTemplateId)
                return true;
            return Directory.Exists(SettingsGeneral.AbsolutePath + "Templates\\" + templateId + "\\template.config");
        }

        public static bool UninstallTemplate(string stringId)
        {
            if (stringId == DefaultTemplateId)
                return false;

            try
            {

                var templatepath = HttpContext.Current.Server.MapPath("~/Templates/" + stringId);

                foreach (var file in Directory.GetFiles(templatepath))
                {
                    FileHelpers.DeleteFile(file);
                }

                foreach (var directory in Directory.GetDirectories(templatepath).Where(d => !d.ToLower().EndsWith("design")))
                {
                    FileHelpers.DeleteDirectory(directory);
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }
            finally
            {
                DownloadableContentService.Uninstall(stringId, "template");
                CacheManager.Clean();
            }

            return !DownloadableContentService.IsInstall(stringId);
        }

        public static bool InstallTemplate(int id, string stringid, bool isPreview)
        {
            if (stringid == DefaultTemplateId)
                return true;

            var templateConfig = HttpContext.Current.Server.MapPath("~/Templates/" + stringid + "/template.config");

            if (File.Exists(templateConfig))
            {
                DownloadableContentService.Install(
                    new DownloadableContentObject
                    {
                        StringId = stringid,
                        Version = "В режиме отладки",
                        IsInstall = true,
                        DcType = "template",
                        Active = !isPreview
                    });
                CacheManager.Clean();
                return DownloadableContentService.IsInstall(stringid);
            }

            var message = GetTemplateArchiveFromRemoteServer(id, isPreview);
            if (string.IsNullOrEmpty(message))
            {
                var templatesFromServer = GetTemplatesFromRemoteServer();
                var templateInfoFromServer = new DownloadableContentObject();

                if (templatesFromServer != null &&
                    templatesFromServer.Items.Count > 0 &&
                    (templateInfoFromServer = templatesFromServer.Items.Find(t => t.StringId == stringid)) != null)
                {
                    DownloadableContentService.Install(
                        new DownloadableContentObject
                        {
                            Version = templateInfoFromServer.Version,
                            StringId = templateInfoFromServer.StringId,
                            IsInstall = true,
                            DcType = "template",
                            Active = templateInfoFromServer.Active
                        });
                    CacheManager.Clean();

                    return DownloadableContentService.IsInstall(templateInfoFromServer.StringId);
                }

            }
            return DownloadableContentService.IsInstall(stringid);
        }

        public static bool UpdateTemplate(int id, string stringId)
        {
            var templates = GetTemplates();

            var template = 
                templates.Items.Find(x => x.StringId.ToLower() == stringId.ToLower()) ??
                DownloadableContentService.GetOne(stringId);

            if (template == null || template.IsLocalVersion || template.IsCustomVersion)
                return false;
            
            var currentTemplateId = SettingsDesign.Template;
            var updatingCurrentTemplate = currentTemplateId.Equals(stringId, StringComparison.OrdinalIgnoreCase);

            if (updatingCurrentTemplate)
            {
                SettingsDesign.ChangeTemplate(DefaultTemplateId, false);
                Thread.Sleep(1000);
            }
            
            if (!UninstallTemplate(template.StringId))
                return false;

            var result = InstallTemplate(id, stringId, false);

            if (updatingCurrentTemplate)
            {
                SettingsDesign.ChangeTemplate(currentTemplateId, false);
            }

            return result;
        }

        private static string GetTemplateArchiveFromRemoteServer(int templateId, bool preview)
        {
            var zipFileName = HttpContext.Current.Server.MapPath("~/App_Data/" + Guid.NewGuid() + ".Zip");
            try
            {
                using (var wc = new WebClient())
                {
                    wc.DownloadFile(string.Format(RequestUrlGetTemplateArchive, SettingsLic.LicKey, templateId, SettingsGeneral.SiteVersionDev, preview), zipFileName);
                }

                if (!FileHelpers.UnZipFile(zipFileName, HttpContext.Current.Server.MapPath("~/Templates/")))
                {
                    if (File.Exists(zipFileName))
                        File.Delete(zipFileName);

                    return "error on UnZipFile";
                }

                FileHelpers.DeleteFile(zipFileName);
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                return "error on download or unzip";
            }

            return string.Empty;
        }
    }
}