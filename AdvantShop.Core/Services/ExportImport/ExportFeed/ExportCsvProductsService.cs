using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.SQL;
using AdvantShop.Core.SQL2;

namespace AdvantShop.ExportImport
{
    public class ExportCsvProductsService<TProduct, TSettings>
        where TProduct : ExportProductModel
        where TSettings: IExportFeedCsvFilterOptions
    {
        private readonly int _exportFeedId;
        private readonly ExportFeedSettings<TSettings> _settings;
        private readonly TSettings _advancedSettings;

        private readonly int _batchSize;

        public ExportCsvProductsService(int exportFeedId, ExportFeedSettings<TSettings> settings)
        {
            _exportFeedId = exportFeedId;
            _settings = settings;
            if (settings != null)
                _advancedSettings = settings.AdvancedSettings;
            _batchSize = SettingsExport.ExportProductsBatchSize;
        }

        public IEnumerable<TProduct> GetProducts(int productsCount, Func<SqlDataReader, TProduct> getProductFromReader = null)
        {
            foreach (var product in GetProducts(productsCount, false, getProductFromReader))
                yield return product;

            if (_advancedSettings.CsvExportNoInCategory)
            {
                foreach (var product in GetProducts(productsCount, true, getProductFromReader))
                    yield return product;
            }
        }

        private IEnumerable<TProduct> GetProducts(int productsCount, bool withoutCategories,
            Func<SqlDataReader, TProduct> getProductFromReader = null)
        {
            int? fromProductId = null;
            bool complete = false;
            int processedCount = 0;

            do
            {
                var products = GetProductsBatch(fromProductId, withoutCategories, getProductFromReader);
                fromProductId = products.LastOrDefault()?.ProductId;

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

        private List<TProduct> GetProductsBatch(int? fromProductId, bool withoutCategories, 
            Func<SqlDataReader, TProduct> getProductFromReader = null)
        {
            var query = new SqlBuilder()
                .Top(_batchSize)
                .Select(
                    "p.*",
                    "exop.*",
                    "Photo.PhotoName"
                    );

            SetQueryTables(query, false, withoutCategories);

            if (fromProductId.HasValue)
                query.Where("p.ProductId > {0}", fromProductId);

            SetQueryFilter(query, false, withoutCategories);

            query.OrderBy("p.ProductId");

            var sqlCommand = query.GetQuery();

            if (getProductFromReader != null)
            {
                return SQLDataAccess.ExecuteReadList(sqlCommand, CommandType.Text,
                    getProductFromReader,
                    query.GetSqlParams());
            }

            return SQLDataAccess.Query<TProduct>(
                sqlCommand,
                query.GetSqlParamsObject(),
                CommandType.Text).ToList();
        }

        private void SetQueryTables(SqlBuilder query, bool onlyCount, bool withoutCategories)
        {
            query.From("Catalog.Product p")
                .LeftJoin("Catalog.ProductExportOptions exop ON exop.ProductId = p.ProductId")
                .LeftJoin("Settings.ExportFeedExcludedProducts exclP ON exclP.ProductId = p.ProductId AND exclP.ExportFeedId = {0}", _exportFeedId);

            if (!onlyCount)
            {
                query.LeftJoin("Catalog.Photo ON Photo.ObjId = p.ProductId AND [Type] = {0} AND Photo.Main = 1", PhotoType.Product.ToString());
            }
            else if (withoutCategories) // количество товаров без категории
            {
                query.LeftJoin("Catalog.ProductCategories leftpc on leftpc.ProductID = p.ProductId and leftpc.Main = 1");
            }
            else  // количество товаров в категориях
            {
                query.InnerJoin("Catalog.ProductCategories pc on pc.ProductID = p.ProductId" +
                                (_advancedSettings.ExportFromMainCategories == true ? " AND pc.Main = 1" : null))
                    .InnerJoin("Settings.ExportFeedCategoriesCache catCache ON catCache.CategoryId = pc.CategoryID AND catCache.ExportFeedId = {0}", _exportFeedId);
            }
        }

        private void SetQueryFilter(SqlBuilder query, bool onlyCount, bool withoutCategories)
        {
            if (!_advancedSettings.ExportNotAvailable)
            {
                query.Where("p.Enabled = 1");
                query.Where("EXISTS (SELECT 1 FROM Catalog.Offer o WHERE o.ProductId = p.ProductId AND o.Price > 0 AND o.Amount > 0)");
            }
            if (!onlyCount)
            {
                if (withoutCategories) // товары без категории
                {
                    query.Where("NOT EXISTS (SELECT 1 FROM Catalog.ProductCategories pc WHERE pc.ProductID = p.ProductID AND pc.Main = 1)");
                }
                else // товары в выбранных категориях
                {
                    query.Where("EXISTS (SELECT 1 FROM Catalog.ProductCategories pc " +
                        "INNER JOIN Settings.ExportFeedCategoriesCache catCache ON catCache.CategoryId = pc.CategoryID AND catCache.ExportFeedId = {0} " +
                        "WHERE pc.ProductID = p.ProductID" +
                        (_advancedSettings.ExportFromMainCategories ? " AND pc.Main = 1" : null) +
                        ")", _exportFeedId);
                }
            }
            else if (withoutCategories) // количество товаров без категории
            {
                query.Where("leftpc.ProductID IS NULL");
            }
            if (!_settings.ExportAllProducts)
                query.Where("exclP.ProductID IS NULL");
            if (!_settings.ExportAdult)
                query.Where("(exop.Adult IS NULL OR exop.Adult = 0)");
        }

        public int GetProductsCount()
        {
            var count = GetProductsCount(false);

            if (_advancedSettings.CsvExportNoInCategory)
                count += GetProductsCount(true);

            return count;
        }

        private int GetProductsCount(bool withoutCategories)
        {
            var query = new SqlBuilder()
                .Select("COUNT(DISTINCT p.ProductId)");

            SetQueryTables(query, true, withoutCategories);

            SetQueryFilter(query, true, withoutCategories);

            return SQLDataAccess.ExecuteScalar<int>(
                query.GetQuery(),
                CommandType.Text,
                60 * 3,
                query.GetSqlParams());
        }
    }
}