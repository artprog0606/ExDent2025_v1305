using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.SQL;

namespace AdvantShop.Web.Admin.Handlers.Marketing.Analytics.Reports
{
    public class ManagerStatisticData
    {
        public Guid CustomerId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public int OrdersCount { get; set; }
        public float OrdersSum { get; set; }
        public int PaidOrdersCount { get; set; }
        public float PaidOrdersSum { get; set; }
    }

    public class ManagersHandler
    {
        private readonly DateTime _dateFrom;
        private readonly DateTime _dateTo;

        public ManagersHandler(DateTime dateFrom, DateTime dateTo)
        {
            _dateFrom = dateFrom;
            _dateTo = dateTo;
        }
        
        public List<ManagerStatisticData> Execute()
        {
            return SQLDataAccess.Query<ManagerStatisticData>(
                //Email, Name, ID
                "SELECT Email, (Lastname + ' ' + Firstname) AS Name, Customer.CustomerId, " +
                //общее кол-во заказов
                "(SELECT COUNT(orderId) FROM [Order].[Order] " +
                "WHERE ManagerId=[Managers].ManagerId AND OrderDate >= @DateFrom AND OrderDate <= @DateTo) AS OrdersCount, " +
                //общая сумма заказов
                "(SELECT Sum(SUM) FROM [Order].[Order] " +
                "WHERE ManagerId=[Managers].ManagerId AND OrderDate >= @DateFrom AND OrderDate <= @DateTo) AS OrdersSum, " +

                //кол-во оплаченных заказов
                "(SELECT COUNT(orderId) FROM [Order].[Order] " +
                "WHERE ManagerId=[Managers].ManagerId AND PaymentDate IS NOT NULL AND OrderDate >= @DateFrom AND OrderDate <= @DateTo) AS PaidOrdersCount, " +
                //сумма оплаченных заказов
                "(SELECT Sum(SUM) FROM [Order].[Order] " +
                "WHERE ManagerId=[Managers].ManagerId AND PaymentDate IS NOT NULL AND OrderDate >= @DateFrom AND OrderDate <= @DateTo) AS PaidOrdersSum " +
                
                "FROM [Customers].[Customer] " +
                "INNER JOIN [Customers].[Managers] ON [Customer].CustomerID = [Managers].[CustomerId] " +
                "ORDER BY Name",
                new {DateFrom = _dateFrom, DateTo = _dateTo}).ToList();
        }
    }
}