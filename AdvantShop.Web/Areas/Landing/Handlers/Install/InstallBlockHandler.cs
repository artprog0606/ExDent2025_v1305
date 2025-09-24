using System;
using System.Collections.Generic;
using AdvantShop.App.Landing.Extensions;
using AdvantShop.App.Landing.Domain.Blocks;
using AdvantShop.App.Landing.Domain.ColorSchemes;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.Services.Landing.Blocks;
using AdvantShop.Core.Services.Landing.Forms;
using AdvantShop.Diagnostics;
using Newtonsoft.Json;

namespace AdvantShop.App.Landing.Handlers.Install
{
    public class InstallBlockHandler
    {
        #region Ctor

        private readonly string _blockName;
        private readonly string _template;
        private readonly int _landingId;
        private readonly int _sortOrder;
        private readonly LpConfiguration _configuration;
        private readonly LpBlockService _lpBlockService;

        

        public InstallBlockHandler(string blockName, string template, int landingId, int sortOrder, LpConfiguration configuration)
        {
            _blockName = blockName;
            _template = template;
            _landingId = landingId;
            _sortOrder = sortOrder;
            _configuration = configuration ?? new LpConfiguration();

            _lpBlockService = new LpBlockService();
        }

        #endregion

        public InstallBlockResult Execute()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_blockName) || string.IsNullOrWhiteSpace(_template) || _landingId == 0)
                    return new InstallBlockResult();

                var blockConfigService = new LpBlockConfigService();

                var blockConfig = blockConfigService.Get(_blockName, _template);
                if (blockConfig == null)
                    return new InstallBlockResult();

                var block = new LpBlock()
                {
                    LandingId = _landingId,
                    Name = _blockName,
                    Enabled = true,
                    Type = blockConfig.Type,
                    Settings = GetPreparedBlockSettings(blockConfig.Type, blockConfig.Settings),
                    SortOrder = _sortOrder
                };

                block.NoCache = block.TryGetSetting("no_cache") != null && block.TryGetSetting("no_cache");

                BeforeAddingBlock(block);

                _lpBlockService.Add(block);

                AfterAddingBlock(block, blockConfig.Settings);

                if (blockConfig.SubBlocks != null)
                {
                    var j = 0;
                    foreach (var subBlock in blockConfig.SubBlocks)
                    {
                        var result = new InstallSubBlock(block, subBlock, j).Execute();
                        j += 100;
                    }
                }

                return new InstallBlockResult() { Result = true, BlockId = block.Id };
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            return new InstallBlockResult();
        }
        
        private void BeforeAddingBlock(LpBlock block)
        {
            if (_configuration == null)
                return;

        }

        private void AfterAddingBlock(LpBlock block, dynamic blockSettings)
        {
            var form = block.TryGetSetting<LpForm>("form");
            if (form != null)
            {
                var formService = new LpFormService();
                
                var newForm = formService.GetDefaultForm();

                newForm.LpId = block.LandingId;
                newForm.BlockId = block.Id;

                if (form.Title != null)
                    newForm.Title = form.Title;

                if (form.SubTitle != null)
                    newForm.SubTitle = form.SubTitle;

                if (form.Fields != null && form.Fields.Count > 0)
                    newForm.Fields = form.Fields;

                if (form.ButtonText != null)
                    newForm.ButtonText = form.ButtonText;

                if (form.PostAction != FormPostAction.None)
                    newForm.PostAction = form.PostAction;

                if (form.PostMessageText != null)
                    newForm.PostMessageText = form.PostMessageText;

                if (form.PostMessageRedirectLpId != null)
                    newForm.PostMessageRedirectLpId = form.PostMessageRedirectLpId;

                if (form.PostMessageRedirectUrl != null)
                    newForm.PostMessageRedirectUrl = form.PostMessageRedirectUrl;

                if (form.ShowAgreement.HasValue)
                    newForm.ShowAgreement = form.ShowAgreement;

                if (form.AgreementText != null)
                    newForm.AgreementText = form.AgreementText;

                if (form.IsHidden)
                    newForm.IsHidden = true;

                if (form.EmailSubject != null)
                    newForm.EmailSubject = form.EmailSubject;

                if (form.EmailText != null)
                    newForm.EmailText = form.EmailText;

                //if (form.OfferId != null)
                //    newForm.OfferId = form.OfferId;

                //if (form.OfferPrice != null)
                //    newForm.OfferPrice = form.OfferPrice;
                
                if (form.OfferItems != null)
                    newForm.OfferItems = form.OfferItems;
                else if (form.OfferId != null)
                    newForm.OfferItems = new List<LpFormOfferItem>()
                    {
                        new LpFormOfferItem() {OfferId = form.OfferId, OfferPrice = form.OfferPrice}
                    };

                newForm.InputTextPosition = form.InputTextPosition;

                formService.Add(newForm);

                var button = block.TryGetSetting<LpButton>("button");
                if (button != null && button.Action == "Form")
                {
                    button.ActionForm = newForm.Id.ToString();
                    block.TrySetSetting("button", button);

                    _lpBlockService.Update(block);
                }
            }

            if (blockSettings.color_scheme != null && blockSettings.color_scheme == "color-scheme--custom" &&
                blockSettings.color_scheme_custom != null)
            {
                var colorSchemeCustom = blockSettings.color_scheme_custom.ToObject<LpColorScheme>();
                if (colorSchemeCustom != null)
                {
                    colorSchemeCustom.LpId = block.LandingId;
                    colorSchemeCustom.LpBlockId = block.Id;
                    colorSchemeCustom.Class = "color-scheme--custom";

                    new LpColorSchemeService().AddUpdate(colorSchemeCustom);
                }
            }
        }

        private ILpBlockDefaultSettings TryGetDefaultBlockSettings(string name)
        {
            var type = LpBlockService.GetTypes(typeof(ILpBlockDefaultSettings).Name).Find(x => name.Contains(x.Name.ToLower().Replace("lpblockdefaultsettings", "")));

            return type != null ? (ILpBlockDefaultSettings)Activator.CreateInstance(type) : null;
        }

        private string GetPreparedBlockSettings(string blockType, dynamic settings)
        {
            if (settings == null)
                return null;

            var blockDefaultSettings = TryGetDefaultBlockSettings(blockType);
            if (blockDefaultSettings != null)
            {
                var s = blockDefaultSettings.GetSettings(settings);
                return JsonConvert.SerializeObject(s);
            }

            return JsonConvert.SerializeObject(settings);
        }
    }
}
