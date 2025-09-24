using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.SQL2;
using AdvantShop.Web.Admin.Models.Settings;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Handlers.Settings.System
{
    public class GetLocalizations
    {
        private readonly AdminLocalizationsFilterModel _filterModel;
        private SqlPaging _paging;

        public GetLocalizations(AdminLocalizationsFilterModel filterModel)
        {
            _filterModel = filterModel;
        }

        public FilterResult<LocalizedResource> Execute()
        {
            var model = new FilterResult<LocalizedResource>();

            GetPaging();

            model.TotalItemsCount = _paging.TotalRowsCount;
            model.TotalPageCount = _paging.PageCount();
            model.TotalString = $"Найдено позиций: {model.TotalItemsCount}";

            if (model.TotalPageCount < _filterModel.Page && _filterModel.Page > 1)
            {
                return model;
            }

            model.DataItems = _paging.PageItemsList<LocalizedResource>();

            return model;
        }

        public List<int> GetItemsIds()
        {
            GetPaging();

            return _paging.ItemsIds<int>("ResourceValue");
        }

        private void GetPaging()
        {
            _paging = new SqlPaging()
            {
                ItemsPerPage = _filterModel.ItemsPerPage,
                CurrentPageIndex = _filterModel.Page
            };

            _paging.Select(
                "[Localization].[LanguageId]",
                "ResourceKey",
                "ResourceValue",
                "[Language].[LanguageCode]"
            );

            _paging.From("[Settings].[Localization]");
            _paging.Left_Join("[Settings].[Language] on [Language].[LanguageID] = [Localization].[LanguageId]");

            Sorting();
            Filter();
        }

        private void Filter()
        {
            if (!_filterModel.ChangeAll)
            {
                _paging.Where("ResourceKey not like 'Admin.%' and ResourceKey not like 'Core.%'");
            }
            if (_filterModel.Value != null && _filterModel.LanguageId != null)
            {
                _paging.Where("[Localization].[LanguageId] = {0}", _filterModel.LanguageId);
            }
            if (!string.IsNullOrWhiteSpace(_filterModel.Search))
            {
                _paging.Where("(ResourceKey LIKE '%'+{0}+'%' OR ResourceValue LIKE '%'+{0}+'%')", _filterModel.Search);
            }
            if (_filterModel.ResourceKey != null)
            {
                _paging.Where("ResourceKey LIKE '%'+{0}+'%'", _filterModel.ResourceKey);
            }
            if (!string.IsNullOrWhiteSpace(_filterModel.ResourceValue))
            {
                _paging.Where("ResourceValue LIKE '%'+{0}+'%'", _filterModel.ResourceValue);
            }
        }

        private void Sorting()
        {
            if (string.IsNullOrEmpty(_filterModel.Sorting) || _filterModel.SortingType == FilterSortingType.None)
            {
                _paging.OrderBy(new SqlCritera("ResourceKey", "", SqlSort.Asc));
                return;
            }

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
}
