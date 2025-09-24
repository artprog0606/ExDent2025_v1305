using AdvantShop.Core.Common.Attributes;

namespace AdvantShop.Core.Services.Crm.ProjectStatuses
{
    public class ProjectStatus
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int SortOrder { get; set; }
        public string Color { get; set; }
        public ProjectStatusType Status { get; set; }
        public TaskStatusType StatusType { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object obj)
        {
            var other = obj as ProjectStatus;
            if (other == null)
                return false;

            return Id == other.Id &&
                   Name == other.Name &&
                   SortOrder == other.SortOrder &&
                   Status == other.Status;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode() ^ (Name ?? "").GetHashCode() ^ SortOrder.GetHashCode() ^ Status.GetHashCode();
        }
    }

    public enum ProjectStatusType
    {
        None = 0,
        FinalSuccess = 1,
        Canceled = 2
    }

    public enum TaskStatusType
    {
        [Localize("Core.Crm.TaskStatusType.Open")]
        Open = 0,
        [Localize("Core.Crm.TaskStatusType.InProgress")]
        InProgress = 1,
        [Localize("Core.Crm.TaskStatusType.Completed")]
        Completed = 2,
        [Localize("Core.Crm.TaskStatusType.Accepted")]
        Accepted = 3,
        [Localize("Core.Crm.TaskStatusType.Canceled")]
        Canceled = 4,
        
    }
}
