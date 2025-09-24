using AdvantShop.Configuration;
using AdvantShop.Core.Services.Booking;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.Services.Statistic;
using AdvantShop.Customers;
using AdvantShop.Web.Admin.ViewModels.Shared.Common;

namespace AdvantShop.Web.Admin.Handlers.Shared.Common
{
    public class GetLastStastics
    {
        public LastStatisticsViewModel Execute()
        {
            var model = new LastStatisticsViewModel();

            var currentCustomer = CustomerContext.CurrentCustomer;
            var currentManager = ManagerService.GetManager(currentCustomer.Id);

            if (currentCustomer.HasRoleAction(RoleAction.Tasks) && currentManager != null)
                model.LastTasksCount = TaskService.GetOpenTasksCount(currentManager.ManagerId);

            // статистика для админа без учета менеджера
            var managerId = currentCustomer.IsAdmin || currentManager == null ? (int?)null : currentManager.ManagerId;

            if (currentCustomer.HasRoleAction(RoleAction.Orders))
                model.LastOrdersCount = StatisticService.GetLastOrdersCount(managerId);
            if (currentCustomer.HasRoleAction(RoleAction.Crm))
                model.LastLeadsCount = LeadService.GetNewLeadsCount(managerId);
            if (currentCustomer.HasRoleAction(RoleAction.Catalog))
                model.LastReviews = StatisticService.GetLastReviewsCount();
            if (currentCustomer.HasRoleAction(RoleAction.Booking))
                model.LastBookingCount = BookingService.GetLastBookingCount(managerId: managerId);

            model.CongratulationsSteps = (!SettingsCongratulationsDashboard.StoreInfoDone ? 1 : 0) +
                                         (!SettingsCongratulationsDashboard.ProductDone ? 1 : 0) +
                                         (!SettingsCongratulationsDashboard.DesignDone ? 1 : 0) +
                                         (!SettingsCongratulationsDashboard.DomainDone ? 1 : 0);

            return model;
        }
    }
}
