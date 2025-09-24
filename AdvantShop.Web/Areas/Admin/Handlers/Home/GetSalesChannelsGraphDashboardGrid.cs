using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Core.Services.SalesChannels;
using AdvantShop.Core.Services.Statistic;
using AdvantShop.Orders;
using AdvantShop.Repository.Currencies;
using AdvantShop.Saas;
using AdvantShop.Web.Admin.Handlers.Marketing.Analytics.Reports;
using AdvantShop.Web.Admin.Models.Home;
using AdvantShop.Web.Admin.Models.Shared.Common;

namespace AdvantShop.Web.Admin.Handlers.Home
{
    public class GetSalesChannelsGraphDashboardGrid : AnalyticsBaseHandler
    {
        private const string CachePrefix = CacheNames.OrderPrefix + "SalesChannelsGraphDashboard_";
        private const string AllChannelsTypeId = "All";
        private const string AllChannelsName = "Все каналы";
        private readonly SalesChannelsGraphDashboardFilter _filter;

        public GetSalesChannelsGraphDashboardGrid(SalesChannelsGraphDashboardFilter filter)
        {
            _filter = filter ?? new SalesChannelsGraphDashboardFilter();
        }

        public List<SalesChannelsGraphDashboardGridModel> GetGridItems()
        {
            return CacheManager.Get($"{CachePrefix}grid{_filter.From}_{_filter.To}_{_filter.OnlyPaid}_{CurrencyService.CurrentCurrency.Iso3}", 
                GetItems);
        }

        public SalesChannelsGraphDashboardModel GetGraph()
        {
            return GetGraphData();
        }

        private SalesChannelsGraphDashboardModel GetGraphData()
        {
            var salesChannelModels = GetGridItems();

            if (_filter.SelectedSalesChannelIds == null)
                _filter.SelectedSalesChannelIds = new List<string>() {salesChannelModels[0].Id};

            var salesChannelModelData = new List<SalesChannelsGraphDashboardItem>();

            foreach (var salesChannelModel in salesChannelModels.Where(x => _filter.SelectedSalesChannelIds.Contains(x.Id)))
            {
                var group = _filter.GroupDate == EGroupDateBy.Day
                    ? "dd"
                    : (_filter.GroupDate == EGroupDateBy.Week ? "wk" : "mm");

                var sum =
                    CacheManager.Get($"{CachePrefix}graph_{salesChannelModel.Id}_{_filter.From}_{_filter.To}_{_filter.GroupDate}_{_filter.OnlyPaid}_{CurrencyService.CurrentCurrency.Iso3}",
                        () => 
                            OrderStatisticsService.GetOrdersSum(group, _filter.From, _filter.To,
                                _filter.OnlyPaid, null, true, salesChannelModel.OrderSourceIds));


                var data = new Dictionary<DateTime, float>();
                switch (_filter.GroupDate)
                {
                    case EGroupDateBy.Day:
                        data = GetByDays(sum, _filter.From, _filter.To);
                        break;
                    case EGroupDateBy.Week:
                        data = GetByWeeks(sum, _filter.From, _filter.To);
                        break;
                    case EGroupDateBy.Month:
                        data = GetByMonths(sum, _filter.From, _filter.To);
                        break;
                }

                salesChannelModelData.Add(new SalesChannelsGraphDashboardItem()
                {
                    Name = salesChannelModel.Name,
                    DateTimes = data.Keys.Select(x => x).ToList(),
                    Data = data.Values.Select(x => x).ToList(),
                });

                if (salesChannelModel.TypeId == AllChannelsTypeId)
                {
                    var profit =
                        CacheManager.Get($"{CachePrefix}graph_profit_{salesChannelModel.Id}_{_filter.From}_{_filter.To}_{_filter.GroupDate}_{_filter.OnlyPaid}_{CurrencyService.CurrentCurrency.Iso3}",
                            () => 
                                OrderStatisticsService.GetOrdersProfit(group, _filter.From, _filter.To, 
                                                                        _filter.OnlyPaid, null, 
                                                                        salesChannelModel.OrderSourceIds));
                    
                    var profitData = new Dictionary<DateTime, float>();
                    switch (_filter.GroupDate)
                    {
                        case EGroupDateBy.Day:
                            profitData = GetByDays(profit, _filter.From, _filter.To);
                            break;
                        case EGroupDateBy.Week:
                            profitData = GetByWeeks(profit, _filter.From, _filter.To);
                            break;
                        case EGroupDateBy.Month:
                            profitData = GetByMonths(profit, _filter.From, _filter.To);
                            break;
                    }

                    salesChannelModelData.Add(new SalesChannelsGraphDashboardItem()
                    {
                        Name = $"{salesChannelModel.Name} (Прибыль)",
                        DateTimes = profitData.Keys.Select(x => x).ToList(),
                        Data = profitData.Values.Select(x => x).ToList(),
                    });
                }
            }

            var graphData = new ChartDataJsonModel()
            {
                Series = salesChannelModelData.Select(x => x.Name).ToList(),
                Data = salesChannelModelData.Select(x => (object) x.Data).ToList(),
                Labels = salesChannelModelData.SelectMany(x => x.DateTimes)
                    .OrderBy(x => x)
                    .Distinct()
                    .Select(x =>x.ToString("d MMM"))
                    .ToList(),
                Colors = new List<string>() {"#2E9DEC","#71c73e", "#77b7c4", "#4dc9f6", "#f67019", "#f53794", "#537bc4", "#acc236", "#166a8f", "#00a950", "#58595b", "#8549ba"}
            };
            
            return new SalesChannelsGraphDashboardModel()
            {
                Grid = salesChannelModels,
                Graph = graphData,
                DateFrom = _filter.From.ToString("yyyy-MM-dd"),
                DateTo = _filter.To.ToString("yyyy-MM-dd"),
                GroupDate = _filter.GroupDate.ToString().ToLower()
            };
        }
        
