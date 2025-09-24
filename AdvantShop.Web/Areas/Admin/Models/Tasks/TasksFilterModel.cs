using System;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.Services.Crm.ProjectStatuses;
using AdvantShop.Customers;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Models.Tasks
{
    public class TasksFilterModel : BaseFilterModel
    {
        public TasksPreFilterType FilterBy { get; set; }

        public bool UseKanban { get; set; }
        
        public TaskPriority? Priority { get; set; }
        public ProjectStatus Status { get; set; }
        public int? StatusType { get; set; }

        public int? AppointedManagerId { get; set; }
        public int? AssignedManagerId { get; set; }
        public int? TaskGroupId { get; set; }

        public bool? Accepted { get; set; }
        public bool? Canceled { get; set; }
        public bool? Viewed { get; set; }

        public DateTime? DueDateFrom { get; set; }
        public DateTime? DueDateTo { get; set; }

        public DateTime? DateCreatedFrom { get; set; }
        public DateTime? DateCreatedTo { get; set; }

        public string ObjId { get; set; }
        public int? IntObjId { get { return ObjId.TryParseInt(isNullable: true); } }
        public Guid? GuidObjId { get { return ObjId.TryParseGuid(true); } }

        public int? ObserverId { get; set; }
    }

    public class TasksCommand : TasksFilterModel
    {
        public Customer Customer { get; set; }
        public int ManagerId { get; set; }
    }
}
