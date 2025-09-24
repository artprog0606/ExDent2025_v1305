using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.Services.Crm.BusinessProcesses;
using AdvantShop.Core.Services.Crm.BusinessProcesses.Customers;
using AdvantShop.Core.Services.Crm.LeadFields;
using AdvantShop.Core.Services.Triggers.Customers;
using AdvantShop.Core.Services.Triggers.Leads;
using AdvantShop.Core.Services.Triggers.Orders;
using AdvantShop.Core.SQL;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using Newtonsoft.Json;

namespace AdvantShop.Core.Services.Triggers
{
    public static class TriggerRuleService
    {
        public static ITriggerObjectFilter GetTriggerFilterFromJson<T>(string json) where T : TriggerRule, new()
        {
            if (string.IsNullOrEmpty(json))
                return null;

            var jsonObject = JsonConvert.DeserializeObject<dynamic>(json);
            var jsonFieldComparers = jsonObject["Comparers"];

            var objectType = new T().ObjectType;
            switch (objectType)
            {
                case ETriggerObjectType.Order:
                    var orderFilter = new OrderFilter();

                    foreach (var jsonOrderFieldComparer in jsonFieldComparers)
                    {
                        var orderFieldComparer = new OrderFieldComparer
                        {
                            FieldType = (EOrderFieldType) jsonOrderFieldComparer["FieldType"],
                            CompareType =
                                jsonOrderFieldComparer["CompareType"] != null
                                    ? (BizObjectFieldCompareType) jsonOrderFieldComparer["CompareType"]
                                    : BizObjectFieldCompareType.Equal
                        };

                        var jsonFieldComparer = jsonOrderFieldComparer["FieldComparer"];    // FieldComparer FieldComparer

                        FieldComparer fieldComparer;
                        if (GetFieldComparerFromJson(jsonFieldComparer, orderFieldComparer.FieldType, orderFieldComparer.FieldType == EOrderFieldType.CustomerField, false, out fieldComparer))
                        {
                            orderFieldComparer.FieldComparer = fieldComparer;
                            orderFilter.Comparers.Add(orderFieldComparer);
                        }
                    }
                    return orderFilter;

                case ETriggerObjectType.Lead:
                    var leadFilter = new LeadFilter();

                    foreach (var jsonLeadFieldComparer in jsonFieldComparers)
                    {
                        var leadFieldComparer = new LeadFieldComparer
                        {
                            FieldType = (ELeadFieldType) jsonLeadFieldComparer["FieldType"],
                            CompareType =
                                jsonLeadFieldComparer["CompareType"] != null
                                    ? (BizObjectFieldCompareType) jsonLeadFieldComparer["CompareType"]
                                    : BizObjectFieldCompareType.Equal
                        };

                        var jsonFieldComparer = jsonLeadFieldComparer["FieldComparer"];    // FieldComparer FieldComparer

                        FieldComparer fieldComparer;
                        if (GetFieldComparerFromJson(jsonFieldComparer, leadFieldComparer.FieldType, leadFieldComparer.FieldType == ELeadFieldType.CustomerField, leadFieldComparer.FieldType == ELeadFieldType.LeadField, out fieldComparer))
                        {
                            leadFieldComparer.FieldComparer = fieldComparer;
                            leadFilter.Comparers.Add(leadFieldComparer);
                        }
                    }
                    return leadFilter;

                case ETriggerObjectType.Customer:
                    var customerFilter = new CustomerTriggerFilter();

                    foreach (var jsonCustomerFieldComparer in jsonFieldComparers)
                    {
                        var customerFieldComparer = new CustomerFieldComparer
                        {
                            FieldType = (ECustomerFieldType) jsonCustomerFieldComparer["FieldType"],
                            CompareType =
                                jsonCustomerFieldComparer["CompareType"] != null
                                    ? (BizObjectFieldCompareType) jsonCustomerFieldComparer["CompareType"]
                                    : BizObjectFieldCompareType.Equal
                        };

                        var jsonFieldComparer = jsonCustomerFieldComparer["FieldComparer"];    // FieldComparer FieldComparer

                        FieldComparer fieldComparer;
                        if (GetFieldComparerFromJson(jsonFieldComparer, customerFieldComparer.FieldType, customerFieldComparer.FieldType == ECustomerFieldType.CustomerField, false, out fieldComparer))
                        {
                            customerFieldComparer.FieldComparer = fieldComparer;
                            customerFilter.Comparers.Add(customerFieldComparer);
                        }
                    }
                    return customerFilter;
                    
                default:
                    throw new NotImplementedException("No implementation for object type " + objectType);
            }
        }

