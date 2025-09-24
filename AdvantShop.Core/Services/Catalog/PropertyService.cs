//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.SqlClient;
using System.Text;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.SQL;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Configuration;

namespace AdvantShop.Catalog
{
    public class PropertyService
    {
        #region Property

        private static Property GetPropertyFromReader(SqlDataReader reader)
        {
            return new Property
            {
                PropertyId = SQLDataHelper.GetInt(reader, "PropertyId"),

                Name = SQLDataHelper.GetString(reader, "Name"),
                NameDisplayed = SQLDataHelper.GetString(reader, "NameDisplayed"),
                SortOrder = SQLDataHelper.GetInt(reader, "SortOrder"),
                UseInFilter = SQLDataHelper.GetBoolean(reader, "UseInFilter"),
                UseInDetails = SQLDataHelper.GetBoolean(reader, "UseInDetails"),
                UseInBrief = SQLDataHelper.GetBoolean(reader, "UseInBrief"),

                Expanded = SQLDataHelper.GetBoolean(reader, "Expanded"),

                Description = SQLDataHelper.GetString(reader, "Description"),
                Unit = SQLDataHelper.GetString(reader, "Unit"),
                Type = SQLDataHelper.GetInt(reader, "Type"),
                GroupId = SQLDataHelper.GetNullableInt(reader, "GroupId"),
            };
        }

        /// <summary>
        /// returns property of product by it's ID
        /// </summary>
        /// <param name="propertyId"></param>
        /// <returns></returns>
        public static Property GetPropertyById(int propertyId)
        {
            return SQLDataAccess.ExecuteReadOne("[Catalog].[sp_GetPropertyByID]", CommandType.StoredProcedure,
                                                    GetPropertyFromReader, new SqlParameter("@PropertyID", propertyId));
        }

        public static Property GetPropertyByName(string name)
        {
            return SQLDataAccess.ExecuteReadOne("Select TOP 1 * From Catalog.Property Where Name=@name", CommandType.Text, GetPropertyFromReader, new SqlParameter("@name", name));
        }


        public static List<Property> GetAllProperties()
        {
            return SQLDataAccess.ExecuteReadList("[Catalog].[sp_GetAllProperties]", CommandType.StoredProcedure,
                    GetPropertyFromReader);
        }
        
        public static List<Property> GetAllPropertiesByPaging(int limit, int currentPage, string q)
        {
            var p = new Core.SQL2.SqlPaging(currentPage, limit)
                .Select("PropertyId", "Name")
                .From("Catalog.Property")
                .OrderBy("Name");
            
            if (!string.IsNullOrWhiteSpace(q))
                p.Where("Name like '%' + {0} + '%'", q);

            return p.PageItemsList<Property>();
        }

        public static int GetProductsCountByProperty(int propId)
        {
            return SQLDataAccess.ExecuteScalar<int>("[Catalog].[sp_GetCOUNTProductsByProperty]", CommandType.StoredProcedure, new SqlParameter("@PropertyID", propId));
        }

        public static int GetProductsCountByPropertyValue(int propValueId)
        {
            return SQLDataAccess.ExecuteScalar<int>("SELECT COUNT(1) FROM [Catalog].[ProductPropertyValue] WHERE PropertyValueID = @propValueId", CommandType.Text, new SqlParameter("@propValueId", propValueId));
        }

        /// <summary>
        /// add's new property into DB
        /// </summary>
        /// <param name="property"></param>
        public static int AddProperty(Property property)
        {
            CacheManager.RemoveByPattern(CacheNames.PropertiesInCategory);

            property.PropertyId = SQLDataAccess.ExecuteScalar<int>("[Catalog].[sp_AddProperty]", CommandType.StoredProcedure,
                                                    new SqlParameter("@Name", property.Name),
                                                    new SqlParameter("@NameDisplayed", string.IsNullOrEmpty(property.NameDisplayed) ? property.Name : property.NameDisplayed),
                                                    new SqlParameter("@UseInFilter", property.Type != (int)PropertyType.Textarea ? property.UseInFilter : false),
                                                    new SqlParameter("@UseInDetails", property.UseInDetails),
                                                    new SqlParameter("@UseInBrief", property.UseInBrief),
                                                    new SqlParameter("@Expanded", property.Expanded),
                                                    new SqlParameter("@SortOrder", property.SortOrder),
                                                    new SqlParameter("@Description", property.Description ?? (object)DBNull.Value),
                                                    new SqlParameter("@Unit", property.Unit ?? (object)DBNull.Value),
                                                    new SqlParameter("@Type", property.Type),
                                                    new SqlParameter("@GroupId", property.GroupId ?? (object)DBNull.Value));
            return property.PropertyId;
        }

        /// <summary>
        /// Deletes property from DB
        /// </summary>
        /// <param name="propertyId"></param>
        public static void DeleteProperty(int propertyId)
        {
            CacheManager.RemoveByPattern(CacheNames.PropertiesInCategory);

            SQLDataAccess.ExecuteNonQuery("[Catalog].[sp_DeleteProperty]", CommandType.StoredProcedure, new SqlParameter() { ParameterName = "@PropertyID", Value = propertyId });
        }

