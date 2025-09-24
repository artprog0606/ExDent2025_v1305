using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Catalog.Warehouses;
using AdvantShop.Core.SQL;
using AdvantShop.Core.SQL2;
using AdvantShop.ExportImport;
using AdvantShop.Repository.Currencies;
using AdvantShop.SEO;

namespace AdvantShop.Core.Services.OneC
{
    public class OneCService
    {
        public static IEnumerable<ExportFeedCsvProduct> GetProducts(DateTime fromDate, DateTime toDate, bool onlyFromOrders,
            ExportFeedSettings<ExportFeedCsvOptions> options)
        {
            int? fromProductId = null;
            bool complete = false;
            int batchSize = SettingsExport.ExportProductsBatchSize;

            do
            {
                var products = GetProductsBatch(fromDate, toDate, onlyFromOrders, fromProductId, options);
                fromProductId = products.LastOrDefault()?.ProductId;

                foreach (var productRow in products)
                    yield return productRow;

                if (!fromProductId.HasValue || products.Count < batchSize)
                    complete = true;

            } while (!complete);
        }

        private static List<ExportFeedCsvProduct> GetProductsBatch(DateTime fromDate, DateTime toDate, bool onlyFromOrders, 
            int? fromProductId, ExportFeedSettings<ExportFeedCsvOptions> settings)
        {
            var query = new SqlBuilder();

            query.Select("*")
                .From("[Catalog].[Product]")
                .LeftJoin("[Catalog].[ProductExportOptions] ON [ProductExportOptions].[ProductId] = [Product].[ProductID]")
                .LeftJoin("[Catalog].[Photo] ON [Photo].[ObjId] = [Product].[ProductID] AND Type = {0} AND Photo.[Main] = 1", PhotoType.Product.ToString());

            if (fromProductId.HasValue)
                query.Where("p.ProductId > {0}", fromProductId);

            if (!fromDate.IsDefault() && !toDate.IsDefault())
                query.Where("DateModified >= {0} and DateModified <= {1}", fromDate, toDate);
            if (onlyFromOrders)
                query.Where("EXISTS (SELECT 1 FROM [Order].[OrderItems] WHERE [OrderItems].[ProductID] = [Product].[ProductID])");

            query.OrderBy("Product.ProductId");

            return SQLDataAccess.ExecuteReadList(query.GetQuery(), CommandType.Text,
                reader => GetFromReader(reader, settings),
                query.GetSqlParams());
        }

