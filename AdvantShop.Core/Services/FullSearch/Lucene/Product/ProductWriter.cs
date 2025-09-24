using System;
using AdvantShop.Catalog;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.FullSearch.Core;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AdvantShop.Diagnostics;

namespace AdvantShop.Core.Services.FullSearch
{
    public class ProductWriter : BaseWriter<ProductDocument>
    {
        private static bool isRun = false;
        static readonly object locker = new object();
        private static Task _reindexTask;

        private const int BatchSize = 700;
        
        public ProductWriter()
            : base(string.Empty)
        {
        }
        public ProductWriter(string path)
            : base(path)
        {
        }

        public void AddUpdateToIndex(Product model)
        {
            AddUpdateItemsToIndex(new List<ProductDocument> { (ProductDocument)model });
        }

        public void AddUpdateToIndex(ProductLuceneDto model)
        {
            AddUpdateItemsToIndex(new List<ProductDocument> { (ProductDocument)model });
        }

        public void AddUpdateToIndex(List<Product> model)
        {
            AddUpdateItemsToIndex(model.Select(p => (ProductDocument)p).ToList());
        }

        public void DeleteFromIndex(Product model)
        {
            DeleteItemsFromIndex(new List<ProductDocument> { (ProductDocument)model });
        }

        public void DeleteFromIndex(int id)
        {
            DeleteItemsFromIndex(new List<ProductDocument> { new ProductDocument { Id = id } });
        }

        //static 
        public static void AddUpdate(Product model)
        {
            TryDo(() =>
            {
                using (var writer = new ProductWriter())
                    writer.AddUpdateToIndex(model);
            });
        }

        public static void AddUpdate(List<Product> model)
        {
            TryDo(() =>
            {
                using (var writer = new ProductWriter())
                    writer.AddUpdateToIndex(model);
            });
        }


        public static void Delete(Product model)
        {
            TryDo(() =>
            {
                using (var writer = new ProductWriter())
                    writer.DeleteFromIndex(model);
            });
        }

        public static void Delete(int id)
        {
            TryDo(() =>
            {
                using (var writer = new ProductWriter())
                    writer.DeleteFromIndex(id);
            });
        }

        public static Task CreateIndexInTask()
        {
            return Task.Factory.StartNew(CreateIndexFromDb, TaskCreationOptions.LongRunning);
        }

        public static void CreateIndex()
        {
            if (_reindexTask == null || _reindexTask.IsFaulted)
            {
                _reindexTask = CreateIndexInTask();
            }
            _reindexTask.Wait();
            _reindexTask = null;
        }

        public static void CreateIndexFromDb()
        {
            if (isRun) return;

            isRun = true;
            lock (locker)
            {
                try
                {
                    var basePath = BasePath(nameof(ProductDocument));
                    var tempPath = basePath + "_temp";
                    var mergePath = basePath + "_temp2";

                    var offers = GetOffers();
                    var tags = GetTags();
                    using (var writer = new ProductWriter(tempPath))
                    {
                        foreach (var item in GetProducts())
                        {
                            item.Offers = offers.TryGetValue(item.ProductId, out var offersList)
                                ? offersList
                                : new List<ProductOfferLuceneDto>();
                            
                            item.Tags = tags.TryGetValue(item.ProductId, out var tagsList)
                                ? tagsList
                                : new List<ProductTagLuceneDto>();
                            
                            writer.AddUpdateToIndex(item);
                        }
                        
                        writer.Optimize();
                    }

                    FileHelpers.CreateDirectory(basePath);

                    if (Directory.Exists(mergePath))
                        Directory.Delete(mergePath, true);

                    Directory.Move(basePath, mergePath);
                    Directory.Move(tempPath, basePath);
                    Directory.Delete(mergePath, true);
                }
                catch (Exception ex)
                {
                    Debug.Log.Error("ProductWriter CreateIndexFromDb", ex);
                }
            }
            isRun = false;
        }

        public static void CreateIndexByCategoryInTask(int categoryId)
        {
            Task.Factory.StartNew(() => CreateIndexByCategory(categoryId), TaskCreationOptions.LongRunning);
        }

