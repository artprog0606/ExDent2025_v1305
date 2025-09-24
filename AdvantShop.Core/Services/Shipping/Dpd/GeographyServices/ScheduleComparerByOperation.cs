using System;
using System.Collections.Generic;
using AdvantShop.Shipping.Dpd.Api.Geography;

namespace AdvantShop.Shipping.Dpd.GeographyServices
{
    public class ScheduleComparerByOperation : IEqualityComparer<schedule>
    {
        public bool Equals(schedule x, schedule y)
        {
            return string.Equals(x.operation, y.operation, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(schedule obj)
        {
            return obj.operation.GetHashCode();
        }
    }
}
