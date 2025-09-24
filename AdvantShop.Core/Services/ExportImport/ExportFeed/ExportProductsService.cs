using System.Collections.Generic;
using System.Data;
using System.Linq;
using AdvantShop.Configuration;
using AdvantShop.Core.SQL2;
using AdvantShop.Helpers;
using AdvantShop.Repository.Currencies;
using SQLDataAccess = AdvantShop.Core.SQL.SQLDataAccess;

namespace AdvantShop.ExportImport
{
    public class ExportProductsService<TProduct, TSettings>
        where TProduct : ExportProductModel
        where TSettings: IExportFeedFilterOptions
    {
        private enum EQueryType
        {
            GetProducts,
            GetCountOfProducts,
            GetOfferIds
        }

        private readonly int _exportFeedId;
        private readonly ExportFeedSettings<TSettings> _settings;
        private readonly TSettings _advancedSettings;

        private readonly int _batchSize;

        public ExportProductsService(int exportFeedId, ExportFeedSettings<TSettings> settings)
        {
            _exportFeedId = exportFeedId;
            _settings = settings;
            if (settings != null)
                _advancedSettings = settings.AdvancedSettings;
            _batchSize = SettingsExport.ExportOffersBatchSize;
        }

        public IEnumerable<TProduct> GetProducts(int productsCount)
        {
            int? fromProductId = null;
            int? fromOfferId = null;
            bool complete = false;
            int processedCount = 0;

            do
            {
                var products = GetProductsBatch(fromProductId, fromOfferId);
                fromProductId = products.LastOrDefault()?.ProductId;
                fromOfferId = products.LastOrDefault()?.OfferId;

                foreach (var productRow in products)
                {
                    yield return productRow;
                    if (++processedCount > productsCount)
                        break;
                }

                if (!fromProductId.HasValue || products.Count < _batchSize || processedCount > productsCount)
                    complete = true;

            } while (!complete);
        }

        private void SetQueryTables(SqlBuilder query, EQueryType queryType)
        {
            query.From("Catalog.Product p")
                .InnerJoin("Catalog.Offer o ON o.ProductID = p.ProductID" + (_advancedSettings.OnlyMainOfferToExport ? " AND o.Main = 1" : null))
                .LeftJoin("Catalog.ProductExportOptions exop ON exop.ProductId = p.ProductId")
                .LeftJoin("Settings.ExportFeedExcludedProducts exclP ON exclP.ProductId = p.ProductId AND exclP.ExportFeedId = {0}", _exportFeedId);
            
            if (_advancedSettings.PriceRuleId.HasValue)
                query.InnerJoin("Catalog.OfferPriceRule opr ON opr.OfferId = o.OfferID AND opr.PriceRuleId = {0}", _advancedSettings.PriceRuleId.Value);
            
            if (_advancedSettings.PriceRuleIdForOldPrice.HasValue)
                query.LeftJoin("Catalog.OfferPriceRule opr_old_price ON opr_old_price.OfferId = o.OfferID AND opr_old_price.PriceRuleId = {0}", _advancedSettings.PriceRuleIdForOldPrice.Value);

            if (queryType == EQueryType.GetProducts ||
                _advancedSettings.PriceFrom != null || _advancedSettings.PriceTo != null)
            {
                query.InnerJoin("Catalog.Currency ON Currency.CurrencyID = p.CurrencyID");
            }

            if (queryType == EQueryType.GetProducts)
            {
                query
                    .LeftJoin("Catalog.Color ON Color.ColorID = o.ColorID")
                    .LeftJoin("Catalog.Size ON Size.SizeID = o.SizeID")
                    .LeftJoin("Catalog.Brand ON Brand.BrandID = p.BrandID")
                    .LeftJoin("Customers.Country as bCountry ON Brand.CountryID = bCountry.CountryID")
                    .LeftJoin("Customers.Country as bmCountry ON Brand.CountryOfManufactureID = bmCountry.CountryID")
                    .CrossApply("(SELECT TOP (1) pc.CategoryId FROM Catalog.ProductCategories pc " +
                        "INNER JOIN Settings.ExportFeedCategoriesCache catCache ON catCache.CategoryId = pc.CategoryID AND catCache.ExportFeedId = {0} " +
                            "WHERE pc.ProductID = p.ProductID " +
                            "ORDER BY pc.Main DESC, pc.CategoryId) crossCategory", _exportFeedId);
            }
            else
            {
                query.InnerJoin("Catalog.ProductCategories pc ON pc.ProductID = p.ProductID")
                    .InnerJoin("Settings.ExportFeedCategoriesCache catCache ON catCache.CategoryId = pc.CategoryID AND catCache.ExportFeedId = {0}", _exportFeedId);
            }

            if (_advancedSettings.ConsiderMultiplicityInPrice)
                query.LeftJoin("Catalog.Units on Units.Id = p.Unit");

            if (_advancedSettings.WarehouseIds != null && _advancedSettings.WarehouseIds.Count > 0)
                query.OuterApply(
                    "(" +
                    "SELECT isnull(Sum(ws.Quantity), 0) AS AmountByWarehouses " +
                    "FROM [Catalog].[WarehouseStocks] AS ws Where ws.[OfferId] = o.[OfferID] AND ws.[WarehouseId] IN ({0})" +
                    ") wstocks", _advancedSettings.WarehouseIds.ToArray());
        }

        private void SetQueryFilter(SqlBuilder query, EQueryType queryType)
        {
            var hasWarehouseIds = _advancedSettings.WarehouseIds != null && _advancedSettings.WarehouseIds.Count > 0;
                
            if (!_advancedSettings.ExportNotAvailable)
            {
                query.Where("p.Enabled = 1")
                    .Where("p.CategoryEnabled = 1");
                
                if (!_advancedSettings.NotExportAmountCount.HasValue)
                {
                    query.Where(
                        _advancedSettings.AllowPreOrderProducts
                            ? hasWarehouseIds 
                                ? "(wstocks.AmountByWarehouses > 0 OR p.AllowPreOrder = 1)" 
                                : "(o.Amount > 0 OR p.AllowPreOrder = 1)"
                            : hasWarehouseIds 
                                ? "wstocks.AmountByWarehouses > 0" 
                                : "o.Amount > 0"
                        );
                }
                query.Where("o.Price > 0");
            }

            if (_advancedSettings.NotExportAmountCount.HasValue)
                query.Where(
                    !hasWarehouseIds 
                        ? "o.Amount >= {0}" 
                        : "wstocks.AmountByWarehouses >= {0}",
                    _advancedSettings.NotExportAmountCount ?? 0);
            
            if (!_settings.ExportAllProducts)
                query.Where("exclP.ProductID IS NULL");
            
            if (!_settings.ExportAdult)
                query.Where("(exop.Adult IS NULL OR exop.Adult = 0)");

            if (_advancedSettings.DontExportProductsWithoutDimensionsAndWeight)
            {
                query.Where("o.Width IS NOT NULL AND o.Width != 0")
                    .Where("o.Height IS NOT NULL AND o.Height != 0")
                    .Where("o.Length IS NOT NULL AND o.Length != 0")
                    .Where("o.Weight IS NOT NULL AND o.Weight != 0");
            }

            if (_advancedSettings.PriceFrom != null || _advancedSettings.PriceTo != null)
            {
                var currency = CurrencyService.GetCurrencyByIso3(_advancedSettings.Currency);

                if (_advancedSettings.PriceRuleId == null)
                {
                    if (_advancedSettings.PriceFrom != null)
                        query.Where("Round(o.Price/{1}*Currency.CurrencyValue, 2) >= {0}",
                            _advancedSettings.PriceFrom.Value, currency.Rate);

                    if (_advancedSettings.PriceTo != null)
                        query.Where("Round(o.Price/{1}*Currency.CurrencyValue, 2) <= {0}",
                            _advancedSettings.PriceTo.Value, currency.Rate);
                }
                else
                {
                    if (_advancedSettings.PriceFrom != null)
                        query.Where("(opr.PriceByRule is not null AND Round(opr.PriceByRule/{1}*Currency.CurrencyValue, 2) >= {0})",
                            _advancedSettings.PriceFrom.Value, currency.Rate);

                    if (_advancedSettings.PriceTo != null)
                        query.Where("(opr.PriceByRule is not null AND Round(opr.PriceByRule/{1}*Currency.CurrencyValue, 2) <= {0})",
                            _advancedSettings.PriceTo.Value, currency.Rate);
                }
            }
        }

        private List<TProduct> GetProductsBatch(int? fromProductId, int? fromOfferId)
        {
            var query = new SqlBuilder()
                .Top(_batchSize)
                .Select(
                    "p.ProductID",
                    "p.ArtNo",
                    "p.Enabled",
                    "p.Discount",
                    "p.DiscountAmount",
                    "p.AllowPreOrder",
                    "p.ShippingPrice",
                    "p.Name",
                    "p.UrlPath",
                    "p.Description",
                    "p.BriefDescription",
                    "p.MinAmount",
                    "p.Multiplicity",
                    "p.IsDigital",
                    "p.IsMarkingRequired",
                    "o.OfferId",
                    "o.ArtNo".AsSqlField("OfferArtNo"),
                    "o.Amount",
                    "o.Price",
                    "o.Main",
                    "o.ColorID",
                    "o.SizeID",
                    "o.Length",
                    "o.Width",
                    "o.Height",
                    "o.Weight",
                    "o.SupplyPrice",
                    "o.BarCode",
                    "crossCategory.CategoryId".AsSqlField("ParentCategory"),
                    "Color.ColorName",
                    "Size.SizeName",
                    "Brand.BrandName",
                    "bCountry.CountryName".AsSqlField("BrandCountry"),
                    "bmCountry.CountryName".AsSqlField("BrandCountryManufacture"),
                    "Currency.CurrencyValue",
                    "Settings.PhotoToString(o.ColorID, p.ProductId)".AsSqlField("Photos"),
                    "exop.Adult",
                    "exop.Gtin",
                    "exop.Mpn",
                    "exop.GoogleProductCategory",
                    "exop.GoogleAvailabilityDate",
                    "exop.YandexSalesNote",
                    "exop.YandexTypePrefix",
                    "exop.YandexModel",
                    "exop.YandexName",
                    "exop.YandexDeliveryDays",
                    "exop.YandexSizeUnit",
                    "exop.YandexProductDiscounted",
                    "exop.YandexProductDiscountCondition",
                    "exop.YandexProductDiscountReason",
                    "exop.YandexMarketCategory",
                    "exop.ManufacturerWarranty",
                    "exop.Bid",
                    "exop.YandexMarketExpiry",
                    "exop.YandexMarketWarrantyDays",
                    "exop.YandexMarketCommentWarranty",
                    "exop.YandexMarketPeriodOfValidityDays",
                    "exop.YandexMarketServiceLifeDays",
                    "exop.YandexMarketTnVedCode",
                    "exop.YandexMarketStepQuantity",
                    "exop.YandexMarketMinQuantity",
                    "exop.YandexProductQuality",
                    "exop.YandexMarketCategoryId"
                );

            if (_advancedSettings.PriceRuleId.HasValue)
                query.Select("opr.PriceByRule".AsSqlField("PriceByRule"));
            
            if (_advancedSettings.PriceRuleIdForOldPrice.HasValue)
                query.Select("opr_old_price.PriceByRule".AsSqlField("OldPriceByRule"));

            if (_advancedSettings.ConsiderMultiplicityInPrice)
                query.Select("Units.DisplayName".AsSqlField("Unit"));

            if (_advancedSettings.WarehouseIds != null && _advancedSettings.WarehouseIds.Count > 0)
                query.Select("wstocks.AmountByWarehouses");

            SetQueryTables(query, EQueryType.GetProducts);

            if (fromProductId.HasValue)
                query.Where("(p.ProductId > {0} OR (p.ProductId = {0} AND o.OfferID > {1}))", fromProductId, fromOfferId);

            SetQueryFilter(query, EQueryType.GetProducts);

            query.OrderBy("p.ProductId")
                .OrderBy("o.OfferId");


            return SQLDataAccess.Query<TProduct>(
                query.GetQuery(),
                query.GetSqlParamsObject(),
                CommandType.Text
                ).ToList();
        }

        public HashSet<int> GetOfferIds()
        {
            var query = new SqlBuilder()
                .Select("DISTINCT o.OfferId");
            
            SetQueryTables(query, EQueryType.GetOfferIds);

            SetQueryFilter(query, EQueryType.GetOfferIds);

            return SQLDataAccess.ExecuteReadHashSet<int>(
                query.GetQuery(),
                CommandType.Text,
                60 * 3,
                reader => SQLDataHelper.GetInt(reader, "OfferId"),
                query.GetSqlParams());
        }

        public int GetProductsCount()
        {
            var query = new SqlBuilder()
                .Select("COUNT(DISTINCT o.OfferId)");

            SetQueryTables(query, EQueryType.GetCountOfProducts);

            SetQueryFilter(query, EQueryType.GetCountOfProducts);

            return SQLDataAccess.ExecuteScalar<int>(
                query.GetQuery(),
                CommandType.Text,
                60 * 3,
                query.GetSqlParams());
        }
    }
}