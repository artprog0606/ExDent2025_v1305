using System;
using AdvantShop.Core.Services.ChangeHistories;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Customers;

namespace AdvantShop.Core.Services.Customers
{
    public class CustomerHistoryService
    {
        public static void NewCustomer(Customer customer, ChangedBy changedBy)
        {
            ChangeHistoryService.Add(new ChangeHistory(changedBy)
            {
                ObjId = customer.InnerId,
                ObjType = ChangeHistoryObjType.Customer,
                ParameterName = LocalizationService.GetResourceFormat("Core.Customers.CustomerHistory.CustomerCreated", customer.Id)
            });
        }
        
        public static void DeleteCustomer(Customer customer, ChangedBy changedBy)
        {
            ChangeHistoryService.Add(new ChangeHistory(changedBy)
            {
                ObjId = customer.InnerId,
                ObjType = ChangeHistoryObjType.Customer,
                ParameterName = LocalizationService.GetResourceFormat("Core.Customers.CustomerHistory.CustomerDeleted", customer.Id)
            });
        }

        public static void TrackChanges(Customer customer, ChangedBy changedBy)
        {
            var oldCustomer = CustomerService.GetCustomerFromDb(customer.Id);
            if (oldCustomer == null)
                return;

            var history = ChangeHistoryService.GetChanges(customer.InnerId, ChangeHistoryObjType.Customer, oldCustomer, customer, changedBy);

            ChangeHistoryService.Add(history);
        }
        
        public static void TrackEmailChanges(Guid customerId, string newEmail, ChangedBy changedBy)
        {
            var customer = CustomerService.GetCustomer(customerId);
            if (customer == null)
                return;

            ChangeHistoryService.Add(new ChangeHistory(changedBy)
            {
                ObjId = customer.InnerId,
                ObjType = ChangeHistoryObjType.Customer,
                ParameterName = LocalizationService.GetResource("Core.Customers.Customer.Email"),
                OldValue = customer.EMail,
                NewValue = newEmail
            });
        }

        public static void TrackPasswordChanges(Guid customerId, ChangedBy changedBy)
        {
            var customer = CustomerService.GetCustomer(customerId);
            if (customer == null)
                return;

            ChangeHistoryService.Add(new ChangeHistory(changedBy)
            {
                ObjId = customer.InnerId,
                ObjType = ChangeHistoryObjType.Customer,
                ParameterName = LocalizationService.GetResource("Core.Customers.Customer.PasswordChanged")
            });
        }

        public static void TrackManagerChanges(Guid customerId, int? managerId, ChangedBy changedBy)
        {
            var customer = CustomerService.GetCustomerFromDb(customerId);
            if (customer == null)
                return;

            ChangeHistoryService.Add(new ChangeHistory(changedBy)
            {
                ObjId = customer.InnerId,
                ObjType = ChangeHistoryObjType.Customer,
                ParameterName = LocalizationService.GetResource("Core.Customers.Customer.ManagerId"),
                OldValue = customer?.ManagerId.ToString(),
                NewValue = managerId?.ToString()
            });
        }
        
        public static void TrackCustomerFieldChanges(Guid customerId, string fieldName, string oldValue, string newValue, ChangedBy changedBy)
        {
            var customer = CustomerService.GetCustomer(customerId);
            if (customer == null)
                return;

            ChangeHistoryService.Add(new ChangeHistory(changedBy)
            {
                ObjId = customer.InnerId,
                ObjType = ChangeHistoryObjType.Customer,
                ParameterName = fieldName,
                OldValue = oldValue,
                NewValue = newValue
            });
        }
        
        public static void TrackContactChanges(CustomerContact contact, ChangedBy changedBy)
        {
            var oldContact = CustomerService.GetCustomerContact(contact.ContactId);
            if (oldContact == null)
                return;

            var customer = CustomerService.GetCustomer(contact.CustomerGuid);
            if (customer == null)
                return;

            var history = ChangeHistoryService.GetChanges(customer.InnerId, ChangeHistoryObjType.Customer, oldContact, contact, changedBy);

            ChangeHistoryService.Add(history);
        }
        
        public static void DeleteContact(Guid contactId, ChangedBy changedBy)
        {
            var contact = CustomerService.GetCustomerContact(contactId);
            if (contact == null)
                return;

            var customer = CustomerService.GetCustomer(contact.CustomerGuid);
            if (customer == null)
                return;
            
            ChangeHistoryService.Add(new ChangeHistory(changedBy)
            {
                ObjId = customer.InnerId,
                ObjType = ChangeHistoryObjType.Customer,
                ParameterName = LocalizationService.GetResourceFormat("Core.Customers.CustomerHistory.CustomerContactDeleted", customer.Id)
            });
        }
    }
}
