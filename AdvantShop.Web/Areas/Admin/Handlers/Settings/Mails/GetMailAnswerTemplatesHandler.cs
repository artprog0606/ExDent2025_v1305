using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.SQL2;
using AdvantShop.Web.Admin.Models.Settings;
using AdvantShop.Web.Admin.Models.Settings.SettingsMail;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Handlers.Settings.Mails
{
    public class GetMailAnswerTemplatesHandler
    {
        private readonly MailAnswerTemplateFilterModel _filterModel;
        private SqlPaging _paging;

        public GetMailAnswerTemplatesHandler(MailAnswerTemplateFilterModel filterModel)
        {
            _filterModel = filterModel;
        }

        public FilterResult<MailAnswerTemplateFilterModel> Execute()
        {
            var model = new FilterResult<MailAnswerTemplateFilterModel>();

            GetPaging();

            model.TotalItemsCount = _paging.TotalRowsCount;
            model.TotalPageCount = _paging.PageCount();
            model.TotalString = LocalizationService.GetResourceFormat("Admin.Grid.FildTotal", model.TotalItemsCount);

            if (model.TotalPageCount < _filterModel.Page && _filterModel.Page > 1)
            {
                return model;
            }

            model.DataItems = _paging.PageItemsList<MailAnswerTemplateFilterModel>();
            
            return model;
        }

        public List<int> GetItemsIds()
        {
            GetPaging();

            return _paging.ItemsIds<int>("TemplateId");
        }

        private void GetPaging()
        {
            _paging = new SqlPaging()
            {
                ItemsPerPage = _filterModel.ItemsPerPage,
                CurrentPageIndex = _filterModel.Page
            };

            _paging.Select(
                "TemplateId",
                "Name",
                "Active",
                "Body",
                "MailTemplate.SortOrder".AsSqlField("SortOrder"),
                "Subject"
                );

            _paging.From("[Settings].[MailTemplate]");

            Sorting();
            Filter();
        }

        private void Filter()
        {
            if (!string.IsNullOrEmpty(_filterModel.Name))
                _filterModel.Search = _filterModel.Name;

            if (!string.IsNullOrEmpty(_filterModel.Search))
            {
                _paging.Where("Name LIKE '%'+{0}+'%'", _filterModel.Search);
            }

            if (_filterModel.Active != null)
            {
                _paging.Where("Active = {0}", Convert.ToInt32(_filterModel.Active.Value));
            }
        }

        private void Sorting()
        {
            if (string.IsNullOrEmpty(_filterModel.Sorting) || _filterModel.SortingType == FilterSortingType.None)
            {
                _paging.OrderBy("MailTemplate.SortOrder");
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