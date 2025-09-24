using AdvantShop.Core.SQL;
using AdvantShop.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace AdvantShop.Catalog
{
    public class SizeChartService
    {
        public static SizeChart Get(int sizeChartId)
        {
            return SQLDataAccess.ExecuteReadOne("SELECT TOP 1 * FROM Catalog.SizeChart WHERE Id=@Id",
                        CommandType.Text, 
                        GetFromReader,
                        new SqlParameter("@Id", sizeChartId));
        }

        public static SizeChart GetByName(string name)
        {
            return SQLDataAccess.ExecuteReadOne("SELECT TOP 1 * FROM Catalog.SizeChart WHERE Name=@Name",
                        CommandType.Text,
                        GetFromReader,
                        new SqlParameter("@Name", name));
        }

        public static List<SizeChart> GetAll(bool enabled = false)
        {
            return SQLDataAccess.ExecuteReadList("SELECT * FROM Catalog.SizeChart" + (enabled ? " where Enabled = 1" : string.Empty + " ORDER BY SortOrder"), 
                CommandType.Text, GetFromReader);
        }

        private static SizeChart GetFromReader(SqlDataReader reader)
        {
            return new SizeChart
            {
                Id = SQLDataHelper.GetInt(reader, "Id"),
                Name = SQLDataHelper.GetString(reader, "Name"),
                ModalHeader = SQLDataHelper.GetString(reader, "ModalHeader"),
                LinkText = SQLDataHelper.GetString(reader, "LinkText"),
                SourceType = (ESizeChartSourceType)SQLDataHelper.GetInt(reader, "SourceType"),
                Text = SQLDataHelper.GetString(reader, "Text"),
                Enabled = SQLDataHelper.GetBoolean(reader, "Enabled"),
                SortOrder = SQLDataHelper.GetInt(reader, "SortOrder")
            };
        }

        public static int Add(SizeChart sizeChart)
        {
            sizeChart.Id = SQLDataAccess.ExecuteScalar<int>(
                "INSERT INTO [Catalog].[SizeChart] (Name, LinkText, SourceType, Text, Enabled, SortOrder, ModalHeader) VALUES (@Name, @LinkText, @SourceType, @Text, @Enabled, @SortOrder, @ModalHeader); SELECT SCOPE_IDENTITY();", 
                CommandType.Text,
                new SqlParameter("@Name", sizeChart.Name),
                new SqlParameter("@ModalHeader", sizeChart.ModalHeader ?? (object)DBNull.Value),
                new SqlParameter("@LinkText", sizeChart.LinkText),
                new SqlParameter("@SourceType", (int)sizeChart.SourceType),
                new SqlParameter("@Text", sizeChart.Text),
                new SqlParameter("@Enabled", sizeChart.Enabled),
                new SqlParameter("@SortOrder", sizeChart.SortOrder));

            return sizeChart.Id;
        }

        public static void Update(SizeChart sizeChart)
        {
            SQLDataAccess.ExecuteNonQuery(
                "UPDATE [Catalog].[SizeChart] SET Name = @Name, LinkText = @LinkText, SourceType = @SourceType, Text = @Text, Enabled = @Enabled, SortOrder = @SortOrder, ModalHeader = @ModalHeader WHERE Id = @Id", 
                CommandType.Text,
                new SqlParameter("@Id", sizeChart.Id),
                new SqlParameter("@Name", sizeChart.Name),
                new SqlParameter("@ModalHeader", sizeChart.ModalHeader ?? (object)DBNull.Value),
                new SqlParameter("@LinkText", sizeChart.LinkText),
                new SqlParameter("@SourceType", (int)sizeChart.SourceType),
                new SqlParameter("@Text", sizeChart.Text),
                new SqlParameter("@Enabled", sizeChart.Enabled),
                new SqlParameter("@SortOrder", sizeChart.SortOrder));
        }

        public static void Delete(int sizeChartId)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM Catalog.SizeChart WHERE Id = @Id", CommandType.Text, new SqlParameter("@Id", sizeChartId));
        }

        #region SizeChartMap

        public static List<SizeChart> Get(int objId, ESizeChartEntityType objType, bool enabled = false)
        {
            return SQLDataAccess.ExecuteReadList(
                        @"SELECT * FROM Catalog.SizeChart
                        INNER JOIN Catalog.SizeChartMap map ON map.SizeChartId = SizeChart.Id
                        WHERE map.ObjId = @ObjId AND map.ObjType = @ObjType" + (enabled ? " AND [SizeChart].[Enabled] = 1" : string.Empty),
                        CommandType.Text, 
                        GetFromReader,
                        new SqlParameter("@ObjId", objId),
                        new SqlParameter("@ObjType", (int)objType));
        }

        public static List<SizeChart> GetFilteredSizeChart(int objId, ESizeChartEntityType objType, int? brandId = null, List<int> propertyValueIds = null, bool enabled = false)
        {
            var conditions = new List<string>
            {
                "map.ObjId = @ObjId",
                "map.ObjType = @ObjType"
            };
            var sqlParams = new List<SqlParameter>
            {
                new SqlParameter("@ObjId", objId),
                new SqlParameter("@ObjType", (int)objType)
            };
            if (propertyValueIds != null)
            {
                if (propertyValueIds.Count > 0)
                {
                    conditions.Add(
                        "(NOT EXISTS(SELECT * FROM [Catalog].[SizeChartPropertyValue] prop WHERE SizeChart.Id = prop.SizeChartId) " +
                        $"OR EXISTS(SELECT * FROM [Catalog].[SizeChartPropertyValue] prop WHERE SizeChart.Id = prop.SizeChartId AND [PropertyValueId] IN ({string.Join(",", propertyValueIds)})))");
                }
                else
                {
                    conditions.Add("NOT EXISTS(SELECT * FROM [Catalog].[SizeChartPropertyValue] prop WHERE SizeChart.Id = prop.SizeChartId)");
                }
            }
            if (brandId.HasValue)
            {
                if (brandId > 0)
                {
                    sqlParams.Add(new SqlParameter("@BrandId", brandId));
                    conditions.Add(
                        @"(NOT EXISTS(SELECT * FROM [Catalog].[SizeChartBrand] brand WHERE SizeChart.Id = brand.SizeChartId)
                        OR EXISTS(SELECT * FROM [Catalog].[SizeChartBrand] brand WHERE SizeChart.Id = brand.SizeChartId AND BrandId = @BrandId))");
                }
                else
                {
                    conditions.Add("NOT EXISTS(SELECT * FROM [Catalog].[SizeChartBrand] brand WHERE SizeChart.Id = brand.SizeChartId)");
                }
            }
            if (enabled)
                conditions.Add("[SizeChart].[Enabled] = 1");
            return SQLDataAccess.ExecuteReadList(
                        @"SELECT * FROM Catalog.SizeChart
                        INNER JOIN Catalog.SizeChartMap map ON map.SizeChartId = SizeChart.Id
                        WHERE " + string.Join(" AND ", conditions),
                        CommandType.Text,
                        GetFromReader,
                        sqlParams.ToArray());
        }

        public static List<int> GetObjIdsForSizeChart(ESizeChartEntityType objType, int sizeChartId)
        {
            return SQLDataAccess.ExecuteReadColumn<int>(
                        @"SELECT ObjId FROM Catalog.SizeChartMap
                        WHERE SizeChartId = @SizeChartId AND ObjType = @ObjType ",
                        CommandType.Text,
                        "ObjId",
                        new SqlParameter("@SizeChartId", sizeChartId),
                        new SqlParameter("@ObjType", (int)objType));
        }

        public static void AddUpdateMap(int sizeChartId, int objId, ESizeChartEntityType objType)
        {
            SQLDataAccess.ExecuteNonQuery(
                @"IF EXISTS (SELECT * FROM [Catalog].[SizeChartMap] WHERE ObjId = @ObjId AND ObjType = @ObjType)
                    UPDATE [Catalog].[SizeChartMap] SET SizeChartId = @SizeChartId WHERE ObjId = @ObjId AND ObjType = @ObjType
                ELSE 
                    INSERT INTO [Catalog].[SizeChartMap] (SizeChartId, ObjId, ObjType) VALUES (@SizeChartId, @ObjId, @ObjType)",
                CommandType.Text,
                new SqlParameter("@SizeChartId", sizeChartId),
                new SqlParameter("@ObjId", objId),
                new SqlParameter("@ObjType", (int)objType));
        }

        public static void ReplaceAllMapsInSizeChart(int sizeChartId, List<int> objIds, ESizeChartEntityType objType)
        {
            DeleteMapForSizeChart(sizeChartId, objType);
            if (objIds == null || objIds.Count == 0)
                return;
            var tableBulk = SQLDataAccess.ExecuteTable(@"SELECT * FROM [Catalog].[SizeChartMap] WHERE SizeChartId = @SizeChartId AND ObjType = @ObjType",
                CommandType.Text,
                new SqlParameter("@SizeChartId", sizeChartId),
                new SqlParameter("@ObjType", (int)objType));
            var addedObjIds = new List<int>();
            foreach (var objId in objIds)
            {
                if (addedObjIds.Contains(objId))
                    continue;
                addedObjIds.Add(objId);
                var row = tableBulk.NewRow();

                row.SetField("SizeChartId", sizeChartId);
                row.SetField("ObjId", objId);
                row.SetField("ObjType", (int)objType);

                tableBulk.Rows.Add(row);

                if (tableBulk.Rows.Count % 100 == 0)
                    InsertBulk(tableBulk, "[Catalog].[SizeChartMap]");
            }
            if (tableBulk.Rows.Count > 0)
                InsertBulk(tableBulk, "[Catalog].[SizeChartMap]");
        }

        public static void DeleteMap(int objId, ESizeChartEntityType objType)
        {
            SQLDataAccess.ExecuteNonQuery(
                "DELETE FROM Catalog.SizeChartMap WHERE ObjId = @ObjId AND ObjType = @ObjType", 
                CommandType.Text,
                new SqlParameter("@ObjId", objId),
                new SqlParameter("@ObjType", (int)objType));
        }

        public static void DeleteMapForSizeChart(int sizeChartId, ESizeChartEntityType? objType = null)
        {
            SQLDataAccess.ExecuteNonQuery(
                "DELETE FROM Catalog.SizeChartMap WHERE SizeChartId = @SizeChartId" + (objType.HasValue ? " AND ObjType = " + (int)objType.Value : null),
                CommandType.Text,
                new SqlParameter("@SizeChartId", sizeChartId));
        }

        #endregion

        #region SizeChartBrands

        public static List<int> GetSizeChartBrandIds(int sizeChartId)
        {
            return SQLDataAccess.ExecuteReadColumn<int>("SELECT BrandId FROM Catalog.SizeChartBrand WHERE SizeChartId = @SizeChartId",
                CommandType.Text, "BrandId", new SqlParameter("@SizeChartId", sizeChartId));
        }

        public static void ReplaceSizeChartBrands(int sizeChartId, List<int> brandIds)
        {
            DeleteSizeChartBrands(sizeChartId);
            if (brandIds == null || brandIds.Count == 0)
                return;
            var tableBulk = SQLDataAccess.ExecuteTable(@"SELECT * FROM Catalog.SizeChartBrand WHERE SizeChartId = @SizeChartId",
                CommandType.Text,
                new SqlParameter("@SizeChartId", sizeChartId));
            var addedBrandIds = new List<int>();
            foreach (var brandId in brandIds)
            {
                if (addedBrandIds.Contains(brandId))
                    continue;
                addedBrandIds.Add(brandId);
                var row = tableBulk.NewRow();

                row.SetField("SizeChartId", sizeChartId);
                row.SetField("BrandId", brandId);

                tableBulk.Rows.Add(row);

                if (tableBulk.Rows.Count % 100 == 0)
                    InsertBulk(tableBulk, "[Catalog].[SizeChartBrand]");
            }
            if (tableBulk.Rows.Count > 0)
                InsertBulk(tableBulk, "[Catalog].[SizeChartBrand]");
        }

        public static void DeleteSizeChartBrands(int sizeChartId)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM Catalog.SizeChartBrand WHERE SizeChartId = @Id", CommandType.Text, new SqlParameter("@Id", sizeChartId));
        }

        #endregion

        #region SizeChartPropertyValues

        public static List<SizeChartPropertyValue> GetSizeChartPropertyValues(int sizeChartId)
        {
            return SQLDataAccess.ExecuteReadList(
                @"SELECT sch.PropertyValueId, PropertyValue.Value as PropertyValueName, Property.Name as PropertyName
                FROM Catalog.SizeChartPropertyValue sch
                INNER JOIN [Catalog].[PropertyValue] ON PropertyValue.PropertyValueID = sch.PropertyValueId
                INNER JOIN [Catalog].[Property] ON PropertyValue.PropertyID = Property.PropertyID
                WHERE SizeChartId = @SizeChartId",
                CommandType.Text, (reader) => new SizeChartPropertyValue
                {
                    PropertyValueId = SQLDataHelper.GetInt(reader, "PropertyValueId"),
                    PropertyName = SQLDataHelper.GetString(reader, "PropertyName"),
                    PropertyValueName = SQLDataHelper.GetString(reader, "PropertyValueName"),
                }, new SqlParameter("@SizeChartId", sizeChartId));
        }

        public static void ReplaceSizeChartPropertyValues(int sizeChartId, List<int> propertyValueIds)
        {
            DeleteSizeChartPropertyValues(sizeChartId);
            if (propertyValueIds == null || propertyValueIds.Count == 0)
                return;
            var tableBulk = SQLDataAccess.ExecuteTable(@"SELECT * FROM Catalog.SizeChartPropertyValue WHERE SizeChartId = @SizeChartId",
                CommandType.Text,
                new SqlParameter("@SizeChartId", sizeChartId));
            var addedBrandIds = new List<int>();
            foreach (var propertyValueId in propertyValueIds)
            {
                if (addedBrandIds.Contains(propertyValueId))
                    continue;
                addedBrandIds.Add(propertyValueId);
                var row = tableBulk.NewRow();

                row.SetField("SizeChartId", sizeChartId);
                row.SetField("PropertyValueId", propertyValueId);

                tableBulk.Rows.Add(row);

                if (tableBulk.Rows.Count % 100 == 0)
                    InsertBulk(tableBulk, "[Catalog].[SizeChartPropertyValue]");
            }
            if (tableBulk.Rows.Count > 0)
                InsertBulk(tableBulk, "[Catalog].[SizeChartPropertyValue]");
        }

        public static void DeleteSizeChartPropertyValues(int sizeChartId)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM Catalog.SizeChartPropertyValue WHERE SizeChartId = @Id", CommandType.Text, new SqlParameter("@Id", sizeChartId));
        }

        #endregion

        private static void InsertBulk(DataTable data, string tableName)
        {
            if (data.Rows.Count > 0)
            {
                using (SqlConnection dbConnection = new SqlConnection(Connection.GetConnectionString()))
                {
                    dbConnection.Open();
                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(dbConnection))
                    {
                        sqlBulkCopy.DestinationTableName = tableName;
                        sqlBulkCopy.WriteToServer(data);
                        data.Rows.Clear();
                    }
                    dbConnection.Close();
                }
            }
        }

    }
}
