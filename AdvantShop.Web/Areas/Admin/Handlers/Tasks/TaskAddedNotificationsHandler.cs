using System.Linq;
using AdvantShop.CMS;
using AdvantShop.Core.Services.ChangeHistories;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Customers;
using AdvantShop.Web.Admin.Handlers.Shared.AdminNotifications;

namespace AdvantShop.Web.Admin.Handlers.Tasks
{
    public class TaskAddedNotificationsHandler
    {
        private readonly Task _task;

        public TaskAddedNotificationsHandler(Task task)
        {
            _task = task;
        }

        public bool Execute()
        {
            if (_task.IsDeferred)
                return false;

            var modifier = CustomerContext.CurrentCustomer;

            var notificationsHandler = new AdminNotificationsHandler();
            foreach (var manager in _task.Managers.Where(x => x.CustomerId != modifier.Id && x.HasRoleAction(RoleAction.Tasks)))
            {
                notificationsHandler.NotifyCustomers(new OnSetTaskNotification(_task, modifier), manager.CustomerId);
            }
            notificationsHandler.UpdateTasks();

            return true;
        }
    }
}
