using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using AdvantShop.App.Landing.Domain.Settings;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.Services.WebManifest;
using AdvantShop.Diagnostics;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using Newtonsoft.Json;

namespace AdvantShop.Web.Admin.Handlers.Landings.MobileApp
{
    public class WebManifestHandler
    {
        private readonly int _siteId;
        private readonly string _landingPath;
        public WebManifestHandler(int siteId)
        {
            LpService.CurrentSiteId = _siteId = siteId;
            _landingPath = HttpContext.Current.Server.MapPath(string.Format(LpFiles.LpSitePath, _siteId));
        }

        public void Execute()
        {
            if (!string.IsNullOrEmpty(LSiteSettings.MobileAppManifestName))
            {
                FileHelpers.DeleteFile(_landingPath + LSiteSettings.MobileAppManifestName.Split('?').FirstOrDefault());
                LSiteSettings.MobileAppManifestName = null;
            }

            if (!LSiteSettings.MobileAppActive)
                return;

            var data = new WebManifest
            {
                Name = LSiteSettings.MobileAppName,
                ShortName = LSiteSettings.MobileAppShortName
            };

            if (!string.IsNullOrEmpty(LSiteSettings.MobileAppIconImageName))
            {
                foreach (MobileAppIcon type in Enum.GetValues(typeof(MobileAppIcon)))
                {
                    var size = FileHelpers.GetMobileAppIconMaxSize(type);
                    data.Icons.Add(new WebManifestIcon
                    {
                        Src = FoldersHelper.GetMobileAppIconPath(type, LSiteSettings.MobileAppIconImageName, string.Format(LpFiles.LpSitePathRelative, _siteId)),
                        Sizes = size.Width + "x" + size.Height,
                        Type = "image/png"
                    });
                }
            }

            var relatedApps = new List<RelatedApp>();
            if (!string.IsNullOrEmpty(LSiteSettings.MobileAppAppleAppStoreLink))
            {
                relatedApps.Add(new RelatedApp
                {
                    Platform = "itunes",
                    Url = LSiteSettings.MobileAppAppleAppStoreLink
                });
            }
            if (!string.IsNullOrEmpty(LSiteSettings.MobileAppGooglePlayMarket))
            {
                var androidApp = new RelatedApp
                {
                    Platform = "play",
                    Url = LSiteSettings.MobileAppGooglePlayMarket
                };

                try
                {
                    var uri = new Uri(LSiteSettings.MobileAppGooglePlayMarket);
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
            LSiteSettings.MobileAppManifestName = "manifest.json";
            using (var file = File.CreateText(_landingPath + LSiteSettings.MobileAppManifestName))
            {
                var serializer = new JsonSerializer();

                serializer.Serialize(file, data);
            }

            LSiteSettings.MobileAppManifestName += "?r=" + json.Sha256();
        }
    }
}
