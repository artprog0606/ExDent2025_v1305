using System.Linq;
using AdvantShop.Areas.Api.Models.Shared;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Shipping;

namespace AdvantShop.Areas.Api.Models.Deliveries.v2
{
    internal sealed class DeliveryPoint
    {
        internal DeliveryPoint(string id)
        {
            Id = id;
        }

        internal static DeliveryPoint CreateBy(BaseShippingPoint point)
        {
            var timeOfWork = point.TimeWorkStr;
            if (timeOfWork == null && point.TimeWork != null && point.TimeWork.Count > 0)
                timeOfWork = string.Join("<br/> ", point.TimeWork.Select(x => $"{x.Label} {x.From} - {x.To}"));
                
            return new DeliveryPoint(point.Id)
            {
                Name = point.Name,
                Address = point.Address,
                Description = point.Description,
                AddressComment = point.AddressComment,
                Phones = point.Phones,
                TimeWork = timeOfWork.IsNotEmpty() ? timeOfWork : null,
                WarehouseId = point.WarehouseId,
                Coordinates =
                    point.Latitude.HasValue && point.Longitude.HasValue
                        ? new DeliveryPointCoordinates(point.Latitude.Value, point.Longitude.Value)
                        : null
            };
        }

        internal void SetInHouse(bool inHouse) => InHouse = inHouse;

        public string Id { get; }
        public string Name { get; private set; }
        public string Address { get; private set; }
        public string Description { get; private set; }
        public string TimeWork { get; private set; }
        public string AddressComment { get; private set; }
        public string[] Phones { get; private set; }
        public bool InHouse { get; private set; }
        public int? WarehouseId { get; private set; }
        public IGeoCoordinates Coordinates { get; private set;  }
    }

    public class DeliveryPointCoordinates : IGeoCoordinates
    {
        public DeliveryPointCoordinates(float latitude, float longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public float Latitude { get; }
        public float Longitude { get; }
    }
}