using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.ChangeHistories;
using AdvantShop.Core.Services.Customers;
using AdvantShop.Core.SQL;

namespace AdvantShop.Customers
{
    public class CustomerFieldService
    {
        #region CustomerField

        public static CustomerField GetCustomerField(int id)
        {
            return SQLDataAccess.Query<CustomerField>("SELECT * FROM Customers.CustomerField WHERE Id = @Id", new { id }).FirstOrDefault();
        }

        public static List<CustomerField> GetCustomerFields(bool enabled = true, CustomerType? customerType = null)
        {
            string condition = string.Empty;
            if (enabled)
                condition = "Where Enabled = 1" + (customerType.HasValue ? " and CustomerType = " + (int)customerType : "");  
            else if (customerType.HasValue)
                condition = "Where CustomerType = " + (int)customerType;
            return SQLDataAccess.Query<CustomerField>($"SELECT * FROM Customers.CustomerField {condition} ORDER BY SortOrder, Name").ToList();
        }

        public static CustomerField GetCustomerFieldByFieldAssignment(CustomerFieldAssignment fieldAssignment, bool enabled = true)
        {
            return SQLDataAccess.Query<CustomerField>("SELECT * FROM Customers.CustomerField Where FieldAssignment = @fieldAssignment " + (enabled ? "and Enabled = 1 " : ""),
                new { fieldAssignment }).FirstOrDefault();
        }

        public static CustomerFieldWithValue GetCustomerFieldWithValueByFieldAssignment(Guid customerId, CustomerFieldAssignment fieldAssignment)
        {
            return SQLDataAccess.Query<CustomerFieldWithValue>(
                "SELECT cf.*, map.Value " +
                "FROM Customers.CustomerField as cf " +
                "LEFT JOIN Customers.CustomerFieldValuesMap as map ON map.CustomerId = @customerId and map.CustomerFieldId = cf.Id " +
                "WHERE cf.Enabled = 1 and cf.FieldAssignment = @fieldAssignment",
                new { fieldAssignment, customerId }).FirstOrDefault();
        }

        public static List<CustomerFieldWithValue> GetCustomerFieldsWithValue(Guid customerId)
        {
            if (customerId == Guid.Empty)
                return SQLDataAccess.Query<CustomerFieldWithValue>(
                    "SELECT cf.* " +
                    "FROM Customers.CustomerField as cf " +
                    "WHERE cf.Enabled = 1 " +
                    "ORDER BY cf.SortOrder",
                    new { customerId }).ToList();

            return SQLDataAccess.Query<CustomerFieldWithValue>(
                "SELECT cf.*, map.Value " +
                "FROM Customers.CustomerField as cf " +
                "LEFT JOIN Customers.CustomerFieldValuesMap as map ON map.CustomerId = @customerId and map.CustomerFieldId = cf.Id " +
                "WHERE cf.Enabled = 1 " +
                "ORDER BY cf.SortOrder",
                new { customerId }).ToList();
        }

        public static List<CustomerFieldWithValue> GetMappedCustomerFieldsWithValue(Guid customerId)
        {
            return SQLDataAccess.Query<CustomerFieldWithValue>(
                "SELECT cf.*, map.Value " +
                "FROM Customers.CustomerField as cf " +
                "INNER JOIN Customers.CustomerFieldValuesMap as map ON map.CustomerId = @customerId and map.CustomerFieldId = cf.Id " +
                "WHERE cf.Enabled = 1 " +
                "ORDER BY cf.SortOrder",
                new { customerId }).ToList();
        }

        public static CustomerFieldWithValue GetCustomerFieldsWithValue(int customerFieldId)
        {
            return
                SQLDataAccess.Query<CustomerFieldWithValue>("SELECT * FROM Customers.CustomerField WHERE Id = @customerFieldId", new { customerFieldId })
                    .FirstOrDefault();
        }

        public static CustomerFieldWithValue GetCustomerFieldsWithValue(Guid customerId, int customerFieldId)
        {
            return
                SQLDataAccess.Query<CustomerFieldWithValue>(
                        "SELECT cf.*, map.Value " +
                        "FROM Customers.CustomerField as cf " +
                        "LEFT JOIN Customers.CustomerFieldValuesMap as map ON map.CustomerId = @customerId and map.CustomerFieldId = cf.Id " +
                        "Where cf.Id = @customerFieldId ", 
                        new { customerId, customerFieldId })
                    .FirstOrDefault();
        }

