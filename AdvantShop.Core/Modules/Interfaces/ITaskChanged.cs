//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using AdvantShop.CMS;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Customers;

namespace AdvantShop.Core.Modules.Interfaces
{
    public interface ITaskChanged
    {
        void DoTaskAdded(ITask task, ICustomer managerToNotify);
        void DoTaskChanged(ITask oldTask, ITask newTask, ICustomer managerToNotify);

        void DoTaskCommentAdded(ITask task, IAdminComment comment, ICustomer managerToNotify);
    }
}