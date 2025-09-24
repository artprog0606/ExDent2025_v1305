using System.IO;
using System.Linq;
using System.Text;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Customers;
using AdvantShop.Localization;
using AdvantShop.Orders;
using AdvantShop.Payment;
using OfficeOpenXml;

namespace AdvantShop.ExportImport
{
    public class OrderExcelExport
    {
        public const string TemplateSingleOrder = "~/App_Data/Reports/exportSingleOrder.xlsx";

        private void RenderOrderItems(ExcelWorksheet worksheet, Order order)
        {
            var i = 1;
            foreach (OrderItem item in order.OrderItems)
            {
                //copy style
                if (i != 1)
                {
                    worksheet.InsertRow(22, 1, 21); //shift down
                                                    //worksheet.Row(19 + i).StyleID = worksheet.Row(20).StyleID;
                }

                var currentRow = i != 1 ? 22 : 21;
                worksheet.Cells[currentRow, 1].Value = item.ArtNo;
                worksheet.Cells[currentRow, 2].Value = item.Name;

                var html = new StringBuilder();
                if (item.Color.IsNotEmpty())
                    html.AppendFormat("{0}: {1}\n", SettingsCatalog.ColorsHeader, item.Color);
                if (item.Size.IsNotEmpty())
                    html.AppendFormat("{0}: {1}\n", SettingsCatalog.SizesHeader, item.Size);
                foreach (EvaluatedCustomOptions ev in item.SelectedOptions)
                    html.AppendFormat("- {0}: {1}\n", ev.CustomOptionTitle, ev.OptionTitle);
                worksheet.Cells[currentRow, 3].Value = html.ToString();

                worksheet.Cells[currentRow, 4].Value = (decimal)PriceService.SimpleRoundPrice(item.Price);
                worksheet.Cells[currentRow, 5].Value = order.OrderCurrency.CurrencySymbol.Trim();
                worksheet.Cells[currentRow, 6].Value = (decimal)item.Amount;
                worksheet.Cells[currentRow, 7].Value = (decimal)PriceService.SimpleRoundPrice(item.Price * item.Amount, order.OrderCurrency);
                worksheet.Cells[currentRow, 8].Value = order.OrderCurrency.CurrencySymbol.Trim();

                i++;
            }
        }

