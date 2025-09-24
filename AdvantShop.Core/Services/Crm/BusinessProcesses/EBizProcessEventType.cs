using AdvantShop.Core.Common.Attributes;

namespace AdvantShop.Core.Services.Crm.BusinessProcesses
{
    public enum EBizProcessEventType
    {
        None = 0,
        [Localize("Core.Services.EBizProcessEventType.OrderCreated")]
        OrderCreated = 1,
        [Localize("Core.Services.EBizProcessEventType.OrderStatusChanged")]
        OrderStatusChanged = 2,
        [Localize("Core.Services.EBizProcessEventType.LeadCreated")]
        LeadCreated = 3,
        [Localize("Core.Services.EBizProcessEventType.LeadStatusChanged")]
        LeadStatusChanged = 4,
        [Localize("Core.Services.EBizProcessEventType.CallMissed")]
        CallMissed = 5,
        [Localize("Core.Services.EBizProcessEventType.ReviewAdded")]
        ReviewAdded = 6,
        [Localize("Core.Services.EBizProcessEventType.MessageReply")]
        MessageReply = 7,
        [Localize("Core.Services.EBizProcessEventType.TaskCreated")]
        TaskCreated = 8,
        [Localize("Core.Services.EBizProcessEventType.TaskStatusChanged")]
        TaskStatusChanged = 9,
        [Localize("Core.Services.EBizProcessEventType.OrderManagerAssigned")]
        OrderManagerAssigned = 10,
        [Localize("Core.Services.EBizProcessEventType.BookingCreated")]
        BookingCreated = 11,
        [Localize("Core.Services.EBizProcessEventType.BookingChanged")]
        BookingChanged = 12,
        [Localize("Core.Services.EBizProcessEventType.BookingDeleted")]
        BookingDeleted = 13,
        [Localize("Core.Services.EBizProcessEventType.LeadChanged")]
        LeadChanged = 14,
    }

}