        public static int AddCustomerField(CustomerField field)
        {
            return SQLDataAccess.ExecuteScalar<int>(
                "INSERT INTO Customers.CustomerField (Name, FieldType, SortOrder, Required, Enabled, ShowInRegistration, ShowInCheckout, DisableCustomerEditing, CustomerType, FieldAssignment, ShowInUserEditing) " +
                "VALUES (@Name, @FieldType, @SortOrder, @Required, @Enabled, @ShowInRegistration, @ShowInCheckout, @DisableCustomerEditing, @CustomerType, @FieldAssignment, @ShowInUserEditing); " +
                "SELECT SCOPE_IDENTITY();",
                CommandType.Text,
                new SqlParameter("@Name", field.Name),
                new SqlParameter("@FieldType", field.FieldType),
                new SqlParameter("@SortOrder", field.SortOrder),
                new SqlParameter("@Required", field.Required),
                new SqlParameter("@Enabled", field.Enabled),
                new SqlParameter("@ShowInRegistration", field.ShowInRegistration),
                new SqlParameter("@ShowInCheckout", field.ShowInCheckout),
                new SqlParameter("@DisableCustomerEditing", field.DisableCustomerEditing),
                new SqlParameter("@CustomerType", field.CustomerType),
                new SqlParameter("@FieldAssignment", field.FieldAssignment),
                new SqlParameter("@ShowInUserEditing", field.ShowInUserEditing)
                );
        }

        public static void UpdateCustomerField(CustomerField field)
        {
            SQLDataAccess.ExecuteNonQuery(
                "UPDATE Customers.CustomerField " +
                "SET Name = @Name, FieldType = @FieldType, SortOrder = @SortOrder, Required = @Required, Enabled = @Enabled, ShowInRegistration = @ShowInRegistration, " +
                "ShowInCheckout = @ShowInCheckout, DisableCustomerEditing = @DisableCustomerEditing, CustomerType = @CustomerType, FieldAssignment = @FieldAssignment, ShowInUserEditing = @ShowInUserEditing " +
                "WHERE Id = @Id",
                CommandType.Text,
                new SqlParameter("@Id", field.Id),
                new SqlParameter("@Name", field.Name),
                new SqlParameter("@FieldType", field.FieldType),
                new SqlParameter("@SortOrder", field.SortOrder),
                new SqlParameter("@Required", field.Required),
                new SqlParameter("@Enabled", field.Enabled),
                new SqlParameter("@ShowInRegistration", field.ShowInRegistration),
                new SqlParameter("@ShowInCheckout", field.ShowInCheckout),
                new SqlParameter("@DisableCustomerEditing", field.DisableCustomerEditing),
                new SqlParameter("@CustomerType", field.CustomerType),
                new SqlParameter("@FieldAssignment", field.FieldAssignment),
                new SqlParameter("@ShowInUserEditing", field.ShowInUserEditing)
                );
        }

        public static void DeleteCustomerField(int id)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM Customers.CustomerField WHERE Id = @Id", CommandType.Text, new SqlParameter("@Id", id));
        }

        public static void DisableAllCustomerFields()
        {
            SQLDataAccess.ExecuteNonQuery(@"Update Customers.CustomerField set Enabled = 0", CommandType.Text);
        }

        public static void AddCustomerFieldForLegalEntity()
        {
            SQLDataAccess.ExecuteNonQuery(
                @"if not Exists(Select 1 From Customers.CustomerField Where CustomerType = 1) 
                    INSERT INTO Customers.CustomerField 
                    (Name, FieldType, SortOrder, Required, Enabled, ShowInRegistration, ShowInCheckout, DisableCustomerEditing, CustomerType, FieldAssignment, ShowInUserEditing)
                    VALUES 
                    ('Название организации', 1, 0, 1, 1, 1, 1, 0, 1, 1, 0), 
                    ('Юридический адрес', 1, 0, 0, 1, 1, 1, 0, 1, 2, 0), 
                    ('ИНН', 2, 0, 1, 1, 1, 1, 0, 1, 3, 0), 
                    ('КПП', 2, 0, 0, 1, 1, 1, 0, 1, 4, 0), 
                    ('ОГРН', 2, 0, 0, 1, 1, 1, 0, 1, 5, 0), 
                    ('ОКПО', 2, 0, 0, 1, 1, 1, 0, 1, 6, 0), 
                    ('БИК', 2, 0, 0, 1, 1, 1, 0, 1, 7, 0), 
                    ('Название банка', 1, 0, 0, 1, 1, 1, 0, 1, 8, 0), 
                    ('Корреспондентский счёт', 2, 0, 0, 1, 1, 1, 0, 1, 9, 0), 
                    ('Расчётный счёт', 2, 0, 0, 1, 1, 1, 0, 1, 10, 0)", CommandType.Text
                );
        }