        private static bool GetFieldComparerFromJson(dynamic jsonFieldComparer, Enum bizObjFieldType, bool isCustomerField, bool isLeadField, out FieldComparer fieldComparer)
        {
            EFieldType? fieldType = null;
            var fieldObjId = (int?)jsonFieldComparer["FieldObjId"];
            if (isCustomerField && fieldObjId.HasValue)
                fieldType = GetCustomerFieldType(fieldObjId.Value);
            else if (isLeadField && fieldObjId.HasValue)
                fieldType = GetLeadFieldType(fieldObjId.Value);
            else
                fieldType = GetFieldType(bizObjFieldType);

            if (!fieldType.HasValue)
            {
                fieldComparer = null;
                return false;
            }

            var fieldComparerType = (EFieldComparerType)jsonFieldComparer["Type"];  // EFieldComparerType Type
            switch (fieldComparerType)
            {
                case EFieldComparerType.Equal:
                    var value = jsonFieldComparer["Value"];
                    fieldComparer = new FieldEqualityComparer
                    {
                        Value = value,             // string Value
                    };
                    break;
                case EFieldComparerType.Range:
                    if (fieldType == EFieldType.Date || fieldType == EFieldType.Datetime || fieldType == EFieldType.Time)
                    {
                        var dateFrom = (object)jsonFieldComparer["DateFrom"];
                        var dateTo = (object)jsonFieldComparer["DateTo"];
                        fieldComparer = new FieldRangeComparer
                        {
                            DateFrom = dateFrom != null ? dateFrom.ToString().TryParseDateTime(true) : null,   // DateTime? From 
                            DateTo = dateTo != null ? dateTo.ToString().TryParseDateTime(true) : null,         // DateTime? To
                            ShowTime = fieldType == EFieldType.Datetime,
                            OnlyTime = fieldType == EFieldType.Time
                        };
                    }
                    else
                    {
                        var from = (object)jsonFieldComparer["From"];
                        var to = (object)jsonFieldComparer["To"];
                        fieldComparer = new FieldRangeComparer
                        {
                            From = from != null ? from.ToString().TryParseFloat(true) : null,       // float? From 
                            To = to != null ? to.ToString().TryParseFloat(true) : null              // float? To
                        };
                    }
                    break;
                case EFieldComparerType.Flag:
                    var flag = (object)jsonFieldComparer["Flag"];
                    fieldComparer = new FieldFlagComparer
                    {
                        Flag = flag != null ? flag.ToString().TryParseBool() : false,       // bool From 
                    };
                    break;
                case EFieldComparerType.Contains:
                    var valueContains = jsonFieldComparer["Value"];
                    fieldComparer = new FieldContainsComparer
                    {
                        Value = valueContains,       // string Value
                    };
                    break;

                case EFieldComparerType.Products:
                    List<FieldItemModel> products = null;
                    try
                    {
                        products = jsonFieldComparer["Products"].ToObject(typeof(List<FieldItemModel>));
                    }
                    catch
                    {
                    }
                    fieldComparer = new FieldsProductsComparer() {Products = products};
                    break;

                case EFieldComparerType.Categories:
                    List<FieldItemModel> categories = null;
                    try
                    {
                        categories = jsonFieldComparer["Categories"].ToObject(typeof(List<FieldItemModel>));
                    }
                    catch
                    {
                    }
                    fieldComparer = new FieldsCategoriesComparer() { Categories = categories };
                    break;

                case EFieldComparerType.CustomerSegment:
                    fieldComparer = new FieldsCustomerSegmentComparer() {Value = Convert.ToInt32(jsonFieldComparer["Value"]) };
                    break;

                case EFieldComparerType.OrdersPaidSum:
                {
                    var from = (object) jsonFieldComparer["From"];
                    var to = (object) jsonFieldComparer["To"];

                    fieldComparer = new FieldsOrdersCustomerSumCountComparer()
                    {
                        SubType = EFieldComparerOrdersCustomerSubType.PaidSum,
                        From = from != null ? from.ToString().TryParseFloat(true) : null,
                        To = to != null ? to.ToString().TryParseFloat(true) : null
                    };
                    break;
                }

                case EFieldComparerType.OrdersCount:
                {
                    var from = (object)jsonFieldComparer["From"];
                    var to = (object)jsonFieldComparer["To"];

                    fieldComparer = new FieldsOrdersCustomerSumCountComparer()
                    {
                        SubType = EFieldComparerOrdersCustomerSubType.Count,
                        From = from != null ? from.ToString().TryParseFloat(true) : null,
                        To = to != null ? to.ToString().TryParseFloat(true) : null
                    };
                    break;
                }

                case EFieldComparerType.OrdersPaidCount:
                {
                    var from = (object)jsonFieldComparer["From"];
                    var to = (object)jsonFieldComparer["To"];

                    fieldComparer = new FieldsOrdersCustomerSumCountComparer()
                    {
                        SubType = EFieldComparerOrdersCustomerSubType.PaidCount,
                        From = from != null ? from.ToString().TryParseFloat(true) : null,
                        To = to != null ? to.ToString().TryParseFloat(true) : null
                    };
                    break;
                }

                case EFieldComparerType.OpenLeadsInFunnel:
                {
                    var salesFunnelId = (object)jsonFieldComparer["SalesFunnelId"];
                    var dealStatusId = jsonFieldComparer["DealStatusId"] != null ? (object)jsonFieldComparer["DealStatusId"] : null;

                    fieldComparer = new FieldsOpenLeadsInFunnelComparer()
                    {
                        SalesFunnelId = salesFunnelId != null ? Convert.ToInt32(salesFunnelId) : 0,
                        DealStatusId = dealStatusId != null ? dealStatusId.ToString().TryParseInt(true) : default(int?)
                    };
                    break;
                }

                case EFieldComparerType.DealStatus:
                {
                    var salesFunnelId = (object) jsonFieldComparer["SalesFunnelId"];
                    var dealStatusId = jsonFieldComparer["DealStatusId"] != null ?(object) jsonFieldComparer["DealStatusId"] : null;

                    fieldComparer = new FieldsDealStatusComparer()
                    {
                        SalesFunnelId = salesFunnelId != null ? Convert.ToInt32(salesFunnelId) : 0,
                        DealStatusId = dealStatusId != null ? dealStatusId.ToString().TryParseInt(true) : default(int?)
                    };
                    break;
                }

                default:
                    fieldComparer = null;
                    return false;
            }
            fieldComparer.FieldObjId = jsonFieldComparer["FieldObjId"];      // int? FieldObjId
            fieldComparer.ValueObjId = jsonFieldComparer["ValueObjId"];      // int? ValueObjId

            return true;
        }

