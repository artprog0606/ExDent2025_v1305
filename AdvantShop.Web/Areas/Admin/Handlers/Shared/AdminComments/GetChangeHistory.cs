using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.ChangeHistories;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.SQL2;
using AdvantShop.Web.Admin.Models.Shared.AdminComments;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Handlers.Shared.AdminComments
{
    public class GetChangeHistory
    {
        private readonly ChangeHistoryFilter _filterModel;
        private SqlPaging _paging;

        public GetChangeHistory(ChangeHistoryFilter filterModel)
        {
            _filterModel = filterModel;
        }

        public FilterResult<ChangeHistoryModel> Execute()
        {
            var model = new FilterResult<ChangeHistoryModel>();

            GetPaging();

            model.TotalItemsCount = _paging.TotalRowsCount;
            model.TotalPageCount = _paging.PageCount();
            model.TotalString = LocalizationService.GetResourceFormat("Admin.Grid.FildTotal", model.TotalItemsCount);

            if (model.TotalPageCount < _filterModel.Page && _filterModel.Page > 1)
                return model;

            var items = _paging.PageItemsList<ChangeHistoryModel>();
            var newItems = new List<ChangeHistoryModel>();

            
            for (var i = 0; i < items.Count; i++)
            {
                if (i == 0 ||
                    !(items[i].ParameterName == items[i - 1].ParameterName &&
                      items[i].ChangedByName == items[i - 1].ChangedByName &&
                      items[i].NewValue == items[i - 1].NewValue &&
                      items[i].OldValue == items[i - 1].OldValue &&
                      (items[i].ModificationTime - items[i - 1].ModificationTime) < TimeSpan.FromSeconds(1)))
                {
                    newItems.Add(items[i]);
                }
            }

            model.DataItems = newItems;

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
                "ParameterName",
                "OldValue",
                "NewValue",
                "ChangedById",
                "ChangedByName",
                "ModificationTime"
                );

            if (_filterModel.Type == ChangeHistoryObjType.Product)
            {
                _paging.From("[Catalog].[ProductChangeHistory]");
                _paging.Where("[ProductId] = {0}", _filterModel.ObjId);
            }
            else
            {
                _paging.From("[CMS].[ChangeHistory]");
                _paging.Where("[ObjId] = {0} and ObjType = {1}", _filterModel.ObjId, (int) _filterModel.Type);
            }

            Sorting();
        }
        

        private void Sorting()
        {
            if (string.IsNullOrEmpty(_filterModel.Sorting) || _filterModel.SortingType == FilterSortingType.None)
            {
                _paging.OrderByDesc("ModificationTime");
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
