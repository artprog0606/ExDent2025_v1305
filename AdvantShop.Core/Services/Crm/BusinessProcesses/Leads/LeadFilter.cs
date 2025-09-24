using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Triggers;

namespace AdvantShop.Core.Services.Crm.BusinessProcesses
{
    public class LeadFilter : IBizObjectFilter, ITriggerObjectFilter
    {
        public LeadFilter()
        {
            Comparers = new List<LeadFieldComparer>();
        }

        public List<LeadFieldComparer> Comparers { get; set; }

        public bool Check(IBizObject bizObject)
        {
            var lead = (Lead)bizObject;

            foreach (var comparer in Comparers)
            {
                if (!comparer.CheckField(lead))
                    return false;
            }
            // если не заданы условия - лид подходит в любом случае
            return true;
        }

        public bool Check(ITriggerObject triggerObject)
        {
            var lead = (Lead)triggerObject;

            foreach (var comparerGroup in Comparers.GroupBy(x => new { x.CompareType, x.FieldType, FieldObjId = x.FieldComparer != null ? x.FieldComparer.FieldObjId : null }))
            {
                var result = false;
                var i = 0;

                foreach (var comparer in comparerGroup)
                {
                    if (i == 0)
                    {
                        result = comparer.CheckField(lead);
                    }
                    else
                    {
                        result = comparer.CompareType == BizObjectFieldCompareType.Equal
                            ? result || comparer.CheckField(lead)
                            : result && comparer.CheckField(lead);
                    }
                    i++;
                }

                if (!result)
                    return false;
            }

            // если не заданы условия - лид подходит в любом случае
            return true;
        }
    }
}
