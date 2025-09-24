using System;

namespace AdvantShop.Core.Services.Bonuses.Model.Enums
{
    public enum EBonusStatus : byte
    {
        Create,
        Zero,
        Substract,
        [Obsolete("Unsupported", true)]
        RecoveryAdd,
        Removed,
    }
}
