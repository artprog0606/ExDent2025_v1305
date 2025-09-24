using System;
using AdvantShop.Customers;

namespace AdvantShop.Core.Services.Crm
{
    public class TaskGroup
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int SortOrder { get; set; }
        public DateTime DateCreated { get; set; }
        public bool Enabled { get; set; }
        public bool IsPrivateComments { get; set; }
        public ManagersTaskConstraint? ManagersTaskGroupConstraint { get; set; }
        public bool IsNotBeCompleted { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as TaskGroup;
            if (other == null)
                return false;

            return Id == other.Id &&
                   Name == other.Name &&
                   SortOrder == other.SortOrder &&
                   Enabled == other.Enabled &&
                   DateCreated == other.DateCreated &&
                   IsPrivateComments == other.IsPrivateComments &&
                   ManagersTaskGroupConstraint == other.ManagersTaskGroupConstraint &&
                   IsNotBeCompleted == other.IsNotBeCompleted;
        }

        public override int GetHashCode()
        {
            return
                Id.GetHashCode() ^
                (Name ?? "").GetHashCode() ^
                SortOrder.GetHashCode() ^
                DateCreated.GetHashCode() ^
                Enabled.GetHashCode() ^
                IsPrivateComments.GetHashCode() ^
                (ManagersTaskGroupConstraint != null ? (int)ManagersTaskGroupConstraint : -1).GetHashCode()^
                IsNotBeCompleted.GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
