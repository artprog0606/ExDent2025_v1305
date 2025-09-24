using System;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.SQL2;
using AdvantShop.Web.Admin.Models.Marketing.Emailings;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Marketing.Emailings
{
    public class GetManualEmailingsHandler : AbstractHandler<ManualEmailingsFilterModel, Guid, ManualEmailingModel>
    {
        public GetManualEmailingsHandler(ManualEmailingsFilterModel filterModel) : base(filterModel)
        {

        }

        protected override SqlPaging Select(SqlPaging paging)
        {
            paging.Select("Id", "Subject", "TotalCount", "DateCreated");
            paging.From("Customers.ManualEmailing");
            return paging;
        }

        protected override SqlPaging Filter(SqlPaging paging)
        {
            if (FilterModel.Search.IsNotEmpty())
                paging.Where("Subject LIKE '%'+{0}+'%'", FilterModel.Search);

            if (FilterModel.Subject.IsNotEmpty())
                paging.Where("Subject LIKE '%'+{0}+'%'", FilterModel.Subject);

            if (FilterModel.TotalCountFrom.HasValue)
                paging.Where("TotalCount >= {0}", FilterModel.TotalCountFrom.Value);
            if (FilterModel.TotalCountTo.HasValue)
                paging.Where("TotalCount <= {0}", FilterModel.TotalCountTo.Value);

            if (FilterModel.DateCreatedFrom.HasValue)
                paging.Where("DateCreated >= {0}", FilterModel.DateCreatedFrom.Value);
            if (FilterModel.DateCreatedTo.HasValue)
                paging.Where("DateCreated <= {0}", FilterModel.DateCreatedTo.Value);

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