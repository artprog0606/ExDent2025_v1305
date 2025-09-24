using AdvantShop.Core.Common.Attributes;

namespace AdvantShop.Core.Services.Auth.Calls
{
    public enum EAuthCall
    {
        [Localize("Core.Calls.EAuthCall.Flash")]
        Flash = 1,
        
        [Localize("Core.Calls.EAuthCall.Voice")]
        Voice = 2
    }
}