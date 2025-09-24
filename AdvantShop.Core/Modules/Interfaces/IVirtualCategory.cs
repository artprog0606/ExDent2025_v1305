using AdvantShop.Catalog;
using AdvantShop.CMS;
using System.Collections.Generic;

namespace AdvantShop.Core.Modules.Interfaces
{
    public interface IVirtualCategory
    {
        Category GetVirtualCategory(Category category);
        ICategoryModel GetVirtualCategoryModel(ICategoryModel categoryModel);
        List<BreadCrumbs> GetVirtualCategoryBreadCrumbs(List<BreadCrumbs> breadCrumbs);
        Dictionary<int, KeyValuePair<float, float>> GetRangeIds(Dictionary<int, KeyValuePair<float, float>> rangeIds);
        string GetUrlParentCategory(string url);
    }
}