using System;
using System.Linq;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;
using AdvantShop.Shipping;
using AdvantShop.Shipping.Measoft;
using AdvantShop.Shipping.Measoft.Api;
using AdvantShop.Web.Admin.Models.Orders.Measoft;
using Newtonsoft.Json;

namespace AdvantShop.Web.Admin.Handlers.Orders.Shippings.Measoft
{
    public class MeasoftOrderActions
    {
        private readonly int _orderId;

        public MeasoftOrderActions(int orderId)
        {
            _orderId = orderId;
        }

        public OrderActionsModel Execute()
        {
            var trackNumber = OrderService.GetOrderAdditionalData(_orderId,
                Shipping.Measoft.Measoft.TrackNumberOrderAdditionalDataName);

            return new OrderActionsModel
            {
                OrderId = _orderId,
                ShowCreateOrder = trackNumber.IsNullOrEmpty(),
                ShowDeleteOrder = trackNumber.IsNotEmpty()
            };
        }
    }
}
