using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.SQL2;
using AdvantShop.Web.Admin.Models.Bonuses.NotificationTemplates;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Handlers.NotificationsTemplates
{
    public class GetNotificationsTemplatesHandler
    {
        private readonly NotificationTemplateFilterModel _filterModel;
        private SqlPaging _paging;

        public GetNotificationsTemplatesHandler(NotificationTemplateFilterModel filterModel)
        {
            _filterModel = filterModel;
        }

        public FilterResult<NotificationsTemplateGridModel> Execute()
        {
            var model = new FilterResult<NotificationsTemplateGridModel>();

            GetPaging();

            model.TotalItemsCount = _paging.TotalRowsCount;
            model.TotalPageCount = _paging.PageCount();
            model.TotalString = LocalizationService.GetResourceFormat("Admin.Grid.FildTotal", model.TotalItemsCount);

            if (model.TotalPageCount < _filterModel.Page && _filterModel.Page > 1)
            {
                return model;
            }

            model.DataItems = _paging.PageItemsList<NotificationsTemplateGridModel>();
            
            return model;
        }

        public List<int> GetItemsIds()
        {
            GetPaging();

            return _paging.ItemsIds<int>("NotificationTemplateId");
        }

        private void GetPaging()
        {
            _paging = new SqlPaging()
            {
                ItemsPerPage = _filterModel.ItemsPerPage,
                CurrentPageIndex = _filterModel.Page
            };

            _paging.Select(
                "NotificationTypeId".AsSqlField("Type"),
                "NotificationBody",
                "NotificationMethod".AsSqlField("Method"),
                "NotificationTemplateId".AsSqlField("NotificationId")
                );

            _paging.From("[Bonus].[NotificationTemplate]");

            Sorting();
            Filter();
        }

        private void Filter()
        {
            if (!string.IsNullOrEmpty(_filterModel.Search))
            {
                _paging.Where("[NotificationBody] LIKE '%'+{0}+'%'", _filterModel.Search);
            }
        }

        private void Sorting()
        {
            if (string.IsNullOrEmpty(_filterModel.Sorting) || _filterModel.SortingType == FilterSortingType.None)
            {
                _paging.OrderBy(
                    new SqlCritera("NotificationTypeId", "", SqlSort.Asc),
                    new SqlCritera("NotificationMethod", "", SqlSort.Asc)
                );
                return;
            }
            
            if(_filterModel.Sorting == "NotificationTypeName")
            {
                if (_filterModel.SortingType == FilterSortingType.Asc)
                {
                    _paging.OrderBy(
                    new SqlCritera("NotificationTypeId", "", SqlSort.Asc),
                    new SqlCritera("NotificationMethod", "", SqlSort.Asc)
                        );
                }
                else
                {
                    _paging.OrderByDesc(
                        new SqlCritera("NotificationTypeId", "", SqlSort.Desc),
                        new SqlCritera("NotificationMethod", "", SqlSort.Desc)
                    );
                }
                return;
            }

            var sorting = _filterModel.Sorting.ToLower().Replace("formatted", "");

            var field = _paging.SelectFields().FirstOrDefault(x => x.FieldName == sorting);
            if (field != null)
            {
                if (_filterModel.SortingType == FilterSortingType.Asc)
                {
                    _paging.OrderBy(sorting);
                }
                else
                {
                    _paging.OrderByDesc(sorting);
                }
            }
        }
    }
}