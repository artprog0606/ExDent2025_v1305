using AdvantShop.Configuration;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.FilePath;
using CsvHelper;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace AdvantShop.Web.Admin.Handlers.Settings.System
{
    public class CheckPhoneDuplicateHandler
    {
        public (bool haveDuplicates, string urlPath) Execute()
        {
            var duplicates = GetDuplicates();
            if (duplicates.Count == 0)
                return (false, null);

            var fileDirectory = FoldersHelper.GetPathAbsolut(FolderType.PriceTemp);
            var fileName = "export_phoneDuplicateCustomers.csv";
            var fullPath = fileDirectory + fileName;

            if (File.Exists(fullPath))
                File.Delete(fullPath);

            using (var stream = new CsvWriter(
                new StreamWriter(fullPath, false, Encoding.UTF8),
                new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ";" }))
            {
                WriteHeader(stream);

                foreach (var item in duplicates)
                {
                    WriteItem(stream, item);
                } 
            }

            var urlPath = UrlService.GetUrl(FoldersHelper.PhotoFoldersPath[FolderType.PriceTemp]) + fileName;
            return (true, urlPath);
        }

        private void WriteHeader(CsvWriter writer)
        {
            writer.WriteField("CustomerId");

            writer.WriteField("FirstName");
            writer.WriteField("LastName");
            writer.WriteField("Patronymic");
            writer.WriteField("Phone");
            writer.WriteField("Email");
            writer.WriteField("Customergroup");
            writer.WriteField("City");
            writer.WriteField("Organization");
            writer.WriteField("RegistrationDateTime");
            writer.WriteField("BirthDay");

            writer.WriteField("ManagerId");
            writer.WriteField("CustomerType");

            writer.WriteField("AdminComment");

            writer.NextRecord();
        }

        private void WriteItem(CsvWriter writer, Customer customer)
        {
            writer.WriteField(customer.Id);

            writer.WriteField(customer.FirstName);
            writer.WriteField(customer.LastName);
            writer.WriteField(customer.Patronymic);
            writer.WriteField(customer.Phone);
            writer.WriteField(customer.EMail);

            var customerGroupId = CustomerService.GetCustomerGroupId(customer.Id);
            var customerGroup = CustomerGroupService.GetCustomerGroup(customerGroupId);
            writer.WriteField(customerGroup.GroupName);
            writer.WriteField(customer.City);
            writer.WriteField(customer.Organization);
            writer.WriteField(customer.RegistrationDateTime);

            if (customer.BirthDay.HasValue)
                writer.WriteField(customer.BirthDay.Value);
            else
                writer.WriteField(string.Empty);

            writer.WriteField(customer.ManagerId);
            writer.WriteField((int)customer.CustomerType);

            if (SettingsDesign.ShowUserAgreementForPromotionalNewsletter)
                writer.WriteField(customer.IsAgreeForPromotionalNewsletter);

            writer.WriteField(customer.AdminComment);

            writer.NextRecord();
        }

        public List<Customer> GetDuplicates()
        {
            return CustomerService.GetCustomers()
                .GroupBy(x => Helpers.StringHelper.ConvertToStandardPhone(x.Phone))
                .Where(x => x.Count() > 1)
                .SelectMany(x => x)
                .ToList();
        }
    }
}
