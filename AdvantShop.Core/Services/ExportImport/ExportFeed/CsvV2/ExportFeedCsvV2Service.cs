//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

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
using AdvantShop.Core.Services.ChangeHistories;
using AdvantShop.Core.SQL;
using AdvantShop.Localization;
using AdvantShop.Repository.Currencies;
using AdvantShop.Saas;
using AdvantShop.SEO;
using AdvantShop.Taxes;

namespace AdvantShop.ExportImport
{
    public class ExportFeedCsvV2Service
    {
        public static int GetMaxProductCategoriesCount(int exportFeedId, ExportFeedSettings<ExportFeedCsvV2Options> commonSettings)
        {
            return SQLDataAccess.ExecuteScalar<int>(
                "[Settings].[sp_GetCsvMaxProductCategoriesCount]",
                CommandType.StoredProcedure,
                60 * 3,
                new SqlParameter("@exportFeedId", exportFeedId),
                new SqlParameter("@exportAllProducts", commonSettings.ExportAllProducts),
                new SqlParameter("@exportNotAvailable", true)
                );
        }

        public static List<string> GetProductPropertyNames(int exportFeedId, ExportFeedSettings<ExportFeedCsvV2Options> commonSettings)
        {
            return SQLDataAccess.ExecuteReadColumn<string>(
                "[Settings].[sp_GetCsvProductPropertyNames]",
                CommandType.StoredProcedure,
                "Name", null, 60 * 3,
                new SqlParameter("@exportFeedId", exportFeedId),
                new SqlParameter("@exportAllProducts", commonSettings.ExportAllProducts),
                new SqlParameter("@exportNotAvailable", true),
                new SqlParameter("@exportNoInCategory", commonSettings.AdvancedSettings.CsvExportNoInCategory)
                );
        }

