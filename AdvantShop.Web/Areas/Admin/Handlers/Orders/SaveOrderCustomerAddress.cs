using System;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using AdvantShop.Web.Admin.Models.Orders;

namespace AdvantShop.Web.Admin.Handlers.Orders
{
    public class SaveOrderCustomerAddress
    {
        private readonly OrderCustomerModel _orderCustomer;
        private readonly Order _order;

        public SaveOrderCustomerAddress(OrderCustomerModel orderCustomer)
        {
            _orderCustomer = orderCustomer;
            _order = OrderService.GetOrder(_orderCustomer.OrderId);
        }

        public bool Execute()
        {
            try
            {
                _order.IsFromAdminArea = true;
                
                if (_order.OrderCustomer == null)
                    _order.OrderCustomer = new OrderCustomer();

                if (_orderCustomer != null)
                {
                    if (_orderCustomer.OrderCustomer != null)
                    {
                        _order.OrderCustomer.CustomerID = _orderCustomer.OrderCustomer.CustomerID;

                        if (_order.OrderCustomer.CustomerID == Guid.Empty)
                            _order.OrderCustomer.CustomerID = Guid.NewGuid();

                        _order.OrderCustomer.Country = _orderCustomer.OrderCustomer.Country;
                        _order.OrderCustomer.Region = _orderCustomer.OrderCustomer.Region;
                        _order.OrderCustomer.District = _orderCustomer.OrderCustomer.District;
                        _order.OrderCustomer.City = _orderCustomer.OrderCustomer.City;
                        _order.OrderCustomer.Zip = _orderCustomer.OrderCustomer.Zip;

                        _order.OrderCustomer.Street = _orderCustomer.OrderCustomer.Street;
                        _order.OrderCustomer.House = _orderCustomer.OrderCustomer.House;
                        _order.OrderCustomer.Apartment = _orderCustomer.OrderCustomer.Apartment;
                        _order.OrderCustomer.Structure = _orderCustomer.OrderCustomer.Structure;
                        _order.OrderCustomer.Entrance = _orderCustomer.OrderCustomer.Entrance;
                        _order.OrderCustomer.Floor = _orderCustomer.OrderCustomer.Floor;
                    }
                }

                var changedBy = new OrderChangedBy(CustomerContext.CurrentCustomer);

                var updateModules = !_order.IsDraft;

                OrderService.UpdateOrderCustomer(_order.OrderCustomer, changedBy, updateModules);
                
                return true;
            }
            catch (Exception ex)
            {
                Debug.Log.Error("SaveOrderCustomerAddress error", ex);
            }

            return false;
        }
    }
}
