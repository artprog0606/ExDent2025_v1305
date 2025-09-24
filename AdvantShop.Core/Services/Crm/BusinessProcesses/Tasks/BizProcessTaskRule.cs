using System.Text;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Localization;

namespace AdvantShop.Core.Services.Crm.BusinessProcesses
{
    public enum BPTaskStatus
    {
        [Localize("Core.Crm.TaskStatus.Open")]
        Open = 0,
        [Localize("Core.Crm.TaskStatus.InProgress")]
        InProgress = 1,
        [Localize("Core.Crm.TaskStatus.Completed")]
        Completed = 2,
        [Localize("Core.Crm.BPTaskStatus.Accepted")]
        Accepted = 3,
    }

    public class BizProcessTaskRule : BizProcessRule
    {
        public override EBizProcessObjectType ObjectType => EBizProcessObjectType.Task;

        public override string[] AvailableVariables
        {
            get
            {
                return new string[] { "#TASK_ID#" };
            }
        }

        public override string ReplaceVariables(string value, IBizObject bizObject)
        {
            var task = (Task)bizObject;
            var sb = new StringBuilder(value);
            sb.Replace("#TASK_ID#", task.Id.ToString());

            return sb.ToString();
        }
    }


    public class BizProcessTaskCreatedRule : BizProcessTaskRule
    {
        public override EBizProcessEventType EventType => EBizProcessEventType.TaskCreated;
    }

    public class BizProcessTaskStatusChangedRule : BizProcessTaskRule
    {
        public override EBizProcessEventType EventType => EBizProcessEventType.TaskStatusChanged;

        private string _eventName;
        public override string EventName
        {
            get
            {
                if (_eventName != null)
                    return _eventName;

                _eventName = base.EventName + (EventObjId.HasValue ? 
                    string.Format(" {0} {1}", LocalizationService.GetResource("Core.BizProcessRule.ChangeState.On"), ((BPTaskStatus)EventObjId.Value).Localize()) 
                    : string.Empty);

                return _eventName;
            }
        }
    }
}