        private List<SalesChannelsGraphDashboardGridModel> GetItems()
        {
            var list = new List<SalesChannelsGraphDashboardGridModel>();

            var modelAll = new SalesChannelsGraphDashboardGridModel()
            {
                Name = AllChannelsName,
                TypeId = AllChannelsTypeId
            };
            SetOrderStatistics(modelAll, null);
            list.Add(modelAll);

            var channel = SalesChannelService.GetByType(ESalesChannelType.Store);
            if (channel != null && channel.Enabled)
            {
                var model = new SalesChannelsGraphDashboardGridModel()
                {
                    SalesChannelType = channel.Type,
                    Icon = channel.Icon,
                    Name = "Интернет-магазин"
                };

                SetOrderStatistics(model, new List<int>()
                {
                    OrderSourceService.GetOrderSource(OrderType.ShoppingCart).Id,
                    OrderSourceService.GetOrderSource(OrderType.Mobile).Id,
                    OrderSourceService.GetOrderSource(OrderType.OneClick).Id,
                    OrderSourceService.GetOrderSource(OrderType.PreOrder).Id
                });
                
                list.Add(model);
            }

            if (!SaasDataService.IsSaasEnabled || SaasDataService.CurrentSaasData.NativeMobileApp)
            {
                var model = new SalesChannelsGraphDashboardGridModel()
                {
                    TypeId = "MobileApp",
                    Icon = "<svg width=\"40\" height=\"40\"><use xlink:href=\"../areas/admin/sales-channels.svg#mobile-app\"></use></svg>",
                    Name = "Мобильное приложение",
                };
                SetOrderStatistics(model, new List<int>() {OrderSourceService.GetOrderSource(OrderType.MobileApp).Id});
                
                if (model.OrdersCount > 0)
                    list.Add(model);
            }
            
            channel = SalesChannelService.GetByType(ESalesChannelType.Funnel);
            if (channel != null && channel.Enabled)
            {
                var funnels = new LpSiteService().GetList().Where(x => x.Enabled);

                foreach (var funnel in funnels)
                {
                    var name = OrderSourceService.PrepareOrderSourceName(OrderType.LandingPage, funnel.Name);
                    var orderSource = OrderSourceService.GetOrderSource(OrderType.LandingPage, funnel.Id) ?? OrderSourceService.GetOrderSource(name);
                    
                    if (orderSource == null)
                        continue;

                    var model = new SalesChannelsGraphDashboardGridModel()
                    {
                        SalesChannelType = ESalesChannelType.Funnel,
                        TypeId = funnel.Id.ToString(),
                        Icon = channel.Icon,
                        Name = name,
                    };
                    SetOrderStatistics(model, new List<int>() {orderSource.Id});
                    
                    list.Add(model);
                }
            }

            foreach (var moduleType in AttachedModules.GetModules<IOrderStatistics>())
            {
                var module = (IOrderStatistics) Activator.CreateInstance(moduleType);
                if (module == null)
                    continue;

                var orderSourceId = module.GetOrderSourceId();
                if (orderSourceId == null)
                    continue;

                channel = SalesChannelService.GetByType(ESalesChannelType.Module, module.ModuleStringId);

                var model = new SalesChannelsGraphDashboardGridModel()
                {
                    SalesChannelType = ESalesChannelType.Module,
                    TypeId = module.ModuleStringId,
                    Icon = channel != null ? channel.Icon : "",
                    Name = channel != null ? channel.Name : module.ModuleName
                };

                SetOrderStatistics(model, new List<int>() {orderSourceId.Value});
                
                if (model.OrdersCount > 0)
                    list.Add(model);
            }

            return list;
        }

        private void SetOrderStatistics(SalesChannelsGraphDashboardGridModel model, List<int> orderSourceIds)
        {
            model.OrderSourceIds = orderSourceIds;
            
            var statistic =
                OrderStatisticsService.GetOrdersStatistics(_filter.From, _filter.To, _filter.OnlyPaid, 
                                                false, orderSourceIds);

            if (statistic == null) 
                return;
            
            model.OrdersCount = statistic.OrdersCount;
            model.OrdersSum = statistic.OrdersSum.FormatPrice();
            model.AvgOrderSum = statistic.AvgCheck.FormatPrice();
            model.Profit = statistic.Profit.FormatPrice();
            model.ProductsCount =
                OrderStatisticsService.GetOrdersStatisticsProductsCount(_filter.From, _filter.To, _filter.OnlyPaid, 
                                                            false, orderSourceIds);
        }
    }
}