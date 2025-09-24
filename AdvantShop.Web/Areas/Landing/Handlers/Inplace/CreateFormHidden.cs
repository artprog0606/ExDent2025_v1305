using AdvantShop.App.Landing.Controllers;
using AdvantShop.App.Landing.Domain;
using AdvantShop.App.Landing.Models.Inplace;
using System.Linq;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.Services.Landing.Blocks;

namespace AdvantShop.App.Landing.Handlers.Inplace
{
    public class CreateFormHidden
    {
        #region Ctor

        private readonly int _lpId;
        private readonly LandingBaseController _controller;

        private readonly LpService _lpService;
        private readonly LpBlockService _lpBlockService;

        public CreateFormHidden(int lpId, LandingBaseController controller)
        {
            _lpId = lpId;
            _controller = controller;

            _lpService = new LpService();
            _lpBlockService = new LpBlockService();
        }

        #endregion

        public AddBlockResultModel Execute()
        {
            var lp = _lpService.Get(_lpId);
            if (lp == null)
                return new AddBlockResultModel();

            var blocks = _lpBlockService.GetList(_lpId);
            var sortOrder = blocks.Max(x => x.SortOrder) + 100;

            return new AddBlock(new AddBlockModel()
            {
                LpId = _lpId,
                Name = "FormHidden",
                SortOrder = sortOrder
            }, 
            null, null).Execute();
        }
    }
}
