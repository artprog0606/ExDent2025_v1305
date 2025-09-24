using System;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Shipping.Grastin;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;
using AdvantShop.Shipping.Grastin;
using AdvantShop.Web.Admin.Models.Orders.Grastin;
using Newtonsoft.Json;

namespace AdvantShop.Web.Admin.Handlers.Orders.Shippings.Grastin
{
    public class GrastinOrderActions
    {
        private readonly int _orderId;

        public GrastinOrderActions(int orderId)
        {
            _orderId = orderId;
        }

        public OrderActionsModel Execute()
        {
            var model = new OrderActionsModel() {OrderId =  _orderId};

            if (string.IsNullOrEmpty(OrderService.GetOrderAdditionalData(_orderId, Shipping.Grastin.Grastin.KeyNameIsSendOrderInOrderAdditionalData)))
            {
                var orderPickPoint = OrderService.GetOrderPickPoint(_orderId);
                if (orderPickPoint != null && orderPickPoint.AdditionalData.IsNotEmpty())
                {
                    GrastinEventWidgetData grastinEventWidget = null;

                    try
                    {
                        grastinEventWidget =
                            JsonConvert.DeserializeObject<GrastinEventWidgetData>(orderPickPoint.AdditionalData);
                    }
                    catch (Exception ex)
                    {
                        Debug.Log.Warn(ex);
                    }

                    if (grastinEventWidget != null &&
                        grastinEventWidget.DeliveryType != EnDeliveryType.None &&
                        grastinEventWidget.Partner != EnPartner.None)
                    {
                        if (grastinEventWidget.Partner == EnPartner.Grastin)
                            model.ShowSendOrderForGrasting = true;

                        if (grastinEventWidget.Partner == EnPartner.RussianPost)
                            model.ShowSendOrderForRussianPost = true;

                        if (grastinEventWidget.Partner == EnPartner.Boxberry)
                            model.ShowSendOrderForBoxberry = true;

                        if (grastinEventWidget.Partner == EnPartner.Hermes)
                            model.ShowSendOrderForHermes = true;

                        if (grastinEventWidget.Partner == EnPartner.Partner)
                            model.ShowSendOrderForPartner = true;

                        if (grastinEventWidget.Partner == EnPartner.Cdek)
                            model.ShowSendOrderForCdek = true;

                    }
                }
            }
            else
            {
                //model.ShowSendRequestForIntake = true;
                model.ShowSendRequestForAct = true;
                model.ShowSendRequestForMark = true;
            }

            return model;
        }
    }
}
