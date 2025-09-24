using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Booking;

namespace AdvantShop.App.Landing.Domain.Booking
{
    public class LpReservationResourceService
    {
        public static List<LpReservationResource> GetByIds(List<int> ids)
        {
            if (ids == null || ids.Count == 0)
                return null;

            var resources = ReservationResourceService.GetList().Where(res => res.Enabled && ids.Contains(res.Id)).ToList();
            return resources.Select(resource => (LpReservationResource)resource).ToList();
        }

        public static List<LpReservationResource> GetLpReservationResources(int affiliateId)
        {
            var resources = ReservationResourceService.GetByAffiliate(affiliateId, onlyActive: false);
            return resources.Select(resource => (LpReservationResource)resource).ToList();
        }
    }
}
