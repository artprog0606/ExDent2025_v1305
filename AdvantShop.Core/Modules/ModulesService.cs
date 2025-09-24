//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Saas;
using Newtonsoft.Json;
using AdvantShop.Core.Services.Mails;
using System.Collections.Generic;
using AdvantShop.Core.Services.Crm.SalesFunnels;
using AdvantShop.Core.Services.SalesChannels;
using AdvantShop.Customers;

namespace AdvantShop.Core.Modules
{
    public class ModulesService
    {
        private const string RequestUrlGetModules = "http://modules.advantshop.net/DownloadableContent/GetDlcs?id={0}&dlctype=Module&storeversion={1}";
        private const string RequestUrlGetModuleArchive = "http://modules.advantshop.net/DownloadableContent/GetDlc?lickey={0}&dlcId={1}&storeversion={2}&update={3}";
        private const string RequestUrlGetModuleObject = "http://modules.advantshop.net/DownloadableContent/GetDlcObject?id={0}&dlcStringId={1}&dlctype=Module";
        private const string RequestUrlDeleteModule = "http://modules.advantshop.net/DownloadableContent/DeleteDlc?lickey={0}&dlcStringId={1}";

        private const string ModulesCacheKey = "AdvantshopModules";
        private const string ModulesSalesChannelCacheKey = ModulesCacheKey + "_SalesChannels";
        private const string ModulesPaymentCacheKey = ModulesCacheKey + "_Payment";
        private const string ModulesShippingCacheKey = ModulesCacheKey + "_Shipping";

        #region Process modules from remote server

        public static ModuleBox GetModules()
        {
            var modules = GetModulesFromRemoteServer() ?? new ModuleBox();
            var existingModulesInDb = ModulesRepository.GetModulesFromDb();

            foreach (var attachedModuleType in AttachedModules.GetModules())
            {
                var attachedModuleInst = (IModule)Activator.CreateInstance(attachedModuleType);

                bool isMobileAdminReady = false;

                if (attachedModuleInst is IAdminModuleSettings moduleWithSettings)
                    isMobileAdminReady = moduleWithSettings.IsMobileAdminReady;

                var moduleFromRemoteServer = modules.Items.Count > 0
                    ? modules.Items.FirstOrDefault(x => string.Equals(x.StringId, attachedModuleInst.ModuleStringId, StringComparison.OrdinalIgnoreCase))
                    : null;

                var isCustom = File.Exists(System.Web.Hosting.HostingEnvironment.MapPath("~/modules/" + attachedModuleInst.ModuleStringId + "/custom.txt"));
                if (moduleFromRemoteServer != null)
                {
                    moduleFromRemoteServer.IsInstall = attachedModuleInst.CheckAlive() &&
                                                   ModulesRepository.IsInstallModule(attachedModuleInst.ModuleStringId);
                    moduleFromRemoteServer.Enabled = ModulesRepository.IsActiveModule(attachedModuleInst.ModuleStringId);
                    moduleFromRemoteServer.IsCustomVersion = isCustom;
                    moduleFromRemoteServer.IsMobileAdminReady = isMobileAdminReady;

                    if (existingModulesInDb.All(x => string.Equals(x.StringId, attachedModuleInst.ModuleStringId, StringComparison.OrdinalIgnoreCase) is false))
                    {
                        moduleFromRemoteServer.IsLocalVersion = true;
                        moduleFromRemoteServer.Version = moduleFromRemoteServer.CurrentVersion = LocalizationService.GetResource("Admin.Core.Modules.ModuleInDebug");
                    }
                }
                else
                {
                    modules.Items.Add(new Module
                    {
                        Name = attachedModuleInst.ModuleName,
                        StringId = attachedModuleInst.ModuleStringId,
                        Version = LocalizationService.GetResource("Admin.Core.Modules.ModuleInDebug"),
                        IsInstall =
                            attachedModuleInst.CheckAlive() && ModulesRepository.IsInstallModule(attachedModuleInst.ModuleStringId),
                        Price = 0,
                        IsLocalVersion = true,
                        Active = true,
                        Enabled = ModulesRepository.IsActiveModule(attachedModuleInst.ModuleStringId),
                        Icon =
                            File.Exists(System.Web.Hosting.HostingEnvironment.MapPath("~/modules/" + attachedModuleInst.ModuleStringId + "/icon.jpg"))
                                ? UrlService.GetUrl("modules/" + attachedModuleInst.ModuleStringId + "/icon.jpg")
                                : UrlService.GetUrl("images/nophoto_small.png"),//null
                        IsCustomVersion = isCustom,
                        IsPersonalModule = true,
                        IsMobileAdminReady = isMobileAdminReady
                    });
                }
            }

            if (existingModulesInDb.Count > 0)
            {
                foreach (var module in modules.Items)
                {
                    var moduleFromDb = existingModulesInDb.FirstOrDefault(x => string.Equals(x.StringId, module.StringId, StringComparison.OrdinalIgnoreCase));
                    if (moduleFromDb != null)
                    {
                        module.CurrentVersion = moduleFromDb.Version;
                        module.IsLocalVersion = module.CurrentVersion.Equals(LocalizationService.GetResource("Admin.Core.Modules.ModuleInDebug"));
                        module.IsInstall = moduleFromDb.IsInstall;
                        module.DateAdded = moduleFromDb.DateAdded;
                    }
                }
            }

            return modules;
        }

