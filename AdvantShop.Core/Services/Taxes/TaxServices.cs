//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------


using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Taxes;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using AdvantShop.Repository.Currencies;

namespace AdvantShop.Taxes
{
    public class TaxService
    {
        private const string TaxCachekey = "Tax_";

        public static void AddTax(TaxElement t)
        {
            t.TaxId = SQLDataAccess.ExecuteScalar<int>(
                "INSERT INTO [Catalog].[Tax]([Name], [Enabled], [ShowInPrice], [Rate], [TaxType]) VALUES (@name, @enabled, @showInPrice, @Rate, @TaxType); Select scope_identity();",
                CommandType.Text,
                new SqlParameter("@name", t.Name),
                new SqlParameter("@enabled", t.Enabled),
                new SqlParameter("@showInPrice", t.ShowInPrice),
                new SqlParameter("@Rate", t.Rate),
                new SqlParameter("@TaxType", (int) t.TaxType));

            CacheManager.RemoveByPattern(TaxCachekey);
        }

        public static List<TaxElement> GetTaxes()
        {
            return CacheManager.Get(TaxCachekey + "list",
                () => SQLDataAccess.Query<TaxElement>("SELECT * FROM [Catalog].[Tax]")
                    .OrderBy(x => x.Rate)
                    .ToList());
        }

        public static TaxElement GetTax(int id)
        {
            return CacheManager.Get(TaxCachekey + id,
                () =>
                    SQLDataAccess.Query<TaxElement>("SELECT * FROM [Catalog].[Tax] WHERE [TaxId] = @id", new {id})
                        .FirstOrDefault());
        }

        public static void UpdateTax(TaxElement t)
        {
            SQLDataAccess.ExecuteNonQuery(
                "UPDATE [Catalog].[Tax] SET Name=@name, Enabled=@enabled, ShowInPrice=@showInPrice, Rate=@Rate, TaxType=@TaxType WHERE TaxId=@TaxId",
                CommandType.Text,
                new SqlParameter("@TaxId", t.TaxId),
                new SqlParameter("@name", t.Name),
                new SqlParameter("@enabled", t.Enabled),
                new SqlParameter("@showInPrice", t.ShowInPrice),
                new SqlParameter("@Rate", t.Rate),
                new SqlParameter("@TaxType", (int)t.TaxType));

            CacheManager.RemoveByPattern(TaxCachekey);
        }

        public static bool CanDeleteTax(int taxId)
        {
            if (taxId == SettingsCatalog.DefaultTaxId)
                return false;

            var productsCount =
                Convert.ToInt32(
                    SQLDataAccess.ExecuteScalar(
                        "Select Count(ProductId) FROM [Catalog].[Product] WHERE [TaxId] = @TaxId", CommandType.Text,
                        new SqlParameter("@TaxId", taxId)));

            var usedInCertificates = SQLDataAccess.ExecuteScalar<bool>(
                "SELECT CAST(COUNT (1) as bit) [TaxID] FROM [Settings].[GiftCertificateTaxes] where [GiftCertificateTaxes].[TaxID] = @TaxId", CommandType.Text,
                new SqlParameter("@TaxId", taxId));

            var usedInPaymentMethods = SQLDataAccess.ExecuteScalar<bool>(
                "SELECT CAST(COUNT (1) as bit) [TaxId] FROM [Order].[PaymentMethod] where [PaymentMethod].[TaxId] = @TaxId", CommandType.Text,
                new SqlParameter("@TaxId", taxId));

            var usedInShippingMethods = SQLDataAccess.ExecuteScalar<bool>(
                "SELECT CAST(COUNT (1) as bit) [TaxId] FROM [Order].[ShippingMethod] where [ShippingMethod].[TaxId] = @TaxId", CommandType.Text,
                new SqlParameter("@TaxId", taxId));

            return productsCount == 0 && !usedInCertificates && !usedInPaymentMethods && !usedInShippingMethods;
        }

        public static void DeleteTax(int taxId)
        {
            if (CanDeleteTax(taxId))
            {
                SQLDataAccess.ExecuteNonQuery("DELETE FROM [Catalog].[Tax] WHERE [TaxId] = @TaxId", CommandType.Text, new SqlParameter("@TaxId", taxId));
                CacheManager.RemoveByPattern(TaxCachekey);
            }
        }


        public static List<int> GetAllTaxesIDs()
        {
            List<int> result = SQLDataAccess.ExecuteReadList("select [TaxId] from [Catalog].[Tax]", CommandType.Text,
                                                             reader => SQLDataHelper.GetInt(reader, "TaxId"));
            return result;
        }
        
