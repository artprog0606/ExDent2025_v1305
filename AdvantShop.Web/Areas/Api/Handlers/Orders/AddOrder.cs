using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Areas.Api.Model.Orders;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Bonuses;
using AdvantShop.Core.Services.Bonuses.Model;
using AdvantShop.Core.Services.Bonuses.Service;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Mails;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Core.Services.Webhook.Models.Api;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Mails;
using AdvantShop.Orders;
using AdvantShop.Repository.Currencies;
using AdvantShop.Taxes;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Orders
{
    public class AddOrder : ICommandHandler<AddOrderModel, OrderModel>
    {
        public OrderModel Execute(AddOrderModel model)
        {
            var customer = TryGetCustomer(model.OrderCustomer);

            Card bonusCard = null;
            
            if (model.BonusCardNumber != null)
            {
                bonusCard = CardService.Get(model.BonusCardNumber.Value);
                if (bonusCard == null)
                    throw new BlException("Бонусная карта не найдена");

                if (customer?.BonusCardNumber != null && 
                    customer.BonusCardNumber != bonusCard.CardNumber)
                    throw new BlException("Бонусная карта не принадлежит покупателю");
            }
            
            if (bonusCard == null && customer?.BonusCardNumber != null)
                bonusCard = CardService.Get(customer.BonusCardNumber.Value);

            if (model.BonusCost != 0)
            {
                if (bonusCard == null)
                    throw new BlException("Бонусная карта не найдена");
                
                if (bonusCard.Blocked)
                    throw new BlException("Бонусная карта заблокирована");
                
                if ((float)bonusCard.BonusesTotalAmount < model.BonusCost)
                    throw new BlException($"Не возможно списать {model.BonusCost} бонусов. На карте всего {bonusCard.BonusesTotalAmount} бонусов.");
            }
            
            try
            {
                var order = new Order()
                {
                    OrderCustomer = new OrderCustomer()
                    {
                        CustomerID = model.OrderCustomer.CustomerId ?? Guid.Empty,
                        FirstName = model.OrderCustomer.FirstName.EncodeOrEmpty(),
                        LastName = model.OrderCustomer.LastName.EncodeOrEmpty(),
                        Patronymic = model.OrderCustomer.Patronymic.EncodeOrEmpty(),
                        Organization = model.OrderCustomer.Organization.EncodeOrEmpty(),
                        Email = model.OrderCustomer.Email.EncodeOrEmpty(),
                        Phone = model.OrderCustomer.Phone.EncodeOrEmpty(),
                        Country = model.OrderCustomer.Country.EncodeOrEmpty(),
                        Region = model.OrderCustomer.Region.EncodeOrEmpty(),
                        District = model.OrderCustomer.District.EncodeOrEmpty(),
                        City = model.OrderCustomer.City.EncodeOrEmpty(),
                        Zip = model.OrderCustomer.Zip.EncodeOrEmpty(),
                        CustomField1 = model.OrderCustomer.CustomField1.EncodeOrEmpty(),
                        CustomField2 = model.OrderCustomer.CustomField2.EncodeOrEmpty(),
                        CustomField3 = model.OrderCustomer.CustomField3.EncodeOrEmpty(),
                        Street = model.OrderCustomer.Street.EncodeOrEmpty(),
                        House = model.OrderCustomer.House.EncodeOrEmpty(),
                        Apartment = model.OrderCustomer.Apartment.EncodeOrEmpty(),
                        Structure = model.OrderCustomer.Structure.EncodeOrEmpty(),
                        Entrance = model.OrderCustomer.Entrance.EncodeOrEmpty(),
                        Floor = model.OrderCustomer.Floor.EncodeOrEmpty(),
                    },
                    OrderItems = new List<OrderItem>(),
                    OrderStatusId = OrderStatusService.DefaultOrderStatus,
                    OrderDate = DateTime.Now,
                    CustomerComment = model.CustomerComment.EncodeOrEmpty(),
                    AdminOrderComment = model.AdminComment.EncodeOrEmpty(),

                    ArchivedShippingName = model.ShippingName.EncodeOrEmpty(),
                    ArchivedPaymentName = model.PaymentName.EncodeOrEmpty(),
                    DeliveryTime = model.DeliveryTime.EncodeOrEmpty(),
                    DeliveryDate = model.DeliveryDate,

                    ShippingCost = model.ShippingCost,
                    PaymentCost = model.PaymentCost,
                    BonusCost = model.BonusCost,
                    OrderDiscount = model.OrderDiscount,
                    OrderDiscountValue = model.OrderDiscountValue,

                    BonusCardNumber = model.BonusCardNumber,
                    LpId = model.LpId,
                    AffiliateID = model.AffiliateId ?? 0,
                    TrackNumber = model.TrackNumber.EncodeOrEmpty(),

                    TotalWeight = model.TotalWeight,
                    TotalLength = model.TotalLength,
                    TotalWidth = model.TotalWidth,
                    TotalHeight = model.TotalHeight,

                    LinkedCustomerId = customer?.Id
                };

                order.OrderCustomer.StandardPhone =
                    !string.IsNullOrWhiteSpace(order.OrderCustomer.Phone)
                        ? StringHelper.ConvertToStandardPhone(order.OrderCustomer.Phone, force: true)
                        : null;

                if (!string.IsNullOrWhiteSpace(model.OrderSource))
                {
                    var orderSource = OrderSourceService.GetOrderSource(model.OrderSource);
                    if (orderSource == null)
                    {
                        orderSource = new OrderSource() {Name = model.OrderSource};

                        OrderSourceService.AddOrderSource(orderSource);
                    }
                    order.OrderSourceId = orderSource.Id;
                }

                Currency currency = null;

                if (!string.IsNullOrWhiteSpace(model.Currency))
                    currency = CurrencyService.Currency(model.Currency);

                if (currency == null)
                    currency = CurrencyService.Currency(SettingsCatalog.DefaultCurrencyIso3);

                order.OrderCurrency = currency;

                if (!string.IsNullOrEmpty(model.ShippingTaxName))
                {
                    var tax =
                        TaxService.GetTaxes().FirstOrDefault(x => x.Name.ToLower() == model.ShippingTaxName.ToLower());
                    if (tax != null)
                        order.ShippingTaxType = tax.TaxType;
                }

                if (model.OrderItems != null)
                {
                    foreach (var item in model.OrderItems)
                    {
                        var offer = OfferService.GetOffer(item.ArtNo);
                        if (offer == null)
                        {
                            var p = ProductService.GetProduct(item.ArtNo);
                            if (p != null && p.Offers.Count == 1)
                                offer = p.Offers[0];
                        }

                        if (model.CheckOrderItemExist && offer == null)
                        {
                            throw new BlException($"Товар с артикулом {item.ArtNo} не найден. Заказ не будет создан");
                        }

                        var allowBuyOutOfStockProducts = offer?.Product.AllowBuyOutOfStockProducts() ?? SettingsCheckout.OutOfStockAction == eOutOfStockAction.Cart;
                        var available = offer != null && (offer.Amount > 0 || allowBuyOutOfStockProducts) &&
                                        (offer.RoundedPrice >= 0 || allowBuyOutOfStockProducts);

                        if (model.CheckOrderItemAvailable &&
                            (offer == null || !offer.Product.Enabled || !offer.Product.CategoryEnabled || !available))
                        {
                            throw new BlException($"Товар {item.ArtNo} не в наличии. Заказ не будет создан");
                        }

                        if (offer != null)
                        {
                            order.OrderItems.Add(GetOrderItem(offer, item.Price, item.Amount));
                        }
                        else
                        {
                            order.OrderItems.Add(new OrderItem()
                            {
                                ArtNo = item.ArtNo.EncodeOrEmpty(),
                                Name = item.Name.EncodeOrEmpty(),
                                Price = item.Price ?? 0,
                                Amount = item.Amount ?? 1
                            });
                        }
                    }
                }

                if (!string.IsNullOrEmpty(model.ManagerEmail))
                {
                    var managerCustomer = CustomerService.GetCustomerByEmail(model.ManagerEmail);
                    if (managerCustomer != null && managerCustomer.IsManager)
                    {
                        var manager = ManagerService.GetManager(managerCustomer.Id);
                        if (manager != null)
                            order.ManagerId = manager.ManagerId;
                    }
                }

                if (model.ValidateBonusCost && 
                    model.BonusCost != 0 && bonusCard != null && BonusSystem.MaxOrderPercent != 100)
                {
                    var totalPrice = order.OrderItems.Sum(x => x.Price * x.Amount);
                    var totalDiscount = order.TotalDiscount;

                    var productsPrice = totalPrice - totalDiscount;
                    
                    var priceForBonusCost = 
                        productsPrice - order.OrderItems.Where(x => x.DoNotApplyOtherDiscounts).Sum(x => (x.Price - x.Price / totalPrice * totalDiscount) * x.Amount);
                    
                    var sumPrice = BonusSystem.BonusType == EBonusType.ByProductsCostWithShipping
                        ? priceForBonusCost + order.ShippingCost
                        : priceForBonusCost;

                    var bonusPrice = sumPrice > order.BonusCost ? order.BonusCost : sumPrice;

                    if (bonusPrice * 100 / sumPrice > BonusSystem.MaxOrderPercent)
                        throw new BlException($"Заказ может быть оплачен бонусами на {BonusSystem.MaxOrderPercent}%. Для этого заказ можно списать {(sumPrice * BonusSystem.MaxOrderPercent / 100).SimpleRoundPrice()} бонусов.");
                }

                OrderService.AddOrder(order, new OrderChangedBy("Api"), true, !model.SkipBusinessProcessesAfterCreatingOrder, !model.SkipBusinessProcessesAfterCreatingOrder, model.SkipBusinessProcessesAfterCreatingOrder);

                var status = !string.IsNullOrEmpty(model.OrderStatusName)
                    ? OrderStatusService.GetOrderStatuses()
                        .FirstOrDefault(x => x.StatusName.ToLower() == model.OrderStatusName.ToLower())
                    : null;
                var statusId = status?.StatusID ?? OrderStatusService.DefaultOrderStatus;

                OrderStatusService.ChangeOrderStatus(order.OrderID, statusId, LocalizationService.GetResource("Core.OrderStatus.Created"), false);

                order.OrderStatusId = statusId;
                order.OrderStatus = null;

                SetOrderNumber(order, model);
                
                if (!model.SkipBonusPurchase && BonusSystem.IsActive && bonusCard != null && !bonusCard.Blocked)
                {
                    BonusSystemService.MakeBonusPurchase(bonusCard.CardNumber, order);
                }

                if (model.IsPaied)
                    OrderService.PayOrder(
                        order.OrderID,
                        pay: true,
                        !model.SkipBusinessProcessesAfterCreatingOrder,
                        changedBy: new OrderChangedBy("Api"));

                if (!model.SkipBusinessProcessesAfterCreatingOrder)
                {
                    var mail = NewOrderMailTemplate.Create(order);
                    MailService.SendMailNow(SettingsMail.EmailForOrders, mail);
                }

                return OrderModel.FromOrder(order);
            }
            catch (BlException ex)
            {
                Debug.Log.Warn(ex);

                throw;
            }
            catch (Exception ex)
            {
                Debug.Log.Warn(ex);
                throw new BlException(ex.Message);
            }
        }

        private OrderItem GetOrderItem(Offer offer, float? price, float? amount)
        {
            if (amount == null)
                amount = offer.Product.GetMinAmount();

            var orderItem = new OrderItem
            {
                ProductID = offer.ProductId,
                Name = offer.Product.Name,
                ArtNo = offer.ArtNo,
                BarCode = offer.BarCode,
                Price = price == null ? offer.RoundedPrice : price.Value,
                BasePrice = price == null ? offer.RoundedPrice : price.Value,
                Amount = amount ?? 1,
                SupplyPrice = offer.SupplyPrice,
                Color = offer.ColorID != null ? offer.Color.ColorName : null,
                Size = offer.SizeID != null ? offer.SizeForCategory.GetFullName() : null,
                PhotoID = offer.Photo != null ? offer.Photo.PhotoId : (int?)null,
                AccrueBonuses = offer.Product.AccrueBonuses,
                Weight = offer.GetWeight(),
                Width = offer.GetWidth(),
                Length = offer.GetLength(),
                Height = offer.GetHeight(),
                PaymentMethodType = offer.Product.PaymentMethodType,
                PaymentSubjectType = offer.Product.PaymentSubjectType,
                MeasureType = offer.Product.Unit?.MeasureType,
                Unit = offer.Product.Unit?.DisplayName,
                TypeItem = TypeOrderItem.Product,
                DoNotApplyOtherDiscounts = offer.Product.DoNotApplyOtherDiscounts,
                IsMarkingRequired = offer.Product.IsMarkingRequired,
                DownloadLink = offer.Product.DownloadLink
            };
            
            orderItem.IgnoreOrderDiscount |= orderItem.DoNotApplyOtherDiscounts;

            var tax = offer.Product.TaxId != null ? TaxService.GetTax(offer.Product.TaxId.Value) : null;
            if (tax != null && tax.Enabled)
            {
                orderItem.TaxId = tax.TaxId;
                orderItem.TaxName = tax.Name;
                orderItem.TaxType = tax.TaxType;
                orderItem.TaxRate = tax.Rate;
                orderItem.TaxShowInPrice = tax.ShowInPrice;
            }

            return orderItem;
        }

        private Customer TryGetCustomer(OrderModelCustomerDto orderCustomer)
        {
            if (orderCustomer == null)
                return null;
            
            Customer customer = null;

            if (orderCustomer.CustomerId.HasValue)
                customer = CustomerService.GetCustomer(orderCustomer.CustomerId.Value);

            if (customer == null && orderCustomer.Email.IsNotEmpty())
                customer = CustomerService.GetCustomerByEmail(orderCustomer.Email);

            if (customer == null && orderCustomer.Phone.IsNotEmpty())
            {
                var standardPhone = StringHelper.ConvertToStandardPhone(orderCustomer.Phone, force: true);
                customer = CustomerService.GetCustomerByPhone(orderCustomer.Phone, standardPhone);
            }

            return customer;
        }

        private void SetOrderNumber(Order order, AddOrderModel model)
        {
            var setNumber = false;
            
            if (!string.IsNullOrEmpty(model.Number))
            {
                var number = !string.IsNullOrEmpty(model.OrderPrefix) ? model.OrderPrefix + model.Number : model.Number;
                if (OrderService.GetOrderIdByNumber(number) == 0)
                {
                    order.Number = number;
                    OrderService.UpdateNumber(order.OrderID, order.Number);
                    setNumber = true;
                }
            }
                
            if (!setNumber && !string.IsNullOrEmpty(model.OrderPrefix))
            {
                var number = model.OrderPrefix + order.OrderID;
                if (OrderService.GetOrderIdByNumber(number) == 0)
                {
                    order.Number = number;
                    OrderService.UpdateNumber(order.OrderID, order.Number);
                }
            }
        }
    }
}