using System;
using AdvantShop.Core.Services.Bonuses;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using AdvantShop.Repository.Currencies;
using AdvantShop.Web.Admin.Models.Orders;

namespace AdvantShop.Web.Admin.Handlers.Orders
{
    public class SaveOrderDraftModel
    {
        public int OrderId { get; set; }
        public Guid CustomerId { get; set; }
    }

    public class SaveOrderDraft
    {
        private readonly OrderDraftModel _draftOrder;
        private readonly Order _order;

        public SaveOrderDraft(OrderDraftModel draftOrder)
        {
            _draftOrder = draftOrder;
            _order = OrderService.GetOrder(_draftOrder.OrderId) ?? new Order() {IsDraft = true};
        }

        public SaveOrderDraft(Order order)
        {
            _order = order;
            _order.IsDraft = false;
        }


        public SaveOrderDraftModel Execute()
        {
            if (_order.OrderID == 0)
            {
                _order.OrderDate = DateTime.Now;
                _order.OrderCurrency = CurrencyService.CurrentCurrency;
                _order.OrderStatusId = OrderStatusService.DefaultOrderStatus;
            }

            try
            {
                _order.IsFromAdminArea = true;

                var isOrderCustomerChanged = false;

                if (_order.OrderCustomer == null)
                    _order.OrderCustomer = new OrderCustomer();

                if (_draftOrder != null)
                {
                    _order.StatusComment = _draftOrder.StatusComment;
                    _order.AdminOrderComment = _draftOrder.AdminOrderComment;
                    _order.OrderSourceId = _draftOrder.OrderSourceId;
                    _order.ManagerId = _draftOrder.ManagerId;
                    _order.TrackNumber = _draftOrder.TrackNumber;

                    if (_draftOrder.OrderCustomer != null)
                    {
                        isOrderCustomerChanged = _order.OrderCustomer.CustomerID != _draftOrder.OrderCustomer.CustomerID;

                        SetOrderCustomer(_order.OrderCustomer, _draftOrder.OrderCustomer);

                        var currentCustomer = CustomerService.GetCustomer(_draftOrder.OrderCustomer.CustomerID);
                        var group = currentCustomer != null ? currentCustomer.CustomerGroup : CustomerGroupService.GetCustomerGroup();
                        _order.GroupDiscount = group.GroupDiscount;
                        _order.GroupName = group.GroupName;
                    }
                }

                var changedBy = new OrderChangedBy(CustomerContext.CurrentCustomer);
                var isNewOrder = false;

                if (_order.OrderID == 0)
                {
                    _order.OrderID = OrderService.AddOrder(_order, changedBy, trackChanges: !_order.IsDraft);
                    isNewOrder = !_order.IsDraft;
                }
                else
                {
                    var updateModules = false;

                    var prevOrder = OrderService.GetOrder(_order.OrderID);

                    if (!_order.IsDraft && prevOrder != null && prevOrder.IsDraft)
                        isNewOrder = updateModules = true;

                    OrderService.UpdateOrderMain(_order, updateModules: updateModules, changedBy: changedBy, trackChanges: false);
                    OrderService.UpdateOrderCustomer(_order.OrderCustomer, changedBy, false);
                    OrderService.UpdateStatusComment(_order.OrderID, _order.StatusComment);
                    
                    if (isOrderCustomerChanged && _order.OrderItems != null && _order.OrderItems.Count > 0)
                    {
                        foreach (var orderItem in _order.OrderItems)
                            orderItem.Price = OrderItemPriceService.CalculateFinalPrice(orderItem, _order, out _, out _);

                        OrderService.AddUpdateOrderItems(_order.OrderItems, OrderService.GetOrderItems(_order.OrderID),
                            _order, changedBy, false, updateModules);
                    }
                }
                
                if (isNewOrder)
                {
                    OrderStatusService.ChangeOrderStatusForNewOrder(_order.OrderID, LocalizationService.GetResource("Core.OrderStatus.CreatedFromAdmin"));
                    
                    Core.Services.Bonuses.Model.Card bonusCard = null;
                    if (BonusSystem.IsActive && (bonusCard = _order.GetOrderBonusCard()) != null && !bonusCard.Blocked)
                    {
                        BonusSystemService.MakeBonusPurchase(bonusCard.CardNumber, _order);
                    }
                }

                return new SaveOrderDraftModel()
                {
                    OrderId = _order.OrderID,
                    CustomerId = _order.OrderCustomer?.CustomerID ?? Guid.NewGuid()
                };
            }
            catch (Exception ex)
            {
                Debug.Log.Error("Update order draft", ex);
            }

            return null;
        }

        private void SetOrderCustomer(OrderCustomer orderCustomer, OrderCustomer draftCustomer)
        {
            orderCustomer.CustomerID = draftCustomer.CustomerID;
                        
            if (orderCustomer.CustomerID == Guid.Empty)
                orderCustomer.CustomerID = Guid.NewGuid();

            var isEmailChanged = orderCustomer.Email != draftCustomer.Email;
            
            orderCustomer.Email = draftCustomer.Email;
            
            var standardPhone = StringHelper.ConvertToStandardPhone(draftCustomer.Phone, true, true);
            var isPhoneChanged = orderCustomer.StandardPhone != standardPhone;
            
            orderCustomer.Phone = draftCustomer.Phone;
            orderCustomer.StandardPhone = standardPhone;

            var isCustomerIdChanged = false;
            
            if (isEmailChanged && !string.IsNullOrWhiteSpace(orderCustomer.Email))
            {
                var c = CustomerService.GetCustomerByEmail(orderCustomer.Email);
                if (c != null)
                {
                    orderCustomer.CustomerID = c.Id;
                    isCustomerIdChanged = true;
                }
            }
            
            if (!isCustomerIdChanged && 
                isPhoneChanged && orderCustomer.StandardPhone != null && orderCustomer.StandardPhone != 0)
            {
                var c = CustomerService.GetCustomerByPhone(orderCustomer.Phone, orderCustomer.StandardPhone);
                if (c != null)
                    orderCustomer.CustomerID = c.Id;
            }
            
            orderCustomer.FirstName = draftCustomer.FirstName;
            orderCustomer.LastName = draftCustomer.LastName;
            orderCustomer.Patronymic = draftCustomer.Patronymic;
            orderCustomer.Organization = draftCustomer.Organization;
            orderCustomer.CustomerType = draftCustomer.CustomerType;

            orderCustomer.Country = draftCustomer.Country;
            orderCustomer.Region = draftCustomer.Region;
            orderCustomer.District = draftCustomer.District;
            orderCustomer.City = draftCustomer.City;
            orderCustomer.Zip = draftCustomer.Zip;
            orderCustomer.CustomField1 = draftCustomer.CustomField1;
            orderCustomer.CustomField2 = draftCustomer.CustomField2;
            orderCustomer.CustomField3 = draftCustomer.CustomField3;

            orderCustomer.Street = draftCustomer.Street;
            orderCustomer.House = draftCustomer.House;
            orderCustomer.Apartment = draftCustomer.Apartment;
            orderCustomer.Structure = draftCustomer.Structure;
            orderCustomer.Entrance = draftCustomer.Entrance;
            orderCustomer.Floor = draftCustomer.Floor;
        }
    }
}
