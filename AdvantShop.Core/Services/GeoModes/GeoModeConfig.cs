using AdvantShop.Shipping;

namespace AdvantShop.GeoModes
{
    public sealed class GeoModeConfig
    {
        /// <summary>
        /// Текущий id доставки
        /// </summary>
        public const string PointCookieName = "geomode_point";
        
        /// <summary>
        /// id доставки последнего заказа
        /// </summary>
        public const string PreviousShippingIdCookieName = "prev_geomode";
        
        /// <summary>
        /// id пункта выдачи доставки последнего заказа
        /// </summary>
        public const string PreviousShippingPointIdCookieName = "prev_geomode_pid";
        
        /// <summary>
        /// Состояние переключателя: курьер | самовывоз (CourierType | PickPointType)
        /// </summary>
        public const string ShippingTypeCookieName = "advShippingType";
        
        /// <summary>
        /// Тип самовывоз
        /// </summary>
        public static string PickPointType => CalculationVariants.PickPoint.ToString().ToLower();
        
        /// <summary>
        /// Тип курьер
        /// </summary>
        public static string CourierType => CalculationVariants.Courier.ToString().ToLower();
    }
}