        public static ITriggerParams GetTriggerParams(string json, ETriggerEventType type)
        {
            switch (type)
            {
                case ETriggerEventType.SignificantDate:
                    return JsonConvert.DeserializeObject<TriggerParamsDate>(json);

                case ETriggerEventType.SignificantCustomerDate:
                    return JsonConvert.DeserializeObject<TriggerParamsDaysBeforeDate>(json);
            }
            return null;
        }
        
        public static EFieldType GetFieldType(Enum objFieldType)
        {
            return objFieldType.FieldTypeValue().Type;
        }

        public static EFieldType? GetCustomerFieldType(int fieldObjId)
        {
            var customerField = CustomerFieldService.GetCustomerField(fieldObjId);
            return customerField != null ? customerField.FieldType.FieldType() : (EFieldType?)null;
        }


        public static EFieldType? GetLeadFieldType(int fieldObjId)
        {
            var leadField = LeadFieldService.GetLeadField(fieldObjId);
            return leadField != null ? leadField.FieldType.FieldType() : (EFieldType?)null;
        }

        public static T Get<T>(int id) where T : TriggerRule, new()
        {
            return SQLDataAccess.ExecuteReadOne<T>("SELECT * FROM CRM.TriggerRule WHERE Id = @Id", CommandType.Text,
                GetTriggerRuleFromReader<T>, new SqlParameter("@Id", id));
        }

