using AdvantShop.Catalog;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.ChangeHistories;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Loging.TrafficSource;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.ExportImport;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using AdvantShop.Repository.Currencies;
using AdvantShop.Statistic;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MissingFieldException = CsvHelper.MissingFieldException;

namespace AdvantShop.Core.Services.ExportImport.ImportServices
{
    public class CsvImportOrdersFieldsMapping : Dictionary<string, int>
    {
        public CsvImportOrdersFieldsMapping() : base()
        {
            CustomOptionsMap = new Dictionary<int, string>();
        }

        /// <summary>
        /// key: index of column in csv; value: property name
        /// </summary>
        public Dictionary<int, string> CustomOptionsMap { get; set; }

        public void AddField(string field, int index)
        {
            if (field.IsNullOrEmpty())
                return;

            if (field == EOrderFields.OrderItemCustomOptions.StrName() && !CustomOptionsMap.ContainsKey(index))
                CustomOptionsMap.Add(index, string.Empty);
            else if (!ContainsKey(field))
                Add(field, index);
        }
    }

    public class CsvImportOrdersFields : Dictionary<EOrderFields, object>
    {
        public CsvImportOrdersFields() : base()
        {
            CustomOptions = new Dictionary<string, string>();
        }

        public Dictionary<string, string> CustomOptions { get; set; }
    }

    public class CsvImportOrders
    {
        private readonly string _fullPath;
        private readonly string _columnSeparator;
        private readonly string _customOptionOptionsSeparator;
        private readonly string _encodings;
        private readonly int _defaultOrderSource;
        private readonly bool _hasHeaders;
        private readonly bool _updateCustomer;
        private readonly OrderChangedBy _orderChangedBy;
        private readonly ChangedBy _changedBy;
        private readonly Regex _dimensionsRegex;
        private readonly List<Manager> _managers;
        private readonly List<Currency> _currencies;

        private CsvImportOrdersFieldsMapping _fieldMapping;
        private Order _order;
        private TrafficSource _orderTrafficSource;
        private bool _useCommonStatistic;

        public CsvImportOrders(string filePath, bool hasHeaders, string columnSeparator, string customOptionOptionsSeparator, 
            string encodings, CsvImportOrdersFieldsMapping fieldMapping, bool updateCustomer = true, bool useCommonStatistic = true)
        {
            _fullPath = filePath;
            _hasHeaders = hasHeaders;
            _fieldMapping = fieldMapping;
            _encodings = encodings;
            _columnSeparator = columnSeparator;
            _customOptionOptionsSeparator = customOptionOptionsSeparator;

            _updateCustomer = updateCustomer;
            _useCommonStatistic = useCommonStatistic;

            _orderChangedBy = new OrderChangedBy("CSV import") {CustomerId = CustomerContext.CurrentCustomer?.Id};
            _defaultOrderSource = OrderSourceService.GetOrderSource(OrderType.OrderImport).Id;
            _dimensionsRegex = new Regex("\\d+([.,]\\d+)?x\\d+([.,]\\d+)?x\\d+([.,]\\d+)?", RegexOptions.IgnoreCase);
            _managers = ManagerService.GetManagersList();
            _currencies = CurrencyService.GetAllCurrencies();
            _changedBy = new ChangedBy("CSV order import") { CustomerId = CustomerContext.CurrentCustomer?.Id };
        }

        private CsvReader InitReader(bool? hasHeaderRecord = null)
        {
            var csvConfiguration = CsvConstants.DefaultCsvConfiguration;
            csvConfiguration.Delimiter = _columnSeparator ?? SeparatorsEnum.SemicolonSeparated.StrName();
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

        public System.Threading.Tasks.Task<bool> ProcessThroughACommonStatistic(
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
                LogError(ex.Message);
            }
        }

        private void _process()
        {
            DoForCommonStatistic(() => CommonStatistic.WriteLog("Start of import"));

            if (_fieldMapping == null)
                MapFields();

            if (_fieldMapping == null)
                throw new Exception("can't map colums");

            if (_fieldMapping.CustomOptionsMap.Any())
                GetCustomOptionNames();

            DoForCommonStatistic(() => CommonStatistic.TotalRow = GetRowCount());

            ProcessRow();

            CacheManager.Clean();
            FileHelpers.DeleteFile(_fullPath);

            DoForCommonStatistic(() => CommonStatistic.WriteLog("End of import"));
        }

