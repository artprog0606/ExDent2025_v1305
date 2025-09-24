using System;
using System.Collections.Generic;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Scheduler;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.Services.Crm.ProjectStatuses;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.FilePath;
using AdvantShop.Localization;
using AdvantShop.Web.Infrastructure.Admin;
using Newtonsoft.Json;

namespace AdvantShop.Web.Admin.Models.Tasks
{
    public enum ETasksKanbanColumn
    {
        None = 0,
        [Localize("Новые"), Color("#8bc34a")]
        New = 1,
        [Localize("В работе"), Color("#78909c")]
        InProgress = 2,
        [Localize("Сделаны"), Color("#b0bec5")]
        Done = 3,
        [Localize("Приняты")]
        Accepted = 4
    }

    public enum ETasksKanbanViewTasks
    {
        [Localize("Все")]
        All = 0,
        [Localize("Мои задачи")]
        AssignedToMe = 1,
        [Localize("Поставленные мной задачи")]
        AppointedByMe = 2,
        [Localize("Просматриваемые мной задачи")]
        ObservedByMe = 3,
    }

    public class TasksKanbanModel : KanbanModel<TaskKanbanModel> 
    {
        public int AssignedToMeTasksCount { get; set; }
        public int AppointedByMeTasksCount { get; set; }
        public int ObservedByMeTasksCount { get; set; }
    }

    public class TasksKanbanColumnModel : KanbanColumnModel<TaskKanbanModel>
    {
        public int? ProjectStatusId { get; set; }
        public int? StatusType { get; set; }
    }

    public class TasksKanbanFinishColumnModel : TasksKanbanColumnModel
    {
        public TaskStatusType StatusType { get; set; }
        public ProjectStatusType Status { get; set; }
    }

    public class TaskKanbanModel : KanbanCardModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int TaskGroupId { get; set; }
        public string TaskGroupName { get; set; }
        public string TaskGroupUrl { get; set; }

        /// <summary>
        /// Постановщик
        /// </summary>
        public Guid? AppointedCustomerId { get; set; }
        public string AppointedName { get; set; }

        /// <summary>
        /// Исполнители
        /// </summary>
        public string ManagersJson { get; set; }
        private List<TaskManagerModel> _managers;
        public List<TaskManagerModel> Managers
        {
            get { return _managers ?? (_managers = JsonConvert.DeserializeObject<List<TaskManagerModel>>(ManagersJson ?? string.Empty)); }
        }

        public string Description { get; set; }
        public int StatusId { get; set; }
        private ProjectStatus _status;
        public ProjectStatus Status 
        { 
            get { return _status ?? (_status = ProjectStatusService.Get(StatusId)); } 
        }
        public bool Accepted { get; set; }
        public TaskPriority Priority { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime DateAppointed { get; set; }

        public string AppointedCustomerAvatar { get; set; }
        public string AppointedCustomerAvatarSrc
        {
            get
            {
                return AppointedCustomerAvatar.IsNotEmpty() 
                    ? string.Format("{0}?rnd={1}", FoldersHelper.GetPath(FolderType.Avatar, AppointedCustomerAvatar, false), new Random().Next()) 
                    : UrlService.GetAdminStaticUrl() + "images/no-avatar.jpg";
            }
        }

        public string AssignedCustomerAvatar { get; set; }
        public string AssignedCustomerAvatarSrc
        {
            get
            {
                return AssignedCustomerAvatar.IsNotEmpty() 
                    ? string.Format("{0}?rnd={1}", FoldersHelper.GetPath(FolderType.Avatar, AssignedCustomerAvatar, false), new Random().Next())
                    : UrlService.GetAdminStaticUrl() + "images/no-avatar.jpg";
            }
        }

        public string StatusString { get { return Status.Name.ToString().ToLower(); } }
        public string StatusFormatted { get { return Status.Status == ProjectStatusType.FinalSuccess && Accepted ? LocalizationService.GetResource("Admin.Tasks.TaskModel.Accepted") : Status.Name; } }
        public string PriorityFormatted { get { return Priority.Localize(); } }
        public string DueDateFormatted { get { return DueDate.HasValue ? Culture.ConvertDate(DueDate.Value) : string.Empty; } }
        public string DateAppointedFormattedFull
        {
            get { return DateAppointed.Year == DateTime.Now.Year ? DateAppointed.ToString("dd MMMM HH:mm") : DateAppointed.ToString("dd MMMM yyyy HH:mm"); }
        }
        public string DueDateInFormatted
        {
            get
            {
                if (!DueDate.HasValue)
                    return string.Empty;

                TimeInterval ti;
                var datesRange = (DueDate.Value - DateTime.Now).Duration();
                if (datesRange.TotalDays > 1)
                    ti = new TimeInterval() { Interval = (int)Math.Ceiling(datesRange.TotalDays), IntervalType = TimeIntervalType.Days };
                else if (datesRange.TotalHours > 1)
                    ti = new TimeInterval() { Interval = (int)Math.Ceiling(datesRange.TotalHours), IntervalType = TimeIntervalType.Hours };
                else
                    ti = new TimeInterval() { Interval = (int)Math.Ceiling(datesRange.TotalMinutes), IntervalType = TimeIntervalType.Minutes };

                return string.Format("{0} {1}", ti.Interval, ti.Numeral("минут", true));
            }
        }

        public string DaysAfterCreateInFormatted
        {
            get
            {
                TimeInterval ti;
                var datesRange = (DateAppointed - DateTime.Now).Duration();
                if (datesRange.TotalDays > 1)
                    ti = new TimeInterval() { Interval = (int)Math.Ceiling(datesRange.TotalDays), IntervalType = TimeIntervalType.Days };
                else if (datesRange.TotalHours > 1)
                    ti = new TimeInterval() { Interval = (int)Math.Ceiling(datesRange.TotalHours), IntervalType = TimeIntervalType.Hours };
                else
                    ti = new TimeInterval() { Interval = (int)Math.Ceiling(datesRange.TotalMinutes), IntervalType = TimeIntervalType.Minutes };

                return string.Format("{0} {1}", ti.Interval, ti.Numeral("минут", true));
            }
        }

        //public bool CanDelete { get; set; }
        public DateTime? ViewDate { get; set; }
        public bool Viewed { get; set; }
        public int NewCommentsCount { get; set; }

        public bool Completed { get { return Status.Status == ProjectStatusType.FinalSuccess; } }
        public bool Overdue { get { return DueDate.HasValue && DueDate.Value < DateTime.Now; } } //!Completed && 
        public bool InProgress { get { return Status.Status == ProjectStatusType.None; } }

        public int? LeadId { get; set; }
        public int? OrderId { get; set; }

        public bool isNotBeCompleted { get; set; }
    }
}
