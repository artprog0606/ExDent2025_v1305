using AdvantShop.Areas.Api.Model.OrderStatus;
using AdvantShop.Core.Services.Webhook.Models.Api;
using AdvantShop.Core.SQL2;
using AdvantShop.Web.Infrastructure.Api;

namespace AdvantShop.Areas.Api.Handlers.OrderStatus
{
    public class GetOrderStatuses : EntitiesHandler<FilterOrderStatusesModel, OrderStatusModel>
    {
        public GetOrderStatuses(FilterOrderStatusesModel model) : base(model) { }

        protected override SqlPaging Select(SqlPaging paging)
        {
            paging.Select(
                "OrderStatusId".AsSqlField("Id"),
                "StatusName".AsSqlField("Name"),
                "IsCanceled",
                "IsCompleted",
                "Hidden"
                );

            paging.From("[Order].[OrderStatus]");

            return paging;
        }

        protected override SqlPaging Filter(SqlPaging paging)
        {
            if (FilterModel.IsCanceled.HasValue)
                paging.Where("IsCanceled = {0}", FilterModel.IsCanceled.Value);

            if (FilterModel.IsCompleted.HasValue)
                paging.Where("IsCompleted = {0}", FilterModel.IsCompleted.Value);

            if (FilterModel.Hidden.HasValue)
                paging.Where("Hidden = {0}", FilterModel.Hidden.Value);

            return paging;
        }

        protected override SqlPaging Sorting(SqlPaging paging)
        {
            if (string.IsNullOrEmpty(FilterModel.Sorting) || FilterModel.SortingType == FilterSortingType.None)
            {
                paging.OrderBy("SortOrder");

                return paging;
            }

            /*var sorting = FilterModel.Sorting;

            var field = paging.SelectFields().FirstOrDefault(x => x.FieldName.Equals(sorting, StringComparison.OrdinalIgnoreCase));
            if (field != null)
            {
                if (FilterModel.SortingType == FilterSortingType.Asc)
                    paging.OrderBy(sorting);
                else
                    paging.OrderByDesc(sorting);
            }*/

            return paging;
        }
    }
}