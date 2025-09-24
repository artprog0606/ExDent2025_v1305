using System;
using AdvantShop.Core.Common.Attributes;

namespace AdvantShop.Core.Services.Partners
{
    [Flags]
    public enum EPartnerMessageType
    {
        None = 0,
        [Localize("Core.Partners.EPartnerMessageType.CustomerBinded")]
        CustomerBinded = 1,
        [Localize("Core.Partners.EPartnerMessageType.RewardAdded")]
        RewardAdded = 2,
        [Localize("Core.Partners.EPartnerMessageType.MonthReport")]
        MonthReport = 4
    }
}
