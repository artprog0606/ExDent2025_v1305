using System.Collections.Generic;
using AdvantShop.Core.Services.Crm.ProjectStatuses;
using AdvantShop.Customers;

namespace AdvantShop.Web.Admin.Models.Tasks.TaskGroups
{
    public partial class TaskGroupModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int SortOrder { get; set; }
        public bool Enabled { get; set; }
        public bool IsPrivateComments { get; set; }
        public int TasksCount { get; set; }
        public int NewTaskCount { get; set; }
        public int InProgressTaskCount { get; set; }
        public int CompletedTaskCount { get; set; }
        public int AcceptedTaskCount { get; set; }
        public int CanceledTaskCount { get; set; }
        public List<int> ManagerIds { get; set; }
        public List<int> ManagerRoleIds { get; set; }

        public List<int> ParticipantIds { get; set; }

        public bool CanBeDeleted => TasksCount == 0;

        public int ManagersTaskGroupConstraint { get; set; }
        public bool IsNotBeCompleted { get; set; }
        public List<ProjectStatus> ProjectStatuses { get; set; }
    }
}
