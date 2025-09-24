using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.SQL2;
using AdvantShop.Web.Admin.Models.Booking.Affiliate.SmsTemplate;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Handlers.Booking.Affiliate.SmsTemplate
{
    public class GetSmsTemplatesHandler
    {
        private readonly SmsTemplatesFilterModel _filterModel;
        private SqlPaging _paging;

        public GetSmsTemplatesHandler(SmsTemplatesFilterModel filterModel)
        {
            _filterModel = filterModel;
        }

        public FilterResult<SmsTemplateModel> Execute()
        {
            var model = new FilterResult<SmsTemplateModel>();

            GetPaging();

            model.TotalItemsCount = _paging.TotalRowsCount;
            model.TotalPageCount = _paging.PageCount();
            model.TotalString = string.Format("Найдено шаблонов: {0}", model.TotalItemsCount);

            if (model.TotalPageCount < _filterModel.Page && _filterModel.Page > 1)
            {
                return model;
            }

            model.DataItems = _paging.PageItemsList<SmsTemplateModel>();

            return model;
        }

        public List<int> GetItemsIds(string fieldName)
        {
            GetPaging();

            return _paging.ItemsIds<int>(fieldName);
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
                "AffiliateId",
                "Status",
                "Text",
                "Enabled"
                );

            _paging.From("Booking.AffiliateSmsTemplate");

            Sorting();
            Filter();
        }

        private void Filter()
        {
            if (_filterModel.Search.IsNotEmpty())
            {
                _paging.Where("Text LIKE '%'+{0}+'%'", _filterModel.Search);
            }
            if (_filterModel.AffiliateFilterId.HasValue)
                _paging.Where("AffiliateId = {0}", _filterModel.AffiliateFilterId.Value);
            if (_filterModel.Status.HasValue)
                _paging.Where("Status = {0}", (int)_filterModel.Status.Value);
            if (_filterModel.Enabled.HasValue)
                _paging.Where("Enabled = {0}", _filterModel.Enabled.Value);
        }

        private void Sorting()
        {
            if (string.IsNullOrEmpty(_filterModel.Sorting) || _filterModel.SortingType == FilterSortingType.None)
            {
                _paging.OrderBy("Status");
                return;
            }

            if(_filterModel.Sorting == "StatusName")
            {
                if (_filterModel.SortingType == FilterSortingType.Asc)
                {
                    _paging.OrderBy("Status");
                }
                else
                {
                    _paging.OrderByDesc("Status");
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
