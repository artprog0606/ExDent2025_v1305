using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Bonuses;
using AdvantShop.Core.Services.Bonuses.Model;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.SQL;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using AdvantShop.Repository.Currencies;

namespace AdvantShop.Core.Services.Orders
{
    public static class OrderExtensions
    {

        public static int GetOrdersCount(this OrderStatus orderStatus)
        {
            return Convert.ToInt32(SQLDataAccess.ExecuteScalar(
                "SELECT Count(*) FROM [Order].[Order] WHERE [OrderStatusID] = @OrderStatusID and IsDraft = 0",
                CommandType.Text, new SqlParameter("@OrderStatusID", orderStatus.StatusID)));
        }

        public static bool ShowBillingLink(this Order order)
        {
            var show = order != null && order.OrderCustomer != null && !order.Payed && !order.IsDraft && !(order.OrderStatus != null && order.OrderStatus.IsCanceled);
            return show;
        }

        public static float GetOrderDiscountPrice(this Order order)
        {
            var totalDiscount = order.OrderDiscount > 0
                                    ? order.OrderDiscount*order.OrderItems.Where(x => !x.IgnoreOrderDiscount).Sum(x => x.Price*x.Amount)/100
                                    : 0;

            totalDiscount += order.OrderDiscountValue;

            return totalDiscount.SimpleRoundPrice(order.OrderCurrency);
        }

        public static float GetOrderCouponPrice(this Order order)
        {
            if (order.Coupon == null || !order.Coupon.IsAppliedToOrder(order))
                return 0;
            
            var couponPrice = 0f;
            
            switch (order.Coupon.Type)
            {
                case CouponType.Fixed:
                    var productsPrice = order.OrderItems.Where(x => x.IsCouponApplied).Sum(x => x.Price * x.Amount);
                    couponPrice = productsPrice >= order.Coupon.Value ? order.Coupon.Value : productsPrice;
                    break;
                case CouponType.Percent:
                    couponPrice = order.OrderItems.Where(x => x.IsCouponApplied).Sum(x => order.Coupon.Value*x.Price/100*x.Amount);
                    break;
            }

            return couponPrice.SimpleRoundPrice(order.OrderCurrency);
        }

        public static bool CanAccessToPaymentReceipt(this Order order, string codeHash, Customer customer)
        {
            if (order.OrderCustomer != null && order.OrderCustomer.CustomerID == customer.Id)
                return true;
            
            if (customer.IsAdmin ||
                (customer.CustomerRole == Role.Moderator && customer.HasRoleAction(RoleAction.Orders)))
                return true;

            if (!string.IsNullOrEmpty(codeHash))
            {
                var orderCodeHash = order.Code.ToString().Md5(false);
                if (orderCodeHash.Equals(codeHash))
                    return true;
            }
            
            if (order.OrderSourceId != 0
                && order.OrderSourceId == OrderSourceService.GetOrderSource(OrderType.MobileApp).Id
                && DateTime.Now.AddDays(-1) < order.OrderDate)
                return true;

            return false;
        }

        /// <summary>
        /// Street + House + Apartment + Structure + Entrance + Floor
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public static string GetCustomerAddress(this OrderCustomer customer)
        {
            if (customer == null)
                return "";
            
            var result = customer.Street ?? "";

            if (!string.IsNullOrEmpty(customer.House))
                result += " " + LocalizationService.GetResource("Core.Orders.OrderContact.House") + " " + customer.House;

            if (!string.IsNullOrEmpty(customer.Structure))
                result += ", " + LocalizationService.GetResource("Core.Orders.OrderContact.Structure") + " " + customer.Structure;

            if (!string.IsNullOrEmpty(customer.Apartment))
                result += ", " + LocalizationService.GetResource("Core.Orders.OrderContact.Apartment") + " " + customer.Apartment;

            if (!string.IsNullOrEmpty(customer.Entrance))
                result += ", " + LocalizationService.GetResource("Core.Orders.OrderContact.Entrance") + " " + customer.Entrance;

            if (!string.IsNullOrEmpty(customer.Floor))
                result += ", " + LocalizationService.GetResource("Core.Orders.OrderContact.Floor") + " " + customer.Floor;


            return result;
        }

        /// <summary>
        /// Full address: country, region, district, city, zip, street, house, apartment, structure, entrance, floor
        /// </summary>
        public static string GetCustomerFullAddress(this OrderCustomer customer)
        {
            if (customer == null)
                return "";
            
            return StringHelper.AggregateStrings(", ",
                customer.Zip,
                customer.Country,
                customer.Region,
                customer.District,
                customer.City,
                GetCustomerAddress(customer));
        }

