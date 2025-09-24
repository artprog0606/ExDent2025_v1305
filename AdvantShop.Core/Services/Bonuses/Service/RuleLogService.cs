using AdvantShop.Core.Services.Bonuses.Model;
using AdvantShop.Core.Services.Bonuses.Model.Enums;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;
using System;
using System.Data;
using System.Data.SqlClient;

namespace AdvantShop.Core.Services.Bonuses.Service
{
    public class RuleLogService
    {
        public static void Add(RuleLog model)
        {
            SQLDataAccess2.ExecuteNonQuery("insert into Bonus.RuleLog (CardId, RuleType, Created, ObjectType, ObjectId)" +
                                           " values (@CardId, @RuleType, @Created, @ObjectType, @ObjectId);"
                , model);
        }
        public static bool IsHasBonusReview(Guid id, int? objId, ERule type, EBonusRuleObjectType objType)
        {
            return SQLDataHelper.GetBoolean(SQLDataAccess.ExecuteScalar(
                "Select 1 " +
                "from Bonus.RuleLog where CardId=@id and RuleType=@type and ObjectType=@objType and ObjectId=@objId",
                CommandType.Text,
                new SqlParameter("@id", id),
                new SqlParameter("type", type),
                new SqlParameter("@objType", objType),
                new SqlParameter("@objId", objId ?? 0)));
        }
    }
}