        /// <summary>
        /// updates property in DB
        /// </summary>
        /// <param name="property"></param>
        public static void UpdateProperty(Property property)
        {
            SQLDataAccess.ExecuteNonQuery("[Catalog].[sp_UpdateProperty]", CommandType.StoredProcedure,
                                            new SqlParameter("@PropertyID", property.PropertyId),
                                            new SqlParameter("@Name", property.Name),
                                            new SqlParameter("NameDisplayed", string.IsNullOrEmpty(property.NameDisplayed) ? property.Name : property.NameDisplayed),
                                            new SqlParameter("@UseInFilter", property.Type != (int)PropertyType.Textarea ? property.UseInFilter : false),
                                            new SqlParameter("@UseInDetails", property.UseInDetails),
                                            new SqlParameter("@Expanded", property.Expanded),
                                            new SqlParameter("@SortOrder", property.SortOrder),
                                            new SqlParameter("@Description", property.Description ?? (object)DBNull.Value),
                                            new SqlParameter("@Unit", property.Unit ?? (object)DBNull.Value),
                                            new SqlParameter("@Type", property.Type),
                                            new SqlParameter("@GroupId", property.GroupId ?? (object)DBNull.Value),
                                            new SqlParameter("@UseInBrief", property.UseInBrief));

            SQLDataAccess.ExecuteNonQuery("Update [Catalog].[PropertyValue] Set [UseInFilter]=@UseInFilter, [UseInDetails]=@UseInDetails, UseInBrief=@UseInBrief Where PropertyId=@PropertyId ",
                                            CommandType.Text,
                                            new SqlParameter("@PropertyId", property.PropertyId),
                                            new SqlParameter("@UseInFilter", property.UseInFilter),
                                            new SqlParameter("@UseInDetails", property.UseInDetails),
                                            new SqlParameter("@UseInBrief", property.UseInBrief));

            CacheManager.RemoveByPattern(CacheNames.PropertiesInCategory);
        }

        #endregion

        #region Property Values

        public static Dictionary<int, string> GetPropertyValuesByNameAndProductId(string text, int productId, int propertyId = 0)
        {
            var command =
                "Select DISTINCT PropertyValueID, Value " +
                "From Catalog.PropertyValue " +
                "WHERE Value like @name + '%' AND PropertyValueID NOT IN (Select PropertyValueID From Catalog.ProductPropertyValue Where ProductID = @productId)";

            var sqlParameters = new List<SqlParameter>()
            {
                new SqlParameter("@name", text),
                new SqlParameter("@productId", productId)
            };

            if (propertyId != 0)
            {
                command += " AND PropertyID = @PropertyID";

                sqlParameters.Add(new SqlParameter("@PropertyID", propertyId));
            }

            return SQLDataAccess.ExecuteReadDictionary<int, string>(command, CommandType.Text, "PropertyValueID", "Value", sqlParameters.ToArray());
        }

        public static Dictionary<int, string> GetPropertiesByName(string name)
        {
            return SQLDataAccess.ExecuteReadDictionary<int, string>("Select DISTINCT PropertyID, Name From Catalog.Property WHERE Name like @name + '%' or Name like @tName + '%'",
                                                              CommandType.Text, "PropertyID", "Name", new SqlParameter("@name", name), new SqlParameter("@tName", StringHelper.TranslitToRusKeyboard(name)));
        }

        public static bool IsExistPropertyValueInProduct(int productId, int propertyValueId)
        {
            return (int)SQLDataAccess.ExecuteScalar("Select Count(PropertyValueID) From Catalog.ProductPropertyValue Where ProductID = @productId And PropertyValueID = @propertyValueID", CommandType.Text, new SqlParameter("@productId", productId), new SqlParameter("@propertyValueID", propertyValueId)) > 0;
        }

        public static bool IsExistPropertyValueInProduct(int productId, int propertyId, string value)
        {
            return (int)SQLDataAccess.ExecuteScalar("Select Count(PropertyValueID) From Catalog.ProductPropertyValue Where ProductID = @productId And PropertyValueID In (Select PropertyValueID From Catalog.PropertyValue Where PropertyId = @propertyId And Value = @value )",
                CommandType.Text,
                new SqlParameter("@productId", productId),
                new SqlParameter("@propertyId", propertyId),
                new SqlParameter("@value", value)) > 0;
        }

        public static bool IsExistPropertyInProduct(int productId, int propertyID)
        {
            return (int)SQLDataAccess.ExecuteScalar(
                @"IF EXISTS (Select TOP (1) 1 From Catalog.ProductPropertyValue 
                            INNER JOIN Catalog.PropertyValue ON ProductPropertyValue.PropertyValueID = PropertyValue.PropertyValueID 
                            Where ProductID = @productId And PropertyValue.PropertyID = @propertyID)
                    SELECT 1;
                ELSE
                    SELECT 0;", CommandType.Text, 
                new SqlParameter("@productId", productId), 
                new SqlParameter("@propertyID", propertyID)) > 0;
        }