        public void SingleOrder(string templatePath, string filename, Order order)
        {
            using (var excel = new ExcelPackage(new System.IO.FileInfo(templatePath)))
            {
                var worksheet = excel.Workbook.Worksheets.First();

                worksheet.Name = string.Format("{0} {1}", LocalizationService.GetResource("Core.ExportImport.ExcelSingleOrder.ItemNum"), order.Number);
                //title
                worksheet.Cells[1, 1].Value = string.Format("{0} {1}", LocalizationService.GetResource("Core.ExportImport.ExcelSingleOrder.ItemNum"), order.Number);

                //status
                worksheet.Cells[2, 1].Value = "(" + order.OrderStatus.StatusName + ")";

                // Date
                worksheet.Cells[4, 1].Value = LocalizationService.GetResource("Core.ExportImport.ExcelSingleOrder.Date");
                worksheet.Cells[4, 3].Value = Culture.ConvertDate(order.OrderDate);

                //StatusComment
                worksheet.Cells[5, 1].Value = LocalizationService.GetResource("Admin.Orders.OrderItem.StatusComment");
                worksheet.Cells[5, 3].Value = order.StatusComment;

                //Email
                worksheet.Cells[6, 1].Value = LocalizationService.GetResource("Admin.Orders.OrderItem.Email");
                worksheet.Cells[6, 3].Value = order.OrderCustomer.Email;

                //Phone
                worksheet.Cells[7, 1].Value = LocalizationService.GetResource("Admin.Orders.OrderItem.Phone");
                worksheet.Cells[7, 3].Value = order.OrderCustomer.Phone;

                worksheet.Cells[9, 1].Value = LocalizationService.GetResource("Admin.Orders.OrderItem.Billing");
                worksheet.Cells[9, 3].Value = LocalizationService.GetResource("Admin.Orders.OrderItem.Shipping");
                //worksheet.Cells[9, 3].Value = LocalizationService.GetResource("Admin.Orders.OrderItem.ShippingMethod");

                var customerName = order.OrderCustomer.GetFullName();

                worksheet.Cells[10, 1].Value = string.Format("     {0} {1}", LocalizationService.GetResource("Admin.Orders.OrderItem.ContactName"), customerName);
                worksheet.Cells[10, 3].Value = string.Format("     {0} {1}", LocalizationService.GetResource("Admin.Orders.OrderItem.ContactName"), customerName);

                var shippingMethodName = order.ArchivedShippingName;
                if (order.OrderPickPoint != null)
                    shippingMethodName += order.OrderPickPoint.PickPointAddress.Replace("<br/>", " ");
                //worksheet.Cells[10, 3].Value = "     " + shippingMethodName;


                worksheet.Cells[11, 1].Value = string.Format("     {0} {1}", LocalizationService.GetResource("Admin.Orders.OrderItem.Country"), order.OrderCustomer.Country);
                worksheet.Cells[11, 3].Value = string.Format("     {0} {1}", LocalizationService.GetResource("Admin.Orders.OrderItem.Country"), order.OrderCustomer.Country);
                //worksheet.Cells[11, 3].Value = LocalizationService.GetResource("Admin.Orders.OrderItem.PaymentType");

                worksheet.Cells[12, 1].Value = string.Format("     {0} {1}", LocalizationService.GetResource("Admin.Orders.OrderItem.City"), order.OrderCustomer.City);
                worksheet.Cells[12, 3].Value = string.Format("     {0} {1}", LocalizationService.GetResource("Admin.Orders.OrderItem.City"), order.OrderCustomer.City);
                //worksheet.Cells[12, 3].Value = "     " + order.PaymentMethodName;

                worksheet.Cells[13, 1].Value = string.Format("     {0} {1}", LocalizationService.GetResource("Admin.Orders.OrderItem.Zone"), order.OrderCustomer.Region);
                worksheet.Cells[13, 3].Value = string.Format("     {0} {1}", LocalizationService.GetResource("Admin.Orders.OrderItem.Zone"), order.OrderCustomer.Region);


                worksheet.Cells[14, 1].Value = string.Format("     {0} {1}", LocalizationService.GetResource("Admin.Orders.OrderItem.Zip"), order.OrderCustomer.Zip);
                worksheet.Cells[14, 3].Value = string.Format("     {0} {1}", LocalizationService.GetResource("Admin.Orders.OrderItem.Zip"), order.OrderCustomer.Zip);

                worksheet.Cells[15, 1].Value = string.Format("     {0} {1}", LocalizationService.GetResource("Admin.Orders.OrderItem.Address"), order.OrderCustomer.GetCustomerAddress());
                worksheet.Cells[15, 3].Value = string.Format("     {0} {1}", LocalizationService.GetResource("Admin.Orders.OrderItem.Address"), order.OrderCustomer.GetCustomerAddress());

                worksheet.Cells[16, 1].Value = string.Format("     {0} {1}", LocalizationService.GetResource("Admin.Orders.OrderItem.Organization"), order.OrderCustomer.Organization);
                worksheet.Cells[16, 3].Value = string.Format("     {0} {1}", LocalizationService.GetResource("Admin.Orders.OrderItem.Organization"), order.OrderCustomer.Organization);

                if (order.PaymentDetails != null)
                {
                    worksheet.Cells[9, 4].Value = LocalizationService.GetResource("Admin.Orders.OrderItem.Payer");
                    worksheet.Cells[10, 4].Value = string.Format("     {0} {1}", LocalizationService.GetResource("Admin.Orders.OrderItem.Company"), order.PaymentDetails.CompanyName);
                    worksheet.Cells[11, 4].Value = string.Format("     {0} {1}", LocalizationService.GetResource("Admin.Orders.OrderItem.Inn"), order.PaymentDetails.INN);
                    worksheet.Cells[12, 4].Value = string.Format("     {0} {1}", LocalizationService.GetResource("Admin.Orders.OrderItem.Kpp"), order.PaymentDetails.Kpp);
                }
                else if (order.PaymentMethod is Bill method)
                {
                    string billInn = "", billCompanyName = "", kpp = "";

                    if (method.GetCustomerDataMethod == EGetCustomerDataMethod.FromAdditionalFields)
                    {
                        var customerCompanyNameField =
                            CustomerFieldService.GetCustomerFieldsWithValue(order.OrderCustomer.CustomerID, method.CustomerCompanyNameField.TryParseInt());
                        var customerINNField =
                            CustomerFieldService.GetCustomerFieldsWithValue(order.OrderCustomer.CustomerID, method.CustomerINNField.TryParseInt());
                        var customerKppField =
                            CustomerFieldService.GetCustomerFieldsWithValue(order.OrderCustomer.CustomerID, method.CustomerKppField.TryParseInt());
                    
                        billCompanyName = customerCompanyNameField?.Value;
                        billInn = customerINNField?.Value;
                        kpp = customerKppField?.Value;
                    }
                    
                    worksheet.Cells[9, 4].Value = LocalizationService.GetResource("Admin.Orders.OrderItem.Payer");
                    worksheet.Cells[10, 4].Value = string.Format("     {0} {1}", LocalizationService.GetResource("Admin.Orders.OrderItem.Company"), billCompanyName);
                    worksheet.Cells[11, 4].Value = string.Format("     {0} {1}", LocalizationService.GetResource("Admin.Orders.OrderItem.Inn"), billInn);
                    worksheet.Cells[11, 4].Value = string.Format("     {0} {1}", LocalizationService.GetResource("Admin.Orders.OrderItem.Kpp"), kpp);
                }

                worksheet.Cells[18, 1].Value = LocalizationService.GetResource("Admin.Orders.OrderItem.OrderItem");

                worksheet.Cells[20, 1].Value = LocalizationService.GetResource("Admin.Orders.OrderItem.Sku");
                worksheet.Cells[20, 2].Value = LocalizationService.GetResource("Admin.Orders.OrderItem.ItemName");
                worksheet.Cells[20, 3].Value = LocalizationService.GetResource("Admin.Orders.OrderItem.CustomOptions");
                worksheet.Cells[20, 4].Value = LocalizationService.GetResource("Admin.Orders.OrderItem.Price");
                worksheet.Cells[20, 5].Value = LocalizationService.GetResource("Admin.Orders.OrderItem.ItemAmount");
                worksheet.Cells[20, 7].Value = LocalizationService.GetResource("Admin.Orders.OrderItem.ItemCost");

                //productprice
                var currency = order.OrderCurrency;
                float productPrice = order.OrderItems.Sum(item => PriceService.SimpleRoundPrice(item.Amount * item.Price, currency));
                worksheet.Cells[22, 6].Value = LocalizationService.GetResource("Admin.Orders.OrderItem.ProductsPrice");
                worksheet.Cells[22, 7].Value = (decimal)productPrice;
                worksheet.Cells[22, 8].Value = currency.CurrencySymbol.Trim();

                int summaryRow = 23;
                int styleRow = 23;
                //totalsum
                worksheet.Cells[summaryRow, 6].Value = LocalizationService.GetResource("Admin.Orders.OrderItem.TotalPrice");
                worksheet.Cells[summaryRow, 7].Value = (decimal)order.Sum;
                worksheet.Cells[summaryRow, 8].Value = currency.CurrencySymbol.Trim();

                //comment
                worksheet.Cells[24, 1].Value = LocalizationService.GetResource("Admin.Orders.OrderItem.CustomerComment");
                worksheet.Cells[25, 1].Value = order.CustomerComment;



                //if (order.PaymentCost > 0)
                //{
                //insert before summaryRow row with copy style from styleRow

                worksheet.InsertRow(summaryRow, 1, styleRow);
                if (order.ArchivedPaymentName.IsNotEmpty())
                    worksheet.Cells[summaryRow, 6].Value = "(" + order.ArchivedPaymentName + ")";

                worksheet.InsertRow(summaryRow, 1, styleRow);
                worksheet.Cells[summaryRow, 6].Value = LocalizationService.GetResource("Core.ExportImport.ExcelSingleOrder.PaymentExtracharge");
                worksheet.Cells[summaryRow, 7].Value = (decimal)order.PaymentCost;
                worksheet.Cells[summaryRow, 8].Value = currency.CurrencySymbol.Trim();

                //}

                worksheet.InsertRow(summaryRow, 1, styleRow);
                if (shippingMethodName.IsNotEmpty())
                    worksheet.Cells[summaryRow, 6].Value = "(" + shippingMethodName + ")";

                worksheet.InsertRow(summaryRow, 1, styleRow);
                worksheet.Cells[summaryRow, 6].Value = LocalizationService.GetResource("Core.ExportImport.ExcelSingleOrder.ShippingPrice");
                worksheet.Cells[summaryRow, 7].Value = (decimal)order.ShippingCost;
                worksheet.Cells[summaryRow, 8].Value = order.OrderCurrency.CurrencySymbol.Trim();

                var taxedItems = order.Taxes;
                if (taxedItems.Count > 0)
                {
                    foreach (var tax in taxedItems)
                    {
                        worksheet.InsertRow(summaryRow, 1, styleRow);
                        worksheet.Cells[summaryRow, 6].Value = (tax.ShowInPrice ? LocalizationService.GetResource("Core.Tax.IncludeTax") + " " : "") + tax.Name + ":";
                        worksheet.Cells[summaryRow, 7].Value = (decimal?)tax.Sum;
                        worksheet.Cells[summaryRow, 8].Value = currency.CurrencySymbol.Trim();
                    }
                }
                else
                {
                    worksheet.InsertRow(summaryRow, 1, styleRow);
                    worksheet.Cells[summaryRow, 6].Value = LocalizationService.GetResource("Admin.Orders.OrderItem.Taxes");
                    worksheet.Cells[summaryRow, 7].Value = 0;
                    worksheet.Cells[summaryRow, 8].Value = currency.CurrencySymbol.Trim();
                }

                float bonusPrice = order.BonusCost;
                if (bonusPrice > 0)
                {
                    worksheet.InsertRow(summaryRow, 1, styleRow);
                    worksheet.Cells[summaryRow, 6].Value = LocalizationService.GetResource("Admin.Orders.OrderItem.Bonuses");
                    worksheet.Cells[summaryRow, 7].Value = -1 * (decimal)bonusPrice;
                    worksheet.Cells[summaryRow, 8].Value = currency.CurrencySymbol.Trim();
                }

                if (order.Certificate != null)
                {
                    worksheet.InsertRow(summaryRow, 1, styleRow);
                    worksheet.Cells[summaryRow, 6].Value = LocalizationService.GetResource("Admin.Orders.OrderItem.Certificate");
                    worksheet.Cells[summaryRow, 7].Value = -1 * (decimal)order.Certificate.Price;
                    worksheet.Cells[summaryRow, 8].Value = currency.CurrencySymbol.Trim();
                }

                if (order.OrderDiscount != 0 || order.OrderDiscountValue != 0)
                {
                    var productsIgnoreDiscountPrice = order.OrderItems.Where(item => item.IgnoreOrderDiscount).Sum(item => item.Price * item.Amount);
                    worksheet.InsertRow(summaryRow, 1, styleRow);
                    worksheet.Cells[summaryRow, 6].Value = LocalizationService.GetResource("Admin.Orders.OrderItem.Discount");
                    worksheet.Cells[summaryRow, 7].Value = -1 * (decimal)PriceService.SimpleRoundPrice((productPrice - productsIgnoreDiscountPrice) / 100 * order.OrderDiscount + order.OrderDiscountValue);
                    worksheet.Cells[summaryRow, 8].Value = currency.CurrencySymbol.Trim();
                }

                if (order.Coupon != null)
                {
                    // coupon code
                    worksheet.InsertRow(summaryRow, 1, styleRow);
                    //insert before summaryRow row with copy style from styleRow
                    worksheet.InsertRow(summaryRow, 1, styleRow);
                    worksheet.Cells[summaryRow, 6].Value = LocalizationService.GetResource("Admin.Orders.OrderItem.Coupon");
                    var productsWithCoupon = order.OrderItems.Where(item => item.IsCouponApplied).Sum(item => item.Price * item.Amount);
                    switch (order.Coupon.Type)
                    {
                        case CouponType.Fixed:
                            worksheet.Cells[summaryRow, 7].Value = -1 * (decimal)PriceService.SimpleRoundPrice(order.Coupon.Value, currency);
                            worksheet.Cells[summaryRow + 1, 6].Value = string.Format("({0})", order.Coupon.Code);
                            break;
                        case CouponType.Percent:
                            worksheet.Cells[summaryRow, 7].Value = -1 * (decimal)PriceService.SimpleRoundPrice(productsWithCoupon * order.Coupon.Value / 100, currency);
                            worksheet.Cells[summaryRow + 1, 6].Value = string.Format("({0} ({1}%))", order.Coupon.Code, PriceFormatService.FormatPriceInvariant(order.Coupon.Value));
                            break;
                    }
                    worksheet.Cells[summaryRow, 8].Value = currency.CurrencySymbol.Trim();
                }

                RenderOrderItems(worksheet, order);

                excel.SaveAs(new FileInfo(filename));
            }
        }
    }
}
