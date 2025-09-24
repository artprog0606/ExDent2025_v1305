using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Core.Common.Attributes.TemplateDocx;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Customers;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Localization;
using AdvantShop.Orders;
using AdvantShop.Repository;
using AdvantShop.Repository.Currencies;
using AdvantShop.Taxes;

namespace AdvantShop.Core.Services.TemplatesDocx.Templates
{
    public class OrderTemplate : BaseTemplate
    {
        public OrderTemplate()
        {
            OrderItems = new List<OrderItemTemplate>();
            OrderItemsIncludingAllDiscounts = new List<OrderItemTemplate>();
            Taxes = new List<OrderTaxTemplate>();
        }

        //[TemplateDocxProperty("Id", LocalizeDescription = "Номер заказа")]
        public int OrderId { get; set; }

        [TemplateDocxProperty("Number", LocalizeDescription = "Номер заказа")]
        public string Number { get; set; }

        [TemplateDocxProperty("StatusName", LocalizeDescription = "Статус")]
        public string StatusName { get; set; }

        [TemplateDocxProperty("StatusComment", LocalizeDescription = "Core.Orders.Order.StatusComment")]
        public string StatusComment { get; set; }

        [TemplateDocxProperty("OrderDate", LocalizeDescription = "Core.Orders.Order.OrderDate")]
        public DateTime OrderDate { get; set; }

        [TemplateDocxProperty("OrderDateFormatted", LocalizeDescription = "Core.Orders.Order.OrderDate")]
        public string OrderDateFormatted
        {
            get { return Culture.ConvertDate(OrderDate); }
        }

        [TemplateDocxProperty("OrderDateShortFormatted", LocalizeDescription = "Core.Orders.Order.OrderShortDate")]
        public string OrderDateShortFormatted
        {
            get { return Culture.ConvertDateWithoutHours(OrderDate); }
        }

        [TemplateDocxProperty("OrderSource", LocalizeDescription = "Core.Orders.Order.OrderSourceId")]
        public string OrderSourceName { get; set; }

        [TemplateDocxProperty("IsDraft", LocalizeDescription = "Черновик")]
        public bool IsDraft { get; set; }

        [TemplateDocxProperty("Payed", LocalizeDescription = "Оплачен")]
        public bool Payed { get; set; }

        [TemplateDocxProperty("PaymentDate", LocalizeDescription = "Core.Booking.Booking.PaymentDate")]
        public DateTime? PaymentDate { get; set; }

        [TemplateDocxProperty("PaymentDateFormatted", LocalizeDescription = "Core.Booking.Booking.PaymentDate")]
        public string PaymentDateFormatted
        {
            get { return PaymentDate.HasValue ? Culture.ConvertDate(PaymentDate.Value) : string.Empty; }
        }

        [TemplateDocxProperty("PaymentMethodName", LocalizeDescription = "Core.Orders.Order.PaymentName")]
        public string PaymentMethodName { get; set; }

        [TemplateDocxProperty("PaymentDetails", Type = TypeItem.InheritedFields)]
        public PaymentDetailsTemplate PaymentDetails { get; set; }

        [TemplateDocxProperty("Customer", Type = TypeItem.InheritedFields)]
        public CustomerTemplate Customer { get; set; }

        [TemplateDocxProperty("OrderRecipient", Type = TypeItem.InheritedFields)]
        public OrderRecipientTemplate OrderRecipient { get; set; }

        [TemplateDocxProperty("CustomerComment", LocalizeDescription = "Core.Orders.Order.CustomerComment")]
        public string CustomerComment { get; set; }

        [TemplateDocxProperty("Manager", Type = TypeItem.InheritedFields)]
        public ManagerTemplate Manager { get; set; }

        [TemplateDocxProperty("ShippingMethodName", LocalizeDescription = "Core.Orders.Order.ShippingName")]
        public string ShippingMethodName { get; set; }

        [TemplateDocxProperty("PickPointAddress", LocalizeDescription = "Адрес точки самовывоза")]
        public string PickPointAddress { get; set; }

