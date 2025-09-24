using System;
using System.Collections.Generic;
using AdvantShop.App.Landing.Domain.Settings;
using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.Services.Landing.Blocks;
using AdvantShop.Core.Services.Landing.Forms;
using AdvantShop.Diagnostics;
using AdvantShop.Web.Infrastructure.Handlers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AdvantShop.Web.Admin.Handlers.Landings
{
    public class CreateCopyLandingSite : AbstractCommandHandler<int>
    {
        #region Ctor

        private readonly int _landingSiteId;

        private readonly LpSiteService _siteService;
        private readonly LpService _lpService;
        private readonly LpSiteSettingsService _settingsService;
        private readonly LpFormService _formService;
        private readonly LpBlockService _blockService;

        private readonly Dictionary<int, CopyLandingPageResult> _pages = new Dictionary<int, CopyLandingPageResult>();

        public CreateCopyLandingSite(int landingSiteId)
        {
            _landingSiteId = landingSiteId;

            _siteService = new LpSiteService();
            _lpService = new LpService();
            _settingsService = new LpSiteSettingsService();
            _formService = new LpFormService();
            _blockService = new LpBlockService();
        }

        #endregion

        protected override int Handle()
        {
            var sourceSite = _siteService.Get(_landingSiteId);
            if (sourceSite == null)
                throw new BlException(T("Admin.Landings.CreateCopyLandingSite.FunnelNotFound"));

            var site = (LpSite) sourceSite.Clone();

            site.Name = site.Name.Reduce(100) + T("Admin.Landings.CreateCopyLandingSite.Copy");
            site.Url = _siteService.GetAvailableUrl(site.Url.Reduce(100));
            site.DomainUrl = null;

            _siteService.Add(site);
            
            foreach (var setting in _settingsService.GetList(sourceSite.Id))
            {
                _settingsService.AddOrUpdate(site.Id, setting.Name, setting.Value);
            }
            

            foreach (var page in _lpService.GetList(sourceSite.Id))
            {
                var copyPageResult = new CreateCopyLandingPage(page, site.Id).Execute();
                _pages.Add(page.Id, copyPageResult);
            }


            // post process in new pages and blocks

            foreach (var page in _lpService.GetList(site.Id))
            {
                foreach (var block in _blockService.GetList(page.Id))
                    PostProcessBlockSettings(block);

                // update forms
                foreach (var form in _formService.GetListByLandingPageId(page.Id))
                {
                    if (form.PostMessageRedirectLpId != null)
                    {
                        int pageId;
                        if (TryGetPageId(form.PostMessageRedirectLpId.TryParseInt(), out pageId))
                        {
                            form.PostMessageRedirectLpId = pageId.ToString();
                            _formService.Update(form);
                        }
                    }
                }
            }

            return site.Id;
        }

        private void PostProcessBlockSettings(LpBlock block)
        {
            try
            {
                var needUpdate = false;

                dynamic settings = JObject.Parse(block.Settings);

                if (settings != null)
                {
                    if (settings.button != null)
                        needUpdate |= UpdateButton(settings.button);

                    if (settings.button2 != null)
                        needUpdate |= UpdateButton(settings.button2);
                }

                if (needUpdate)
                {
                    block.Settings = JsonConvert.SerializeObject(settings);
                    _blockService.Update(block);
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }
        }

        private bool UpdateButton(dynamic button)
        {
            var needUpdate = false;

            if (button.action_upsell_lp_id != null)
            {
                int pageId;
                var actionUpsellLpId = ((object) button.action_upsell_lp_id).ToString().TryParseInt();

                if (TryGetPageId(actionUpsellLpId, out pageId))
                {
                    needUpdate = true;
                    button.action_upsell_lp_id = pageId;
                }
            }

            if (button.action_url_lp_id != null)
            {
                int pageId;
                var actionUrlLpId = ((object)button.action_url_lp_id).ToString().TryParseInt();

                if (TryGetPageId(actionUrlLpId, out pageId))
                {
                    needUpdate = true;
                    button.action_url_lp_id = pageId;
                }
            }

            if (button.action_form != null)
            {
                int formId;
                var actionForm = ((object) button.action_form).ToString().TryParseInt();

                if (TryGetFormId(actionForm, out formId))
                {
                    needUpdate = true;
                    button.action_form = formId;
                }
            }

            return needUpdate;
        }

        private bool TryGetPageId(int oldPageId, out int newPageId)
        {
            CopyLandingPageResult result;
            if (_pages.TryGetValue(oldPageId, out result))
            {
                newPageId = result.Lp.Id;
                return true;
            }
            newPageId = 0;
            return false;
        }

        private bool TryGetFormId(int oldFormId, out int newFormId)
        {
            foreach (var page in _pages)
            {
                if (page.Value.Forms.TryGetValue(oldFormId, out newFormId))
                    return true;
            }
            newFormId = 0;
            return false;
        }
    }
}
