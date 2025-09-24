using AdvantShop.Core.Common.Attributes;

namespace AdvantShop.Core.Services.Auth
{
    public enum EAuthMethod
    {
        [Localize("Core.AuthService.EAuthMethod.Email")]
        Email = 0,
        [Localize("Core.AuthService.EAuthMethod.Code")]
        Code = 1,
    }
}