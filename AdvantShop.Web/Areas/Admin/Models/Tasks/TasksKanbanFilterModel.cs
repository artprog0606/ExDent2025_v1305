using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.Services.Crm.ProjectStatuses;
using AdvantShop.Web.Infrastructure.Admin;
using System;

namespace AdvantShop.Web.Admin.Models.Tasks
{
    public class TasksKanbanFilterModel : KanbanFilterModel<TasksKanbanColumnFilterModel>
    {
        public string Name { get; set; }

        public TaskPriority? Priority { get; set; }
        public int Status { get; set; }

        public int? AppointedManagerId { get; set; }
        public int? AssignedManagerId { get; set; }
        public int? TaskGroupId { get; set; }

        public bool? Accepted { get; set; }
        public bool? Viewed { get; set; }

        public DateTime? DueDateFrom { get; set; }
        public DateTime? DueDateTo { get; set; }

        public DateTime? DateCreatedFrom { get; set; }
        public DateTime? DateCreatedTo { get; set; }
        public int? ObserverId { get; set; }
        public bool ShowAcceptedTasks { get; set; }
        public ETasksKanbanViewTasks SelectTasks { get; set; }
    }

    public class TasksKanbanColumnFilterModel : KanbanColumnFilterModel
    {
        public TasksKanbanColumnFilterModel() : base() { }
        public TasksKanbanColumnFilterModel(string id) : base(id) { }

        public ProjectStatus ProjectStatus
        {
            get { return _projectStatus ?? (_projectStatus = ProjectStatusService.Get(Id.TryParseInt())); }
        }
        private ProjectStatus _projectStatus;

        public int? StatusType
        {
            get
            {
                TaskStatusType status;
                if (Enum.TryParse(Id, out status))
                    return (int)status;

                return null;
            }
        }
    }
}
