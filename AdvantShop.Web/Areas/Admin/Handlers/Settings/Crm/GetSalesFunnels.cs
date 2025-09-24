using System;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.SQL2;
using AdvantShop.Web.Admin.Models.Crm.SalesFunnels;
using AdvantShop.Web.Admin.Models.Settings;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Crm.SalesFunnels
{
    public class GetSalesFunnels : AbstractHandler<SalesFunnelsFilterModel, int, SalesFunnelModel>
    {
        public GetSalesFunnels(SalesFunnelsFilterModel filterModel) : base(filterModel) { }

        protected override SqlPaging Select(SqlPaging paging)
        {
            paging.Select(
                "Id",
                "Name",
                "SortOrder",
                "FinalSuccessAction",
                "NotSendNotificationsOnLeadCreation",
                "Enable"
                );

            paging.From("[Crm].[SalesFunnel]");

            return paging;
        }

        protected override SqlPaging Filter(SqlPaging paging)
        {
            if (FilterModel.Search.IsNotEmpty())
                paging.Where("Name LIKE '%'+{0}+'%'", FilterModel.Search);

            if (FilterModel.Name.IsNotEmpty())
                paging.Where("Name LIKE {0}+'%'", FilterModel.Name);

            if (FilterModel.Enable.HasValue)
                paging.Where("Enable = {0}", Convert.ToBoolean(FilterModel.Enable.Value));
            
            return paging;
        }

        protected override SqlPaging Sorting(SqlPaging paging)
        {
            if (string.IsNullOrEmpty(FilterModel.Sorting) || FilterModel.SortingType == FilterSortingType.None)
            {
                paging.OrderByDesc("Enable");
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