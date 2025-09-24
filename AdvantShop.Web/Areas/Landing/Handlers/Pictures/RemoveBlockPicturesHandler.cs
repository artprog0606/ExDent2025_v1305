using System;
using System.IO;
using System.Web.Hosting;
using AdvantShop.Diagnostics;
using AdvantShop.App.Landing.Domain;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Helpers;

namespace AdvantShop.App.Landing.Handlers.Pictures
{
    public class RemoveBlockPicturesHandler
    {
        private int _lpId;
        private int _blockId;

        public RemoveBlockPicturesHandler(int lpId, int blockId)
        {
            _lpId = lpId;
            _blockId = blockId;
        }

        public bool Execute()
        {
            try
            {
                var lp = new LpService().Get(_lpId);
                var landingBlockFolder = HostingEnvironment.MapPath(string.Format(LpFiles.UploadPictureFolderLandingBlock, lp.LandingSiteId, _lpId, _blockId));

                if (Directory.Exists(landingBlockFolder))
                {
                    if (SettingsGeneral.BackupPhotosBeforeDeleting)
                    {
                        foreach (var file in Directory.GetFiles(landingBlockFolder))
                            FileHelpers.BackupPhoto(file);
                    }
                    else
                    {
                        Directory.Delete(landingBlockFolder, true);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error("LandingPage module, RemoveBlockPicturesHandler", ex);
                return false;
            }
            return true;
        }
    }
}
