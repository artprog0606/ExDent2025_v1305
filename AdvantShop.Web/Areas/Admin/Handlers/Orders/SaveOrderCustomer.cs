using System;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using AdvantShop.Web.Admin.Models.Orders;

namespace AdvantShop.Web.Admin.Handlers.Orders
{
    public class SaveOrderCustomer
    {
        private readonly OrderCustomerModel _model;
        private readonly Order _order;

        public SaveOrderCustomer(OrderCustomerModel model)
        {
            _model = model;
            _order = OrderService.GetOrder(_model.OrderId);
        }

        public bool Execute()
        {
            try
            {
                _order.IsFromAdminArea = true;
                
                var isOrderCustomerChanged = false;

                if (_order.OrderCustomer == null)
                    _order.OrderCustomer = new OrderCustomer();

                if (_model != null && _model.OrderCustomer != null)
                {
                    isOrderCustomerChanged = _order.OrderCustomer.CustomerID != _model.OrderCustomer.CustomerID;

                    SetOrderCustomer(_order.OrderCustomer, _model.OrderCustomer);
                    
                    var currentCustomer = CustomerService.GetCustomer(_model.OrderCustomer.CustomerID);
                    var group = currentCustomer != null ? currentCustomer.CustomerGroup : CustomerGroupService.GetCustomerGroup();
                    
                    _order.GroupDiscount = group.GroupDiscount;
                    _order.GroupName = group.GroupName;
                }

                if (isOrderCustomerChanged)
                    _order.BonusCardNumber = null;

                var changedBy = new OrderChangedBy(CustomerContext.CurrentCustomer);

                var updateModules = !_order.IsDraft;

                OrderService.UpdateOrderCustomer(_order.OrderCustomer, changedBy, updateModules);
                OrderService.UpdateOrderMain(_order, updateModules: updateModules, changedBy: changedBy, trackChanges: updateModules);
                
                
                if (isOrderCustomerChanged && _order.OrderItems != null && _order.OrderItems.Count > 0)
                {
                    foreach (var orderItem in _order.OrderItems)
                        orderItem.Price = OrderItemPriceService.CalculateFinalPrice(orderItem, _order, out _, out _);

                    OrderService.AddUpdateOrderItems(_order.OrderItems, OrderService.GetOrderItems(_order.OrderID),
                        _order, changedBy, false, updateModules);
                }

                if (_order.OrderCustomer.CustomerID != Guid.Empty && _model != null &&
                    _model.CustomerFields != null && _model.CustomerFields.Count > 0)
                {
                    var customer = CustomerService.GetCustomer(_order.OrderCustomer.CustomerID);
                    if (customer != null)
                    {
                        foreach (var customerField in _model.CustomerFields)
                            CustomerFieldService.AddUpdateMap(customer.Id, customerField.Id, customerField.Value ?? "");
                    }
                }

                if (isOrderCustomerChanged)
                    new UpdateOrderTotal(_model.OrderId, null).Execute();

                return true;
            }
            catch (Exception ex)
            {
                Debug.Log.Error("SaveOrderCustomer error", ex);
            }

            return false;
        }

        private void SetOrderCustomer(OrderCustomer orderCustomer, OrderCustomer newCustomer)
        {
            orderCustomer.CustomerID = newCustomer.CustomerID;
                        
            if (orderCustomer.CustomerID == Guid.Empty)
                orderCustomer.CustomerID = Guid.NewGuid();

            var isEmailChanged = orderCustomer.Email != newCustomer.Email;
            orderCustomer.Email = newCustomer.Email;
            
            var standardPhone = StringHelper.ConvertToStandardPhone(newCustomer.Phone, true, true);
            var isPhoneChanged = orderCustomer.StandardPhone != standardPhone;
            
            orderCustomer.Phone = newCustomer.Phone;
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
            
            orderCustomer.FirstName = newCustomer.FirstName;
            orderCustomer.LastName = newCustomer.LastName;
            orderCustomer.Patronymic = newCustomer.Patronymic;
            orderCustomer.Organization = newCustomer.Organization;
            orderCustomer.CustomerType = newCustomer.CustomerType;
                        
            orderCustomer.CustomField1 = newCustomer.CustomField1;
            orderCustomer.CustomField2 = newCustomer.CustomField2;
            orderCustomer.CustomField3 = newCustomer.CustomField3;
        }
    }
}