        /// <summary>
        /// returns all values that includes in property
        /// </summary>
        /// <param name="propertyId"></param>
        /// <returns></returns>
        public static List<PropertyValue> GetValuesByPropertyId(int propertyId)
        {
            return SQLDataAccess.ExecuteReadList("[Catalog].[sp_GetPropertyValuesByPropertyID]", CommandType.StoredProcedure,
                                                    GetPropertyValueFromReader, new SqlParameter("@PropertyID", propertyId));
        }

        /// <summary>
        /// returns all values of propepties belonging to product
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public static List<PropertyValue> GetPropertyValuesByProductId(int productId)
        {
            return SQLDataAccess.ExecuteReadList("[Catalog].[sp_GetPropertyValuesByProductID]", CommandType.StoredProcedure,
                                                    GetPropertyValueFromReader, new SqlParameter("@ProductID", productId));
        }

        public static PropertyValue GetPropertyValueById(int propertyValueId)
        {
            return SQLDataAccess.ExecuteReadOne("[Catalog].[sp_GetPropertyValueByID]", CommandType.StoredProcedure,
                                                    GetPropertyValueFromReader, new SqlParameter("@PropertyValueId", propertyValueId));
        }

        public static PropertyValue GetPropertyValueByName(int propertyId, string value)
        {
            return SQLDataAccess.ExecuteReadOne("Select Top 1 PropertyValueID, Value, NameOfValue From Catalog.PropertyValue Where PropertyID=@propertyID And Value=@value", CommandType.Text,
                                                    reader => new PropertyValue
                                                    {
                                                        PropertyValueId = SQLDataHelper.GetInt(reader, "PropertyValueID"),
                                                        Value = SQLDataHelper.GetString(reader, "Value"),
                                                        PropertyId = propertyId,
                                                        Name = SQLDataHelper.GetString(reader, "NameOfValue")
                                                    }, new SqlParameter("@propertyID", propertyId), new SqlParameter("@value", value));
        }

        public static List<PropertyValue> GetProductPropertyValues(int productId, int propertyId)
        {
            return SQLDataAccess.ExecuteReadList<PropertyValue>(
                "SELECT PropertyValue.* From Catalog.PropertyValue " +
                "INNER JOIN Catalog.ProductPropertyValue ON ProductPropertyValue.PropertyValueID = PropertyValue.PropertyValueID " +
                "WHERE ProductPropertyValue.ProductID = @productId AND PropertyValue.PropertyID = @PropertyID", 
                CommandType.Text, reader => new PropertyValue
                {
                    PropertyId = SQLDataHelper.GetInt(reader, "PropertyID"),
                    PropertyValueId = SQLDataHelper.GetInt(reader, "PropertyValueID"),
                    Value = SQLDataHelper.GetString(reader, "Value"),
                    Name = SQLDataHelper.GetString(reader, "NameOfValue")
                },
                new SqlParameter("@ProductID", productId),
                new SqlParameter("@PropertyID", propertyId));
        }

        private static PropertyValue GetPropertyValueFromReader(SqlDataReader reader)
        {
            return new PropertyValue
            {
                PropertyValueId = SQLDataHelper.GetInt(reader, "PropertyValueID"),
                PropertyId = SQLDataHelper.GetInt(reader, "PropertyID"),
                Value = SQLDataHelper.GetString(reader, "Value"),
                SortOrder = SQLDataHelper.GetInt(reader, "SortOrder"),
                Name = SQLDataHelper.GetString(reader, "NameOfValue"),
                Property = new Property
                {
                    GroupId = SQLDataHelper.GetNullableInt(reader, "GroupId"),
                    PropertyId = SQLDataHelper.GetInt(reader, "PropertyID"),
                    Name = SQLDataHelper.GetString(reader, "PropertyName"),
                    NameDisplayed = SQLDataHelper.GetString(reader, "PropertyNameDisplayed"),
                    SortOrder = SQLDataHelper.GetInt(reader, "PropertySortOrder"),
                    Expanded = SQLDataHelper.GetBoolean(reader, "Expanded"),
                    UseInDetails = SQLDataHelper.GetBoolean(reader, "UseInDetails"),
                    UseInFilter = SQLDataHelper.GetBoolean(reader, "UseInFilter"),
                    UseInBrief = SQLDataHelper.GetBoolean(reader, "UseInBrief"),
                    Type = SQLDataHelper.GetInt(reader, "Type"),
                    Description = SQLDataHelper.GetString(reader, "Description"),
                    Unit = SQLDataHelper.GetString(reader, "Unit"),
                    Group =
                        SQLDataHelper.GetNullableInt(reader, "GroupId") != null
                            ? new PropertyGroup()
                            {
                                PropertyGroupId = SQLDataHelper.GetInt(reader, "GroupId"),
                                Name = SQLDataHelper.GetString(reader, "GroupName"),
                                NameDisplayed = SQLDataHelper.GetString(reader, "GroupNameDisplayed"),
                                SortOrder = SQLDataHelper.GetInt(reader, "GroupSortOrder")
                            }
                            : null
                }
            };
        }

        /// <summary>
        /// returns property that include curent value
        /// </summary>
        /// <param name="valueId"></param>
        /// <returns></returns>
        public static Property GetPropertyByValueId(int valueId)
        {
            return SQLDataAccess.ExecuteReadOne("[Catalog].[sp_GetPropertyByValueID]", CommandType.StoredProcedure,
                                                GetPropertyFromReader, new SqlParameter("@PropertyValueId", valueId));
        }


