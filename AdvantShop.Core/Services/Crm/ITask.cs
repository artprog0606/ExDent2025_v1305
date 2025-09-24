using System;
using System.Collections.Generic;
using AdvantShop.CMS;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Services.Attachments;
using AdvantShop.Core.Services.Crm.BusinessProcesses;
using AdvantShop.Customers;
using AdvantShop.Orders;
using AdvantShop.Core.Services.Crm.ProjectStatuses;

namespace AdvantShop.Core.Services.Crm
{
    public interface ITask
    {
        int Id { get; set; }
        int TaskGroupId { get; set; }

        int? AppointedManagerId { get; set; }

        string Name { get; set; }

        string Description { get; set; }
        int StatusId { get; set; }

        bool Accepted { get; set; }
        TaskPriority Priority { get; set; }

        DateTime? DueDate { get; set; }


        DateTime DateCreated { get; set; }
        DateTime DateModified { get; set; }

        DateTime DateAppointed { get; set; }

        TaskReminder Reminder { get; set; }
        bool Reminded { get; set; }

        List<AdminComment> Comments { get; }

        List<Manager> Managers { get; }

        Manager AppointedManager { get; }

        TaskGroup TaskGroup { get; }

        List<Manager> Observers { get; }
    }
}