        public static void DisabledCustomerFieldByCustomerType(CustomerType customerType)
        {
            var enabledFields = SQLDataAccess.ExecuteReadColumn<int>("SELECT Id FROM Customers.CustomerField where Enabled = 1 and CustomerType = @customerType", CommandType.Text, "Id",
                new SqlParameter("@customerType", (int)customerType));
            if (enabledFields?.Count > 0)
                SettingsCustomers.CustomerFieldsEnabledBeforeDisabled = String.Join(",", enabledFields);
            else
                return;
            SQLDataAccess.ExecuteNonQuery(@"Update Customers.CustomerField set Enabled = 0 where CustomerType = " + (int)customerType, CommandType.Text);
        }

        public static void EnabledCustomerFieldByCustomerType(CustomerType customerType)
        {
            var enabledFields = SettingsCustomers.CustomerFieldsEnabledBeforeDisabled;
            var condition = string.IsNullOrEmpty(enabledFields) ? string.Empty : $" and Id IN ({enabledFields})";
            SQLDataAccess.ExecuteNonQuery($"Update Customers.CustomerField set Enabled = 1 where CustomerType = {(int)customerType}{condition}", CommandType.Text);
        }
        #endregion

        #region CustomerFieldValue

        public static CustomerFieldValue GetCustomerFieldValue(int id)
        {
            return SQLDataAccess.Query<CustomerFieldValue>("SELECT * FROM Customers.CustomerFieldValue WHERE Id = @Id", new { id }).FirstOrDefault();
        }

        public static List<CustomerFieldValue> GetCustomerFieldValues(int customerFieldId)
        {
            return SQLDataAccess.Query<CustomerFieldValue>("SELECT * FROM Customers.CustomerFieldValue WHERE CustomerFieldId = @customerFieldId ORDER BY SortOrder, Value", new { customerFieldId }).ToList();
        }

        public static int AddCustomerFieldValue(CustomerFieldValue value)
        {
            return SQLDataAccess.ExecuteScalar<int>(
                "INSERT INTO Customers.CustomerFieldValue (Value, CustomerFieldId, SortOrder) VALUES (@Value, @CustomerFieldId, @SortOrder); SELECT SCOPE_IDENTITY();",
                CommandType.Text,
                new SqlParameter("@Value", value.Value),
                new SqlParameter("@CustomerFieldId", value.CustomerFieldId),
                new SqlParameter("@SortOrder", value.SortOrder)
                );
        }

        public static void UpdateCustomerFieldValue(CustomerFieldValue value)
        {
            SQLDataAccess.ExecuteNonQuery(
                "UPDATE Customers.CustomerFieldValue SET Value = @Value, CustomerFieldId = @CustomerFieldId, SortOrder = @SortOrder WHERE Id = @Id",
                CommandType.Text,
                new SqlParameter("@Id", value.Id),
                new SqlParameter("@Value", value.Value),
                new SqlParameter("@CustomerFieldId", value.CustomerFieldId),
                new SqlParameter("@SortOrder", value.SortOrder)
                );
        }

        public static void DeleteCustomerFieldValue(int id)
        {
            SQLDataAccess.ExecuteNonQuery(
                "DELETE FROM Customers.CustomerFieldValue WHERE Id = @Id",
                CommandType.Text,
                new SqlParameter("@Id", id));
        }

        public static void DeleteCustomerFieldValues(int customerFieldId)
        {
            SQLDataAccess.ExecuteNonQuery(
                "DELETE FROM Customers.CustomerFieldValue WHERE CustomerFieldId = @CustomerFieldId",
                CommandType.Text,
                new SqlParameter("@CustomerFieldId", customerFieldId));
        }

