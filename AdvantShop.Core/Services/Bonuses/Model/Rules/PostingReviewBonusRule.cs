using AdvantShop.Configuration;
using AdvantShop.Core.Services.Bonuses.Model.Enums;
using AdvantShop.Core.Services.Bonuses.Service;
using AdvantShop.Core.Services.Bonuses.Notification;
using AdvantShop.Core.Services.Bonuses.Notification.Template;
using AdvantShop.Customers;
using AdvantShop.Orders;
using Quartz;
using System;

namespace AdvantShop.Core.Services.Bonuses.Model.Rules
{
	[DisallowConcurrentExecution]
	public class PostingReviewBonusRule : BaseRule
	{
		public decimal GiftBonus { get; set; }

        public void Execute(Guid customerId, long cardNumber, int? productId)
        {
            var prrule = CustomRuleService.Get(ERule.PostingReview);
            if (prrule == null || !prrule.Enabled) return;
            var rule = BaseRule.Get(prrule) as PostingReviewBonusRule;
            if (rule == null) return;
            
            var companyname = SettingsMain.ShopName;    
            var customer = CustomerService.GetCustomer(customerId);    
            processCard(cardNumber, customer, rule, prrule.Name, companyname, productId);
        }

        private void processCard(long cardNumber, Customer customer, PostingReviewBonusRule rule, string title, string companyname, int? productId)
        {
            var card = CardService.Get(cardNumber);
            if (card == null || card.Blocked) return;
            var objType = productId.HasValue ? EBonusRuleObjectType.Product : EBonusRuleObjectType.Store;
            if (RuleLogService.IsHasBonusReview(card.CardId, productId, ERule.PostingReview, objType)) // получал бонус за отзыв
                return;
            else if (productId.HasValue && !OrderService.IsCustomerHasPaidOrderWithProduct(customer.Id, (int)productId)) // не оплачивал данный товар
                return;

            var newbonus = new Bonus
            {
                CardId = card.CardId,
                Amount = rule.GiftBonus,
                Name = title,
                Status = EBonusStatus.Create
            };
            newbonus.Id = BonusService.Add(newbonus);
            
            var balance = BonusService.ActualSum(card.CardId);
            
            var transLog = Transaction.Factory(card.CardId, newbonus.Amount, title, EOperationType.AddBonus, balance, null, newbonus.Id, null);
            TransactionService.Create(transLog);
            
            var r = new RuleLog
            {
                CardId = card.CardId,
                RuleType = ERule.PostingReview,
                Created = DateTime.Today,
                ObjectType = objType,
                ObjectId = productId.HasValue ? productId.ToString() : "0"
            };
            RuleLogService.Add(r);

            if (customer.StandardPhone.HasValue || string.IsNullOrEmpty(customer.EMail))
                NotificationService.Process(card.CardId, ENotifcationType.OnAddBonus, new OnAddBonusTempalte
                {
                    CompanyName = companyname,
                    Bonus = rule.GiftBonus,
                    Balance = balance,
                    Basis = title
                });
        }
    }
}
