using System.Collections.Generic;

namespace AdvantShop.Core.Services.Shipping.ApiShip.Map
{
    public class YPointParams
    {
        public Dictionary<string, object> LazyPointsParams { get; set; }

        /// <summary>
        /// FeatureCollection https://tech.yandex.ru/maps/jsapi/doc/2.1/dg/concepts/object-manager/frontend-docpage/#json-format
        /// </summary>
        public FeatureCollection Points { get; set; }
        public bool IsLazyPoints { get; set; }
        public bool PointsByDestination { get; set; }
    }
}
