using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.Services.Crm.BusinessProcesses;
using AdvantShop.Customers;
using AdvantShop.Web.Admin.Handlers.Settings.Users;
using AdvantShop.Web.Admin.Handlers.Tasks;
using AdvantShop.Web.Admin.Models.Settings.Users;
using AdvantShop.Web.Admin.Models.Tasks;

namespace AdvantShop.Web.Admin.Handlers.BizProcesses
{
    public class BizProcessHandler<TRule> where TRule: BizProcessRule
    {
        private readonly TRule _rule;
        private readonly IBizObject _bizObject;

        protected readonly List<AdminUserModel> Employees;
        protected readonly AdminUserModel Employee;
        protected TaskModel TaskModel { get; set; }

        public BizProcessHandler(List<TRule> rules, IBizObject bizObject)
        {
            _bizObject = bizObject;
            _rule = rules.FirstOrDefault(x => x.Filter == null || x.Filter.Check(_bizObject));

            var customers = (_rule != null && _rule.ManagerFilter != null 
                ? _rule.ManagerFilter.GetCustomers(bizObject, _rule.EventType) 
                : null) ?? new List<Customer>();

            Employees = customers.Select(customer => new GetUserModel(customer).Execute()).ToList();
            Employee = Employees.FirstOrDefault();
        }

        public virtual TaskModel GenerateTask()
        {
            if (!SettingsTasks.TasksActive)
                return null;

            if (_rule == null)
                return null;

            if (_rule.ObjectType == EBizProcessObjectType.Task)
            {
                var bindedTask = (Task)_bizObject;
                if (bindedTask.BindedTaskId.HasValue) // не создаем цепочку из задач к задачам
                    return null;
                if (_rule.EventType == EBizProcessEventType.TaskStatusChanged && TaskModel != null)
                    TaskModel.BindedObjectStatus = (int)TaskService.GetBPTaskStatus(bindedTask);
            }
            if (TaskModel == null)
                TaskModel = new TaskModel();
            
            TaskModel.Name = _rule.ReplaceVariables(_rule.TaskName, _bizObject);
            TaskModel.Description = _rule.ReplaceVariables(_rule.TaskDescription, _bizObject);
            TaskModel.ManagerIds = Employees.Where(employee => employee.AssociatedManagerId.HasValue).Select(employee => employee.AssociatedManagerId.Value).ToList();
            TaskModel.TaskGroupId = _rule.TaskGroupId ?? 0;
            TaskModel.Priority = _rule.TaskPriority;
            TaskModel.IsAutomatic = true;
            TaskModel.BizProcessRuleId = _rule.Id;

            if (_rule.TaskCreateInterval == null)
            {
                TaskModel.DateAppointed = DateTime.Now;
                TaskModel.IsDeferred = false;
            }
            else
            {
                TaskModel.DateAppointed = _rule.TaskCreateInterval.GetDateTime(DateTime.Now);
                TaskModel.IsDeferred = true;
            }
            // крайний срок с даты назначения задачи
            TaskModel.DueDate = _rule.TaskDueDateInterval != null 
                ? _rule.TaskDueDateInterval.GetDateTime(TaskModel.DateAppointed)
                : (DateTime?)null;

            var task = new AddTaskHandler(TaskModel).Execute();

            new TaskAddedNotificationsHandler(task).Execute();

            return TaskModel;
        }

        public virtual void ProcessBizObject()
        {

        }
    }


}
