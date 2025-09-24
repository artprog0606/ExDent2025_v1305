using System.Collections.Generic;
using System.Linq;
using AdvantShop.App.Landing.Domain;
using AdvantShop.App.Landing.Domain.Templates;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.Services.Landing.Blocks;
using AdvantShop.Core.Services.Landing.Templates;

namespace AdvantShop.App.Landing.Handlers.Inplace
{
    public class GetBlocks
    {
        private readonly int _landingId;
        private readonly LpService _lpService;
        private readonly LpTemplateService _lpTemplateService;

        public GetBlocks(int landingId)
        {
            _landingId = landingId;
            _lpService = new LpService();
            _lpTemplateService = new LpTemplateService();
        }

        public List<LpBlockListItem> Execute()
        {
            var lp = _lpService.Get(_landingId);
            if (lp == null)
                return null;
            
            var template = _lpTemplateService.GetTemplate(lp.Template);
            if (template == null)
                return null;
            
            var blockLists = _lpTemplateService.GetAllBlocks() ?? new List<LpBlockListItem>();


            if (template.BlockLists != null)
            {
                foreach (var tplBlockList in template.BlockLists)
                {
                    var bc = blockLists.FirstOrDefault(x => x.Category == tplBlockList.Category);
                    if (bc != null)
                        bc.Blocks.AddRange(tplBlockList.Blocks);
                    else
                        blockLists.Add(tplBlockList);
                }
            }

            foreach (var list in blockLists)
            {
                list.Blocks.RemoveAll(BlockIsHidden);
            }

            return blockLists.Where(x => x.Blocks.Any()).ToList();
        }

        private bool BlockIsHidden(LpBlockItem block)
        {
            if (block.SettingValues != null && block.SettingValues.Any())
            {
                return !block.SettingValues.Select(SettingProvider.GetSqlSettingValue).Any(x => x.IsNullOrEmpty() || x.TryParseBool());
            }

            return false;
        }
    }
}
