using System.Linq;
using AdvantShop.Core.Services.Booking;
using AdvantShop.Customers;
using AdvantShop.Web.Admin.ViewModels.Booking;

namespace AdvantShop.Web.Admin.Handlers.Booking.NavMenu
{
    public class GetNavMenuHandler
    {
        private readonly Customer _currentCustomer;
        private Core.Services.Booking.Affiliate _selectedAffiliate;
        private readonly bool _showByAccess;

        public GetNavMenuHandler(Core.Services.Booking.Affiliate selectedAffiliate, bool showByAccess = true)
        {
            _selectedAffiliate = selectedAffiliate;
            _showByAccess = showByAccess;
            _currentCustomer = CustomerContext.CurrentCustomer;
        }

        public NavMenuModel Execute()
        {
            Manager currentManager = _currentCustomer.IsManager ? ManagerService.GetManager(_currentCustomer.Id) : null;

            var model = new NavMenuModel()
            {
                SelectedAffiliate = _selectedAffiliate,
                AccessToEditing = !_showByAccess || (_selectedAffiliate != null && AffiliateService.CheckAccessToEditing(_selectedAffiliate, currentManager)),
                AccessToAnalytic = !_showByAccess || (_selectedAffiliate != null && AffiliateService.CheckAccessToAnalytic(_selectedAffiliate, currentManager)),
                AccessToSettings = !_showByAccess || _currentCustomer.IsAdmin || RoleActionService.GetCustomerRoleActionsByCustomerId(_currentCustomer.Id).Any(x => x.Role == RoleAction.Settings)
            };

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
                        !_currentCustomer.IsAdmin && currentManager != null ? currentManager.ManagerId : (int?) null);

                    return item;
                })
                .ToList();

            return model;
        }
    }
}
