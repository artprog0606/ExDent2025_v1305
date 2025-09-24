using System;
using System.Linq;
using AdvantShop.Core;
using AdvantShop.Core.Services.Domains;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.Services.Landing.Domains;
using AdvantShop.Diagnostics;

namespace AdvantShop.Web.Admin.Handlers.Landings
{
    public class SaveSiteDomain
    {
        private readonly int _id;
        private readonly string _domain;
        private readonly bool _isAdditional;

        private readonly LpSiteService _siteService;
        private readonly DomainService _domainService;
        private readonly LpDomainService _lpDomainService;

        public SaveSiteDomain(int id, string domain, bool? isAdditional)
        {
            _id = id;
            _domain = domain;
            _isAdditional = isAdditional.HasValue && isAdditional.Value;

            _siteService = new LpSiteService();
            _domainService = new DomainService();
            _lpDomainService = new LpDomainService();
        }

        public bool Execute()
        {
            var site = _siteService.Get(_id);
            if (site == null)
                throw new BlException("Лендинг не найден");

            if (string.IsNullOrWhiteSpace(_domain))
                throw new BlException("Укажите доменное имя");

            var domain = _domain.Replace("http://", "").Replace("https://", "").Replace("www.", "").Trim('/').ToLower();
            
            if (!domain.Contains("."))
                throw new BlException("Укажите правильное доменное имя");
            
            
            var domainInDb = _lpDomainService.Get(_domain);
            if (domainInDb != null)
            {
                var domainSite = _siteService.Get(domainInDb.LandingSiteId);
                if (domainSite != null)
                    throw new BlException("Домен уже используется в лендинге \"" + domainSite.Name + "\"");
            }

            try
            {
                _domainService.Add(domain);

                var lpDomain = new LpDomain()
                {
                    LandingSiteId = site.Id,
                    DomainUrl = domain,
                    IsMain = !_lpDomainService.GetList(site.Id).Any(x => x.IsMain)
                };

                _lpDomainService.Add(lpDomain);

                if (lpDomain.IsMain)
                {
                    site.DomainUrl = domain;
                    _siteService.Update(site);
                }

                Track.TrackService.TrackEvent(Track.ETrackEvent.Shop_Funnels_BindDomain);
            }
            catch (Exception ex)
            {
                var msg = "Ошибка при добавлении домена " + domain + " в лендинге";

                Debug.Log.Error(msg, ex);
                throw new BlException(msg);
            }

            return true;
        }
    }
}
