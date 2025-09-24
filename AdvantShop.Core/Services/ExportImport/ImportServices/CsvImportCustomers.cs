using System;
using System.IO;
using System.Text;
using System.Globalization;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.ChangeHistories;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Statistic;
using AdvantShop.Customers;
using AdvantShop.Repository;
using CsvHelper;
using CsvHelper.Configuration;
using MissingFieldException=CsvHelper.MissingFieldException;
using System.Text.RegularExpressions;
using AdvantShop.Core.Services.Customers;

namespace AdvantShop.ExportImport
{
    public class CsvImportCustomers
    {
        private readonly string _fullPath;
        private readonly bool _hasHeaders;

        private readonly string _separator;
        private readonly string _encodings;

        private readonly int _defaultCustomerGroupId;

        private Dictionary<string, int> _fieldMapping;
        private bool _useCommonStatistic;

        private readonly ChangedBy _changedBy;
        private readonly Regex _numberRegex;
        private readonly List<Manager> _managers;

        public CsvImportCustomers(string filePath, bool hasHeaders, string separator, string encodings, Dictionary<string, int> fieldMapping, int defaultCustomerGroupId,
            bool useCommonStatistic = true)
        {
            _fullPath = filePath;
            _hasHeaders = hasHeaders;
            _fieldMapping = fieldMapping;
            _encodings = encodings;
            _separator = separator;

            _defaultCustomerGroupId = defaultCustomerGroupId;
            _useCommonStatistic = useCommonStatistic;

            _changedBy = CustomerContext.CurrentCustomer != null
                ? new ChangedBy("CSV Import " + CustomerContext.CurrentCustomer.GetShortName()) {CustomerId = CustomerContext.CustomerId}
                : new ChangedBy("CSV Import");
            _numberRegex = new Regex("^-?\\d+[.,]?\\d*$");
            _managers = ManagerService.GetManagersList();
        }

        private CsvReader InitReader(bool? hasHeaderRecord = null)
        {
            var csvConfiguration = CsvConstants.DefaultCsvConfiguration;
            csvConfiguration.Delimiter = _separator ?? SeparatorsEnum.SemicolonSeparated.StrName();
            csvConfiguration.HasHeaderRecord = hasHeaderRecord ?? _hasHeaders;
            var reader = new CsvReader(new StreamReader(_fullPath, Encoding.GetEncoding(_encodings ?? EncodingsEnum.Utf8.StrName())), csvConfiguration);
            return reader;
        }

        public List<string[]> ReadFirstRecord()
        {
            var list = new List<string[]>();
            using (var csv = InitReader())
            {
                int count = 0;
                while (csv.Read())
                {
                    if (count == 2)
                        break;

                    if (csv.Parser.Record != null)
                        list.Add(csv.Parser.Record);
                    count++;
                }
            }
            return list;
        }

        // private - чтобы нельзя было запускать импорт при уже запущеном,
        // через данный метот это не контролируется
        private void Process()
        {
           try
           {
               _process();
           }
           catch (Exception ex)
           {
               Debug.Log.Error(ex);
               DoForCommonStatistic(() =>
               {
                   CommonStatistic.WriteLog(ex.Message);
                   CommonStatistic.TotalErrorRow++;
               });
           }
        }

        public Task<bool> ProcessThroughACommonStatistic(
            string currentProcess,
            string currentProcessName,
            Action onBeforeImportAction = null)
        {
            return CommonStatistic.StartNew(() =>
                {
                    if (onBeforeImportAction != null)
                        onBeforeImportAction();

                    _useCommonStatistic = true;
                    Process();
                },
                currentProcess,
                currentProcessName);
        }

        private void _process()
        {
            Log("Начало импорта");

            if (_fieldMapping == null)
                MapFields();

            if (_fieldMapping == null)
                throw new Exception("can mapping colums");

            DoForCommonStatistic(() => CommonStatistic.TotalRow = GetRowCount());
            
            ProcessRow();

            CacheManager.Clean();
            FileHelpers.DeleteFile(_fullPath);

            Log("Окончание импорта");
        }

