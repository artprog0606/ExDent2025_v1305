using System;

namespace AdvantShop.Core.Services.Bonuses.Model.Enums
{
    public enum EOperationType : byte
    {
        [Obsolete]
        AddMainBonus = 1,

        [Obsolete]
        SubtractMainBonus = 2,
        
        [Obsolete]
        AddAdditionBonus = 3,
        
        [Obsolete]
        SubtractAdditionBonus = 4,
        
        [Obsolete("Unsupported", true)]
        AddAfiliate = 5,
        
        [Obsolete("Unsupported", true)]
        SubtractAfiliate = 6,
        
        AddBonus = 7,
        SubtractBonus = 8,
    }
}