        public static void UpdateOrInsertProductProperty(int productId, string name, string value, int sortOrder)
        {
            SQLDataAccess.ExecuteNonQuery("[Catalog].[sp_UpdateOrInsertProductProperty]", CommandType.StoredProcedure,
                                                 new SqlParameter("@ProductID", productId),
                                                 new SqlParameter("@Name", name),
                                                 new SqlParameter("@Value", value),
                                                 new SqlParameter("@RangeValue", value.RemoveChars()),
                                                 new SqlParameter("@SortOrder", sortOrder)
                                                 );
        }

        /// <summary>
        /// adds new value for some property
        /// </summary>
        /// <param name="propVal"></param>
        public static int AddPropertyValue(PropertyValue propVal)
        {
            if (propVal == null)
                throw new ArgumentNullException("propVal");
            if (propVal.PropertyId == 0)
                throw new ArgumentException(@"PropertyId cannot be zero", "propVal");

            propVal.PropertyValueId = SQLDataAccess.ExecuteScalar<int>("[Catalog].[sp_AddPropertyValue]", CommandType.StoredProcedure,
                                                            new SqlParameter("@Value", propVal.Value),
                                                            new SqlParameter("@NameOfValue", propVal.Name ?? (object)DBNull.Value),
                                                            new SqlParameter("@PropertyID", propVal.PropertyId),
                                                            new SqlParameter("@SortOrder", propVal.SortOrder),
                                                            new SqlParameter("@RangeValue", propVal.Value.RemoveChars()));

            CacheManager.RemoveByPattern(CacheNames.PropertiesInCategory);

            return propVal.PropertyValueId;
        }

        /// <summary>
        /// Deletes value from DB
        /// </summary>
        /// <param name="propertyValueId"></param>
        public static void DeletePropertyValueById(int propertyValueId)
        {
            SQLDataAccess.ExecuteNonQuery("[Catalog].[sp_DeletePropertyValue]", CommandType.StoredProcedure, new SqlParameter("@PropertyValueID", propertyValueId));

            CacheManager.RemoveByPattern(CacheNames.PropertiesInCategory);
        }

        /// <summary>
        /// updates value in DB
        /// </summary>
        /// <param name="value"></param>
        public static void UpdatePropertyValue(PropertyValue value)
        {
            SQLDataAccess.ExecuteNonQuery("[Catalog].[sp_UpdatePropertyValue]", CommandType.StoredProcedure,
                                                 new SqlParameter("@Value", value.Value),
                                                 new SqlParameter("@NameOfValue", value.Name ?? (object)DBNull.Value),
                                                 new SqlParameter("@SortOrder", value.SortOrder),
                                                 new SqlParameter("@PropertyValueId", value.PropertyValueId),
                                                 new SqlParameter("@RangeValue", value.Value.RemoveChars()));

            CacheManager.RemoveByPattern(CacheNames.PropertiesInCategory);
        }

        /// <summary>
        /// returns all products that includes this value
        /// </summary>
        /// <param name="propVal"></param>
        /// <returns></returns>
        public static List<int> GetProductsIDsByPropertyValue(PropertyValue propVal)
        {
            List<int> productIDs = SQLDataAccess.ExecuteReadList<int>("select ProductID from [Catalog].[ProductPropertyValue] Where PropertyValueID = @ValueID",
                                                                 CommandType.Text,
                                                                 reader => SQLDataHelper.GetInt(reader, "ProductID"),
                                                                 new SqlParameter("@ValueID", propVal.PropertyValueId));
            return productIDs;
        }
        
        public static List<PropertyValue> GetAllPropertyValuesByPaging(int limit, int currentPage, string q, int? propertyId)
        {
            var p = new Core.SQL2.SqlPaging(currentPage, limit)
                .Select("PropertyValueId", "[Value]")
                .From("Catalog.PropertyValue")
                .OrderBy("[Value]");

            if (propertyId != null)
                p.Where("PropertyId = {0}", propertyId.Value);
            
            if (!string.IsNullOrWhiteSpace(q))
                p.Where("[Value] like '%' + {0} + '%'", q);

            return p.PageItemsList<PropertyValue>();
        }

        #endregion

        private const string PropertyFilterQuery = 
@";WITH ppvid (propertyvalueid)
		 AS
		 (
		    select DISTINCT ppv.propertyvalueid 
            from [catalog].product p 
			inner join [catalog].[productpropertyvalue] ppv on p.productid = ppv.productid  AND p.[enabled] = 1 
			{0}
            {1}
		 )
		 
	SELECT     pv.[propertyvalueid],
			   pv.[propertyid],
			   pv.[value],
			   pv.[rangevalue],
               pv.[sortorder],
			   p.[name]               AS propertyname,
			   p.NameDisplayed        AS propertyNameDisplayed,
			   p.sortorder            AS propertysortorder,
			   p.expanded             AS propertyexpanded,
			   p.unit                 AS propertyunit,
			   p.[TYPE]               AS propertytype,
			   p.[Description]        AS propertydescription
			   
