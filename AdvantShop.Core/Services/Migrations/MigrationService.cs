using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Core.Services.Files;
using AdvantShop.Core.Services.Helpers;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;

namespace AdvantShop.Core.Services.Migrations
{
    public class MigrationService
    {
        public void Initialize()
        {
            try
            {
                MigrateFilesInRootInDb();
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }
        }

        private void MigrateFilesInRootInDb()
        {
            if (SettingsMigration.FilesInRootMigrationСompleted)
                return;

            var exclusions = new List<string>() { "app_offline", "app_block", "sitemap", };
            var contentTypeHelper = new FileExtensionContentTypeHelper();

            // files in root
            var files =
                Directory.GetFiles(HostingEnvironment.MapPath("~/"))
                    .Where(x => x.EndsWith(".txt", StringComparison.OrdinalIgnoreCase) ||
                                x.EndsWith(".html", StringComparison.OrdinalIgnoreCase) ||
                                x.EndsWith(".htm", StringComparison.OrdinalIgnoreCase))
                    .ToList();
            
            foreach (var filePath in files)                        
                MigrateFile(filePath, exclusions, contentTypeHelper, null);                
            
            // robots.txt for mobile domain
            var mobileRobotsTxtPath = HostingEnvironment.MapPath("~/areas/mobile/robots.txt");
            if (File.Exists(mobileRobotsTxtPath))
                MigrateFile(mobileRobotsTxtPath, exclusions, contentTypeHelper, "/areas/mobile/robots.txt", "utf-8");
                     

            SettingsMigration.FilesInRootMigrationСompleted = true;
        }

        private void MigrateFile(string filePath, List<string> exclusions, FileExtensionContentTypeHelper contentTypeHelper, string path, string charset = null)
        {
            try
            {
                var fileName = filePath.Split('\\').LastOrDefault();
                if (exclusions.Any(x => fileName.Contains(x, StringComparison.OrdinalIgnoreCase)))
                    return;

                if (path == null)
                    path = "/" + fileName;

                if (FileInDbService.IsExist(path))
                {
                    Debug.Log.Error($"MigrateFilesInRoot. Файл \"{fileName}\" с путем \"{path}\" уже находится в бд.");
                    return;
                }

                if (!contentTypeHelper.TryGetContentType(fileName, out var contentType))
                    contentType = "text/plain";

                FileInDbService.Add(new FileInDb()
                {
                    Name = fileName,
                    Path = path,
                    ContentType = contentType,
                    Content = File.ReadAllBytes(filePath),
                    Charset = charset,
                });

                var newFilePath = HostingEnvironment.MapPath("~/pictures_deleted/" + fileName.Replace("/", "_").Replace(".", "_moved."));

                FileHelpers.ReplaceFile(filePath, newFilePath);
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }
        }
    }
}
