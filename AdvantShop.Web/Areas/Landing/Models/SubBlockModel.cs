using AdvantShop.App.Landing.Domain;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.Services.Landing.Blocks;

namespace AdvantShop.App.Landing.Models
{
    public class SubBlockModel
    {
        public string ViewPath { get; set; }
        public bool InPlace { get; set; }
        public LpSubBlock SubBlock { get; private set; }

        private LpBlock _parentBlock = null;
        public LpBlock ParentBlock
        {
            get
            {
                if (_parentBlock != null)
                    return _parentBlock;

                return _parentBlock = new LpBlockService().Get(SubBlock.LandingBlockId);
            }
        }

        public bool HidePlaceholder { get; internal set; }

        public SubBlockModel(LpSubBlock subBlock)
        {
            SubBlock = subBlock;
            InPlace = LpService.Inplace;
        }
    }
}
