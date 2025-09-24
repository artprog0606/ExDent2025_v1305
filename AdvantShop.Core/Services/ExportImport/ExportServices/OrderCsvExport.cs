using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Loging.TrafficSource;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Localization;
using AdvantShop.Orders;
using AdvantShop.Repository;
using AdvantShop.Statistic;
using CsvHelper;

namespace AdvantShop.ExportImport
{
    public class OrderCsvExport
    {
        private Dictionary<int, OrderStatus> _ordersStatuses;

        private Dictionary<int, OrderStatus> OrdersStatuses
            => _ordersStatuses ?? (_ordersStatuses = OrderStatusService.GetOrderStatuses()
                                                                       .ToDictionary(status => status.StatusID));
        
        private Dictionary<int, OrderSource> _ordersSources;

        private Dictionary<int, OrderSource> OrdersSources 
            => _ordersSources ?? (_ordersSources = OrderSourceService.GetOrderSources()
                                                                     .ToDictionary(source => source.Id));

        private string _colorsHeader;
        private string ColorsHeader => _colorsHeader ?? (_colorsHeader = SettingsCatalog.ColorsHeader);

        private string _sizesHeader;
        private string SizesHeader => _sizesHeader ?? (_sizesHeader = SettingsCatalog.SizesHeader);

        private HashSet<string> _customOptionTitles;

