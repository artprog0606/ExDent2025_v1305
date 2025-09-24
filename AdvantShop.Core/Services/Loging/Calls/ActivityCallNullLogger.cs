using System;
using System.Collections.Generic;

namespace AdvantShop.Core.Services.Loging.Calls
{
    public class ActivityCallNullLogger : ICallLogger
    {
        public virtual void LogCall(Call call)
        {
        }

        public virtual List<Call> GetCalls(Guid customerId, string call)
        {
            return null;
        }
    }
}
