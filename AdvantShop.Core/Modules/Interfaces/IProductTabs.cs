//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Collections.Generic;

namespace AdvantShop.Core.Modules.Interfaces
{
    public interface IProductTabs : IModule
    {
        List<BaseTab> GetProductDetailsTabsCollection(int productId);

        List<BaseTabLink> GetProductDetailsTabsLinksCollection(int productId);
    }
}