        private void MapFields()
        {
            _fieldMapping = new Dictionary<string, int>();
            using (var csv = InitReader(false))
            {
                csv.Read();
                for (var i = 0; i < csv.Parser.Record.Length; i++)
                {
                    if (csv.Parser.Record[i] == ECustomerFields.None.StrName()) continue;
                    if (!_fieldMapping.ContainsKey(csv.Parser.Record[i]))
                        _fieldMapping.Add(csv.Parser.Record[i], i);
                }
            }
        }

        private long GetRowCount()
        {
            long count = 0;
            using (var csv = InitReader())
            {
                if (_hasHeaders)
                {
                    csv.Read();
                    csv.ReadHeader();
                }
                
                while (csv.Read())
                    count++;
            }
            return count;
        }

        private void ProcessRow()
        {
            if (!File.Exists(_fullPath)) return;
            using (var csv = InitReader())
            {
                if (_hasHeaders)
                {
                    csv.Read();
                    csv.ReadHeader();
                }
                
                while (csv.Read())
                {
                    if (_useCommonStatistic && (!CommonStatistic.IsRun || CommonStatistic.IsBreaking))
                    {
                        csv.Dispose();
                        FileHelpers.DeleteFile(_fullPath);
                        return;
                    }
                    try
                    {
                        var customerInStrings = PrepareRow(csv);
                        if (customerInStrings == null)
                        {
                            DoForCommonStatistic(() => CommonStatistic.RowPosition++);
                            continue;
                        }

                        UpdateInsertCustomer(customerInStrings, csv);

                    }
                    catch (Exception ex)
                    {
                        Debug.Log.Error(ex);
                        DoForCommonStatistic(() =>
                        {
                            CommonStatistic.WriteLog(string.Format("{0}: {1}", CommonStatistic.RowPosition, ex.Message));
                            CommonStatistic.TotalErrorRow++;
                        });
                    }
                }
            }
        }

        private Dictionary<ECustomerFields, object> PrepareRow(IReader csv)
        {
            var customerInStrings = new Dictionary<ECustomerFields, object>();

            foreach (ECustomerFields field in Enum.GetValues(typeof(ECustomerFields)))
            {
                try
                {
                    switch (field.Status())
                    {
                        case CsvFieldStatus.String:
                            GetString(field, csv, customerInStrings);
                            break;
                        case CsvFieldStatus.StringRequired:
                            GetStringRequired(field, csv, customerInStrings);
                            break;
                        case CsvFieldStatus.NotEmptyString:
                            GetStringNotNull(field, csv, customerInStrings);
                            break;
                        case CsvFieldStatus.Float:
                            if (!GetDecimal(field, csv, customerInStrings))
                                return null;
                            break;
                        case CsvFieldStatus.NullableFloat:
                            if (!GetNullableDecimal(field, csv, customerInStrings))
                                return null;
                            break;
                        case CsvFieldStatus.Int:
                            if (!GetInt(field, csv, customerInStrings))
                                return null;
                            break;
                        case CsvFieldStatus.DateTime:
                            if (!GetDateTime(field, csv, customerInStrings))
                                return null;
                            break;
                        case CsvFieldStatus.NullableDateTime:
                            if (!GetNullableDateTime(field, csv, customerInStrings))
                                return null;
                            break;
                    }
                }
                catch (MissingFieldException exception)
                {
                    DoForCommonStatistic(() => {
                        CommonStatistic.WriteLog($"Строка №{CommonStatistic.RowPosition}: Не валидный формат строки - пропущено поле {field.Localize()}");
                        CommonStatistic.TotalErrorRow++;
                    });
                    return null;
                }
            }
            return customerInStrings;
        }