        public static TriggerRule GetTrigger(int id)
        {
            return SQLDataAccess.ExecuteReadOne<TriggerRule>("SELECT * FROM CRM.TriggerRule WHERE Id = @Id",
                CommandType.Text, GetTriggerRuleFromReaderByType,
                new SqlParameter("@Id", id));
        }

        public static List<TriggerRule> GetTriggersByType(ETriggerEventType eventType)
        {
            return SQLDataAccess.ExecuteReadList("SELECT * FROM CRM.TriggerRule WHERE EventType = @EventType and Enabled = 1",
                CommandType.Text, GetTriggerRuleFromReaderByType,
                new SqlParameter("@EventType", (int)eventType));
        }

        public static List<TriggerRule> GetTriggersByProcessType(ETriggerProcessType processType, bool onlyEnabled = true)
        {
            return SQLDataAccess.ExecuteReadList(
                "SELECT * FROM CRM.TriggerRule WHERE ProcessType = @ProcessType" + (onlyEnabled ? " and Enabled = 1" : ""),
                CommandType.Text, 
                GetTriggerRuleFromReaderByType,
                new SqlParameter("@ProcessType", (int)processType));
        }

        public static List<TriggerRule> GetTriggersByObjectType(ETriggerObjectType objectType)
        {
            return SQLDataAccess.ExecuteReadList("SELECT * FROM CRM.TriggerRule WHERE ObjectType = @ObjectType and Enabled = 1",
                CommandType.Text, GetTriggerRuleFromReaderByType,
                new SqlParameter("@ObjectType", (int)objectType));
        }

        private static TriggerRule GetTriggerRuleFromReaderByType(SqlDataReader reader)
        {
            var eventType = (ETriggerEventType)SQLDataHelper.GetInt(reader, "EventType");
            switch (eventType)
            {
                case ETriggerEventType.OrderCreated:
                    return GetTriggerRuleFromReader<OrderCreatedTriggerRule>(reader);

                case ETriggerEventType.OrderStatusChanged:
                    return GetTriggerRuleFromReader<OrderStatusChangedTriggerRule>(reader);

                case ETriggerEventType.OrderPaied:
                    return GetTriggerRuleFromReader<OrderPayTriggerRule>(reader);

                case ETriggerEventType.LeadCreated:
                    return GetTriggerRuleFromReader<LeadCreatedTriggerRule>(reader);

                case ETriggerEventType.LeadStatusChanged:
                    return GetTriggerRuleFromReader<LeadStatusChangedTriggerRule>(reader);

                case ETriggerEventType.CustomerCreated:
                    return GetTriggerRuleFromReader<CustomerCreatedTriggerRule>(reader);

                case ETriggerEventType.TimeFromLastOrder:
                    return GetTriggerRuleFromReader<TimeFromLastOrderTriggerRule>(reader);

                case ETriggerEventType.SignificantDate:
                    return GetTriggerRuleFromReader<SignificantDateTriggerRule>(reader);

                case ETriggerEventType.SignificantCustomerDate:
                    return GetTriggerRuleFromReader<SignificantCustomerDateTriggerRule>(reader);

                case ETriggerEventType.InstallMobileApp:
                    return GetTriggerRuleFromReader<InstallMobileAppTriggerRule>(reader);

                default:
                    throw new NotImplementedException();
            }
        }

