using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Customers;
using AdvantShop.ExportImport;
using AdvantShop.Web.Admin.ViewModels.Catalog.Import;
using CsvHelper;
using CsvHelper.Configuration;

namespace AdvantShop.Web.Admin.Handlers.Catalog.Import
{
    public class GetExampleCustomersFileHandler
    {
        private readonly string _columnSeparator;
        private readonly string _encoding;
        private readonly string _outputFilePath;

        private List<Dictionary<ECustomerFields, string>> _customerExamples;

        public GetExampleCustomersFileHandler(ImportCustomersModel model, string outputFilePath)
        {
            _columnSeparator = model.ColumnSeparator.IsNotEmpty() && model.ColumnSeparator.ToLower() == SeparatorsEnum.Custom.ToString().ToLower()
                ? model.CustomColumnSeparator
                : model.ColumnSeparator;
            _encoding = model.Encoding;
            _outputFilePath = outputFilePath;
        }

        private CsvWriter InitWriter()
        {
            var csvConfiguration = CsvConstants.DefaultCsvConfiguration;
            csvConfiguration.Delimiter = _columnSeparator;
            var streamWriter = new StreamWriter(_outputFilePath, false, System.Text.Encoding.GetEncoding(_encoding));
            var writer = new CsvWriter(streamWriter, csvConfiguration);
            return writer;
        }

        public object Execute()
        {
            if (_columnSeparator.IsNullOrEmpty())
                throw new BlException(LocalizationService.GetResource("Admin.Import.Errors.NotColumnSeparator"));

            if (File.Exists(_outputFilePath))
                File.Delete(_outputFilePath);

            GetCustomerExample();
            using (var csvWriter = InitWriter())
            {
                var customerFields = CustomerFieldService.GetCustomerFields(true);
                foreach (ECustomerFields item in _customerExamples[0].Keys)
                {
                    if (item == ECustomerFields.None)
                        continue;
                    csvWriter.WriteField(item.StrName().ToLower());
                }
                foreach (var additionalField in customerFields)
                {
                    csvWriter.WriteField(additionalField.Name);
                }
                csvWriter.NextRecord();
                
                foreach (var customerExample in _customerExamples)
                {
                    foreach (var item in customerExample)
                    {
                        if (item.Key == ECustomerFields.None)
                            continue;
                        csvWriter.WriteField(item.Value);
                    }
                    foreach (var additionalField in customerFields)
                    {
                        if ((!customerExample.TryGetValue(ECustomerFields.CustomerType, out string customerType) 
                            || customerType == "1") 
                            && additionalField.CustomerType != CustomerType.PhysicalEntity)
                        {
                            switch (additionalField.FieldAssignment)
                            {
                                case CustomerFieldAssignment.CompanyName:
                                    csvWriter.WriteField("ООО \"Организация\"");
                                    continue;
                                case CustomerFieldAssignment.LegalAddress:
                                    csvWriter.WriteField("г. Москва");
                                    continue;
                                case CustomerFieldAssignment.INN:
                                    csvWriter.WriteField("7712345678");
                                    continue;
                                case CustomerFieldAssignment.KPP:
                                    csvWriter.WriteField("771234567");
                                    continue;
                                case CustomerFieldAssignment.OGRN:
                                    csvWriter.WriteField("1234560056789");
                                    continue;
                                case CustomerFieldAssignment.BIK:
                                    csvWriter.WriteField("034567895");
                                    continue;
                                case CustomerFieldAssignment.BankName:
                                    csvWriter.WriteField("ООО \"Банк\"");
                                    continue;
                                case CustomerFieldAssignment.CorrespondentAccount:
                                    csvWriter.WriteField("30101010101010101234");
                                    continue;
                                case CustomerFieldAssignment.PaymentAccount:
                                    csvWriter.WriteField("40702810546781234567");
                                    continue;
                            }
                        }
                        else if (additionalField.CustomerType == CustomerType.PhysicalEntity && customerType == "1"
                                || customerType == "0" && additionalField.CustomerType == CustomerType.LegalEntity)
                        {
                            csvWriter.WriteField(string.Empty);
                            continue;
                        }
                        switch (additionalField.FieldType)
                        {
                            case CustomerFieldType.Tel:
                                csvWriter.WriteField("+79998887766");
                                continue;
                            case CustomerFieldType.Email:
                                csvWriter.WriteField("additional@email.com");
                                continue;
                            case CustomerFieldType.Number:
                                csvWriter.WriteField("123456789");
                                continue;
                            case CustomerFieldType.Checkbox:
                                csvWriter.WriteField("True");
                                continue;
                            case CustomerFieldType.Date:
                                csvWriter.WriteField(DateTime.Now.ToString("d"));
                                continue;
                            default:
                                csvWriter.WriteField(additionalField.Name);
                                break;
                        }
                    }
                    csvWriter.NextRecord();
                }
            }