        [TemplateDocxProperty("DeliveryDate", LocalizeDescription = "Дата доставки")]
        public DateTime? DeliveryDate { get; set; }

        [TemplateDocxProperty("DeliveryDateFormatted", LocalizeDescription = "Дата доставки")]
        public string DeliveryDateFormatted
        {
            get { return DeliveryDate.HasValue ? Culture.ConvertDate(DeliveryDate.Value) : string.Empty; }
        }

        [TemplateDocxProperty("DeliveryDateShortFormatted", LocalizeDescription = "Дата доставки (дд.мм.гггг)")]
        public string DeliveryDateShortFormatted
        {
            get { return DeliveryDate.HasValue ? Culture.ConvertDateWithoutHours(DeliveryDate.Value) : string.Empty; }
        }

        [TemplateDocxProperty("DeliveryTime", LocalizeDescription = "Время доставки")]
        public string DeliveryTime { get; set; }

        [TemplateDocxProperty("TrackNumber", LocalizeDescription = "Core.Orders.Order.TrackNumber")]
        public string TrackNumber { get; set; }

        [TemplateDocxProperty("Table Items", Type = TypeItem.Table, LocalizeDescription = "Содержание заказа для табличного представления")]
        [TemplateDocxProperty("List Items", Type = TypeItem.List, LocalizeDescription = "Содержание заказа для представления нумерованного или маркированного списка")]
        [TemplateDocxProperty("Repeat Items", Type = TypeItem.Repeat, LocalizeDescription = "Содержание заказа для представления списком")]
        public List<OrderItemTemplate> OrderItems { get; set; }

        [TemplateDocxProperty("Table Items Finance", Type = TypeItem.Table, LocalizeDescription = "Содержание заказа с учтенными скидками для табличного представления")]
        [TemplateDocxProperty("List Items Finance", Type = TypeItem.List, LocalizeDescription = "Содержание заказа с учтенными скидками для представления нумерованного или маркированного списка")]
        [TemplateDocxProperty("Repeat Items Finance", Type = TypeItem.Repeat, LocalizeDescription = "Содержание заказа с учтенными скидками для представления списком")]
        public List<OrderItemTemplate> OrderItemsIncludingAllDiscounts { get; set; }

        [TemplateDocxProperty("Table Taxes", Type = TypeItem.Table, LocalizeDescription = "Налоги для табличного представления")]
        [TemplateDocxProperty("List Taxes", Type = TypeItem.List, LocalizeDescription = "Налоги для представления нумерованного или маркированного списка")]
        [TemplateDocxProperty("Repeat Taxes", Type = TypeItem.Repeat, LocalizeDescription = "Налоги для представления списком")]
        public List<OrderTaxTemplate> Taxes { get; set; }

        [TemplateDocxProperty("Currency", Type = TypeItem.InheritedFields)]
        public CurrencyTemplate CurrencyTemplate { get; set; }

        public OrderCurrency Currency { get; set; }

        [TemplateDocxProperty("ShippingCost", LocalizeDescription = "Core.Orders.Order.ShippingCost")]
        public float ShippingCost { get; set; }

        [TemplateDocxProperty("ShippingCostFormatted", LocalizeDescription = "Core.Orders.Order.ShippingCost")]
        public string ShippingCostFormatted { get { return ShippingCost.RoundAndFormatPrice(Currency); } }

        [TemplateDocxProperty("ShippingCostInWords", LocalizeDescription = "Стоимость доставки прописью")]
        public string ShippingCostInWords { get { return NumberToString.ConvertToString((decimal)ShippingCost, false); } }

        [TemplateDocxProperty("ShippingTaxName", LocalizeDescription = "Core.Orders.Order.ShippingTaxName")]
        public string ShippingTaxName { get; set; }
        
        [TemplateDocxProperty("ShippingTaxRate", LocalizeDescription = "Core.Orders.Order.ShippingTaxRate")]
        public float? ShippingTaxRate { get; set; }
        
        [TemplateDocxProperty("ShippingTaxSum", LocalizeDescription = "Core.Orders.Order.ShippingTaxSum")]
        public float? ShippingTaxSum { get; set; }

