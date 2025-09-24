using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.SQL;

namespace AdvantShop.Core.Services.Crm.LeadFields
{
    public class LeadFieldService
    {
        #region LeadField

        public static LeadField GetLeadField(int id)
        {
            return SQLDataAccess.Query<LeadField>("SELECT * FROM CRM.LeadField WHERE Id = @Id", new { id }).FirstOrDefault();
        }

        public static List<LeadField> GetLeadFields(bool onlyEnabled = true)
        {
            return GetLeadFields(null, onlyEnabled);
        }

        public static List<LeadField> GetLeadFields(int? salesFunnelId, bool onlyEnabled = true)
        {
            var conditions = new List<string>();
            if (salesFunnelId.HasValue)
                conditions.Add("SalesFunnelId = @SalesFunnelId");
            if (onlyEnabled)
                conditions.Add("Enabled = 1");
            return SQLDataAccess.Query<LeadField>(
                "SELECT * FROM CRM.LeadField " +
                (conditions.Any() ? "WHERE " : "") +
                string.Join(" AND ", conditions) + 
                " ORDER BY SortOrder, Name", 
                new { salesFunnelId }).ToList();
        }

        public static List<LeadFieldWithValue> GetLeadFieldsWithValue(int? leadId, int? salesFunnelId = null)
        {
            if (!leadId.HasValue && !salesFunnelId.HasValue)
                throw new ArgumentNullException();

            if (!leadId.HasValue)
                return SQLDataAccess.Query<LeadFieldWithValue>(
                    "SELECT lf.* " +
                    "FROM CRM.LeadField as lf " +
                    "WHERE lf.Enabled = 1 and lf.SalesFunnelId = @salesFunnelId " +
                    "ORDER BY lf.SortOrder", new { salesFunnelId }).ToList();

            return SQLDataAccess.Query<LeadFieldWithValue>(
                "SELECT lf.*, map.Value " +
                "FROM CRM.LeadField as lf " +
                "INNER JOIN CRM.SalesFunnel ON lf.SalesFunnelId = SalesFunnel.Id " +
                "INNER JOIN [Order].Lead ON Lead.SalesFunnelId = lf.SalesFunnelId " +
                "LEFT JOIN CRM.LeadFieldValuesMap as map ON map.LeadId = @leadId and map.LeadFieldId = lf.Id " +
                "WHERE lf.Enabled = 1 AND Lead.Id = @leadId " +
                "ORDER BY lf.SortOrder",
                new { leadId }).ToList();
        }

        public static List<LeadFieldWithValue> GetMappedLeadFieldsWithValue(int leadId)
        {
            return SQLDataAccess.Query<LeadFieldWithValue>(
                "SELECT lf.*, map.Value " +
                "FROM CRM.LeadField as lf " +
                "INNER JOIN CRM.LeadFieldValuesMap as map ON map.LeadId = @leadId and map.LeadFieldId = lf.Id " +
                "WHERE lf.Enabled = 1 " +
                "ORDER BY lf.SortOrder",
                new { leadId }).ToList();
        }

        public static LeadFieldWithValue GetLeadFieldWithValue(int fieldId)
        {
            return SQLDataAccess.Query<LeadFieldWithValue>("SELECT * FROM CRM.LeadField WHERE Id = @fieldId", new { fieldId }).FirstOrDefault();
        }

        public static LeadFieldWithValue GetLeadFieldWithValue(int leadId, int fieldId)
        {
            return SQLDataAccess.Query<LeadFieldWithValue>(
                "SELECT lf.*, map.Value " +
                "FROM CRM.LeadField as lf " +
                "LEFT JOIN CRM.LeadFieldValuesMap as map ON map.LeadId = @leadId and map.LeadFieldId = lf.Id " +
                "Where lf.Id = @fieldId ", 
                new { fieldId, leadId }).FirstOrDefault();
        }

        public static void AddLeadField(LeadField field)
        {
            field.Id = SQLDataAccess.ExecuteScalar<int>(
                "INSERT INTO CRM.LeadField (Name, FieldType, SortOrder, Required, Enabled, SalesFunnelId) " +
                "VALUES (@Name, @FieldType, @SortOrder, @Required, @Enabled, @SalesFunnelId); SELECT SCOPE_IDENTITY();",
                CommandType.Text,
                new SqlParameter("@Name", field.Name),
                new SqlParameter("@FieldType", field.FieldType),
                new SqlParameter("@SortOrder", field.SortOrder),
                new SqlParameter("@Required", field.Required),
                new SqlParameter("@Enabled", field.Enabled),
                new SqlParameter("@SalesFunnelId", field.SalesFunnelId)
                );
        }

        public static void UpdateLeadField(LeadField field)
        {
            SQLDataAccess.ExecuteNonQuery(
                "UPDATE CRM.LeadField " +
                "SET Name = @Name, FieldType = @FieldType, SortOrder = @SortOrder, Required = @Required, Enabled = @Enabled, SalesFunnelId = @SalesFunnelId " +
                "WHERE Id = @Id",
                CommandType.Text,
                new SqlParameter("@Id", field.Id),
                new SqlParameter("@Name", field.Name),
                new SqlParameter("@FieldType", field.FieldType),
                new SqlParameter("@SortOrder", field.SortOrder),
                new SqlParameter("@Required", field.Required),
                new SqlParameter("@Enabled", field.Enabled),
                new SqlParameter("@SalesFunnelId", field.SalesFunnelId)
                );
        }

