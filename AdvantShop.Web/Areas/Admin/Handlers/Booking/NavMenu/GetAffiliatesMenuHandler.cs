using System.Linq;
using AdvantShop.Core.Services.Booking;
using AdvantShop.Customers;
using AdvantShop.Web.Admin.ViewModels.Booking;

namespace AdvantShop.Web.Admin.Handlers.Booking.NavMenu
{
    public class GetAffiliatesMenuHandler
    {
        private readonly Customer _currentCustomer;
        private Core.Services.Booking.Affiliate _selectedAffiliate;
        private readonly bool _showByAccess;

        public GetAffiliatesMenuHandler(Core.Services.Booking.Affiliate selectedAffiliate, bool showByAccess = true)
        {
            _selectedAffiliate = selectedAffiliate;
            _showByAccess = showByAccess;
            _currentCustomer = CustomerContext.CurrentCustomer;
        }

        public AffiliatesMenuModel Execute()
        {
            var model = new AffiliatesMenuModel()
            {
                SelectedAffiliate = _selectedAffiliate,
            };

            Manager currentManager = _currentCustomer.IsManager ? ManagerService.GetManager(_currentCustomer.Id) : null;

            model.Affiliates = AffiliateService.GetList()
                .Where(x => !_showByAccess || AffiliateService.CheckAccess(x, currentManager, true))
                .Select(x =>
                {
                    var item = new AffiliateMenu
                    {
                        Id = x.Id,
                        Name = x.Name,
                        SortOrder = x.SortOrder,
                        Enabled = x.Enabled,
                    };

                    item.CountNewBooking = BookingService.GetLastBookingCount(x.Id,
                        !_currentCustomer.IsAdmin && currentManager != null ? currentManager.ManagerId : (int?)null);

                    return item;
                })
                .ToList();

            return model;
        }
    }
}
