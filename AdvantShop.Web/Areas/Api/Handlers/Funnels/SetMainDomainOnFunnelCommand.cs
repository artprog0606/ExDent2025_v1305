using System.Linq;
using AdvantShop.Areas.Api.Model.Funnels;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.Services.Landing.Domains;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Funnels
{
    public class SetMainDomainOnFunnelCommand : ICommandHandler<SetMainDomainOnFunnelDto, bool>
    {
        private readonly LpDomainService _lpDomainService;
        private readonly LpSiteService _siteService;
        private LpSite _site;

        public SetMainDomainOnFunnelCommand()
        {
            _siteService = new LpSiteService();
            _lpDomainService = new LpDomainService();
        }

        public bool Execute(SetMainDomainOnFunnelDto model)
        {
            if (SettingsLic.LicKey != model.Lickey)
                throw new BlException("wrong lickey");

            _site = _siteService.Get(model.FunnelId);
            var domainList = _lpDomainService.GetList(model.FunnelId);
            var current = domainList.FirstOrDefault(x => x.DomainUrl == model.Domain);

            if (current == null)
                throw new BlException("Домен отсутствует \"" + model.Domain + "\""); ;

            if (current.IsMain)
                return true;

            var currentMain = domainList.FirstOrDefault(x => x.IsMain);
            if (currentMain != null)
            {
                currentMain.IsMain = false;
                _lpDomainService.Update(currentMain);
            }

            current.IsMain = true;
            _lpDomainService.Update(current);
            _site.DomainUrl = model.Domain;
            _siteService.Update(_site);

            return true;
        }
    }
}