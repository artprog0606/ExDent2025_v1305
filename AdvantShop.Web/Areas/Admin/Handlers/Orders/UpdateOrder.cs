using AdvantShop.Core.Services.Bonuses;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Core.Services.Crm.BusinessProcesses;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Core.Services.Smses;
using AdvantShop.Core.Services.Triggers;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using AdvantShop.Web.Admin.Models.Orders.OrdersEdit;
using System;
using System.Text.RegularExpressions;

namespace AdvantShop.Web.Admin.Handlers.Orders
{
    public class UpdateOrder
    {
        private readonly OrderModel _model;
        private readonly Order _order;

        public UpdateOrder()
        {
        }

        public UpdateOrder(OrderModel model)
        {
            _model = model;
            _order = _model.Order;
        }

        public bool Execute()
        {
            var order = OrderService.GetOrder(_order.OrderID);
            
            if (order == null || !OrderService.CheckAccess(order))
                return false;

            try
            {
                var payedChanged = _model.IsPayed != order.Payed;
                var managerConfirmedChanged = order.ManagerConfirmed != _order.ManagerConfirmed;

                order.StatusComment = _order.StatusComment;
                order.AdminOrderComment = _order.AdminOrderComment;
                order.ManagerConfirmed = _order.ManagerConfirmed;
                order.OrderSourceId = _order.OrderSourceId;
                order.ManagerId = _order.ManagerId;
                order.TrackNumber = _order.TrackNumber;
                order.OrderDate = _order.OrderDate;

                if (order.OrderDate == DateTime.MinValue)
                {
                    var d = DateTime.Now;
                    order.OrderDate = new DateTime(d.Year, d.Month, d.Day, d.Hour, d.Minute, d.Second, d.Kind);
                }

                if (Settings1C.Enabled)
                    order.UseIn1C = _order.UseIn1C;

                if (order.OrderCustomer == null)
                {
                    order.OrderCustomer = new OrderCustomer();
                }

                order.OrderCustomer.OrderID = _order.OrderID;
                order.OrderCustomer.CustomerID = _order.OrderCustomer.CustomerID != Guid.Empty ? _order.OrderCustomer.CustomerID : Guid.NewGuid();
                order.OrderCustomer.FirstName = _order.OrderCustomer.FirstName;
                order.OrderCustomer.LastName = _order.OrderCustomer.LastName;
                order.OrderCustomer.Patronymic = _order.OrderCustomer.Patronymic;
                order.OrderCustomer.Organization = _order.OrderCustomer.Organization;
                order.OrderCustomer.Email = _order.OrderCustomer.Email;
                order.OrderCustomer.CustomerType = _order.OrderCustomer.CustomerType;

                var phone = Regex.Replace(_order.OrderCustomer.Phone ?? string.Empty, @"[^\d]", "");

                order.OrderCustomer.Phone = !string.IsNullOrEmpty(phone) ? _order.OrderCustomer.Phone : "";
                order.OrderCustomer.StandardPhone =
                    !string.IsNullOrEmpty(order.OrderCustomer.Phone)
                        ? StringHelper.ConvertToStandardPhone(order.OrderCustomer.Phone, true, true)
                        : null;

                order.OrderCustomer.Country = _order.OrderCustomer.Country;
                order.OrderCustomer.Region = _order.OrderCustomer.Region;
                order.OrderCustomer.District = _order.OrderCustomer.District;
                order.OrderCustomer.City = _order.OrderCustomer.City;
                order.OrderCustomer.Zip = _order.OrderCustomer.Zip;
                //order.OrderCustomer.Address = _order.OrderCustomer.Address;
                order.OrderCustomer.CustomField1 = _order.OrderCustomer.CustomField1;
                order.OrderCustomer.CustomField2 = _order.OrderCustomer.CustomField2;
                order.OrderCustomer.CustomField3 = _order.OrderCustomer.CustomField3;

                order.OrderCustomer.Street = _order.OrderCustomer.Street;
                order.OrderCustomer.House = _order.OrderCustomer.House;
                order.OrderCustomer.Apartment = _order.OrderCustomer.Apartment;
                order.OrderCustomer.Structure = _order.OrderCustomer.Structure;
                order.OrderCustomer.Entrance = _order.OrderCustomer.Entrance;
                order.OrderCustomer.Floor = _order.OrderCustomer.Floor;

                var isDraftChanged = order.IsDraft != _order.IsDraft;

                order.IsDraft = _order.IsDraft;

                var changedBy = new OrderChangedBy(CustomerContext.CurrentCustomer);
                var isDraft = _order.IsDraft;

                OrderService.UpdateOrderMain(order, !isDraft, changedBy: changedBy, trackChanges: !isDraft && !isDraftChanged);
                OrderService.UpdateStatusComment(order.OrderID, order.StatusComment, changedBy: changedBy, trackChanges: !isDraft);

                if (OrderService.GetOrderCustomer(order.OrderID) == null)
                {
                    OrderService.AddOrderCustomer(order.OrderID, order.OrderCustomer);
                }
                else
                {
                    OrderService.UpdateOrderCustomer(order.OrderCustomer, changedBy, trackChanges: !isDraft);
                }

                if (!isDraft && isDraftChanged)
                {
                    var statusId = order.OrderStatusId != 0
                        ? order.OrderStatusId
                        : OrderStatusService.DefaultOrderStatus;

                    OrderHistoryService.NewOrder(order, changedBy);
                    OrderStatusService.ChangeOrderStatus(order.OrderID, statusId, LocalizationService.GetResource("Core.OrderStatus.CreatedFromAdmin"), false, true);

                    var couponCode = order.Coupon?.Code;
                    OrderService.CreateCustomerByOrderCustomer(order.OrderCustomer, couponCode);

                    if (_model.CustomerFields != null)
                    {
                        var customer = CustomerService.GetCustomer(_order.OrderCustomer.CustomerID);
                        if (customer != null)
                        {
                            foreach (var customerField in _model.CustomerFields)
                            {
                                CustomerFieldService.AddUpdateMap(customer.Id, customerField.Id, customerField.Value ?? "");
                            }
                        }
                    }

                    Core.Services.Bonuses.Model.Card bonusCard = null;
                    if (BonusSystem.IsActive && (bonusCard = _order.GetOrderBonusCard()) != null && !bonusCard.Blocked)
                    {
                        BonusSystemService.MakeBonusPurchase(bonusCard.CardNumber, order);
                    }

                    //ModulesExecuter.SendNotificationsOnOrderAdded(order); // it's call in UpdateOrderMain
                    OrderMailService.SendMail(order);
                    SmsNotifier.SendSmsOnOrderAdded(order);

                    BizProcessExecuter.OrderAdded(order);
                    Core.Services.Api.ApiWebhookExecuter.OrderAdded(order);

                    TriggerProcessService.ProcessEvent(ETriggerEventType.OrderCreated, order);
                    
                    Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Orders_OrderCreated_AdminArea);
                }
                else if (!isDraft)
                {
                    Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Orders_EditOrder);
                    if (payedChanged)
                    {
                        Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Orders_OrderPayed_AdminArea);
                    }

                    if (managerConfirmedChanged)
                        Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Orders_OrderConfirmedByManager);
                }
                
                if (payedChanged)
                {
                    SmsNotifier.SendSmsOnOrderPayStatusChanging(order);
                }

                new UpdateOrderTotal(OrderService.GetOrder(order.OrderID)).Execute();
                OrderService.PayOrder(_order.OrderID, _model.IsPayed, trackChanges: !_model.Order.IsDraft);
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                return false;
            }

            return true;
        }
    }
}