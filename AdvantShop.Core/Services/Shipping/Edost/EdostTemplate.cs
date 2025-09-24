//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using AdvantShop.Core.Common.Attributes;

namespace AdvantShop.Shipping.Edost
{
    public enum EDemensionsUnit
    {
        [StringName("Не выбрано")]
        None = 0,
        [StringName("Сантиметры")]
        Centimeters = 10,
        [StringName("Миллиметры")]
        Millimeters = 1,
        [StringName("Метры")]
        Meters = 1000,
    }

    public class EdostTemplate : DefaultCargoParams
    {
        public const string ShopId = "ShopId";
        public const string Password = "Password";
        public const string Extracharge = "Extracharge";
        public const string ShipIdCOD = "ShipIdCOD";
        public const string EnabledCOD = "EnabledCOD";
        public const string ShipIdPickPoint = "ShipIdPickPoint";
        public const string EnabledPickPoint = "EnabledPickPoint";
        public const string DemensionsUnit = "DemensionsUnit";
    }
}