        [TemplateDocxProperty("ShippingTaxSumFormatted", LocalizeDescription = "Core.Orders.Order.ShippingTaxSumFormatted")]
        public string ShippingTaxSumFormatted => ShippingTaxSum.HasValue
            ? ShippingTaxSum.Value.RoundAndFormatPrice(Currency)
            : ShippingTaxName;
        
        [TemplateDocxProperty("PaymentCost", LocalizeDescription = "Core.Orders.Order.PaymentCost")]
        public float PaymentCost { get; set; }

        [TemplateDocxProperty("PaymentCostFormatted", LocalizeDescription = "Core.Orders.Order.PaymentCost")]
        public string PaymentCostFormatted { get { return PaymentCost.RoundAndFormatPrice(Currency); } }

        [TemplateDocxProperty("PaymentCostInWords", LocalizeDescription = "Стоимость оплаты прописью")]
        public string PaymentCostInWords { get { return NumberToString.ConvertToString((decimal)PaymentCost, false); } }

        [TemplateDocxProperty("BonusCost", LocalizeDescription = "Core.Orders.Order.BonusCost")]
        public float BonusCost { get; set; }

        [TemplateDocxProperty("BonusCardNumber", LocalizeDescription = "Core.Orders.Order.BonusCardNumber")]
        public long? BonusCardNumber { get; set; }

        [TemplateDocxProperty("Discount", LocalizeDescription = "Итоговая скидка")]
        public float DiscountCost { get; set; }

        [TemplateDocxProperty("TaxSum", LocalizeDescription = "Сумма налогов")]
        public float? TaxSum { get { return Taxes.Sum(x => x.Sum); } }

        [TemplateDocxProperty("TaxSumFormatted", LocalizeDescription = "Сумма налогов")]
        public string TaxSumFormatted { get { return TaxSum.HasValue ? TaxSum.Value.RoundAndFormatPrice(Currency) : "-"; } }

        [TemplateDocxProperty("TaxSumInWords", LocalizeDescription = "Сумма налогов прописью")]
        public string TaxSumInWords { get { return NumberToString.ConvertToString((decimal)TaxSum, false); } }

        [TemplateDocxProperty("Sum", LocalizeDescription = "Сумма")]
        public float Sum { get; set; }

        [TemplateDocxProperty("SumFormatted", LocalizeDescription = "Сумма")]
        public string SumFormatted { get { return Sum.RoundAndFormatPrice(Currency); } }

        [TemplateDocxProperty("Sum in words", LocalizeDescription = "Сумма прописью")]
        public string SumInWords { get { return NumberToString.ConvertToString((decimal)Sum, false); } }

        [TemplateDocxProperty("SumWithoutTax", LocalizeDescription = "Сумма без налогов")]
        public float SumWithoutTax { get { return Sum - (TaxSum ?? 0f); } }

        [TemplateDocxProperty("SumWithoutTaxFormatted", LocalizeDescription = "Сумма без налогов")]
        public string SumWithoutTaxFormatted { get { return SumWithoutTax.RoundAndFormatPrice(Currency); } }

        [TemplateDocxProperty("SumWithoutTaxInWords", LocalizeDescription = "Сумма без налогов прописью")]
        public string SumWithoutTaxInWords { get { return NumberToString.ConvertToString((decimal)SumWithoutTax, false); } }

        [TemplateDocxProperty("SumWithoutShipping", LocalizeDescription = "Сумма без доставки")]
        public float SumWithoutShipping { get { return Sum - ShippingCost; } }

        [TemplateDocxProperty("SumWithoutShippingFormatted", LocalizeDescription = "Сумма без доставки")]
        public string SumWithoutShippingFormatted { get { return SumWithoutShipping.RoundAndFormatPrice(Currency); } }

        [TemplateDocxProperty("SumWithoutShippingInWords", LocalizeDescription = "Сумма без доставки прописью")]
        public string SumWithoutShippingInWords { get { return NumberToString.ConvertToString((decimal)SumWithoutShipping, false); } }

