//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;

namespace AdvantShop.Core.Modules.Interfaces
{
    public interface IShippingMethod : IModule
    {
        string ShippingKey { get; }
        string ShippingName { get; }
    }
}
