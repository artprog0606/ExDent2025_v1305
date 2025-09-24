using System.Collections.Generic;
using AdvantShop.Core.Services.Crm.BusinessProcesses;
using AdvantShop.Core.Services.Crm.BusinessProcesses.MessageReplies;
using AdvantShop.Customers;
using AdvantShop.Web.Admin.Models.Tasks;

namespace AdvantShop.Web.Admin.Handlers.BizProcesses
{
    public class BizProcessCustomerHandler<TRule> : BizProcessHandler<TRule> where TRule : BizProcessCustomerRule
    {
        private readonly Customer _customer;

        public BizProcessCustomerHandler(List<TRule> rules, Customer customer, EMessageReplyFieldType type) : base(rules, new MessageReplyBizObj(customer, type))
        {
            _customer = customer;
        }

        public override TaskModel GenerateTask()
        {
            TaskModel = new TaskModel
            {
                ClientCustomerId = _customer.Id
            };

            return base.GenerateTask();
        }
    }
}
