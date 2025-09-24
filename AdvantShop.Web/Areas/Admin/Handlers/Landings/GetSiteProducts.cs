using System.Linq;
using AdvantShop.Core.SQL2;
using AdvantShop.Web.Admin.Models.Landings;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Landings
{
    public class GetSiteProducts : AbstractHandler<SiteProductsFilterModel, int, SiteProductModel>
    {
        public GetSiteProducts(SiteProductsFilterModel filterModel) : base(filterModel)
        {
        }

        protected override SqlPaging Select(SqlPaging paging)
        {
            paging.Select(
                "Product.ProductId",
                "Product.Name",
                "Product.ArtNo"
                );

            paging.From("Catalog.Product");
            paging.Inner_Join("CMS.LandingSite_Product ON LandingSite_Product.ProductId = Product.ProductId");

            return paging;
        }

        protected override SqlPaging Filter(SqlPaging paging)
        {
            paging.Where("LandingSite_Product.LandingSiteId = {0}", FilterModel.SiteId);

            if (!string.IsNullOrWhiteSpace(FilterModel.Search))
            {
                paging.Where("(Product.ArtNo LIKE '%'+{0}+'%' OR Product.Name LIKE '%'+{0}+'%')", FilterModel.Search);
            }

            return paging;
        }

        protected override SqlPaging Sorting(SqlPaging paging)
        {
            if (string.IsNullOrEmpty(FilterModel.Sorting) || FilterModel.SortingType == FilterSortingType.None)
            {
                paging.OrderByDesc("Product.DateAdded");
                return paging;
            }

            var sorting = FilterModel.Sorting.ToLower().Replace("formatted", "");

            var field = paging.SelectFields().FirstOrDefault(x => x.FieldName == sorting);
            if (field != null)
            {
                if (FilterModel.SortingType == FilterSortingType.Asc)
                {
                    paging.OrderBy(sorting);
                }
                else
                {
                    paging.OrderByDesc(sorting);
                }
            }

            return paging;
        }
    }
}