        public static Dictionary<TaxElement, float> CalculateTaxes(float price)
        {
            return GetTaxes().Where(tax=>tax.Enabled).ToDictionary(tax => tax, tax => CalculateTax(price, tax));
        }

        public static Dictionary<TaxElement, float> CalculateTaxes(ShoppingCart cart, float productsPriceWithDiscount, float shippingCost, TaxType shippingTaxType)
        {
            var items = cart.Select(x => new TaxCartItem()
            {
                Price = x.PriceWithDiscount,
                Amount = x.Amount,
                TaxId = x.Offer.Product.TaxId
            }).ToList();

            return GetOrderTaxes(items, productsPriceWithDiscount, shippingCost, shippingTaxType);
        }

        public static List<OrderTax> GetOrderTaxes(List<OrderItem> orderItems, float sum, float shippingCost, TaxType shippingTaxType)
        {
            if (orderItems == null || !orderItems.Any())
            {
                return new List<OrderTax>();
            }

            var items = orderItems.Select(x => new TaxCartItem()
            {
                Price = x.Price,
                Amount = x.Amount,
                TaxId = x.TaxId
            }).ToList();

            return GetOrderTaxes(items, sum - shippingCost, shippingCost, shippingTaxType).Select(x => new OrderTax()
            {
                TaxId = x.Key.TaxId,
                Name = x.Key.Name,
                ShowInPrice = x.Key.ShowInPrice,
                Rate = x.Key.TaxType == TaxType.VatWithout ? (float?)null : x.Key.Rate,
                Sum = x.Key.TaxType == TaxType.VatWithout ? (float?)null : x.Value
            }).ToList();
        }

        public static Dictionary<TaxElement, float> GetOrderTaxes(Order order)
        {
            var items = order.OrderItems.Select(x => new TaxCartItem()
            {
                Price = x.Price,
                Amount = x.Amount,
                TaxId = x.TaxId
            }).ToList();

            return GetOrderTaxes(items, order.Sum - order.ShippingCostWithDiscount, order.ShippingCostWithDiscount, order.ShippingTaxType);
        }

        public static Dictionary<TaxElement, float> GetOrderTaxes(List<TaxCartItem> taxCartItems, float productsPriceWithDiscount, float shippingCost, TaxType shippingTaxType)
        {
            var result = new Dictionary<TaxElement, float>();

            var productsTotal = taxCartItems.Sum(x => x.Amount * x.Price);

            if (productsTotal > 0)
            {
                var div = productsPriceWithDiscount - productsTotal;

                foreach (var item in taxCartItems)
                {
                    item.Price += item.Price / productsTotal * div;
                }
            }

            var newTotal = taxCartItems.Sum(item => item.Amount * item.Price);
            if (newTotal != productsPriceWithDiscount)
            {
                taxCartItems.Last().Price += (float)Math.Round((productsPriceWithDiscount - newTotal) / taxCartItems.Last().Amount);
            }

            var taxes = GetTaxes();

            if (shippingCost != 0)
            {
                var shippingTax = taxes.FirstOrDefault(x => x.TaxType == shippingTaxType && x.Enabled);
                if (shippingTax == null && shippingTaxType != TaxType.None)
                {
                    shippingTax = new TaxElement()
                    {
                        TaxId = 0,
                        Name = shippingTaxType.Localize(),
                        Rate = GetVatByTaxType(shippingTaxType),
                        Enabled = true,
                        ShowInPrice = true
                    };

                    //вытаскиваем чтобы не записать в кеш
                    taxes = taxes.ToList();
                    taxes.Add(shippingTax);
                }

                if (shippingTax != null)
                    taxCartItems.Add(new TaxCartItem() { TaxId = shippingTax.TaxId, Price = shippingCost, Amount = 1 });
            }

            foreach (var taxCartItem in taxCartItems.Where(x => x.TaxId != null))
            {
                var tax = taxes.Find(x => x.TaxId == taxCartItem.TaxId);
                if (tax != null && tax.ShowInPrice)
                {
                    // стоимость товара / (100 + ндс) * ндс
                    var vat = (float) Math.Round(taxCartItem.Price*taxCartItem.Amount/(100 + tax.Rate)*tax.Rate, 2);

                    var t = result.Keys.FirstOrDefault(x => x.TaxId == tax.TaxId);
                    if (t != null)
                    {
                        result[t] += vat;
                    }
                    else
                    {
                        result.Add(tax, vat);
                    }
                }
            }

            return result;
        }


