using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Customers;
using System;
using System.Linq;

namespace AdvantShop.Core.Services.Crm.BusinessProcesses
{
    public enum ETaskFieldType
    {
        [Localize("Core.Crm.ETaskFieldType.None")]
        None = 0,
        [Localize("Core.Crm.ETaskFieldType.AssignedManager"), FieldType(EFieldType.Select), FieldTypeValue(EFieldType.Text, "Core.Crm.ETaskFieldType.FilterTaskGroup", 10)]
        AssignedManager = 1,
        [Localize("Core.Crm.ETaskFieldType.AppointedManager"), FieldType(EFieldType.Select), FieldTypeValue(EFieldType.Text, "Core.Crm.ETaskFieldType.FilterTaskGroup", 20)]
        AppointedManager = 2,
        [Localize("Core.Crm.ETaskFieldType.Priority"), FieldType(EFieldType.Select), FieldTypeValue(EFieldType.Text, "Core.Crm.ETaskFieldType.FilterTaskGroup", 30)]
        Priority = 3,
        [Localize("Core.Crm.ETaskFieldType.TaskGroup"), FieldType(EFieldType.Select), FieldTypeValue(EFieldType.Text, "Core.Crm.ETaskFieldType.FilterTaskGroup", 40)]
        TaskGroup = 4,
        [Localize("Core.Crm.ETaskFieldType.Name"), FieldType(EFieldType.Text), FieldTypeValue(EFieldType.Text, "Core.Crm.ETaskFieldType.FilterTaskGroup", 50)]
        Name = 5,
        [Localize("Core.Crm.ETaskFieldType.Description"), FieldType(EFieldType.Text), FieldTypeValue(EFieldType.Text, "Core.Crm.ETaskFieldType.FilterTaskGroup", 60)]
        Description = 6,
    }

    public class TaskFieldComparer : IBizObjectFieldComparer<Task>
    {
        public ETaskFieldType FieldType { get; set; }

        public string FieldTypeStr => FieldType.ToString().ToLower();

        public FieldComparer FieldComparer { get; set; }

        public BizObjectFieldCompareType CompareType { get; set; }

        public bool CheckField(Task task)
        {
            var check = Check(task);
            return CompareType == BizObjectFieldCompareType.Equal ? check : !check;
        }

        private bool Check(Task task)
        {
            switch (FieldType)
            {
                case ETaskFieldType.AssignedManager:
                    return task.ManagerIds.Any(FieldComparer.Check);
                case ETaskFieldType.AppointedManager:
                    return FieldComparer.Check(task.AppointedManagerId);
                case ETaskFieldType.Priority:
                    return FieldComparer.Check(task.Priority.ConvertIntString());
                case ETaskFieldType.TaskGroup:
                    return FieldComparer.Check(task.TaskGroupId);
                case ETaskFieldType.Name:
                    return FieldComparer.Check(task.Name);
                case ETaskFieldType.Description:
                    return FieldComparer.Check(task.Description);
                default:
                    return false;
            }
        }

        private string _fieldName;
        public string FieldName
        {
            get
            {
                if (_fieldName != null)
                    return _fieldName;
                switch (FieldType)
                {
                    default:
                        _fieldName = FieldType.Localize();
                        break;
                }
                return _fieldName;
            }
        }

        private string _fieldValueObjectName;
        public string FieldValueObjectName
        {
            get
            {
                if (_fieldValueObjectName != null)
                    return _fieldValueObjectName;

                if (FieldComparer == null || !FieldComparer.ValueObjId.HasValue)
                {
                    _fieldValueObjectName = string.Empty;
                    return _fieldValueObjectName;
                }

                var fieldValueObjId = FieldComparer.ValueObjId.Value;
                switch (FieldType)
                {
                    case ETaskFieldType.AssignedManager:
                        var assignedManager = ManagerService.GetManager(fieldValueObjId);
                        _fieldValueObjectName = assignedManager != null ? assignedManager.FullName : string.Empty;
                        break;
                    case ETaskFieldType.AppointedManager:
                        var appointedManager = ManagerService.GetManager(fieldValueObjId);
                        _fieldValueObjectName = appointedManager != null ? appointedManager.FullName : string.Empty;
                        break;
                    case ETaskFieldType.Priority:
                        _fieldValueObjectName = Enum.IsDefined(typeof(TaskPriority), fieldValueObjId) ? ((TaskPriority)fieldValueObjId).Localize() : string.Empty;
                        break;
                    case ETaskFieldType.TaskGroup:
                        var taskGroup = TaskGroupService.GetTaskGroup(fieldValueObjId);
                        _fieldValueObjectName = taskGroup != null ? taskGroup.Name : string.Empty;
                        break;
                    default:
                        _fieldValueObjectName = string.Empty;
                        break;
                }

                return _fieldValueObjectName;
            }
        }

        public bool IsValid()
        {
            if (FieldComparer == null)
                return false;
            if (!FieldComparer.ValueObjId.HasValue)
                return true;

            var fieldValueObjId = FieldComparer.ValueObjId.Value;
            switch (FieldType)
            {
                case ETaskFieldType.AssignedManager:
                    return ManagerService.GetManager(fieldValueObjId) != null;
                case ETaskFieldType.AppointedManager:
                    return ManagerService.GetManager(fieldValueObjId) != null;
                case ETaskFieldType.Priority:
                    return Enum.IsDefined(typeof(TaskPriority), fieldValueObjId);
                case ETaskFieldType.TaskGroup:
                    return TaskGroupService.GetTaskGroup(fieldValueObjId) != null;
            }
            return true;
        }
    }
}
