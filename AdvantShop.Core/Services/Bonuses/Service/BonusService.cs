using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Bonuses.Model;
using AdvantShop.Core.Services.Bonuses.Model.Enums;
using AdvantShop.Core.SQL;

namespace AdvantShop.Core.Services.Bonuses.Service
{
    public class BonusService
    {
        public static Bonus Get(int id)
        {
            return SQLDataAccess2.Query<Bonus>("Select * from Bonus.Bonuses where Id=@id", new { id = id });
        }

        public static int Add(Bonus model)
        {
            return SQLDataAccess2.ExecuteScalar<int>("insert into Bonus.Bonuses (CardId, Name, Amount, StartDate, EndDate, Description, Status, [NotifiedAboutExpiry], [CreateOn])" +
                                                     " values (@CardId, @Name, @Amount, @StartDate, @EndDate, @Description, @Status, 0, @CreateOn);" +
                                                     " select cast(scope_identity() as int)", model);
        }

        public static void Update(Bonus model)
        {
            SQLDataAccess2.ExecuteNonQuery("Update Bonus.Bonuses set CardId=@CardId, " +
                                           "Name=@Name, " +
                                           "Amount=@Amount, " +
                                           "StartDate=@StartDate, " +
                                           "EndDate=@EndDate, " +
                                           "Description=@Description, " +
                                           "NotifiedAboutExpiry=@NotifiedAboutExpiry, " +
                                           "Status=@Status where Id=@Id", model);
        }

        public static decimal ActualSum(Guid cardId)
        {
            var temp = SQLDataAccess2.ExecuteScalar<decimal>("Select Sum(Amount) from Bonus.Bonuses where CardId=@cardId" +
                                                             " and (EndDate is null or EndDate>=@end)" +
                                                             " and (StartDate is null or StartDate<=@start)" +
                                                             " and Amount > 0" +
                                                             " and Status <> @status", new
                                                             {
                                                                 cardId = cardId,
                                                                 start = DateTime.Today,
                                                                 end = DateTime.Today,
                                                                 status = (int)EBonusStatus.Removed
                                                             });
            return temp;
        }

        public static List<Bonus> Actual(Guid cardId)
        {
            var temp = SQLDataAccess2.ExecuteReadIEnumerable<Bonus>(
                "Select * from Bonus.Bonuses where CardId=@cardId" +
                " and (EndDate is null or EndDate>=@end)" +
                " and (StartDate is null or StartDate<=@start)" +
                " and Amount > 0" +
                " and Status <> @status", new
                {
                    cardId = cardId,
                    start = DateTime.Today,
                    end = DateTime.Today,
                    status = (int) EBonusStatus.Removed
                }).ToList();
            return temp;
        }
        
        public static List<Bonus> GetAll(Guid cardId)
        {
            var temp = SQLDataAccess2.ExecuteReadIEnumerable<Bonus>(
                "Select * from Bonus.Bonuses where CardId=@cardId" +
                " and (EndDate is null or EndDate>=@end)" +
                " and Amount > 0" +
                " and Status <> @status", new
                {
                    cardId = cardId,
                    end = DateTime.Today,
                    status = (int) EBonusStatus.Removed
                }).ToList();
            return temp;
        }

        public static List<Bonus> GetAllTemporary(Guid cardId)
        {
            var temp = SQLDataAccess2.ExecuteReadIEnumerable<Bonus>(
                "SELECT * FROM Bonus.Bonuses" +
                " WHERE CardId = @cardId" +
                " AND EndDate >= @end" +
                " AND Amount > 0" +
                " AND Status <> @status", new
                {
                    cardId = cardId,
                    end = DateTime.Today,
                    status = (int)EBonusStatus.Removed
                }).ToList();
            return temp;
        }

        public static Bonus RollBack(Bonus bonus, decimal amount, bool acceptBonuses)
        {
            if (acceptBonuses)
                bonus.Amount += amount;
            else
                bonus.Amount -= amount;

            if (bonus.Amount < 0m)
                bonus.Amount = 0m;

            if (bonus.Status != EBonusStatus.Removed)
                bonus.Status =
                    bonus.Amount == 0m
                        ? EBonusStatus.Zero
                        : EBonusStatus.Substract;
            
            Update(bonus);
            return bonus;
        }

        public static void DeleteByCard(Guid cardId)
        {
             SQLDataAccess2.ExecuteNonQuery("Delete from Bonus.Bonuses where CardId=@cardId", new { cardId = cardId });
        }
    }
}
