using System.Linq;
using AdvantShop.Core.SQL2;
using AdvantShop.Web.Admin.Models.Settings.Jobs;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Handlers.Settings.System
{
    internal class GetJobRunsHandler
    {
        private readonly JobRunsFilterModel _filterModel;
        private SqlPaging _paging;

        public GetJobRunsHandler(JobRunsFilterModel filterModel)
        {
            _filterModel = filterModel;
        }

        public FilterResult<JobRunsFilterResultModel> Execute()
        {
            var model = new FilterResult<JobRunsFilterResultModel>();
            GetPaging();

            model.TotalItemsCount = _paging.TotalRowsCount;
            model.TotalPageCount = _paging.PageCount();

            model.DataItems = _paging.PageItemsList<JobRunsFilterResultModel>();

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
                "[Name]",
                "[Group]",
                "Initiator",
                "Status",
                "StartDate",
                "EndDate",
                "DATEDIFF(second, StartDate, EndDate)".AsSqlField("ExecutionTime"),
                "(SELECT CAST(CASE WHEN EXISTS(SELECT * FROM [Settings].[QuartzJobRunLogs] WHERE [QuartzJobRunLogs].JobRunId = [QuartzJobRuns].Id) THEN 1 ELSE 0 END AS BIT))"
                    .AsSqlField("HasLogs"));
            _paging.From("[Settings].[QuartzJobRuns]");

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
                .Replace("formatted", "")
                .Replace("name", "[name]")
                .Replace("group", "[group]");
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
                    "[Group] LIKE '%'+{0}+'%' OR " +
                    "[Name] LIKE '%'+{0}+'%') ",
                    _filterModel.Search);
            }

            if (string.IsNullOrWhiteSpace(_filterModel.Name) is false)
            {
                _paging.Where("[Name] LIKE '%'+{0}+'%'", _filterModel.Name);
            }

            if (string.IsNullOrWhiteSpace(_filterModel.Group) is false)
            {
                _paging.Where("[Group] LIKE '%'+{0}+'%'", _filterModel.Group);
            }

            if (string.IsNullOrWhiteSpace(_filterModel.Status) is false)
            {
                _paging.Where("Status LIKE '%'+{0}+'%'", _filterModel.Status);
            }

            if (_filterModel.StartDateFrom.HasValue)
                _paging.Where("StartDate >= {0}", _filterModel.StartDateFrom.Value);
            if (_filterModel.StartDateTo.HasValue)
                _paging.Where("StartDate <= {0}", _filterModel.StartDateTo.Value);

            if (_filterModel.EndDateFrom.HasValue)
                _paging.Where("EndDate >= {0}", _filterModel.EndDateFrom.Value);
            if (_filterModel.EndDateTo.HasValue)
                _paging.Where("EndDate <= {0}", _filterModel.EndDateTo.Value);
            
            if (_filterModel.ExecutionTimeFrom.HasValue)
                _paging.Where("DATEDIFF(second, StartDate, EndDate) >= {0}", _filterModel.ExecutionTimeFrom.Value);
            if (_filterModel.ExecutionTimeTo.HasValue)
                _paging.Where("DATEDIFF(second, StartDate, EndDate) <= {0}", _filterModel.ExecutionTimeTo.Value);
        }
    }
}