        private void MapFields()
        {
            var fieldNames = new Dictionary<string, string>();
            foreach (EOrderFields item in Enum.GetValues(typeof(EOrderFields)))
                fieldNames.TryAddValue(item.Localize(), item.ToString());
            _fieldMapping = new CsvImportOrdersFieldsMapping();
            using (var csv = InitReader(false))
            {
                csv.Read();
                for (var i = 0; i < csv.Parser.Record.Length; i++)
                {
                    var header = csv.Parser.Record[i];
                    if (header.IsNullOrEmpty() || header == EOrderFields.None.StrName())
                        continue;
                    var identPart = header.Split(':').FirstOrDefault();
                    var field = fieldNames.ContainsKey(header)
                        ? fieldNames[header]
                        : fieldNames.ElementOrDefault(identPart, null);
                    _fieldMapping.AddField(field, i);
                }
            }
        }

        private void GetCustomOptionNames()
        {
            using (var csv = InitReader(false))
            {
                csv.Read();
                var invalidIndexes = new List<int>();
                foreach (var index in _fieldMapping.CustomOptionsMap.Keys.ToList()) // ToList() needed, modifying dictionary values while iterating
                {
                    if (index > csv.Parser.Record.Length || csv.Parser.Record[index].IsNullOrEmpty())
                    {
                        invalidIndexes.Add(index);
                        continue;
                    }
                    var colonIndex = csv.Parser.Record[index].IndexOf(':');
                    // if contains ":" - take last part, else - whole string
                    var name = csv.Parser.Record[index].Substring(colonIndex + 1, csv.Parser.Record[index].Length - colonIndex - 1);
                    if (name.IsNullOrEmpty())
                    {
                        invalidIndexes.Add(index);
                        continue;
                    }
                    _fieldMapping.CustomOptionsMap[index] = name.SupperTrim();
                }
                foreach (var index in invalidIndexes)
                {
                    DoForCommonStatistic(() => CommonStatistic.WriteLog(LocalizationService.GetResourceFormat("Core.ExportImport.ImportCsvV2.WrongPropertyHeader", (index + 1))));
                    _fieldMapping.CustomOptionsMap.Remove(index);
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
            if (!File.Exists(_fullPath)) 
                return;
            
            using (var csv = InitReader())
            {
                if (_hasHeaders)
                {
                    csv.Read();
                    csv.ReadHeader();
                }
                
                while (csv.Read())
                {
                    if (csv.Parser.Row == 1 && csv.Parser.Record != null && 
                        csv.Parser.Record.Length > 0 && csv.Parser.Record[0].Equals("номер заказа", StringComparison.OrdinalIgnoreCase)) //  first row is header
                    {
                        DoForCommonStatistic(() => CommonStatistic.RowPosition++);
                        continue;
                    }

                    if (_useCommonStatistic && (!CommonStatistic.IsRun || CommonStatistic.IsBreaking))
                    {
                        csv.Dispose();
                        FileHelpers.DeleteFile(_fullPath);
                        return;
                    }
                    try
                    {
                        var orderInString = PrepareRow(csv);
                        if (orderInString == null)
                        {
                            DoForCommonStatistic(() => CommonStatistic.RowPosition++);
                            continue;
                        }

                        ProcessOrder(orderInString, csv);
                    }
                    catch (Exception ex)
                    {
                        Debug.Log.Error(ex);
                        LogError($"{CommonStatistic.RowPosition}: {ex.Message}");
                    }
                }
                if (_order != null)
                    AddUpdateOrder();
            }
        }

        private CsvImportOrdersFields PrepareRow(IReader csv)
        {
            var orderInStrings = new CsvImportOrdersFields();

            foreach (EOrderFields field in Enum.GetValues(typeof(EOrderFields)))
            {
                try
                {
                    if (field == EOrderFields.OrderItemCustomOptions)
                    {
                        foreach (var index in _fieldMapping.CustomOptionsMap.Keys)
                            if (!orderInStrings.CustomOptions.ContainsKey(_fieldMapping.CustomOptionsMap[index]))
                                orderInStrings.CustomOptions.Add(_fieldMapping.CustomOptionsMap[index], csv[index].DefaultOrEmpty().SupperTrim());
                        
                        continue;
                    }

                    switch (field.Status())
                    {
                        case CsvFieldStatus.String:
                            GetString(field, csv, orderInStrings);
                            break;
                        case CsvFieldStatus.StringRequired:
                            GetStringRequired(field, csv, orderInStrings);
                            break;
                        case CsvFieldStatus.NotEmptyString:
                            GetStringNotNull(field, csv, orderInStrings);
                            break;
                        case CsvFieldStatus.Float:
                            if (!GetDecimal(field, csv, orderInStrings))
                                return null;
                            break;
                        case CsvFieldStatus.NullableFloat:
                            if (!GetNullableDecimal(field, csv, orderInStrings))
                                return null;
                            break;
                        case CsvFieldStatus.Int:
                            if (!GetInt(field, csv, orderInStrings))
                                return null;
                            break;
                        case CsvFieldStatus.DateTime:
                            if (!GetDateTime(field, csv, orderInStrings))
                                return null;
                            break;
                        case CsvFieldStatus.NullableDateTime:
                            if (!GetNullableDateTime(field, csv, orderInStrings))
                                return null;
                            break;
                    }
                }
                catch (MissingFieldException)
                {
                    LogError($"Строка №{CommonStatistic.RowPosition}: Не валидный формат строки - пропущено поле {field.Localize()}");
                    return null;
                }
            }
            return orderInStrings;
        }

        private void ProcessOrder(CsvImportOrdersFields orderInStrings, IReader csv)
        {
            try
            {
                var orderNumber = orderInStrings.TryGetValue(EOrderFields.Number, out var value)
                    ? value.AsString()
                    : null;
                if (orderNumber.IsNullOrEmpty())
                {
                    DoForCommonStatistic(() =>
                    {
                        CommonStatistic.RowPosition++;
                        CommonStatistic.TotalErrorRow++;
                        CommonStatistic.WriteLog("Wrong order number");
                    });
                    return;
                }
                if (_order?.Number != null && _order.Number != orderNumber)
                {
                    AddUpdateOrder();
                }
                else if (_order?.Number != null)
                {
                    ProcessOrderItem(orderInStrings);
                    DoForCommonStatistic(() =>
                    {
                        CommonStatistic.RowPosition++;
                    });
                    return;
                }

                _order = OrderService.GetOrderByNumber(orderNumber) ?? new Order() { Number = orderNumber };
                _order.OrderItems.Clear();
                _orderTrafficSource = null;

                if (orderInStrings.TryGetValue(EOrderFields.OrderStatus, out value))
                {
                    var orderStatusName = value.AsString();
                    if (orderStatusName.IsNotEmpty())
                    {
                        var orderStatus = OrderStatusService.GetOrderStatusByName(orderStatusName);
                        if (orderStatus == null)
                        {
                            orderStatus = new OrderStatus
                            {
                                StatusName = orderStatusName
                            };
                            orderStatus.StatusID = OrderStatusService.AddOrderStatus(orderStatus);
                        }
                        if (orderStatus.StatusID != 0)
                            _order.OrderStatusId = orderStatus.StatusID;
                    }
                }
                if (_order.OrderStatusId == 0)
                    _order.OrderStatusId = OrderStatusService.DefaultOrderStatus;

                _order.OrderDate = DateTime.Now;
                if (orderInStrings.TryGetValue(EOrderFields.OrderDateTime, out value))
                {
                    var orderDate = value.AsString();
                    if (orderDate.IsNotEmpty())
                        _order.OrderDate = orderDate.TryParseDateTime();
                }

                if (orderInStrings.TryGetValue(EOrderFields.OrderSource, out value))
                {
                    var orderSourceName = value.AsString();
                    if (orderSourceName.IsNotEmpty())
                    {
                        var orderSource = OrderSourceService.GetOrderSource(orderSourceName);
                        _order.OrderSourceId = orderSource?.Id ?? _defaultOrderSource;
                    }
                }

                if (_order.OrderID == 0 || _updateCustomer)
                    ProcessOrderCustomer(orderInStrings);

                if (orderInStrings.TryGetValue(EOrderFields.CustomerGroup, out value))
                {
                    var groupName = value.AsString();
                    _order.GroupName = groupName;
                }

                _order.OrderRecipient = _order.OrderRecipient ?? new OrderRecipient();
                if (orderInStrings.TryGetValue(EOrderFields.RecipientFullName, out value))
                {
                    var (lastName, firstName, patronymic) = GetFIOFromFullName(value.AsString());
                    _order.OrderRecipient.LastName = lastName;
                    _order.OrderRecipient.FirstName = firstName;
                    _order.OrderRecipient.Patronymic = patronymic;
                }

                if (orderInStrings.TryGetValue(EOrderFields.RecipientPhone, out value))
                {
                    _order.OrderRecipient.Phone = value.AsString();
                    _order.OrderRecipient.StandardPhone = _order.OrderRecipient.Phone.IsNullOrEmpty()
                        ? null
                        : StringHelper.ConvertToStandardPhone(_order.OrderRecipient.Phone);
                }

                if (orderInStrings.TryGetValue(EOrderFields.Weight, out value))
                    _order.TotalWeight = value.AsString().TryParseFloat();
                
                if (orderInStrings.TryGetValue(EOrderFields.Dimensions, out value))
                {
                    var dimensionStr = value.AsString();
                    if (dimensionStr.IsNotEmpty())
                    {
                        var dimensions = _dimensionsRegex.Match(dimensionStr);
                        var dimnesionsArr = dimensions.Value.Split(new char[] { 'x' }, StringSplitOptions.RemoveEmptyEntries);
                        if (dimnesionsArr.Length == 3)
                        {
                            _order.TotalLength = dimnesionsArr[0].TryParseFloat();
                            _order.TotalWidth = dimnesionsArr[1].TryParseFloat();
                            _order.TotalHeight = dimnesionsArr[2].TryParseFloat();
                        }
                    }
                }

                if (orderInStrings.TryGetValue(EOrderFields.DiscountValue, out value))
                {
                    var discount = value.AsString().TryParseFloat();
                    _order.OrderDiscountValue = discount;
                }

                if (orderInStrings.TryGetValue(EOrderFields.ShippingCost, out value))
                {
                    var shippingCost = value.AsString();
                    _order.ShippingCost = shippingCost.TryParseFloat();
                }

                if (orderInStrings.TryGetValue(EOrderFields.ShippingName, out value))
                    _order.ArchivedShippingName = value.AsString();
                
                if (orderInStrings.TryGetValue(EOrderFields.PaymentCost, out value))
                {
                    var paymentCost = value.AsString();
                    _order.PaymentCost = paymentCost.TryParseFloat();
                }

                if (orderInStrings.TryGetValue(EOrderFields.PaymentName, out value))
                    _order.ArchivedPaymentName = value.AsString();

                var couponValue = 0f;
                if (orderInStrings.TryGetValue(EOrderFields.Coupon, out value))
                    couponValue = value.AsString().TryParseFloat();

                string couponCode = null;
                if (orderInStrings.TryGetValue(EOrderFields.CouponCode, out value))
                    couponCode = value.AsString();
                if (couponCode.IsNotEmpty() || couponValue != 0)
                    _order.Coupon = new OrderCoupon
                    {
                        Code = couponCode,
                        Value = couponValue,
                        Type = CouponType.Fixed
                    };

                if (orderInStrings.TryGetValue(EOrderFields.BonusCost, out value))
                    _order.BonusCost = value.AsString().TryParseFloat();

                if (orderInStrings.TryGetValue(EOrderFields.Currency, out value))
                {
                    var currencySymbol = value.AsString()?.Trim();
                    if (currencySymbol.IsNotEmpty())
                    {
                        var currency = _currencies.Find(x =>
                            x.Symbol != null &&
                            x.Symbol.Trim().Equals(currencySymbol, StringComparison.OrdinalIgnoreCase));
                        if (currency != null)
                            _order.OrderCurrency = currency;
                    }
                }

                if (orderInStrings.TryGetValue(EOrderFields.DeliveryDate, out value))
                {
                    var deliveryDate = value.AsString().TryParseDateTime(true);
                    _order.DeliveryDate = deliveryDate;
                }

                if (orderInStrings.TryGetValue(EOrderFields.DeliveryTime, out value))
                    _order.DeliveryTime = value.AsString();

                if (orderInStrings.TryGetValue(EOrderFields.CustomerComment, out value))
                    _order.CustomerComment = value.AsString();

                if (orderInStrings.TryGetValue(EOrderFields.AdminComment, out value))
                    _order.AdminOrderComment = value.AsString();

                if (orderInStrings.TryGetValue(EOrderFields.StatusComment, out value))
                    _order.StatusComment = value.AsString();

                if (orderInStrings.TryGetValue(EOrderFields.Payed, out value) && value.AsString()?.Equals("да", StringComparison.OrdinalIgnoreCase) is true)
                    _order.PaymentDate = DateTime.Now;
                else
                    _order.PaymentDate = null;

                if (orderInStrings.TryGetValue(EOrderFields.Manager, out value))
                {
                    var managerName = value.AsString();
                    var manager = _managers.FirstOrDefault(x => managerName.Contains(x.FirstName, StringComparison.OrdinalIgnoreCase)
                        && managerName.Contains(x.LastName, StringComparison.OrdinalIgnoreCase));

                    _order.ManagerId = manager?.ManagerId;
                }

                if (_order.OrderID == 0 && _order.OrderCustomer != null && _order.OrderCustomer.CustomerID != Guid.Empty)
                {
                    _orderTrafficSource = new TrafficSource
                    {
                        CustomerId = _order.OrderCustomer.CustomerID,
                        CreateOn = DateTime.Now
                    };
                    if (orderInStrings.TryGetValue(EOrderFields.GoogleClientId, out value))
                        _orderTrafficSource.GoogleClienId = value.AsString() ?? string.Empty;
                    if (orderInStrings.TryGetValue(EOrderFields.YandexClientId, out value))
                        _orderTrafficSource.YandexClientId = value.AsString() ?? string.Empty;
                    if (orderInStrings.TryGetValue(EOrderFields.Referral, out value))
                        _orderTrafficSource.Referrer = value.AsString();
                    if (orderInStrings.TryGetValue(EOrderFields.LoginPage, out value))
                        _orderTrafficSource.Url = value.AsString();
                    if (orderInStrings.TryGetValue(EOrderFields.UtmSource, out value))
                        _orderTrafficSource.utm_source = value.AsString();
                    if (orderInStrings.TryGetValue(EOrderFields.UtmMedium, out value))
                        _orderTrafficSource.utm_medium = value.AsString();
                    if (orderInStrings.TryGetValue(EOrderFields.UtmCampaign, out value))
                        _orderTrafficSource.utm_campaign = value.AsString();
                    if (orderInStrings.TryGetValue(EOrderFields.UtmContent, out value))
                        _orderTrafficSource.utm_content = value.AsString();
                    if (orderInStrings.TryGetValue(EOrderFields.UtmTerm, out value))
                        _orderTrafficSource.utm_term = value.AsString();
                }

                if (orderInStrings.TryGetValue(EOrderFields.Sum, out value))
                    _order.Sum = value.AsString().TryParseFloat();

                ProcessOrderItem(orderInStrings);
            }
            catch (Exception e)
            {
                Debug.Log.Error(e);
                LogError(CommonStatistic.RowPosition + ": " + e.Message);
            }

            orderInStrings.Clear();
            DoForCommonStatistic(() => CommonStatistic.RowPosition++);
        }

        private void ProcessOrderItem(CsvImportOrdersFields orderInStrings)
        {
            string artNo = null;
            if (orderInStrings.TryGetValue(EOrderFields.OrderItemArtNo, out var value))
                artNo = value.AsString();
            
            if (artNo.IsNullOrEmpty())
                return;

            var orderItem = new OrderItem
            {
                ArtNo = artNo
            };

            if (orderInStrings.TryGetValue(EOrderFields.OrderItemName, out value))
                orderItem.Name = value.AsString();

            if (orderInStrings.TryGetValue(EOrderFields.OrderItemSize, out value))
                orderItem.Size = value.AsString();

            if (orderInStrings.TryGetValue(EOrderFields.OrderItemColor, out value))
                orderItem.Color = value.AsString();

            if (orderInStrings.TryGetValue(EOrderFields.OrderItemPrice, out value))
                orderItem.Price = value.AsString().TryParseFloat();

            if (orderInStrings.TryGetValue(EOrderFields.OrderItemAmount, out value))
                orderItem.Amount = value.AsString().TryParseFloat();

            if (orderInStrings.CustomOptions.Count > 0)
            {
                orderItem.SelectedOptions = orderInStrings.CustomOptions
                    .SelectMany(customOption => customOption.Value.Split(_customOptionOptionsSeparator)
                    .Where(option => option.IsNotEmpty())
                    .Select(option => new EvaluatedCustomOptions
                    {
                        CustomOptionTitle = customOption.Key,
                        OptionTitle = customOption.Key != option ? option : string.Empty
                    })).ToList();
            }

            if (_order.Coupon != null)
                orderItem.IsCouponApplied = true;

            _order.OrderItems.Add(orderItem);
        }

        private void ProcessOrderCustomer(CsvImportOrdersFields orderInStrings)
        {
            _order.OrderCustomer = _order.OrderCustomer ?? new OrderCustomer();
            if (orderInStrings.TryGetValue(EOrderFields.FullName, out var value))
            {
                var (lastName, firstName, patronymic) = GetFIOFromFullName(value.AsString());
                _order.OrderCustomer.LastName = lastName;
                _order.OrderCustomer.FirstName = firstName;
                _order.OrderCustomer.Patronymic = patronymic;
            }
            Customer existsCustomer = null;
            if (orderInStrings.TryGetValue(EOrderFields.Email, out value))
            {
                _order.OrderCustomer.Email = value.AsString();
                if (_order.OrderCustomer.Email.IsNotEmpty())
                    existsCustomer = CustomerService.GetCustomerByEmail(_order.OrderCustomer.Email);
            }

            if (orderInStrings.TryGetValue(EOrderFields.Phone, out value))
            {
                _order.OrderCustomer.Phone = value.AsString();
                if (_order.OrderCustomer.Phone.IsNotEmpty())
                {
                    _order.OrderCustomer.StandardPhone = StringHelper.ConvertToStandardPhone(_order.OrderCustomer.Phone);
                    if (existsCustomer == null)
                        existsCustomer = CustomerService.GetCustomerByPhone(_order.OrderCustomer.Phone, _order.OrderCustomer.StandardPhone);
                }
            }

            if (existsCustomer != null)
                _order.OrderCustomer.CustomerID = existsCustomer.Id;

            if (orderInStrings.TryGetValue(EOrderFields.Country, out value))
                _order.OrderCustomer.Country = value.AsString();

            if (orderInStrings.TryGetValue(EOrderFields.City, out value))
                _order.OrderCustomer.City = value.AsString();

            if (orderInStrings.TryGetValue(EOrderFields.Region, out value))
                _order.OrderCustomer.Region = value.AsString();

            if (orderInStrings.TryGetValue(EOrderFields.District, out value))
                _order.OrderCustomer.District = value.AsString();

            if (orderInStrings.TryGetValue(EOrderFields.Street, out value))
                _order.OrderCustomer.Street = value.AsString();

            if (orderInStrings.TryGetValue(EOrderFields.Zip, out value))
                _order.OrderCustomer.Zip = value.AsString();

            if (orderInStrings.TryGetValue(EOrderFields.House, out value))
                _order.OrderCustomer.House = value.AsString();

            if (orderInStrings.TryGetValue(EOrderFields.Structure, out value))
                _order.OrderCustomer.Structure = value.AsString();

            if (orderInStrings.TryGetValue(EOrderFields.Apartment, out value))
                _order.OrderCustomer.Apartment = value.AsString();

            if (orderInStrings.TryGetValue(EOrderFields.Floor, out value))
                _order.OrderCustomer.Floor = value.AsString();

            if (orderInStrings.TryGetValue(EOrderFields.House, out value))
                _order.OrderCustomer.House = value.AsString();

            if (orderInStrings.TryGetValue(EOrderFields.Entrance, out value))
                _order.OrderCustomer.Entrance = value.AsString();

            if (existsCustomer == null && (_order.OrderCustomer.Email.IsNotEmpty() || _order.OrderCustomer.Phone.IsNotEmpty()))
            {
                _order.OrderCustomer.CustomerID = CustomerService.InsertNewCustomer(new Customer
                {
                    EMail = _order.OrderCustomer.Email,
                    Phone = _order.OrderCustomer.Phone,
                    StandardPhone = _order.OrderCustomer.StandardPhone,
                    FirstName = _order.OrderCustomer.FirstName,
                    LastName = _order.OrderCustomer.LastName,
                    Patronymic = _order.OrderCustomer.Patronymic
                }, changedBy: _changedBy);
            }
            else if (existsCustomer != null && _updateCustomer)
            {
                existsCustomer.FirstName = _order.OrderCustomer.FirstName;
                existsCustomer.LastName = _order.OrderCustomer.LastName;
                existsCustomer.Patronymic = _order.OrderCustomer.Patronymic;
                existsCustomer.Phone = _order.OrderCustomer.Phone;
                existsCustomer.EMail = _order.OrderCustomer.Email;
                CustomerService.UpdateCustomer(existsCustomer, changedBy: _changedBy);
            }

            if (_order.OrderCustomer.CustomerID != Guid.Empty)
            {
                var fullName = _order.OrderCustomer.GetFullName();
                var contacts = existsCustomer == null ? null : CustomerService.GetCustomerContacts(_order.OrderCustomer.CustomerID);
                var existsContact = contacts?.FirstOrDefault(contact =>
                    StringEqualsOrNull(contact.Name, fullName)
                    && StringEqualsOrNull(contact.Country, _order.OrderCustomer.Country)
                    && StringEqualsOrNull(contact.Region, _order.OrderCustomer.Region)
                    && StringEqualsOrNull(contact.City, _order.OrderCustomer.City)
                    && StringEqualsOrNull(contact.District, _order.OrderCustomer.District)
                    && StringEqualsOrNull(contact.Zip, _order.OrderCustomer.Zip)
                    && StringEqualsOrNull(contact.Street, _order.OrderCustomer.Street)
                    && StringEqualsOrNull(contact.House, _order.OrderCustomer.House)
                    && StringEqualsOrNull(contact.Apartment, _order.OrderCustomer.Apartment)
                    && StringEqualsOrNull(contact.Structure, _order.OrderCustomer.Structure)
                    && StringEqualsOrNull(contact.Entrance, _order.OrderCustomer.Entrance)
                    && StringEqualsOrNull(contact.Floor, _order.OrderCustomer.Floor));

                if (existsContact == null)
                    CustomerService.AddContact(new CustomerContact
                    {
                        CustomerGuid = _order.OrderCustomer.CustomerID,
                        Country = _order.OrderCustomer.Country,
                        Region = _order.OrderCustomer.Region,
                        City = _order.OrderCustomer.City,
                        District = _order.OrderCustomer.District,
                        Zip = _order.OrderCustomer.Zip,
                        Street = _order.OrderCustomer.Street,
                        House = _order.OrderCustomer.House,
                        Apartment = _order.OrderCustomer.Apartment,
                        Structure = _order.OrderCustomer.Structure,
                        Entrance = _order.OrderCustomer.Entrance,
                        Floor = _order.OrderCustomer.Floor,
                        Name = fullName
                    }, _order.OrderCustomer.CustomerID);
            }
        }

        private void AddUpdateOrder()
        {
            if (_order.OrderID == 0)
            {
                OrderService.AddOrder(_order, _orderChangedBy, true, false, false, true);
                if (_orderTrafficSource != null)
                    OrderTrafficSourceService.Add(_order.OrderID, TrafficSourceType.Order, _orderTrafficSource);
                DoForCommonStatistic(() =>
                {
                    CommonStatistic.TotalAddRow++;
                    CommonStatistic.WriteLog("Order added: " + _order.OrderID);
                });
                Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Orders_OrderCreated_Csv);
            }
            else
            {
                OrderService.UpdateOrderMain(_order, false, _orderChangedBy, true);
                if (OrderService.GetOrderCustomer(_order.OrderID) == null)
                    OrderService.AddOrderCustomer(_order.OrderID, _order.OrderCustomer, false);
                else
                    OrderService.UpdateOrderCustomer(_order.OrderCustomer, _orderChangedBy, trackChanges: true);
                OrderService.AddUpdateOrderItems(_order.OrderItems, new List<OrderItem>(), _order, _orderChangedBy, true, false);
                DoForCommonStatistic(() =>
                {
                    CommonStatistic.TotalUpdateRow++;
                    CommonStatistic.WriteLog("Order updated: " + _order.OrderID);
                });
                Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Orders_EditOrder_Csv);
            }
            OrderService.PayOrder(_order.OrderID, _order.Payed, changedBy: _orderChangedBy, onlyUpdatePaymentDate: true);
            _order = null;
        }

