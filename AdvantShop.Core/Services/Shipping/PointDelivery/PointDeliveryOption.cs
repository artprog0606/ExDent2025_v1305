using AdvantShop.Core.Common.Extensions;
using AdvantShop.Orders;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace AdvantShop.Shipping.PointDelivery
{
    public class PointDeliveryOption : BaseShippingOption, ISelectShippingPoint
    {
        public List<DeliveryPointShipping> ShippingPoints { get; set; }
        public string PointListTitle { get; set; }

        public PointDeliveryOption()
        {
        }

        public PointDeliveryOption(ShippingMethod method, float preCost) : base(method, preCost)
        {
            HideAddressBlock = true;
        }

        public override string TemplateName
        {
            get { return "PointDeliverySelectOption.html"; }
        }

        public override void Update(BaseShippingOption option)
        {
            var opt = option as PointDeliveryOption;
            if (opt != null && this.Id == opt.Id)
            {
                this.SelectedPoint = opt.SelectedPoint != null ? this.ShippingPoints.FirstOrDefault(x => x.Id == opt.SelectedPoint.Id) : null;
                // this.SelectedPoint = this.SelectedPoint ?? opt.ShippingPoints.FirstOrDefault();
            }
        }

        public override OrderPickPoint GetOrderPickPoint()
        {
            return SelectedPoint != null
                ? new OrderPickPoint
                {
                    PickPointId = SelectedPoint.Id,
                    WarehouseIds = SelectedPoint.WarehouseId.HasValue 
                        ? new List<int> {SelectedPoint.WarehouseId.Value} 
                        : null,
                    PickPointAddress = SelectedPoint.Address,
                    AdditionalData = JsonConvert.SerializeObject(SelectedPoint)
                }
                : null;
        }

        public override void UpdateFromOrderPickPoint(OrderPickPoint orderPickPoint)
        {
            if (orderPickPoint.PickPointId.IsNotEmpty())
                SelectedPoint = ShippingPoints?.FirstOrDefault(x => x.Id == orderPickPoint.PickPointId);
        }


        public void SelectShippingPoint(string pointId)
        {
            SelectedPoint = ShippingPoints.FirstOrDefault(x => x.Id == pointId) ?? 
                            ShippingPoints.FirstOrDefault();
        }
    }
}
