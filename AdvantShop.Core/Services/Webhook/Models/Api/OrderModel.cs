using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Api;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using Newtonsoft.Json;

namespace AdvantShop.Core.Services.Webhook.Models.Api
{
    public class OrderModel : IApiResponse
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public string Currency { get; set; }
        public float Sum { get; set; }
        
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string PreparedSum { get; set; }
        
        public DateTime Date { get; set; }

        public string CustomerComment { get; set; }
        public string AdminComment { get; set; }

        public string PaymentName { get; set; }
        public float PaymentCost { get; set; }
        
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string PreparedPaymentCost { get; set; }

        public string ShippingName { get; set; }
        public float ShippingCost { get; set; }
        
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string PreparedShippingCost { get; set; }
        
        public string ShippingTaxName { get; set; }
        public string TrackNumber { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string DeliveryTime { get; set; }
        
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ShippingAddress { get; set; }
        
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string PickPointAddress { get; set; }
        
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string PickPointData { get; set; }

        public float OrderDiscount { get; set; }
        public float OrderDiscountValue { get; set; }
        
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public float? DiscountPrice { get; set; }
        
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string PreparedDiscountCost { get; set; }

        public long? BonusCardNumber { get; set; }
        public float BonusCost { get; set; }
        
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string PreparedBonusCost { get; set; }
        
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string PreparedNewBonusesForOrder { get; set; }
        
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public float? ProductsCost { get; set; }
        
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string PreparedProductsCost { get; set; }
        
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Coupon { get; set; }
        
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public float? CouponPrice { get; set; }
        
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string PreparedCouponPrice { get; set; }
        
        public int? LpId { get; set; }

        public bool IsPaid { get; set; }
        
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string BillingApiLink { get; set; }
        
        public DateTime? PaymentDate { get; set; }
        
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public OrderCustomerModel Customer { get; set; }
        
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public OrderStatusModel Status { get; set; }
        
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public OrderSourceModel Source { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<OrderItemModel> Items { get; set; }
        
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<KeyValuePair<string,string>> Taxes { get; set; }
        
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public OrderReviewModel Review { get; set; }
        
        public DateTime ModifiedDate { get; set; }
        
        [JsonIgnore]
        public bool IsDraft { get; set; }
        
        [JsonIgnore]
        public Guid Code { get; set; }
        
        public static OrderModel FromOrder(Order order, bool preparePrices = false, bool loadExtended = false)
        {
            if (order == null || order.IsDraft)
                return null;

            var model = new OrderModel
            {
                Id = order.OrderID,
                Number = order.Number,
                Currency = order.OrderCurrency?.CurrencyCode,
                Sum = order.Sum,
                Date = order.OrderDate,
                ModifiedDate = order.ModifiedDate,

                CustomerComment = order.CustomerComment,
                AdminComment = order.AdminOrderComment,

                PaymentName = order.ArchivedPaymentName,
                PaymentCost = order.PaymentCost,

                ShippingName = order.ArchivedShippingName,
                ShippingCost = order.ShippingCost,
                ShippingTaxName = order.ShippingTaxType.Localize(),
                TrackNumber = order.TrackNumber,
                DeliveryDate = order.DeliveryDate,
                DeliveryTime = order.DeliveryTime,

                OrderDiscount = order.OrderDiscount,
                OrderDiscountValue = order.OrderDiscountValue,

                BonusCardNumber = order.BonusCardNumber,
                BonusCost = order.BonusCost,

                LpId = order.LpId,

                IsPaid = order.Payed,
                PaymentDate = order.PaymentDate,

                Customer = OrderCustomerModel.FromOrderCustomer(order.OrderCustomer),

                Status = OrderStatusModel.FromOrderStatus(order.OrderStatus),

                Source = OrderSourceModel.FromOrderSource(order.OrderSource),

                Items = order.OrderItems != null
                    ? order.OrderItems.Select(OrderItemModel.FromOrderItem).ToList()
                    : new List<OrderItemModel>(),
                
                Coupon = order.Coupon?.ToString(),
                
                BillingApiLink = 
                    order.ShowBillingLink() 
                        ? UrlService.GetUrl($"checkout/apiauth?type=billing&code={order.Code}&hash={OrderService.GetBillingLinkHash(order)}") 
                        : null
            };

            if (loadExtended)
            {
                model.ProductsCost = order.OrderCertificates != null && order.OrderCertificates.Count > 0
                    ? order.OrderCertificates.Sum(x => x.Sum)
                    : order.OrderItems.Sum(x => PriceService.SimpleRoundPrice(x.Price * x.Amount, order.OrderCurrency));

                model.PickPointAddress = order.OrderPickPoint?.PickPointAddress;
                model.PickPointData = order.OrderPickPoint?.AdditionalData;
                
                model.DiscountPrice = order.GetOrderDiscountPrice();
                model.CouponPrice = order.GetOrderCouponPrice();
                model.ShippingAddress = order.OrderCustomer != null ? order.OrderCustomer.GetCustomerFullAddress() : "";

                model.Taxes =
                    order.Taxes.Select(
                        tax =>
                            new KeyValuePair<string, string>(
                                (tax.ShowInPrice ? LocalizationService.GetResource("Core.Tax.IncludeTax") : "") + " " + tax.Name,
                                tax.Sum.HasValue
                                    ? PriceFormatService.FormatPricePlain(tax.Sum.Value, order.OrderCurrency)
                                    : tax.Name)).ToList();
            }

            if (preparePrices)
            {
                GetPreparedPrices(model, order.OrderCurrency);

                if (model.CouponPrice != null)
                {
                    model.PreparedCouponPrice = order.OrderCurrency != null
                        ? PriceFormatService.FormatPricePlain(model.CouponPrice.Value, order.OrderCurrency)
                        : PriceFormatService.FormatPriceInvariant(model.CouponPrice.Value);
                }

                if (model.DiscountPrice != null)
                {
                    model.PreparedDiscountCost = order.OrderCurrency != null
                        ? PriceFormatService.FormatPricePlain(model.DiscountPrice.Value, order.OrderCurrency)
                        : PriceFormatService.FormatPriceInvariant(model.DiscountPrice.Value);
                }

                if (model.ProductsCost != null)
                {
                    model.PreparedProductsCost = order.OrderCurrency != null
                        ? PriceFormatService.FormatPricePlain(model.ProductsCost.Value, order.OrderCurrency)
                        : PriceFormatService.FormatPriceInvariant(model.ProductsCost.Value);
                }
            }

            return model;
        }

        public static OrderModel FromReader(IDataReader reader, bool loadItems, bool loadSource, bool loadCustomer, 
                                            bool preparePrices, bool loadReview, bool loadBillingApiLink)
        {
            var item = new OrderModel
            {
                Id = SQLDataHelper.GetInt(reader, "OrderID"),
                Number = SQLDataHelper.GetString(reader, "Number"),
                Currency = SQLDataHelper.GetString(reader, "CurrencyCode"),
                Sum = SQLDataHelper.GetFloat(reader, "Sum"),
                Date = SQLDataHelper.GetDateTime(reader, "OrderDate"),

                CustomerComment = SQLDataHelper.GetString(reader, "CustomerComment"),
                AdminComment = SQLDataHelper.GetString(reader, "AdminOrderComment"),

                PaymentName = SQLDataHelper.GetString(reader, "PaymentMethodName"),
                PaymentCost = SQLDataHelper.GetFloat(reader, "PaymentCost"),

                ShippingName = SQLDataHelper.GetString(reader, "ShippingMethodName"),
                ShippingCost = SQLDataHelper.GetFloat(reader, "ShippingCost"),
                ShippingTaxName = ((AdvantShop.Taxes.TaxType)SQLDataHelper.GetInt(reader, "ShippingTaxType")).Localize(),
                TrackNumber = SQLDataHelper.GetString(reader, "TrackNumber"),
                DeliveryDate = SQLDataHelper.GetNullableDateTime(reader, "DeliveryDate"),
                DeliveryTime = SQLDataHelper.GetString(reader, "DeliveryTime"),

                OrderDiscount = SQLDataHelper.GetFloat(reader, "OrderDiscount"),
                OrderDiscountValue = SQLDataHelper.GetFloat(reader, "OrderDiscountValue"),

                BonusCardNumber = SQLDataHelper.GetNullableLong(reader, "BonusCardNumber"),
                BonusCost = SQLDataHelper.GetFloat(reader, "BonusCost"),

                LpId = SQLDataHelper.GetNullableInt(reader, "LpId"),

                IsPaid = SQLDataHelper.GetNullableDateTime(reader, "PaymentDate").HasValue,
                PaymentDate = SQLDataHelper.GetNullableDateTime(reader, "PaymentDate"),
                
                Code = SQLDataHelper.GetGuid(reader, "Code"),
                IsDraft = SQLDataHelper.GetBoolean(reader, "IsDraft"),
                ModifiedDate = SQLDataHelper.GetDateTime(reader, "ModifiedDate"),
            };

            var statusId = SQLDataHelper.GetInt(reader, "OrderStatusID");
            var sourceId = SQLDataHelper.GetInt(reader, "OrderSourceId");

            if (loadCustomer)
            {
                item.Customer = new OrderCustomerModel
                {
                    CustomerId = SQLDataHelper.GetGuid(reader, "CustomerID"),
                    FirstName = SQLDataHelper.GetString(reader, "FirstName"),
                    LastName = SQLDataHelper.GetString(reader, "LastName"),
                    Patronymic = SQLDataHelper.GetString(reader, "Patronymic"),
                    Organization = SQLDataHelper.GetString(reader, "Organization"),
                    Email = SQLDataHelper.GetString(reader, "Email"),
                    Phone = SQLDataHelper.GetString(reader, "Phone"),
                    Country = SQLDataHelper.GetString(reader, "Country"),
                    Region = SQLDataHelper.GetString(reader, "Region"),
                    District = SQLDataHelper.GetString(reader, "District"),
                    City = SQLDataHelper.GetString(reader, "City"),
                    Zip = SQLDataHelper.GetString(reader, "Zip"),
                    CustomField1 = SQLDataHelper.GetString(reader, "CustomField1"),
                    CustomField2 = SQLDataHelper.GetString(reader, "CustomField2"),
                    CustomField3 = SQLDataHelper.GetString(reader, "CustomField3"),
                    Street = SQLDataHelper.GetString(reader, "Street"),
                    House = SQLDataHelper.GetString(reader, "House"),
                    Apartment = SQLDataHelper.GetString(reader, "Apartment"),
                    Structure = SQLDataHelper.GetString(reader, "Structure"),
                    Entrance = SQLDataHelper.GetString(reader, "Entrance"),
                    Floor = SQLDataHelper.GetString(reader, "Floor"),
                };
            }

            item.Status = statusId > 0
                ? new OrderStatusModel
                    {
                        Id = statusId,
                        Name = SQLDataHelper.GetString(reader, "StatusName"),
                        Color = SQLDataHelper.GetString(reader, "StatusColor"),
                        IsCanceled = SQLDataHelper.GetBoolean(reader, "StatusIsCanceled"),
                        IsCompleted = SQLDataHelper.GetBoolean(reader, "StatusIsCompleted"),
                        Hidden = SQLDataHelper.GetBoolean(reader, "StatusHidden"),
                        IsCancellationForbidden = SQLDataHelper.GetBoolean(reader, "StatusIsCancellationForbidden"),
                    }
                : null;

            item.Source = loadSource && sourceId > 0
                ? new OrderSourceModel
                    {
                        Id = sourceId,
                        Name = SQLDataHelper.GetString(reader, "SourceName"),
                        Main = SQLDataHelper.GetBoolean(reader, "SourceMain"),
                        Type = SQLDataHelper.GetString(reader, "SourceType").TryParseEnum<Orders.OrderType>().ToString(),
                    }
                : null;

            item.Items = loadItems
                ? OrderService.GetOrderItems(item.Id).Select(OrderItemModel.FromOrderItem).ToList()
                : null;
            
            if (preparePrices)
            {
                var orderCurrency =
                    !string.IsNullOrEmpty(SQLDataHelper.GetString(reader, "CurrencyCode"))
                        ? new OrderCurrency
                        {
                            CurrencyCode = SQLDataHelper.GetString(reader, "CurrencyCode"),
                            CurrencyValue = SQLDataHelper.GetFloat(reader, "CurrencyValue"),
                            CurrencySymbol = SQLDataHelper.GetString(reader, "CurrencySymbol"),
                            IsCodeBefore = SQLDataHelper.GetBoolean(reader, "IsCodeBefore"),
                            EnablePriceRounding = SQLDataHelper.GetBoolean(reader, "EnablePriceRounding"),
                            RoundNumbers = SQLDataHelper.GetFloat(reader, "RoundNumbers"),
                        }
                        : null;

                GetPreparedPrices(item, orderCurrency);
            }

            if (loadReview)
            {
                var review = OrderService.GetOrderReview(item.Id);
                item.Review = review != null ? new OrderReviewModel(review) : null;
            }

            if (loadBillingApiLink)
            {
                var order = (Order) item;
            
                item.BillingApiLink = 
                    order.ShowBillingLink() 
                        ? UrlService.GetUrl($"checkout/apiauth?type=billing&code={order.Code}&hash={OrderService.GetBillingLinkHash(order)}") 
                        : null;
            }

            return item;
        }

        private static void GetPreparedPrices(OrderModel item, OrderCurrency orderCurrency)
        {
            item.PreparedSum = orderCurrency != null
                ? PriceFormatService.FormatPricePlain(item.Sum, orderCurrency)
                : PriceFormatService.FormatPriceInvariant(item.Sum);
                
            item.PreparedBonusCost = orderCurrency != null
                ? PriceFormatService.FormatPricePlain(item.BonusCost, orderCurrency)
                : PriceFormatService.FormatPriceInvariant(item.BonusCost);
                
            item.PreparedPaymentCost = orderCurrency != null
                ? PriceFormatService.FormatPricePlain(item.PaymentCost, orderCurrency)
                : PriceFormatService.FormatPriceInvariant(item.PaymentCost);
                
            item.PreparedShippingCost = orderCurrency != null
                ? PriceFormatService.FormatPricePlain(item.ShippingCost, orderCurrency)
                : PriceFormatService.FormatPriceInvariant(item.ShippingCost);
            
            if (item.Items != null && item.Items.Count > 0)
            {
                foreach (var orderItem in item.Items)
                {
                    orderItem.PreparedPrice = orderCurrency != null
                        ? PriceFormatService.FormatPricePlain(orderItem.Price, orderCurrency)
                        : PriceFormatService.FormatPriceInvariant(orderItem.Price);
                }
            }
        }
        
        public static explicit operator Order(OrderModel item)
        {
            return new Order()
            {
                OrderID = item.Id,
                Number = item.Number,
                Code = item.Code,
                Sum = item.Sum,
                OrderDate = item.Date,
                IsDraft = item.IsDraft,
                
                CustomerComment = item.CustomerComment,
                AdminOrderComment = item.AdminComment,
                
                PaymentDate = item.PaymentDate,
                PaymentCost = item.PaymentCost,
                ArchivedShippingName = item.ShippingName,
                ShippingCost = item.ShippingCost,
                TrackNumber = item.TrackNumber,
                DeliveryDate = item.DeliveryDate,
                DeliveryTime = item.DeliveryTime,

                OrderDiscount = item.OrderDiscount,
                OrderDiscountValue = item.OrderDiscountValue,

                BonusCardNumber = item.BonusCardNumber,
                BonusCost = item.BonusCost,

                LpId = item.LpId,

                OrderCustomer = item.Customer != null ? (OrderCustomer)item.Customer : null,
                OrderStatus = item.Status != null ? (OrderStatus)item.Status : null
            };
        } 
    }
}
