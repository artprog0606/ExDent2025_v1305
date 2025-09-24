using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.App.Landing.Domain.ColorSchemes;
using AdvantShop.App.Landing.Handlers.Install;
using AdvantShop.App.Landing.Handlers.Landings;
using AdvantShop.App.Landing.Models;
using AdvantShop.App.Landing.Models.Inplace;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.Services.Landing.Blocks;
using AdvantShop.Core.Services.Landing.Forms;
using AdvantShop.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AdvantShop.App.Landing.Handlers.Inplace
{
    public class SaveBlockSettings
    {
        #region Ctor

        private readonly int _blockId;
        private readonly BlockSettings _blockSettings;
        private readonly LpBlockService _blockService;
        private readonly LpBlockConfigService _blockConfigService;

        public SaveBlockSettings()
        {
            _blockConfigService = new LpBlockConfigService();
        }

        public SaveBlockSettings(int blockId, string settings) : this()
        {
            _blockId = blockId;
            _blockSettings = JsonConvert.DeserializeObject<BlockSettings>(settings);

            _blockService = new LpBlockService();
        }

        public SaveBlockSettings(int blockId, Dictionary<string, object> settings, LpForm form) : this()
        {
            _blockId = blockId;
            _blockSettings = new BlockSettings() { BlockId = _blockId, Settings = settings};

            if (form != null)
            {
                var oldForm = new LpFormService().GetByBlock(_blockId);
                if (oldForm != null)
                {
                    form.Id = oldForm.Id;
                    form.BlockId = oldForm.BlockId;
                    form.LpId = oldForm.LpId;

                    if (settings.ContainsKey("form") && settings["form"] != null)
                    {
                        var offerId = ((JObject) settings["form"])["OfferId"];
                        if (offerId != null)
                        {
                            //form.OfferId = offerId != null ? offerId.ToString() : null;
                            form.OfferItems = new List<LpFormOfferItem>() {new LpFormOfferItem() {OfferId = offerId.ToString()}};
                        }
                    }

                    _blockSettings.Form = form;
                }
            }

            _blockService = new LpBlockService();
        }

        #endregion

        public ResultModel Execute()
        {
            if (_blockId == 0 || _blockSettings == null || _blockSettings.Settings == null)
                return new ResultModel();

            var block = _blockService.Get(_blockId);
            if (block == null)
                return new ResultModel();

            try
            {
                block.Settings = MergeSettings(block.Settings, _blockSettings.Settings);

                block.ShowOnAllPagesTop = _blockSettings.Settings.ContainsKey("show_on_all_landing_pages_top") &&
                                          Convert.ToBoolean(_blockSettings.Settings["show_on_all_landing_pages_top"]);
                
                block.ShowOnAllPagesBottom = _blockSettings.Settings.ContainsKey("show_on_all_landing_pages_bottom") &&
                                             Convert.ToBoolean(_blockSettings.Settings["show_on_all_landing_pages_bottom"]);

                block.NoCache = _blockSettings.Settings.ContainsKey("no_cache") && Convert.ToBoolean(_blockSettings.Settings["no_cache"]);

                _blockService.Update(block);

                if (_blockSettings.Subblocks != null && _blockSettings.Subblocks.Count > 0)
                {
                    var subBlocks = _blockService.GetSubBlocks(block.Id);

                    foreach (var subBlock in subBlocks)
                    {
                        var newSubBlock = _blockSettings.Subblocks.Find(x => x.Id == subBlock.Id);
                        if (newSubBlock == null)
                            continue;
                        
                        subBlock.ContentHtml = newSubBlock.ContentHtml;
                        subBlock.Settings = newSubBlock.Settings != null ? MergeSettings(subBlock.Settings, newSubBlock.Settings) : null;

                        _blockService.UpdateSubBlock(subBlock);
                    }

                    foreach (var subblock in _blockSettings.Subblocks.Where(x => !subBlocks.Any(s => s.Name == x.Name)))
                    {
                        var lp = new LpService().Get(block.LandingId);
                        var config = _blockConfigService.Get(block.Name, lp.Template);

                        var subblockConfig = config.SubBlocks.Find(x => x.Name == subblock.Name);
                        if (subblockConfig != null)
                        {
                            var result = new InstallSubBlock(block, subblockConfig, subBlocks.Max(x => x.SortOrder) + 100, true).Execute();
                        }
                    }
                }

                if (_blockSettings.Form != null && _blockSettings.Form.Id != 0)
                    new SaveFormSettings(_blockSettings.Form).Execute();

                var colorScheme = _blockSettings.Settings.ContainsKey("color_scheme") ? _blockSettings.Settings["color_scheme"] as string : null;
                if (colorScheme != null)
                {
                    if (colorScheme == "color-scheme--custom")
                    {
                        var colorSchemeCustomObj =
                            _blockSettings.Settings.ContainsKey("color_scheme_custom")
                                ? _blockSettings.Settings["color_scheme_custom"] as JObject
                                : null;

                        if (colorSchemeCustomObj != null)
                        {
                            var colorSchemeCustom = colorSchemeCustomObj.ToObject<LpColorScheme>();
                            if (colorSchemeCustom != null)
                            {
                                colorSchemeCustom.LpId = block.LandingId;
                                colorSchemeCustom.LpBlockId = block.Id;
                                colorSchemeCustom.Class = "color-scheme--custom";

                                new LpColorSchemeService().AddUpdate(colorSchemeCustom);
                            }
                        }
                    }
                    else
                    {
                        new LpColorSchemeService().Delete(block.LandingId, block.Id);
                    }
                }

                LpSiteService.UpdateModifiedDateByLandingId(block.LandingId);

                return new ResultModel() {Result = true};
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            return new ResultModel();
        }

        private string MergeSettings(string oldSettings, Dictionary<string, object> settingsNew)
        {
            var settingsOld =
                !string.IsNullOrEmpty(oldSettings)
                    ? JsonConvert.DeserializeObject<Dictionary<string, object>>(oldSettings)
                    : null;

            if (settingsOld == null)
                settingsOld = new Dictionary<string, object>();

            foreach (var key in settingsNew.Keys)
            {
                if (settingsOld.ContainsKey(key))
                    settingsOld[key] = settingsNew[key];
                else
                    settingsOld.Add(key, settingsNew[key]);
            }

            return JsonConvert.SerializeObject(settingsOld);
        }
    }
}