        [TemplateDocxProperty("SumWithoutShippingAndTax", LocalizeDescription = "Сумма без доставки и налогов")]
        public float SumWithoutShippingAndTax { get { return Sum - (TaxSum ?? 0f) - ShippingCost; } }

        [TemplateDocxProperty("SumWithoutShippingAndTaxFormatted", LocalizeDescription = "Сумма без доставки и налогов")]
        public string SumWithoutShippingAndTaxFormatted { get { return SumWithoutShippingAndTax.RoundAndFormatPrice(Currency); } }

        [TemplateDocxProperty("SumWithoutShippingAndTaxInWords", LocalizeDescription = "Сумма без доставки и налогов прописью")]
        public string SumWithoutShippingAndTaxInWords { get { return NumberToString.ConvertToString((decimal)SumWithoutShippingAndTax, false); } }

        [TemplateDocxProperty("Coupon", LocalizeDescription = "Core.Orders.Order.Coupon")]
        public string CouponCode { get; set; }

        [TemplateDocxProperty("AdminOrderComment", LocalizeDescription = "Core.Orders.Order.AdminOrderComment")]
        public string AdminOrderComment { get; set; }

        [TemplateDocxProperty("TotalWeight", LocalizeDescription = "Core.Orders.Order.TotalWeight")]
        public float TotalWeight { get; set; }

        [TemplateDocxProperty("TotalDimensions", LocalizeDescription = "Core.Orders.Order.TotalDimensions")]
        public string TotalDimensions { get; set; }

        [TemplateDocxProperty("TotalAmount", LocalizeDescription = "Core.Orders.Order.TotalAmount")]
        public float TotalAmount { get { return OrderItems != null ? OrderItems.Sum(x => x.Amount) : 0; } }

        [TemplateDocxProperty("TotalAmountFormatted", LocalizeDescription = "Core.Orders.Order.TotalAmountFormatted")]
        public string TotalAmountFormatted { get { return NumberToString.NumberToWords((ulong)TotalAmount); } }

        [TemplateDocxProperty("ItemsCount", LocalizeDescription = "Core.Orders.Order.ItemsCount")]
        public int ItemsCount { get { return OrderItems?.Count ?? 0; } }

        [TemplateDocxProperty("ItemsCountFormatted", LocalizeDescription = "Core.Orders.Order.ItemsCountFormatted")]
        public string ItemsCountFormatted { get { return NumberToString.NumberToWords((ulong)(OrderItems != null ? OrderItems.Count : 0)); } }

        [TemplateDocxProperty("ItemsCountAndShipping", LocalizeDescription = "Core.Orders.Order.ItemsCountAndShipping")]
        public int ItemsCountAndShipping { get { return (OrderItems?.Count ?? 0) + (ShippingMethodName.IsNullOrEmpty() ? 0 : 1); } }

        [TemplateDocxProperty("ItemsCountAndShippingFormatted", LocalizeDescription = "Core.Orders.Order.ItemsCountAndShippingFormatted")]
        public string ItemsCountAndShippingFormatted { get { return NumberToString.NumberToWords((ulong)((OrderItems != null ? OrderItems.Count : 0) + (ShippingMethodName.IsNullOrEmpty() ? 0 : 1))); } }

        [TemplateDocxProperty("FinanceItemsCountAndShipping", LocalizeDescription = "Core.Orders.Order.FinanceItemsCountAndShipping")]
        public int FinanceItemsCountAndShipping { get { return (OrderItemsIncludingAllDiscounts?.Count ?? 0) + (ShippingMethodName.IsNullOrEmpty() ? 0 : 1); } }

        [TemplateDocxProperty("FinanceItemsCountAndShippingFormatted", LocalizeDescription = "Core.Orders.Order.FinanceItemsCountAndShippingFormatted")]
        public string FinanceItemsCountAndShippingFormatted { get { return NumberToString.NumberToWords((ulong)((OrderItemsIncludingAllDiscounts != null ? OrderItemsIncludingAllDiscounts.Count : 0) + (ShippingMethodName.IsNullOrEmpty() ? 0 : 1))); } }

