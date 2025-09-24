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
    public class CancellationsBonusRule : BaseRule
    {
        public int SmsDayBefore { get; set; }
        public int AgeCard { get; set; }
        public int CleanAfterMonth => AgeCard;
        public bool NotSendSms { get; set; }

        public override void Process(IJobExecutionContext context)
        {
            var cancelRule = CustomRuleService.Get(ERule.CancellationsBonus);
            if (cancelRule == null || !cancelRule.Enabled) return;

            var rule = BaseRule.Get(cancelRule) as CancellationsBonusRule;
            if (rule == null)
                return;
     
            SmsDayBefore = rule.SmsDayBefore;
            AgeCard = rule.AgeCard;
            NotSendSms = rule.NotSendSms;
       
            ProcessNotification();
            ProcessCard(cancelRule);
        }

        private void ProcessNotification()
        {
            if (NotSendSms) return;

            var today = DateTime.Today;
            var endDay = today.AddMonths(-CleanAfterMonth).AddDays(SmsDayBefore);

            var result =
                SQLDataAccess2.ExecuteReadIEnumerable<CancellationsBonusRuleDto>(
                                   "select ad.Id as BonusId, ad.Amount, card.CardId, customer.StandardPhone " +
                                   "from Bonus.Bonuses ad " +
                                   "inner join Bonus.card card on card.CardId=ad.CardId " +
                                   "inner join Customers.Customer customer on card.CardId=customer.CustomerID " +
                                   "where " +
                                   "   EndDate is null" +
                                   "   and Status <> @status " +
                                   "   and ((ad.[StartDate] IS NULL AND ad.[CreateOn] < @endDay) OR (ad.[StartDate] < @endDay)) " +
                                   "   and ad.NotifiedAboutExpiry <> 1 " +
                                   "ORDER BY card.CardId",
                                   new {today, endDay, status = EBonusStatus.Removed, rule = ERule.CancellationsBonus})
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
            NotificationService.Process(cardId, ENotifcationType.OnCancellationsBonus,
                new OnCancellationsBonusTempalte
                {
                    CompanyName = companyname,
                    Balance = balance,
                    CleanBonuses = cleanBonuses,
                    DayLeft = SmsDayBefore
                });

            RuleLogService.Add(new RuleLog
            {
                CardId = cardId,
                RuleType = ERule.CancellationsBonus,
                Created = DateTime.Today,
                ObjectType = EBonusRuleObjectType.None,
                ObjectId = string.Empty
            });
        }

        private void ProcessCard(CustomRule rule)
        {
            var today = DateTime.Today;
            var endDay = today.AddMonths(-CleanAfterMonth);

            var result =
                SQLDataAccess2.ExecuteReadIEnumerable<CancellationsBonusRuleDto>(
                                   "select ad.Id as BonusId, card.CardId, customer.StandardPhone " +
                                   "from Bonus.Bonuses ad " +
                                   "inner join Bonus.card card on card.CardId=ad.CardId " +
                                   "inner join Customers.Customer customer on card.CardId=customer.CustomerID " +
                                   "where " +
                                   "   EndDate is null" +
                                   "   and Status <> @status " +
                                   "   and ((ad.[StartDate] IS NULL AND ad.[CreateOn] < @endDay) OR (ad.[StartDate] < @endDay))",
                                   new {today, endDay, status = EBonusStatus.Removed, rule = ERule.CancellationsBonus})
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
                    var transLog = Transaction.Factory(item.CardId, pristineAmount, rule.Name,
                        EOperationType.SubtractBonus, balance, null, bonus.Id, null);
                    TransactionService.Create(transLog);
                }
            }
        }

        private class CancellationsBonusRuleDto
        {
            public int BonusId { get; set; }
            public Guid CardId { get; set; }
            public long StandardPhone { get; set; }
            public decimal Amount { get; set; }
        }
    }
}