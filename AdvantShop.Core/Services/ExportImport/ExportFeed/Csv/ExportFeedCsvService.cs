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
using AdvantShop.Saas;
using AdvantShop.SEO;
using AdvantShop.Taxes;

namespace AdvantShop.ExportImport
{
    public class ExportFeedCsvService
    {
        public static ExportFeedCsvProduct GetCsvProductsFromReader(SqlDataReader reader, ExportFeedSettings<ExportFeedCsvOptions> commonSettings)
        {
            var product = ProductService.GetProductFromReaderWithExportOptions(reader);

            var fieldMapping = commonSettings.AdvancedSettings.FieldMapping;

            var productCsv = new ExportFeedCsvProduct
            {
                ProductId = product.ProductId,
                ArtNo = product.ArtNo,
                Name = product.Name,
                UrlPath = product.UrlPath,
                Enabled = product.Enabled ? "+" : "-",
                ShippingPrice = product.ShippingPrice == null ? "" : product.ShippingPrice.Value.ToString("F2"),
                YandexDeliveryDays = product.ExportOptions.YandexDeliveryDays ?? "",
                Discount = product.Discount.Type == DiscountType.Percent ? product.Discount.Percent.ToString("F2") : "",
                DiscountAmount = product.Discount.Type == DiscountType.Amount ? product.Discount.Amount.ToString("F2") : "",
                BriefDescription = product.BriefDescription,
                Description = product.Description,
                OrderByRequest = product.AllowPreOrder ? "+" : "-",
                AccrueBonuses = product.AccrueBonuses ? "+" : "-",
                
                YandexSalesNote = product.ExportOptions.YandexSalesNote,
                Gtin = product.ExportOptions.Gtin,
                Mpn = product.ExportOptions.Mpn,
                GoogleProductCategory = product.ExportOptions.GoogleProductCategory,
                GoogleAvailabilityDate = product.ExportOptions.GoogleAvailabilityDate?.ToString("O", CultureInfo.InvariantCulture),
                YandexTypePrefix = product.ExportOptions.YandexTypePrefix,
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
                Adult = product.ExportOptions.Adult ? "+" : "-",
                ManufacturerWarranty = product.ExportOptions.ManufacturerWarranty ? "+" : "-",
                
                MinAmount = product.MinAmount == null ? "" : product.MinAmount.Value.ToString("F2"),
                MaxAmount = product.MaxAmount == null ? "" : product.MaxAmount.Value.ToString("F2"),
                Multiplicity = product.Multiplicity.ToString("F5").Trim('0'),
                Bid = product.ExportOptions.Bid.ToString(),
                YandexName = product.ExportOptions.YandexName ?? string.Empty,
                ManualRatio = product.ManualRatio == null ? "" : product.ManualRatio.Value.ToString("F2"),
                IsMarkingRequired = product.IsMarkingRequired ? "+" : "-",
                Comment = product.Comment,
                DoNotApplyOtherDiscounts = product.DoNotApplyOtherDiscounts ? "+" : "-",
                IsDigital = product.IsDigital ? "+" : "-",
                DownloadLink = product.DownloadLink,
                SizeChart = product.SizeChart?.Name
            };

            if (fieldMapping.Any(x => x == ProductFields.Weight || x == ProductFields.Size || 
                                                x == ProductFields.BarCode))
            {
                var offerForExport = OfferService.GetMainOfferForExport(product.ProductId);

                productCsv.Weight = (offerForExport?.GetWeight() ?? 0).ToFormatString();
                productCsv.Size = (offerForExport != null ? offerForExport.GetDimensions() : "0x0x0");
                productCsv.BarCode = offerForExport != null ? offerForExport.BarCode : string.Empty;
            }

            if (fieldMapping.Contains(ProductFields.Producer))
            {
                productCsv.Producer = BrandService.BrandToString(product.BrandId);
            }

            if (fieldMapping.Contains(ProductFields.Unit))
            {
                productCsv.Unit = product.UnitId.HasValue
                    ? UnitService.UnitToString(product.UnitId.Value)
                    : string.Empty;
            }

            if (fieldMapping.Contains(ProductFields.Category))
            {
                productCsv.Category = CategoryService.GetCategoryStringByProductId(product.ProductId, commonSettings.AdvancedSettings.CsvColumSeparator);
            }

            if (commonSettings.AdvancedSettings.CsvCategorySort)
            {
                productCsv.Sorting = CategoryService.GetCategoryStringByProductId(product.ProductId, commonSettings.AdvancedSettings.CsvColumSeparator, true);
            }

            if (fieldMapping.Contains(ProductFields.Markers))
            {
                productCsv.Markers = ProductService.MarkersToString(product.BestSeller, product.New,
                    product.Recomended, product.OnSale, commonSettings.AdvancedSettings.CsvColumSeparator);
            }

            if (fieldMapping.Contains(ProductFields.Photos))
            {
                productCsv.Photos =
                    PhotoService.PhotoToString(
                        PhotoService.GetPhotos<ProductPhoto>(product.ProductId, PhotoType.Product),
                        commonSettings.AdvancedSettings.CsvColumSeparator, commonSettings.AdvancedSettings.CsvPropertySeparator);
            }

            if (fieldMapping.Contains(ProductFields.Videos))
            {
                var videos = ProductVideoService.GetProductVideos(product.ProductId);
                productCsv.Videos = ProductVideoService.VideoToString(videos);
            }

            if (fieldMapping.Contains(ProductFields.Properties))
            {
                productCsv.Properties =
                    PropertyService.PropertiesToString(
                        PropertyService.GetPropertyValuesByProductId(product.ProductId),
                        commonSettings.AdvancedSettings.CsvColumSeparator, commonSettings.AdvancedSettings.CsvPropertySeparator);
            }

            if (fieldMapping.Contains(ProductFields.Related))
            {
                productCsv.Related =
                    ProductService.LinkedProductToString(product.ProductId,
                        RelatedType.Related, commonSettings.AdvancedSettings.CsvColumSeparator);
            }

            if (fieldMapping.Contains(ProductFields.Alternative))
            {
                productCsv.Alternative =
                    ProductService.LinkedProductToString(product.ProductId,
                        RelatedType.Alternative, commonSettings.AdvancedSettings.CsvColumSeparator);
            }

            if (fieldMapping.Contains(ProductFields.CustomOption))
            {
                productCsv.CustomOption =
                    CustomOptionsService.CustomOptionsToString(
                        CustomOptionsService.GetCustomOptionsByProductId(product.ProductId));
            }

            if (fieldMapping.Contains(ProductFields.Gifts))
            {
                productCsv.Gifts = ProductGiftService.GiftsToString(product.ProductId,
                    commonSettings.AdvancedSettings.CsvColumSeparator);
            }

            if (fieldMapping.Contains(ProductFields.Title) ||
                fieldMapping.Contains(ProductFields.H1) ||
                fieldMapping.Contains(ProductFields.MetaKeywords) ||
                fieldMapping.Contains(ProductFields.MetaDescription))
            {
                var meta = MetaInfoService.GetMetaInfo(productCsv.ProductId, MetaType.Product) ??
                           new MetaInfo(0, 0, MetaType.Product, string.Empty, string.Empty, string.Empty, string.Empty);

                productCsv.Title = meta.Title;
                productCsv.H1 = meta.H1;
                productCsv.MetaKeywords = meta.MetaKeywords;
                productCsv.MetaDescription = meta.MetaDescription;
            }

            if (fieldMapping.Contains(ProductFields.Price) ||
                fieldMapping.Contains(ProductFields.PurchasePrice) ||
                fieldMapping.Contains(ProductFields.Amount) ||
                fieldMapping.Contains(ProductFields.MultiOffer))
            {
                if (commonSettings.PriceMarginInPercents != 0 || commonSettings.PriceMarginInNumbers != 0)
                {
                    foreach (var offer in product.Offers)
                    {
                        var markup = offer.BasePrice * commonSettings.PriceMarginInPercents / 100 +
                                     commonSettings.PriceMarginInNumbers;

                        offer.BasePrice += markup;
                    }
                }

                if (!product.Offers.Any(offer => offer.ColorID.HasValue || offer.SizeID.HasValue || offer.ArtNo != product.ArtNo) &&
                    !commonSettings.AdvancedSettings.AllOffersToMultiOfferColumn)
                {
                    var offer = product.Offers.FirstOrDefault() ?? new Offer();
                    productCsv.Price = offer.BasePrice.ToString("F2");
                    productCsv.PurchasePrice = offer.SupplyPrice.ToString("F2");
                    
                    if ((commonSettings.AdvancedSettings.StocksFromWarehouses?.Count ?? 0) > 0)
                    {
                        if (commonSettings.AdvancedSettings.StocksFromWarehouses.Count == 1)
                            offer.SetAmountByWarehouse(commonSettings.AdvancedSettings.StocksFromWarehouses[0]);
                        else
                            offer.SetAmountByStocks(
                                Core.Services.Catalog.Warehouses.WarehouseStocksService.GetOfferStocks(offer.OfferId)
                                    .Where(x =>
                                         commonSettings.AdvancedSettings.StocksFromWarehouses.Contains(x.WarehouseId))
                            );
                    }
                    productCsv.Amount = offer.Amount.ToString(CultureInfo.CurrentCulture);
                    productCsv.MultiOffer = string.Empty;
                }
                else
                {
                    productCsv.Price = string.Empty;
                    productCsv.PurchasePrice = string.Empty;
                    productCsv.Amount = string.Empty;
                    
                    if ((commonSettings.AdvancedSettings.StocksFromWarehouses?.Count ?? 0) > 0)
                    {
                        var stocks =
                            Core.Services.Catalog.Warehouses.WarehouseStocksService.GetProductStocks(product.ProductId)
                                .Where(s => commonSettings.AdvancedSettings.StocksFromWarehouses.Contains(s.WarehouseId))
                                .ToList();

                        foreach (var offer in product.Offers)
                            offer.SetAmountByStocks(
                                stocks
                                   .Where(s => s.OfferId == offer.OfferId)
                            );
                    }
                    productCsv.MultiOffer = OfferService.OffersToString(product.Offers,
                        commonSettings.AdvancedSettings.CsvColumSeparator,
                        commonSettings.AdvancedSettings.CsvPropertySeparator);
                }
            }

            if (fieldMapping.Contains(ProductFields.Currency))
            {
                productCsv.Currency = CurrencyService.GetCurrency(product.CurrencyID).Iso3;
            }

            if (fieldMapping.Contains(ProductFields.Tags) &&
                (!SaasDataService.IsSaasEnabled || SaasDataService.CurrentSaasData.HaveTags))
            {
                var tags = TagService.Gets(product.ProductId, ETagType.Product);
                if (tags != null)
                {
                    productCsv.Tags =
                        tags.Select(item => item.Name).AggregateString(commonSettings.AdvancedSettings.CsvColumSeparator);
                }
            }

            if (fieldMapping.Contains(ProductFields.Tax))
            {
                if (product.TaxId != null)
                {
                    var tax = TaxService.GetTax(product.TaxId.Value);
                    if (tax != null)
                        productCsv.Tax = tax.Name;
                }
            }

            if (fieldMapping.Contains(ProductFields.PaymentMethodType))
            {
                productCsv.PaymentMethodType = product.PaymentMethodType.ToString();
            }

            if (fieldMapping.Contains(ProductFields.PaymentSubjectType))
            {
                productCsv.PaymentSubjectType = product.PaymentSubjectType.ToString();
            }

            //if (fieldMapping.Contains(ProductFields.Store) ||
            //    fieldMapping.Contains(ProductFields.Funnel) ||
            //    fieldMapping.Contains(ProductFields.Vk) ||
            //    fieldMapping.Contains(ProductFields.Instagram) ||
            //    fieldMapping.Contains(ProductFields.Yandex) ||
            //    fieldMapping.Contains(ProductFields.Avito) ||
            //    fieldMapping.Contains(ProductFields.Google) ||
            //    fieldMapping.Contains(ProductFields.Facebook) ||
            //    fieldMapping.Contains(ProductFields.Bonus) ||
            //    fieldMapping.Contains(ProductFields.Referal))
            //{
            //    var productEnableInSalesChannel = SalesChannelService.GetExcludedProductSalesChannelList(product.ProductId);
            //    productCsv.Store = productEnableInSalesChannel.Any(item => item == ESalesChannelsKeys.Store.ToString()) ? "-" : "+";
            //    productCsv.Funnel = productEnableInSalesChannel.Any(item => item == ESalesChannelsKeys.Funnel.ToString()) ? "-" : "+";
            //    productCsv.Vk = productEnableInSalesChannel.Any(item => item == ESalesChannelsKeys.Vk.ToString()) ? "-" : "+";
            //    productCsv.Instagram = productEnableInSalesChannel.Any(item => item == ESalesChannelsKeys.Instagram.ToString()) ? "-" : "+";
            //    productCsv.Yandex = productEnableInSalesChannel.Any(item => item == ESalesChannelsKeys.Yandex.ToString()) ? "-" : "+";
            //    productCsv.Avito = productEnableInSalesChannel.Any(item => item == ESalesChannelsKeys.Avito.ToString()) ? "-" : "+";
            //    productCsv.Google = productEnableInSalesChannel.Any(item => item == ESalesChannelsKeys.Google.ToString()) ? "-" : "+";
            //    productCsv.Facebook = productEnableInSalesChannel.Any(item => item == ESalesChannelsKeys.Facebook.ToString()) ? "-" : "+";
            //    productCsv.Bonus = productEnableInSalesChannel.Any(item => item == ESalesChannelsKeys.Bonus.ToString()) ? "-" : "+";
            //    productCsv.Referal = productEnableInSalesChannel.Any(item => item == ESalesChannelsKeys.Referal.ToString()) ? "-" : "+";
            //}

            return productCsv;
        }
    }
}