using AdvantShop.Core.Common.Attributes;

namespace AdvantShop.Shipping.LPost
{
    public class LPostTemplate 
    {
        public const string SecretKey = "secret";
        public const string YandexMapApiKey = "yandexmapapikey";
        public const string DeliveryTypes = "deliverytypes";
        public const string TypeViewPoints = "typeviewpoints";
        public const string ReceivePoint = "receivepoint";
        public const string WithInsure = "withinsure";
    }

    public enum TypeDelivery
    {
        [Localize("AdvantShop.Core.Shipping.TypeOfDelivery.SelfDelivery")]
        PVZ = 0,

        [Localize("AdvantShop.Core.Shipping.TypeOfDelivery.Courier")]
        Courier = 1
    }

    public enum TypeViewPoints
    {
        [Localize("Списком")]
        List = 0,

        [Localize("Через Яндекс.Карты")]
        WidgetPVZ = 1
    }
}
