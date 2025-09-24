using AdvantShop.Core.Common.Attributes;

namespace AdvantShop.Web.Admin.Models.Tasks
{
    // EnumIgnore - not shown in admin part
    public enum TasksPreFilterType
    {
        [Localize("Admin.Models.Tasks.TasksPreFilterType.None")]
        None = 0,
        [Localize("Admin.Models.Tasks.TasksPreFilterType.AssignedToMe")]
        AssignedToMe = 1,
        [Localize("Admin.Models.Tasks.TasksPreFilterType.AppointedByMe")]
        AppointedByMe = 2,
        [Localize("Core.Crm.TaskStatusType.Open")]
        Open = 3,
        [Localize("Core.Crm.TaskStatusType.InProgress")]
        InProgress = 4,
        [Localize("Admin.Models.Tasks.TasksPreFilterType.Completed")]
        Completed = 5,
        [Localize("Admin.Models.Tasks.TasksPreFilterType.Accepted")]
        Accepted = 6,
        [Localize("Core.Crm.TaskStatusType.Canceled")]
        Canceled = 7,

        [EnumIgnore]
        Order = 8,
        [EnumIgnore]
        Lead = 9,

        [Localize("Admin.Models.Tasks.TasksPreFilterType.ObservedByMe")]
        ObservedByMe = 10,

        [EnumIgnore]
        Customer = 11,

        //[Localize("Admin.Models.Tasks.TasksPreFilterType.Canceled")]
        //Canceled = 9
    }
}
