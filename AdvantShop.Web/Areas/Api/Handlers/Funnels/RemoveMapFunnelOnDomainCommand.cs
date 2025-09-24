using System;
using System.Linq;
using AdvantShop.Areas.Api.Model.Funnels;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.Services.Domains;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.Services.Landing.Domains;
using AdvantShop.Diagnostics;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Funnels
{
    public class RemoveMapFunnelOnDomainCommand: ICommandHandler<RemoveMapFunnelOnDomainDto, bool>
    {
        private readonly LpDomainService _lpDomainService;
        //private readonly DomainService _domainService;
        private readonly LpSiteService _siteService;
        //private LpSite _site;

        public RemoveMapFunnelOnDomainCommand()
        {
            //_domainService = new DomainService();
            _siteService = new LpSiteService();
            _lpDomainService = new LpDomainService();
        }

        public bool Execute(RemoveMapFunnelOnDomainDto obj)
        {
            if (SettingsLic.LicKey != obj.Lickey)
                throw new BlException("wrong lickey");

            var site = _siteService.Get(obj.FunnelId);
            if (site == null)
                return true;

            try
            {
                var domainInDb = _lpDomainService.Get(obj.Domain);
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
                var msg = "Ошибка при удалении домена " + obj.Domain + " в лендинге";

                Debug.Log.Error(msg, ex);
                throw new BlException(msg);
            }

            return true;
        }
    }
}
