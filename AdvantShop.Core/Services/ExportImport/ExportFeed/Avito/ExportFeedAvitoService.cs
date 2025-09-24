//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.SQL;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Repository.Currencies;

namespace AdvantShop.ExportImport
{
    public class ExportFeedAvitoService
    {
        public static ExportFeedAvitoProduct GetAvitoProductsFromReader(SqlDataReader reader, ExportFeedSettings<ExportFeedAvitoOptions> settings)
        {
            var product = ProductService.GetProductFromReaderWithExportOptions(reader);
            var offerForExport = OfferService.GetMainOfferForExport(product.ProductId);

            var productCsv = new ExportFeedAvitoProduct
            {
                ProductId = product.ProductId,
                ArtNo = product.ArtNo,
                Name = product.Name,
                UrlPath = product.UrlPath,
                Unit = product.UnitId.HasValue
                    ? UnitService.UnitToString(product.UnitId.Value)
                    : string.Empty,
                Price = offerForExport?.BasePrice ?? 0,
                Discount = product.Discount.Type == DiscountType.Percent ? product.Discount.Percent : 0,
                DiscountAmount = product.Discount.Type == DiscountType.Amount ? product.Discount.Amount : 0,
                Weight = (offerForExport?.GetWeight() ?? 0).ToString("F2"),
                Size = (offerForExport != null ? offerForExport.GetDimensions() : "0x0x0"),
                BarCode = offerForExport != null ? offerForExport.BarCode : string.Empty,
                BriefDescription = product.BriefDescription,
                Description = product.Description,
                Properties = 
                    product.ProductPropertyValues
                        .Select(x => $"{x.Property.NameDisplayed}: {x.Value}")
                        .AggregateString(","),
                BrandName = BrandService.BrandToString(product.BrandId),
                PhotosList = PhotoService.GetPhotos<ProductPhoto>(product.ProductId, PhotoType.Product),
                Offers = new List<Offer>(),
                AvitoProperties = GetProductProperties(product.ProductId) ?? new List<ExportFeedAvitoProductProperty>()
            };

            if (settings.AdvancedSettings.PriceRuleId != null && offerForExport != null)
            {
                var priceRule = 
                    PriceRuleService.GetOfferPriceRules(offerForExport.OfferId)
                        .FirstOrDefault(x => x.PriceRuleId == settings.AdvancedSettings.PriceRuleId);
                
                if (priceRule != null)
                    productCsv.PriceByRule = priceRule.PriceByRule;
            }

            foreach (var offer in product.Offers)
            {
                if (offer.Amount <= 0 && !settings.AdvancedSettings.ExportNotAvailable)
                    continue;
                
                productCsv.Offers.Add(offer);
            }

            productCsv.Colors = String.Join(", ",
                productCsv.Offers.Where(x => x.Color != null).Select(x => x.Color.ColorName).Distinct());
            productCsv.Sizes = String.Join(", ",
                productCsv.Offers.Where(x => x.Size != null).Select(x => x.Size.SizeName).Distinct());

            productCsv.Currency = CurrencyService.GetCurrency(product.CurrencyID, true);

            return productCsv;
        }

        public static List<ExportFeedAvitoProductProperty> GetProductProperties(int productId)
        {
            return SQLDataAccess.ExecuteReadList(
                "Select * From [Catalog].[AvitoProductProperties] Where ProductId = @ProductId",
                CommandType.Text, 
                reader => new ExportFeedAvitoProductProperty
                {
                    ProductId = SQLDataHelper.GetInt(reader, "ProductId"),
                    PropertyName = SQLDataHelper.GetString(reader, "PropertyName"),
                    PropertyValue = SQLDataHelper.GetString(reader, "PropertyValue")
                },
                new SqlParameter("@ProductId", productId));
        }

        public static void DeleteProductProperties(int productId)
        {
            SQLDataAccess.ExecuteNonQuery("Delete From [Catalog].[AvitoProductProperties] Where ProductId = @ProductId",
                CommandType.Text,
                new SqlParameter("@ProductId", productId));
        }

        public static bool AddProductProperties(List<ExportFeedAvitoProductProperty> productProperties)
        {
            if (productProperties == null ||
                productProperties.GroupBy(x => x.PropertyName)
                                 .Any(x => x.Count() > 1))
            {
                return false;
            }

            foreach (var productProperty in productProperties)
            {
                if (string.IsNullOrEmpty(productProperty.PropertyName) || string.IsNullOrEmpty(productProperty.PropertyValue))
                    continue;

                SQLDataAccess.ExecuteNonQuery(
                    "Insert Into [Catalog].[AvitoProductProperties] (ProductId, PropertyName, PropertyValue) Values (@ProductId,@PropertyName,@PropertyValue)",
                    CommandType.Text,
                    new SqlParameter("@ProductId", productProperty.ProductId),
                    new SqlParameter("@PropertyName", productProperty.PropertyName),
                    new SqlParameter("@PropertyValue", productProperty.PropertyValue));
            }

            return true;
        }

        public static void ImportProductProperties(int productId, string productProperties, string columSeparator, string propertySeparator)
        {
            DeleteProductProperties(productId);

            var items = productProperties.Split(new[] { columSeparator }, StringSplitOptions.RemoveEmptyEntries);
            var avitoProductProperties = new List<ExportFeedAvitoProductProperty>();

            foreach (string s in items)
            {
                var temp = s.SupperTrim().Split(new[] { propertySeparator }, StringSplitOptions.RemoveEmptyEntries);
                if (temp.Length == 2)
                {
                    avitoProductProperties.Add(new ExportFeedAvitoProductProperty
                    {
                        ProductId = productId,
                        PropertyName = temp[0].SupperTrim(),
                        PropertyValue = temp[1].SupperTrim()
                    });
                }
            }

            if (avitoProductProperties.Count != 0)
            {
                AddProductProperties(avitoProductProperties);
            }
        }

        public static string GetProductPropertiesInString(int productId, string columSeparator, string propertySeparator)
        {
            var productProperties = GetProductProperties(productId);
            var stringProperties = string.Empty;
            for (int i = 0; i < productProperties.Count; i++)
            {
                if (i == 0)
                {
                    stringProperties += productProperties[i].PropertyName + propertySeparator + productProperties[i].PropertyValue;
                }
                else
                {
                    stringProperties += columSeparator + productProperties[i].PropertyName + propertySeparator + productProperties[i].PropertyValue;
                }
            }
            return stringProperties;
        }
    }
}