        #region HelpMethods

        private bool GetString(EOrderFields rEnum, IReaderRow csv, IDictionary<EOrderFields, object> orderInStrings)
        {
            var nameField = rEnum.StrName();
            if (_fieldMapping.ContainsKey(nameField))
                orderInStrings.Add(rEnum, TrimAnyWay(csv[_fieldMapping[nameField]]));
            return true;
        }

        private bool GetStringRequired(EOrderFields rEnum, IReaderRow csv, IDictionary<EOrderFields, object> orderInStrings)
        {
            var nameField = rEnum.StrName();
            if (!_fieldMapping.ContainsKey(nameField)) return true;
            var tempValue = TrimAnyWay(csv[_fieldMapping[nameField]]);
            if (!string.IsNullOrEmpty(tempValue))
                orderInStrings.Add(rEnum, tempValue);
            else
            {
                LogError(string.Format(LocalizationService.GetResource("Core.ExportImport.ImportCsv.CanNotEmpty"), rEnum.Localize(), CommonStatistic.RowPosition + 2));
                return false;
            }
            return true;
        }

        private bool GetStringNotNull(EOrderFields rEnum, IReaderRow csv, IDictionary<EOrderFields, object> orderInStrings)
        {
            var nameField = rEnum.StrName();
            if (!_fieldMapping.ContainsKey(nameField)) return true;
            var tempValue = TrimAnyWay(csv[_fieldMapping[nameField]]);
            if (!string.IsNullOrEmpty(tempValue))
                orderInStrings.Add(rEnum, tempValue);
            return true;
        }