            return new { Result = true, };
        }

        private void GetCustomerExample()
        {
            _customerExamples = new List<Dictionary<ECustomerFields, string>>
            {
                new Dictionary<ECustomerFields, string>
                {
                    { ECustomerFields.CustomerId, Guid.NewGuid().ToString() },
                    { ECustomerFields.FirstName, "Иван" },
                    { ECustomerFields.LastName, "Иванов" },
                    { ECustomerFields.Patronymic, "Иванович" },
                    { ECustomerFields.Phone, "+79999999999" },
                    { ECustomerFields.Email, "example@example.com" },
                    { ECustomerFields.BirthDay, "1980-01-31" },
                    { ECustomerFields.CustomerGroup, "Постоянный покупатель" },
                    { ECustomerFields.Enabled, "+" },
                    { ECustomerFields.AdminComment, "Комментарий администратора" },

                    { ECustomerFields.City, "Москва" },
                    { ECustomerFields.Region, "Московская область" },
                    { ECustomerFields.Country, "Россия" },
                    { ECustomerFields.Zip, "012345" },
                    { ECustomerFields.Street, "Красная площадь" },
                    { ECustomerFields.House, "1" },
                    { ECustomerFields.Apartment, "1" },
                    { ECustomerFields.ManagerName, "Администратор Магазина" },
                    { ECustomerFields.ManagerId, "1" },
                }
            };

            if (SettingsCustomers.IsRegistrationAsPhysicalEntity && SettingsCustomers.IsRegistrationAsLegalEntity)
            {
                _customerExamples[0].Add(ECustomerFields.CustomerType, "1");
                _customerExamples.Add(new Dictionary<ECustomerFields, string>
                {
                    { ECustomerFields.CustomerId, Guid.NewGuid().ToString() },
                    { ECustomerFields.FirstName, "Петр" },
                    { ECustomerFields.LastName, "Петров" },
                    { ECustomerFields.Patronymic, "Петрович" },
                    { ECustomerFields.Phone, "+79876543210" },
                    { ECustomerFields.Email, "petrov@example.com" },
                    { ECustomerFields.BirthDay, "1980-01-01" },
                    { ECustomerFields.CustomerGroup, "Постоянный покупатель" },
                    { ECustomerFields.Enabled, "+" },
                    { ECustomerFields.AdminComment, "Комментарий администратора" },

                    { ECustomerFields.City, "Москва" },
                    { ECustomerFields.Region, "Московская область" },
                    { ECustomerFields.Country, "Россия" },
                    { ECustomerFields.Zip, "012345" },
                    { ECustomerFields.Street, "Красная площадь" },
                    { ECustomerFields.House, "1" },
                    { ECustomerFields.Apartment, "1" },
                    { ECustomerFields.ManagerName, "Администратор Магазина" },
                    { ECustomerFields.ManagerId, "1" },
                    { ECustomerFields.CustomerType, "0"}
                });
            }
            else if (SettingsCustomers.IsRegistrationAsLegalEntity)
            {
                _customerExamples[0].Add(ECustomerFields.CustomerType, "1");
            }
            else
            {
                _customerExamples[0].Add(ECustomerFields.CustomerType, "0");
            }

            if (SettingsDesign.ShowUserAgreementForPromotionalNewsletter)
                _customerExamples.ForEach(item => item.Add(ECustomerFields.IsAgreeForPromotionalNewsletter, "+"));
        }
    }
}
