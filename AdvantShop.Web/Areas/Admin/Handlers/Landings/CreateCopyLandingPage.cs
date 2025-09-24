using System.Collections.Generic;
using AdvantShop.App.Landing.Domain.Settings;
using AdvantShop.App.Landing.Handlers.Inplace;
using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.Services.Landing.Blocks;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Landings
{
    public class CreateCopyLandingPage : AbstractCommandHandler<CopyLandingPageResult>
    {
        #region Ctor

        private readonly Lp _sourcePage;
        private readonly int _landingSiteId;

        private readonly LpService _lpService;
        private readonly LpBlockService _blockService;
        private readonly LpSettingsService _settingsService;

        private readonly bool _isInnerCopy;

        public CreateCopyLandingPage()
        {
            _lpService = new LpService();
            _blockService = new LpBlockService();
            _settingsService = new LpSettingsService();
        }

        public CreateCopyLandingPage(Lp sourcePage, int landingSiteId) : this()
        {
            _sourcePage = sourcePage;
            _landingSiteId = landingSiteId;
        }

        public CreateCopyLandingPage(int landingPageId) : this()
        {
            _sourcePage = _lpService.Get(landingPageId);
            _landingSiteId = _sourcePage.LandingSiteId;

            _isInnerCopy = true;
        }

        #endregion

        protected override CopyLandingPageResult Handle()
        {
            if (_sourcePage == null)
                throw new BlException("Страница не найдена");

            var newPage = (Lp)_sourcePage.Clone();
            newPage.LandingSiteId = _landingSiteId;

            if (_isInnerCopy)
            {
                newPage.Name += " - Копия";
                newPage.Url = _lpService.GetAvailableUrl(newPage.LandingSiteId, _sourcePage.Name);
                newPage.IsMain = false;
            }

            _lpService.Add(newPage);

            foreach (var setting in _settingsService.GetList(_sourcePage.Id))
            {
                _settingsService.AddOrUpdate(newPage.Id, setting.Name, setting.Value);
            }

            var result = new CopyLandingPageResult() {Lp = newPage};

            foreach (var block in _blockService.GetList(_sourcePage.Id))
            {
                if (_isInnerCopy && (block.ShowOnAllPagesTop || block.ShowOnAllPagesBottom))
                    continue;

                var newBlockResult = new CopyBlock(block, newPage).Execute();
                if (newBlockResult != null)
                {
                    result.Blocks.Add(block.Id, newBlockResult.Block.Id);
                    result.Forms.AddRange(newBlockResult.Forms);
                }
            }

            return result;
        }
    }

    public class CopyLandingPageResult
    {
        public Lp Lp { get; set; }
        public Dictionary<int, int> Blocks { get; set; }
        public Dictionary<int, int> Forms { get; set; }

        public CopyLandingPageResult()
        {
            Blocks = new Dictionary<int, int>();
            Forms = new Dictionary<int, int>();
        }
    }
}
