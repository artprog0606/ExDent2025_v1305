using System;
using System.Linq;
using AdvantShop.App.Landing.Domain;
using AdvantShop.Core;
using AdvantShop.Core.Services.Domains;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.Services.Landing.Domains;
using AdvantShop.Diagnostics;

namespace AdvantShop.Web.Admin.Handlers.Landings
{
    public class RemoveSiteDomain
    {
        private readonly int _id;
        private readonly string _domain;
        private readonly LpSiteService _siteService;
        private readonly LpDomainService _lpDomainService;

        public RemoveSiteDomain(int id, string domain)
        {
            _id = id;
            _domain = domain;
            _siteService = new LpSiteService();
            _lpDomainService = new LpDomainService();
        }

        public bool Execute()
        {
            var site = _siteService.Get(_id);
            if (site == null)
                throw new BlException("Лендинг не найден");

            try
            {
                new DomainService().Remove(site.DomainUrl);

                var domainInDb = _lpDomainService.Get(_domain);
                if (domainInDb != null)
                {
                    if (site.DomainUrl == domainInDb.DomainUrl)
                    {
                        site.DomainUrl = "";
                        _siteService.Update(site);
                    }

                    _lpDomainService.Delete(domainInDb.Id);

                    if (domainInDb.IsMain)
                    {
                        var d = _lpDomainService.GetList(site.Id).FirstOrDefault();
                        if (d != null)
                        {
                            d.IsMain = true;
                            _lpDomainService.Update(d);

                            site.DomainUrl = d.DomainUrl;
                            _siteService.Update(site);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var msg = "Ошибка при удалении домена " + _domain + " в лендинге " + ex.Message;

                Debug.Log.Error(msg, ex);
                throw new BlException(msg);
            }

            return true;
        }
    }
}