        private static T GetTriggerRuleFromReader<T>(SqlDataReader reader) where T : TriggerRule, new()
        {
            var rule = new T
            {
                Id = SQLDataHelper.GetInt(reader, "Id"),
                EventObjId = SQLDataHelper.GetNullableInt(reader, "EventObjId"),
                EventObjValue = SQLDataHelper.GetNullableInt(reader, "EventObjValue"),
                CategoryId = SQLDataHelper.GetNullableInt(reader, "CategoryId"),
                Name = SQLDataHelper.GetString(reader, "Name"),
                Enabled = SQLDataHelper.GetBoolean(reader, "Enabled"),
                WorksOnlyOnce = SQLDataHelper.GetBoolean(reader, "WorksOnlyOnce"),
                DateCreated = SQLDataHelper.GetDateTime(reader, "DateCreated"),
                DateModified = SQLDataHelper.GetDateTime(reader, "DateModified"),
                PreferredHour = SQLDataHelper.GetNullableInt(reader, "PreferredHour"),

                Filter = GetTriggerFilterFromJson<T>(SQLDataHelper.GetString(reader, "Filter"))
            };

            var triggerParams = SQLDataHelper.GetString(reader, "TriggerParams");

            if (!string.IsNullOrEmpty(triggerParams))
                rule.TriggerParams = GetTriggerParams(triggerParams, rule.EventType);
            
            return rule;
        }

        public static bool HasTriggerRulesOfCategory(int categoryId)
        {
            return
                SQLDataAccess.ExecuteScalar<bool>(
                    "SELECT CASE WHEN EXISTS(SELECT 1 FROM CRM.TriggerRule WHERE CategoryId = @CategoryId) THEN 1 ELSE 0 END",
                    CommandType.Text, new SqlParameter("@CategoryId", categoryId));
        }

        public static List<TriggerRuleShortDto> GetTriggerRules()
        {
            return SQLDataAccess.Query<TriggerRuleShortDto>("Select * From CRM.TriggerRule Order by DateCreated").ToList();
        }

        public static int Add(TriggerRule rule)
        {
            rule.Id =
                SQLDataAccess.ExecuteScalar<int>(
                    "Insert Into CRM.TriggerRule (EventType, ObjectType, EventObjId, Name, Filter, DateCreated, DateModified, Enabled, WorksOnlyOnce, ProcessType, EventObjValue, TriggerParams, PreferredHour, CategoryId) " +
                    "Values (@EventType, @ObjectType, @EventObjId, @Name, @Filter, GETDATE(), GETDATE(), @Enabled, @WorksOnlyOnce, @ProcessType, @EventObjValue, @TriggerParams, @PreferredHour, @CategoryId); Select scope_identity();",
                    CommandType.Text,
                    new SqlParameter("@EventType", rule.EventType),
                    new SqlParameter("@ObjectType", rule.ObjectType),
                    new SqlParameter("@EventObjId", rule.EventObjId ?? (object) DBNull.Value),
                    new SqlParameter("@EventObjValue", rule.EventObjValue ?? (object) DBNull.Value),
                    new SqlParameter("@CategoryId", rule.CategoryId ?? (object) DBNull.Value),
                    new SqlParameter("@Name", rule.Name),
                    new SqlParameter("@Filter",
                        rule.Filter != null ? JsonConvert.SerializeObject(rule.Filter) : (object) DBNull.Value),
                    new SqlParameter("@Enabled", rule.Enabled),
                    new SqlParameter("@WorksOnlyOnce", rule.WorksOnlyOnce),
                    new SqlParameter("@ProcessType", rule.ProcessType),
                    new SqlParameter("@TriggerParams",
                        rule.TriggerParams != null ? JsonConvert.SerializeObject(rule.TriggerParams) : (object) DBNull.Value),
                    new SqlParameter("@PreferredHour", rule.PreferredHour ?? (object)DBNull.Value)
                );

            return rule.Id;
        }

