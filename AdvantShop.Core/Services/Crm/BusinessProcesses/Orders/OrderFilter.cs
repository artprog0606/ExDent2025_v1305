using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Triggers;
using AdvantShop.Orders;

namespace AdvantShop.Core.Services.Crm.BusinessProcesses
{
    public class OrderFilter : IBizObjectFilter, ITriggerObjectFilter
    {
        public OrderFilter()
        {
            Comparers = new List<OrderFieldComparer>();
        }

        public List<OrderFieldComparer> Comparers { get; set; }

        public bool Check(IBizObject bizObject)
        {
            var order = (Order)bizObject;

            foreach (var comparer in Comparers)
            {
                if (!comparer.CheckField(order))
                    return false;
            }
            // если не заданы условия - заказ подходит в любом случае
            return true;
        }

        public bool Check(ITriggerObject triggerObject)
        {
            var order = (Order)triggerObject;

            foreach (var comparerGroup in Comparers.GroupBy(x => new {x.CompareType, x.FieldType}))
            {
                var result = false;
                var i = 0;

                foreach (var comparer in comparerGroup)
                {
                    if (i == 0)
                    {
                        result = comparer.CheckField(order);
                    }
                    else
                    {
                        result = comparer.CompareType == BizObjectFieldCompareType.Equal
                            ? result || comparer.CheckField(order)
                            : result && comparer.CheckField(order);
                    }
                    i++;
                }

                if (!result)
                    return false;
            }

            // если не заданы условия - заказ подходит в любом случае
            return true;
        }
    }
}