	FROM       [catalog].[propertyvalue] pv
	INNER JOIN [catalog].[property] p	ON p.propertyid = pv.propertyid AND p.[useinfilter] = 1
	inner join ppvid on ppvid.propertyvalueid = pv.[propertyvalueid]
	WHERE      pv.[useinfilter] = 1
	ORDER BY   propertysortorder,
			   pv.[propertyid],
			   pv.[sortorder],
			   pv.[rangevalue],
			   pv.[value]";

        public static List<PropertyValue> GetPropertyValuesByCategories(int categoryId, 
                                                                        bool useDepth, 
                                                                        List<int> productIds, 
                                                                        List<int> warehouseIds,
                                                                        bool showOnlyAvailable)
        {
            var queryInners = new List<string>();
            var queryWheres = new List<string>();
            var queryParams = new List<SqlParameter>()
            {
                new SqlParameter("@CategoryId", categoryId)
            };
            
            if (useDepth)
            {
                if (productIds != null && productIds.Count > 0)
                {
                    if (categoryId != 0)
                        queryInners.Add("inner join [catalog].[ProductCategories] pc on p.ProductId = pc.ProductId and pc.[CategoryId] = @CategoryId");
                    
                    queryInners.Add("inner join (select value from [Settings].[SPLIT_INT](@productIds, ',')) as sr on p.ProductId = sr.value");
                    queryParams.Add(new SqlParameter("@productIds", string.Join(",", productIds)));
                }
                else
                {
                    queryInners.Add("inner join [catalog].[ProductCategories] pc on p.ProductId = pc.ProductId");
                    queryInners.Add("inner join [settings].[getchildcategorybyparent] (@CategoryId) ch on pc.[CategoryId] = ch.id");
                }
            }
            else
            {
                queryInners.Add("inner join [catalog].[ProductCategories] pc on p.ProductId=pc.ProductId and pc.[CategoryId] = @CategoryId");
            }

            if (showOnlyAvailable)
            {
                queryInners.Add("inner join [catalog].[ProductExt] pex on p.ProductId=pex.ProductId and (pex.MaxAvailable>0 OR p.[AllowPreOrder] = 1)");
            }

            if (warehouseIds != null)
            {
                queryWheres.Add(
                    " (p.AllowPreOrder = 1 " +
                    "       OR Exists(" +
                    "           Select 1 from [Catalog].[Offer] " +
                    "           Inner Join [Catalog].[WarehouseStocks] on [Offer].[OfferID] = [WarehouseStocks].[OfferId] " +
                    "           Where [WarehouseStocks].[WarehouseId] in (Select value From [Settings].[SPLIT_INT](@warehouseIds, ',')) " +
                    "                   AND Offer.ProductId = p.ProductID " +
                    "                   AND [WarehouseStocks].[Quantity] > 0)) ");
                queryParams.Add(new SqlParameter("@warehouseIds", string.Join(",", warehouseIds)));
            }

            var sql = string.Format(PropertyFilterQuery, 
                queryInners.AggregateString(" "), 
                queryWheres.Count > 0 ? "Where " + queryWheres.AggregateString(" and ") : null);

            return SQLDataAccess.ExecuteReadList(
                sql,
                CommandType.Text,
                reader => new PropertyValue
                {
                    PropertyValueId = SQLDataHelper.GetInt(reader, "PropertyValueID"),
                    PropertyId = SQLDataHelper.GetInt(reader, "PropertyID"),
                    Value = SQLDataHelper.GetString(reader, "Value"),
                    RangeValue = SQLDataHelper.GetFloat(reader, "RangeValue"),
                    SortOrder = SQLDataHelper.GetInt(reader, "SortOrder"),
                    Property = new Property
                    {
                        PropertyId = SQLDataHelper.GetInt(reader, "PropertyID"),
                        Name = SQLDataHelper.GetString(reader, "PropertyName"),
                        NameDisplayed = SQLDataHelper.GetString(reader, "PropertyNameDisplayed"),
                        SortOrder = SQLDataHelper.GetInt(reader, "PropertySortOrder"),
                        Expanded = SQLDataHelper.GetBoolean(reader, "PropertyExpanded"),
                        Type = SQLDataHelper.GetInt(reader, "PropertyType"),
                        Unit = SQLDataHelper.GetString(reader, "PropertyUnit"),
                        Description = SQLDataHelper.GetString(reader, "PropertyDescription")
                    }
                },
                queryParams.ToArray());
        }

