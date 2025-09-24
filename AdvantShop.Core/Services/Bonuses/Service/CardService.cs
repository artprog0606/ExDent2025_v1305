﻿using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Bonuses.Model;
using AdvantShop.Core.SQL;
using AdvantShop.Customers;
using System.Data.SqlClient;
using AdvantShop.Core.Services.Bonuses.Model.Enums;
using AdvantShop.Core.Services.Bonuses.Model.Rules;
using AdvantShop.Core.Modules;

namespace AdvantShop.Core.Services.Bonuses.Service
{
    public class CardService
    {
        public static List<Card> Gets()
        {
            return SQLDataAccess2.ExecuteReadIEnumerable<Card>("Select * from Bonus.Card").ToList();
        }

        public static Card Get(Guid id)
        {
            Card card;
            var cacheKey = CacheNames.BonusCard + id;

            if (!CacheManager.TryGetValue(cacheKey, out card))
            {
                card = SQLDataAccess2.Query<Card>("Select * from Bonus.Card where CardId=@id", new { id = id });
                CacheManager.Insert(cacheKey, card ?? new Card(), 2);
            }

            return card != null && card.CardNumber != 0 ? card : null;
        }

        public static Card Get(long cardNumber)
        {
            return SQLDataAccess2.Query<Card>("Select * from Bonus.Card where CardNumber=@id", new { id = cardNumber });
        }

        public static long Add(Card model, bool updateModules = true)
        {
            SQLDataAccess2.ExecuteNonQuery("insert into Bonus.Card (CardId,CardNumber,Blocked,GradeId,CreateOn,ManualGrade)" +
                                                         " values (@CardId, @CardNumber, @Blocked, @GradeId,@CreateOn,@ManualGrade)", model);

            var customer = CustomerService.GetCustomer(model.CardId);
            if (customer != null && customer.BonusCardNumber != model.CardNumber)
            {
                customer.BonusCardNumber = model.CardNumber;

                SQLDataAccess.ExecuteNonQuery("update Customers.Customer set BonusCardNumber = @BonusCardNumber where CustomerID = @CustomerID",
                    System.Data.CommandType.Text, new SqlParameter("@BonusCardNumber", customer.BonusCardNumber), new SqlParameter("@CustomerID", customer.Id));

            }

            CacheManager.RemoveByPattern(CacheNames.BonusCard + model.CardId);
            
            new NewCardRule().Execute(model.CardId);
            
            var newgrade = GradeService.Get(model.GradeId);
            var bonusHistory = new PersentHistory
            {
                GradeName = newgrade.Name,
                BonusPersent = newgrade.BonusPercent,
                CardId = model.CardId,
                CreateOn = DateTime.Now,
                ByAction = EHistoryAction.HandChangeUI
            };
            PersentHistoryService.Add(bonusHistory);
            
            if (updateModules)
                ModulesExecuter.BonusCardAdded(model);
            
            return model.CardNumber;
        }

        public static void Update(Card model, bool updateModules = true)
        {
            SQLDataAccess2.ExecuteNonQuery("Update Bonus.Card set CardNumber=@CardNumber,Blocked=@Blocked,GradeId=@GradeId,ManualGrade=@ManualGrade  where CardId=@CardId", model);
            
            var customer = CustomerService.GetCustomer(model.CardId);
            if (customer != null && customer.BonusCardNumber != model.CardNumber)
            {
                customer.BonusCardNumber = model.CardNumber;
                CustomerService.UpdateCustomer(customer);
            }
            if (updateModules)
                ModulesExecuter.BonusCardUpdated(model);

            CacheManager.RemoveByPattern(CacheNames.BonusCard + model.CardId);
        }

        public static void AddHistory(PersentHistory model)
        {
            SQLDataAccess2.ExecuteNonQuery("Insert into Bonus.PersentHistory (CardId,GradeName,BonusPersent,CreatOn,ByAction)" +
                                           " values (@CardId,@GradeName,@BonusPersent,@CreatOn,@ByAction);select cast(scope_identity() as int)", model);
        }

        public static List<PersentHistory> GetHistory(Guid cardId)
        {
            return SQLDataAccess2.ExecuteReadIEnumerable<PersentHistory>("Select * from Bonus.Card where CardId=@id", new { id = cardId }).ToList();
        }
        public static Card GetByPhone(string phone)
        {
            var temp = phone.TryParseLong();
            return
                SQLDataAccess2.Query<Card>(
                    "Select card.* From Bonus.Card card Inner Join Customers.Customer cus on card.CardId=cus.CustomerId Where cus.Phone = @phone",
                    new { phone = temp });
        }
        
        public static void Delete(Guid cardId)
        {
            TransactionService.DeleteByCard(cardId);
            BonusService.DeleteByCard(cardId);
            PersentHistoryService.DeleteByCard(cardId);
            PurchaseService.DeleteByCard(cardId);
            ModulesExecuter.BonusCardDeleted(cardId);
            SQLDataAccess2.ExecuteNonQuery("delete from [Bonus].[Card] where CardId=@id", new { id = cardId });

            var customer = CustomerService.GetCustomer(cardId);
            if (customer != null)
            {
                customer.BonusCardNumber = null;
                CustomerService.UpdateCustomer(customer);
            }

            CacheManager.RemoveByPattern(CacheNames.BonusCard + cardId);
        }

        public static List<CardExportModel> GetExportCards()
        {
            string sql =
                "Select [Card].cardid,[Card].CardNumber,[customer].phone,[customer].email,[customer].FirstName,[customer].LastName,[customer].LastName,[customer].Patronymic,[customer].BirthDay,[grade].name as GradeName," +
                "SUM(CASE WHEN ([Bonuses].EndDate is null or [Bonuses].EndDate>=GETDATE()) and ([Bonuses].StartDate is null or [Bonuses].StartDate<=GETDATE()) and [Bonuses].Amount > 0 and [Bonuses].[Status] <> 1 THEN [Bonuses].Amount ELSE 0 END) as BonusAmount " +
                "from[bonus].[Card] left join [bonus].Bonuses on [Card].CardId = Bonuses.CardId inner join[customers].customer on[card].cardid = customer.customerid inner join[bonus].grade on[card].gradeid = [grade].id " +
                "group by[Card].cardid,[Card].CardNumber,[customer].phone,[customer].email,[customer].FirstName,[customer].LastName,[customer].LastName,[customer].Patronymic,[customer].BirthDay,[grade].name";
            return SQLDataAccess2.ExecuteReadIEnumerable<CardExportModel>(sql).ToList();
        }
    }
}