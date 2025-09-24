using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Bonuses.Model;
using AdvantShop.Core.SQL2;
using AdvantShop.Web.Admin.Models.Bonuses.NotificationTemplates;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Bonuses.NotificationTemplates
{
    public class NotificationLogView : NotificationLog
    {
        public string Created_Str
        {
            get { return Created.ToString("dd.MM.yyyy hh:mm:ss"); }
        }
    }

    public class GetNotificationLogHandler : AbstractHandler<NotificationLogFilterModel, int, NotificationLogView>
    {
        public GetNotificationLogHandler(NotificationLogFilterModel filterModel) : base(filterModel)
        {
        }

        protected override SqlPaging Select(SqlPaging paging)
        {
            paging.Select("Id", "Body", "State", "Contact", "Created");
            paging.From("[Bonus].[NotificationLog]");
            return paging;
        }

        protected override SqlPaging Filter(SqlPaging paging)
        {
            if (FilterModel.Search.IsNotEmpty())
            {
                paging.Where("Body like '%'+{0}+'%'", FilterModel.Search);
            }

            if (!string.IsNullOrWhiteSpace(FilterModel.Contact))
            {
                paging.Where("Contact = {0}", FilterModel.Contact);
            }
            if (!string.IsNullOrWhiteSpace(FilterModel.Body))
            {
                paging.Where("Body like '%'+{0}+'%'", FilterModel.Body);
            }
            return paging;
        }

        protected override SqlPaging Sorting(SqlPaging paging)
        {
            if (string.IsNullOrEmpty(FilterModel.Sorting) || FilterModel.SortingType == FilterSortingType.None)
            {
                paging.OrderByDesc("Created");
                return paging;
            }

            var sorting = FilterModel.Sorting.ToLower().Replace("formatted", "").Replace("_str","");

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
