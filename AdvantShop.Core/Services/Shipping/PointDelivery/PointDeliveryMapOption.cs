using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Orders;
using Newtonsoft.Json;

namespace AdvantShop.Shipping.PointDelivery
{
    public class PointDeliveryMapOption : BaseShippingOption, IPointDeliveryMapOption, ISelectShippingPoint
    {
        public MapParams MapParams { get; set; }
        public PointParams PointParams { get; set; }
        [JsonIgnore]
        public List<DeliveryPointShipping> MapPoints { get; set; }
        public int YaSelectedPoint { get; set; }
        public string PickpointId { get; set; }

        public PointDeliveryMapOption()
        {
        }

        public PointDeliveryMapOption(ShippingMethod method, float preCost) : base(method, preCost)
        {
            HideAddressBlock = true;
        }

        public override void Update(BaseShippingOption option)
        {
            var opt = option as PointDeliveryMapOption;
            if (opt != null && this.Id == opt.Id && this.MapPoints != null)
            {
                this.PickpointId = opt.PickpointId;
                this.YaSelectedPoint = opt.YaSelectedPoint;
                this.SelectedPoint = this.MapPoints.FirstOrDefault(x => x.Id == opt.PickpointId);
            }
        }

        public override string TemplateName
        {
            get { return "PointDeliveryMapOption.html"; }
        }

        public override OrderPickPoint GetOrderPickPoint()
        {
            return !string.IsNullOrWhiteSpace(PickpointId)
                ? new OrderPickPoint
                {
                    PickPointId = PickpointId,
                    WarehouseIds = SelectedPoint?.WarehouseId != null
                        ? new List<int> {SelectedPoint.WarehouseId.Value} 
                        : null,
                    PickPointAddress = SelectedPoint?.Address,
                    AdditionalData = JsonConvert.SerializeObject(SelectedPoint)
                }
                : null;
        }

        public override void UpdateFromOrderPickPoint(OrderPickPoint orderPickPoint)
        {
            if (orderPickPoint.PickPointId.IsNotEmpty())
            {
                PickpointId = orderPickPoint.PickPointId;
                SelectedPoint = MapPoints?.FirstOrDefault(x => x.Id == PickpointId);
                if (SelectedPoint != null)
                    YaSelectedPoint = SelectedPoint.Id.TryParseInt(SelectedPoint.Id.GetHashCode());
            }
        }

        public void SelectShippingPoint(string pointId)
        {
            SelectedPoint = MapPoints.FirstOrDefault(x => x.Id == pointId);
        }
    }

    public interface IPointDeliveryMapOption
    {
        MapParams MapParams { get; set; }
        PointParams PointParams { get; set; }
        int YaSelectedPoint { get; set; }
        string PickpointId { get; set; }
        BaseShippingPoint SelectedPoint { get; set; }
    }

    public class MapParams
    {
        public string Lang { get; set; }
        public string YandexMapsApikey { get; set; }
        public string Destination { get; set; }
    }

    public class PointParams
    {
        public Dictionary<string, object> LazyPointsParams { get; set; }

        /// <summary>
        /// FeatureCollection https://tech.yandex.ru/maps/jsapi/doc/2.1/dg/concepts/object-manager/frontend-docpage/#json-format
        /// </summary>
        public FeatureCollection Points { get; set; }
        public bool IsLazyPoints { get; set; }
        public bool PointsByDestination { get; set; }
    }

    public class FeatureCollection
    {
        [JsonProperty("type")]
        public string Type { get { return "FeatureCollection"; } }

        [JsonProperty("features")]
        public List<Feature> Features { get; set; }
    }

    public class Feature
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("type")]
        public string Type { get { return "Feature"; } }

        [JsonProperty("geometry")]
        public PointGeometry Geometry { get; set; }

        /// <summary>
        /// Properties https://tech.yandex.ru/maps/jsapi/doc/2.1/ref/reference/GeoObject-docpage/#GeoObject
        /// </summary>
        [JsonProperty("properties")]
        public PointProperties Properties { get; set; }

        /// <summary>
        /// Options https://tech.yandex.ru/maps/jsapi/doc/2.1/ref/reference/GeoObject-docpage/#GeoObject
        /// </summary>
        [JsonProperty("options")]
        public PointOptions Options { get; set; }
    }

    public class PointGeometry
    {
        [JsonProperty("type")]
        public string Type { get { return "Point"; } }
        [JsonIgnore]
        public float PointX { get; set; }
        [JsonIgnore]
        public float PointY { get; set; }

        [JsonProperty("coordinates")]
        public float[] Coordinates { get { return new float[] { PointX, PointY }; } }
    }

    public class PointProperties
    {
        [JsonProperty("hintContent")]
        public string HintContent { get; set; }

        [JsonProperty("balloonContentHeader")]
        public string BalloonContentHeader { get; set; }

        [JsonProperty("balloonContentBody")]
        public string BalloonContentBody { get; set; }

        [JsonProperty("balloonContentFooter")]
        public string BalloonContentFooter { get; set; }
    }

    public class PointOptions
    {
        [JsonProperty("preset")]
        public string Preset { get; set; }
    }

}
