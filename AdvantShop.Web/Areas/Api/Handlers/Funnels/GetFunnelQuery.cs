using System.Collections.Generic;
using System.Linq;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Funnels
{
    public class GetFunnelQuery : ICommandHandler<string, IList<MapFunnelsDto>>
    {
        private readonly LpSiteService _siteService;

        public GetFunnelQuery()
        {
            _siteService = new LpSiteService();
        }

        public IList<MapFunnelsDto> Execute(string lickey)
        {
            if (SettingsLic.LicKey != lickey)
                throw new BlException("wrong lickey");

            var data = _siteService.GetList().Select(x => new MapFunnelsDto
            {
                FunnelId = x.Id,
                Name = x.Name,
                Url = x.Url
            }).ToList();

            return data;
        }
    }

    public class MapFunnelsDto
    {
        public int FunnelId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
    }
}
