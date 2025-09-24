using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Hosting;
using AdvantShop.App.Landing.Domain.ColorSchemes;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.Services.Landing.Blocks;
using AdvantShop.Core.Services.Landing.Forms;
using AdvantShop.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AdvantShop.App.Landing.Handlers.Inplace
{
    public class CopyBlock
    {
        #region Ctor

        private readonly LpBlock _sourceBlock;
        private readonly Lp _lp;
        private readonly int _sourceLandingSiteId;

        private readonly LpBlockService _blockService;
        private readonly LpService _lpService;
        private readonly LpFormService _formService;
        private readonly LpColorSchemeService _colorSchemeService;

        private readonly bool _isForNewPage;

        public CopyBlock()
        {
            _blockService = new LpBlockService();
            _lpService = new LpService();
            _formService = new LpFormService();
            _colorSchemeService = new LpColorSchemeService();
        }

        public CopyBlock(int blockId) : this()
        {
            _sourceBlock = _blockService.Get(blockId);

            if (_sourceBlock != null)
            {
                _lp = _lpService.Get(_sourceBlock.LandingId);
                if (_lp != null)
                    _sourceLandingSiteId = _lp.LandingSiteId;
            }
        }

        public CopyBlock(LpBlock block, Lp newLpPage) : this()
        {
            _sourceBlock = block;
            _lp = newLpPage;

            var sourceLp = _lpService.Get(_sourceBlock.LandingId);
            if (sourceLp != null)
                _sourceLandingSiteId = sourceLp.LandingSiteId;

            _isForNewPage = true;
        }

        #endregion

        public CopyBlockResult Execute()
        {
            if (_sourceBlock == null || _lp == null)
                return null;
            
            try
            {
                var newBlock = (LpBlock)_sourceBlock.Clone();

                if (_isForNewPage)
                {
                    newBlock.LandingId = _lp.Id;
                }
                else
                {
                    newBlock.SortOrder += 1;
                }

                _blockService.Add(newBlock);

                var mappedSettings = JObject.Parse(newBlock.Settings) as dynamic;
                var updateMappedSettings = false;

                foreach (var subBlock in _blockService.GetSubBlocks(_sourceBlock.Id))
                {
                    subBlock.LandingBlockId = newBlock.Id;

                    if (!string.IsNullOrEmpty(subBlock.Settings))
                    {
                        subBlock.Settings =
                            subBlock.Settings.Replace(
                                string.Format("pictures/landing/{0}/{1}/{2}/", _sourceLandingSiteId, _sourceBlock.LandingId, _sourceBlock.Id),
                                string.Format("pictures/landing/{0}/{1}/{2}/", _lp.LandingSiteId, _lp.Id, newBlock.Id));
                    }

                    _blockService.AddSubBlock(subBlock);
                }

                var result = new CopyBlockResult() {Block = newBlock, Forms = new Dictionary<int, int>()};

                var form = _formService.GetByBlock(_sourceBlock.Id);
                if (form != null)
                {
                    var sourceFormId = form.Id;

                    form.BlockId = newBlock.Id;
                    
                    if (_isForNewPage)
                        form.LpId = _lp.Id;

                    _formService.Add(form);

                    result.Forms.Add(sourceFormId, form.Id);

                    if (mappedSettings != null && mappedSettings.button != null)
                    {
                        mappedSettings.button.action_form = form.Id;
                        updateMappedSettings = true;
                    }
                }

                var colorScheme = _colorSchemeService.GetByBlockId(_sourceBlock.Id);
                if (colorScheme != null)
                {
                    colorScheme.LpId = _lp.Id;
                    colorScheme.LpBlockId = newBlock.Id;

                    _colorSchemeService.AddUpdate(colorScheme);
                }

                if (updateMappedSettings || newBlock.Settings.Contains("pictures/landing/"))
                {
                    newBlock.Settings = JsonConvert.SerializeObject(mappedSettings);

                    newBlock.Settings =
                        newBlock.Settings.Replace(
                                string.Format("pictures/landing/{0}/{1}/{2}/", _sourceLandingSiteId, _sourceBlock.LandingId, _sourceBlock.Id),
                                string.Format("pictures/landing/{0}/{1}/{2}/", _lp.LandingSiteId, _lp.Id, newBlock.Id));

                    _blockService.Update(newBlock);
                }
                
                var sourceBlockFiles = HostingEnvironment.MapPath(string.Format(LpFiles.UploadPictureFolderLandingBlock, _sourceLandingSiteId, _sourceBlock.LandingId, _sourceBlock.Id));

                if (Directory.Exists(sourceBlockFiles))
                {
                    var newBlockFiles = HostingEnvironment.MapPath(string.Format(LpFiles.UploadPictureFolderLandingBlock, _lp.LandingSiteId, _lp.Id, newBlock.Id));

                    Directory.CreateDirectory(newBlockFiles);

                    foreach (var path in Directory.GetFiles(sourceBlockFiles, "*.*", SearchOption.AllDirectories))
                        File.Copy(path, path.Replace(sourceBlockFiles, newBlockFiles), true);
                }

                return result;
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            return null;
        }
    }

    public class CopyBlockResult
    {
        public LpBlock Block { get; set; }
        public Dictionary<int, int> Forms { get; set; }
    }
}
