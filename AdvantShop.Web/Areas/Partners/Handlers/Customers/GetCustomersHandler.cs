using System;
using AdvantShop.Areas.Partners.Models.Customers;
using AdvantShop.Areas.Partners.ViewModels.Customers;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Models;
using AdvantShop.Core.Services.Partners;
using AdvantShop.Core.SQL2;

namespace AdvantShop.Areas.Partners.Handlers.Customers
{
    public class GetCustomersHandler
    {
        private readonly int _currentPage;
        private readonly int? _itemsPerPage;

        private readonly Partner _partner;

        public GetCustomersHandler(int? page = null, int? itemsPerPage = null)
        {
            _currentPage = page ?? 1;
            _itemsPerPage = itemsPerPage;
            _partner = PartnerContext.CurrentPartner;
        }

        public CustomersViewModel Get()
        {
            var paging = new SqlPaging();
            var model = new CustomersViewModel();

            paging.Select(
                "Customer.CustomerId",
                "Customer.Email",
                "Customer.Phone",
                "BindedCustomer.DateCreated".AsSqlField("BindDate"),
                "BindedCustomer.VisitDate",
                "BindedCustomer.CouponCode",
                "BindedCustomer.Url",
                "BindedCustomer.UrlReferrer",
                "BindedCustomer.UtmSource",
                "BindedCustomer.UtmMedium",
                "BindedCustomer.UtmCampaign",
                "BindedCustomer.UtmTerm",
                "BindedCustomer.UtmContent",
                ("(SELECT ISNULL(SUM((o.[Sum] - o.ShippingCost - o.PaymentCost) * oCurr.CurrencyValue),0) FROM [Order].[Order] o " +
                    "INNER JOIN Partners.[Transaction] t ON t.OrderId = o.OrderId " +  // есть начисления с заказа
                    "INNER JOIN [Order].[OrderCustomer] oCust ON o.[OrderID] = oCust.[OrderId] " +
                    "INNER JOIN [order].[OrderCurrency] oCurr ON oCurr.OrderID = o.OrderID " +
                    "WHERE oCust.CustomerID = Customer.[CustomerId] AND o.[PaymentDate] IS NOT NULL)").AsSqlField("PaymentSum"),
                ("(SELECT ISNULL(SUM(t.Amount * tCurr.CurrencyValue), 0) FROM Partners.[Transaction] t " +
                    "INNER JOIN Partners.TransactionCurrency tCurr ON tCurr.TransactionId = t.Id " +
                    "WHERE t.Amount > 0 AND t.CustomerId = Customer.CustomerID)").AsSqlField("RewardSum")
                );

            paging.From("Customers.Customer");
            paging.Inner_Join("Partners.BindedCustomer on BindedCustomer.CustomerId = Customer.CustomerId");
            paging.OrderByDesc("BindedCustomer.DateCreated");
            paging.Where("BindedCustomer.PartnerId = {0}", _partner.Id);

            paging.ItemsPerPage = _itemsPerPage.HasValue ? _itemsPerPage.Value : 10;
            paging.CurrentPageIndex = _currentPage != 0 ? _currentPage : 1;

            var totalCount = paging.TotalRowsCount;
            var totalPages = paging.PageCount(totalCount);

            model.Pager = new Pager()
            {
                TotalItemsCount = totalCount,
                TotalPages = totalPages,
                CurrentPage = _currentPage
            };

            if ((totalPages < _currentPage && _currentPage > 1) || _currentPage < 0)
            {
                return model;
            }

            model.Customers = paging.PageItemsList<CustomerModel>();

            return model;
        }
    }
}