        public static List<PropertyValue> GetPropertyValuesByCategories(EProductOnMain productOnMainType,
                                                                        int productListId, 
                                                                        List<int> warehouseIds, 
                                                                        bool showOnlyAvailable)
        {
            var queryInners = new List<string>();
            var queryWheres = new List<string>();
            var queryParams = new List<SqlParameter>();

            switch (productOnMainType)
            {
                case EProductOnMain.New:
                    queryWheres.Add("New=1");
                    break;
                case EProductOnMain.Best:
                    queryWheres.Add("Bestseller=1");
                    break;
                case EProductOnMain.Sale:
                    queryWheres.Add("(Discount>0 or DiscountAmount>0)");
                    break;
                case EProductOnMain.List:
                    if (productListId == 0)
                        return null;
                    
                    queryWheres.Add("p.ProductId in (Select ProductId from [Catalog].[Product_ProductList] where [Product_ProductList].ListId = @listId)");
                    queryParams.Add(new SqlParameter("@listId", productListId));
                    break;
                case EProductOnMain.NewArrivals:
                    return new List<PropertyValue>();
            }
            
            if (showOnlyAvailable)
            {
                queryInners.Add("Inner Join [catalog].[ProductExt] pex on p.ProductId = pex.ProductId and (pex.MaxAvailable>0 OR p.[AllowPreOrder] = 1)");
            }
            
            if (warehouseIds != null)
            {
                queryWheres.Add( 
                    "(p.AllowPreOrder = 1 " +
                    "   OR Exists(" +
                    "       Select 1 from [Catalog].[Offer] " +
                    "       Inner Join [Catalog].[WarehouseStocks] on [Offer].[OfferID] = [WarehouseStocks].[OfferId] " +
                    "       Where [WarehouseStocks].[WarehouseId] in (Select value From [Settings].[SPLIT_INT](@warehouseIds, ',')) " +
                    "               AND Offer.ProductId = p.ProductID " +
                    "               AND [WarehouseStocks].[Quantity] > 0))");
                queryParams.Add(new SqlParameter("@warehouseIds", string.Join(",", warehouseIds)));
            }

            var sql = string.Format(PropertyFilterQuery, 
                queryInners.AggregateString(" "),
                queryWheres.Count > 0 ? "Where " + queryWheres.AggregateString(" and ") : null);

            return SQLDataAccess.ExecuteReadList(
                sql,
                CommandType.Text,
                reader => new PropertyValue
                {
                    PropertyValueId = SQLDataHelper.GetInt(reader, "PropertyValueID"),
                    PropertyId = SQLDataHelper.GetInt(reader, "PropertyID"),
                    Value = SQLDataHelper.GetString(reader, "Value"),
                    RangeValue = SQLDataHelper.GetFloat(reader, "RangeValue"),
                    SortOrder = SQLDataHelper.GetInt(reader, "SortOrder"),
                    Property = new Property
                    {
                        PropertyId = SQLDataHelper.GetInt(reader, "PropertyID"),
                        Name = SQLDataHelper.GetString(reader, "PropertyName"),
                        NameDisplayed = SQLDataHelper.GetString(reader, "PropertyNameDisplayed"),
                        SortOrder = SQLDataHelper.GetInt(reader, "PropertySortOrder"),
                        Expanded = SQLDataHelper.GetBoolean(reader, "PropertyExpanded"),
                        Type = SQLDataHelper.GetInt(reader, "PropertyType"),
                        Unit = SQLDataHelper.GetString(reader, "PropertyUnit"),
                        Description = SQLDataHelper.GetString(reader, "PropertyDescription")
                    }
                },
                queryParams.ToArray());
        }

        public static void DeleteProductPropertyValue(int productId, int propertyValueId)
        {
            SQLDataAccess.ExecuteNonQuery("[Catalog].[sp_DeleteProductPropertyValue]", CommandType.StoredProcedure,
                                            new SqlParameter("@ProductID", productId),
                                            new SqlParameter("@PropertyValueID", propertyValueId));

            CacheManager.RemoveByPattern(CacheNames.PropertiesInCategory);
        }

        public static void DeleteProductProperties(int productId)
        {
            SQLDataAccess.ExecuteNonQuery("[Catalog].[sp_DeleteProductProperties]", CommandType.StoredProcedure,
                                            new SqlParameter("@ProductID", productId));

            CacheManager.RemoveByPattern(CacheNames.PropertiesInCategory);
        }

        public static void DeleteProductPropertyValues(int productId, int propertyId)
        {
            SQLDataAccess.ExecuteNonQuery(
                "DELETE ppv FROM [Catalog].[ProductPropertyValue] ppv " +
                    "INNER JOIN [Catalog].[PropertyValue] pv ON pv.PropertyValueID = ppv.PropertyValueID " +
                "WHERE ppv.ProductID = @ProductId AND pv.PropertyID = @PropertyId", 
                CommandType.Text,
                new SqlParameter("@ProductID", productId),
                new SqlParameter("@PropertyID", propertyId));

            CacheManager.RemoveByPattern(CacheNames.PropertiesInCategory);
        }

        public static void AddProductProperyValue(int propValId, int productId)
        {
            if (propValId == 0)
                throw new ArgumentException(@"Value cannot be zero", "propValId");

            SQLDataAccess.ExecuteNonQuery("INSERT INTO [Catalog].[ProductPropertyValue] ([ProductID],[PropertyValueID]) VALUES (@ProductID,@PropertyValueID)",
                                            CommandType.Text,
                                            new SqlParameter("@ProductID", productId),
                                            new SqlParameter("@PropertyValueID", propValId)
                                            );

            CacheManager.RemoveByPattern(CacheNames.PropertiesInCategory);
        }