        public static void DeleteLeadField(int id)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM CRM.LeadField WHERE Id = @Id", CommandType.Text, new SqlParameter("@Id", id));
        }

        #endregion

        #region LeadFieldValue

        public static LeadFieldValue GetLeadFieldValue(int valueId)
        {
            return SQLDataAccess.Query<LeadFieldValue>("SELECT * FROM CRM.LeadFieldValue WHERE Id = @valueId", new { valueId }).FirstOrDefault();
        }

        public static List<LeadFieldValue> GetLeadFieldValues(int fieldId)
        {
            return SQLDataAccess.Query<LeadFieldValue>("SELECT * FROM CRM.LeadFieldValue WHERE LeadFieldId = @fieldId ORDER BY SortOrder, Value", new { fieldId }).ToList();
        }

        public static void AddLeadFieldValue(LeadFieldValue value)
        {
            value.Id = SQLDataAccess.ExecuteScalar<int>(
                "INSERT INTO CRM.LeadFieldValue (Value, LeadFieldId, SortOrder) VALUES (@Value, @LeadFieldId, @SortOrder); SELECT SCOPE_IDENTITY();",
                CommandType.Text,
                new SqlParameter("@Value", value.Value),
                new SqlParameter("@LeadFieldId", value.LeadFieldId),
                new SqlParameter("@SortOrder", value.SortOrder)
                );
        }

        public static void UpdateLeadFieldValue(LeadFieldValue value)
        {
            SQLDataAccess.ExecuteNonQuery(
                "UPDATE CRM.LeadFieldValue SET Value = @Value, LeadFieldId = @LeadFieldId, SortOrder = @SortOrder WHERE Id = @Id",
                CommandType.Text,
                new SqlParameter("@Id", value.Id),
                new SqlParameter("@Value", value.Value),
                new SqlParameter("@LeadFieldId", value.LeadFieldId),
                new SqlParameter("@SortOrder", value.SortOrder)
                );
        }

        public static void DeleteLeadFieldValue(int valueId)
        {
            SQLDataAccess.ExecuteNonQuery(
                "DELETE FROM CRM.LeadFieldValue WHERE Id = @Id",
                CommandType.Text,
                new SqlParameter("@Id", valueId));
        }

        public static void DeleteLeadFieldValues(int fieldId)
        {
            SQLDataAccess.ExecuteNonQuery(
                "DELETE FROM CRM.LeadFieldValue WHERE LeadFieldId = @LeadFieldId",
                CommandType.Text,
                new SqlParameter("@LeadFieldId", fieldId));
        }

        #endregion

        #region LeadFieldValuesMap

        public static bool IsLeadFieldDefined(int leadId, int fieldId)
        {
            return SQLDataAccess.ExecuteScalar<bool>(
                "IF EXISTS (SELECT 1 FROM CRM.LeadFieldValuesMap WHERE LeadId = @LeadId AND LeadFieldId = @LeadFieldId AND Value IS NOT NULL AND Value <> '') SELECT 1 ELSE SELECT 0",
                CommandType.Text,
                new SqlParameter("@LeadId", leadId),
                new SqlParameter("@LeadFieldId", fieldId));
        }

        public static void DeleteFieldsMap(int leadId)
        {
            SQLDataAccess.ExecuteNonQuery(
                "DELETE FROM CRM.LeadFieldValuesMap WHERE LeadId = @LeadId",
                CommandType.Text,
                new SqlParameter("@LeadId", leadId)
                );
        }

        public static void DeleteFieldMap(int leadId, int fieldId)
        {
            SQLDataAccess.ExecuteNonQuery(
                "DELETE FROM CRM.LeadFieldValuesMap WHERE LeadId = @LeadId AND LeadFieldId = @LeadFieldId",
                CommandType.Text, 
                new SqlParameter("@LeadId", leadId),
                new SqlParameter("@LeadFieldId", fieldId)
                );
        }

        public static void UpdateFieldsMapValues(int fieldId, string value)
        {
            SQLDataAccess.ExecuteNonQuery(
                "UPDATE CRM.LeadFieldValuesMap SET Value = @Value WHERE LeadFieldId = @LeadFieldId",
                CommandType.Text,
                new SqlParameter("@LeadFieldId", fieldId),
                new SqlParameter("@Value", value)
                );
        }

        public static void AddUpdateMap(int leadId, int fieldId, string value)
        {
            SQLDataAccess.ExecuteNonQuery(
                "IF EXISTS (SELECT 1 FROM CRM.LeadFieldValuesMap WHERE LeadId = @LeadId AND LeadFieldId = @LeadFieldId) " +
                "UPDATE CRM.LeadFieldValuesMap SET Value = @Value WHERE LeadId = @LeadId AND LeadFieldId = @LeadFieldId " +
                "ELSE INSERT INTO CRM.LeadFieldValuesMap (LeadId, LeadFieldId, Value) VALUES (@LeadId, @LeadFieldId, @Value) ",
                CommandType.Text, 
                new SqlParameter("@LeadId", leadId),
                new SqlParameter("@LeadFieldId", fieldId),
                new SqlParameter("@Value", value.DefaultOrEmpty())
                );
        }

        #endregion
    }
}