        private void UpdateInsertCustomer(Dictionary<ECustomerFields, object> customerInStrings, IReader csv)
        {
            try
            {
                Customer customer = null;

                var email = customerInStrings.TryGetValue(ECustomerFields.Email, out var s)
                    ? Convert.ToString(s)
                    : string.Empty;

                var customerId = customerInStrings.TryGetValue(ECustomerFields.CustomerId, out var s1)
                    ? Convert.ToString(s1).TryParseGuid()
                    : Guid.Empty;

                var phone = customerInStrings.TryGetValue(ECustomerFields.Phone, out var s2)
                    ? Convert.ToString(s2)
                    : string.Empty;

                if (customerId != Guid.Empty)
                {
                    customer = CustomerService.GetCustomer(customerId);
                }

                if (string.IsNullOrEmpty(email) && string.IsNullOrEmpty(phone) && customer == null)
                {
                    DoForCommonStatistic(() => CommonStatistic.TotalErrorRow++);
                    Log(CommonStatistic.RowPosition + ": no email, phone and wrong customer id");

                    customerInStrings.Clear();
                    DoForCommonStatistic(() => CommonStatistic.RowPosition++);
                    return;
                }
                
                if(customer == null && !string.IsNullOrEmpty(email))
                {
                    customer = CustomerService.GetCustomerByEmail(email);
                }

                var standardPhone = StringHelper.ConvertToStandardPhone(phone);
                if (customer == null && !string.IsNullOrEmpty(phone))
                {
                    customer = CustomerService.GetCustomerByPhone(phone, standardPhone);
                }

                if (customer == null)
                {
                    customer = new Customer(CustomerGroupService.DefaultCustomerGroup)
                    {
                        EMail = email,
                        Enabled = true
                    };
                }

                if (customer.Id == Guid.Empty &&
                    customerInStrings.TryGetValue(ECustomerFields.RegistrationDateTime, out var fieldRegDate))
                {
                    if (fieldRegDate is DateTime regDate)
                    {
                        customer.RegistrationDateTime = regDate;
                    }
                }

                if (customerInStrings.TryGetValue(ECustomerFields.CustomerType, out var s3))
                {
                    var customerTypeStr = s3.AsString();
                    var customerTypeInt = customerTypeStr.TryParseInt();

                    if (customerTypeInt != 0)
                    {
                        customer.CustomerType = (CustomerType) customerTypeInt;
                    }
                    else if (customerTypeStr != null)
                    {
                        if (customerTypeStr.Equals(CustomerType.PhysicalEntity.Localize(), StringComparison.OrdinalIgnoreCase))
                            customer.CustomerType = CustomerType.PhysicalEntity;
                        else if (customerTypeStr.Equals(CustomerType.LegalEntity.Localize(), StringComparison.OrdinalIgnoreCase))
                            customer.CustomerType = CustomerType.LegalEntity;
                    }
                }

                var additionalFields = GetCustomerAdditionalFields(csv, customer.CustomerType);

                if (additionalFields == null)
                {
                    Log("Не удалось добавить покупателя: " + customer.GetFullName());
                    customerInStrings.Clear();
                    DoForCommonStatistic(() => CommonStatistic.RowPosition++);
                    return;
                }

                if (customerInStrings.TryGetValue(ECustomerFields.FirstName, out var s4))
                    customer.FirstName = Convert.ToString(s4);

                if (customerInStrings.TryGetValue(ECustomerFields.LastName, out var s5))
                    customer.LastName = Convert.ToString(s5);

                if (customerInStrings.TryGetValue(ECustomerFields.Patronymic, out var s6))
                    customer.Patronymic = Convert.ToString(s6);

                if (customerInStrings.TryGetValue(ECustomerFields.BirthDay, out var s7))
                    customer.BirthDay = (DateTime?)s7;

                if (customerInStrings.TryGetValue(ECustomerFields.Phone, out var s8))
                {
                    customer.Phone = standardPhone.HasValue ? standardPhone.ToString() : Convert.ToString(s8);
                    customer.StandardPhone = standardPhone;
                }

                if (customerInStrings.TryGetValue(ECustomerFields.Enabled, out var enabled))
                    customer.Enabled = enabled.AsString() == "+"
                        ? true
                        : (enabled.AsString() == "-" ? false : Convert.ToBoolean(enabled));

                if (customerInStrings.TryGetValue(ECustomerFields.AdminComment, out var s10))
                    customer.AdminComment = Convert.ToString(s10);

                if (customerInStrings.TryGetValue(ECustomerFields.Organization, out var s11))
                    customer.Organization = Convert.ToString(s11);

                if (customerInStrings.TryGetValue(ECustomerFields.CustomerGroup, out var s12))
                {
                    var customerGroupName = Convert.ToString(s12);
                    var customerGroup = CustomerGroupService.GetCustomerGroup(customerGroupName);
                    if (customerGroup == null)
                    {
                        if (!string.IsNullOrWhiteSpace(customerGroupName))
                        {
                            customerGroup = new CustomerGroup
                            {
                                GroupDiscount = 0,
                                MinimumOrderPrice = 0,
                                GroupName = customerGroupName
                            };
                            CustomerGroupService.AddCustomerGroup(customerGroup);
                        }
                        else
                        {
                            customerGroup = CustomerGroupService.GetCustomerGroup(_defaultCustomerGroupId) ??
                                            CustomerGroupService.GetDefaultCustomerGroup();
                        }
                    }
                    customer.CustomerGroupId = customerGroup.CustomerGroupId;
                }
                else
                {
                    customer.CustomerGroupId = _defaultCustomerGroupId;
                }

                if (customerInStrings.TryGetValue(ECustomerFields.ManagerId, out var s13))
                {
                    var manager = ManagerService.GetManager(Convert.ToString(s13).TryParseInt());
                    if (manager != null)
                        customer.ManagerId = manager.ManagerId;
                }

                if (!customer.ManagerId.HasValue && customerInStrings.TryGetValue(ECustomerFields.ManagerName, out var s14))
                {
                    var managerName = Convert.ToString(s14);
                    var manager = _managers.FirstOrDefault(x => managerName.Contains(x.FirstName) && managerName.Contains(x.LastName));
                    if (manager != null)
                        customer.ManagerId = manager.ManagerId;
                }

                if (customerInStrings.TryGetValue(ECustomerFields.IsAgreeForPromotionalNewsletter, out var isAgree))
                {
                    var isAgreeStr = isAgree.AsString();
                    
                    customer.IsAgreeForPromotionalNewsletter =
                        isAgreeStr == "+"
                            ? true
                            : (isAgreeStr == "-" ? false : isAgreeStr.TryParseBool());
                }
                
                if (customerInStrings.ContainsKey(ECustomerFields.Tags))
                {
                    var tagsString = customerInStrings[ECustomerFields.Tags].AsString();
                
                    if (!string.IsNullOrEmpty(tagsString))
                    {
                        var tags = 
                            tagsString.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries)
                                .Select(x => x.TrimStart().TrimEnd())
                                .ToList();

                        var tagsList = new List<Tag>();

                        foreach (var tag in tags)
                        {
                            var t = CacheManager.Get($"get_tag_{tag}", () => TagService.Get(tag));
                            
                            tagsList.Add(t ?? new Tag(){Name = tag, Enabled = true});
                        }

                        customer.Tags = tagsList;
                    }
                    else
                    {
                        customer.Tags = new List<Tag>();
                    }
                }

                if (customer.Id == Guid.Empty)
                {
                    customer.Id = CustomerService.InsertNewCustomer(customer, trackChanges:true, changedBy:_changedBy);
                    DoForCommonStatistic(() => CommonStatistic.TotalAddRow++);
                    Log("покупатель добавлен " + customer.GetFullName());
                    Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Customers_CustomerCreated_Csv);
                }
                else
                {
                    CustomerService.UpdateCustomer(customer, trackChanges:true, changedBy:_changedBy);
                    DoForCommonStatistic(() => CommonStatistic.TotalUpdateRow++);
                    Log("покупатель обновлен " + customer.GetFullName());
                    Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Customers_EditCustomer_Csv);
                }

