using AdvantShop.Core.Common.Attributes;

namespace AdvantShop.Shipping.FivePost
{
    public enum ETypeViewPoints
    {
        [Localize("Core.Shipping.TypeViewPoint.List")]
        List = 0,

        [Localize("Core.Shipping.TypeViewPoint.YandexMaps")]
        YandexMap = 1,

        //[Localize("Через виджет 5Пост")]
        //FivePostWidget = 2
    }

    public enum EFivePostBarcodeEnrichment
    {
        [Localize("Core.Shipping.FivePost.BarCodeFromFivePost")]
        None = 0,

        [Localize("Core.Shipping.FivePost.BarCodeFromAdvantshop")]
        Required = 1,

        [Localize("Core.Shipping.FivePost.BarCodePartial")]
        Partial = 2,
    }
}
