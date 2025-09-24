using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.SQL2;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using AdvantShop.Localization;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Handlers.Marketing.Analytics.Reports
{
    public class ProductOrdersStatisticsFilterModel : BaseFilterModel
    {
        public string DateFrom { get; set; }

        public string DateTo { get; set; }

        public int ProductId { get; set; }

        public bool? Paid { get; set; }
    }

    public class ProductOrdersStatisticsItemsModel
    {
        public int OrderId { get; set; }
        public string Number { get; set; }
        public bool IsPaid { get; set; }

        public float Sum { get; set; }
        public string SumFormatted
        {
            get
            {
                return PriceFormatService.FormatPrice(Sum, CurrencyValue, CurrencySymbol, CurrencyCode, IsCodeBefore);
            }
        }

        public string CurrencyCode { get; set; }
        public float CurrencyValue { get; set; }
        public string CurrencySymbol { get; set; }
        public bool IsCodeBefore { get; set; }

        public DateTime OrderDate { get; set; }
        public string OrderDateFormatted
        {
            get { return Culture.ConvertDate(OrderDate); }
        }

        public Guid CustomerId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Organization { get; set; }
        public string BuyerName { get; set; }
        public float ProductAmount { get; set; }
        public string Email { get; set; }
    }

    public class ProductOrdersStatistics
    {
        private readonly ProductOrdersStatisticsFilterModel _filterModel;
        private SqlPaging _paging;

        public ProductOrdersStatistics(ProductOrdersStatisticsFilterModel filterModel)
        {
            _filterModel = filterModel;
        }

        public FilterResult<ProductOrdersStatisticsItemsModel> Execute()
        {
            var model = new FilterResult<ProductOrdersStatisticsItemsModel>();

            GetPaging();

            model.TotalItemsCount = _paging.TotalRowsCount;
            model.TotalPageCount = _paging.PageCount();

            if (model.TotalPageCount < _filterModel.Page && _filterModel.Page > 1)
            {
                return model;
            }

            model.DataItems = _paging.PageItemsList<ProductOrdersStatisticsItemsModel>();

            model.TotalString = string.Format(LocalizationService.GetResource("Admin.Reports.ProductOrdersStatistics.OrdersFound"), model.TotalItemsCount,
                PriceFormatService.FormatPrice(
                    _paging.GetCustomData("Sum ([Order].[Sum]*[OrderCurrency].[CurrencyValue]) as totalPrice", "", reader => SQLDataHelper.GetFloat(reader, "totalPrice"), true).FirstOrDefault(),
                    SettingsCatalog.DefaultCurrency));

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
                "[Order].[OrderID]",
                "Number",
                "case IsNumeric(number) when 1 then Replicate('0', 20 - Len(number)) + number else number end".AsSqlField("SortNumber"),
                "(case when PaymentDate is null then 0 else 1 end)".AsSqlField("IsPaid"),
                ("(Select [OrderCustomer].FirstName + ' ' + [OrderCustomer].LastName + ' ' + (case when [OrderCustomer].Organization is null or [OrderCustomer].Organization = '' then '' else  ' (' + [OrderCustomer].Organization  + ')' end ))").AsSqlField("BuyerName"),
                "[OrderCustomer].FirstName",
                "[OrderCustomer].LastName",
                "[OrderCustomer].Email",
                "Sum",
                "OrderDate",
                "CurrencyValue",
                "CurrencyCode",
                "CurrencySymbol",
                "[OrderCustomer].CustomerID",
                "[OrderCustomer].Organization",
                "IsCodeBefore",
                "(Select Sum(oi.[Amount]) From [Order].[OrderItems] as oi Where oi.[OrderId]=[Order].[OrderID] and oi.ProductId=[OrderItems].[ProductId])".AsSqlField("ProductAmount")
            );


            _paging.From("[Order].[Order]");

            _paging.Left_Join("[Order].[OrderCustomer] ON [Order].[OrderID]=[OrderCustomer].[OrderID]");
            _paging.Left_Join("[Order].[OrderCurrency] ON [Order].[OrderID] = [OrderCurrency].[OrderID]");
            _paging.Left_Join("[Customers].[Customer] ON [OrderCustomer].[CustomerId] = [Customer].[CustomerId]");
            _paging.Left_Join("[Order].[OrderItems] ON [OrderItems].[OrderID] = [Order].[OrderID]");

            Sorting();
            Filter();
        }

        private void Filter()
        {
            _paging.Where("IsDraft = {0}", false);
            _paging.Where("[OrderItems].[ProductId] = {0}", _filterModel.ProductId);

            if (_filterModel.Paid != null)
            {
                _paging.Where("PaymentDate is " + (_filterModel.Paid.Value ? "not" : "") + " null");
            }

            if (!string.IsNullOrEmpty(_filterModel.Search))
            {
                _paging.Where(
                    "(Number LIKE '%'+{0}+'%' OR " +
                    "[OrderCustomer].FirstName LIKE '%'+{0}+'%' OR " +
                    "[OrderCustomer].LastName LIKE '%'+{0}+'%' OR " +
                    "[OrderCustomer].FirstName + ' ' + [OrderCustomer].LastName LIKE '%'+{0}+'%' OR " +
                    "[OrderCustomer].LastName + ' ' + [OrderCustomer].FirstName LIKE '%'+{0}+'%' OR " +
                    "[OrderCustomer].Email LIKE '%'+{0}+'%' OR " +
                    "[OrderCustomer].Organization LIKE '%'+{0}+'%' OR " +
                    "[Customer].Phone LIKE '%'+{0}+'%')",
                    _filterModel.Search);
            }

            DateTime from, to;

            if (!string.IsNullOrWhiteSpace(_filterModel.DateFrom) && DateTime.TryParse(_filterModel.DateFrom, out from))
            {
                _paging.Where("OrderDate >= {0}", from);
            }

            if (!string.IsNullOrWhiteSpace(_filterModel.DateTo) && DateTime.TryParse(_filterModel.DateTo, out to))
            {
                to = new DateTime(to.Year, to.Month, to.Day, 23, 59, 59);
                _paging.Where("OrderDate <= {0}", to);
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
        }

        private void Sorting()
        {
            if (string.IsNullOrEmpty(_filterModel.Sorting) || _filterModel.SortingType == FilterSortingType.None)
            {
                _paging.OrderByDesc("OrderDate");
                return;
            }

            var sorting = _filterModel.Sorting.ToLower().Replace("number", "sortnumber").Replace("formatted", "");

            var field = _paging.SelectFields().FirstOrDefault(x => x.FieldName == sorting);
            if (field != null)
            {
                if (_filterModel.SortingType == FilterSortingType.Asc)
                {
                    _paging.OrderBy(sorting);
                }
                else
                {
                    _paging.OrderByDesc(sorting);
                }
            }
        }
    }
}