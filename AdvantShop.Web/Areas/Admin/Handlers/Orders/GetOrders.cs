using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.SQL2;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using AdvantShop.Saas;
using AdvantShop.Web.Admin.Models.Orders;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Handlers.Orders
{
    public class GetOrdersHandler
    {
        private readonly OrdersFilterModel _filterModel;
        private SqlPaging _paging;
        private readonly List<CustomerField> _customerFields;

        public GetOrdersHandler(OrdersFilterModel filterModel)
        {
            _filterModel = filterModel;
            _customerFields = CustomerFieldService.GetCustomerFields();
        }

        public OrdersFilterResult Execute()
        {
            var model = new OrdersFilterResult();

            GetPaging();

            model.TotalItemsCount = _paging.TotalRowsCount;
            model.TotalPageCount = _paging.PageCount();

            var countPaidOrder = OrderStatusService.GetOrderCountByPaymentStatus(true);
            var countNotPaidOrder = OrderStatusService.GetOrderCountByPaymentStatus(false);
            model.OrdersCountPreFilter = new Dictionary<OrdersPreFilterType, int>
            {
                { OrdersPreFilterType.Paid, countPaidOrder },
                { OrdersPreFilterType.NotPaid, countNotPaidOrder },
                { OrdersPreFilterType.Drafts, OrderStatusService.GetCountDraftOrder() },
                { OrdersPreFilterType.New, OrderStatusService.GetOrderCountByStatusId(OrderStatusService.DefaultOrderStatus) },
                { OrdersPreFilterType.None, countPaidOrder + countNotPaidOrder }
            };

            var groupOrders = OrderService.GetListOrderCountGroupByStatus();
            foreach (var status in groupOrders)
                model.OrdersCount.Add(status.StatusID, status.CountOrdersInStatus);

            if (model.TotalPageCount < _filterModel.Page && _filterModel.Page > 1)
            {
                return model;
            }

            model.DataItems = _paging.PageItemsList<OrderItemsModel>();

            var totalPrice =
                _paging.GetCustomData($"Sum ([Order].[Sum]/{SettingsCatalog.DefaultCurrency.Rate.ToInvariantString()}*[OrderCurrency].[CurrencyValue]) as totalPrice", "",
                    reader => SQLDataHelper.GetFloat(reader, "totalPrice"), true).FirstOrDefault();

            model.TotalString = string.Format(LocalizationService.GetResource("Admin.Handlers.Orders.GetOrders.OrdersFound"), model.TotalItemsCount,
                totalPrice.FormatPrice(SettingsCatalog.DefaultCurrency));
            
            return model;
        }

        public List<int> GetItemsIds(string fieldName)
        {
            GetPaging();

            return _paging.ItemsIds<int>(fieldName);
        }

        private void GetPaging()
        {
            _paging = new SqlPaging()
            {
                ItemsPerPage = _filterModel.ItemsPerPage,
                CurrentPageIndex = _filterModel.Page
            };

            _paging.Select(
                    "[Order].OrderID",
                    "[Order].Number",
                    "[Order].OrderDate",
                    "[Order].PaymentDate",
                    "[Order].Sum",
                    "[Order].OrderStatusID".AsSqlField("OrderStatusID"),
                    "[Order].PaymentMethodName",
                    "[Order].ShippingMethodName",
                    "[Order].ManagerConfirmed",
                    "[Order].ManagerId",

                    "[OrderStatus].StatusName",
                    "[OrderStatus].Color",

                    "CASE WHEN [Order].PaymentMethodID IS NOT NULL THEN [PaymentMethod].Name ELSE [Order].PaymentMethodName END".AsSqlField("PaymentMethod"),
                    "[ShippingMethod].Name".AsSqlField("ShippingMethod"),
                    "[OrderCustomer].Zip",
                    "[OrderCustomer].Street",
                    "[OrderCustomer].House",
                    "[OrderCustomer].Structure",
                    "[OrderCustomer].Apartment",
                    "[OrderCustomer].CustomerID",
                    "[OrderCustomer].FirstName",
                    "[OrderCustomer].LastName",
                    "[OrderCustomer].City",
                    "[OrderCustomer].Organization",

                    "[OrderCurrency].CurrencyCode",
                    "[OrderCurrency].CurrencyNumCode",
                    "[OrderCurrency].CurrencyValue",
                    "[OrderCurrency].CurrencySymbol",
                    "[OrderCurrency].IsCodeBefore",
                    "[OrderCurrency].RoundNumbers",
                    "[OrderCurrency].EnablePriceRounding",
                    "[Order].[GroupName]",
                    "[Order].[DeliveryDate]",
                    "[Order].[DeliveryTime]"
                )
                .From("[Order].[Order]")
                
                .Left_Join("[Order].[OrderCustomer] ON [Order].[OrderID] = [OrderCustomer].[OrderID]")
                .Left_Join("[Order].[OrderStatus] ON [OrderStatus].[OrderStatusID] = [Order].[OrderStatusID]")
                .Left_Join("[Order].[OrderCurrency] ON [Order].[OrderID] = [OrderCurrency].[OrderID]")
                .Left_Join("[Order].[PaymentMethod] ON [Order].[PaymentMethodID] = [Order].[PaymentMethod].[PaymentMethodID]")
                .Left_Join("[Order].[ShippingMethod] ON [Order].[ShippingMethodID] = [Order].[ShippingMethod].ShippingMethodID")
                .Left_Join("[Customers].[Customer] ON [OrderCustomer].[CustomerId] = [Customer].[CustomerId]");

            if (!SaasDataService.IsSaasEnabled || SaasDataService.CurrentSaasData.HaveCrm)
            {
                _paging
                    .Select("[ManagerCustomer].FirstName + ' ' + [ManagerCustomer].LastName".AsSqlField("ManagerName"))
                    
                    .Left_Join("[Customers].[Managers] ON [Order].[ManagerId] = [Managers].[ManagerID]")
                    .Left_Join("[Customers].[Customer] as ManagerCustomer ON [Managers].[CustomerId] = [ManagerCustomer].[CustomerId]");
            }
            
            if (SettingsCustomers.IsRegistrationAsLegalEntity)
            {
                var field = _customerFields.FirstOrDefault(x => x.FieldAssignment == CustomerFieldAssignment.CompanyName);
                if (field != null)
                {
                    _paging
                        .Select("[CustomerFieldValuesMap].Value".AsSqlField("CompanyName"))
                        .Left_Join(
                            "[Customers].[CustomerFieldValuesMap] ON [CustomerFieldValuesMap].[CustomerId] = [OrderCustomer].[CustomerId] " +
                            "AND [OrderCustomer].[CustomerType] = 1 AND CustomerFieldId = " + field.Id);
                }
            }

            var fields = _filterModel.LoadFields;
            
            if (fields != null)
            {
                var loadOrderItems = fields.OrderItems == null || fields.OrderItems.Value ? 1 : 0;
                
                _paging.Select($"{loadOrderItems}".AsSqlField("LoadOrderItems"));
            }
            
            if (fields == null || fields.AdminOrderComment != null && fields.AdminOrderComment.Value)
                _paging.Select("[Order].AdminOrderComment");
            
            Sorting();
            Filter();
        }

        private void Filter()
        {
            if (!string.IsNullOrEmpty(_filterModel.Filter) &&
                (string.IsNullOrWhiteSpace(_filterModel.OrderDateFrom) && string.IsNullOrWhiteSpace(_filterModel.OrderDateTo)))
            {
                switch (_filterModel.Filter)
                {
                    case "lastmonth":
                        _filterModel.OrderDateFrom = DateTime.Now.Date.AddDays(-1).ToString("dd.MM.yyyy");
                        _filterModel.OrderDateFrom = DateTime.Now.Date.AddMonths(1).ToString("dd.MM.yyyy");
                        break;

                    case "today":
                        _filterModel.OrderDateFrom = DateTime.Now.Date.ToString("dd.MM.yyyy");
                        _filterModel.OrderDateFrom = DateTime.Now.Date.ToString("dd.MM.yyyy");
                        break;

                    case "yesterday":
                        _filterModel.OrderDateFrom = DateTime.Now.Date.AddDays(-1).ToString("dd.MM.yyyy");
                        _filterModel.OrderDateFrom = DateTime.Now.Date.AddDays(-1).ToString("dd.MM.yyyy");
                        break;
                }
            }

            switch (_filterModel.FilterBy)
            {
                case OrdersPreFilterType.New:
                    _filterModel.OrderStatusId = OrderStatusService.DefaultOrderStatus;
                    break;
                case OrdersPreFilterType.NotPaid:
                    _filterModel.IsPaid = false;
                    break;
                case OrdersPreFilterType.Paid:
                    _filterModel.IsPaid = true;
                    break;
                case OrdersPreFilterType.Drafts:
                    _filterModel.IsDraft = true;
                    break;
                default:
                    break;
            }
            
            _paging.Where("IsDraft = {0}", Convert.ToInt32(_filterModel.IsDraft));

            if (_filterModel.CustomerId != null)
            {
                _paging.Where("[OrderCustomer].CustomerId = {0}", _filterModel.CustomerId);
            }

            if (!string.IsNullOrEmpty(_filterModel.Search))
            {
                var conditions = new List<string>
                {
                    "Number LIKE '%'+{0}+'%'",
                    "[OrderCustomer].FirstName LIKE '%'+{0}+'%'",
                    "[OrderCustomer].LastName LIKE '%'+{0}+'%'",
                    "[OrderCustomer].FirstName + ' ' + [OrderCustomer].LastName LIKE '%'+{0}+'%'",
                    "[OrderCustomer].LastName + ' ' + [OrderCustomer].FirstName LIKE '%'+{0}+'%'",
                    "[OrderCustomer].Organization LIKE '%'+{0}+'%'",
                    "[OrderCustomer].Phone LIKE '%'+{0}+'%'",
                    "[OrderCustomer].StandardPhone LIKE '%'+{0}+'%'",
                    "[Customer].Phone LIKE '%'+{0}+'%'",
                    "[Customer].StandardPhone LIKE '%'+{0}+'%'",
                    "TrackNumber = {0}",
                };
                if (SettingsCustomers.IsRegistrationAsLegalEntity)
                {
                    if (_customerFields.Any(x => x.FieldAssignment == CustomerFieldAssignment.CompanyName))
                        conditions.Add(@"[CustomerFieldValuesMap].Value LIKE '%'+{0}+'%'");
                    var innField = _customerFields.FirstOrDefault(x => x.FieldAssignment == CustomerFieldAssignment.INN);
                    if (innField != null)
                        conditions.Add(@"(SELECT TOP(1) Value FROM [Customers].[CustomerFieldValuesMap] 
                                        WHERE [CustomerFieldValuesMap].[CustomerId] = [OrderCustomer].[CustomerId]
                                        AND [OrderCustomer].[CustomerType] = 1 AND CustomerFieldId = " + innField.Id + ") LIKE '%'+{0}+'%'");
                }
                _paging.Where($"({string.Join(" OR ", conditions)})", _filterModel.Search);
            }

            if (!string.IsNullOrWhiteSpace(_filterModel.Number))
            {
                _paging.Where("Number LIKE '%'+{0}+'%'", _filterModel.Number);
            }

            if (_filterModel.OrderStatusId != 0)
            {
                _paging.Where("[Order].[OrderStatusID] = {0}", _filterModel.OrderStatusId);
            }
            
            if (_filterModel.StatusId != null && _filterModel.StatusId != 0)
            {
                _paging.Where("[Order].[OrderStatusID] = {0}", _filterModel.StatusId.Value);
            }

            if (_filterModel.PriceFrom != 0)
            {
                _paging.Where("Sum >= {0}", _filterModel.PriceFrom);
            }
            if (_filterModel.PriceTo != 0)
            {
                _paging.Where("Sum <= {0}", _filterModel.PriceTo);
            }

            if (!string.IsNullOrEmpty(_filterModel.ProductNameArtNo))
            {
                _paging.Where(
                    "Exists(Select 1 From [Order].[OrderItems] as oi " +
                    "Where oi.[OrderId] = [Order].[OrderId] and (oi.ArtNo LIKE '%'+{0}+'%' OR oi.Name LIKE '%'+{0}+'%'))",
                    _filterModel.ProductNameArtNo);
            }
            
            if (!string.IsNullOrEmpty(_filterModel.BuyerName))
            {
                var conditions = new List<string>
                {
                    "[OrderCustomer].FirstName LIKE '%'+{0}+'%'",
                    "[OrderCustomer].LastName LIKE '%'+{0}+'%'",
                    "[OrderCustomer].FirstName + ' ' + [OrderCustomer].LastName LIKE '%'+{0}+'%'",
                    "[OrderCustomer].LastName + ' ' + [OrderCustomer].FirstName LIKE '%'+{0}+'%'",
                    "[OrderCustomer].Organization LIKE '%'+{0}+'%'"
                };
                if (SettingsCustomers.IsRegistrationAsLegalEntity)
                {
                    if (_customerFields.Any(x => x.FieldAssignment == CustomerFieldAssignment.CompanyName))
                        conditions.Add(@"[CustomerFieldValuesMap].Value LIKE '%'+{0}+'%'");
                    var innField = _customerFields.FirstOrDefault(x => x.FieldAssignment == CustomerFieldAssignment.INN);
                    if (innField != null)
                        conditions.Add(@"(SELECT TOP(1) Value FROM [Customers].[CustomerFieldValuesMap] 
                                        WHERE [CustomerFieldValuesMap].[CustomerId] = [OrderCustomer].[CustomerId]
                                        AND [OrderCustomer].[CustomerType] = 1 AND CustomerFieldId = " + innField.Id + ") LIKE '%'+{0}+'%'");
                }
                _paging.Where($"({string.Join(" OR ", conditions)})", _filterModel.BuyerName);
            }

            if (!string.IsNullOrEmpty(_filterModel.BuyerPhone))
            {
                _paging.Where("([OrderCustomer].Phone LIKE '%'+{0}+'%' OR [OrderCustomer].StandardPhone LIKE '%'+{0}+'%')", _filterModel.BuyerPhone);
            }

            if (!string.IsNullOrEmpty(_filterModel.BuyerEmail))
            {
                _paging.Where("([OrderCustomer].Email LIKE '%'+{0}+'%')", _filterModel.BuyerEmail);
            }

            if (!string.IsNullOrEmpty(_filterModel.BuyerCity))
            {
                _paging.Where("([OrderCustomer].City LIKE '%'+{0}+'%')", _filterModel.BuyerCity);
            }

            if (!string.IsNullOrEmpty(_filterModel.PaymentMethod))
            {
                _paging.Where("PaymentMethod.Name = {0}", _filterModel.PaymentMethod);
            }

            if (!string.IsNullOrEmpty(_filterModel.ShippingMethod))
            {
                _paging.Where("ShippingMethodName LIKE '%'+{0}+'%'", _filterModel.ShippingMethod);
            }

            if (!string.IsNullOrEmpty(_filterModel.CouponCode))
            {
                _paging.Where("[OrderCoupon].[Code] = {0}", _filterModel.CouponCode);
                _paging.Left_Join("[Order].[OrderCoupon] On [OrderCoupon].[OrderId] = [Order].[OrderId]");
            }

            if (_filterModel.IsPaid != null)
            {
                _paging.Where(_filterModel.IsPaid.Value ? "PaymentDate is not null" : "PaymentDate is null");
            }

            if (_filterModel.ManagerId.HasValue && 
                (!SaasDataService.IsSaasEnabled || SaasDataService.CurrentSaasData.HaveCrm))
            {
                if (_filterModel.ManagerId.Value == -1)
                {
                    _paging.Where("[Order].ManagerId IS NULL");
                }
                else
                {
                    _paging.Where("[Order].ManagerId = {0}", _filterModel.ManagerId.Value);
                }
            }

            if (!string.IsNullOrWhiteSpace(_filterModel.OrderDateFrom) && DateTime.TryParse(_filterModel.OrderDateFrom, out var from))
            {
                _paging.Where("OrderDate >= {0}", from);
            }
            if (!string.IsNullOrWhiteSpace(_filterModel.OrderDateTo) && DateTime.TryParse(_filterModel.OrderDateTo, out var to))
            {
                _paging.Where("OrderDate <= {0}", to);
            }

            if (_filterModel.OrderSourceId != null)
            {
                _paging.Where("OrderSourceId = {0}", _filterModel.OrderSourceId.Value);
            }

            if (_filterModel.DeliveryDateFrom.IsNotEmpty() &&
                DateTime.TryParse(_filterModel.DeliveryDateFrom, out var deliveryDateFrom))
            {
                _paging.Where("DeliveryDate >= {0}", deliveryDateFrom);
            }

            if (_filterModel.DeliveryDateTo.IsNotEmpty() &&
                DateTime.TryParse(_filterModel.DeliveryDateTo, out var deliveryDateTo))
            {
                deliveryDateTo = new DateTime(deliveryDateTo.Year, deliveryDateTo.Month, deliveryDateTo.Day).AddDays(1);
                _paging.Where("DeliveryDate < {0}", deliveryDateTo);
            }

            var customer = CustomerContext.CurrentCustomer;

            if (customer.IsModerator)
            {
                var manager = ManagerService.GetManager(customer.Id);
                if (manager != null && manager.Enabled)
                {
                    if (SettingsManager.ManagersOrderConstraint == ManagersOrderConstraint.Assigned)
                    {
                        _paging.Where("[Order].ManagerId = {0}", manager.ManagerId);
                    }
                    else if (SettingsManager.ManagersOrderConstraint == ManagersOrderConstraint.AssignedAndFree)
                    {
                        _paging.Where("([Order].ManagerId = {0} or [Order].ManagerId is null)", manager.ManagerId);
                    }
                }
            }

            if (_filterModel.GroupName.IsNotEmpty())
            {
                _paging.Where("[Order].[GroupName] = {0}", _filterModel.GroupName);
            }

            if (!string.IsNullOrEmpty(_filterModel.BuyerZip))
            {
                _paging.Where("([OrderCustomer].Zip LIKE '%'+{0}+'%')", _filterModel.BuyerZip);
            }

            if (!string.IsNullOrEmpty(_filterModel.BuyerApartment))
            {
                _paging.Where("([OrderCustomer].Apartment LIKE '%'+{0}+'%')", _filterModel.BuyerApartment);
            }

            if (!string.IsNullOrEmpty(_filterModel.BuyerStreet))
            {
                _paging.Where("([OrderCustomer].Street LIKE '%'+{0}+'%')", _filterModel.BuyerStreet);
            }

            if (!string.IsNullOrEmpty(_filterModel.BuyerHouse))
            {
                _paging.Where("([OrderCustomer].House LIKE '%'+{0}+'%')", _filterModel.BuyerHouse);
            }

            if (!string.IsNullOrEmpty(_filterModel.BuyerStructure))
            {
                _paging.Where("([OrderCustomer].Structure LIKE '%'+{0}+'%')", _filterModel.BuyerStructure);
            }
        }

        private void Sorting()
        {
            if (string.IsNullOrEmpty(_filterModel.Sorting) || _filterModel.SortingType == FilterSortingType.None)
            {
                _paging.OrderByDesc("[Order].OrderId");
                return;
            }

            var sorting = _filterModel.Sorting.ToLower().Replace("formatted", "");
            switch (sorting)
            {
                case "number":
                {
                    _paging.Select(
                        "(case IsNumeric(number) when 1 then Replicate('0', 20 - Len(number)) + number else number end)"
                            .AsSqlField("sortnumber"));
                
                    if (_filterModel.SortingType == FilterSortingType.Asc)
                        _paging.OrderBy("sortnumber");
                    else
                        _paging.OrderByDesc("sortnumber");
                    
                    break;
                }
                
                case "buyername":
                {
                    _paging.Select("([OrderCustomer].FirstName + ' ' + [OrderCustomer].LastName)".AsSqlField("CustomerName"));
                
                    if (_filterModel.SortingType == FilterSortingType.Asc)
                        _paging
                            .OrderBy("[OrderCustomer].Organization")
                            .OrderBy("CustomerName");
                    else
                        _paging
                            .OrderByDesc("[OrderCustomer].Organization")
                            .OrderByDesc("CustomerName");
                    
                    break;
                }
                
                case "ispaid":
                {
                    _paging.Select("(Case When [Order].PaymentDate is null Then 0 Else 1 End)".AsSqlField("IsPaid"));

                    if (_filterModel.SortingType == FilterSortingType.Asc)
                        _paging.OrderBy("IsPaid");
                    else
                        _paging.OrderByDesc("IsPaid");
                    
                    break;
                }
                
                case "deliveryaddress":
                {
                    if (_filterModel.SortingType == FilterSortingType.Asc)
                        _paging.OrderBy("[OrderCustomer].Country", 
                                        "[OrderCustomer].Region", 
                                        "[OrderCustomer].District", 
                                        "[OrderCustomer].City", 
                                        "[OrderCustomer].Street", 
                                        "[OrderCustomer].House", 
                                        "[OrderCustomer].Structure");
                    else
                        _paging.OrderByDesc("[OrderCustomer].Country",
                                        "[OrderCustomer].Region",
                                        "[OrderCustomer].District",
                                        "[OrderCustomer].City",
                                        "[OrderCustomer].Street",
                                        "[OrderCustomer].House",
                                        "[OrderCustomer].Structure");

                        break;
                }

                case "deliverydatetime":
                    {
                        if (_filterModel.SortingType == FilterSortingType.Asc)
                            _paging.OrderBy("[Order].[DeliveryDate]");
                        else
                            _paging.OrderByDesc("[Order].[DeliveryDate]");

                        break;
                    }

                default:
                {
                    var field = _paging.SelectFields().FirstOrDefault(x => x.FieldName == sorting);
                    if (field != null)
                    {
                        if (_filterModel.SortingType == FilterSortingType.Asc)
                            _paging.OrderBy(sorting);
                        else
                            _paging.OrderByDesc(sorting);
                    }
                    break;
                }
            }
        }
    }
}