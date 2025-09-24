using System;
using AdvantShop.Areas.Api.Model.Orders;
using AdvantShop.Core;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Orders
{
    public class SetPaid : ICommandHandler
    {
        private readonly SetPaidModel model;

        public SetPaid(SetPaidModel model)
        {
            this.model = model;
        }

        public void Execute()
        {
            try
            {
                OrderService.PayOrder(model.OrderId, true, changedBy: new OrderChangedBy("Api"));
            }
            catch (Exception ex)
            {
                Debug.Log.Warn(ex);
                throw new BlException(ex.Message);
            }
        }
    }
}