using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Models.MyAccount;
using AdvantShop.Orders;
using AdvantShop.Shipping;

namespace AdvantShop.Handlers.Common
{
    public class GetShippingHistoryAndPointInfoHandler
    {
        private readonly Order _order;

        public GetShippingHistoryAndPointInfoHandler(Order order)
        {
            _order = order;
        }

        public ShippingHistoryOfMovementInfo Get()
        {
            if (_order?.ShippingMethod is null)
                return null;

            var shippingMethod = _order.ShippingMethod;
            var pointId = _order.OrderPickPoint?.PickPointId;
            
            var shippingType = ReflectionExt.GetTypeByAttributeValue<Core.Common.Attributes.ShippingKeyAttribute>(typeof(BaseShipping), atr => atr.Value, shippingMethod.ShippingType);
            var shipping = (BaseShipping)Activator.CreateInstance(shippingType, shippingMethod, null);
            
            var pointInfo = pointId.IsNotEmpty()
                ? shipping.LoadShippingPointInfo(pointId)
                : null;
            List<HistoryOfMovement> historyOfMovement = null;
            
            if (shipping is IShippingSupportingTheHistoryOfMovement iShippingSupportingTheHistoryOfMovement)
                if (iShippingSupportingTheHistoryOfMovement.ActiveHistoryOfMovement)
                    historyOfMovement = iShippingSupportingTheHistoryOfMovement.GetHistoryOfMovement(_order);

            return pointInfo is null
                   && historyOfMovement is null
                ? null
                : new ShippingHistoryOfMovementInfo
                {
                    HistoryOfMovement = historyOfMovement?.Select(HistoryOfMovementModel.CreateBy).ToList(),
                    PointInfo = pointInfo != null
                        ? PointInfoModel.CreateBy(pointInfo)
                        : null
                };
        }
    }
}