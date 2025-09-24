using System;
using System.Linq;
using AdvantShop.Web.Admin.Handlers.Marketing.Analytics.Reports;
using AdvantShop.Web.Admin.Models.Marketing.Analytics;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Handlers.Marketing.Analytics
{
    public class GetAnalyticsFilterRfm
    {
        private readonly BaseFilterModel _filterModel;
        private readonly string _group;
        private readonly DateTime _from;
        private readonly DateTime _to;

        public GetAnalyticsFilterRfm(BaseFilterModel filterModel, string group, DateTime from, DateTime to)
        {
            _filterModel = filterModel;
            _group = group;
            _from = from;
            _to = to;
        }

        public FilterResult<AnalyticsFilterRfmModel> Execute()
        {
            var model = new FilterResult<AnalyticsFilterRfmModel>();

            if (string.IsNullOrEmpty(_group))
                return model;

            var arr = _group.ToUpper().Split('_');
            if (arr.Length != 4)
                return model;
            
            var handler = new RfmAnalysisHandler(_from, _to);
            var data = handler.GetDataItems();

            if (data == null)
                return model;

            var items = arr[1] == "M"
                ? data.Where(x => x.R.ToString() == arr[2] && x.M.ToString() == arr[3]).ToList()
                : data.Where(x => x.R.ToString() == arr[2] && x.F.ToString() == arr[3]).ToList();

            if (items.Count == 0)
                return model;

            var customerIds = items.Select(x => x.CustomerId).ToList();

            model.TotalItemsCount = customerIds.Count;
            model.TotalPageCount = (int)Math.Ceiling((double)model.TotalItemsCount / _filterModel.ItemsPerPage);

            model.DataItems =
                customerIds.Skip((_filterModel.Page - 1)*_filterModel.ItemsPerPage)
                    .Take(_filterModel.ItemsPerPage)
                    .Select(x => new AnalyticsFilterRfmModel() {CustomerId = x})
                    .ToList();

            foreach (var dataItem in model.DataItems)
            {
                var item = data.Find(x => x.CustomerId == dataItem.CustomerId);
                if (item != null)
                {
                    dataItem.LastOrderNumber = item.LastOrderNumber;
                    dataItem.LastOrderDate = item.LastOrderDate;
                    dataItem.OrdersCount = item.OrdersCount;
                    dataItem.OrdersSum = item.OrdersSum;
                }
            }

            return model;
        }

    }
}
