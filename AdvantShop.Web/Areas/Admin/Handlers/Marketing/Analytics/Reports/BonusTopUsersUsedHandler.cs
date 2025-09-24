using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.SQL;

namespace AdvantShop.Web.Admin.Handlers.Marketing.Analytics.Reports
{
    public class BonusTopUsersUsedItemModel
    {
        public Guid CardId { get; set; }
        public Guid CustomerId { get; set; }
        public string Name { get; set; }
        public float UsedNumber { get; set; }
        public int OrdersCount { get; set; }
        public string Used =>
            UsedNumber != 0
                ? PriceFormatService.FormatPrice(UsedNumber < 0 ? 0 : UsedNumber)
                : UsedNumber.FormatPrice();
    }
    
    public class BonusTopUsersUsedHandler
    {
        private readonly DateTime _dateFrom;
        private readonly DateTime _dateTo;

        public BonusTopUsersUsedHandler(DateTime dateFrom, DateTime dateTo)
        {
            _dateFrom = dateFrom;
            _dateTo = dateTo;
        }
        
        public List<BonusTopUsersUsedItemModel> Execute()
        {
            var users = GetBonusTopUsersUsed(_dateFrom, _dateTo);

            return users;
        }
        
        private List<BonusTopUsersUsedItemModel> GetBonusTopUsersUsed(DateTime dateFrom, DateTime dateTo)
        {
            var query =
                @"SELECT TOP (10)
                    c.CardId, cus.CustomerID AS CustomerId
                    ,cus.FirstName + ' ' + cus.LastName AS Name
                    ,t.AmountSum AS UsedNumber
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
                        (OperationType = 2 OR OperationType = 4 OR OperationType = 6 OR OperationType = 8)
                        AND (CreateOn >= @dateFrom)
                        AND (CreateOn <= @dateTo)
                    GROUP BY CardId
                ) AS t ON t.CardId = c.CardId
                LEFT JOIN Customers.Customer AS cus ON cus.CustomerId = c.CardId
                ORDER BY UsedNumber DESC";

            return SQLDataAccess2.ExecuteReadIEnumerable<BonusTopUsersUsedItemModel>(query, new { dateFrom, dateTo }).ToList();
        }
    }
}