using System;
using System.Web.Hosting;
using AdvantShop.App.Landing.Domain;
using AdvantShop.App.Landing.Domain.Settings;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;

namespace AdvantShop.App.Landing.Handlers.Inplace
{

    public class RemoveFavicon
    {
        private readonly int _lpId;


        public RemoveFavicon(int lpId)
        {
            _lpId = lpId;
        }

        public bool Execute()
        {
            try
            {
                var lp = new LpService().Get(_lpId);

                var lpFolder = HostingEnvironment.MapPath(string.Format(LpFiles.LpSitePath, lp.LandingSiteId));

                FileHelpers.DeleteFile(lpFolder + LSiteSettings.Favicon);
                LSiteSettings.Favicon = string.Empty;
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                return false;
            }
            return true;
        }
    }
}