        public static ExportFeedCsvV2Product GetCsvProductsFromReader(SqlDataReader reader, ExportFeedSettings<ExportFeedCsvV2Options> commonSettings)
        {
            var product = ProductService.GetProductFromReaderWithExportOptions(reader);
            var advancedSettings = commonSettings.AdvancedSettings;
            var fieldMapping = advancedSettings.FieldMapping;

            var productCsv = new ExportFeedCsvV2Product
            {
                ProductId = product.ProductId,
                ArtNo = product.ArtNo,
                Name = product.Name,
                UrlPath = product.UrlPath,
                DownloadLink = product.DownloadLink,
                Enabled = product.Enabled ? "+" : "-",
                Discount = product.Discount.Type == DiscountType.Percent ? product.Discount.Percent.ToString("F2") : null,
                DiscountAmount = product.Discount.Type == DiscountType.Amount ? product.Discount.Amount.ToString("F2") : null,
                ShippingPrice = product.ShippingPrice?.ToString("F2"),
                BriefDescription = product.BriefDescription,
                Description = product.Description,
                MarkerNew = product.New ? "+" : "-",
                MarkerBestseller = product.BestSeller ? "+" : "-",
                MarkerRecomended = product.Recomended ? "+" : "-",
                MarkerOnSale = product.OnSale ? "+" : "-",
                ManualRatio = product.ManualRatio?.ToString("F2"),
                OrderByRequest = product.AllowPreOrder ? "+" : "-",
                AccrueBonuses = product.AccrueBonuses ? "+" : "-",
                YandexSalesNotes = product.ExportOptions.YandexSalesNote,
                YandexDeliveryDays = product.ExportOptions.YandexDeliveryDays,
                YandexTypePrefix = product.ExportOptions.YandexTypePrefix,
                YandexName = product.ExportOptions.YandexName,
                YandexModel = product.ExportOptions.YandexModel,
                YandexSizeUnit = product.ExportOptions.YandexSizeUnit,
                YandexDiscounted = product.ExportOptions.YandexProductDiscounted ? "+" : "-",
                YandexDiscountCondition = product.ExportOptions.YandexProductDiscountCondition != EYandexDiscountCondition.None ? product.ExportOptions.YandexProductDiscountCondition.StrName() : null,
                YandexDiscountReason = product.ExportOptions.YandexProductDiscountReason,
                YandexProductQuality = product.ExportOptions.YandexProductQuality != EYandexProductQuality.None ? product.ExportOptions.YandexProductQuality.StrName() : null,
                YandexMarketExpiry = product.ExportOptions.YandexMarketExpiry,
                YandexMarketWarrantyDays = product.ExportOptions.YandexMarketWarrantyDays,
                YandexMarketCommentWarranty = product.ExportOptions.YandexMarketCommentWarranty,
                YandexMarketPeriodOfValidityDays = product.ExportOptions.YandexMarketPeriodOfValidityDays,
                YandexMarketServiceLifeDays = product.ExportOptions.YandexMarketServiceLifeDays,
                YandexMarketTnVedCode = product.ExportOptions.YandexMarketTnVedCode,
                YandexMarketStepQuantity = product.ExportOptions.YandexMarketStepQuantity != null ? product.ExportOptions.YandexMarketStepQuantity.ToString() : null,
                YandexMarketMinQuantity = product.ExportOptions.YandexMarketMinQuantity != null ? product.ExportOptions.YandexMarketMinQuantity.ToString() : null,
                YandexMarketCategoryId = product.ExportOptions.YandexMarketCategoryId?.ToString(),
                YandexBid = product.ExportOptions.Bid.ToString(),
                GoogleGtin = product.ExportOptions.Gtin,
                GoogleMpn = product.ExportOptions.Mpn,
                GoogleProductCategory = product.ExportOptions.GoogleProductCategory,
                GoogleAvailabilityDate = product.ExportOptions.GoogleAvailabilityDate?.ToString("O", CultureInfo.InvariantCulture),
                Adult = product.ExportOptions.Adult ? "+" : "-",
                ManufacturerWarranty = product.ExportOptions.ManufacturerWarranty ? "+" : "-",
                MinAmount = product.MinAmount?.ToString("F2"),
                MaxAmount = product.MaxAmount?.ToString("F2"),
                Multiplicity = product.Multiplicity.ToString("F5").Trim('0'),
                IsMarkingRequired = product.IsMarkingRequired ? "+" : "-",
                Comment = product.Comment,
                DoNotApplyOtherDiscounts = product.DoNotApplyOtherDiscounts ? "+" : "-",
                IsDigital = product.IsDigital ? "+" : "-",
                SizeChart = product.SizeChart?.Name,
            };

            if (fieldMapping.Contains(EProductField.PaymentSubjectType))
                productCsv.PaymentSubjectType = product.PaymentSubjectType.Localize();
            
            if (fieldMapping.Contains(EProductField.PaymentMethodType))
                productCsv.PaymentMethodType = product.PaymentMethodType.Localize();
            

            var offers = new List<Offer>();
            if (fieldMapping.Any(x => x.IsOfferField()))
                offers = OfferService.GetProductOffers(product.ProductId).OrderByDescending(x => x.Main).ToList();

            var photos = new List<ProductPhoto>();
            if (fieldMapping.Any(field => field == EProductField.OfferPhotos || field == EProductField.Photos))
                photos = PhotoService.GetPhotos<ProductPhoto>(product.ProductId, PhotoType.Product);

            productCsv.Offers = offers.Select(offer => new ExportFeedCsvV2Offer
            {
                OfferId = offer.OfferId,
                ArtNo = offer.ArtNo,
                Price = (offer.BasePrice +
                         (offer.BasePrice * commonSettings.PriceMarginInPercents / 100 + commonSettings.PriceMarginInNumbers)
                        ).ToString("F2"),
                PurchasePrice = offer.SupplyPrice.ToString("F2"),
                Amount = offer.Amount.ToString(CultureInfo.CurrentCulture),
                Size = fieldMapping.Contains(EProductField.Size)
                    ? offer.Size?.SizeName
                    : null,
                Color = fieldMapping.Contains(EProductField.Color)
                    ? offer.Color?.ColorName
                    : null,
                OfferPhotos =
                    fieldMapping.Contains(EProductField.OfferPhotos) && offer.ColorID.HasValue
                        ? photos.Where(photo => photo.ColorID == offer.ColorID).Select(photo => photo.PhotoName)
                                .AggregateString(advancedSettings.CsvColumSeparator)
                        : null,
                Weight = offer.GetWeight().ToFormatString(),
                Dimensions = offer.GetDimensions(),
                BarCode = offer.BarCode,
            }).ToList();

            if (fieldMapping.Contains(EProductField.Category))
            {
                productCsv.Categories = CategoryService.GetCategoriesPathAndSort(product.ProductId)
                    .Select(x => new ExportFeedCsvV2Category { Path = x.Key, Sort = x.Value }).ToList();
            }

            if (fieldMapping.Contains(EProductField.Currency))
            {
                productCsv.Currency = CurrencyService.GetCurrency(product.CurrencyID).Iso3;
            }

            if (fieldMapping.Contains(EProductField.Photos))
            {
                productCsv.Photos = photos.Where(photo => !photo.ColorID.HasValue).Select(photo => photo.PhotoName).AggregateString(advancedSettings.CsvColumSeparator);
            }

            if (fieldMapping.Contains(EProductField.Property))
            {
                var propertyValues = PropertyService.GetPropertyValuesByProductId(product.ProductId);
                productCsv.Properties = propertyValues.GroupBy(x => x.Property.Name)
                    .ToDictionary(x => x.Key, x => x.Select(y => y.Value).AggregateString(advancedSettings.CsvColumSeparator));
            }

            if (fieldMapping.Contains(EProductField.PriceRule))
            {
                foreach (var offer in productCsv.Offers)
                {
                    var rules = PriceRuleService.GetOfferPriceRules(offer.OfferId, false);

                    offer.PricesByPriceRule = rules.ToDictionary(x => x.Name, x => x.PriceByRule?.ToString("F2"));
                }
            }

            if (fieldMapping.Contains(EProductField.WarehouseStock))
                foreach (var offer in productCsv.Offers)
                    offer.AmountByWarehouses =
                        WarehouseStocksService.GetOfferStocks(offer.OfferId)
                                              .ToDictionary(_ => _.WarehouseId, _ => _.Quantity.ToString(CultureInfo.CurrentCulture));

            if (fieldMapping.Contains(EProductField.SeoTitle) || fieldMapping.Contains(EProductField.SeoH1) ||
                fieldMapping.Contains(EProductField.SeoMetaKeywords) || fieldMapping.Contains(EProductField.SeoMetaDescription))
            {
                var meta = MetaInfoService.GetMetaInfo(productCsv.ProductId, MetaType.Product) ??
                           new MetaInfo(0, 0, MetaType.Product, string.Empty, string.Empty, string.Empty, string.Empty);
                productCsv.H1 = meta.H1;
                productCsv.Title = meta.Title;
                productCsv.MetaKeywords = meta.MetaKeywords;
                productCsv.MetaDescription = meta.MetaDescription;
            }
            if (fieldMapping.Contains(EProductField.Related))
            {
                productCsv.Related =
                    ProductService.LinkedProductToString(product.ProductId,
                        RelatedType.Related, advancedSettings.CsvColumSeparator);
            }
            if (fieldMapping.Contains(EProductField.Alternative))
            {
                productCsv.Alternative =
                    ProductService.LinkedProductToString(product.ProductId,
                        RelatedType.Alternative, advancedSettings.CsvColumSeparator);
            }
            if (fieldMapping.Contains(EProductField.Videos))
            {
                var videos = ProductVideoService.GetProductVideos(product.ProductId);
                productCsv.Videos = ProductVideoService.VideoToString(videos);
            }
            if (fieldMapping.Contains(EProductField.Producer))
            {
                productCsv.Producer = BrandService.BrandToString(product.BrandId);
            }

            if (fieldMapping.Contains(EProductField.Unit))
            {
                productCsv.Unit = product.UnitId.HasValue
                    ? UnitService.UnitToString(product.UnitId.Value)
                    : string.Empty;
            }
            if (fieldMapping.Contains(EProductField.CustomOptions))
            {
                productCsv.CustomOptions =
                    CustomOptionsService.CustomOptionsToString(
                        CustomOptionsService.GetCustomOptionsByProductId(product.ProductId));
            }
            // AvitoProductProperties
            if (fieldMapping.Contains(EProductField.AvitoProductProperties))
            {
                productCsv.AvitoProductProperties = ExportFeedAvitoService.GetProductPropertiesInString(product.ProductId, advancedSettings.CsvColumSeparator, advancedSettings.CsvPropertySeparator);
            }
            if (fieldMapping.Contains(EProductField.Tags) &&
                (!SaasDataService.IsSaasEnabled || SaasDataService.CurrentSaasData.HaveTags))
            {
                var tags = TagService.Gets(product.ProductId, ETagType.Product);
                productCsv.Tags = tags.Select(item => item.Name).AggregateString(advancedSettings.CsvColumSeparator);
            }
            if (fieldMapping.Contains(EProductField.Gifts))
            {
                productCsv.Gifts = ProductGiftService.GiftsToString(product.ProductId,
                    advancedSettings.CsvColumSeparator);
            }
            if (fieldMapping.Contains(EProductField.Tax) && product.TaxId.HasValue)
            {
                var tax = TaxService.GetTax(product.TaxId.Value);
                if (tax != null)
                    productCsv.Tax = tax.Name;
            }

            if (fieldMapping.Contains(EProductField.ModifiedDate))
            {
                if (SettingsMain.TrackProductChanges)
                {
                    var lastChanges = ChangeHistoryService.GetLast(product.ProductId, ChangeHistoryObjType.Product);
                    productCsv.ModifiedDate = lastChanges != null ? Culture.ConvertDate(lastChanges.ModificationTime) : "";
                }
            }

            return productCsv;
        }
    }
}