        public void MultiOrder(List<Order> orders, string filename, string encoding, bool useOrderItemsInString, string customOptionOptionsSeparator)
        {
            _customOptionTitles = new HashSet<string>();
            foreach (var order in orders)
                foreach (var orderItem in order.OrderItems)
                    foreach (var customOption in orderItem.SelectedOptions)
                        _customOptionTitles.Add(customOption.CustomOptionTitle);
            
            using (var streamWriter = new StreamWriter(filename, false, Encoding.GetEncoding(encoding)))
            using (var writer = new CsvWriter(streamWriter, CsvConstants.DefaultCsvConfiguration))
            {
                // headers
                writer.WriteField(LocalizationService.GetResource("Core.ExportImport.MultiOrder.OrderID"));
                writer.WriteField(LocalizationService.GetResource("Core.ExportImport.MultiOrder.Status"));
                writer.WriteField(LocalizationService.GetResource("Core.ExportImport.MultiOrder.OrderSource"));
                writer.WriteField(LocalizationService.GetResource("Core.ExportImport.MultiOrder.OrderDate"));
                writer.WriteField(LocalizationService.GetResource("Core.ExportImport.MultiOrder.FIO"));
                writer.WriteField(LocalizationService.GetResource("Core.ExportImport.MultiOrder.CustomerEmail"));
                writer.WriteField(LocalizationService.GetResource("Core.ExportImport.MultiOrder.CustomerPhone"));
                writer.WriteField(LocalizationService.GetResource("Core.ExportImport.MultiOrder.GroupName"));
                writer.WriteField(LocalizationService.GetResource("Core.ExportImport.MultiOrder.RecipientFIO"));
                writer.WriteField(LocalizationService.GetResource("Core.ExportImport.MultiOrder.RecipientPhone"));

                if (useOrderItemsInString)
                    writer.WriteField(LocalizationService.GetResource("Core.ExportImport.MultiOrder.OrderedItems"));
                else
                {
                    writer.WriteField(LocalizationService.GetResource("Core.ExportImport.MultiOrder.ArtNo"));
                    writer.WriteField(LocalizationService.GetResource("Core.ExportImport.MultiOrder.Name"));
                    foreach (var customOption in _customOptionTitles)
                        writer.WriteField(LocalizationService.GetResource("Core.ExportImport.MultiOrder.CustomOptions") + ":" + customOption);
                    writer.WriteField(SizesHeader);
                    writer.WriteField(ColorsHeader);
                    writer.WriteField(LocalizationService.GetResource("Core.ExportImport.MultiOrder.Price"));
                    writer.WriteField(LocalizationService.GetResource("Core.ExportImport.MultiOrder.Amount"));
                }
                writer.WriteField(LocalizationService.GetResource("Core.ExportImport.MultiOrder.TotalWeight"));
                writer.WriteField(LocalizationService.GetResource("Core.ExportImport.MultiOrder.TotalDimensions"));
                writer.WriteField(LocalizationService.GetResource("Core.ExportImport.MultiOrder.Payed"));

                writer.WriteField("Скидка");
                writer.WriteField("Стоимость доставки");
                writer.WriteField("Наценка оплаты");
                writer.WriteField("Купон");
                writer.WriteField(LocalizationService.GetResource("Core.ExportImport.MultiOrder.BonusCost"));

                writer.WriteField(LocalizationService.GetResource("Core.ExportImport.MultiOrder.Total"));
                writer.WriteField(LocalizationService.GetResource("Core.ExportImport.MultiOrder.Currency"));
                writer.WriteField(LocalizationService.GetResource("Core.ExportImport.MultiOrder.Tax"));
                writer.WriteField(LocalizationService.GetResource("Core.ExportImport.MultiOrder.Cost"));
                writer.WriteField(LocalizationService.GetResource("Core.ExportImport.MultiOrder.Profit"));
                writer.WriteField(LocalizationService.GetResource("Core.ExportImport.MultiOrder.Payment"));
                writer.WriteField(LocalizationService.GetResource("Core.ExportImport.MultiOrder.Shipping"));
                writer.WriteField(LocalizationService.GetResource("Core.ExportImport.MultiOrder.ShippingAddress"));
                writer.WriteField(LocalizationService.GetResource("Core.ExportImport.MultiOrder.Country"));
                writer.WriteField(LocalizationService.GetResource("Core.ExportImport.MultiOrder.Region"));
                writer.WriteField(LocalizationService.GetResource("Core.ExportImport.MultiOrder.City"));
                writer.WriteField(LocalizationService.GetResource("Core.ExportImport.MultiOrder.Street"));
                writer.WriteField(LocalizationService.GetResource("Core.ExportImport.MultiOrder.Zip"));
                writer.WriteField(LocalizationService.GetResource("Core.ExportImport.MultiOrder.House"));
                writer.WriteField(LocalizationService.GetResource("Core.ExportImport.MultiOrder.Structure"));
                writer.WriteField(LocalizationService.GetResource("Core.ExportImport.MultiOrder.Apartment"));
                writer.WriteField(LocalizationService.GetResource("Core.ExportImport.MultiOrder.Entrance"));
                writer.WriteField(LocalizationService.GetResource("Core.ExportImport.MultiOrder.Floor"));
                writer.WriteField(LocalizationService.GetResource("Core.ExportImport.MultiOrder.DeliveryDate"));
                writer.WriteField(LocalizationService.GetResource("Core.ExportImport.MultiOrder.DeliveryTime"));
                writer.WriteField(LocalizationService.GetResource("Core.ExportImport.MultiOrder.CustomerComment"));
                writer.WriteField(LocalizationService.GetResource("Core.ExportImport.MultiOrder.AdminComment"));
                writer.WriteField(LocalizationService.GetResource("Core.ExportImport.MultiOrder.StatusComment"));
                writer.WriteField(LocalizationService.GetResource("Core.ExportImport.MultiOrder.Manager"));
                writer.WriteField(LocalizationService.GetResource("Core.ExportImport.MultiOrder.CouponCode"));
                writer.WriteField("Google client id");
                writer.WriteField("Yandex client id");
                writer.WriteField("Referral");
                writer.WriteField(LocalizationService.GetResource("Admin.Orders.Orderinfo.LoginPage"));
                writer.WriteField("UTM Source");
                writer.WriteField("UTM Medium");
                writer.WriteField("UTM Campaign");
                writer.WriteField("UTM Content");
                writer.WriteField("UTM Term");
                writer.WriteField("IP");

                writer.NextRecord();

                foreach (var order in orders)
                {
                    if (useOrderItemsInString || order.OrderItems.Count == 0)
                        WriteRow(writer, order, useOrderItemsInString, customOptionOptionsSeparator);
                    else
                        foreach (var orderItem in order.OrderItems)
                            WriteRow(writer, order, useOrderItemsInString, customOptionOptionsSeparator, orderItem);
                    CommonStatistic.RowPosition++;
                }
            }
        }