        #endregion

        #region CustomerFieldValuesMap

        public static bool IsCustomerFieldDefined(Guid customerId, int fieldId)
        {
            return SQLDataAccess.ExecuteScalar<int>(
                "Select Count(*) FROM Customers.CustomerFieldValuesMap WHERE CustomerId = @CustomerId AND CustomerFieldId = @CustomerFieldId AND Value IS NOT NULL AND Value <> ''",
                CommandType.Text,
                new SqlParameter("@CustomerId", customerId),
                new SqlParameter("@CustomerFieldId", fieldId)) > 0;
        }

        public static void DeleteFieldMap(Guid customerId, int fieldId)
        {
            SQLDataAccess.ExecuteNonQuery(
                "DELETE FROM Customers.CustomerFieldValuesMap WHERE CustomerId = @CustomerId AND CustomerFieldId = @CustomerFieldId",
                CommandType.Text, 
                new SqlParameter("@CustomerId", customerId),
                new SqlParameter("@CustomerFieldId", fieldId)
                );
        }

        public static void UpdateFieldsMapValues(int fieldId, string value)
        {
            SQLDataAccess.ExecuteNonQuery(
                "UPDATE Customers.CustomerFieldValuesMap SET Value = @Value WHERE CustomerFieldId = @CustomerFieldId",
                CommandType.Text,
                new SqlParameter("@CustomerFieldId", fieldId),
                new SqlParameter("@Value", value)
                );
        }

        public static void AddUpdateMap(Guid customerId, int fieldId, string value, bool onlyWithValue = false,
                                        bool trackChanges = true, ChangedBy changedBy = null)
        {
            var prevFieldValueMap =
                SQLDataAccess.Query<CustomerFieldValueMapShort>(
                    "Select top(1) cf.Name, map.Value " +
                    "From Customers.CustomerField as cf " +
                    "Inner join Customers.CustomerFieldValuesMap as map ON map.CustomerId = @customerId and map.CustomerFieldId = cf.Id " +
                    "Where cf.Id = @fieldId ",
                    new {customerId, fieldId}).FirstOrDefault();

            if (prevFieldValueMap == null)
            {
                SQLDataAccess.ExecuteNonQuery(
                    "INSERT INTO Customers.CustomerFieldValuesMap (CustomerId, CustomerFieldId, Value) VALUES (@CustomerId, @CustomerFieldId, @Value) ",
                    CommandType.Text,
                    new SqlParameter("@CustomerId", customerId),
                    new SqlParameter("@CustomerFieldId", fieldId),
                    new SqlParameter("@Value", value.DefaultOrEmpty())
                );
            }
            else if (!onlyWithValue || value.IsNotEmpty())
            {
                SQLDataAccess.ExecuteNonQuery(
                    "UPDATE Customers.CustomerFieldValuesMap SET Value = @Value WHERE CustomerId = @CustomerId AND CustomerFieldId = @CustomerFieldId ",
                    CommandType.Text,
                    new SqlParameter("@CustomerId", customerId),
                    new SqlParameter("@CustomerFieldId", fieldId),
                    new SqlParameter("@Value", value.DefaultOrEmpty())
                );
                
                if (trackChanges)
                    CustomerHistoryService.TrackCustomerFieldChanges(customerId, prevFieldValueMap.Name, prevFieldValueMap.Value, value, changedBy);
            }
        }

        #endregion

        public static List<object> GetCustomerFieldTypesSelectOptions()
        {
            return Enum.GetValues(typeof(CustomerFieldType)).Cast<CustomerFieldType>().Select(x => (object)new
            {
                label = x.Localize(),
                value = (int)x,
            }).ToList();
        }

        public static List<object> GetCustomerFieldCustomerTypesSelectOptions()
        {
            return Enum.GetValues(typeof(CustomerType)).Cast<CustomerType>().Select(x => (object)new
            {
                label = x.Localize(),
                value = (int)x,
            }).ToList();
        }

        public static List<object> GetCustomerFieldFieldAssignmentsSelectOptions()
        {
            return Enum.GetValues(typeof(CustomerFieldAssignment)).Cast<CustomerFieldAssignment>().Select(x => (object)new
            {
                label = x.Localize(),
                value = (int)x,
            }).ToList();
        }
    }
}
