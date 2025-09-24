using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using AdvantShop.Configuration;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.ExportImport;
using AdvantShop.FilePath;
using AdvantShop.Saas;
using AdvantShop.Web.Admin.Models.Crm.Leads;
using AdvantShop.Web.Admin.Models.Customers.CustomerSegments;
using AdvantShop.Web.Infrastructure.Admin;
using CsvHelper;
using CsvHelper.Configuration;

namespace AdvantShop.Web.Admin.Handlers.Customers.CustomerSegments
{
    public class ExportCustomersToCsv
    {
        private readonly List<CustomerBySegmentViewModel> _collection;
        private readonly string _fileName;
        private readonly bool _showAdditionalFields;
        private readonly bool _showCustomerType;

        public ExportCustomersToCsv(string fileName)
        {
            _fileName = fileName;
            _showAdditionalFields = !SaasDataService.IsSaasEnabled ||
                                    (SaasDataService.IsSaasEnabled &&
                                     SaasDataService.CurrentSaasData.CustomerAdditionFields);
            _showCustomerType = SettingsCustomers.IsRegistrationAsLegalEntity && SettingsCustomers.IsRegistrationAsPhysicalEntity;
        }

        public ExportCustomersToCsv(List<CustomerBySegmentViewModel> collection, string fileName) : this(fileName)
        {
            _collection = collection;
            _showCustomerType = SettingsCustomers.IsRegistrationAsLegalEntity && SettingsCustomers.IsRegistrationAsPhysicalEntity;
        }

        public ExportCustomersToCsv(FilterResult<CustomerBySegmentViewModel> collection, string fileName): this(fileName)
        {
            _collection = collection.DataItems;
            _showCustomerType = SettingsCustomers.IsRegistrationAsLegalEntity && SettingsCustomers.IsRegistrationAsPhysicalEntity;
        }

        //public ExportCustomersToCsv(LeadsFilterResult leads, string fileName) : this(fileName)
        //{
        //    if (leads != null && leads.DataItems != null)
        //    {
        //        _collection = new List<CustomerBySegmentViewModel>();

        //        foreach (var lead in leads.DataItems)
        //        {
        //            if (lead.CustomerId == null || _collection.Any(x => x.CustomerId == lead.CustomerId.Value))
        //                continue;

        //            var model = new CustomerBySegmentViewModel()
        //            {
        //                CustomerId = lead.CustomerId.Value,
        //                Email = lead.CustomerEmail,
        //                FirstName = lead.CustomerFirstName,
        //                LastName = lead.CustomerLastName,
        //                Patronymic = lead.CustomerPatronymic,
        //                Organization = lead.Organization,
        //                Phone = lead.CustomerPhone,
        //                ManagerName = lead.ManagerName,

        //                Location = lead.Location,
        //                BirthDay = lead.CustomerBirthDay,
        //                RegistrationDateTime = lead.CustomerRegistrationDateTime ?? DateTime.MinValue,
        //                LastOrderId = lead.LastOrderId,
        //                LastOrderNumber = lead.LastOrderNumber,
        //                OrdersCount = lead.OrdersCount,
        //                OrdersSum = lead.OrdersSum
        //            };
        //            _collection.Add(model);
        //        }
        //    }
        //}

        public string Execute()
        {
            if (_collection == null)
                return "";

            var fileDirectory = FoldersHelper.GetPathAbsolut(FolderType.PriceTemp);

            using (var writer = new CsvWriter(new StreamWriter(fileDirectory + _fileName, false, Encoding.UTF8), CsvConstants.DefaultCsvConfiguration))
            {
                var additionalFields = new List<CustomerField>();
                if (_showAdditionalFields)
                    additionalFields = CustomerFieldService.GetCustomerFields(true);

                WriteHeader(writer, additionalFields);

                foreach (var item in _collection)
                    WriteItem(writer, item, additionalFields);
            }

            Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Customers_ExportCustomersBySegment);

            return fileDirectory + _fileName;
        }

        public string GetFileUrl()
        {
            var path = Execute();

            return !string.IsNullOrEmpty(path)
                ? UrlService.GetUrl(FoldersHelper.GetPathRelative(FolderType.PriceTemp, _fileName, false))
                : path;
        }

        private void WriteHeader(CsvWriter writer, List<CustomerField> additionalFields)
        {
            writer.WriteField("CustomerId");
            writer.WriteField("Name");
            writer.WriteField("Organization");
            writer.WriteField("Email");
            writer.WriteField("Phone");
            writer.WriteField("ManagerName");

            writer.WriteField("LastOrderId");
            writer.WriteField("LastOrderNumber");

            writer.WriteField("OrdersSum");
            writer.WriteField("OrdersCount");

            writer.WriteField("Location");

            writer.WriteField("RegistrationDateTime");

            writer.WriteField("BirthDay");

            if(_showCustomerType)
                writer.WriteField("CustomerType");

            foreach (var field in additionalFields)
            {
                writer.WriteField(field.Name);
            }

            writer.NextRecord();
        }

        private void WriteItem(CsvWriter writer, CustomerBySegmentViewModel customer, List<CustomerField> additionalFields)
        {
            writer.WriteField(customer.CustomerId);
            writer.WriteField(customer.Name);
            writer.WriteField(customer.Organization);
            writer.WriteField(customer.Email);
            writer.WriteField(customer.Phone);
            writer.WriteField(customer.ManagerName);
            
            writer.WriteField(customer.LastOrderId);
            writer.WriteField(customer.LastOrderNumber);

            writer.WriteField(customer.OrdersSum);
            writer.WriteField(customer.OrdersCount);

            writer.WriteField(customer.Location);

            writer.WriteField(customer.RegistrationDateTime);

            if (customer.BirthDay.HasValue)
                writer.WriteField(customer.BirthDay.Value);
            else
                writer.WriteField(string.Empty);

            if(_showCustomerType)
                writer.WriteField(customer.CustomerType);

            if (_showAdditionalFields)
            {
                var values = CustomerFieldService.GetCustomerFieldsWithValue(customer.CustomerId);
                foreach (var field in additionalFields)
                {
                    var customerFieldValue = values.FirstOrDefault(item => item.Id == field.Id);
                    writer.WriteField(customerFieldValue != null ? customerFieldValue.Value : string.Empty);
                }
            }

            writer.NextRecord();
        }
    }
}
