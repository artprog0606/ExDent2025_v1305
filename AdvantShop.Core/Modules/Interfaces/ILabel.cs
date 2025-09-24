using AdvantShop.Catalog;
using System.Collections.Generic;

namespace AdvantShop.Core.Modules.Interfaces
{
    public interface ILabel : IModule
    {
        ProductLabel GetLabel();
        List<ProductLabel> GetLabels();
    }

    public interface IMarker : IModule
    {
        List<ProductMarker> GetMarkersByProductId(int productId);

        List<ProductMarkerMap> GetMarkersByProductIds(List<int> productIds);
    }
}