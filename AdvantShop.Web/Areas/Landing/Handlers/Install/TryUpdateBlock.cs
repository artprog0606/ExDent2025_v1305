using System.Collections.Generic;
using AdvantShop.App.Landing.Domain;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.Services.Landing.Blocks;
using Newtonsoft.Json;

namespace AdvantShop.App.Landing.Handlers.Install
{
    public class TryUpdateBlock
    {
        private readonly int _blockId;

        public TryUpdateBlock(int blockId)
        {
            _blockId = blockId;
        }

        public bool Execute()
        {
            var block = new LpBlockService().Get(_blockId);
            if (block == null)
                return false;

            var lp = new LpService().Get(block.LandingId);

            var config = new LpBlockConfigService().Get(block.Name, lp.Template);

            if (config == null)
                return false;

            var blockServise = new LpBlockService();
            blockServise.TryToUpdateBlock(block, config);

            return true;
        }

       

       
    }
}