        public static string GetCustomerAddress(this CustomerContact customer)
        {
            if (customer == null)
                return "";

            var result = customer.Street ?? "";

            if (!string.IsNullOrEmpty(customer.House))
                result += " " + LocalizationService.GetResource("Core.Orders.OrderContact.House") + " " + customer.House;

            if (!string.IsNullOrEmpty(customer.Apartment))
                result += ", " + LocalizationService.GetResource("Core.Orders.OrderContact.Apartment") + " " + customer.Apartment;

            if (!string.IsNullOrEmpty(customer.Structure))
                result += ", " + LocalizationService.GetResource("Core.Orders.OrderContact.Structure") + " " + customer.Structure;

            if (!string.IsNullOrEmpty(customer.Entrance))
                result += ", " + LocalizationService.GetResource("Core.Orders.OrderContact.Entrance") + " " + customer.Entrance;

            if (!string.IsNullOrEmpty(customer.Floor))
                result += ", " + LocalizationService.GetResource("Core.Orders.OrderContact.Floor") + " " + customer.Floor;


            return result;
        }

        /// <summary>
        /// Street + House + Apartment
        /// </summary>
        public static string GetCustomerShortAddress(this OrderCustomer customer)
        {
            if (customer == null)
                return "";
            
            var result = customer.Street ?? "";

            if (!string.IsNullOrEmpty(customer.House))
                result += " " + LocalizationService.GetResource("Core.Orders.OrderContact.House") + customer.House;

            if (!string.IsNullOrEmpty(customer.Apartment))
                result += ", " + LocalizationService.GetResource("Core.Orders.OrderContact.Apartment") + customer.Apartment;
            
            return result;
        }

        public static string GetCustomerAddress(this CheckoutAddress address)
        {
            if (address == null)
                return "";
            
            var result = address.Street ?? "";

            if (!string.IsNullOrEmpty(address.House))
                result += " " + LocalizationService.GetResource("Core.Orders.OrderContact.House") + " " + address.House;

            if (!string.IsNullOrEmpty(address.Apartment))
                result += ", " + LocalizationService.GetResource("Core.Orders.OrderContact.Apartment") + " " + address.Apartment;

            if (!string.IsNullOrEmpty(address.Structure))
                result += ", " + LocalizationService.GetResource("Core.Orders.OrderContact.Structure") + " " + address.Structure;

            if (!string.IsNullOrEmpty(address.Entrance))
                result += ", " + LocalizationService.GetResource("Core.Orders.OrderContact.Entrance") + " " + address.Entrance;

            if (!string.IsNullOrEmpty(address.Floor))
                result += ", " + LocalizationService.GetResource("Core.Orders.OrderContact.Floor") + " " + address.Floor;


            return result;
        }

        public static Card GetOrderBonusCard(this Order order)
        {
            Card bonusCard = null;
            if (order.BonusCardNumber != null)
                bonusCard = BonusSystemService.GetCard(order.BonusCardNumber);

            if (bonusCard == null && order.OrderCustomer != null)
            {
                var customer = CustomerService.GetCustomer(order.OrderCustomer.CustomerID);
                if (customer != null)
                    bonusCard = BonusSystemService.GetCard(customer.Id);
            }

            return bonusCard;
        }

        public static IList<OrderItem> GetOrderItemsForFiscal(this Order order, bool toIntegerAmount = false) 
            => GetOrderItemsForFiscal(order, order.OrderCurrency, toIntegerAmount);

        public static IList<OrderItem> GetOrderItemsForFiscal(this Order order, Currency currency, bool toIntegerAmount = false)
        {
            var orderItemPriceAdjuster = order.GetOrderItemsWithDiscountsAndFee()
                                              .AcceptableDifference(0f)
                                              .NoChangeAmount()
                                              .WithCurrency(currency);
            if (toIntegerAmount)
                orderItemPriceAdjuster.CeilingAmountToInteger();

            var items = orderItemPriceAdjuster.GetItems(out var difference);
            if (difference == 0f)
                return items;
            
            // повторяем без NoChangeAmount
            orderItemPriceAdjuster = order.GetOrderItemsWithDiscountsAndFee()
                                              .AcceptableDifference(0f)
                                              .WithCurrency(currency);
            if (toIntegerAmount)
                orderItemPriceAdjuster.CeilingAmountToInteger();
            
            items = orderItemPriceAdjuster.GetItems();

            return items;
        }

        public static IOrderItemPriceAdjuster GetOrderItemsWithDiscountsAndFee(this Order order)
        {
            return new OrderItemPriceAdjuster(order);
        }

        public static CustomerGroup GetCustomerGroup(this Order order)
        {
            var group = CustomerGroupService.GetCustomerGroupList().FirstOrDefault(x =>
                x.GroupName.Equals(order.GroupName ?? "", StringComparison.OrdinalIgnoreCase) &&
                x.GroupDiscount == order.GroupDiscount);

            return group ?? new CustomerGroup {GroupName = order.GroupName, GroupDiscount = order.GroupDiscount};
        }
    }
}
