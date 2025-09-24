using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Booking;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.Services.Crm.SalesFunnels;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.SQL;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;
using AdvantShop.Repository.Currencies;
using AdvantShop.Taxes;

namespace AdvantShop.Core.Services.ChangeHistories
{
    public class ChangeHistoryService
    {
        private static readonly Type CompareAttributeType = typeof(CompareAttribute);
        private static readonly Type LocalizeAttributeType = typeof(LocalizeAttribute);

        #region Add/Get/Delete

        public static void Add(List<ChangeHistory> histories)
        {
            if (histories.Count == 0)
                return;

            foreach (var history in histories)
                Add(history);
        }

        public static int Add(ChangeHistory history)
        {
            if (history.ObjType == ChangeHistoryObjType.Product)
                return ProductChangeHistoryService.Add(history);
            
            return
                SQLDataAccess.ExecuteScalar<int>(
                    "Insert Into [CMS].[ChangeHistory] (ObjId,ObjType,ParameterName,OldValue,NewValue,ParameterType,ParameterId,ChangedByName,ChangedById,ModificationTime) " +
                    "Values (@ObjId,@ObjType,@ParameterName,@OldValue,@NewValue,@ParameterType,@ParameterId,@ChangedByName,@ChangedById,@ModificationTime);" +
                    "Select scope_identity();",
                    CommandType.Text,
                    new SqlParameter("@ObjId", history.ObjId),
                    new SqlParameter("@ObjType", (int) history.ObjType),

                    new SqlParameter("@ParameterName", history.ParameterName.Reduce(350)),
                    new SqlParameter("@OldValue", history.OldValue ?? ""),
                    new SqlParameter("@NewValue", history.NewValue ?? ""),
                    new SqlParameter("@ParameterType", (int) history.ParameterType),
                    new SqlParameter("@ParameterId", history.ParameterId ?? (object) DBNull.Value),

                    new SqlParameter("@ChangedByName", history.ChangedByName ?? ""),
                    new SqlParameter("@ChangedById", history.ChangedById ?? (object) DBNull.Value),
                    new SqlParameter("@ModificationTime",
                        history.ModificationTime != DateTime.MinValue ? history.ModificationTime : DateTime.Now)
                    );
        }

        public static List<ChangeHistory> Get(int objId, ChangeHistoryObjType objType)
        {
            if (objType == ChangeHistoryObjType.Product)
                return ProductChangeHistoryService.Get(objId);

            return SQLDataAccess.Query<ChangeHistory>(
                "Select * from [CMS].[ChangeHistory] Where ObjId=@objId and ObjType=@objType Order By ModificationTime desc", new {objId, objType})
                .ToList();
        }

        public static ChangeHistory GetLast(int objId, ChangeHistoryObjType objType)
        {
            if (objType == ChangeHistoryObjType.Product)
                return ProductChangeHistoryService.GetLast(objId);

            return SQLDataAccess.Query<ChangeHistory>(
                                     "Select top(1) * from [CMS].[ChangeHistory] Where ObjId=@objId and ObjType=@objType Order By ModificationTime desc", new { objId, objType })
                                .FirstOrDefault();
        }