        public static void Update(TriggerRule rule)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Update CRM.TriggerRule " +
                "Set EventType=@EventType, ObjectType=@ObjectType, EventObjId=@EventObjId, Name=@Name, Filter=@Filter, DateModified=GETDATE(), " +
                "Enabled=@Enabled, WorksOnlyOnce=@WorksOnlyOnce, EventObjValue=@EventObjValue, ProcessType=@ProcessType, TriggerParams=@TriggerParams, PreferredHour=@PreferredHour, " +
                "CategoryId=@CategoryId " +
                "Where Id=@Id",
                CommandType.Text,
                new SqlParameter("@Id", rule.Id),
                new SqlParameter("@EventType", rule.EventType),
                new SqlParameter("@ObjectType", rule.ObjectType),
                new SqlParameter("@EventObjId", rule.EventObjId ?? (object) DBNull.Value),
                new SqlParameter("@EventObjValue", rule.EventObjValue ?? (object)DBNull.Value),
                new SqlParameter("@CategoryId", rule.CategoryId ?? (object)DBNull.Value),
                new SqlParameter("@Name", rule.Name),
                new SqlParameter("@Filter",
                    rule.Filter != null ? JsonConvert.SerializeObject(rule.Filter) : (object) DBNull.Value),
                new SqlParameter("@Enabled", rule.Enabled),
                new SqlParameter("@WorksOnlyOnce", rule.WorksOnlyOnce),
                new SqlParameter("@ProcessType", rule.ProcessType),
                new SqlParameter("@TriggerParams",
                    rule.TriggerParams != null ? JsonConvert.SerializeObject(rule.TriggerParams) : (object)DBNull.Value),
                new SqlParameter("@PreferredHour", rule.PreferredHour ?? (object)DBNull.Value)
            );
        }

        public static void Delete(int id)
        {
            var actions = TriggerActionService.GetTriggerActions(id);

            foreach (var action in actions)
                foreach (var coupon in CouponService.GetCouponsByTriggerAction(action.Id))
                    CouponService.DeleteCoupon(coupon.CouponID);

            var couponTemplateByTrigger = CouponService.GetCouponByTrigger(id);
            if (couponTemplateByTrigger != null)
                CouponService.DeleteCoupon(couponTemplateByTrigger.CouponID);

            SQLDataAccess.ExecuteNonQuery("Delete From CRM.TriggerRule Where Id=@Id", CommandType.Text, new SqlParameter("@Id", id));
        }

        public static ITriggerObject GetTriggerObject(ETriggerObjectType objectType, int entityId)
        {
            switch (objectType)
            {
                case ETriggerObjectType.Order:
                    return OrderService.GetOrder(entityId);

                case ETriggerObjectType.Lead:
                    return LeadService.GetLead(entityId);

                case ETriggerObjectType.Customer:
                    return CustomerService.GetCustomer(entityId);

                default:
                    throw new NotImplementedException();
            }
        }

        public static void SetActive(int id, bool enabled)
        {
            SQLDataAccess.ExecuteNonQuery(
                 "Update [CRM].[TriggerRule] Set Enabled = @Enabled Where Id = @Id",
                 CommandType.Text,
                 new SqlParameter("@Id", id),
                 new SqlParameter("@Enabled", enabled));
        }

        public static void SetName(int id, string name)
        {
            SQLDataAccess.ExecuteNonQuery(
                 "Update [CRM].[TriggerRule] Set Name = @Name Where Id = @Id",
                 CommandType.Text,
                 new SqlParameter("@Id", id),
                 new SqlParameter("@Name", name));
        }

        public static int GetTriggersCount(bool onlyEnabled = true)
        {
            return SQLDataAccess.ExecuteScalar<int>("SELECT count(*) FROM CRM.TriggerRule" + (onlyEnabled ? " where [Enabled] = 1" : ""), CommandType.Text);
        }


        public static void DeactivateTriggersMoreThan(int activeTriggersCount)
        {
            if (activeTriggersCount <= 0)
                return;

            SQLDataAccess.ExecuteNonQuery(
                @"if(Select (Count([Id])) From [CRM].[TriggerRule] Where Enabled = 1 ) > @triggersNumber
                  BEGIN
                    ;WITH triggersToDeactivate AS 
                    ( 
	                    Select 
	                    Top(Select (Count(Id) - @triggersNumber) From [CRM].[TriggerRule] Where Enabled = 1) Id
	                    From [CRM].[TriggerRule]
	                    Where Enabled = 1 
	                    Order by [DateCreated] Desc
                    ) 
                    UPDATE [CRM].[TriggerRule] SET Enabled = 0 Where Id in (Select Id from triggersToDeactivate)
                  END",
                CommandType.Text,
                new SqlParameter("@triggersNumber", activeTriggersCount));
        }


    }
}