        private static float CalculateTax(float price, TaxElement tax)
        {
            var returnTax = tax.Rate;

            returnTax = tax.ShowInPrice ? returnTax*price/(100.0F + returnTax) : returnTax*price/100.0F;

            return returnTax.RoundPrice(CurrencyService.CurrentCurrency.Rate);
        }

        private static int GetVatByTaxType(TaxType taxType)
        {
            if (taxType == TaxType.Vat20) return 20;
            if (taxType == TaxType.Vat18) return 18;
            if (taxType == TaxType.Vat10) return 10;
            return 0;
        }
        
        [Obsolete]
        public static void SetOrderTaxes(int orderId, List<OrderTax> taxValues)
        {
            using (var db = new SQLDataAccess())
            {
                db.cmd.CommandText = "insert into [Order].[OrderTax] (TaxID, TaxName, TaxSum, TaxRate, TaxShowInPrice, OrderId) values (@TaxID, @TaxName, @TaxSum, @TaxRate, @TaxShowInPrice, @OrderId)";
                db.cmd.CommandType = CommandType.Text;
                db.cnOpen();
                foreach (var tax in taxValues)
                {
                    db.cmd.Parameters.Clear();
                    db.cmd.Parameters.AddWithValue("@OrderId", orderId);
                    db.cmd.Parameters.AddWithValue("@TaxID", tax.TaxId);
                    db.cmd.Parameters.AddWithValue("@TaxName", tax.Name);
                    db.cmd.Parameters.AddWithValue("@TaxSum", tax.Sum ?? 0f);
                    db.cmd.Parameters.AddWithValue("@TaxRate", tax.Rate ?? 0f);
                    db.cmd.Parameters.AddWithValue("@TaxShowInPrice", tax.ShowInPrice);
                    db.cmd.ExecuteNonQuery();
                }
                db.cnClose();
            }
        }

        [Obsolete]
        public static void ClearOrderTaxes(int orderId)
        {
            SQLDataAccess.ExecuteNonQuery("delete from [Order].[OrderTax] where [OrderId] = @OrderId",
                                            CommandType.Text,
                                            new SqlParameter("@OrderId", orderId));
        }

        public static string BuildTaxTable(List<OrderTax> taxes, Currency currency, string message)
        {
            var sb = new StringBuilder();
            if (!taxes.Any())
            {
                sb.Append("<tr><td style=\"background-color: #FFFFFF; text-align: right\">");
                sb.Append(message);
                sb.Append("&nbsp;</td><td style=\"background-color: #FFFFFF; width: 150px\">");
                sb.Append(PriceFormatService.FormatPrice(0, currency));
                sb.Append("</td></tr>");
            }
            else
                foreach (OrderTax tax in taxes)
                {
                    sb.Append("<tr><td style=\"background-color: #FFFFFF; text-align: right\">");
                    sb.Append((tax.ShowInPrice ? LocalizationService.GetResource("Core.Tax.IncludeTax") : "") + " " + tax.Name);
                    sb.Append(":&nbsp</td><td style=\"background-color: #FFFFFF; width: 150px\">" + (tax.ShowInPrice ? "" : "+"));
                    sb.Append(tax.Sum.HasValue ? PriceFormatService.FormatPrice(PriceService.RoundPrice(tax.Sum.Value, currency, currency.Rate), currency) : tax.Name);
                    sb.Append("</td></tr>");
                }
            return sb.ToString();
        }

        #region Certificates
        public static void DeleteCertificateTaxes()
        {
            SQLDataAccess.ExecuteScalar("DELETE FROM [Settings].[GiftCertificateTaxes]", CommandType.Text);
        }

        public static void SaveCertificateTax(int taxId)
        {
            DeleteCertificateTaxes();

            SQLDataAccess.ExecuteScalar(
                "INSERT INTO [Settings].[GiftCertificateTaxes] ([TaxID]) VALUES (@TaxID)",
                CommandType.Text,
                new SqlParameter("@TaxID", taxId));
        }

        public static TaxElement GetCertificateTax()
        {
            return SQLDataAccess.Query<TaxElement>("SELECT top 1 * FROM [Settings].[GiftCertificateTaxes] inner join Catalog.Tax on GiftCertificateTaxes.TaxID = Tax.TaxID").FirstOrDefault();
        }

        public static float? CalculateCertificateTax(float price, out TaxElement tax)
        {
            tax = GetCertificateTax();
            return tax != null && tax.Enabled ? CalculateTax(price, tax) : (float?)null;
        }

        #endregion 
    }
}