        public static void DeleteExpiredHistory()
        {
            try
            {
                using (var db = new SQLDataAccess())
                {
                    db.cmd.CommandTimeout = 60 * 10; // 10 mins
                    db.cmd.CommandText = "Delete from [CMS].[ChangeHistory] Where ModificationTime < @date AND ObjType <> @objType OR ObjType = @objType AND ModificationTime < @dateForTasks";
                    db.cmd.Parameters.Clear();
                    db.cmd.Parameters.Add(new SqlParameter("@Date", DateTime.Now.AddMonths(-6)));
                    db.cmd.Parameters.Add(new SqlParameter("@dateForTasks", DateTime.Now.AddMonths(-36)));
                    db.cmd.Parameters.Add(new SqlParameter("@objType", (int)ChangeHistoryObjType.Task));

                    db.cnOpen();
                    db.cmd.ExecuteNonQuery();
                    db.cnClose();
                }

            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }


            try
            {
                // храним только 10 последних изменений одного поля одной сущности из одного источника
                using (var db = new SQLDataAccess())
                {
                    db.cmd.CommandTimeout = 60 * 20; // 20 mins
                    db.cmd.CommandText = @"
                                DECLARE @ObjId INT, @ObjType NVARCHAR(max),  
                                    @ParameterName NVARCHAR(max), @ChangedByName NVARCHAR(max);  
  
                                DECLARE history_cursor CURSOR FOR   
  
	                                  select [ObjId], [ObjType], [ParameterName], [ChangedByName]
	                                  FROM [CMS].[ChangeHistory]
	                                  group by [ObjId], [ObjType], [ParameterName], [ChangedByName]
	                                  having COUNT(*) > @Count

                                OPEN history_cursor  
  
                                FETCH NEXT FROM history_cursor   
                                INTO @ObjId, @ObjType, @ParameterName, @ChangedByName

                                WHILE @@FETCH_STATUS = 0  
                                BEGIN  
	                                delete from [CMS].[ChangeHistory]
	                                where [ObjId] = @ObjId and [ObjType] = @ObjType and [ParameterName] = @ParameterName and [ChangedByName] = @ChangedByName
	                                and [id] not in 
	                                (
		                                select top (@Count) [id] from [CMS].[ChangeHistory]
		                                where ObjId = @ObjId and  [ObjType] = @ObjType and [ParameterName] = @ParameterName and [ChangedByName] = @ChangedByName
		                                order by [id] desc
	                                )

	                                FETCH NEXT FROM history_cursor   
                                    INTO @ObjId, @ObjType, @ParameterName, @ChangedByName  
                                END   
                                CLOSE history_cursor;  
                                DEALLOCATE history_cursor;";
                    db.cmd.Parameters.Clear();
                    db.cmd.Parameters.Add(new SqlParameter("@Count", 10));

                    db.cnOpen();
                    db.cmd.ExecuteNonQuery();
                    db.cnClose();
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            ProductChangeHistoryService.DeleteExpiredHistory();

        }
        #endregion

        public static List<ChangeHistory> GetChanges(int objId, ChangeHistoryObjType objType, object oldObj, object newObj, ChangedBy changedBy, 
                                                     int? entityId = null, string entityName = null)
        {
            var history = new List<ChangeHistory>();

            try
            {
                var properties = oldObj.GetType().GetProperties().Where(x => x.IsDefined(CompareAttributeType, false));

                foreach (var property in properties)
                {
                    object oldValue = property.GetValue(oldObj);
                    object newValue = property.GetValue(newObj);

                    if (oldValue is DateTime && newValue is DateTime)
                    {
                        // для сравнения даты обнуляем милисекунды
                        // т.к. в базе не такая высокая точность
                        // и фиксировалось изменение хотя его не было
                        DateTime dt = (DateTime)oldValue;
                        oldValue = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);

                        dt = (DateTime)newValue;
                        newValue = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
                    }

                    if (object.Equals(oldValue, newValue) ||
                        (newValue == null && oldValue is string && (string) oldValue == ""))
                        continue;

                    var attributes = property.GetCustomAttributes(CompareAttributeType, false);
                    if (attributes.Length == 0)
                        continue;

                    var attribute = attributes[0] as ICompareAttribute<string, ChangeHistoryParameterType>;
                    var parameterName = attribute != null ? attribute.Value : property.Name;

                    var parameterId = attribute != null && attribute.Type != ChangeHistoryParameterType.None ? objId : default(int?);
                    var parameterType = attribute != null ? attribute.Type : ChangeHistoryParameterType.None;


                    var item = new ChangeHistory(changedBy)
                    {
                        ObjId = objId,
                        ObjType = objType,
                        ParameterName = GetParameterNameByType(parameterName, parameterType, objId, entityId, entityName),
                        ParameterId = entityId ?? parameterId,
                        ParameterType = parameterType
                    };

                    if (attribute != null && attribute.NotLogValue)
                    {
                        item.ParameterName += " изменен(о)";
                    }
                    else
                    {
                        var propertyType = property.PropertyType;

                        if (propertyType.IsEnum)
                        {
                            if (newValue != null)
                                item.NewValue = GetLocalizedValue(ref newValue);

                            if (oldValue != null)
                                item.OldValue = GetLocalizedValue(ref oldValue);
                        }
                        else if (attribute != null && attribute.Type != ChangeHistoryParameterType.None)
                        {
                            if (newValue != null)
                                item.NewValue = GetValueByType(ref newValue, attribute.Type);

                            if (oldValue != null)
                                item.OldValue = GetValueByType(ref oldValue, attribute.Type);
                        }
                        else if (propertyType == typeof(bool))
                        {
                            item.NewValue = newValue != null ? ((bool)newValue ? LocalizationService.GetResource("Admin.Yes") : LocalizationService.GetResource("Admin.No")) : "";
                            item.OldValue = oldValue != null ? ((bool)oldValue ? LocalizationService.GetResource("Admin.Yes") : LocalizationService.GetResource("Admin.No")) : "";
                        }
                        else if (propertyType.FullName != null &&
                                 propertyType.FullName.Contains("System.Collections.Generic.List"))
                        {
                            var oldValues = oldValue as IEnumerable<object>;
                            var newValues = newValue as IEnumerable<object>;

                            var oldValuesArr = oldValues != null ? oldValues.ToArray() : null;
                            var newValuesArr = newValues != null ? newValues.ToArray() : null;

                            if (oldValuesArr == null && newValuesArr == null)
                                continue;

                            if (oldValuesArr != null && newValuesArr != null && oldValuesArr.Length == newValuesArr.Length)
                            {
                                var diff = oldValuesArr.Except(newValuesArr);
                                if (!diff.Any())
                                    continue;
                            }

                            item.NewValue = newValuesArr != null
                                ? newValuesArr.Select(x => x.ToString()).AggregateString(", ")
                                : "";
                            item.OldValue = oldValuesArr != null
                                ? oldValuesArr.Select(x => x.ToString()).AggregateString(", ")
                                : "";
                        }
                        else
                        {
                            item.NewValue = newValue != null ? newValue.ToString() : "";
                            item.OldValue = oldValue != null ? oldValue.ToString() : "";
                        }
                    }

                    history.Add(item);
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            return history;
        }

        private static string GetLocalizedValue(ref object value)
        {
            var fi = value.GetType().GetField(value.ToString());
            if (fi != null)
            {
                var attributes = fi.GetCustomAttributes(LocalizeAttributeType, false);
                if (attributes.Length > 0)
                {
                    var attribute = attributes[0] as IAttribute<string>;
                    if (attribute != null)
                        return attribute.Value;
                }
            }

            return value.ToString();
        }

        private static string GetValueByType(ref object value, ChangeHistoryParameterType type)
        {
            try
            {
                switch (type)
                {
                    case ChangeHistoryParameterType.SalesFunnel:
                    {
                        var salesFunnel = SalesFunnelService.Get((int) value);
                        return salesFunnel != null ? salesFunnel.Name : "";
                    }

                    case ChangeHistoryParameterType.Manager:
                    {
                        var manager = ManagerService.GetManager((int) value);
                        return manager != null ? manager.FullName : "";
                    }

                    case ChangeHistoryParameterType.OrderSource:
                    {
                        var source = OrderSourceService.GetOrderSource((int) value);
                        return source != null ? source.Name : "";
                    }

                    case ChangeHistoryParameterType.Tax:
                    {
                        var tax = TaxService.GetTax((int) value);
                        return tax != null ? tax.Name : "";
                    }

                    case ChangeHistoryParameterType.Brand:
                    {
                        var brand = BrandService.GetBrandById((int) value);
                        return brand != null ? brand.Name : "";
                    }

                    case ChangeHistoryParameterType.Currency:
                    {
                        var currency = CurrencyService.GetCurrency((int) value);
                        return currency != null ? currency.Name : "";
                    }

                    case ChangeHistoryParameterType.Color:
                    {
                        var color = ColorService.GetColor((int) value);
                        return color != null ? color.ColorName : "";
                    }

                    case ChangeHistoryParameterType.Size:
                    {
                        var size = SizeService.GetSize((int) value);
                        return size != null ? size.SizeName : "";
                    }

                    case ChangeHistoryParameterType.CustomerGroup:
                    {
                        var group = CustomerGroupService.GetCustomerGroup((int) value);
                        if (group != null)
                            return group.GroupName;
                        break;
                    }
                
                    case ChangeHistoryParameterType.Unit:
                    {
                        var unit = UnitService.Get((int) value);
                        return unit != null ? unit.Name : "";
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }
            return value.ToString();
        }

        private static string GetParameterNameByType(string name, ChangeHistoryParameterType type, int objId, int? entityId, string entityName)
        {
            try
            {
                switch (type)
                {
                    case ChangeHistoryParameterType.LeadItemField:
                    {
                        if (entityId != null)
                        {
                            var item = LeadService.GetLeadItem(entityId.Value);
                            if (item != null)
                                name += string.Format(" ({0}{1}{2})", item.ArtNo, (!string.IsNullOrEmpty(item.ArtNo) ? ", " : ""), item.Name);
                        }
                        break;
                    }
                    case ChangeHistoryParameterType.BookingItemField:
                    {
                        if (entityId != null)
                        {
                            var item = BookingItemsService.Get(entityId.Value);
                            if (item != null)
                                name += string.Format(" ({0}{1}{2})", item.ArtNo, (!string.IsNullOrEmpty(item.ArtNo) ? ", " : ""), item.Name);
                        }
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            if (!string.IsNullOrEmpty(entityName))
            {
                name += " " + entityName;
            }

            return name;
        }

    }
}
