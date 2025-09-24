using System.Linq;
using AdvantShop.App.Landing.Domain.Settings;
using AdvantShop.App.Landing.Handlers.Inplace;
using AdvantShop.App.Landing.Models.Inplace;
using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.Services.Landing.Templates;
using AdvantShop.Core.Services.Screenshot;
using AdvantShop.Helpers;
using AdvantShop.Web.Admin.Models.Landings;

namespace AdvantShop.Web.Admin.Handlers.Landings
{
    public class InstallTemplateHandler
    {
        #region Ctor
        
        private readonly LandingAdminIndexPostModel _model;

        private readonly LpService _lpService;
        private readonly LpSiteService _siteService;
        private readonly LpTemplateService _templateService;

        public InstallTemplateHandler(LandingAdminIndexPostModel model)
        {
            _model = model;

            _lpService = new LpService();
            _siteService = new LpSiteService();
            _templateService = new LpTemplateService();
        }

        #endregion

        public Lp Execute()
        {
            var lp = new Lp()
            {
                Name = StringHelper.RemoveHTML(_model.Name).Reduce(100),
                Template = _model.Template,
                Enabled = true,
                IsMain = _model.SiteId == null,
                LandingSiteId = _model.SiteId ?? 0
            };

            if (!string.IsNullOrEmpty(_model.Template))
            {
                if (_templateService.GetTemplate(lp.Template) == null)
                    throw new BlException("Шаблон не существует");
            }
            else
            {
                var t = _templateService.GetTemplates().FirstOrDefault();
                if (t != null)
                    lp.Template = t.Key;
            }
            
            if (lp.LandingSiteId == 0)
            {
                lp.LandingSiteId = _siteService.Add(new LpSite()
                {
                    Name = lp.Name,
                    Template = lp.Template,
                    Enabled = true,
                    Url = _siteService.GetAvailableUrl(lp.Name)
                });
                Track.TrackService.TrackEvent(Track.ETrackEvent.Shop_Funnels_FunnelCreated, lp.Template);
            }

            lp.Url = _lpService.GetAvailableUrl(lp.LandingSiteId, lp.Name);

            _lpService.Add(lp);

            var reGenerateCss = _lpService.GetList(lp.LandingSiteId).Count == 1;

            Track.TrackService.TrackEvent(Track.ETrackEvent.Shop_Funnels_PageCreated);

            new SaveLpPageSettings(lp.Id,
                new InplaceSettingsModel()
                {
                    LpName = lp.Name,
                    PageTitle = lp.Name,
                    FontMain = LSiteSettings.GetDefaultFonts()[0].Name
                },
                reGenerateCss).Execute();

            var site = _siteService.Get(lp.LandingSiteId);
            if (site != null)
                new ScreenshotService().UpdateFunnelScreenShotInBackground(site);

            return lp;
        }

    }
}
