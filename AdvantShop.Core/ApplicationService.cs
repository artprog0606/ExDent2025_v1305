//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Net;
using System.Web;
using System.Web.Routing;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Scheduler;
using AdvantShop.Core.Services.Crm.Telegram;
using AdvantShop.Core.Services.Helpers;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Migrations;
using AdvantShop.Core.Services.Statistic.QuartzJobs;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Diagnostics;
using AdvantShop.ExportImport;
using AdvantShop.FilePath;
using AdvantShop.Statistic;

namespace AdvantShop.Core
{
    public static class AppServiceStartAction
    {
        public static PingDbState state;
        public static string errMessage;
        public static bool isAppNeedToReRun;
        public static bool isAppFistRun;
    }

    public class ApplicationService
    {
        public static void StartApplication(HttpContext current)
        {
            //PreApplicationInit.InitializeModules();

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls |
                                                   SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 |
                                                   SecurityProtocolType.Tls13;
            ServicePointManager.Expect100Continue = false;
            ApplicationUptime.SetApplicationStartTime();

            SettingsGeneral.SetAbsolutePath(current.Server.MapPath("~/"));

            // Set "first run" flag
            AppServiceStartAction.isAppFistRun = true;

            // Try to run DB depend code
            TryToStartDbDependServices();

            // No DB depend code here!
        }

        public static void TryToStartDbDependServices()
        {
            var appStartDbRes = DataBaseService.CheckDbStates();

            AppServiceStartAction.state = appStartDbRes;

            if (AppServiceStartAction.state == PingDbState.NoError)
            {
                // Other db depend codes
                RunDbDependAppStartServices();
            }
        }

        static bool running = false;
        public static void RunDbDependAppStartServices()
        {
            if (running)
                return;

            running = true;

            try
            {
                // loger must init ONLY after SetAbsolutePath 
                Debug.InitLogger();
            }
            catch (Exception ex)
            {
                // nothing here, can't log anything 
            }

            try
            {
                if (SettingsMain.CurrentFilesStorageSize == 0 || SettingsMain.CurrentFilesStorageLastUpdateTime == DateTime.MinValue)
                    FilesStorageService.RecalcAttachmentsSizeInBackground();
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            try
            {
                UpdateRoutes();
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            try
            {
                AttachedModules.LoadModules();
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            try
            {
                ModulesService.CallModulesUpdate();
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            if (!UrlRewriteExtensions.IsAppBlock())
            {
                try
                {
                    QuartzJobsLoggingService.FinalizeDeadJobs();
                    var taskManager = TaskManager.TaskManagerInstance();
                    taskManager.Init();
                    taskManager.Start();

                    var settings = TaskSettings.Settings;
                    var exportFeedSettings = ExportFeedService.GetExportFeedTaskSettings();
                    
                    if (exportFeedSettings != null && exportFeedSettings.Count > 0)
                        settings.AddRange(exportFeedSettings);

                    var discountByTimeSettings = DiscountByTimeService.GetAllDiscountByTimeTaskSettings();
                    if (discountByTimeSettings != null && discountByTimeSettings.Count > 0)
                        settings.AddRange(discountByTimeSettings);
                    
                    taskManager.ManagedTask(settings);
                }
                catch (Exception ex)
                {
                    Debug.Log.Error(ex);
                }
            }

            // LogSessionRestart
            try
            {
                InternalServices.LogApplicationRestart(false);
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            try
            {
                FoldersHelper.InitFolders();
                FoldersHelper.InitExtraCss();
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            try
            {
                LocalizationService.GenerateJsResourcesFile();
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }


            try
            {
                SettingsLic.Activate();
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }


            try
            {
                new TelegramApiService().Initialize();
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }


            try
            {
                new Services.Landing.Blocks.LpBlockService().UpdateAllBlocksIfRequired();
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            try
            {
                SettingsDesign.ApplyInstalledTemplate();
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            try
            {
                AssetsTool.DeleteNotUsedFiles();
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            new MigrationService().Initialize();
        }

        public static void UpdateRoutes()
        {
            try
            {
                var category = CategoryService.GetCategory(0);
                if (category == null)
                {
                    Debug.Log.Error("Can not build route for root category, because it is null");
                    return;
                }
                var route = RouteTable.Routes["CatalogRoot"] as Route;
                var routeMobile = RouteTable.Routes["Mobile_CatalogRoot"] as Route;
                if (route != null)
                    route.Url = category.UrlPath;
                if (routeMobile != null)
                    routeMobile.Url = category.UrlPath;
            }
            catch (Exception ex)
            {
                Debug.Log.Error("at UpdateRoutes", ex);
            }
        }
    }
}