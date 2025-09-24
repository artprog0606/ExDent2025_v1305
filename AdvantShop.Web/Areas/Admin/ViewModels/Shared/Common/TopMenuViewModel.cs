using System;
using System.Collections.Generic;
using AdvantShop.Core.Services.SalesChannels;
using AdvantShop.Web.Admin.Models.Cms.Menus;
using CmsMenuItemModel = AdvantShop.Core.Services.CMS.MenuItemModel;

namespace AdvantShop.Web.Admin.ViewModels.Shared.Common
{
    public class TopMenuViewModel
    {
        public bool DisplayCatalog { get; set; }
        public bool DisplayCustomers { get; set; }
        public bool DisplayOrders { get; set; }
        public bool DisplayCrm { get; set; }
        public bool DisplayTasks { get; set; }
        public bool DisplayBooking { get; set; }
        public bool DisplayCms { get; set; }
        public bool DisplaySettings { get; set; }
        public bool IsDashboard { get; set; }

        public bool ShowAddMenu
        {
            get
            {
                return DisplayCatalog || DisplayCustomers || DisplayOrders || DisplayCrm || DisplayCms || DisplaySettings;
            }
        }

        public List<SalesChannel> SalesChannelsMenuItems { get; set; }
    }
}
