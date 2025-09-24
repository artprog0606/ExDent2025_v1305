using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.SQL;

namespace AdvantShop.Web.Admin.Handlers.Marketing.Analytics.Reports
{
    public class BonusRulesItemModel
    {
        public string Name { get; set; }
        public float AccruedNumber { get; set; }

        public string Accrued =>
            AccruedNumber != 0
                ? PriceFormatService.FormatPrice(AccruedNumber < 0 ? 0 : AccruedNumber)
                : AccruedNumber.FormatPrice();
    }
    
    public class BonusRulesHandler
    {
        private readonly DateTime _dateFrom;
        private readonly DateTime _dateTo;

        public BonusRulesHandler(DateTime dateFrom, DateTime dateTo)
        {
            _dateFrom = dateFrom;
            _dateTo = dateTo;
        }

        public List<BonusRulesItemModel> Execute()
        {
            var items = GetBonusRulesByDate(_dateFrom, _dateTo);

            return items;
        }

        private List<BonusRulesItemModel> GetBonusRulesByDate(DateTime dateFrom, DateTime dateTo)
        {
            var query = 
                @"SELECT
                    cr.Name AS Name,
                    SUM(t.Amount) AS AccruedNumber
                FROM [Bonus].[CustomRule] AS cr
                LEFT JOIN [Bonus].[Transaction] AS t ON t.Basis = cr.Name
                    AND t.OperationType % 2 = 1
                    AND t.CreateOn >= @dateFrom
                    AND t.CreateOn <= @dateTo
                GROUP BY cr.Name";
            
            return SQLDataAccess2.ExecuteReadIEnumerable<BonusRulesItemModel>(query, new { dateFrom, dateTo }).ToList();
        }
    }
}