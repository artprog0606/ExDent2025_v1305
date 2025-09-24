using System;
using System.Linq;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Orders;

namespace AdvantShop.Web.Admin.Handlers.Marketing.Analytics
{
    public class GetOrdersDataHandler
    {
        private readonly DateTime _from;
        private readonly DateTime _to;
        private readonly bool? _paid;
        private readonly int? _orderStatus;

        public GetOrdersDataHandler(DateTime from, DateTime to, bool? paid, int? orderStatus)
        {
            _from = from;
            _to = to;
            _paid = paid;
            _orderStatus = orderStatus;
        }

        public object Execute()
        {
            var allOrders = OrderService.GetAllOrders(_from, _to)
                .Where(x => (!_paid.HasValue || x.Payed == _paid.Value) 
                            && (!_orderStatus.HasValue || x.OrderStatusId == _orderStatus.Value))
                .ToList();

            var sumAllOrders = allOrders.Sum(i => i.Sum);
            var countOrder = allOrders.Count();

            var model = new
            {
                sumAllOrders = sumAllOrders.FormatPrice(),
                countOrder,
                average = countOrder != 0 ? ((float)Math.Round(sumAllOrders / countOrder, 2)).FormatPrice() : "0"
            };

            return model;
        }
    }
}