        public static explicit operator OrderTemplate(Order order)
        {
            var template = new OrderTemplate
            {
                OrderId = order.OrderID,
                Number = order.Number,
                StatusName = order.OrderStatus != null ? order.OrderStatus.StatusName : string.Empty,
                StatusComment = order.StatusComment,
                OrderDate = order.OrderDate,
                OrderSourceName = order.OrderSource != null ? order.OrderSource.Name : string.Empty,
                IsDraft = order.IsDraft,
                Payed = order.Payed,
                PaymentDate = order.PaymentDate,
                PaymentMethodName = order.PaymentMethodName,
                PaymentDetails = order.PaymentDetails != null ? (PaymentDetailsTemplate)order.PaymentDetails : null,
                Customer = order.OrderCustomer != null ? (CustomerTemplate)(Customer) order.OrderCustomer : null,
                CustomerComment = order.CustomerComment,
                Manager = order.Manager != null ? (ManagerTemplate)order.Manager : null,
                ShippingMethodName = !string.IsNullOrEmpty(order.ArchivedShippingName) ? order.ArchivedShippingName : order.ShippingMethodName,
                PickPointAddress = order.OrderPickPoint != null ? order.OrderPickPoint.PickPointAddress : string.Empty,
                DeliveryDate = order.DeliveryDate,
                DeliveryTime = order.DeliveryInterval.ReadableString,
                TrackNumber = order.TrackNumber,
                Currency = order.OrderCurrency,
                CurrencyTemplate = (CurrencyTemplate)(Currency)order.OrderCurrency,
                ShippingCost = order.ShippingCostWithDiscount,
                PaymentCost = order.PaymentCost,
                BonusCost = order.BonusCost,
                BonusCardNumber = order.BonusCardNumber,
                DiscountCost = order.DiscountCost,
                Sum = order.Sum,
                CouponCode = order.Coupon != null ? order.Coupon.Code : string.Empty,
                AdminOrderComment = order.AdminOrderComment,
                TotalWeight = MeasureHelper.GetTotalWeight(order, order.OrderItems),
            };

            int itemIndex = 1;
            foreach (var item in order.OrderItems)
            {
                var oItem = OrderItemToTemplateItem(item, order.OrderCurrency);
                oItem.ItemIndex = itemIndex++;
                template.OrderItems.Add(oItem);
            }

            var roundNumbers =
                order.OrderCurrency.EnablePriceRounding
                    ? order.OrderCurrency.RoundNumbers
                    : (float?)null;

            var orderItemsIncludingAllDiscounts = order.GetOrderItemsWithDiscountsAndFee()
                                                       .RoundNumbers(roundNumbers)
                                                       .GetItems(out var difference);
            if (Math.Abs(difference) > 0.1f)
            {
                orderItemsIncludingAllDiscounts = order.GetOrderItemsWithDiscountsAndFee()
                                                       .RoundNumbers(roundNumbers)
                                                       .AcceptableDifference(0.1f)
                                                       .GetItems();
            }

            itemIndex = 1;
            foreach (var item in orderItemsIncludingAllDiscounts)
            {
                var oItem = OrderItemToTemplateItem(item, order.OrderCurrency);
                oItem.ItemIndex = itemIndex++;
                template.OrderItemsIncludingAllDiscounts.Add(oItem);
            }

            if (order.PaymentMethodTax != null && order.PaymentMethodTax.TaxType != TaxType.None)
            {
                foreach(var item in template.OrderItems)
                {
                    item.TaxName = order.PaymentMethodTax.Name;
                    item.TaxType = order.PaymentMethodTax.TaxType;
                    item.TaxRate = order.PaymentMethodTax.TaxType == TaxType.VatWithout ? null : (float?)order.PaymentMethodTax.Rate;
                }
                foreach (var item in template.OrderItemsIncludingAllDiscounts)
                {
                    item.TaxName = order.PaymentMethodTax.Name;
                    item.TaxType = order.PaymentMethodTax.TaxType;
                    item.TaxRate = order.PaymentMethodTax.TaxType == TaxType.VatWithout ? null : (float?)order.PaymentMethodTax.Rate;
                }
            }

            if (order.ShippingMethod != null 
                && order.ShippingMethod.TaxType != TaxType.None
                && order.ShippingMethod.TaxId.HasValue)
            {
                var tax = TaxService.GetTax(order.ShippingMethod.TaxId.Value);
                template.ShippingTaxName = tax.Name;
                template.ShippingTaxRate = tax.Rate;
                template.ShippingTaxSum = (float) Math.Round(template.ShippingCost/(100 + tax.Rate)*tax.Rate, 2);
            }

            //if (order.ShippingCost > 0)
            //    template.OrderItemsIncludingAllDiscountsAndShipping.Add(new OrderItemTemplate()
            //    {
            //        Name = "Доставка",
            //        Price = order.ShippingCost,
            //        Amount = 1,
            //        Currency = order.OrderCurrency,
            //        SupplyPrice = order.ShippingCost,
            //        TaxType = order.ShippingTaxType
            //    });

            foreach (var item in order.Taxes)
            {
                template.Taxes.Add(new OrderTaxTemplate()
                {
                    TaxName = item.Name,
                    TaxRate = item.Rate,
                    Sum = item.Sum,
                    Currency = order.OrderCurrency,
                });
            }

            var dimensions = MeasureHelper.GetDimensions(order);
            template.TotalDimensions = string.Format("{0}x{1}x{2}", dimensions[0], dimensions[1], dimensions[2]);

            if (order.OrderRecipient != null && order.OrderRecipient.Exists)
                template.OrderRecipient = (OrderRecipientTemplate)order.OrderRecipient;
            else if (template.Customer != null)
                template.OrderRecipient = (OrderRecipientTemplate)(Customer)order.OrderCustomer;

            return template;
        }

