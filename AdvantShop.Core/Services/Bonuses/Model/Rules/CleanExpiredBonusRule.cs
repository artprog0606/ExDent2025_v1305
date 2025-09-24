using System;
using System.Linq;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Bonuses.Model.Enums;
using AdvantShop.Core.Services.Bonuses.Notification;
using AdvantShop.Core.Services.Bonuses.Notification.Template;
using AdvantShop.Core.Services.Bonuses.Service;
using AdvantShop.Core.SQL;
using Quartz;

namespace AdvantShop.Core.Services.Bonuses.Model.Rules
{
    [DisallowConcurrentExecution]
    public class CleanExpiredBonusRule : BaseRule
    {
        public int DayBefore { get; set; }
        public bool NeedSms { get; set; }

        public override void Process(IJobExecutionContext context)
        {
            var cerule = CustomRuleService.Get(ERule.CleanExpiredBonus);
            if (cerule == null || !cerule.Enabled)
                return;

            var rule = BaseRule.Get(cerule) as CleanExpiredBonusRule;
            if (rule == null)
                return;

            DayBefore = rule.DayBefore;
            NeedSms = rule.NeedSms;
            
            Notification();
            ProcessBonuses(cerule.Name);
        }

        public void Notification()
        {
            if (!NeedSms) return;
            var curDate = DateTime.Today.AddDays(DayBefore);
            var result =
                SQLDataAccess2.ExecuteReadIEnumerable<CleanExpiredBonusRuleDto>(
                    "select ad.Id as BonusId, ad.Amount, card.CardId, customer.StandardPhone " +
                    "from Bonus.Bonuses ad " +
                    "   inner join Bonus.card card on card.CardId=ad.CardId " +
                    "   inner join Customers.Customer customer on card.CardId=customer.CustomerID " +
                    "where EndDate is not null and EndDate <  @date and Status <> @status " +
                    "   and ad.NotifiedAboutExpiry <> 1 " + 
                    "ORDER BY card.CardId",
                    new {date = curDate, status = EBonusStatus.Removed, rule = ERule.CleanExpiredBonus })
                    .ToList();

            var companyname = SettingsMain.ShopName;
            Guid lastCartId = default;
            decimal cleanBonusesCart = 0m;
            
            foreach (var item in result)
            {
                if (lastCartId.IsDefault())
                {
                    lastCartId = item.CardId;
                    cleanBonusesCart = 0m;
                }
                else if (lastCartId != item.CardId)
                {
                    NotificationCard(lastCartId, companyname, cleanBonusesCart);
                
                    lastCartId = item.CardId;
                    cleanBonusesCart = 0m;
                }

                SQLDataAccess2.ExecuteNonQuery("Update Bonus.Bonuses set NotifiedAboutExpiry=1 where Id=@BonusId", item);
                // var additionBonus = BonusService.Get(item.BonusId);
                // additionBonus.NotifiedAboutExpiry = true;
                // BonusService.Update(additionBonus);

                cleanBonusesCart += item.Amount;
            }

            // т.к. по последней карте не выполнится в теле цикла
            if (lastCartId != default)
                NotificationCard(lastCartId, companyname, cleanBonusesCart);
        }

        private void NotificationCard(Guid cardId, string companyname, decimal cleanBonuses)
        {
            if (cleanBonuses <= 0m)
                return;
            
            var balance = BonusService.ActualSum(cardId);
            NotificationService.Process(cardId, ENotifcationType.OnCleanExpiredBonus,
                new OnCleanExpiredBonusTempalte
                {
                    CompanyName = companyname,
                    Balance = balance,
                    CleanBonuses = cleanBonuses,
                    DayLeft = DayBefore
                });

            // так понимаю лог тут, как признак уведомления
            RuleLogService.Add(new RuleLog
            {
                CardId = cardId,
                RuleType = ERule.CleanExpiredBonus,
                Created = DateTime.Today,
                ObjectType = EBonusRuleObjectType.None,
                ObjectId = string.Empty
            });
        }

        public void ProcessBonuses(string basis)
        {
            var curDate = DateTime.Today;
            var result =
                SQLDataAccess2.ExecuteReadIEnumerable<CleanExpiredBonusRuleDto>(
                    "select ad.Id as BonusId, card.CardId, customer.StandardPhone " +
                    "from Bonus.Bonuses ad " +
                    "inner join Bonus.card card on card.CardId=ad.CardId " +
                    "inner join Customers.Customer customer on card.CardId=customer.CustomerID " +
                    "where EndDate is not null and EndDate <  @date and Status <> @status ",
                    new {date = curDate, status = EBonusStatus.Removed, rule = ERule.CleanExpiredBonus })
                    .ToList();

            foreach (var item in result)
            {
                var bonus = BonusService.Get(item.BonusId);
                var pristineAmount = bonus.Amount;

                bonus.Status = EBonusStatus.Removed;
                bonus.Amount = 0m;
                BonusService.Update(bonus);

                if (pristineAmount > 0m)
                {
                    var balance = BonusService.ActualSum(item.CardId);
                    var transLog = Transaction.Factory(item.CardId, pristineAmount, basis,
                        EOperationType.SubtractBonus, balance, null, bonus.Id, null);
                    TransactionService.Create(transLog);
                }
            }
        }

        private class CleanExpiredBonusRuleDto
        {
            public int BonusId { get; set; }
            public Guid CardId { get; set; }
            public long StandardPhone { get; set; }
            public decimal Amount { get; set; }
        }
    }
}
