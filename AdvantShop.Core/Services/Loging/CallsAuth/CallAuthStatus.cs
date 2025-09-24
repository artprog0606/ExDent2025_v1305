using AdvantShop.Core.Common.Attributes;

namespace AdvantShop.Core.Services.Loging.CallsAuth
{
    public enum CallAuthStatus
    {
        [Localize("Core.Loging.CallAuthStatus.Sent")]
        Sent,
        [Localize("Core.Loging.CallAuthStatus.Error")]
        Error,
        
        [Localize("Core.Loging.CallAuthStatus.Fault")]
        Fault
    }
}