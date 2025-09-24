using System;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Customers;
using AdvantShop.FilePath;
using AdvantShop.Localization;
using Newtonsoft.Json;
using System.Collections.Generic;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Core.Services.Crm.ProjectStatuses;

namespace AdvantShop.Web.Admin.Models.Tasks
{
    public class TaskModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int TaskGroupId { get; set; }
        public string TaskGroupName { get; set; }

        /// <summary>
        /// Постановщик
        /// </summary>
        public int? AppointedManagerId { get; set; }
        public Guid? AppointedCustomerId { get; set; }
        public string AppointedName { get; set; }

        /// <summary>
        /// Исполнители
        /// </summary>
        public List<int> ManagerIds { get; set; }
        public string ManagersJson { get; set; }
        private List<TaskManagerModel> _managers;
        public List<TaskManagerModel> Managers
        {
            get { return _managers ?? (_managers = JsonConvert.DeserializeObject<List<TaskManagerModel>>(ManagersJson ?? string.Empty)); }
        }

        /// <summary>
        /// Наблюдатели
        /// </summary>
        public List<int> ObserverIds { get; set; }

        public string Description { get; set; }
        public int StatusId { get; set; }

        private string _statusColor;
        public string StatusColor { get { return _statusColor ?? (_statusColor = ProjectStatusService.Get(StatusId)?.Color); } }
        public bool Accepted { get; set; }
        public TaskPriority Priority { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateAppointed { get; set; }

        public int? OrderId { get; set; }
        public string OrderNumber { get; set; }

        public Guid? ClientCustomerId { get; set; }
        public string ClientName { get; set; }

        public int? LeadId { get; set; }
        public string LeadTitle { get; set; }
        public int? LeadSalesFunnelId { get; set; }
        public int? LeadDealStatusId { get; set; }

        public int? ReviewId { get; set; }
        public int? BindedTaskId { get; set; }

        public int? BindedObjectStatus { get; set; }

        public string ResultShort { get; set; }
        public string ResultFull { get; set; }

        public bool IsAutomatic { get; set; }
        public bool IsDeferred { get; set; }
        public bool Remind { get; set; }
        public TaskReminder Reminder { get; set; }

        public bool Reminded { get; set; }

        public string AppointedCustomerAvatar { get; set; }
        public string AppointedCustomerAvatarSrc
        {
            get { return AppointedCustomerAvatar.IsNotEmpty() ? FoldersHelper.GetPath(FolderType.Avatar, AppointedCustomerAvatar, false) : UrlService.GetAdminStaticUrl() + "images/no-avatar.jpg"; }
        }

        public string AssignedCustomerAvatar { get; set; }
        public string AssignedCustomerAvatarSrc
        {
            get { return AssignedCustomerAvatar.IsNotEmpty() ? FoldersHelper.GetPath(FolderType.Avatar, AssignedCustomerAvatar, false) : UrlService.GetAdminStaticUrl() + "images/no-avatar.jpg"; }
        }

        public string StatusString { get { return StatusId.ToString().ToLower(); } }
        public string StatusFormatted { get { return _statusFormatted ?? (_statusFormatted = ProjectStatusService.Get(StatusId)?.Name); } }
        private string _statusFormatted;
        public string StatusType { get { return _statusType ?? (_statusType = ProjectStatusService.Get(StatusId)?.StatusType.ToString()); } }
        private string _statusType;
        public string PriorityFormatted { get { return Priority.Localize(); } }
        public string DueDateFormatted { get { return DueDate.HasValue ? Culture.ConvertDate(DueDate.Value) : string.Empty; } }

        public string DateAppointedFormatted { get { return Culture.ConvertDate(DateAppointed); } }
        public string DateAppointedFormattedFull { get { return DateAppointed.ToString("dd MMMM yyyy HH:mm"); } }
        public string DaysAfterCreateInFormatted
        {
            get
            {
                return Helpers.StringHelper.FormatDateTimeInterval(DateCreated);
            }
        }

        public bool CanDelete { get; set; }
        public DateTime? ViewDate { get; set; }
        public bool Viewed { get; set; }
        public int NewCommentsCount { get; set; }

        //public bool Completed { get { return Status == TaskStatus.Completed; } }
        //public bool Overdue { get { return !Completed && DueDate.HasValue && DueDate.Value < DateTime.Now; } }
        //public bool InProgress { get { return Status == TaskStatus.InProgress; } }

        public bool IsPrivateComments { get; set; }
        public bool IsReadonlyTask { get; set; }
        
        public int BizProcessRuleId { get; set; }

        public static explicit operator TaskModel(Task task)
        {
            return new TaskModel
            {
                Id = task.Id,
                TaskGroupId = task.TaskGroupId,
                ManagerIds = task.ManagerIds,
                AppointedManagerId = task.AppointedManagerId,
                Name = task.Name,
                Description = task.Description,
                StatusId = task.StatusId,
                Accepted = task.Accepted,
                Priority = task.Priority,
                DueDate = task.DueDate,
                LeadId = task.LeadId,
                LeadTitle = task.Lead != null ? task.Lead.Title : null,
                LeadSalesFunnelId = task.Lead != null ? task.Lead.SalesFunnelId : (int?)null,
                LeadDealStatusId = task.Lead != null ? task.Lead.DealStatusId : (int?)null,
                OrderId = task.OrderId,
                OrderNumber = task.Order != null ? task.Order.Number : null,
                ReviewId = task.ReviewId,
                ClientCustomerId = task.CustomerId,
                BindedTaskId = task.BindedTaskId,
                BindedObjectStatus = task.BindedObjectStatus,
                ClientName = task.ClientCustomer != null ? task.ClientCustomer.GetFullName() : null,
                ResultShort = task.ResultShort,
                ResultFull = task.ResultFull,
                DateCreated = task.DateCreated,
                DateAppointed = task.DateAppointed,
                IsAutomatic = task.IsAutomatic,
                Reminder = task.Reminder,
                Remind = task.Reminder != TaskReminder.NotRemind,
                Reminded = task.Reminded,
                ObserverIds = task.ObserverIds,
                BizProcessRuleId= task.BizProcessRuleId ?? 0
            };
        }
    }
}
