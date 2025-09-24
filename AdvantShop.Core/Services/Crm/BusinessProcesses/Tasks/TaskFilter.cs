using System.Collections.Generic;

namespace AdvantShop.Core.Services.Crm.BusinessProcesses
{
    public class TaskFilter : IBizObjectFilter
    {
        public TaskFilter()
        {
            Comparers = new List<TaskFieldComparer>();
        }

        public List<TaskFieldComparer> Comparers { get; set; }

        public bool Check(IBizObject bizObject)
        {
            var task = (Task)bizObject;

            foreach (var comparer in Comparers)
            {
                if (!comparer.CheckField(task))
                    return false;
            }
            // если не заданы условия - задача подходит в любом случае
            return true;
        }
    }
}