        private static OrderItemTemplate OrderItemToTemplateItem(OrderItem item, OrderCurrency orderCurrency)
        {
            var template = new OrderItemTemplate()
            {
                ArtNo = item.ArtNo,
                BarCode = item.BarCode,
                Name = item.Name,
                Price = item.Price,
                Amount = item.Amount,
                Currency = orderCurrency,
                Color = item.Color,
                Size = item.Size,
                SupplyPrice = item.SupplyPrice,
                IsCouponApplied = item.IsCouponApplied,
                Weight = item.Weight,
                IgnoreOrderDiscount = item.IgnoreOrderDiscount,
                AccrueBonuses = item.AccrueBonuses,
                Length = item.Length,
                Width = item.Width,
                Height = item.Height,
                TaxName = item.TaxName,
                TaxType = item.TaxType,
                TaxRate = item.TaxRate,
                Unit = item.Unit,
                Photo =
                    item.Photo == null || string.IsNullOrEmpty(item.Photo.PhotoName)
                        ? null
                        : FoldersHelper.GetImageProductPathAbsolut(ProductImageType.Small, item.Photo.PhotoName),
                SelectedOptions = item.SelectedOptions
            };

            return template;
        }
    }

    public class OrderItemTemplate
    {

        [TemplateDocxProperty("ItemIndex", LocalizeDescription = "Порядковый номер")]
        public int ItemIndex { get; set; }

        [TemplateDocxProperty("Photo", Type = TypeItem.Image, LocalizeDescription = "Изображение товара")]
        public string Photo { get; set; }

        [TemplateDocxProperty("ArtNo", LocalizeDescription = "Core.Orders.OrderItem.ArtNo")]
        public string ArtNo { get; set; }

        [TemplateDocxProperty("Name", LocalizeDescription = "Core.Orders.OrderItem.Name")]
        public string Name { get; set; }

        public OrderCurrency Currency { get; set; }

        [TemplateDocxProperty("Price", LocalizeDescription = "Core.Orders.OrderItem.Price")]
        public float Price { get; set; }

        [TemplateDocxProperty("PriceFormatted", LocalizeDescription = "Core.Orders.OrderItem.Price")]
        public string PriceFormatted { get { return Price.RoundAndFormatPrice(Currency); } }

