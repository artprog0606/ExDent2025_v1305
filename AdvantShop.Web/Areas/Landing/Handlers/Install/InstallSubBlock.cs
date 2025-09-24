using System;
using System.Collections.Generic;
using AdvantShop.App.Landing.Domain.SubBlocks;
using AdvantShop.Core.Services.Landing.Blocks;
using Newtonsoft.Json;

namespace AdvantShop.App.Landing.Handlers.Install
{
    public class InstallSubBlock
    {
        #region Ctor

        private readonly LpBlock _block;
        private readonly LpSubBlockConfig _subBlockConfig;
        private readonly int _sortOrder;
        private readonly LpBlockService _lpBlockService;
        private readonly bool _isEmptyContent;

        private readonly List<string> _simpleTypes = new List<string>() { "text" };

        public InstallSubBlock(LpBlock block, LpSubBlockConfig subBlockConfig, int sortOrder)
        {
            _block = block;
            _subBlockConfig = subBlockConfig;
            _sortOrder = sortOrder;

            _lpBlockService = new LpBlockService();
        }

        public InstallSubBlock(LpBlock block, LpSubBlockConfig subBlockConfig, int sortOrder, bool isEmptyContent)
            : this(block, subBlockConfig, sortOrder)
        {
            _isEmptyContent = isEmptyContent;
        }

        #endregion

        public LpSubBlock Execute()
        {
            string content = null;

            if (!string.IsNullOrWhiteSpace(_subBlockConfig.Name) && !_simpleTypes.Contains(_subBlockConfig.Type))
            {
                var specificSubBlock = TryGetSubBlock(_subBlockConfig.Name);
                if (specificSubBlock != null)
                {
                    content = specificSubBlock.GetContent(null);
                    _subBlockConfig.Settings = specificSubBlock.GetSettings(_block, null, _subBlockConfig.Settings);
                }
            }

            var result = new LpSubBlock()
            {
                LandingBlockId = _block.Id,
                Name = _subBlockConfig.Name,
                Type = _subBlockConfig.Type,
                ContentHtml = 
                    !string.IsNullOrEmpty(content) 
                        ? content 
                        : _isEmptyContent ? null : _subBlockConfig.Placeholder,
                Settings = JsonConvert.SerializeObject(_subBlockConfig.Settings),
                SortOrder = _sortOrder
            };

            _lpBlockService.AddSubBlock(result);

            return result;
        }

        private ILpSubBlock TryGetSubBlock(string key)
        {
            var name = key.ToLower().Replace("_", "") + "subblock";
            var type = LpBlockService.GetTypes(typeof(ILpSubBlock).Name).Find(x => x.Name.ToLower() == name);

            return type != null ? (ILpSubBlock)Activator.CreateInstance(type) : null;
        }
    }
}
