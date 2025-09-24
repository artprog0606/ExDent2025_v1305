using System;
using System.Collections.Generic;
using AdvantShop.CMS;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Services.Attachments;
using AdvantShop.Core.Services.Crm.BusinessProcesses;
using AdvantShop.Core.Services.Crm.ProjectStatuses;
using AdvantShop.Customers;
using AdvantShop.Orders;

namespace AdvantShop.Core.Services.Crm
{
    public class Task : ITask, ICloneable, IBizObject
    {
        public int Id { get; set; }

        private int _taskGroupId;
        public int TaskGroupId
        {
            get => _taskGroupId;
            set
            {
                var isChanged = _taskGroupId != value;
                
                _taskGroupId = value;
                
                if (isChanged)
                    _taskGroup = null;
            }
        }
        
        /// <summary>
        /// Постановщик
        /// </summary>
        public int? AppointedManagerId { get; set; }

        [Compare("Core.Crm.Task.Name")]
        public string Name { get; set; }

        [Compare("Core.Crm.Task.Description")]
        public string Description { get; set; }

        //[Compare("Core.Crm.Task.Status")]
        public int StatusId { get; set; }

        [Compare("Core.Crm.Task.Accepted")]
        public bool Accepted { get; set; }

        [Compare("Core.Crm.Task.Priority")]
        public TaskPriority Priority { get; set; }

        [Compare("Core.Crm.Task.DueDate")]
        public DateTime? DueDate { get; set; }

        public int? LeadId { get; set; }
        public int? OrderId { get; set; }
        public int? ReviewId { get; set; }
        public Guid? CustomerId { get; set; }
        public int? BindedTaskId { get; set; }

        [Compare("Core.Crm.Task.ResultShort")]
        public string ResultShort { get; set; }

        [Compare("Core.Crm.Task.ResultFull")]
        public string ResultFull { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }

        public DateTime DateAppointed { get; set; }
        public bool IsAutomatic { get; set; }
        public bool IsDeferred { get; set; }
        public int? BindedObjectStatus { get; set; }
        public TaskReminder Reminder { get; set; }
        public bool Reminded { get; set; }
        
        public int? BizProcessRuleId { get; set; }

        /// <summary>
        /// Исполнители
        /// </summary>
        private List<int> _managerIds { get; set; }
        public List<int> ManagerIds => _managerIds ?? (_managerIds = TaskService.GetTaskManagerIds(Id));

        private List<AdminComment> _comments;
        public List<AdminComment> Comments => _comments ?? (_comments = AdminCommentService.GetAdminComments(Id, AdminCommentType.Task));

        private List<TaskAttachment> _attachments;
        public List<TaskAttachment> Attachments => _attachments ?? (_attachments = AttachmentService.GetAttachments<TaskAttachment>(Id));

        private List<Manager> _managers;
        /// <summary>
        /// Исполнители
        /// </summary>
        [Compare("Core.Crm.Task.AssignedManager")]
        public List<Manager> Managers => _managers ?? (_managers = ManagerService.GetManagers(ManagerIds));

        private Manager _appointedManager;
        /// <summary>
        /// Постановщик
        /// </summary>
        [Compare("Core.Crm.Task.AppointedManager")]
        public Manager AppointedManager =>
            AppointedManagerId.HasValue 
                ? _appointedManager ?? (_appointedManager = ManagerService.GetManager(AppointedManagerId.Value))
                : null;

        private TaskGroup _taskGroup;

        [Compare("Core.Crm.Task.TaskGroup")]
        public TaskGroup TaskGroup => _taskGroup ?? (_taskGroup = TaskGroupService.GetTaskGroup(TaskGroupId));

        
        private Customer _clientCustomer;
        public Customer ClientCustomer =>
            CustomerId.HasValue 
                ? _clientCustomer ?? (_clientCustomer = CustomerService.GetCustomer(CustomerId.Value))
                : null;

        private Order _order;
        public Order Order => OrderId.HasValue ? _order ?? (_order = OrderService.GetOrder(OrderId.Value)) : null;

        private Lead _lead;
        public Lead Lead => LeadId.HasValue ? _lead ?? (_lead = LeadService.GetLead(LeadId.Value)) : null;

        private string _statusName;
        [Compare("Core.Crm.Task.Status")]
        public string StatusName => string.IsNullOrEmpty(_statusName) ? (_statusName = ProjectStatusService.Get(StatusId).Name) : _statusName;

        private ProjectStatusType? _status;
        public ProjectStatusType Status => _status != null ? _status.Value : (_status = ProjectStatusService.Get(StatusId).Status).Value;
        private string _statusType;
        public string StatusType => _statusType ?? (_statusType = ProjectStatusService.Get(StatusId)?.StatusType.ToString());

        #region IBizObject

        /// <summary>
        /// AppointedManagerId
        /// </summary>
        public int? ManagerId
        {
            get => AppointedManagerId;
            set => AppointedManagerId = value;
        }

        #endregion

        public void SetManagerIds(List<int> ids)
        {
            _managerIds = ids ?? new List<int>();
            _managers = null;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        /// <summary>
        /// Copy with lazy fields
        /// </summary>
        /// <returns></returns>
        public Task DeepClone()
        {
            var task = (Task)Clone();
            var managerIds = task.ManagerIds;
            var managers = task.Managers;
            var attachments = task.Attachments;
            var observers = task.Observers;
            return task;
        }

        private List<int> _observerIds;

        public List<int> ObserverIds
        {
            get => _observerIds ?? (_observerIds = TaskService.GetTaskObserverIds(Id));
            set => _observerIds = value ?? new List<int>();
        }

        private List<Manager> _observers;

        [Compare("Core.Crm.Task.Observer")]
        public List<Manager> Observers => _observers ?? (_observers = ManagerService.GetManagers(ObserverIds));
    }
}
