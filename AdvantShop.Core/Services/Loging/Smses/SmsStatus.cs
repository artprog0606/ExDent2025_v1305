using AdvantShop.Core.Common.Attributes;

namespace AdvantShop.Core.Services.Loging.Smses
{
    public enum SmsStatus
    {
        [Localize("Core.Loging.SmsStatus.Sent")]
        Sent = 0,
        [Localize("Core.Loging.SmsStatus.Error")]
        Error = 1,
        
        [Localize("Core.Loging.SmsStatus.Fault")]
        Fault = 2
    }
}
