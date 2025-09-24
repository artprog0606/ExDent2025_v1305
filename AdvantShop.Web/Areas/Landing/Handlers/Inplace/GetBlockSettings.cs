using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.App.Landing.Domain;
using AdvantShop.App.Landing.Domain.Common;
using AdvantShop.App.Landing.Models.Inplace;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Core.Services.Crm.SalesFunnels;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.Services.Landing.Blocks;
using AdvantShop.Core.Services.Landing.Forms;
using AdvantShop.Diagnostics;
using Newtonsoft.Json;

namespace AdvantShop.App.Landing.Handlers.Inplace
{
    public class GetBlockSettings
    {
        #region Ctor

        private readonly int _blockId;
        private readonly LpBlockService _blockService;
        private readonly LpFormService _formService;
        private readonly LpService _lpService;

        public GetBlockSettings(int blockId)
        {
            _blockId = blockId;
            _blockService = new LpBlockService();
            _formService = new LpFormService();
            _lpService = new LpService();
        }

        #endregion

        public BlockSettingsModel Execute()
        {
            var block = _blockService.Get(_blockId);
            if (block == null)
                return null;

            try
            {
                var settings = JsonConvert.DeserializeObject<Dictionary<string, object>>(block.Settings);

                if (settings != null)
                {
                    if (!settings.ContainsKey("show_on_all_landing_pages_top"))
                        settings.Add("show_on_all_landing_pages_top", block.ShowOnAllPagesTop);
                    
                    if (!settings.ContainsKey("show_on_all_landing_pages_bottom"))
                        settings.Add("show_on_all_landing_pages_bottom", block.ShowOnAllPagesBottom);
                }

                var model = new BlockSettingsModel()
                {
                    BlockId = block.Id,
                    Name = block.Name,
                    Type = block.Type,
                    Settings = settings,

                    Subblocks = _blockService.GetSubBlocks(block.Id).Select(x => new SubBlockSettingsModel()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Type = x.Type,
                        Settings = JsonConvert.DeserializeObject<Dictionary<string, object>>(x.Settings),
                        ContentHtml = x.ContentHtml
                    }).ToList(),
                };

                var form = _formService.GetByBlock(block.Id);
                if (form != null)
                {
                    model.Form = form;
                    model.PostActions = new List<AdvListItem>()
                    {
                        new AdvListItem("Показать сообщение", (int) FormPostAction.ShowMessage),
                        new AdvListItem("Переход на страницу", (int) FormPostAction.RedrectToUrl),
                        new AdvListItem("Переход на страницу и отправка письма", (int)FormPostAction.RedrectToUrlAndEmail)
                    };

                    var addToCardActionHide = settings != null && settings.ContainsKey("add_to_card_action_hide") &&
                                                  Convert.ToBoolean(settings["add_to_card_action_hide"]);

                    if (!addToCardActionHide)
                    {
                        model.PostActions.Add(new AdvListItem("Добавить товар в корзину и перейти на оформление",
                            (int) FormPostAction.AddToCartAndRedrectToUrl));
                    }

                    if ((settings != null && settings.ContainsKey("showRedirectToCheckout") && Convert.ToBoolean(settings["showRedirectToCheckout"])) ||
                        block.Name == "bookingResourcesWithServices")
                    {
                        model.PostActions.Add(new AdvListItem("Переход на оплату", (int)FormPostAction.RedirectToCheckout));
                    }

                    if (model.Form.SalesFunnelId == null)
                        model.Form.SalesFunnelId = SettingsCrm.DefaultSalesFunnelId;

                    var ignoreLeadFields = (settings != null && settings.ContainsKey("hide_lead_settings") && Convert.ToBoolean(settings["hide_lead_settings"])) || block.Name == "bookingResourcesWithServices";
                    model.CrmFields = _formService.GetAllFieldsList(model.Form.SalesFunnelId, ignoreLeadFields);
                    model.SalesFunnels = SalesFunnelService.GetList();
                }

                var lp = _lpService.Get(block.LandingId);
                if (lp != null)
                {
                    var landings =
                        _lpService.GetList(lp.LandingSiteId)
                            .Where(x => x.Id != lp.Id)
                            .Select(x => new AdvListItem(x.Name, x.Id))
                            .ToList();

                    model.Landings = new List<AdvListItem>() {new AdvListItem("Не выбран", "")};
                    model.Landings.AddRange(landings);

                    model.PostMessageRedirectLps = new List<AdvListItem>() { new AdvListItem("Указать свой URL-адрес", default(string)) };
                    model.PostMessageRedirectLps.AddRange(landings.Select(x => new AdvListItem(x.Label, x.Value.ToString())));

                    model.LandingsForUrl = new List<AdvListItem>() { new AdvListItem("Указать свой URL-адрес", default(int?)) };
                    model.LandingsForUrl.AddRange(landings);
                }

                if (block.Type == "reference" && model.Settings != null && model.Settings.ContainsKey("blockId"))
                    AddReferenceItems(model);

                // ссылочные блоки, в которых используется данный блок
                model.ReferenceBlocks =
                    _blockService.GetReferenceBlocksWithBlock(block.Id)
                        .Select(x => new ReferenceBlockItem() {Id = x.Id, Url = _lpService.GetLpLink(x.LandingId)})
                        .ToList();

                return model;
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            return null;
        }

        private void AddReferenceItems(BlockSettingsModel model)
        {
            var ids = model.Settings["blockId"] as string;
            if (string.IsNullOrWhiteSpace(ids))
                return;

            var listBlocks = new List<string>();

            foreach (var id in ids.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries))
            {
                var blockItem = _blockService.Get(id.TryParseInt());
                if (blockItem == null)
                {
                    listBlocks.Add(string.Format("{0} - не найдено ", id.Trim()));
                    continue;
                }

                var lpItem = _lpService.Get(blockItem.LandingId);
                if (lpItem == null)
                    continue;

                var siteItem = new LpSiteService().Get(lpItem.LandingSiteId);
                if (siteItem == null)
                    continue;

                listBlocks.Add(
                    string.Format(
                        "{0} - Воронка \"{1}\" страница \"<a href=\"{2}#block_{0}\" target=\"_blank\">{3}</a>\" ",
                        blockItem.Id, siteItem.Name, _lpService.GetLpLink(lpItem.Id), lpItem.Name));
            }

            if (model.Settings.ContainsKey("blockIdItems"))
                model.Settings["blockIdItems"] = listBlocks;
            else
                model.Settings.Add("blockIdItems", listBlocks);
        }
    }
}