        private static ExportFeedCsvProduct GetFromReader(SqlDataReader reader, ExportFeedSettings<ExportFeedCsvOptions> settings)
        {
            var product = ProductService.GetProductFromReaderWithExportOptions(reader);
            var offerForExport = OfferService.GetMainOfferForExport(product.ProductId);

            var options = settings.AdvancedSettings;

            var productCsv = new ExportFeedCsvProduct
            {
                ProductId = product.ProductId,
                ArtNo = product.ArtNo,
                Name = product.Name,
                UrlPath = product.UrlPath,
                Enabled = product.Enabled ? "+" : "-",
                Unit = product.UnitId.HasValue
                    ? UnitService.UnitToString(product.UnitId.Value)
                    : string.Empty,
                ShippingPrice = product.ShippingPrice == null ? "" : product.ShippingPrice.Value.ToString("F2"),
                Discount = product.Discount.Percent.ToString("F2"),
                DiscountAmount = product.Discount.Amount.ToString("F2"),
                Weight = (offerForExport?.GetWeight() ?? 0).ToFormatString(),
                Size = (offerForExport != null ? offerForExport.GetDimensions() : "0x0x0"),

                BriefDescription = product.BriefDescription,
                Description = product.Description,
                Producer = BrandService.BrandToString(product.BrandId),

                Category = CategoryService.GetCategoryStringByProductId(product.ProductId, options.CsvColumSeparator),
                Sorting = CategoryService.GetCategoryStringByProductId(product.ProductId, options.CsvColumSeparator, true),
                Currency = CurrencyService.GetCurrency(product.CurrencyID).Iso3,
                Markers = ProductService.MarkersToString(
                    product.BestSeller,
                    product.New,
                    product.Recomended,
                    product.OnSale,
                    options.CsvColumSeparator),
                Photos = PhotoService.PhotoToString(
                    PhotoService.GetPhotos<ProductPhoto>(product.ProductId, PhotoType.Product),
                    options.CsvColumSeparator,
                    options.CsvPropertySeparator),
                Videos = ProductVideoService.VideoToString(
                    ProductVideoService.GetProductVideos(product.ProductId)),
                Properties = PropertyService.PropertiesToString(
                    PropertyService.GetPropertyValuesByProductId(product.ProductId),
                    options.CsvColumSeparator,
                    options.CsvPropertySeparator),

                OrderByRequest = product.AllowPreOrder ? "+" : "-",
                
                AccrueBonuses = product.AccrueBonuses ? "+" : "-",

                Related = ProductService.LinkedProductToString(product.ProductId, RelatedType.Related, options.CsvColumSeparator),
                Alternative = ProductService.LinkedProductToString(product.ProductId, RelatedType.Alternative, options.CsvColumSeparator),
                CustomOption = CustomOptionsService.CustomOptionsToString(
                    CustomOptionsService.GetCustomOptionsByProductId(product.ProductId)),

                YandexSalesNote = product.ExportOptions.YandexSalesNote,
                Gtin = product.ExportOptions.Gtin,
                Mpn = product.ExportOptions.Mpn,
                GoogleProductCategory = product.ExportOptions.GoogleProductCategory,
                YandexTypePrefix = product.ExportOptions.YandexTypePrefix,
                YandexModel = product.ExportOptions.YandexModel,
                Adult = product.ExportOptions.Adult ? "+" : "-",
                ManufacturerWarranty = product.ExportOptions.ManufacturerWarranty ? "+" : "-",

                Gifts = ProductGiftService.GiftsToString(product.ProductId, options.CsvColumSeparator),
            };

            var meta = MetaInfoService.GetMetaInfo(productCsv.ProductId, MetaType.Product) ??
                           new MetaInfo(0, 0, MetaType.Product, string.Empty, string.Empty, string.Empty, string.Empty);

            productCsv.Title = meta.Title;
            productCsv.H1 = meta.H1;
            productCsv.MetaKeywords = meta.MetaKeywords;
            productCsv.MetaDescription = meta.MetaDescription;

            if (!product.HasMultiOffer && product.Offers.All(offer => offer.ArtNo == product.ArtNo))
            {
                var offer = product.Offers.FirstOrDefault() ?? new Offer();
                productCsv.Price = offer.BasePrice.ToString("F2");
                productCsv.PurchasePrice = offer.SupplyPrice.ToString("F2");
                
                if ((options.StocksFromWarehouses?.Count ?? 0) > 0)
                {
                    if (options.StocksFromWarehouses.Count == 1)
                        offer.SetAmountByWarehouse(options.StocksFromWarehouses[0]);
                    else
                        offer.SetAmountByStocks(
                            WarehouseStocksService.GetOfferStocks(offer.OfferId)
                                                  .Where(x => options.StocksFromWarehouses.Contains(x.WarehouseId))
                        );
                }
                productCsv.Amount = offer.Amount.ToString(CultureInfo.InvariantCulture);
                productCsv.MultiOffer = string.Empty;
            }
            else
            {
                productCsv.Price = string.Empty;
                productCsv.PurchasePrice = string.Empty;
                productCsv.Amount = string.Empty;

                if ((options.StocksFromWarehouses?.Count ?? 0) > 0)
                {
                    var stocks =
                        WarehouseStocksService.GetProductStocks(product.ProductId)
                                              .Where(s => options.StocksFromWarehouses.Contains(s.WarehouseId))
                                              .ToList();

                    foreach (var offer in product.Offers)
                        offer.SetAmountByStocks(
                            stocks
                               .Where(s => s.OfferId == offer.OfferId)
                        );
                }
                productCsv.MultiOffer = OfferService.OffersToString(product.Offers, options.CsvColumSeparator, options.CsvPropertySeparator);
            }

            return productCsv;
        }
    }
}
