using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.SQL2;
using AdvantShop.Web.Admin.Models.Settings;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Handlers.Settings.Mails
{
    public class GetSmsTemplates
    {
        private SqlPaging _paging;
        private readonly SmsTemplateFilterModel _filterModel;


        public GetSmsTemplates(SmsTemplateFilterModel filterModel)
        {
            _filterModel = filterModel;
        }

        public FilterResult<SmsTemplateModel> Execute()
        {
            var model = new FilterResult<SmsTemplateModel>();

            GetPaging();

            model.TotalItemsCount = _paging.TotalRowsCount;
            model.TotalPageCount = _paging.PageCount();
            model.TotalString = LocalizationService.GetResourceFormat("Admin.Grid.FildTotal", model.TotalItemsCount);

            if (model.TotalPageCount < _filterModel.Page && _filterModel.Page > 1)
                return model;

            model.DataItems = _paging.PageItemsList<SmsTemplateModel>();

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
                "[SmsTemplateOnOrderChanging].[OrderStatusId]",
                "SmsText",
                "Enabled",
                "StatusName".AsSqlField("OrderStatusName")
                );

            _paging.From("[Settings].[SmsTemplateOnOrderChanging]");
            _paging.Left_Join("[Order].[OrderStatus] ON [OrderStatus].[OrderStatusId] = [SmsTemplateOnOrderChanging].[OrderStatusId]");


            Sorting();
            Filter();
        }

        private void Filter()
        {
            if (!string.IsNullOrEmpty(_filterModel.SmsText))
                _filterModel.Search = _filterModel.SmsText;

            if (!string.IsNullOrEmpty(_filterModel.Search))
            {
                _paging.Where("SmsText LIKE '%'+{0}+'%'", _filterModel.Search);
            }
            
            if (_filterModel.Enabled.HasValue)
            {
                _paging.Where("Enabled = {0}", _filterModel.Enabled.Value);
            }

            if (_filterModel.OrderStatusId != null)
            {
                _paging.Where("[SmsTemplateOnOrderChanging].[OrderStatusId] = {0}", _filterModel.OrderStatusId.Value);
            }
        }

        private void Sorting()
        {
            if (string.IsNullOrEmpty(_filterModel.Sorting) || _filterModel.SortingType == FilterSortingType.None)
            {
                _paging.OrderByDesc("[SmsTemplateOnOrderChanging].[OrderStatusId]");
                _paging.OrderByDesc("Id");
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
