using System;
using System.Collections.Generic;
using AdvantShop.Core.Services.SalesChannels;
using AdvantShop.Web.Admin.Models.Shared.Common;
using Newtonsoft.Json;

namespace AdvantShop.Web.Admin.Models.Home
{
    public class SalesChannelsGraphDashboardModel
    {
        public List<SalesChannelsGraphDashboardGridModel> Grid { get; set; }
        
        public ChartDataJsonModel Graph { get; set; }
        
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public string GroupDate { get; set; }
    }

    public class SalesChannelsGraphDashboardGridModel
    {
        public string Id => SalesChannelType != null ? $"{Type}_{TypeId}" : TypeId;
        public ESalesChannelType? SalesChannelType { get; set; }
        public string Type => SalesChannelType.ToString();
        public string TypeId { get; set; }
        public string Icon { get; set; }
        public string Name { get; set; }
        public string OrdersSum { get; set; }
        public int OrdersCount { get; set; }
        public string AvgOrderSum { get; set; }
        public string Profit { get; set; }
        public float ProductsCount { get; set; }
        
        [JsonIgnore]
        public List<int> OrderSourceIds { get; set; }
    }

    public class SalesChannelsGraphDashboardItem
    {
        public string Name { get; set; }
        public List<DateTime> DateTimes { get; set; }
        public List<float> Data { get; set; }
    }
}