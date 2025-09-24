using System.Collections.Generic;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.Services.Crm.BusinessProcesses;
using AdvantShop.Web.Admin.Models.Tasks;

namespace AdvantShop.Web.Admin.Handlers.BizProcesses
{
    public class BizProcessTaskHandler<TRule> : BizProcessHandler<TRule> where TRule : BizProcessTaskRule
    {
        private readonly Task _task;

        public BizProcessTaskHandler(List<TRule> rules, Task task) : base(rules, task)
        {
            _task = task;
        }

        public override TaskModel GenerateTask()
        {
            TaskModel = new TaskModel
            {
                BindedTaskId = _task.Id
            };

            return base.GenerateTask();
        }
    }
}
