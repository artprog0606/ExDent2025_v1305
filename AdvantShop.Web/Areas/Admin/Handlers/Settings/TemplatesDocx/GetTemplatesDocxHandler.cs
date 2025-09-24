using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.SQL2;
using AdvantShop.Web.Admin.Models.Settings.TemplatesDocxSettings;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Handlers.Settings.TemplatesDocx
{
    public class GetTemplatesDocxHandler
    {
        private readonly TemplatesDocxFilterModel _filterModel;
        private SqlPaging _paging;

        public GetTemplatesDocxHandler(TemplatesDocxFilterModel filterModel)
        {
            _filterModel = filterModel;
        }

        public FilterResult<TemplatesDocxModel> Execute()
        {
            var model = new FilterResult<TemplatesDocxModel>();

            GetPaging();

            model.TotalItemsCount = _paging.TotalRowsCount;
            model.TotalPageCount = _paging.PageCount();
            model.TotalString = string.Format("Найдено шаблонов: {0}", model.TotalItemsCount);

            if (model.TotalPageCount < _filterModel.Page && _filterModel.Page > 1)
            {
                return model;
            }

            model.DataItems = _paging.PageItemsList<TemplatesDocxModel>();

            return model;
        }

        public List<int> GetItemsIds()
        {
            GetPaging();

            return _paging.ItemsIds<int>("Id");
        }

        private void GetPaging()
        {
            _paging = new SqlPaging()
            {
                ItemsPerPage = _filterModel.ItemsPerPage,
                CurrentPageIndex = _filterModel.Page
            };

            _paging.Select(
                "Id",
                "Type",
                "Name",
                "SortOrder",
                "FileName",
                "FileSize",
                "DateCreated",
                "DateModified"
                );

            _paging.From("[CMS].[TemplatesDocx]");

            Sorting();
            Filter();
        }

        private void Filter()
        {
            if (_filterModel.Search.IsNotEmpty())
                _paging.Where("Name LIKE '%'+{0}+'%'", _filterModel.Search);

            if (_filterModel.Type.HasValue)
                _paging.Where("Type = {0}", (int)_filterModel.Type.Value);
        }

        private void Sorting()
        {
            if (string.IsNullOrEmpty(_filterModel.Sorting) || _filterModel.SortingType == FilterSortingType.None)
            {
                _paging.OrderBy("SortOrder");
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