        public static void UpdateProductPropertyValue(int productId, int oldPropertyValueId, int newPropertyValueId)
        {
            if (oldPropertyValueId == 0)
                throw new ArgumentException(@"Value cannot be zero", "oldPropertyValueId");
            if (newPropertyValueId == 0)
                throw new ArgumentException(@"Value cannot be zero", "newPropertyValueId");

            SQLDataAccess.ExecuteNonQuery("[Catalog].[sp_UpdateProductProperty]", CommandType.StoredProcedure,
                                            new SqlParameter("@ProductID", productId),
                                            new SqlParameter("@OldPropertyValueID", oldPropertyValueId),
                                            new SqlParameter("@NewPropertyValueID", newPropertyValueId)
                                            );

            CacheManager.RemoveByPattern(CacheNames.PropertiesInCategory);
        }

        public static void UpdateProductPropertyValue(int productId, int propertyValueId, string value)
        {
            if (propertyValueId == 0)
                throw new ArgumentException(@"Value cannot be zero", "propertyValueId");
            //I was drunk
            int propertyId = GetPropertyByValueId(propertyValueId).PropertyId;
            DeleteProductPropertyValue(productId, propertyValueId);
            AddProductProperyValue(AddPropertyValue(new PropertyValue { PropertyId = propertyId, Value = value, SortOrder = 0 }), productId);
        }

        public static void ShowInFilter(int propertyId, bool show)
        {
            SQLDataAccess.ExecuteNonQuery(
                "UPDATE [Catalog].[Property] SET UseInFilter = @UseInFilter WHERE [PropertyID] = @PropertyID",
                CommandType.Text, new SqlParameter("@PropertyID", propertyId),
                new SqlParameter("@UseInFilter", show));

            UpdateUseInFilterPropertyValues(propertyId, show);

            CacheManager.RemoveByPattern(CacheNames.PropertiesInCategory);
        }

        private static void UpdateUseInFilterPropertyValues(int propertyId, bool show)
        {
            SQLDataAccess.ExecuteNonQuery(
                "UPDATE [Catalog].[PropertyValue] SET UseInFilter = @UseInFilter WHERE [PropertyID] = @PropertyID",
                CommandType.Text, new SqlParameter("@PropertyID", propertyId),
                new SqlParameter("@UseInFilter", show));
        }

        public static void ShowInDetails(int propertyId, bool show)
        {
            SQLDataAccess.ExecuteNonQuery(
                "UPDATE [Catalog].[Property] SET UseInDetails = @UseInDetails WHERE [PropertyID] = @PropertyID",
                CommandType.Text, new SqlParameter("@PropertyID", propertyId),
                new SqlParameter("@UseInDetails", show));

            UpdateUseInDetailsPropertyValues(propertyId, show);
        }
        
        private static void UpdateUseInDetailsPropertyValues(int propertyId, bool show)
        {
            SQLDataAccess.ExecuteNonQuery(
                "UPDATE [Catalog].[PropertyValue] SET UseInDetails = @UseInDetails WHERE [PropertyID] = @PropertyID",
                CommandType.Text, new SqlParameter("@PropertyID", propertyId),
                new SqlParameter("@UseInDetails", show));
        }

        public static void ShowInBrief(int propertyId, bool show)
        {
            SQLDataAccess.ExecuteNonQuery(
                "UPDATE [Catalog].[Property] SET UseInBrief = @UseInBrief WHERE [PropertyID] = @PropertyID",
                CommandType.Text, new SqlParameter("@PropertyID", propertyId),
                new SqlParameter("@UseInBrief", show));
            
            UpdateUseInBriefPropertyValues(propertyId, show);
        }
        
        private static void UpdateUseInBriefPropertyValues(int propertyId, bool show)
        {
            SQLDataAccess.ExecuteNonQuery(
                "UPDATE [Catalog].[PropertyValue] SET UseInBrief = @UseInBrief WHERE [PropertyID] = @PropertyID",
                CommandType.Text, new SqlParameter("@PropertyID", propertyId),
                new SqlParameter("@UseInBrief", show));
        }

        public static void UpdateGroup(int propertyId, int? groupId)
        {
            SQLDataAccess.ExecuteNonQuery(
                "UPDATE [Catalog].[Property] SET GroupId = @GroupId WHERE [PropertyId] = @PropertyId",
                CommandType.Text,
                new SqlParameter("@PropertyId", propertyId),
                new SqlParameter("@GroupId", groupId ?? (object)DBNull.Value));
        }

        public static List<Property> GetPropertyNamesByCompareCart()
        {
            return SQLDataAccess.ExecuteReadList<Property>(
                "select distinct Property.PropertyId, Property.Name, Property.NameDisplayed, Property.UseInFilter, Property.UseInDetails, Property.UseInBrief, Expanded, Description, Unit, Type, GroupId, Property.SortOrder " +
                "from catalog.Shoppingcart inner join catalog.Offer on Shoppingcart.OfferID = Offer.OfferId and Shoppingcart.ShoppingCartType = 3" +
                "inner join catalog.ProductPropertyValue on Offer.ProductID = ProductPropertyValue.Productid " +
                "inner join catalog.PropertyValue on PropertyValue.PropertyValueID = ProductPropertyValue.PropertyValueID " +
                "inner join catalog.Property on Property.PropertyID = PropertyValue.PropertyID " +
                "where CustomerId=@CustomerId and (Property.UseInFilter = 1 or  Property.UseInDetails = 1 or Property.UseInBrief = 1)" +
                "order by Property.SortOrder",
                CommandType.Text, GetPropertyFromReader,
                new SqlParameter("@CustomerId", Customers.CustomerContext.CustomerId));
        }

