using System.Linq;
using AdvantShop.Areas.Api.Model.Funnels;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.Services.Landing.Domains;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Funnels
{
    public class AddMapFunnelOnDomainCommand : ICommandHandler<AddMapFunnelOnDomainDto, bool>
    {
        private readonly LpDomainService _lpDomainService;
        private readonly LpSiteService _siteService;
        private readonly RemoveMapFunnelOnDomainCommand _removeMapFunnelOnDomainCommand;
        private LpSite _site;

        public AddMapFunnelOnDomainCommand()
        {
            _siteService = new LpSiteService();
            _lpDomainService = new LpDomainService();
            _removeMapFunnelOnDomainCommand = new RemoveMapFunnelOnDomainCommand();
        }
        
        public bool Execute(AddMapFunnelOnDomainDto model)
        {
            if (SettingsLic.LicKey != model.Lickey)
                throw new BlException("wrong lickey");
            
            if (model.Domain != null)
                model.Domain = model.Domain.ToLower();

            var removeDto = new RemoveMapFunnelOnDomainDto() { Domain = model.Domain, Lickey = model.Lickey };
            _removeMapFunnelOnDomainCommand.Execute(removeDto);

            _site = _siteService.Get(model.FunnelId);

            Validate(model);

            var lpDomain = new LpDomain()
            {
                LandingSiteId = model.FunnelId,
                DomainUrl = model.Domain,
                IsMain = !_lpDomainService.GetList(model.FunnelId).Any(x => x.IsMain)
            };

            _lpDomainService.Add(lpDomain);

            if (!lpDomain.IsMain)
                return true;

            _site.DomainUrl = model.Domain;
            _siteService.Update(_site);

            return true;
        }

        private void Validate(AddMapFunnelOnDomainDto model)
        {
            if (_site == null)
                throw new BlException("Лендинг не найден");

            if (string.IsNullOrWhiteSpace(model.Domain))
                throw new BlException("Укажите доменное имя");

            var domain = model.Domain.Replace("http://", "").Replace("https://", "").Replace("www.", "").Trim('/').ToLower();

            if (!domain.Contains("."))
                throw new BlException("Укажите правильное доменное имя");

            var domainInDb = _lpDomainService.Get(model.Domain);
            if (domainInDb == null) return;
            var domainSite = _siteService.Get(domainInDb.LandingSiteId);
            if (domainSite != null)
                throw new BlException("Домен уже используется в лендинге \"" + domainSite.Name + "\"");
        }
    }
}