        private void WriteRow(CsvWriter writer, Order order, bool useOrderItemsInString, string customOptionOptionsSeparator, OrderItem orderItem = null)
        {
            if (!CommonStatistic.IsRun || CommonStatistic.IsBreaking)
                return;

            writer.WriteField(order.Number);
            writer.WriteField(OrdersStatuses.ContainsKey(order.OrderStatusId) ? OrdersStatuses[order.OrderStatusId]?.StatusName : LocalizationService.GetResource("Core.ExportImport.MultiOrder.NullStatus"));
            writer.WriteField(OrdersSources.ContainsKey(order.OrderSourceId) ? OrdersSources[order.OrderSourceId]?.Name : LocalizationService.GetResource("Core.ExportImport.MultiOrder.NullStatus"));
            writer.WriteField(order.OrderDate.ToString("dd.MM.yyyy HH:mm:ss"));

            if (order.OrderCustomer != null)
            {
                writer.WriteField(order.OrderCustomer.LastName + " " + order.OrderCustomer.FirstName);
                writer.WriteField(order.OrderCustomer.Email ?? string.Empty);
                writer.WriteField(order.OrderCustomer.Phone ?? string.Empty);
            }
            else
            {
                writer.WriteField(LocalizationService.GetResource("Core.ExportImport.MultiOrder.NullCustomer"));
                writer.WriteField(string.Empty);
                writer.WriteField(string.Empty);
            }

            writer.WriteField(order.GroupName ?? string.Empty);

            if (order.OrderRecipient != null)
            {
                var fullNameRecipient = order.OrderRecipient.FullName;
                writer.WriteField(fullNameRecipient ?? string.Empty);
                writer.WriteField(order.OrderRecipient.Phone ?? string.Empty);
            }
            else
            {
                writer.WriteField(string.Empty);
                writer.WriteField(string.Empty);
            }

            if (order.OrderCurrency != null)
            {
                if (useOrderItemsInString)
                    writer.WriteField(RenderOrderedItems(order.OrderItems, order.OrderCurrency) ?? string.Empty);
                else
                {
                    writer.WriteField(orderItem?.ArtNo ?? string.Empty);
                    writer.WriteField(orderItem?.Name ?? string.Empty);
                    foreach (var customOptionTitle in _customOptionTitles)
                    {
                        var options = orderItem?.SelectedOptions.Where(x => x.CustomOptionTitle == customOptionTitle);
                        if (options == null || !options.Any())
                            writer.WriteField(string.Empty);
                        else
                            writer.WriteField(
                                string.Join(customOptionOptionsSeparator,
                                options.Select(x => x.OptionTitle.IsNullOrEmpty() ? x.CustomOptionTitle : x.OptionTitle)));
                    }
                    writer.WriteField(orderItem?.Size ?? string.Empty);
                    writer.WriteField(orderItem?.Color ?? string.Empty);
                    writer.WriteField(orderItem?.Price ?? 0);
                    writer.WriteField(orderItem?.Amount ?? 0);
                }
                writer.WriteField(MeasureHelper.GetTotalWeight(order, order.OrderItems) + " кг");
                var totalDimensions = MeasureHelper.GetDimensions(order);
                writer.WriteField(totalDimensions[0] + " x " + totalDimensions[1] + " x " + totalDimensions[2] + " мм");
                writer.WriteField(LocalizationService.GetResource(order.Payed ? "Admin.Yes" : "Admin.No"));

                writer.WriteField(order.GetOrderDiscountPrice());
                writer.WriteField(PriceService.SimpleRoundPrice(order.ShippingCost, order.OrderCurrency));
                writer.WriteField(PriceService.SimpleRoundPrice(order.PaymentCost, order.OrderCurrency));
                writer.WriteField(order.GetOrderCouponPrice());
                writer.WriteField(order.BonusCost);

                writer.WriteField(PriceService.SimpleRoundPrice(order.Sum, order.OrderCurrency));
                writer.WriteField(order.OrderCurrency.CurrencySymbol);
                writer.WriteField(PriceService.SimpleRoundPrice(order.TaxCost, order.OrderCurrency));
                float totalCost = order.OrderItems.Sum(oi => oi.SupplyPrice * oi.Amount);
                writer.WriteField(PriceService.SimpleRoundPrice(totalCost, order.OrderCurrency));
                writer.WriteField(PriceService.SimpleRoundPrice(order.Sum - order.ShippingCost - order.TaxCost - totalCost, order.OrderCurrency));
                writer.WriteField(order.PaymentMethodName);
                writer.WriteField(order.ArchivedShippingName);
                writer.WriteField(order.OrderCustomer != null
                    ? new List<string>
                    {
                                order.OrderCustomer.Zip,
                                order.OrderCustomer.Country,
                                order.OrderCustomer.Region,
                                order.OrderCustomer.City,
                                order.OrderCustomer.GetCustomerAddress(),
                                order.OrderCustomer.CustomField1,
                                order.OrderCustomer.CustomField2,
                                order.OrderCustomer.CustomField3,
                                order.OrderPickPoint != null ? order.OrderPickPoint.PickPointAddress : string.Empty
                    }.Where(s => s.IsNotEmpty()).AggregateString(", ")
                    : string.Empty);
                writer.WriteField(order.OrderCustomer?.Country);
                writer.WriteField(order.OrderCustomer?.Region);
                writer.WriteField(order.OrderCustomer?.City);
                writer.WriteField(order.OrderCustomer?.Street);
                writer.WriteField(order.OrderCustomer?.Zip);
                writer.WriteField(order.OrderCustomer?.House);
                writer.WriteField(order.OrderCustomer?.Structure);
                writer.WriteField(order.OrderCustomer?.Apartment);
                writer.WriteField(order.OrderCustomer?.Entrance);
                writer.WriteField(order.OrderCustomer?.Floor);
                writer.WriteField(order.DeliveryDate.HasValue
                                    ? Culture.ConvertShortDate(order.DeliveryDate.Value)
                                    : string.Empty);
                writer.WriteField(order.DeliveryTime ?? string.Empty);
                writer.WriteField(order.CustomerComment ?? string.Empty);
                writer.WriteField(order.AdminOrderComment ?? string.Empty);
                writer.WriteField(order.StatusComment ?? string.Empty);
                writer.WriteField(order.Manager != null ? order.Manager.FullName : string.Empty);
                writer.WriteField(order.Coupon != null ? order.Coupon.Code : string.Empty);

                var orderTrafficSource = OrderTrafficSourceService.Get(order.OrderID, TrafficSourceType.Order);
                writer.WriteField(orderTrafficSource?.GoogleClientId ?? string.Empty);
                writer.WriteField(orderTrafficSource?.YandexClientId ?? string.Empty);
                writer.WriteField(orderTrafficSource?.Referrer ?? string.Empty);
                writer.WriteField(orderTrafficSource?.Url ?? string.Empty);
                writer.WriteField(orderTrafficSource?.utm_source ?? string.Empty);
                writer.WriteField(orderTrafficSource?.utm_medium ?? string.Empty);
                writer.WriteField(orderTrafficSource?.utm_campaign ?? string.Empty);
                writer.WriteField(orderTrafficSource?.utm_content ?? string.Empty);
                writer.WriteField(orderTrafficSource?.utm_term ?? string.Empty);
                writer.WriteField(orderTrafficSource?.Ip ?? string.Empty);
            }

            writer.NextRecord();
        }