        [TemplateDocxProperty("Amount", LocalizeDescription = "Core.Orders.OrderItem.Amount")]
        public float Amount { get; set; }

        [TemplateDocxProperty("Sum Item", LocalizeDescription = "Сумма")]
        public float Sum { get { return PriceService.SimpleRoundPrice(Price * Amount, Currency); } }

        [TemplateDocxProperty("Sum Item Formatted", LocalizeDescription = "Сумма")]
        public string SumFormatted { get { return Sum.RoundAndFormatPrice(Currency); } }

        [TemplateDocxProperty("PriceWithoutTax", LocalizeDescription = "Цена без налога")]
        public float PriceWithoutTax { get { return TaxValue.HasValue ? Price - TaxValue.Value : Price; } }

        [TemplateDocxProperty("PriceWithoutTaxFormatted", LocalizeDescription = "Цена без налога")]
        public string PriceWithoutTaxFormatted { get { return PriceWithoutTax.RoundAndFormatPrice(Currency); } }

        [TemplateDocxProperty("SumWithoutTax", LocalizeDescription = "Сумма без налога")]
        public float SumWithoutTax { get { return TaxSum.HasValue ? Sum - TaxSum.Value : Sum;; } }

        [TemplateDocxProperty("SumWithoutTaxFormatted", LocalizeDescription = "Сумма без налога")]
        public string SumWithoutTaxFormatted { get { return SumWithoutTax.RoundAndFormatPrice(Currency); } }

        [TemplateDocxProperty("Color", LocalizeDescription = "Core.Orders.OrderItem.Color")]
        public string Color { get; set; }

        [TemplateDocxProperty("Size", LocalizeDescription = "Core.Orders.OrderItem.Size")]
        public string Size { get; set; }

        //[TemplateDocxProperty("IsCouponApplied", LocalizeDescription = "Core.Orders.OrderItem.IsCouponApplied")]
        public bool IsCouponApplied { get; set; }

        [TemplateDocxProperty("SupplyPrice", LocalizeDescription = "Закупочная цена")]
        public float SupplyPrice { get; set; }

        [TemplateDocxProperty("SupplyPriceFormatted", LocalizeDescription = "Закупочная цена")]
        public string SupplyPriceFormatted { get { return SupplyPrice.RoundAndFormatPrice(Currency); } }

        [TemplateDocxProperty("Weight", LocalizeDescription = "Core.Orders.OrderItem.Weight")]
        public float Weight { get; set; }

        //[TemplateDocxProperty("IgnoreOrderDiscount", LocalizeDescription = "Игнорируется скидка заказа")]
        public bool IgnoreOrderDiscount { get; set; }

        //[TemplateDocxProperty("AccrueBonuses", LocalizeDescription = "Начислять бонусы за покупку этого товара")]
        public bool AccrueBonuses { get; set; }

        [TemplateDocxProperty("Length", LocalizeDescription = "Длина")]
        public float Length { get; set; }

        [TemplateDocxProperty("Width", LocalizeDescription = "Ширина")]
        public float Width { get; set; }

        [TemplateDocxProperty("Height", LocalizeDescription = "Высота")]
        public float Height { get; set; }

        [TemplateDocxProperty("TaxName", LocalizeDescription = "Налог")]
        public string TaxName { get; set; }

        [TemplateDocxProperty("TaxType", LocalizeDescription = "Тип налога")]
        public TaxType? TaxType { get; set; }

        [TemplateDocxProperty("TaxRate", LocalizeDescription = "Ставка налога")]
        public float? TaxRate { get; set; }

        [TemplateDocxProperty("TaxRateFormatted", LocalizeDescription = "Ставка налога")]
        public string TaxRateFormatted
        {
            get
            {
                return TaxType == AdvantShop.Taxes.TaxType.VatWithout
                  ? TaxName
                  : TaxRate.HasValue
                      ? string.Format("{0}%", TaxRate)
                      : null;
            }
        }

