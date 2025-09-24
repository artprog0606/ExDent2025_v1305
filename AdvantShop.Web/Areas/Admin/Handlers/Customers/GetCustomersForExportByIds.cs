using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.SQL2;
using AdvantShop.Customers;
using AdvantShop.Web.Admin.Models.Customers.CustomerSegments;

namespace AdvantShop.Web.Admin.Handlers.Customers
{
    public class GetCustomersForExportByIds
    {
        private readonly List<Guid> _customerIds;
        private SqlPaging _paging;

        public GetCustomersForExportByIds(List<Guid> customerIds)
        {
            _customerIds = customerIds;
        }

        public List<CustomerBySegmentViewModel> Execute()
        {
            if (_customerIds == null || _customerIds.Count == 0)
                return new List<CustomerBySegmentViewModel>();

            GetPaging();

            return _paging.PageItemsList<CustomerBySegmentViewModel>();
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
                ItemsPerPage = 1000000,
                CurrentPageIndex = 1
            };

            _paging.Select(
                "[Customer].CustomerID",
                "[Customer].Email".AsSqlField("Email"),
                "[Customer].Phone".AsSqlField("Phone"),
                "[Customer].FirstName",
                "[Customer].LastName",
                "[Customer].Patronymic",
                "(Select [Customer].Lastname + ' ' + [Customer].Firstname + ' ' + ISNULL([Customer].Patronymic,''))"
                    .AsSqlField("Name"),

                "[Customer].Organization",

                ("(Select COUNT([Order].[OrderId]) From  [Order].[Order] LEFT JOIN [Order].[OrderCustomer] ON [Order].[OrderID] = [OrderCustomer].[OrderId] " +
                 "WHERE [OrderCustomer].[CustomerId] = [Customer].[CustomerId] and [Order].[PaymentDate] is not null)")
                    .AsSqlField("OrdersCount"),

                "[Customer].RegistrationDateTime".AsSqlField("RegistrationDateTime"),
                "[Customer].BirthDay".AsSqlField("BirthDay"),

                ("(Select Top(1) [Order].OrderId From [Order].[Order] LEFT JOIN [Order].[OrderCustomer] ON [Order].[OrderId] = [OrderCustomer].[OrderId] " +
                 "WHERE [CustomerId] = [Customer].[CustomerId] Order by [OrderDate] Desc)").AsSqlField("LastOrderId"),

                ("(Select Top(1) [Order].Number From [Order].[Order] LEFT JOIN [Order].[OrderCustomer] ON [Order].[OrderId] = [OrderCustomer].[OrderId] " +
                 "WHERE [CustomerId] = [Customer].[CustomerId] Order by [OrderDate] Desc)").AsSqlField(
                     "LastOrderNumber"),

                ("(Select ISNULL(SUM([Sum]),0) From  [Order].[Order] LEFT JOIN [Order].[OrderCustomer] ON [Order].[OrderID] = [OrderCustomer].[OrderId] " +
                 "WHERE [OrderCustomer].[CustomerId] = [Customer].[CustomerId] and [Order].[PaymentDate] is not null)")
                    .AsSqlField("OrdersSum"),

                "(Select Top(1) [City] From [Customers].[Contact] Where [Contact].[CustomerID] = [Customer].[CustomerID])"
                    .AsSqlField("Location"),

                "(Select [FirstName] + ' ' + [LastName] From  [Customers].[Customer]  WHERE [CustomerId] = [Managers].[CustomerId])"
                    .AsSqlField("ManagerName")
                );

            _paging.Left_Join("[Customers].[Managers] ON [Customer].[ManagerId] = [Managers].[ManagerId]");


            _paging.From("[Customers].[Customer]");
            _paging.Where("[Customer].[CustomerRole] = " + ((int)Role.User));
            _paging.Where("[Customer].[CustomerId] in (" + String.Join(",", _customerIds.Select(x => "'" + x + "'")) + ")");

            _paging.OrderByDesc("RegistrationDateTime");
        }
    }
}