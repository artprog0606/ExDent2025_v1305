using AdvantShop.Catalog;
using AdvantShop.Core.Services.Triggers;
using AdvantShop.Web.Admin.Models.Triggers;
using AdvantShop.Core.SQL2;
using AdvantShop.Web.Infrastructure.Admin;
using System.Linq;
using System.Collections.Generic;

namespace AdvantShop.Web.Admin.Handlers.Triggers
{
    public class GetTriggerRules
    {
        private readonly TriggerFilterModel _filterModel;
        private SqlPaging _paging;

        public GetTriggerRules(TriggerFilterModel filterModel)
        {
            _filterModel = filterModel;
        }

        public FilterResult<TriggerRuleShortDto> Execute()
        {
            var model = new FilterResult<TriggerRuleShortDto>();

            GetPaging();

            model.TotalItemsCount = _paging.TotalRowsCount;
            model.TotalPageCount = _paging.PageCount();
            model.TotalString = string.Format("Найдено триггеров: {0}", model.TotalItemsCount);

            if (model.TotalPageCount < _filterModel.Page && _filterModel.Page > 1)
            {
                return model;
            }

            model.DataItems = _paging.PageItemsList<TriggerRuleShortDto>();

            return model;
        }

        private void GetPaging()
        {
            _paging = new SqlPaging()
            {
                ItemsPerPage = _filterModel.ItemsPerPage,
                CurrentPageIndex = _filterModel.Page
            };

            _paging.Select(
                "[TriggerRule].[Id]",
                "EventType",
                "ObjectType",
                "EventObjId",
                "CategoryId",
                "[TriggerCategory].[Name]".AsSqlField("CategoryName"),
                "[TriggerRule].[Name]".AsSqlField("Name"),
                "Filter",
                "DateCreated",
                "DateModified",
                "Enabled",
                "WorksOnlyOnce",
                "EventObjValue",
                "ProcessType",
                "TriggerParams",
                "PreferredHour"
                );

            _paging.From("[CRM].[TriggerRule]");

            _paging.Left_Join("[CRM].[TriggerCategory] ON [CRM].[TriggerRule].[CategoryId] = [CRM].[TriggerCategory].[Id]");

            Filter();
            Sorting();
        }

        private void Filter()
        {
            if (!string.IsNullOrEmpty(_filterModel.Search))
            {
                _paging.Where("TriggerRule.Name like '%'+{0}+'%'", _filterModel.Search);
            }

            if (!string.IsNullOrEmpty(_filterModel.Name))
            {
                _paging.Where("TriggerRule.Name like '%'+{0}+'%'", _filterModel.Name);
            }

            if (_filterModel.Enabled.HasValue)
            {
                _paging.Where("Enabled = {0}", _filterModel.Enabled.Value ? "1" : "0");
            }

            if (_filterModel.CategoryId.HasValue)
            {
                if (_filterModel.CategoryId.Value > 0)
                    _paging.Where("CategoryId = {0}", _filterModel.CategoryId.Value);
                else
                    _paging.Where("CategoryId is NULL");
            }
        }

        private void Sorting()
        {
            if (!string.IsNullOrEmpty(_filterModel.Sorting))
            {
                var sorting = _filterModel.Sorting.ToLower();

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

        public List<T> GetItemsIds<T>(string fieldName)
        {
            GetPaging();

            return _paging.ItemsIds<T>(fieldName);
        }
    }
}
