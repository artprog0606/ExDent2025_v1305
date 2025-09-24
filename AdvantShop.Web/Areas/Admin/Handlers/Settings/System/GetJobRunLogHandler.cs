using System.Linq;
using AdvantShop.Core.SQL2;
using AdvantShop.Web.Admin.Models.Settings.Jobs;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Handlers.Settings.System
{
    internal class GetJobRunLogHandler
    {
        private readonly string _jobRunId;
        private readonly JobRunLogFilterModel _filterModel;
        private readonly bool _isExport;
        private SqlPaging _paging;

        public GetJobRunLogHandler(string jobRunId, JobRunLogFilterModel filterModel, bool isExport = false)
        {
            _jobRunId = jobRunId;
            _filterModel = filterModel;
            _isExport = isExport;
        }

        public FilterResult<JobRunLogFilterResultModel> Execute()
        {
            var model = new FilterResult<JobRunLogFilterResultModel>();
            GetPaging();

            model.TotalItemsCount = _paging.TotalRowsCount;
            model.TotalPageCount = _paging.PageCount();

            model.DataItems = _paging.PageItemsList<JobRunLogFilterResultModel>();

            return model;
        }

        private void GetPaging()
        {
            _paging = new SqlPaging
            {
                ItemsPerPage = _filterModel.ItemsPerPage,
                CurrentPageIndex = _filterModel.Page
            };

            _paging.Select(
                "Id",
                "Event",
                "Message",
                "AddDate");

            if (_isExport)
            {
                _paging.Select("JobRunId");
            }

            _paging.From("[Settings].[QuartzJobRunLogs]");
            _paging.Where("JobRunId = {0}", _jobRunId);

            Sorting();
            Filter();
        }

        private void Sorting()
        {
            if (string.IsNullOrEmpty(_filterModel.Sorting) || _filterModel.SortingType == FilterSortingType.None)
            {
                _paging.OrderByDesc("Id");
                return;
            }

            var sorting = _filterModel.Sorting.ToLower()
                .Replace("formatted", "");
            var field = _paging.SelectFields().FirstOrDefault(x => x.FieldName == sorting);

            if (field == null)
                return;

            if (_filterModel.SortingType == FilterSortingType.Asc)
            {
                _paging.OrderBy(sorting);
            }
            else
            {
                _paging.OrderByDesc(sorting);
            }
        }

        private void Filter()
        {
            if (string.IsNullOrWhiteSpace(_filterModel.Search) is false)
            {
                _paging.Where(
                    "(Id LIKE '%'+{0}+'%' OR " +
                    "Event LIKE '%'+{0}+'%' OR " +
                    "Message LIKE '%'+{0}+'%') ",
                    _filterModel.Search);
            }

            if (string.IsNullOrWhiteSpace(_filterModel.Event) is false)
            {
                _paging.Where("Event LIKE '%'+{0}+'%'", _filterModel.Event);
            }

            if (_filterModel.AddDateFrom.HasValue)
                _paging.Where("AddDate >= {0}", _filterModel.AddDateFrom.Value);
            if (_filterModel.AddDateTo.HasValue)
                _paging.Where("AddDate <= {0}", _filterModel.AddDateTo.Value);
        }
    }
}