        private static ModuleBox GetModulesFromRemoteServer()
        {
            return CacheManager.Get(ModulesCacheKey, 5, () =>
            {
                var modules = new ModuleBox();

                try
                {
                    var url = string.Format(RequestUrlGetModules,
                        SaasDataService.IsSaasEnabled ? SettingsGeneral.CurrentSaasId : SettingsLic.LicKey,
                        SettingsGeneral.SiteVersionDev);

                    var request = WebRequest.Create(url);
                    request.Method = "GET";

                    using (var dataStream = request.GetResponse().GetResponseStream())
                    {
                        using (var reader = new StreamReader(dataStream))
                        {
                            var responseFromServer = reader.ReadToEnd();
                            if (!string.IsNullOrEmpty(responseFromServer))
                            {
                                modules = JsonConvert.DeserializeObject<ModuleBox>(responseFromServer);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log.Error(ex);
                }

                return modules;
            });
        }

        private static bool DeleteModuleFromRemoteServer(string stringId)
        {
            bool result = false;
            try
            {
                var url = string.Format(RequestUrlDeleteModule, SettingsLic.LicKey, stringId);

                var request = WebRequest.Create(url);
                request.Method = "GET";

                using (var dataStream = request.GetResponse().GetResponseStream())
                {
                    using (var reader = new StreamReader(dataStream))
                    {
                        var responseFromServer = reader.ReadToEnd();
                        if (!string.IsNullOrEmpty(responseFromServer))
                        {
                            result = JsonConvert.DeserializeObject<bool>(responseFromServer);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                result = false;
            }
            return result;
        }


        public static string GetModuleArchiveFromRemoteServer(string moduleId, string tempFolder = null, bool update = false)
        {
            var zipFileName = HttpContext.Current.Server.MapPath("~/App_Data/" + Guid.NewGuid() + ".Zip");
            try
            {
                new WebClient().DownloadFile(
                    string.Format(RequestUrlGetModuleArchive, SettingsLic.LicKey, moduleId, SettingsGeneral.SiteVersionDev, update),
                    zipFileName
                   );

                if (!FileHelpers.UnZipFile(zipFileName, tempFolder ?? HttpContext.Current.Server.MapPath("~/")))
                {
                    FileHelpers.DeleteFile(zipFileName);
                    return "error on UnZipFile";
                }
                FileHelpers.DeleteFile(zipFileName);
            }
            catch (WebException ex)
            {
                Debug.Log.Error(ex);
                if (ex.Status == WebExceptionStatus.ProtocolError)
                    return ((HttpWebResponse)ex.Response).StatusCode.ToString();
            }
            catch (Exception ex)    
            {
                Debug.Log.Error(ex);
                return "error on UnZipFile";
            }

            return string.Empty;
        }

        public static Module GetModuleObjectFromRemoteServer(string moduleStringId)
        {
            var url = string.Format(RequestUrlGetModuleObject,
                    SaasDataService.IsSaasEnabled ? SettingsGeneral.CurrentSaasId : SettingsLic.LicKey,
                    moduleStringId);

            var request = WebRequest.Create(url);
            request.Method = "GET";
            var module = new Module();
            using (var dataStream = request.GetResponse().GetResponseStream())
            {
                using (var reader = new StreamReader(dataStream))
                {
                    var responseFromServer = reader.ReadToEnd();
                    if (!string.IsNullOrEmpty(responseFromServer))
                    {
                        module = JsonConvert.DeserializeObject<Module>(responseFromServer);
                    }
                }
            }
            return module;
        }




        #endregion

        #region Public methods to process modules

        public static bool InstallModule(string moduleStringId, string version, bool setActive = true)
        {
            var moduleInst = AttachedModules.GetModuleById(moduleStringId.ToLower());
            if (moduleInst != null)
            {
                var module = ((IModule)Activator.CreateInstance(moduleInst, null));

                var isInstallModule = ModulesRepository.IsInstallModule(module.ModuleStringId);
                var result = !isInstallModule
                                ? module.InstallModule()
                                : module.UpdateModule();

                if (result)
                {
                    ModulesRepository.InstallModuleToDb(
                        new Module
                        {
                            StringId = module.ModuleStringId,
                            Name = module.ModuleName,
                            DateModified = DateTime.Now,
                            DateAdded = DateTime.Now,
                            Version = version,
                            Active = false,
                            NeedUpdate = isInstallModule ? true : false
                        });
                    
                    if (setActive)
                        ModulesRepository.SetActiveModule(module.ModuleStringId, true);
                    
                    return true;
                }
            }

            return false;
        }

        public static string UninstallModule(string moduleStringId)
        {
            if (string.IsNullOrWhiteSpace(moduleStringId))
                return "moduleStringId is empty";

            if (!DeleteModuleFromRemoteServer(moduleStringId))
            {
                return "Not deleted from remote server";
            }

            var moduleInst = AttachedModules.GetModuleById(moduleStringId);
            if (moduleInst != null)
            {
                var module = (IModule)Activator.CreateInstance(moduleInst);
                module.UninstallModule();

                FileHelpers.DeleteDirectory(HttpContext.Current.Server.MapPath("~/Modules/" + moduleStringId));
                FileHelpers.DeleteFile(HttpContext.Current.Server.MapPath("~/bin/" + moduleInst.Assembly.GetName().Name + ".dll")); // AdvantShop.Module." + moduleStringId + 
            }

            ModulesRepository.UninstallModuleFromDb(moduleStringId);

            return "";
        }

        public static void CallModulesUpdate()
        {
            var modulesInDb = ModulesRepository.GetModulesFromDb().Where(x => x.NeedUpdate).ToList();

            if (modulesInDb.Count == 0)
                return;

            foreach (var type in AttachedModules.GetModules<IModule>(ignoreActive: true))
            {
                try
                {
                    var module = (IModule)Activator.CreateInstance(type);

                    var moduleInDb = modulesInDb.FirstOrDefault(x => x.StringId.Equals(module.ModuleStringId, StringComparison.OrdinalIgnoreCase));
                    if (moduleInDb == null)
                        continue;

                    if (module.UpdateModule())
                        ModulesRepository.SetModuleNeedUpdate(moduleInDb.StringId, false);
                }
                catch (Exception ex)
                {
                    Debug.Log.Error(ex);
                }
            }
        }

        public static string GetModuleStringIdByUrlPath(string urlPath)
        {
            if (urlPath.IsNullOrEmpty())
                return string.Empty;

            var moduleInst = AttachedModules.GetModules<IClientPageModule>().FirstOrDefault(
                        item =>
                        ((IClientPageModule)Activator.CreateInstance(item, null)).UrlPath.ToLower() == SQLDataHelper.GetString(urlPath).ToLower());

            if (moduleInst != null)
            {
                var module = ((IModule)Activator.CreateInstance(moduleInst, null));
                return module.ModuleStringId;
            }

            return string.Empty;
        }
        #endregion

        #region Call core methods for modules
        public static void SendModuleMail(Guid customerIdTo, string subject, string message, string email, bool isBodyHtml)
        {
            MailService.SendMailNow(customerIdTo, email, subject, message, isBodyHtml);
        }

        public static Dictionary<string, string> GetFunnels()
        {
            var result = new Dictionary<string, string>();

            foreach (var salesFunnel in SalesFunnelService.GetList())
            {
                result.Add(salesFunnel.Id.ToString(), salesFunnel.Name);
            }
            return result;
        }
        #endregion

        public static List<PaymentModule> GetModulePayments()
        {
            return CacheManager.Get(ModulesPaymentCacheKey, 25, () =>
            {
                var moduleBox = GetModulesFromRemoteServer();
                var modules = moduleBox != null && moduleBox.Items != null ? moduleBox.Items : new List<Module>();

                var paymentModules =
                    modules
                       .Where(x => x.IsPayment)
                       .Select(module => new PaymentModule
                        {
                            ModuleId = module.Id,
                            ModuleStringId = module.StringId,
                            ModuleVersion = module.Version,
                            PaymentKey = module.PaymentKey,
                            Name = module.PaymentName,
                            Price = module.Price,
                            PriceString = module.PriceString,
                            Icon = module.PaymentIcon,
                        })
                       .ToList();
                return paymentModules;
            });
        }

        public static List<ShippingModule> GetModuleShippings()
        {
            return CacheManager.Get(ModulesShippingCacheKey, 25, () =>
            {
                var moduleBox = GetModulesFromRemoteServer();
                var modules = moduleBox != null && moduleBox.Items != null ? moduleBox.Items : new List<Module>();

                var paymentModules =
                    modules
                       .Where(x => x.IsShipping)
                       .Select(module => new ShippingModule
                        {
                            ModuleId = module.Id,
                            ModuleStringId = module.StringId,
                            ModuleVersion = module.Version,
                            ShippingKey = module.ShippingKey,
                            Name = module.ShippingName,
                            Price = module.Price,
                            PriceString = module.PriceString,
                            Icon = module.ShippingIcon,
                        })
                       .ToList();
                return paymentModules;
            });
        }

        public static List<SalesChannel> GetModuleSalesChannels()
        {
            return CacheManager.Get(ModulesSalesChannelCacheKey, 25, () =>
            {
                var moduleBox = GetModulesFromRemoteServer();
                var modules = moduleBox != null && moduleBox.Items != null ? moduleBox.Items : new List<Module>();

                var salesChannels = modules.Where(x => x.ShowInSalesChannels).Select(GetSalesChannelFromModule).ToList();
                return salesChannels;
            });
        }

        private static SalesChannel GetSalesChannelFromModule(Module module)
        {
            var showPreview = 
                !module.ShowInstalledAndPreviewInSalesChannel && 
                module.ShowPreview && !ModulesRepository.IsPreviewShowed(module.StringId);
            
            return new SalesChannel(new SalesChannelConfigModel()
            {
                ModuleId = module.Id,
                ModuleStringId = module.StringId,
                ModuleVersion = module.Version,
                Type = ESalesChannelType.Module,
                Role = RoleAction.Modules,
                Name = module.TitleInSalesChannels,
                MenuName = module.MenuNameInSalesChannels,
                Url =
                    (module.ShowInstalledAndPreviewInSalesChannel && !ModulesRepository.IsInstallModule(module.StringId)) || 
                    showPreview
                        ? "modules/preview/" + module.StringId.ToLower()
                        : "modules/details/" + module.StringId.ToLower(),
                MenuIcon = module.MenuIconInSalesChannels,
                Icon = module.IconInSalesChannels,
                Description = module.BriefDescriptionInSalesChannels,
                Details = new SalesChannelDetails()
                {
                    Title = module.TitleInSalesChannels,
                    Text = module.DescriptionInSalesChannels,
                    Images =
                        !string.IsNullOrEmpty(module.ImageInSalesChannels)
                            ? new List<SalesChannelPicture>()
                            {
                                new SalesChannelPicture()
                                {
                                    Src = module.ImageInSalesChannels,
                                    Alt = module.Name
                                }
                            }
                            : new List<SalesChannelPicture>(),
                    PriceString = module.PriceString,
                    Price = module.Price,
                },
                ShowInstalledAndPreview = module.ShowInstalledAndPreviewInSalesChannel,
                ShowPreview = showPreview,
                PreviewRightText = module.PreviewRightTextInSalesChannel,
                PreviewLeftText = module.PreviewLeftTextInSalesChannel,
                PreviewButtonText = module.PreviewButtonTextInSalesChannel
            });
        }

        public static void ClearModuleSalesChannelsCache()
        {
            CacheManager.Remove(ModulesSalesChannelCacheKey);
            CacheManager.RemoveByPattern(SalesChannelService.CacheKey);
        }
    }
}