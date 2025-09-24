using AdvantShop.Core;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.Services.Landing.Domains;

namespace AdvantShop.Web.Admin.Handlers.Landings
{
    public class SaveReuseSiteDomain
    {
        private readonly int _id;
        private readonly string _reuseDomain;

        private readonly LpSiteService _siteService;
        private readonly LpDomainService _lpDomainService;

        public SaveReuseSiteDomain(int id, string reuseDomain)
        {
            _id = id;
            _reuseDomain = reuseDomain;

            _siteService = new LpSiteService();
            _lpDomainService = new LpDomainService();
        }

        public bool Execute()
        {
            var site = _siteService.Get(_id);
            if (site == null)
                throw new BlException("Лендинг не найден");

            if (string.IsNullOrWhiteSpace(_reuseDomain))
                throw new BlException("Укажите доменное имя");
            
            var domain = _lpDomainService.Get(_reuseDomain);
            if (domain == null)
                throw new BlException("Домен не найден");
            
            var domainSite = _siteService.Get(domain.LandingSiteId);
            if (domainSite != null)
            {
                if (domainSite.DomainUrl == domain.DomainUrl)
                {
                    domainSite.DomainUrl = "";
                    _siteService.Update(domainSite);
                }
            }

            domain.LandingSiteId = site.Id;
            domain.IsMain = string.IsNullOrEmpty(site.DomainUrl);
            _lpDomainService.Update(domain);

            if (string.IsNullOrEmpty(site.DomainUrl))
            {
                site.DomainUrl = domain.DomainUrl;
                _siteService.Update(site);
            }

            return true;
        }
    }
}
