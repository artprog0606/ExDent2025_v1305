using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Scheduler;
using AdvantShop.Core.Scheduler.QuartzJobLogging;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Crm.Vk.VkMarket.Models;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using AdvantShop.Repository.Currencies;
using Quartz;
using VkNet;
using VkNet.Exception;

namespace AdvantShop.Core.Services.Crm.Vk.VkMarket
{
    [DisallowConcurrentExecution]
    public class VkOrdersJob : IJob
    {
        private readonly VkOrderService _vkOrderService = new VkOrderService();
        private readonly VkMarketApiService _vkMarketApiService = new VkMarketApiService();
        private readonly VkProductService _vkProductService = new VkProductService();
        private readonly VkApiService _apiService = new VkApiService();

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                var vkOrders = _vkMarketApiService.GetOrders();
                var vk = _apiService.AuthGroup();

                foreach (var vkOrder in vkOrders)
                {
                    if (_vkOrderService.GetOrderId(vkOrder.Id) != 0)
                        continue;

                    AddOrder(vkOrder, vk);
                }
            }
            catch (BlException ex)
            {
                Debug.Log.Error(ex);
                context.LogError(ex.Message);
                StopJob();
            }
            catch (UserAuthorizationFailException ex)
            {
                Debug.Log.Error(ex);
                context.LogError(ex.Message);
                StopJob();
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                context.LogError(ex.Message);
            }
        }

        private void AddOrder(VkOrder vkGroupOrder, VkApi vk)
        {
            var vkOrder = _vkMarketApiService.GetOrder(vkGroupOrder.Id, vkGroupOrder.UserId, true);
            if (vkOrder == null)
                return;
            
            Order order = null;

            try
            {
                order = new Order()
                {
                    OrderCustomer = TryGetOrAddCustomer(vk, vkOrder),
                    OrderItems = new List<OrderItem>(),
                    OrderCurrency = CurrencyService.CurrentCurrency,
                    OrderSourceId = OrderSourceService.GetOrderSource(OrderType.Vk).Id,
                    OrderStatusId = OrderStatusService.DefaultOrderStatus,
                    OrderDate = DateTime.Now,
                    AdminOrderComment = "",
                    CustomerComment = vkOrder.Comment
                };

                if (vkOrder.Delivery != null)
                {
                    order.ArchivedShippingName = !string.IsNullOrEmpty(vkOrder.Delivery.Type)
                        ? vkOrder.Delivery.Type
                        : "";

                    var deliveryCost = 
                        vkOrder.PriceDetails?.Find(x => x.Title != null && x.Title.Equals("Стоимость доставки", StringComparison.OrdinalIgnoreCase));

                    if (deliveryCost != null && deliveryCost.Price != null)
                        order.ShippingCost = deliveryCost.Price.Amount.TryParseLong() / 100;
                }
                
                
                if (vkOrder.Payment != null && vkOrder.Payment.PaymentStatus == "paid")
                {
                    order.PaymentDate = DateTime.Now;
                }
                
                var orderItems =
                    vkOrder.ItemsCount != vkOrder.OrderItems.Count 
                        ? _vkMarketApiService.GetOrderItems(vkOrder.Id, vkOrder.UserId)
                        : vkOrder.OrderItems;

                if (orderItems != null)
                {
                    foreach (var item in orderItems)
                    {
                        var orderItem = new OrderItem()
                        {
                            ArtNo = "",
                            Name = item.Title ?? "",
                            Price = !string.IsNullOrEmpty(item.Price?.Amount) ? item.Price.Amount.TryParseLong() / 100 : 0,
                            Amount = item.Quantity
                        };

                        SetProductId(item.ItemId, orderItem);

                        if (item.Item != null && item.Item.PropertyValues != null)
                            orderItem.Name += String.Join(", ", item.Item.PropertyValues.Select(x => x.PropertyName + " " + x.VariantName));

                        order.OrderItems.Add(orderItem);
                    }
                }

                order.OrderID = OrderService.AddOrder(order, new OrderChangedBy("vk.com API"));
                
                OrderStatusService.ChangeOrderStatusForNewOrder(order.OrderID);

                OrderMailService.SendMail(order);
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            if (order != null && order.OrderID != 0)
            {
                _vkOrderService.Add(order.OrderID, vkOrder.Id);
            }
        }

        private OrderCustomer TryGetOrAddCustomer(VkApi vk, VkOrder vkOrder)
        {
            var address = vkOrder.Delivery?.Address;
            
            var vkUser = VkService.GetUser(vkOrder.UserId);
            if (vkUser != null)
            {
                var customer = CustomerService.GetCustomer(vkUser.CustomerId);
                if (customer != null)
                {
                    var orderCustomer = new OrderCustomer() {CustomerID = customer.Id, Street = address};

                    if (vkOrder.Recipient != null)
                    {
                        if (!string.IsNullOrEmpty(vkOrder.Recipient.Name))
                            orderCustomer.FirstName = vkOrder.Recipient.Name;

                        if (!string.IsNullOrEmpty(vkOrder.Recipient.Phone))
                        {
                            orderCustomer.Phone = vkOrder.Recipient.Phone;
                            orderCustomer.StandardPhone = StringHelper.ConvertToStandardPhone(vkOrder.Recipient.Phone);
                        }
                    }

                    return orderCustomer;
                }
            }
            else if (vkOrder.Recipient != null)
            {
                var customer = new Customer(false)
                {
                    FirstName = vkOrder.Recipient.Name ?? "",
                    Phone = vkOrder.Recipient.Phone,
                    StandardPhone = StringHelper.ConvertToStandardPhone(vkOrder.Recipient.Phone),
                    Contacts = new List<CustomerContact>()
                    {
                        new CustomerContact() {Street = address}
                    },
                    IsAgreeForPromotionalNewsletter = SettingsDesign.ShowUserAgreementForPromotionalNewsletter 
                                                      && SettingsDesign.SetUserAgreementForPromotionalNewsletterChecked
                };

                CustomerService.InsertNewCustomer(customer);
                if (customer.Id != Guid.Empty)
                {
                    var userInfo = _apiService.GetUsersInfo(new List<long>() {vkOrder.UserId}, vk).FirstOrDefault();

                    var user = userInfo ?? new VkUser()
                    {
                        Id = vkOrder.UserId,
                        FirstName = customer.FirstName,
                        MobilePhone = customer.Phone,
                        HomePhone = customer.Phone,
                    };

                    if (userInfo != null)
                        SocialNetworkService.SaveAvatar(customer, user.Photo100);

                    if (string.IsNullOrEmpty(user.MobilePhone) && !string.IsNullOrEmpty(customer.Phone))
                        user.MobilePhone = customer.Phone;

                    user.CustomerId = customer.Id;

                    VkService.AddUser(user);

                    return (OrderCustomer) customer;
                }
            }

            return new OrderCustomer()
            {
                FirstName = vkOrder.Recipient != null ? vkOrder.Recipient.Name : "",
                Phone = vkOrder.Recipient != null ? vkOrder.Recipient.Phone : "",
                StandardPhone = vkOrder.Recipient != null
                    ? StringHelper.ConvertToStandardPhone(vkOrder.Recipient.Phone)
                    : default,
                Street = address
            };
        }

        private void SetProductId(long itemId, OrderItem orderItem)
        {
            var vkProduct = _vkProductService.Get(itemId);
            var offer = vkProduct != null ? OfferService.GetOffer(vkProduct.OfferId) : null;

            if (offer != null)
            {
                orderItem.ArtNo = offer.ArtNo;
                orderItem.ProductID = offer.ProductId;
                orderItem.Color = offer.ColorID != null ? offer.Color.ColorName : null;
                orderItem.Size = offer.SizeID != null ? offer.Size.SizeName : null;
                orderItem.PhotoID = offer.Photo != null ? offer.Photo.PhotoId : (int?) null;
                orderItem.Weight = offer.GetWeight();
                orderItem.Width = offer.GetWidth();
                orderItem.Length = offer.GetLength();
                orderItem.Height = offer.GetHeight();
                orderItem.SupplyPrice = offer.SupplyPrice;
            }
        }

        private void StopJob()
        {
            TaskManager.TaskManagerInstance().RemoveTask(nameof(VkOrdersJob), TaskManager.WebConfigGroup);
        }
    }
}

