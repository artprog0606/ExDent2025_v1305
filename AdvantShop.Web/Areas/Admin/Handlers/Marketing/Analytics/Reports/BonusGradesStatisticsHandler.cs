using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AdvantShop.Core.SQL;
using AdvantShop.Web.Admin.Models.Shared.Common;

namespace AdvantShop.Web.Admin.Handlers.Marketing.Analytics.Reports
{
    public class BonusGradesStatisticsHandler
    {
        public ChartDataJsonModel Execute()
        {
            var data = GetCountBonusCardGrades();

            return new ChartDataJsonModel()
            {
                Data = data.Values.Cast<object>().ToList(),
                Labels = data.Keys.ToList(),
                Colors = CreateColors(data.Count),
            };
        }

        private List<string> CreateColors(int count)
        {
            var colors = new List<string>();
            var random = new Random();

            for (var i = 0; i < count; i++)
            {
                var randomNumber = random.Next(0, 180);
                colors.Add($"#{randomNumber:X2}{randomNumber:X2}{255:X2}");
            }
            
            return colors;
        }
        
        private Dictionary<string, float> GetCountBonusCardGrades()
        {
            var query = 
                @"SELECT Name AS ResourceKey, COUNT(c.CardId) AS ResourceValue
                FROM Bonus.Grade AS g
                JOIN Bonus.Card AS c ON c.GradeId = g.Id
                GROUP BY g.Name";

            return SQLDataAccess.ExecuteReadDictionary<string, float>(
                query, 
                CommandType.Text, 
                "ResourceKey",
                "ResourceValue");
        }
    }
}