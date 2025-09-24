using System.Linq;
using AdvantShop.Core.Services.Booking;
using AdvantShop.Customers;
using AdvantShop.Web.Admin.ViewModels.Booking;

namespace AdvantShop.Web.Admin.Handlers.Booking.NavMenu
{
    public class GetMenuJsonHandler
    {
        private readonly Customer _customer;
        private Core.Services.Booking.Affiliate _selectedAffiliate;
        private readonly bool _showByAccess;
        private readonly bool _isOpen;

        public GetMenuJsonHandler(Core.Services.Booking.Affiliate selectedAffiliate, bool showByAccess = true, bool isOpen = false)
        {
            _selectedAffiliate = selectedAffiliate;
            _showByAccess = showByAccess;
            _customer = CustomerContext.CurrentCustomer;
            _isOpen = isOpen;
        }

        public MenuJsonModel Execute()
        {
            var model = new MenuJsonModel()
            {
                SelectedAffiliate = _selectedAffiliate,
                AccessToEditing = !_showByAccess || (_selectedAffiliate != null && AffiliateService.CheckAccessToEditing(_selectedAffiliate)),
                AccessToAnalytic = !_showByAccess || (_selectedAffiliate != null && AffiliateService.CheckAccessToAnalytic(_selectedAffiliate)),
                AccessToSettings = !_showByAccess || _customer.IsAdmin || RoleActionService.GetCustomerRoleActionsByCustomerId(_customer.Id).Any(x => x.Role == RoleAction.Settings),
                IsOpen = _isOpen
            };

            return model;
        }
    }
}
