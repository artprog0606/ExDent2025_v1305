using AdvantShop.Core.Common.Attributes;

namespace AdvantShop.App.Landing.Domain.Common
{
    public enum ELpShoppingCartType
    {
        [Localize("Landing.Domain.Common.ELpShoppingCartType.Goods")]
        Goods = 0,

        [Localize("Landing.Domain.Common.ELpShoppingCartType.Booking")]
        Booking = 1,
    }
}
