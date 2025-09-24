using System.Collections.Generic;
using AdvantShop.Orders;
using AdvantShop.Repository;

namespace AdvantShop.Core.Modules.Interfaces
{
    public interface IGeoIp : IModule
    {
        IpZone GetIpZone(string ip);

        List<IpZone> GetIpZonesAutocomplete(string q, bool inAdminPart);

        void OnSetZone(IpZone zone);
    }
}
