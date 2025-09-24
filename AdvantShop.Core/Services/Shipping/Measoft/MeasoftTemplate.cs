using AdvantShop.Core.Common.Attributes;

namespace AdvantShop.Shipping.Measoft
{
    public class MeasoftTemplate : DefaultWeightParams
    {
        public const string Login = "login";
        public const string Password = "pass";
        public const string Extra = "extra";
        public const string ClientCode = "clientcode";
        public const string StatusesSync = "StatusesSync";
        public const string StatusesReference = "StatusesReference";
        public const string DeliveryTypes = "DeliveryTypes";
        public const string CalculateCourier = "CalculateCourier";
        public const string YaMapsApiKey = "YaMapsApiKey";
        public const string TypeViewPoints = "TypeViewPoints";
        public const string WithInsure = "WithInsure";
        public const string ActiveDeliveryServices = "ActiveDeliveryServices";
        public const string PaymentCodCardId = "PaymentCodCardId";
        public const string ActiveStoreList = "ActiveStoreList";
    }

    public enum EMeasoftStatus
    {
        [Localize("Ожидает синхронизации")] AWAITING_SYNC = 0,
        [Localize("Успешно создан, передан в службу доставки")] NEW = 1,
        [Localize("Создан забор")] NEWPICKUP = 2,
        [Localize("Забран у отправителя")] PICKUP = 3,
        [Localize("Скомплектован на складе фулфилмента.")] WMSASSEMBLED = 4,
        [Localize("Разукомплектован на склад фулфилмента")] WMSDISASSEMBLED = 5,
        [Localize("Получен складом")] ACCEPTED = 6,
        [Localize("Производится таможенный контроль")] CUSTOMSPROCESS = 7,
        [Localize("Таможенный контроль произведен")] CUSTOMSFINISHED = 8,
        [Localize("Согласована доставка")] CONFIRM = 9,
        [Localize("Не удалось согласовать доставку")] UNCONFIRM = 10,
        [Localize("Планируется отправка со склада на другой склад")] DEPARTURING = 11,
        [Localize("Отправлено со склада на другой склад")] DEPARTURE = 12,
        [Localize("Инвентаризация")] INVENTORY = 13,
        [Localize("Готов к выдаче в ПВЗ")] PICKUPREADY = 14,
        [Localize("Выдан курьеру на доставку.")] DELIVERY = 15,
        [Localize("Доставлен, ожидает подтверждения менеджером")] COURIERDELIVERED = 16,
        [Localize("Частично доставлен, ожидает подтверждения менеджером")] COURIERPARTIALLY = 17,
        [Localize("Отказ предварительно")] COURIERCANCELED = 18,
        [Localize("Возвращено курьером")] COURIERRETURN = 19,
        [Localize("Перенос даты доставки")] DATECHANGE = 20,
        [Localize("Доставлен")] COMPLETE = 21,
        [Localize("Доставлен частично")] PARTIALLY = 22,
        [Localize("Не доставлен (Возврат/Отмена)")] CANCELED = 23,
        [Localize("Планируется возврат заказчику")] RETURNING = 24,
        [Localize("Возвращен заказчику.")] RETURNED = 25,
        [Localize("Утрачен/утерян")] LOST = 26,
        [Localize("Планируется возврат остатков")] PARTLYRETURNING = 27,
        [Localize("Остаток возвращен")] PARTLYRETURNED = 28,
        [Localize("Прибыл на склад перевозчика")] TRANSACCEPTED = 29,
        [Localize("Забран у перевозчика")] PICKUPTRANS = 30,
    }

    public enum TypeDelivery
    {
        [Localize("AdvantShop.Core.Shipping.TypeOfDelivery.SelfDelivery")]
        PVZ = 0,

        [Localize("AdvantShop.Core.Shipping.TypeOfDelivery.Courier")]
        Courier = 1
    }
}
