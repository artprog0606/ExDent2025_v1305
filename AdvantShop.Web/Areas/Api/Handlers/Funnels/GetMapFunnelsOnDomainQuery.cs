using System.Collections.Generic;
using System.Linq;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.Services.Landing.Domains;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Funnels
{
    public class GetMapFunnelsOnDomainQuery : ICommandHandler<string, IList<MapFunnelsOnDomainDto>>
    {
        private readonly LpDomainService _lpDomainService;

        public GetMapFunnelsOnDomainQuery()
        {
            _lpDomainService = new LpDomainService();
        }

        public IList<MapFunnelsOnDomainDto> Execute(string lickey)
        {
            if (SettingsLic.LicKey != lickey)
                throw new BlException("wrong lickey");

            var result = _lpDomainService.GetList().Select(x => new MapFunnelsOnDomainDto
            {
                FunnelId = x.LandingSiteId,
                DomainName = x.DomainUrl,
                Main = x.IsMain
            }).ToList();
            return result;
        }
    }

    public class MapFunnelsOnDomainDto
    {
        public int FunnelId { get; set; }
        public string DomainName { get; set; }
        public bool Main { get; set; }
    }
}
