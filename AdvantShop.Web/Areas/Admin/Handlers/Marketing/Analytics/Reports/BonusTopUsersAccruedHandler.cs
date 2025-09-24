using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.SQL;

namespace AdvantShop.Web.Admin.Handlers.Marketing.Analytics.Reports
{
    public class BonusTopUsersAccruedItemModel
    {
        public Guid CardId { get; set; }
        public Guid CustomerId { get; set; }
        public string Name { get; set; }
        public float AccruedNumber { get; set; }
        public int OrdersCount { get; set; }
        public string Accrued =>
            AccruedNumber != 0
                ? PriceFormatService.FormatPrice(AccruedNumber < 0 ? 0 : AccruedNumber)
                : AccruedNumber.FormatPrice();
    }
    
    public class BonusTopUsersAccruedHandler
    {
        private readonly DateTime _dateFrom;
        private readonly DateTime _dateTo;

        public BonusTopUsersAccruedHandler(DateTime dateFrom, DateTime dateTo)
        {
            _dateFrom = dateFrom;
            _dateTo = dateTo;
        }

        public List<BonusTopUsersAccruedItemModel> Execute()
        {
            var users = GetBonusTopUsersAccrued(_dateFrom, _dateTo);

            return users;
        }

        private List<BonusTopUsersAccruedItemModel> GetBonusTopUsersAccrued(DateTime dateFrom, DateTime dateTo)
        {
            var query =
                @"SELECT TOP (10)
                    c.CardId, cus.CustomerID AS CustomerId
                    ,cus.FirstName + ' ' + cus.LastName AS Name
                    ,t.AmountSum AS AccruedNumber
                    ,(SELECT
                        COUNT(o.OrderID)
                FROM [Order].OrderCustomer AS ocus
                INNER JOIN
                    [Order].[Order] AS o ON o.OrderID = ocus.OrderId
                        AND o.OrderDate >= @dateFrom
                        AND o.OrderDate <= @dateTo
                    WHERE ocus.CustomerID = cus.CustomerID
                ) AS OrdersCount
                FROM Bonus.Card AS c
                LEFT JOIN (
                    SELECT
                        SUM(Amount) AS AmountSum, CardId
                    FROM Bonus.[Transaction]
                    WHERE
                        (OperationType = 1 OR OperationType = 3 OR OperationType = 5 OR OperationType = 7)
                        AND (CreateOn >= @dateFrom)
                        AND (CreateOn <= @dateTo)
                    GROUP BY CardId
                ) AS t ON t.CardId = c.CardId
                LEFT JOIN Customers.Customer AS cus ON cus.CustomerId = c.CardId
                ORDER BY AccruedNumber DESC";

            return SQLDataAccess2.ExecuteReadIEnumerable<BonusTopUsersAccruedItemModel>(query, new { dateFrom, dateTo }).ToList();
        }
    }
}