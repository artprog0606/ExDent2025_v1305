using System.Linq;
using AdvantShop.Core.SQL2;
using AdvantShop.Web.Admin.Models.PartnersReport;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.PartnersReport
{
    public class GetPayoutReportsHandler : AbstractHandler<PayoutReportsFilterModel, int, PayoutReportModel>
    {
        public GetPayoutReportsHandler(PayoutReportsFilterModel filterModel) : base(filterModel)
        {
        }

        protected override SqlPaging Select(SqlPaging paging)
        {
            paging.Select(
                "Id",
                "FileName",
                "PeriodTo",
                "DateCreated");

            paging.From("[Partners].[PayoutReport]");

            return paging;
        }

        protected override SqlPaging Filter(SqlPaging paging)
        {
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