        public static string PropertiesToString(List<PropertyValue> productPropertyValues, string columSeparator, string propertySeparator)
        {
            var res = new StringBuilder();
            for (int i = 0; i < productPropertyValues.Count; i++)
            {
                if (i == 0)
                    res.Append(productPropertyValues[i].Property.Name + propertySeparator + productPropertyValues[i].Value);
                else res.Append(columSeparator + productPropertyValues[i].Property.Name + propertySeparator + productPropertyValues[i].Value);
            }
            return res.ToString();
        }

        public static void PropertiesFromString(int productId, string properties, string columSeparator, string propertySeparator, bool isNewProduct)
        {
            if (string.IsNullOrWhiteSpace(columSeparator) || string.IsNullOrWhiteSpace(propertySeparator))
                _PropertiesFromString(productId, properties, ",",  ":", isNewProduct);
            else
                _PropertiesFromString(productId, properties, columSeparator, propertySeparator, isNewProduct);
        }

        private static void _PropertiesFromString(int productId, string properties, string columSeparator, string propertySeparator, bool isNewProduct)
        {
            try
            {
                if (!isNewProduct)
                    DeleteProductProperties(productId);
                
                if (string.IsNullOrEmpty(properties)) 
                    return;
                
                //type:value,type:value,...
                var items = properties.Split(new[] { columSeparator }, StringSplitOptions.RemoveEmptyEntries);
                var stepSort = 0;
                foreach (string s in items)
                {
                    var temp = s.SupperTrim().Split(new[] { propertySeparator }, StringSplitOptions.RemoveEmptyEntries);
                    if (temp.Length != 2)
                        continue;
                    var tempType = temp[0].SupperTrim();
                    var tempValue = temp[1].SupperTrim();
                    if (!string.IsNullOrWhiteSpace(tempType) && !string.IsNullOrWhiteSpace(tempValue))
                    {
                        // inside stored procedure not thread save/ do save mode by logic 
                        SQLDataAccess.ExecuteNonQuery("[Catalog].[sp_ParseProductProperty]", CommandType.StoredProcedure,
                                                      new SqlParameter("@nameProperty", tempType),
                                                      new SqlParameter("@propertyValue", tempValue),
                                                      new SqlParameter("@rangeValue", tempValue.RemoveChars()),
                                                      new SqlParameter("@productId", productId),
                                                      new SqlParameter("@sort", stepSort));
                        stepSort += 10;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }
        }

        /// <summary>
        /// Add product properties by Dictionary PropertyName, PropertyValue
        /// </summary>
        public static void ProcessProductProperties(int productId, Dictionary<string, string> properties, string columnSeparator)
        {
            foreach (var name in properties.Keys)
            {
                var property = GetPropertyByName(name) ?? new Property
                {
                    Name = name,
                    NameDisplayed = name,
                    UseInFilter = true,
                    UseInDetails = true,
                    UseInBrief = false,
                    Type = 1
                };
                var isNew = property.PropertyId == 0;
                if (isNew)
                    AddProperty(property);

                if (properties[name].IsNullOrEmpty())
                {
                    if (!isNew)
                        DeleteProductPropertyValues(productId, property.PropertyId);
                    continue;
                }

                var existValues = !isNew ? GetProductPropertyValues(productId, property.PropertyId) : new List<PropertyValue>();
                var values = property.Type == (int)PropertyType.Textarea 
                                ? new List<string> { properties[name] }
                                : properties[name].Split(columnSeparator).Select(x => x.SupperTrim()).ToList();

                var actualValueIds = new List<int>();
                foreach (var value in values)
                {
                    if (value.IsNullOrEmpty())
                        continue;

                    var existValue = existValues.FirstOrDefault(x => x.Value == value);
                    if (existValue != null)
                    {
                        actualValueIds.Add(existValue.PropertyValueId);
                        continue;
                    }

                    var propertyValue = (!isNew ? GetPropertyValueByName(property.PropertyId, value) : null) ??
                        new PropertyValue
                        {
                            PropertyId = property.PropertyId,
                            Value = value,
                            RangeValue = value.RemoveChars()
                        };

                    if (propertyValue.PropertyValueId != 0 && (existValue = existValues.FirstOrDefault(x => x.PropertyValueId == propertyValue.PropertyValueId)) != null)
                    {
                        actualValueIds.Add(existValue.PropertyValueId);
                        continue;
                    }

                    if (propertyValue.PropertyValueId == 0)
                        AddPropertyValue(propertyValue);

                    if (!actualValueIds.Contains(propertyValue.PropertyValueId))
                    {
                        AddProductProperyValue(propertyValue.PropertyValueId, productId);
                        actualValueIds.Add(propertyValue.PropertyValueId);
                    }
                }

                // удаление неактуальных значений
                foreach (var propValueId in existValues.Select(x => x.PropertyValueId).Where(x => !actualValueIds.Contains(x)))
                {
                    DeleteProductPropertyValue(productId, propValueId);
                }
            }
        }
    }
}