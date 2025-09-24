//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Repository.Currencies;
using AdvantShop.Taxes;

namespace AdvantShop.ExportImport
{
    public class ExportFeedResellerService
    {
        public static ExportFeedCsvProduct GetCsvProductFromReader(SqlDataReader reader, ExportFeedSettings<ExportFeedResellerOptions> settings)
        {
            var product = ProductService.GetProductFromReaderWithExportOptions(reader);
            var options = settings.AdvancedSettings;

            foreach (var offer in product.Offers)
            {
                var markup = offer.BasePrice * settings.PriceMarginInPercents / 100 + settings.PriceMarginInNumbers;

                offer.SupplyPrice = offer.BasePrice - markup;

                offer.BasePrice += markup;
            }

            var offerForExport = product.Offers.FirstOrDefault(x => x.Main);

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
                Discount = product.Discount.Type == DiscountType.Percent ? product.Discount.Percent.ToString("F2") : "",
                DiscountAmount = product.Discount.Type == DiscountType.Amount ? product.Discount.Amount.ToString("F2") : "",
                Weight = (offerForExport?.GetWeight() ?? 0).ToFormatString(),
                Size = (offerForExport != null ? offerForExport.GetDimensions("|") : "0|0|0"),

                BriefDescription = product.BriefDescription,
                Description = product.Description,
                Producer = BrandService.BrandToString(product.BrandId),

                Category =
                    CategoryService.GetCategoryStringByProductId(product.ProductId,
                        options.CsvColumSeparator,
                        onlyMainCategory: options.UnloadOnlyMainCategory ?? false),
                Sorting =
                    CategoryService.GetCategoryStringByProductId(product.ProductId,
                        options.CsvColumSeparator,
                        onlySort: true,
                        onlyMainCategory: options.UnloadOnlyMainCategory ?? false),
                Currency = CurrencyService.GetCurrency(product.CurrencyID).Iso3,
                Markers = ProductService.MarkersToString(
                    product.BestSeller,
                    product.New,
                    product.Recomended,
                    product.OnSale,
                    options.CsvColumSeparator),
                Photos = PhotoService.PhotoToString(
                    PhotoService.GetPhotos<ProductPhoto>(product.ProductId,
                        PhotoType.Product), ";", ":", true),
                Videos = ProductVideoService.VideoToString(
                    ProductVideoService.GetProductVideos(product.ProductId)),
                Properties = PropertyService.PropertiesToString(
                    PropertyService.GetPropertyValuesByProductId(product.ProductId),
                    options.CsvColumSeparator,
                    options.CsvPropertySeparator),

                OrderByRequest = product.AllowPreOrder ? "+" : "-",
                
                AccrueBonuses = product.AccrueBonuses ? "+" : "-",

                Related =
                    ProductService.LinkedProductToString(product.ProductId,
                        RelatedType.Related, options.CsvColumSeparator),
                Alternative =
                    ProductService.LinkedProductToString(product.ProductId,
                        RelatedType.Alternative, options.CsvColumSeparator),
                CustomOption =
                    CustomOptionsService.CustomOptionsToString(
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
                BarCode = offerForExport != null ? offerForExport.BarCode : string.Empty,

                MinAmount = product.MinAmount == null ? "" : product.MinAmount.Value.ToString("F2"),
                MaxAmount = product.MaxAmount == null ? "" : product.MaxAmount.Value.ToString("F2"),
                Multiplicity = product.Multiplicity.ToString("F5").Trim('0'),
                Bid = product.ExportOptions.Bid.ToString(),
                YandexSizeUnit = product.ExportOptions.YandexSizeUnit,
                YandexName = product.ExportOptions.YandexName ?? string.Empty,
                YandexDeliveryDays = product.ExportOptions.YandexDeliveryDays ?? "",
            };

            if (product.Meta != null)
            {
                productCsv.H1 = product.Meta.H1;
                productCsv.MetaDescription = product.Meta.MetaDescription;
                productCsv.MetaKeywords = product.Meta.MetaKeywords;
                productCsv.Title = product.Meta.Title;
            }

            if (!product.Offers.Any(offer => offer.ColorID.HasValue || offer.SizeID.HasValue || offer.ArtNo != product.ArtNo))
            //if (!product.HasMultiOffer)
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
                            Core.Services.Catalog.Warehouses.WarehouseStocksService.GetOfferStocks(offer.OfferId)
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
                        Core.Services.Catalog.Warehouses.WarehouseStocksService.GetProductStocks(product.ProductId)
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

            if (product.TaxId != null)
            {
                var tax = TaxService.GetTax(product.TaxId.Value);
                if (tax != null)
                    productCsv.Tax = tax.Name;
            }

            var tags = TagService.Gets(product.ProductId, ETagType.Product);
            if (tags != null)
            {
                productCsv.Tags =
                    tags.Select(item => item.Name).AggregateString(options.CsvColumSeparator);
            }

            return productCsv;
        }
    }
}