using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Core.SQL;
using AdvantShop.Web.Admin.Models.Shared.Common;

namespace AdvantShop.Web.Admin.Handlers.Marketing.Analytics.Reports
{
    public class BonusParticipantsStatisticsModel
    {
        public ParticipantsCounts ParticipantsCounts { get; set; }
        public ChartDataJsonModel CountOfParticipantsByDate { get; set; }
    }

    public class ParticipantsCounts
    {
        public int All { get; set; }
        public int New { get; set; }
        public int PercentageOfNew { get; set; }
        public int HavingMovement { get; set; }
        public int PercentageOfHavingMovement { get; set; }
    }
    
    public class BonusParticipantsStatisticsHandler
    {
        private readonly DateTime _dateFrom;
        private readonly DateTime _dateTo;

        public BonusParticipantsStatisticsHandler(DateTime dateFrom, DateTime dateTo)
        {
            _dateFrom = dateFrom;
            _dateTo = dateTo;
        }

        public BonusParticipantsStatisticsModel Execute()
        {
            var model = new BonusParticipantsStatisticsModel()
            {
                ParticipantsCounts = new ParticipantsCounts()
                {
                    All = GetCount(),
                    New = GetCountByDate(_dateFrom, _dateTo),
                    HavingMovement = GetActiveCount(_dateFrom, _dateTo)
                },
                CountOfParticipantsByDate = GetCountOfParticipantsByDate()
            };

            if (model.ParticipantsCounts?.All == 0 || model.ParticipantsCounts?.All == null) return model;
            if (model.ParticipantsCounts?.New != null)
            {
                model.ParticipantsCounts.PercentageOfNew =
                    (int)Math.Round((decimal)model.ParticipantsCounts.New / model.ParticipantsCounts.All * 100);
            }
            if (model.ParticipantsCounts?.HavingMovement != null)
            {
                model.ParticipantsCounts.PercentageOfHavingMovement =
                    (int)Math.Round((decimal)model.ParticipantsCounts.HavingMovement / model.ParticipantsCounts.All * 100);
            }

            return model;
        }

        private ChartDataJsonModel GetCountOfParticipantsByDate()
        {
            var data = CountOfParticipantsByDate();
            
            return new ChartDataJsonModel()
            {
                Data = new List<object>()
                {
                    data.Values.Select(x => x),
                },
                Labels = data.Keys.ToList()
            };
        }
        
        private int GetCount()
        {
            return SQLDataAccess2.ExecuteScalar<int>("SELECT Count(*) FROM [Bonus].[Card]");
        }

        private int GetCountByDate(DateTime dateFrom, DateTime dateTo)
        {
            return SQLDataAccess2.ExecuteScalar<int>("SELECT Count(*) FROM [Bonus].[Card] WHERE CreateOn >= @dateFrom and CreateOn <= @dateTo", new {dateFrom, dateTo});
        }
        
        private int GetActiveCount(DateTime dateFrom, DateTime dateTo)
        {
            var query = 
                @"SELECT COUNT(*)
                FROM [Bonus].[Card] as c
                WHERE EXISTS (
                    Select * 
                    FROM [Bonus].[Purchase] as p
                    WHERE c.CardId = p.CardId
                        and p.CreateOn >= @dateFrom
                        and p.CreateOn <= @dateTo)";
            
            return SQLDataAccess2.ExecuteScalar<int>(query, new { dateFrom, dateTo });
        }

        private Dictionary<string, float> CountOfParticipantsByDate()
        {
            var query = 
                @"DECLARE @count DATETIME = DATEADD(MM,DATEDIFF(MM, 1, DATEADD(MONTH, 1, @StartDate)),0);
                DECLARE @DateList TABLE(DATETIME [Date]);

                IF (@StartDate <> @count)
                BEGIN
	                INSERT 
	                INTO @DateList ([Datetime])
	                VALUES (@StartDate)
                END

                WHILE @count <= @EndDate
                BEGIN
                    INSERT 
                    INTO @DateList ([Datetime])
                    VALUES (@count)
                    SET @count  = DATEADD(MM,DATEDIFF(MM, 1, DATEADD(MONTH, 1, @count)),0)
                END

                IF(@EndDate <> (SELECT MAX([Datetime])FROM @DateList))
                BEGIN
	                INSERT 
	                INTO @DateList ([Datetime])
	                VALUES (@EndDate)
                END

                SELECT
	                CONVERT(varchar(10), [Datetime], 104) as ResourceKey,
	                (
		                SELECT COUNT(CardID)
		                FROM Bonus.Card
		                WHERE CONVERT(date, Card.CreateOn, 104) <= CONVERT(date, [Datetime], 104)
	                ) as ResourceValue
                FROM
	                @DateList
                GROUP BY 
	                [Datetime]";
            
            return SQLDataAccess.ExecuteReadDictionary<string, float>(
                query, 
                CommandType.Text,
                "ResourceKey", "ResourceValue",
                new SqlParameter("@StartDate", _dateFrom),
                new SqlParameter("@EndDate", _dateTo));
        }
    }
}