        [TemplateDocxProperty("TaxValue", LocalizeDescription = "Сумма налога одной еденицы")]
        public float? TaxValue { get { return TaxRate.HasValue ? (float)Math.Round(Price / (100 + TaxRate.Value) * TaxRate.Value, 2) : (float?)null; } }

        [TemplateDocxProperty("TaxSum", LocalizeDescription = "Сумма налога")]
        public float? TaxSum { get { return TaxRate.HasValue ? (float)Math.Round(Price * Amount / (100 + TaxRate.Value) * TaxRate.Value, 2) : (float?)null; } }

        [TemplateDocxProperty("Unit", LocalizeDescription = "Единицы измерения")]
        public string Unit { get; set; }

        public List<EvaluatedCustomOptions> SelectedOptions { get; set; }

        [TemplateDocxProperty("CustomOptions", LocalizeDescription = "Доп опции")]
        public string CustomOptionsFormatted
        {
            get
            {
                if (SelectedOptions == null || SelectedOptions.Count == 0)
                    return null;

                var result = "";
                foreach (var ev in SelectedOptions)
                    result += 
                        (!string.IsNullOrEmpty(result) ? ", " : "") + 
                        (ev.CustomOptionTitle + (!string.IsNullOrEmpty(ev.OptionTitle) ? ": " + ev.OptionTitle + (ev.OptionAmount > 1 ? " (x " + ev.OptionAmount.ToString() + ")" : "") : ""));


                return result;
            }
        }

        [TemplateDocxProperty("BarCode", LocalizeDescription = "Штрихкод")]
        public string BarCode { get; set; }
    }

    public class OrderTaxTemplate
    {
        [TemplateDocxProperty("TaxName", LocalizeDescription = "Налог")]
        public string TaxName { get; set; }

        [TemplateDocxProperty("TaxRate", LocalizeDescription = "Ставка налога")]
        public float? TaxRate { get; set; }

        public OrderCurrency Currency { get; set; }
        [TemplateDocxProperty("Sum", LocalizeDescription = "Сумма налога")]
        public float? Sum { get; set; }

        [TemplateDocxProperty("SumFormatted", LocalizeDescription = "Сумма налога")]
        public string SumFormatted { get { return Sum.HasValue ? Sum.Value.RoundAndFormatPrice(Currency) : TaxName; } }
    }

    public class OrderRecipientTemplate
    {
        [TemplateDocxProperty("FirstName", LocalizeDescription = "Имя получателя")]
        public string FirstName { get; set; }

        [TemplateDocxProperty("LastName", LocalizeDescription = "Фамилия получателя")]
        public string LastName { get; set; }

        [TemplateDocxProperty("Patronymic", LocalizeDescription = "Отчество получателя")]
        public string Patronymic { get; set; }

        [TemplateDocxProperty("Phone", LocalizeDescription = "Телефон получателя")]
        public string Phone { get; set; }

        [TemplateDocxProperty("FullNameWithOrganization", LocalizeDescription = "ФИО и организация")]
        public string FullNameWithOrganization { get; set; }

        public static explicit operator OrderRecipientTemplate(OrderRecipient orderRecipient)
        {
            var orderCustomer = OrderService.GetOrderCustomer(orderRecipient.OrderId);
            return new OrderRecipientTemplate
            {
                FirstName = orderRecipient.FirstName,
                LastName = orderRecipient.LastName,
                Patronymic = orderRecipient.Patronymic,
                Phone = orderRecipient.Phone,
                FullNameWithOrganization = orderRecipient.FullName.IsNullOrEmpty() ? orderCustomer.CustomerCompanyName : $"{orderRecipient.FullName}, {orderCustomer.CustomerCompanyName}"
            };
        }


        public static explicit operator OrderRecipientTemplate(Customer customer)
        {
            var fullName = customer.GetFullName();
            return new OrderRecipientTemplate
            {
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Patronymic = customer.Patronymic,
                Phone = customer.Phone,
                FullNameWithOrganization = fullName.IsNullOrEmpty() ? customer.CustomerCompanyName : $"{fullName}, {customer.CustomerCompanyName}"
            };
        }
    }
}
