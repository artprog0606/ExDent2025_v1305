using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Services.Bonuses.Model;
using AdvantShop.Core.Services.Bonuses.Model.Enums;
using AdvantShop.Core.SQL;

namespace AdvantShop.Core.Services.Bonuses.Service
{
    public class TransactionService
    {
        public static int Create(Transaction tr, bool executeModules = true)
        {
            var transactionId = SQLDataAccess2.ExecuteScalar<int>("insert into [Bonus].[Transaction] (CardId,Amount,Basis,CreateOn, CreateOnCut,OperationType,Balance, PurchaseId,BonusId,IdempotenceKey )" +
                                                     " values (@CardId, @Amount, @Basis, @CreateOn, @CreateOnCut, @OperationType, @Balance, @PurchaseId, @BonusId, @IdempotenceKey);" +
                                                     " SELECT CAST(SCOPE_IDENTITY() AS INT)", tr);
            if (executeModules)
                ModulesExecuter.CreateTransaction(tr);
            return transactionId;
        }

        public static void RollBack(Transaction item, bool executeModules = true)
        {
            if (
#pragma warning disable CS0612 // Type or member is obsolete
                item.OperationType == EOperationType.AddAdditionBonus
#pragma warning restore CS0612 // Type or member is obsolete
                || item.OperationType == EOperationType.AddBonus)
            {
                if (item.Bonus != null)
                {
                    var bonus = BonusService.RollBack(item.Bonus, item.Amount, false);
                    // по недействующим не пишем транзакцию, чтобы не сбивать с толку, т.к. баланс не изменится
                    if (bonus.Status != EBonusStatus.Removed
                        && (bonus.EndDate is null 
                            || bonus.EndDate >= DateTime.Today))
                    {
                        var transLog = Transaction.Factory(item.CardId, item.Amount, item.Basis, EOperationType.SubtractBonus, BonusService.ActualSum(item.CardId), item.PurchaseId, item.BonusId, null);
                        Create(transLog, executeModules);
                    }
                }
                else
                    //нестандартная ситуация, запасной вариант
                    BonusSystemService.SubtractBonuses(item.CardId, item.Amount, item.Basis, item.PurchaseId, executeModules: executeModules);
            }
            if (
#pragma warning disable CS0612 // Type or member is obsolete
                item.OperationType == EOperationType.SubtractAdditionBonus
#pragma warning restore CS0612 // Type or member is obsolete
                || item.OperationType == EOperationType.SubtractBonus)
            {
                if (item.Bonus != null)
                {
                    var bonus = BonusService.RollBack(item.Bonus, item.Amount, true);
                    // по недействующим не пишем транзакцию, чтобы не сбивать с толку, т.к. баланс не изменится
                    if (bonus.Status != EBonusStatus.Removed
                        && (bonus.EndDate is null 
                            || bonus.EndDate >= DateTime.Today))
                    {
                        var transLog = Transaction.Factory(item.CardId, item.Amount, item.Basis, EOperationType.AddBonus, BonusService.ActualSum(item.CardId), item.PurchaseId, item.BonusId, null);
                        Create(transLog, executeModules);
                    }
                }
                else
                    //нестандартная ситуация, запасной вариант
                    BonusSystemService.AcceptBonuses(item.CardId, item.Amount, item.Basis, string.Empty, null, null,
                        item.PurchaseId, false, executeModules);
            }

            // Поддержка старых начислений
            if (
#pragma warning disable CS0612 // Type or member is obsolete
                item.OperationType == EOperationType.AddMainBonus
#pragma warning restore CS0612 // Type or member is obsolete
                )
            {
                var bonuses = BonusService.Actual(item.CardId)
                                           // присутствует в sql скрипте перехода к единым бонусам
                                          .OrderByDescending(x =>
                                               x.Name == "Основные бонусы"
                                               && x.Description == "Переход к единым баллам (автоматическое действие).")
                                          // сортировка в BonusSystemService.SubtractBonuses
                                          .OrderBy(x => x.EndDate ?? DateTime.MaxValue)
                                          .ToList();
                BonusSystemService.SubtractBonuses(item.CardId, item.Amount, item.Basis, item.PurchaseId, bonuses, executeModules);
            }

            // Поддержка старых начислений
            if (
#pragma warning disable CS0612 // Type or member is obsolete
                item.OperationType == EOperationType.SubtractMainBonus
#pragma warning restore CS0612 // Type or member is obsolete
                )
            {
                BonusSystemService.AcceptBonuses(item.CardId, item.Amount, item.Basis, string.Empty, null, null,
                    item.PurchaseId, false, executeModules);
            }

        }

        public static List<Transaction> GetByPurchase(int id)
        {
            return SQLDataAccess2.ExecuteReadIEnumerable<Transaction>("Select * from [Bonus].[Transaction] where PurchaseId=@p", new { p = id }).ToList();
        }

        public static List<Transaction> GetLast(Guid cardId, int top = 10)
        {
            return SQLDataAccess2.ExecuteReadIEnumerable<Transaction>("Select top(@top) * from [Bonus].[Transaction] where CardId=@p order by Id DESC", new { p = cardId, top = top }).ToList();
        }

        public static List<UnitedTransaction> GetLastUnitedByDateAndType(Guid cardId, int top = 100)
        {
            return SQLDataAccess2.ExecuteReadIEnumerable<UnitedTransaction>(
                "SELECT TOP(@top) Basis, SUM(Amount) AS Amount, CreateOn, OperationType" +
                " FROM [Bonus].[Transaction]" +
                " WHERE CardId = @p" +
                " GROUP BY Basis, CreateOn, OperationType" +
                " ORDER BY CreateOn DESC", new
                {
                    p = cardId,
                    top = top
                }).ToList();
        }

        public static void DeleteByCard(Guid cardId)
        {
            SQLDataAccess2.ExecuteNonQuery("Delete from [Bonus].[Transaction] where CardId=@cardId", new { cardId = cardId });
        }

        public static bool ExistsByIdempotenceKey(string idempotenceKey)
        {
            return SQLDataAccess2.ExecuteScalar<int>(
                @"IF EXISTS (SELECT * FROM [Bonus].[Transaction] WHERE [IdempotenceKey] = @IdempotenceKey)
                    SELECT 1
                    ELSE
                    SELECT 0",
                new {IdempotenceKey = idempotenceKey}) == 1;
        }
        
        public static bool ExistsByIdempotenceKey(Guid cardId, string idempotenceKey)
        {
            return SQLDataAccess2.ExecuteScalar<int>(
                @"IF EXISTS (SELECT * FROM [Bonus].[Transaction] WHERE CardId=@cardId AND [IdempotenceKey] = @IdempotenceKey)
                    SELECT 1
                    ELSE
                    SELECT 0",
                new {IdempotenceKey = idempotenceKey, cardId = cardId}) == 1;
        }
    }
}