        private string RenderOrderedItems(IEnumerable<OrderItem> items, OrderCurrency orderCurrency)
        {
            var res = new StringBuilder();

            foreach (var orderItem in items)
            {
                res.AppendFormat("[{0} - {1} - {2}{3} - {4}{5}{6}], ", 
                    orderItem.ArtNo, 
                    orderItem.Name, 
                    PriceService.SimpleRoundPrice(orderItem.Price * orderItem.Amount, orderCurrency), 
                    orderCurrency.CurrencySymbol, 
                    orderItem.Amount,
                    LocalizationService.GetResource("Core.ExportImport.ExcelOrder.Pieces"),
                    RenderSelectedOptions(orderItem.SelectedOptions, orderItem.Color, orderItem.Size));
            }

            return res.ToString().TrimEnd(new[] { ',', ' ' });
        }

        private string RenderSelectedOptions(IList<EvaluatedCustomOptions> evlist, string color, string size)
        {
            if (evlist == null && string.IsNullOrEmpty(color) && string.IsNullOrEmpty(size)) 
                return string.Empty;
            
            var html = string.Empty;
            
            if (!string.IsNullOrEmpty(color))
                html = ColorsHeader + ": " + color;

            if (!string.IsNullOrEmpty(size))
                html += (!string.IsNullOrEmpty(html) ? ", " : "") + SizesHeader + ": " + size;

            if (evlist != null)
            {
                foreach (EvaluatedCustomOptions ev in evlist)
                    html += (!string.IsNullOrEmpty(html) ? ", " : "") + $"{ev.CustomOptionTitle}: {ev.OptionTitle},";
            }

            if (!string.IsNullOrEmpty(html))
                html = " (" + html + ")";

            return html;
        }
    }
}