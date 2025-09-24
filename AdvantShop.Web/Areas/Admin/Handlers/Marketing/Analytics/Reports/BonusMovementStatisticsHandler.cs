using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.SQL;
using AdvantShop.Web.Admin.Models.Shared.Common;

namespace AdvantShop.Web.Admin.Handlers.Marketing.Analytics.Reports
{
    public class BonusMovementStatisticsModel
    {
        public string SumAccrued { get; set; }
        public string SumUsed { get; set; }
        public int CountOrdersUsedBonus { get; set; }
        public int PercentOrdersUsedBonus { get; set; }
        public ChartDataJsonModel AccruedAndUsedByDate { get; set; }
    }
    
    public class BonusMovementStatisticsHandler
    {
        private readonly DateTime _dateFrom;
        private readonly DateTime _dateTo;

        public BonusMovementStatisticsHandler(DateTime dateFrom, DateTime dateTo)
        {
            _dateFrom = dateFrom;
            _dateTo = dateTo;
        }

        public BonusMovementStatisticsModel Execute()
        {
            var model = new BonusMovementStatisticsModel()
            {
                SumAccrued = PriceFormatService.FormatPrice(
                    GetSumBonusesByDateAndType(_dateFrom, _dateTo, true)),
                SumUsed = PriceFormatService.FormatPrice(
                    GetSumBonusesByDateAndType(_dateFrom, _dateTo, false)),
                CountOrdersUsedBonus = GetOrdersWithBonusCountByDate(_dateFrom, _dateTo),
                AccruedAndUsedByDate = GetAccruedAndUsedByDate(_dateFrom, _dateTo)
            };

            var ordersCount = GetOrdersCountByDate(_dateFrom, _dateTo);
            
            model.PercentOrdersUsedBonus = ordersCount == 0 
                ?  0
                : (int)Math.Round((decimal)model.CountOrdersUsedBonus / ordersCount * 100);
            
            return model;
        }

        private ChartDataJsonModel GetAccruedAndUsedByDate(DateTime dateFrom, DateTime dateTo)
        {
            var accruedData = GetMovementByDate(dateFrom, dateTo, true);
            var usedData = GetMovementByDate(dateFrom, dateTo, false);
            
            return new ChartDataJsonModel()
            {
                Data = new List<object>()
                {
                    accruedData.Values.Select(x => x),
                    usedData.Values.Select(x => x),
                },
                Labels = accruedData.Keys.ToList(),
                Colors = new List<string>()
                {
                    "#2E9DEC",
                    "#D9534F"
                }
            };
        }
        
        private float GetSumBonusesByDateAndType(DateTime dateFrom, DateTime dateTo, bool isAdd)
        {
            var query = 
                $@"SELECT SUM(Amount)
                FROM [Bonus].[Transaction]
                WHERE OperationType % 2 = {(isAdd ? 1 : 0)}
                AND CreateOn >= @dateFrom
                AND CreateOn <= @dateTo";
            return SQLDataAccess2.ExecuteScalar<float>(query, new { dateFrom, dateTo });
        }

        private Dictionary<string, float> GetMovementByDate(DateTime dateFrom, DateTime dateTo, bool isAdd)
        {
            var query = 
                $@"DECLARE @StartDate DATE = CAST(@DateFrom as Date);
                DECLARE @EndDate DATE = CAST(@DateTo as Date);

                WITH DateList AS (
                    SELECT @StartDate AS Date
                    UNION ALL
                    SELECT DATEADD(DAY, 1, Date)
                    FROM DateList
                    WHERE DATEADD(DAY, 1, Date) <= @EndDate
                )
                SELECT
                    CONVERT(varchar(10), Date, 120) as ResourceKey,
                    SUM(t.Amount) AS ResourceValue
                FROM DateList
                LEFT JOIN (
                    SELECT
                        Amount,
                        CreateOn
                    FROM [Bonus].[Transaction]
                    WHERE OperationType % 2 = {(isAdd ? 1 : 0)}
                ) AS t ON CAST(t.CreateOn AS Date) = Date
                GROUP BY Date
                OPTION (MAXRECURSION 0);";
            
            return SQLDataAccess.ExecuteReadDictionary<string, float>(
                query, 
                CommandType.Text, 
                "ResourceKey", 
                "ResourceValue", 
                new SqlParameter("@DateFrom", dateFrom),
                new SqlParameter("@DateTo", dateTo));
        }
        
        private int GetOrdersCountByDate(DateTime dateFrom, DateTime dateTo)
        {
            var query = 
                @"SELECT COUNT(*)
                FROM [Order].[Order]
                WHERE OrderDate >= @DateFrom
                AND OrderDate <= @DateTo";
            
            return SQLDataAccess.ExecuteScalar<int>(
                query,
                CommandType.Text,
                new SqlParameter("@DateFrom", dateFrom),
                new SqlParameter("@DateTo", dateTo));
        }

        private int GetOrdersWithBonusCountByDate(DateTime dateFrom, DateTime dateTo)
        {
            var query = 
                @"SELECT COUNT(*)
                FROM [Order].[Order]
                WHERE BonusCost > 0
                AND OrderDate >= @DateFrom
                AND OrderDate <= @DateTo";
            
            return SQLDataAccess.ExecuteScalar<int>(
                query,
                CommandType.Text,
                new SqlParameter("@DateFrom", dateFrom),
                new SqlParameter("@DateTo", dateTo));
        }
    }
}