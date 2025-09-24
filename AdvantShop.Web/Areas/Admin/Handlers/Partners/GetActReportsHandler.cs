using System.Linq;
using AdvantShop.Core.SQL2;
using AdvantShop.Web.Admin.Models.Partners;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Partners
{
    public class GetActReportsHandler : AbstractHandler<PartnerActReportsFilterModel, int, PartnerActReportModel>
    {
        public GetActReportsHandler(PartnerActReportsFilterModel filterModel) : base(filterModel)
        {
        }

        protected override SqlPaging Select(SqlPaging paging)
        {
            paging.Select(
                "Id",
                "PeriodFrom",
                "PeriodTo",
                "DateCreated"
                );

            paging.From("[Partners].[ActReport]");

            return paging;
        }

        protected override SqlPaging Filter(SqlPaging paging)
        {
            paging.Where("PartnerId = {0}", FilterModel.PartnerId);

            return paging;
        }

        protected override SqlPaging Sorting(SqlPaging paging)
        {
            if (string.IsNullOrEmpty(FilterModel.Sorting) || FilterModel.SortingType == FilterSortingType.None)
            {
                paging.OrderByDesc("DateCreated");
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