using System.Collections.Generic;
using AdvantShop.Web.Admin.Models.Tasks;

namespace AdvantShop.Web.Admin.ViewModels.Tasks
{
    public class TasksListViewModel
    {
        public TasksListViewModel()
        {
            PreFilterTypes = new List<TasksPreFilterType>
            {
                TasksPreFilterType.AssignedToMe,
                TasksPreFilterType.AppointedByMe,
                TasksPreFilterType.ObservedByMe,
                TasksPreFilterType.Open,
                TasksPreFilterType.InProgress,
                TasksPreFilterType.Completed,
                TasksPreFilterType.Accepted,
                TasksPreFilterType.Canceled
            };
        }

        public bool UseKanban { get; set; }
        public string SelectTasks { get; set; }
        public string Title { get; set; }
        public int? TaskGroupId { get; set; }
        public TasksPreFilterType PreFilter { get; set; }
        public List<TasksPreFilterType> PreFilterTypes { get; set; }
        public bool ShowAcceptedTasks { get; set; }
    }
}
