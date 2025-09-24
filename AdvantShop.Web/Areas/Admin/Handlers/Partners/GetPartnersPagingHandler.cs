using System;
using System.Linq;
using AdvantShop.Core.SQL2;
using AdvantShop.Web.Admin.Models.Partners;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Partners
{
    public class GetPartnersPagingHandler : AbstractHandler<PartnersFilterModel, int, PartnerModel>
    {
        public GetPartnersPagingHandler(PartnersFilterModel filterModel) : base(filterModel)
        {
        }

        protected override SqlPaging Select(SqlPaging paging)
        {
            paging.Select(
                "Id",
                "Email",
                "Name",
                "Phone",
                "DateCreated",
                "Enabled",
                "Balance",
                "Type");

            paging.From("[Partners].[Partner]");

            return paging;
        }

        protected override SqlPaging Filter(SqlPaging paging)
        {
            if (!string.IsNullOrEmpty(FilterModel.Search))
                paging.Where("(Partner.Email LIKE '%'+{0}+'%' OR Partner.Name LIKE '%'+{0}+'%' OR Partner.Phone LIKE '%'+{0}+'%')", FilterModel.Search);

            if (!string.IsNullOrWhiteSpace(FilterModel.Name))
                paging.Where("Partner.Name LIKE '%'+{0}+'%'", FilterModel.Name);

            if (!string.IsNullOrWhiteSpace(FilterModel.Email))
                paging.Where("Partner.Email LIKE '%'+{0}+'%'", FilterModel.Email);

            if (!string.IsNullOrWhiteSpace(FilterModel.Phone))
                paging.Where("Partner.Phone LIKE '%'+{0}+'%'", FilterModel.Phone);

            if (FilterModel.BalanceFrom.HasValue)
                paging.Where("Partner.Balance >= {0}", FilterModel.BalanceFrom.Value);
            if (FilterModel.BalanceTo.HasValue)
                paging.Where("Partner.Balance <= {0}", FilterModel.BalanceTo.Value);

            DateTime dateFrom, dateTo;
            if (!string.IsNullOrWhiteSpace(FilterModel.DateCreatedFrom) && DateTime.TryParse(FilterModel.DateCreatedFrom, out dateFrom))
                paging.Where("Partner.DateCreated >= {0}", dateFrom);
            if (!string.IsNullOrWhiteSpace(FilterModel.DateCreatedTo) && DateTime.TryParse(FilterModel.DateCreatedTo, out dateTo))
                paging.Where("Partner.DateCreated <= {0}", dateTo);

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