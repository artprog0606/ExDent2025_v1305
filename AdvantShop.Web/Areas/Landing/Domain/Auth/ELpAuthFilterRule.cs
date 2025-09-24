using AdvantShop.Core.Common.Attributes;

namespace AdvantShop.App.Landing.Domain.Auth
{
    public enum ELpAuthFilterRule
    {
        [Localize("Landing.Domain.Auth.ELpAuthFilterRule.Registered")]
        Registered = 0,

        [Localize("Landing.Domain.Auth.ELpAuthFilterRule.WithOrderAndProduct")]
        WithOrderAndProduct = 1,

        [Localize("Landing.Domain.Auth.ELpAuthFilterRule.WithLead")]
        WithLead = 2
    }
}
