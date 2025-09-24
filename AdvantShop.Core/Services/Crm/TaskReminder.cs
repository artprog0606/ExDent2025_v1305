using AdvantShop.Core.Common.Attributes;

namespace AdvantShop.Core.Services.Crm
{
    public enum TaskReminder
    {
        [Localize("Core.Crm.TaskReminder.NotRemind")]
        NotRemind = 0,
        [Localize("Core.Crm.TaskReminder.AtTerm")]
        AtTerm = 1,
        [Localize("Core.Crm.TaskReminder.TenMinutesBefore")]
        TenMinutesBefore = 2,
        [Localize("Core.Crm.TaskReminder.HourBefore")]
        HourBefore = 3,
        [Localize("Core.Crm.TaskReminder.ThreeHoursBefore")]
        ThreeHoursBefore = 4,
        [Localize("Core.Crm.TaskReminder.DayBefore")]
        DayBefore = 5,
        [Localize("Core.Crm.TaskReminder.ThreeDaysBefore")]
        ThreeDaysBefore = 6
    }
}
