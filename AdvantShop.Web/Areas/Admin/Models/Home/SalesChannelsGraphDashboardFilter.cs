using System;
using System.Collections.Generic;
using AdvantShop.Core.Services.Statistic;

namespace AdvantShop.Web.Admin.Models.Home
{
    public class SalesChannelsGraphDashboardFilter
    {
        public EGroupDateBy GroupDate { get; set; }
        
        public DateTime From { get; set; }
        
        public DateTime To { get; set; }
        
        public List<string> SelectedSalesChannelIds { get; set; }
        
        public bool? OnlyPaid { get; set; }

        public SalesChannelsGraphDashboardFilter()
        {
            GroupDate = EGroupDateBy.Day;
            From = DateTime.Now.AddDays(-7);
            To = DateTime.Now;
        }
    }
}