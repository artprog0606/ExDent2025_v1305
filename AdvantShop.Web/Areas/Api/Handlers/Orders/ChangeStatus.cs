using System;
using AdvantShop.Areas.Api.Model.Orders;
using AdvantShop.Core;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Orders
{
    public class ChangeStatus : ICommandHandler
    {
        private readonly ChangeStatusModel model;

        public ChangeStatus(ChangeStatusModel model)
        {
            this.model = model;
        }

        public void Execute()
        {
            try
            {
                OrderStatusService.ChangeOrderStatus(model.OrderId, model.StatusId, "Api");
            }
            catch (Exception ex)
            {
                Debug.Log.Warn(ex);
                throw new BlException(ex.Message);
            }
        }
    }
}