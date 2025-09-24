using System;
using System.Linq;
using AdvantShop.App.Landing.Extensions;
using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Crm.SalesFunnels;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.Services.Landing.Blocks;
using AdvantShop.Core.Services.Landing.Forms;
using AdvantShop.Diagnostics;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.App.Landing.Handlers.Landings
{
    public class SaveFormSettings : ICommandHandler<bool>
    {
        private readonly LpForm _model;
        private readonly LpFormService _formService;
        private readonly LpBlockService _blockService;

        public SaveFormSettings(LpForm model)
        {
            _model = model;
            _formService = new LpFormService();
            _blockService = new LpBlockService();
        }

        public bool Execute()
        {
            var form = _formService.Get(_model.Id);
            if (form == null)
                throw new BlException("Форма не найдена");

            form.Title = _model.Title.DefaultOrEmpty();
            form.SubTitle = _model.SubTitle.DefaultOrEmpty();
            form.ButtonText = _model.ButtonText.DefaultOrEmpty();
            form.PostAction = _model.PostAction;
            form.DontSendLeadId = _model.DontSendLeadId;

            form.PostMessageText = _model.PostMessageText.DefaultOrEmpty();
            form.PostMessageRedirectLpId = _model.PostMessageRedirectLpId;
            form.PostMessageRedirectUrl = _model.PostMessageRedirectUrl.DefaultOrEmpty();
            form.PayProductOfferId = _model.PayProductOfferId;

            form.YaMetrikaEventName = _model.YaMetrikaEventName.DefaultOrEmpty();
            form.GaEventCategory = _model.GaEventCategory.DefaultOrEmpty();
            form.GaEventAction = _model.GaEventAction.DefaultOrEmpty();
            form.ShowAgreement = _model.ShowAgreement;
            form.AgreementDefaultChecked = _model.AgreementDefaultChecked;
            form.AgreementText = _model.AgreementText;
            form.EmailText = _model.EmailText;
            form.EmailSubject = _model.EmailSubject;

            form.OfferId = _model.OfferId;
            form.OfferPrice = _model.OfferPrice;
            form.OfferItems = _model.OfferItems;

            form.Fields = _model.Fields;

            form.InputTextPosition = _model.InputTextPosition;
            form.ActionUpsellLpId = _model.ActionUpsellLpId;
            form.PostMessageRedirectDelay = _model.PostMessageRedirectDelay;
            form.PostMessageRedirectShowMessage = _model.PostMessageRedirectShowMessage;

            var funnels = SalesFunnelService.GetList();
            form.SalesFunnelId =
                _model.SalesFunnelId != null && funnels != null && funnels.Any(x => x.Id == _model.SalesFunnelId.Value)
                    ? _model.SalesFunnelId
                    : null;

            _formService.Update(form);

            if (form.BlockId != null)
            {
                var block = _blockService.Get(form.BlockId.Value);
                if (block != null)
                {
                    try
                    {
                        var button = block.TryGetSetting<LpButton>("button");
                        if (button != null && button.Action == "Form" && button.ActionForm != form.Id.ToString())
                        {
                            button.ActionForm = form.Id.ToString();
                            block.TrySetSetting("button", button);

                            _blockService.Update(block);
                        }
                        
                        var items = block.TryGetSetting("items");
                        if (items != null)
                        {
                            foreach (var item in items)
                                if (item.button != null)
                                    item.button.action_form = form.Id.ToString();

                            block.TrySetSetting("items", (object)items);

                            _blockService.Update(block);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.Log.Error(ex);
                    }
                }
            }

            LpSiteService.UpdateModifiedDateByLandingId(form.LpId);

            return true;
        }
    }
}
