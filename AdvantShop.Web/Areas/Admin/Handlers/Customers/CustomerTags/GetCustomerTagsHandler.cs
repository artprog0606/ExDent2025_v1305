using AdvantShop.Core.SQL2;
using AdvantShop.Web.Admin.Models.Customers.CustomerTags;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Web.Infrastructure.Handlers;
using System;
using System.Linq;

namespace AdvantShop.Web.Admin.Handlers.Customers.CustomerTags
{
    public class GetCustomerTagsHandler : AbstractHandler<CustomerTagsFilterModel, int, CustomerTagModel>
    {
        public GetCustomerTagsHandler(CustomerTagsFilterModel filterModel) : base(filterModel)
        {

        }

        protected override SqlPaging Select(SqlPaging paging)
        {
            paging.Select("Id", "Name", "Enabled", "SortOrder");
            paging.From("[Customers].[Tag]");
            return paging;
        }

        protected override SqlPaging Filter(SqlPaging paging)
        {
            if (!string.IsNullOrEmpty(FilterModel.Search))
            {
                paging.Where("Name LIKE '%'+{0}+'%'", FilterModel.Search);
            }

            if (!string.IsNullOrEmpty(FilterModel.Name))
            {
                paging.Where("Name LIKE '%'+{0}+'%'", FilterModel.Name);
            }

            if (FilterModel.Enabled != null)
            {
                paging.Where("Enabled = {0}", Convert.ToInt32(FilterModel.Enabled.Value));
            }
            return paging;
        }

        protected override SqlPaging Sorting(SqlPaging paging)
        {
            if (string.IsNullOrEmpty(FilterModel.Sorting) || FilterModel.SortingType == FilterSortingType.None)
            {
                paging.OrderBy("SortOrder");
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
