using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.Services.Crm.BusinessProcesses;
using AdvantShop.Core.Services.Localization;

namespace AdvantShop.Web.Admin.Models.Settings
{
    public class TasksSettingsModel
    {
        public TasksSettingsModel()
        {
            EventTypes = new List<EBizProcessEventType>
            {
                EBizProcessEventType.OrderCreated,
                EBizProcessEventType.OrderStatusChanged,
                EBizProcessEventType.LeadCreated,
                EBizProcessEventType.LeadStatusChanged,
                EBizProcessEventType.CallMissed,
                EBizProcessEventType.ReviewAdded,
                EBizProcessEventType.MessageReply,
                EBizProcessEventType.TaskCreated,
                EBizProcessEventType.TaskStatusChanged,
            };
        }

        public bool WebNotificationInNewTab { get; set; }

        public List<SelectListItem> TaskGroups { get; set; }

        public List<EBizProcessEventType> EventTypes { get; set; }

        public bool TasksActive { get; set; }

        public bool ReminderActive { get; set; }
    }
}
