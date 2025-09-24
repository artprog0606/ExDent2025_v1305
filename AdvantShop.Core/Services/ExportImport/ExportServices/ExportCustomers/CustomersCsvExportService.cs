using System;
using System.IO;
using System.Linq;
using System.Text;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Customers;
using AdvantShop.Customers;
using AdvantShop.ExportImport;
using AdvantShop.Repository;
using AdvantShop.Statistic;
using CsvHelper;

namespace AdvantShop.Core.Services.ExportImport.ExportServices.ExportCustomers
{
    public class CustomersCsvExportService : CustomersExportBaseService
    {
        public CustomersCsvExportService(CustomersExportSettings settings) : base(settings)
        {
        }
        
        public void Export(string fullPath)
        {
            CommonStatistic.TotalRow = GetCustomerRowsCount();
            
            using (var writer = InitWriter(fullPath, _settings.Encoding, _settings.ColumnSeparator))
            {
                WriteHeader(writer);
                
                foreach (var customer in GetCustomers())
                {
                    if (!CommonStatistic.IsRun || CommonStatistic.IsBreaking) 
                        return;
                    
                    WriteRow(writer, customer);
                }
            }
        }

        private void WriteHeader(CsvWriter writer)
        {
            foreach (var field in _fields)
                writer.WriteField(field.StrName());
            
            foreach (var field in _additionalFields)
                writer.WriteField(field);
            
            writer.NextRecord();
        }

        private void WriteRow(CsvWriter writer, CustomerExportDto customer)
        {
            foreach (var field in _fields)
            {
                switch (field)
                {
                    case ECustomerFields.CustomerId:
                        writer.WriteField(customer.CustomerId);
                        break;
                    case ECustomerFields.FirstName:
                        writer.WriteField(customer.FirstName);
                        break;
                    case ECustomerFields.LastName:
                        writer.WriteField(customer.LastName);
                        break;
                    case ECustomerFields.Patronymic:
                        writer.WriteField(customer.Patronymic);
                        break;
                    case ECustomerFields.Phone:
                        writer.WriteField(customer.Phone);
                        break;
                    case ECustomerFields.Email:
                        writer.WriteField(customer.Email);
                        break;
                    case ECustomerFields.BirthDay:
                        writer.WriteField(customer.BirthDay?.ToString("d"));
                        break;
                    case ECustomerFields.Organization:
                        writer.WriteField(customer.Organization);
                        break;
                    case ECustomerFields.CustomerGroup:
                        writer.WriteField(customer.CustomerGroupName);
                        break;
                    case ECustomerFields.CustomerType:
                    {
                        var type = (CustomerType) customer.CustomerType;
                        writer.WriteField(type.Localize());
                        break;
                    }
                    case ECustomerFields.Enabled:
                        writer.WriteField(customer.Enabled ? "+" : "-");
                        break;
                    case ECustomerFields.RegistrationDateTime:
                        writer.WriteField(customer.RegistrationDateTime.ToString("g"));
                        break;
                    case ECustomerFields.RegisteredFrom:
                        writer.WriteField(customer.RegisteredFrom);
                        break;
                    case ECustomerFields.RegisteredFromIp:
                        writer.WriteField(customer.RegisteredFromIp);
                        break;
                    case ECustomerFields.AdminComment:
                        writer.WriteField(customer.AdminComment);
                        break;
                    case ECustomerFields.ManagerName:
                        writer.WriteField(customer.ManagerName);
                        break;
                    case ECustomerFields.ManagerId:
                        writer.WriteField(customer.ManagerId);
                        break;
                    case ECustomerFields.IsAgreeForPromotionalNewsletter:
                        writer.WriteField(customer.IsAgreeForPromotionalNewsletter ? "+" : "-");
                        break;
                    case ECustomerFields.AgreeForPromotionalNewsletterDateTime:
                        writer.WriteField(customer.AgreeForPromotionalNewsletterDateTime?.ToString("g"));
                        break;
                    case ECustomerFields.AgreeForPromotionalNewsletterFrom:
                        writer.WriteField(customer.AgreeForPromotionalNewsletterFrom);
                        break;
                    case ECustomerFields.AgreeForPromotionalNewsletterFromIp:
                        writer.WriteField(customer.AgreeForPromotionalNewsletterFromIp);
                        break;
                    case ECustomerFields.Zip:
                        writer.WriteField(customer.Zip);
                        break;
                    case ECustomerFields.Country:
                    {
                        var country = CountryService.GetCountry(customer.CountryId);
                        writer.WriteField(country?.Name);
                        break;
                    }
                    case ECustomerFields.Region:
                    {
                        var region = RegionService.GetRegion(customer.RegionId);
                        writer.WriteField(region?.Name);
                        break;
                    }
                    case ECustomerFields.City:
                        writer.WriteField(customer.City);
                        break;
                    case ECustomerFields.District:
                        writer.WriteField(customer.District);
                        break;
                    case ECustomerFields.Street:
                        writer.WriteField(customer.Street);
                        break;
                    case ECustomerFields.House:
                        writer.WriteField(customer.House);
                        break;
                    case ECustomerFields.Apartment:
                        writer.WriteField(customer.Apartment);
                        break;
                    case ECustomerFields.Structure:
                        writer.WriteField(customer.Structure);
                        break;
                    case ECustomerFields.Entrance:
                        writer.WriteField(customer.Entrance);
                        break;
                    case ECustomerFields.Floor:
                        writer.WriteField(customer.Floor);
                        break;

                    case ECustomerFields.LastOrder:
                        writer.WriteField(customer.LastOrderNumber);
                        break;
                    case ECustomerFields.PaidOrdersCount:
                        writer.WriteField(customer.PaidOrdersCount);
                        break;
                    case ECustomerFields.PaidOrdersSum:
                        writer.WriteField(customer.PaidOrdersSum);
                        break;
                    
                    case ECustomerFields.BonusCard:
                        writer.WriteField(customer.BonusCardNumber);
                        break;
                    
                    case ECustomerFields.Tags:
                        writer.WriteField(
                            TagService.Gets(customer.CustomerId, true)
                                .Select(x => x.Name)
                                .AggregateString("; "));
                        break;
                }
            }

            if (_additionalFields.Count > 0)
            {
                var customerAdditionalFields = CustomerFieldService.GetCustomerFieldsWithValue(customer.CustomerId);
                
                foreach (var field in _additionalFields)
                {
                    var customerAdditionalField = customerAdditionalFields.Find(x => x.Name == field);
                    
                    writer.WriteField(customerAdditionalField?.Value);
                }
            }

            writer.NextRecord();
            CommonStatistic.RowPosition++;
        }
        
        private CsvWriter InitWriter(string strFullPath, string csvEncoding, string csvSeparator)
        {
            var csvConfiguration = CsvConstants.DefaultCsvConfiguration;
            csvConfiguration.Delimiter = csvSeparator;
            var writer = new CsvWriter(new StreamWriter(strFullPath, false, Encoding.GetEncoding(csvEncoding)), csvConfiguration);

            return writer;
        }
    }
}