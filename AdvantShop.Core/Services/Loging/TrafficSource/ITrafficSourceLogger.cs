using System;
using System.Collections.Generic;

namespace AdvantShop.Core.Services.Loging.TrafficSource
{
    public interface ITrafficSourceLogger : IAdvantShopLogger
    {
        void LogTrafficSource();

        void LogOrderTafficSource(int objId, TrafficSourceType type, bool isFromAdminArea);

        List<TrafficSource> GetTrafficSources(Guid customerId);
    }
}