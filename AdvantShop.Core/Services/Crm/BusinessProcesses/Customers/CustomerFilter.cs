using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Crm.BusinessProcesses.Customers;
using AdvantShop.Core.Services.Crm.BusinessProcesses.MessageReplies;
using AdvantShop.Core.Services.Triggers;
using AdvantShop.Customers;

namespace AdvantShop.Core.Services.Crm.BusinessProcesses
{
    public class CustomerFilter : IBizObjectFilter
    {
        public CustomerFilter()
        {
            Comparers = new List<MessageReplyFieldComparer>();
        }

        public List<MessageReplyFieldComparer> Comparers { get; set; }

        public List<CustomerFieldComparer> CustomerComparers { get; set; }

        public bool Check(IBizObject bizObject)
        {
            var obj = (MessageReplyBizObj)bizObject;

            foreach (var comparer in Comparers)
            {
                if (!comparer.CheckField(obj))
                    return false;
            }

            // если не заданы условия - подходит в любом случае
            return true;
        }
    }

    public class CustomerTriggerFilter : ITriggerObjectFilter
    {
        public CustomerTriggerFilter()
        {
            Comparers = new List<CustomerFieldComparer>();
        }
        
        public List<CustomerFieldComparer> Comparers { get; set; }
        

        public bool Check(ITriggerObject triggerObject)
        {
            var obj = (Customer)triggerObject;

            foreach (var comparerGroup in Comparers.GroupBy(x => new {x.CompareType, x.FieldType}))
            {
                var result = false;
                var i = 0;

                foreach (var comparer in comparerGroup)
                {
                    if (i == 0)
                    {
                        result = comparer.CheckField(obj);
                    }
                    else
                    {
                        result = comparer.CompareType == BizObjectFieldCompareType.Equal
                            ? result || comparer.CheckField(obj)
                            : result && comparer.CheckField(obj);
                    }
                    i++;
                }

                if (!result)
                    return false;
            }

            // если не заданы условия - подходит в любом случае
            return true;
        }
    }
}