        public static void CreateIndexByCategory(int categoryId)
        {
            if (isRun) return;

            isRun = true;
            lock (locker)
            {
                try
                {
                    var offers = GetOffersByCategory(categoryId);
                    var tags = GetTags();
                    
                    using (var writer = new ProductWriter())
                    {
                        foreach (var item in GetProducts(categoryId))
                        {
                            item.Offers = offers.TryGetValue(item.ProductId, out var offersList)
                                ? offersList
                                : new List<ProductOfferLuceneDto>();

                            item.Tags = tags.TryGetValue(item.ProductId, out var tagsList)
                                ? tagsList
                                : new List<ProductTagLuceneDto>();

                            writer.AddUpdateToIndex(item);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log.Error(ex);
                }
            }
            isRun = false;
        }

        private static IEnumerable<ProductLuceneDto> GetProducts(int? categoryId = null)
        {
            int? fromProductId = null;
            do
            {
                var products = GetProductsBatch(fromProductId, categoryId);
                
                fromProductId = products.LastOrDefault()?.ProductId;
                
                foreach (var product in products)
                    yield return product;
                
                if (products.Count < BatchSize)
                    break;
                
            } while (fromProductId != null);

            if (categoryId != null) 
                yield break;

            fromProductId = null;
            
            do
            {
                var productsWithoutCategory = GetProductsWithoutCategoryBatch(fromProductId);
                
                fromProductId = productsWithoutCategory.LastOrDefault()?.ProductId;
                
                foreach (var product in productsWithoutCategory)
                    yield return product;
                
                if (productsWithoutCategory.Count < BatchSize)
                    break;
                
            } while (fromProductId != null);
        }

        private static List<ProductLuceneDto> GetProductsBatch(int? fromProductId, int? categoryId = null)
        {
            // если кол-во категорий товара равно кол-ву, в которых он выключен, то скрываем
            var sql =
                "Select TOP(@Count) " +
                        "p.ProductId, p.ArtNo, p.Name, p.Description, p.Enabled, p.CategoryEnabled, p.AllowPreOrder, " +
                        "c2.Name as MainCategoryName, c3.Name as ParentCategoryName, " +
                        "case when pMap.HiddenCount=pMap.CatCount then 1 else 0 end [Hidden] " +
                "From [Catalog].[Product] p " +
                "Inner Join [Catalog].[ProductCategories] pc2 on pc2.ProductID = p.ProductId and pc2.Main = 1 " +
                "Inner Join [Catalog].[Category] c2 on pc2.CategoryID = c2.CategoryID " +
                "Inner Join [Catalog].[Category] c3 on c2.ParentCategory = c3.CategoryID " +
                "Inner Join (" +
                            "Select pc.ProductId, count(pc.ProductId) CatCount, sum(1 * c.Hidden) HiddenCount " +
                            "From [Catalog].[ProductCategories] pc " +
                            "Inner Join [Catalog].[Category] c ON pc.[CategoryId] = c.CategoryId " +
                            (categoryId.HasValue
                                ? "Inner Join [Catalog].[ProductCategories] pcProduct on pc.ProductId = pcProduct.ProductId and pcProduct.CategoryId = @categoryId "
                                : "") +
                            "Group by pc.ProductId " +
                            ") pMap on p.ProductId = pMap.ProductId " +
                (fromProductId != null 
                    ? "Where p.ProductId > @fromProductId "
                    : "") +
                "Order by p.ProductId asc";

            var sqlParams = new List<SqlParameter>() { new SqlParameter("@Count", BatchSize) };
            if (categoryId != null)
                sqlParams.Add(new SqlParameter("@categoryId", categoryId));
            
            if (fromProductId != null)
                sqlParams.Add(new SqlParameter("@fromProductId", fromProductId));

            var products =
                SQLDataAccess.ExecuteReadList(sql, CommandType.Text,
                    reader => new ProductLuceneDto()
                    {
                        ProductId = SQLDataHelper.GetInt(reader, "ProductId"),
                        ArtNo = SQLDataHelper.GetString(reader, "ArtNo"),
                        Name = SQLDataHelper.GetString(reader, "Name"),
                        Description = SQLDataHelper.GetString(reader, "Description"),
                        Enabled = SQLDataHelper.GetBoolean(reader, "Enabled"),
                        CategoryEnabled = SQLDataHelper.GetBoolean(reader, "CategoryEnabled"),
                        AllowPreOrder = SQLDataHelper.GetBoolean(reader, "AllowPreOrder"),
                        Hidden = SQLDataHelper.GetBoolean(reader, "Hidden"),
                        MainCategoryName = SQLDataHelper.GetString(reader, "MainCategoryName"),
                        ParentCategoryName = SQLDataHelper.GetString(reader, "ParentCategoryName"),
                    },
                    sqlParams.ToArray());

            return products;
        }

        private static List<ProductLuceneDto> GetProductsWithoutCategoryBatch(int? fromProductId)
        {
            var sql =
                "SELECT TOP(@Count) p.ProductId, p.ArtNo, p.Name, p.Description, p.Enabled, p.CategoryEnabled, p.AllowPreOrder, 1 as Hidden " +
                "FROM [Catalog].[Product] p " +
                "WHERE not Exists (Select 1 From [Catalog].[ProductCategories] Where [ProductCategories].[ProductId] = p.ProductId) " +
                (fromProductId != null 
                    ? " and p.ProductId > @fromProductId "
                    : "") +
                "Order by p.ProductId asc";
            
            var sqlParams = new List<SqlParameter>() { new SqlParameter("@Count", BatchSize) };
            if (fromProductId != null)
                sqlParams.Add(new SqlParameter("@fromProductId", fromProductId));

            var products =
                SQLDataAccess.ExecuteReadList(sql, CommandType.Text,
                    reader => new ProductLuceneDto()
                    {
                        ProductId = SQLDataHelper.GetInt(reader, "ProductId"),
                        ArtNo = SQLDataHelper.GetString(reader, "ArtNo"),
                        Name = SQLDataHelper.GetString(reader, "Name"),
                        Description = SQLDataHelper.GetString(reader, "Description"),
                        Enabled = SQLDataHelper.GetBoolean(reader, "Enabled"),
                        CategoryEnabled = SQLDataHelper.GetBoolean(reader, "CategoryEnabled"),
                        AllowPreOrder = SQLDataHelper.GetBoolean(reader, "AllowPreOrder"),
                        Hidden = SQLDataHelper.GetBoolean(reader, "Hidden"),
                    },
                    sqlParams.ToArray());

            return products;
        }

        private static Dictionary<int, List<ProductOfferLuceneDto>> GetOffers()
        {
            var offers =
                SQLDataAccess
                    .Query<ProductOfferLuceneDto>(
                        "SELECT o.ProductID, o.ArtNo, o.Amount FROM Catalog.Offer o")
                    .ToList()
                    .GroupBy(x => new { x.ProductId })
                    .ToDictionary(x => x.Key.ProductId, x => x.ToList());

            return offers;
        }
        
        private static Dictionary<int, List<ProductOfferLuceneDto>> GetOffersByCategory(int categoryId)
        {
            var offers =
                SQLDataAccess
                    .Query<ProductOfferLuceneDto>(
                        "SELECT o.ProductID, o.ArtNo, o.Amount " +
                        "FROM Catalog.Offer o " +
                        "Inner Join [Catalog].[ProductCategories] pc on pc.ProductId = o.ProductId and pc.CategoryId = @categoryId",
                        new {categoryId})
                    .ToList()
                    .GroupBy(x => new { x.ProductId })
                    .ToDictionary(x => x.Key.ProductId, x => x.ToList());

            return offers;
        }

        private static Dictionary<int, List<ProductTagLuceneDto>> GetTags()
        {
            var tags =
                SQLDataAccess
                    .Query<ProductTagLuceneDto>(
                        "SELECT TagMap.ObjId as ProductId, Name " +
                        "From Catalog.Tag " +
                        "Inner Join Catalog.TagMap on Tag.Id = TagMap.TagId " +
                        "Where TagMap.Type = @Type AND Enabled=1",
                        new { Type = ETagType.Product.ToString() })
                    .ToList()
                    .GroupBy(x => new { x.ProductId })
                    .ToDictionary(x => x.Key.ProductId, x => x.ToList());

            return tags;
        }

        private static void TryDo(Action action, bool isSecond = false)
        {
            if (isRun) return;

            lock (locker)
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    if (!isSecond && ex.Message != null &&
                        (ex.Message.Contains("Could not find file") || ex.Message.Contains("не найден")))
                    {
                        if (TryRecreateIndexes())
                        {
                            TryDo(action, true);
                            return;
                        }
                    }
                    Debug.Log.Warn(ex);
                }
            }
        }

        private static bool TryRecreateIndexes()
        {
            try
            {
                var basePath = BasePath(nameof(ProductDocument));

                FileHelpers.DeleteDirectory(basePath, false);

                CreateIndex();
            }
            catch (Exception ex)
            {
                Debug.Log.Warn(ex);
                return false;
            }

            return true;
        }
    }

    public class ProductLuceneDto
    {
        public int ProductId { get; set; }
        public string ArtNo { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Enabled { get; set; }
        public bool CategoryEnabled { get; set; }
        public bool AllowPreOrder { get; set; }
        public bool Hidden { get; set; }
        public string MainCategoryName { get; set; }
        public string ParentCategoryName { get; set; }
        public List<ProductOfferLuceneDto> Offers { get; set; }
        public List<ProductTagLuceneDto> Tags { get; set; }
    }

    public class ProductOfferLuceneDto
    {
        public int ProductId { get; set; }
        public string ArtNo { get; set; }
        public float Amount { get; set; }
    }

    public class ProductTagLuceneDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
    }
}