using AdvantShop.App.Landing.Domain;
using AdvantShop.App.Landing.Handlers.Install;
using AdvantShop.App.Landing.Models.Inplace;
using AdvantShop.Diagnostics;
using System.Linq;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.Services.Landing.Blocks;

namespace AdvantShop.App.Landing.Handlers.Inplace
{
    public class AddBlock
    {
        #region Ctor

        private readonly AddBlockModel _model;
        private readonly bool _top;
        private readonly int _blockIdSibling;

        private readonly LpService _lpService;
        private readonly LpBlockService _lpBlockService;

        public AddBlock(AddBlockModel model, bool? top, int? blockIdSibling)
        {
            _model = model;
            _top = top.HasValue ? top.Value : false;
            _blockIdSibling = blockIdSibling.HasValue ? blockIdSibling.Value : 0;

            _lpService = new LpService();
            _lpBlockService = new LpBlockService();
        }

        #endregion

        public AddBlockResultModel Execute()
        {
            var lp = _lpService.Get(_model.LpId);
            if (lp == null)
                return new AddBlockResultModel();

            if (string.IsNullOrWhiteSpace(_model.Name))
                return new AddBlockResultModel();

            int sortOrder = 0;
            var blocks = _lpBlockService.GetList(_model.LpId);
            if (_top && _blockIdSibling != 0 && blocks.Any(x => x.Id == _blockIdSibling))
            {
                var indexSibling = blocks.FindIndex(x => x.Id == _blockIdSibling);

                sortOrder = indexSibling*100 + 100;

                for (int i = 0; i < blocks.Count; i++)
                {
                    if (i < indexSibling)
                        continue;

                    blocks[i].SortOrder = (i + 1)*100 + 100;

                    _lpBlockService.Update(blocks[i]);
                }
            }
            else
            {
                sortOrder = blocks.Count != 0 ? blocks.Max(x => x.SortOrder) + 100 : 0;
            }
            
            var result = new InstallBlockHandler(_model.Name, lp.Template, lp.Id, sortOrder, null).Execute();
            if (!result.Result)
            {
                Debug.Log.Error("Can't add landing block " + _model.Name + " for template " + lp.Template);
                return new AddBlockResultModel();
            }

            var block = _lpBlockService.Get(result.BlockId);
            if (block != null)
            {
                LpSiteService.UpdateModifiedDateByLandingId(block.LandingId);

                return new AddBlockResultModel() {Result = true, Block = block};
            }

            return new AddBlockResultModel();
        }
    }
}
