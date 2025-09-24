using System;
using System.IO;
using System.Web.Hosting;
using AdvantShop.Configuration;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;

namespace AdvantShop.Core.Services.Landing.Pictures
{
    public class RemoveLandingPicturesHandler
    {
        private int _lpId;
        private int _siteId;

        public RemoveLandingPicturesHandler(int lpId, int siteId)
        {
            _lpId = lpId;
            _siteId = siteId;
        }

        public bool Execute()
        {
            try
            {
                var landingFolder = HostingEnvironment.MapPath(string.Format(LpFiles.LpSiteLandingPagePath, _siteId, _lpId));

                if (Directory.Exists(landingFolder))
                {
                    if (SettingsGeneral.BackupPhotosBeforeDeleting)
                        FileHelpers.BackupPhotosDirectory(landingFolder);

                    FileHelpers.DeleteDirectory(landingFolder);
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error("LandingPage module, RemoveLandingPicturesHandler", ex);
                return false;
            }
            return true;
        }
    }
}
