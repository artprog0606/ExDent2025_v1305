using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Configuration;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Customers;
using AdvantShop.ExportImport;
using AdvantShop.FilePath;
using AdvantShop.Orders;
using AdvantShop.Saas;
using AdvantShop.Web.Admin.Models.Customers;
using CsvHelper;

namespace AdvantShop.Web.Admin.Handlers.Customers.Export
{
    public class ExportCustomersFromGrid
    {
        private readonly List<AdminCustomerExportToCsvModel> _collection;
        private readonly string _fileName;
        private readonly bool _includeAdditionalFields;
        private readonly bool _includeBonusCard;

        public ExportCustomersFromGrid(List<AdminCustomerExportToCsvModel> collection, string fileName)
        {
            _collection = collection;
            _fileName = fileName;
            _includeAdditionalFields = SaasDataService.IsEnabledFeature(ESaasProperty.CustomerAdditionFields);
            _includeBonusCard = SettingProvider.GetSqlSettingValue(EProviderSetting.ActiveBonusSystem).TryParseBool() && SaasDataService.IsEnabledFeature(ESaasProperty.BonusSystem);
        }

        public string Execute()
        {
            if (_collection == null)
                return string.Empty;

            var fileDirectory = FoldersHelper.GetPathAbsolut(FolderType.PriceTemp);

            using (var writer = new CsvWriter(
                new StreamWriter(fileDirectory + _fileName, false, Encoding.UTF8), CsvConstants.DefaultCsvConfiguration))
            {
                var additionalFields = new List<CustomerField>();
                if (_includeAdditionalFields)
                    additionalFields = CustomerFieldService.GetCustomerFields(true);
                

                WriteHeader(writer, additionalFields);

                foreach (var item in _collection)
                {
                    WriteItem(writer, item, additionalFields);
                }
            }
            return fileDirectory + _fileName;
        }

        private void WriteHeader(CsvWriter writer, List<CustomerField> additionalFields)
        {
            writer.WriteField("CustomerId");

            writer.WriteField("FirstName");
            writer.WriteField("LastName");
            writer.WriteField("Patronymic");
            writer.WriteField("Phone");
            writer.WriteField("Email");
            writer.WriteField("Customergroup");
            writer.WriteField("Country");
            writer.WriteField("Region");
            writer.WriteField("City");
            writer.WriteField("Zip");
            writer.WriteField("Address");
            writer.WriteField("Organization");
            writer.WriteField("RegistrationDateTime");
            writer.WriteField("RegisteredFrom");
            writer.WriteField("RegisteredFromIp");
            writer.WriteField("BirthDay");

            writer.WriteField("ManagerId");
            writer.WriteField("ManagerName");

            //writer.WriteField("Rating");

            writer.WriteField("LastOrderId");
            writer.WriteField("LastOrderNumber");
            writer.WriteField("LastOrderDate");

            writer.WriteField("OrdersSum");
            writer.WriteField("OrdersCount");
            if (_includeBonusCard)
                writer.WriteField("BonusCard");

            foreach (var field in additionalFields)
            {
                writer.WriteField(field.Name);
            }
            writer.WriteField("CustomerType");

            if (SettingsDesign.ShowUserAgreementForPromotionalNewsletter)
            {
                writer.WriteField("IsAgreeForPromotionalNewsletter");
                writer.WriteField("AgreeForPromotionalNewsletterDateTime");
                writer.WriteField("AgreeForPromotionalNewsletterFrom");
                writer.WriteField("AgreeForPromotionalNewsletterFromIp");
            }
            
            writer.WriteField("AdminComment");

            writer.NextRecord();
        }

        private void WriteItem(CsvWriter writer, AdminCustomerExportToCsvModel customer, List<CustomerField> additionalFields)
        {
            writer.WriteField(customer.CustomerId);

            writer.WriteField(customer.FirstName);
            writer.WriteField(customer.LastName);
            writer.WriteField(customer.Patronymic);
            writer.WriteField(customer.Phone);
            writer.WriteField(customer.Email);

            var customerGroupId = CustomerService.GetCustomerGroupId(customer.CustomerId);
            var customerGroup = CustomerGroupService.GetCustomerGroup(customerGroupId);
            writer.WriteField(customerGroup.GroupName);

            writer.WriteField(customer.Country);
            writer.WriteField(customer.Region);
            writer.WriteField(customer.City);
            writer.WriteField(customer.Zip);

            var orderCustomer = new OrderCustomer()
            {
                Street = customer.Street,
                House = customer.House,
                Structure = customer.Structure,
                Apartment = customer.Apartment,
                Entrance = customer.Entrance,
                Floor = customer.Floor
            };
            writer.WriteField(orderCustomer.GetCustomerAddress());
            
            writer.WriteField(customer.Organization);
            writer.WriteField(customer.RegistrationDateTime);
            writer.WriteField(customer.RegisteredFrom);
            writer.WriteField(customer.RegisteredFromIp);

            if (customer.BirthDay.HasValue)
                writer.WriteField(customer.BirthDay.Value);
            else
                writer.WriteField(string.Empty);

            writer.WriteField(customer.ManagerId);
            writer.WriteField(customer.ManagerName);

            //writer.WriteField(customer.Rating);

            writer.WriteField(customer.LastOrderId);
            writer.WriteField(customer.LastOrderNumber);
            writer.WriteField(customer.LastOrderDate);

            writer.WriteField(customer.OrdersSum);
            writer.WriteField(customer.OrdersCount);

            if (_includeBonusCard)
                writer.WriteField(customer.CardNumber.HasValue ? customer.CardNumber.ToString() : string.Empty);

            if (_includeAdditionalFields)
            {
                var values = CustomerFieldService.GetCustomerFieldsWithValue(customer.CustomerId);
                foreach (var field in additionalFields)
                {
                    var customerFieldValue = values.FirstOrDefault(item => item.Id == field.Id);
                    writer.WriteField(customerFieldValue != null ? customerFieldValue.Value : string.Empty);
                }
            }
            writer.WriteField((int)customer.CustomerType);

            if (SettingsDesign.ShowUserAgreementForPromotionalNewsletter)
            {
                writer.WriteField(customer.IsAgreeForPromotionalNewsletter);
                writer.WriteField(customer.AgreeForPromotionalNewsletterDateTime?.ToString("g"));
                writer.WriteField(customer.AgreeForPromotionalNewsletterFrom);
                writer.WriteField(customer.AgreeForPromotionalNewsletterFromIp);
            }
            
            writer.WriteField(customer.AdminComment);

            writer.NextRecord();
        }
    }
}
