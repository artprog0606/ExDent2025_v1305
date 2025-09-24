using System;
using System.IO;
using AdvantShop.App.Landing.Domain;
using AdvantShop.App.Landing.Models.Inplace;
using AdvantShop.Core;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.Services.Landing.Blocks;
using AdvantShop.Diagnostics;

namespace AdvantShop.App.Landing.Handlers.Inplace
{
    public class RecreateBlock
    {
        private readonly int _blockId;
        private readonly LpBlockService _blockService;

        public RecreateBlock(int blockId)
        {
            _blockId = blockId;
            _blockService = new LpBlockService();
        }

        public bool Execute()
        {
            var block = _blockService.Get(_blockId);
            if (block == null)
                return false;

            try
            {
                var model = new AddBlockModel()
                {
                    LpId = block.LandingId,
                    Name = block.Name,
                    SortOrder = block.SortOrder
                };

                var result = new AddBlock(model, true, _blockId).Execute();
                if (result.Block == null)
                    throw new BlException("Ошибка при создании блока");

                var lp = new LpService().Get(block.LandingId);

                var oldBlockFiles = string.Format(LpFiles.UploadPictureFolderLandingBlock, lp.LandingSiteId, lp.Id, block.Id);

                if (Directory.Exists(oldBlockFiles))
                {
                    var newBlockFiles = string.Format(LpFiles.UploadPictureFolderLandingBlock, lp.LandingSiteId, lp.Id, result.Block.Id);

                    Directory.CreateDirectory(newBlockFiles);

                    foreach (string newPath in Directory.GetFiles(newBlockFiles, "*.*", SearchOption.AllDirectories))
                        File.Copy(newPath, newPath.Replace(oldBlockFiles, newBlockFiles), true);
                }

                //_blockService.Delete(block.Id);

                return true;
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            return false;
        }
    }
}
