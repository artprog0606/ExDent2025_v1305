using AdvantShop.App.Landing.Domain;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.Services.Landing.Blocks;
using AdvantShop.Core.Services.Landing.Forms;

namespace AdvantShop.App.Landing.Models
{
    public class BlockModel
    {
        public LpBlock Block { get; set; }
        public LpBlockConfig Config { get; set; }

        public bool Inplace { get; set; }

        private LpForm _form = null;
        public LpForm Form
        {
            get
            {
                if (_form != null)
                    return _form;

                var service = new LpFormService();
                var lpId = LpService.CurrentLanding != null ? LpService.CurrentLanding.Id : 0;

                return _form = service.GetByBlock(Block.Id) ?? service.AddAndGetDefaultForm(Block.Id, lpId);
            }
        }
        
        public bool ShowStoreClosedWarning { get; set; }
    }
}