        private bool GetDecimal(EOrderFields rEnum, IReaderRow csv, IDictionary<EOrderFields, object> orderInStrings)
        {
            var nameField = rEnum.StrName();
            if (!_fieldMapping.ContainsKey(nameField)) return true;
            var value = TrimAnyWay(csv[_fieldMapping[nameField]]);
            if (string.IsNullOrEmpty(value))
                value = "0";
            float decValue;
            if (float.TryParse(value, out decValue))
            {
                orderInStrings.Add(rEnum, decValue);
            }
            else if (float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out decValue))
            {
                orderInStrings.Add(rEnum, decValue);
            }
            else
            {
                LogError(string.Format(LocalizationService.GetResource("Core.ExportImport.ImportCsv.MustBeNumber"), rEnum.Localize(), CommonStatistic.RowPosition + 2));
                return false;
            }
            return true;
        }

        private bool GetNullableDecimal(EOrderFields rEnum, IReaderRow csv, IDictionary<EOrderFields, object> orderInStrings)
        {
            var nameField = rEnum.StrName();
            if (!_fieldMapping.ContainsKey(nameField)) return true;
            var value = TrimAnyWay(csv[_fieldMapping[nameField]]);

            if (string.IsNullOrEmpty(value))
            {
                orderInStrings.Add(rEnum, default(float?));
                return true;
            }

            float decValue;
            if (float.TryParse(value, out decValue))
            {
                orderInStrings.Add(rEnum, decValue);
            }
            else if (float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out decValue))
            {
                orderInStrings.Add(rEnum, decValue);
            }
            else
            {
                LogError(string.Format(LocalizationService.GetResource("Core.ExportImport.ImportCsv.MustBeNumber"), rEnum.Localize(), CommonStatistic.RowPosition + 2));
                return false;
            }
            return true;
        }

        private bool GetInt(EOrderFields rEnum, IReaderRow csv, IDictionary<EOrderFields, object> orderInStrings)
        {
            var nameField = rEnum.StrName();
            if (!_fieldMapping.ContainsKey(nameField)) return true;
            var value = TrimAnyWay(csv[_fieldMapping[nameField]]);
            if (string.IsNullOrEmpty(value))
                value = "0";
            int intValue;
            if (int.TryParse(value, out intValue))
            {
                orderInStrings.Add(rEnum, intValue);
            }
            else
            {
                LogError(string.Format(LocalizationService.GetResource("Core.ExportImport.ImportCsv.MustBeNumber"), rEnum.Localize(), CommonStatistic.RowPosition + 2));
                return false;
            }
            return true;
        }

        private bool GetDateTime(EOrderFields rEnum, IReaderRow csv, IDictionary<EOrderFields, object> orderInStrings)
        {
            var nameField = rEnum.StrName();
            if (!_fieldMapping.ContainsKey(nameField)) return true;
            var value = TrimAnyWay(csv[_fieldMapping[nameField]]);
            if (string.IsNullOrEmpty(value))
                value = default(DateTime).ToString(CultureInfo.InvariantCulture);
            DateTime dateValue;
            if (DateTime.TryParse(value, out dateValue))
            {
                orderInStrings.Add(rEnum, dateValue);
            }
            else if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out dateValue))
            {
                orderInStrings.Add(rEnum, dateValue);
            }
            else
            {
                LogError(string.Format(LocalizationService.GetResource("Core.ExportImport.ImportCsv.MustBeDateTime"), rEnum.Localize(), CommonStatistic.RowPosition + 2));
                return false;
            }
            return true;
        }

        private bool GetNullableDateTime(EOrderFields rEnum, IReaderRow csv, IDictionary<EOrderFields, object> orderInStrings)
        {
            var nameField = rEnum.StrName();
            if (!_fieldMapping.ContainsKey(nameField)) return true;
            var value = TrimAnyWay(csv[_fieldMapping[nameField]]);

            if (string.IsNullOrEmpty(value))
            {
                orderInStrings.Add(rEnum, default(DateTime?));
                return true;
            }

            DateTime dateValue;
            if (DateTime.TryParse(value, out dateValue))
            {
                orderInStrings.Add(rEnum, dateValue);
            }
            else if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out dateValue))
            {
                orderInStrings.Add(rEnum, dateValue);
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
            });
        }

        protected void DoForCommonStatistic(Action commonStatisticAction)
        {
            if (_useCommonStatistic)
                commonStatisticAction();
        }

        private (string lastName, string firstName, string patronymic) GetFIOFromFullName(string fullName)
        {
            if (fullName.IsNullOrEmpty())
                return (null, null, null);

            var fullNameSplitted = fullName.Split(' ');
            if (fullNameSplitted.Length == 3)
                return (fullNameSplitted[0], fullNameSplitted[1], fullNameSplitted[2]);
            else if (fullNameSplitted.Length == 2)
                return (fullNameSplitted[0], fullNameSplitted[1], null);
            else
                return (fullNameSplitted[0], null, null);
        }

        private bool StringEqualsOrNull(string str1, string str2)
        {
            if (str1.IsNullOrEmpty() && str2.IsNullOrEmpty())
                return true;

            return str1.Equals(str2, StringComparison.OrdinalIgnoreCase);
        }

        #endregion
    }
}
