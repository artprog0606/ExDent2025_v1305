using System.Collections.Generic;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Models.Tasks
{
    public class TasksFilterResult : FilterResult<TaskModel>
    {
        public TasksFilterResult()
        {
            TasksCount = new Dictionary<TasksPreFilterType, int>();
        }

        public Dictionary<TasksPreFilterType, int> TasksCount { get; set; }
    }
}
