using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Core.Services.WebManifest;
using AdvantShop.Diagnostics;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using Newtonsoft.Json;

namespace AdvantShop.Web.Admin.Handlers.Settings.Mobiles
{
    public class WebManifestHandler
    {
        public void Execute()
        {
            if (!SettingsMobile.MobileAppManifestName.IsNullOrEmpty())
            {
                FileHelpers.DeleteFile(SettingsGeneral.AbsolutePath + "/" + SettingsMobile.MobileAppManifestName.Split('?').FirstOrDefault());
                SettingsMobile.MobileAppManifestName = null;
            }

            if (!SettingsMobile.MobileAppActive)
                return;

            var data = new WebManifest
            {
                Name = SettingsMobile.MobileAppName,
                ShortName = SettingsMobile.MobileAppShortName
            };

            if (!string.IsNullOrEmpty(SettingsMobile.MobileAppIconImageName))
            {
                foreach (MobileAppIcon type in Enum.GetValues(typeof(MobileAppIcon)))
                {
                    var size = FileHelpers.GetMobileAppIconMaxSize(type);
                    data.Icons.Add(new WebManifestIcon
                    {
                        Src = FoldersHelper.GetMobileAppIconPath(type, SettingsMobile.MobileAppIconImageName),
                        Sizes = size.Width + "x" + size.Height,
                        Type = "image/png"
                    });
                }
            }

            var relatedApps = new List<RelatedApp>();
            if (!string.IsNullOrEmpty(SettingsMobile.MobileAppAppleAppStoreLink))
            {
                relatedApps.Add(new RelatedApp
                {
                    Platform = "itunes",
                    Url = SettingsMobile.MobileAppAppleAppStoreLink
                });
            }

            if (!string.IsNullOrEmpty(SettingsMobile.MobileAppGooglePlayMarket))
            {
                var androidApp = new RelatedApp
                {
                    Platform = "play",
                    Url = SettingsMobile.MobileAppGooglePlayMarket
                };

                try
                {
                    var uri = new Uri(SettingsMobile.MobileAppGooglePlayMarket);
                    var idParam = HttpUtility.ParseQueryString(uri.Query).Get("id");
                    if (!string.IsNullOrEmpty(idParam))
                        androidApp.Id = idParam;
                }
                catch
                {
                    Debug.Log.Warn("AppManifestHandler: wrong GooglePlayMarket url");
                }

                relatedApps.Add(androidApp);
            }

            data.RelatedApps = relatedApps.Count > 0
                ? relatedApps
                : null;
            data.PreferRelatedApps = data.RelatedApps != null;

            var json = JsonConvert.SerializeObject(data);
            SettingsMobile.MobileAppManifestName = "manifest.json";
            using (var file = File.CreateText(SettingsGeneral.AbsolutePath + "/" + SettingsMobile.MobileAppManifestName))
            {
                var serializer = new JsonSerializer();

                serializer.Serialize(file, data);
            }

            SettingsMobile.MobileAppManifestName += "?r=" + json.Sha256();
        }
    }

}
