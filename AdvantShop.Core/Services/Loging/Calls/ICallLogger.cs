using System;
using System.Collections.Generic;

namespace AdvantShop.Core.Services.Loging.Calls
{
    public interface ICallLogger: IAdvantShopLogger
    {
        void LogCall(Call call);

        List<Call> GetCalls(Guid customerId, string phone);
    }
}