                if (customer.Id != Guid.Empty)
                {
                    CustomerContactFields(customerInStrings, customer.Id);
                    
                    foreach (var field in additionalFields)
                        CustomerFieldService.AddUpdateMap(customer.Id, field.Key, field.Value);
                }
                else
                {
                    Log("Не удалось добавить покупателя: " + customer.GetFullName());
                    DoForCommonStatistic(() => CommonStatistic.TotalErrorRow++);
                }
            }
            catch (Exception e)
            {
                Debug.Log.Error(e);
                DoForCommonStatistic(() => CommonStatistic.TotalErrorRow++);
                Log(CommonStatistic.RowPosition + ": " + e.Message);
            }

            customerInStrings.Clear();
            DoForCommonStatistic(() => CommonStatistic.RowPosition++);
        }

        private void CustomerContactFields(IDictionary<ECustomerFields, object> fields, Guid customerId)
        {
            var contacts = CustomerService.GetCustomerContacts(customerId);
            var customerContact = contacts != null && contacts.Count > 0 ? contacts[0] : new CustomerContact();
            var oldContact = customerContact.DeepClone();
            
            if (fields.ContainsKey(ECustomerFields.Country))
            {
                var countryName = fields[ECustomerFields.Country].AsString();
                
                if (!string.IsNullOrEmpty(countryName))
                {
                    var country = CountryService.GetCountryByName(countryName);
                    if (country == null)
                    {
                        country = new Country {Name = countryName, Iso2 = "", Iso3 = ""};
                        CountryService.Add(country);
                    }

                    customerContact.Country = country.Name;
                    customerContact.CountryId = country.CountryId;
                }
            }
            if (fields.ContainsKey(ECustomerFields.Region))
            {
                var regionName = fields[ECustomerFields.Region].AsString();
                if (!string.IsNullOrEmpty(regionName))
                {
                    var region = RegionService.GetRegion(regionName, customerContact.CountryId);
                    if (region == null)
                    {
                        region = new Region {CountryId = customerContact.CountryId, Name = regionName};
                        RegionService.InsertRegion(region);
                    }

                    customerContact.Region = region.Name;
                    customerContact.RegionId = region.RegionId;
                }
            }

            if (fields.ContainsKey(ECustomerFields.City) && customerContact.RegionId != null)
            {
                var cityName = fields[ECustomerFields.City].AsString();
                
                if (!string.IsNullOrEmpty(cityName))
                {
                    var city = CityService.GetCityByName(cityName);
                    if (city == null)
                    {
                        city = new City {Name = cityName, RegionId = (int) customerContact.RegionId};
                        CityService.Add(city);
                    }

                    customerContact.City = city.Name;
                }
            }

            if (fields.TryGetValue(ECustomerFields.Zip, out var value))
                customerContact.Zip = Convert.ToString(value);
            
            if (fields.TryGetValue(ECustomerFields.House, out var field))
                customerContact.House = Convert.ToString(field);
            
            if (fields.TryGetValue(ECustomerFields.Street, out var field1))
                customerContact.Street = Convert.ToString(field1);
            
            if (fields.TryGetValue(ECustomerFields.Apartment, out var field2))
                customerContact.Apartment = Convert.ToString(field2);
            
            if (fields.TryGetValue(ECustomerFields.District, out var fieldDistrict))
                customerContact.District = Convert.ToString(fieldDistrict);
            
            if (fields.TryGetValue(ECustomerFields.Structure, out var fieldStructure))
                customerContact.Structure = Convert.ToString(fieldStructure);
            
            if (fields.TryGetValue(ECustomerFields.District, out var fieldEntrance))
                customerContact.Entrance = Convert.ToString(fieldEntrance);
            
            if (fields.TryGetValue(ECustomerFields.District, out var fieldFloor))
                customerContact.Floor = Convert.ToString(fieldFloor);

            if (ChangeHistoryService.GetChanges(0, ChangeHistoryObjType.Customer, oldContact, customerContact, new ChangedBy("")).Any())
            {
                if (customerContact.ContactId == Guid.Empty)
                    CustomerService.AddContact(customerContact, customerId);
                else
                    CustomerService.UpdateContact(customerContact, trackChanges:true, changedBy:_changedBy);
            }
        }

        private Dictionary<int, string> GetCustomerAdditionalFields(IReader csv, CustomerType customerType)
        {
            var fieldValues = new Dictionary<int, string>();
            foreach (var additionalField in CustomerFieldService.GetCustomerFields(true))
            {
                var nameField = additionalField.Name.ToLower();
                if (_fieldMapping.TryGetValue(nameField, out var value1))
                {
                    var value = csv[value1];
                    if (additionalField.Required && value.IsNullOrEmpty() && (additionalField.CustomerType == customerType || additionalField.CustomerType == CustomerType.All))
                    {
                        LogError(string.Format(LocalizationService.GetResource("Core.ExportImport.ImportCsv.CanNotEmpty"), additionalField.Name, CommonStatistic.RowPosition + 2));
                        return null;
                    }

                    switch (additionalField.FieldType)
                    {
                        case CustomerFieldType.Text:
                        case CustomerFieldType.TextArea:
                            break;

                        case CustomerFieldType.Number:
                            if (!string.IsNullOrEmpty(value) && !_numberRegex.IsMatch(value))
                            {
                                LogError(string.Format(LocalizationService.GetResource("Core.ExportImport.ImportCsv.MustBeNumber"), additionalField.Name, CommonStatistic.RowPosition + 2));
                                return null;
                            }
                            break;

                        case CustomerFieldType.Date:
                            if (!string.IsNullOrEmpty(value))
                            {
                                var dt = value.TryParseDateTime(true);
                                if (!dt.HasValue)
                                {
                                    LogError(string.Format(LocalizationService.GetResource("Core.ExportImport.ImportCsv.MustBeDateTime"), additionalField.Name, CommonStatistic.RowPosition + 2));
                                    return null;
                                }

                                value = new CustomerFieldWithValue
                                {
                                    FieldType = CustomerFieldType.Date,
                                    Value = dt.Value.ToString("yyyy-MM-dd")
                                }.Value;
                            }
                            break;

                        case CustomerFieldType.Select:
                            if (!string.IsNullOrEmpty(value))
                            {
                                var tempField = new CustomerFieldWithValue
                                {
                                    FieldType = CustomerFieldType.Select,
                                    Id = additionalField.Id,
                                    Name = additionalField.Name,
                                    Enabled = additionalField.Enabled,
                                    Required = additionalField.Required,
                                    ShowInRegistration = additionalField.ShowInRegistration,
                                    ShowInCheckout = additionalField.ShowInCheckout
                                };

                                if (!tempField.Values.Any(x => x.Value.Equals(value, StringComparison.OrdinalIgnoreCase)))
                                {
                                    LogError(string.Format("Поле {0} в строке {1} имеет недопустимое значение", additionalField.Name, CommonStatistic.RowPosition + 2));
                                    return null;
                                }
                            }
                            break;
                    }
                    fieldValues.Add(additionalField.Id, value);
                }
            }
            return fieldValues;
        }

        #region Help methods

        private bool GetString(ECustomerFields rEnum, IReaderRow csv, IDictionary<ECustomerFields, object> customerInStrings)
        {
            var nameField = rEnum.StrName();
            if (_fieldMapping.TryGetValue(nameField, out var value))
                customerInStrings.Add(rEnum, TrimAnyWay(csv[value]));
            return true;
        }

        private bool GetStringNotNull(ECustomerFields rEnum, IReaderRow csv, IDictionary<ECustomerFields, object> customerInStrings)
        {
            var nameField = rEnum.StrName();
            if (!_fieldMapping.ContainsKey(nameField)) return true;
            var tempValue = TrimAnyWay(csv[_fieldMapping[nameField]]);
            if (!string.IsNullOrEmpty(tempValue))
                customerInStrings.Add(rEnum, tempValue);
            return true;
        }

        private bool GetStringRequired(ECustomerFields rEnum, IReaderRow csv, IDictionary<ECustomerFields, object> customerInStrings)
        {
            var nameField = rEnum.StrName();
            if (!_fieldMapping.ContainsKey(nameField)) return true;
            var tempValue = TrimAnyWay(csv[_fieldMapping[nameField]]);
            if (!string.IsNullOrEmpty(tempValue))
                customerInStrings.Add(rEnum, tempValue);
            else
            {
                LogError(string.Format(LocalizationService.GetResource("Core.ExportImport.ImportCsv.CanNotEmpty"), rEnum.Localize(), CommonStatistic.RowPosition + 2));
                return false;
            }
            return true;
        }

        private bool GetDecimal(ECustomerFields rEnum, IReaderRow csv, IDictionary<ECustomerFields, object> customerInStrings)
        {
            var nameField = rEnum.StrName();
            if (!_fieldMapping.ContainsKey(nameField)) return true;
            var value = TrimAnyWay(csv[_fieldMapping[nameField]]);
            if (string.IsNullOrEmpty(value))
                value = "0";
            float decValue;
            if (float.TryParse(value, out decValue))
            {
                customerInStrings.Add(rEnum, decValue);
            }
            else if (float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out decValue))
            {
                customerInStrings.Add(rEnum, decValue);
            }
            else
            {
                LogError(string.Format(LocalizationService.GetResource("Core.ExportImport.ImportCsv.MustBeNumber"), rEnum.Localize(), CommonStatistic.RowPosition + 2));
                return false;
            }
            return true;
        }

        private bool GetNullableDecimal(ECustomerFields rEnum, IReaderRow csv, IDictionary<ECustomerFields, object> customerInStrings)
        {
            var nameField = rEnum.StrName();
            if (!_fieldMapping.ContainsKey(nameField)) return true;
            var value = TrimAnyWay(csv[_fieldMapping[nameField]]);

            if (string.IsNullOrEmpty(value))
            {
                customerInStrings.Add(rEnum, default(float?));
                return true;
            }

            float decValue;
            if (float.TryParse(value, out decValue))
            {
                customerInStrings.Add(rEnum, decValue);
            }
            else if (float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out decValue))
            {
                customerInStrings.Add(rEnum, decValue);
            }
            else
            {
                LogError(string.Format(LocalizationService.GetResource("Core.ExportImport.ImportCsv.MustBeNumber"), rEnum.Localize(), CommonStatistic.RowPosition + 2));
                return false;
            }
            return true;
        }

        private bool GetInt(ECustomerFields rEnum, IReaderRow csv, IDictionary<ECustomerFields, object> customerInStrings)
        {
            var nameField = rEnum.StrName();
            if (!_fieldMapping.ContainsKey(nameField)) return true;
            var value = TrimAnyWay(csv[_fieldMapping[nameField]]);
            if (string.IsNullOrEmpty(value))
                value = "0";
            int intValue;
            if (int.TryParse(value, out intValue))
            {
                customerInStrings.Add(rEnum, intValue);
            }
            else
            {
                LogError(string.Format(LocalizationService.GetResource("Core.ExportImport.ImportCsv.MustBeNumber"), rEnum.Localize(), CommonStatistic.RowPosition + 2));
                return false;
            }
            return true;
        }

        private bool GetDateTime(ECustomerFields rEnum, IReaderRow csv, IDictionary<ECustomerFields, object> customerInStrings)
        {
            var nameField = rEnum.StrName();
            if (!_fieldMapping.ContainsKey(nameField)) return true;
            var value = TrimAnyWay(csv[_fieldMapping[nameField]]);
            if (string.IsNullOrEmpty(value))
                value = default(DateTime).ToString(CultureInfo.InvariantCulture);
            DateTime dateValue;
            if (DateTime.TryParse(value, out dateValue))
            {
                customerInStrings.Add(rEnum, dateValue);
            }
            else if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out dateValue))
            {
                customerInStrings.Add(rEnum, dateValue);
            }
            else
            {
                LogError(string.Format(LocalizationService.GetResource("Core.ExportImport.ImportCsv.MustBeDateTime"), rEnum.Localize(), CommonStatistic.RowPosition + 2));
                return false;
            }
            return true;
        }

        private bool GetNullableDateTime(ECustomerFields rEnum, IReaderRow csv, IDictionary<ECustomerFields, object> customerInStrings)
        {
            var nameField = rEnum.StrName();
            if (!_fieldMapping.ContainsKey(nameField)) return true;
            var value = TrimAnyWay(csv[_fieldMapping[nameField]]);

            if (string.IsNullOrEmpty(value))
            {
                customerInStrings.Add(rEnum, default(DateTime?));
                return true;
            }

            DateTime dateValue;
            if (DateTime.TryParse(value, out dateValue))
            {
                customerInStrings.Add(rEnum, dateValue);
            }
            else if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out dateValue))
            {
                customerInStrings.Add(rEnum, dateValue);
            }
            else
            {
                LogError(string.Format(LocalizationService.GetResource("Core.ExportImport.ImportCsv.MustBeDateTime"), rEnum.Localize(), CommonStatistic.RowPosition + 2));
                return false;
            }
            return true;
        }

        private static string TrimAnyWay(string str)
        {
            return string.IsNullOrEmpty(str) ? str : str.Trim();
        }

        private void LogError(string message)
        {
            DoForCommonStatistic(() =>
            {
                CommonStatistic.WriteLog(message);
                CommonStatistic.TotalErrorRow++;
                //CommonStatistic.RowPosition++;
            });
        }

        private void Log(string message)
        {
            DoForCommonStatistic(() => CommonStatistic.WriteLog(message));
        }

        protected void DoForCommonStatistic(Action commonStatisticAction)
        {
            if (_useCommonStatistic)
                commonStatisticAction();
        }

        #endregion
    }
}