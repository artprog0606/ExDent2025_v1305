using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.SalesChannels;
using AdvantShop.Web.Admin.Models.Cms.Menus;
using CmsMenuItemModel = AdvantShop.Core.Services.CMS.MenuItemModel;

namespace AdvantShop.Web.Admin.ViewModels.Shared.Common
{
    public class LeftMenuViewModel
    {
        public LeftMenuViewModel()
        {
            MenuItems = new List<AdminGroupMenuModel>();
            DisplayCatalog = true;
            DisplayCustomers = true;
            DisplayOrders = true;
            DisplayCrm = true;
            CustomMenuItems = new List<CmsMenuItemModel>();
            NumberLimitationMenuItem = 0;
        }

        public List<AdminGroupMenuModel> MenuItems { get; set; }
        public List<CmsMenuItemModel> CustomMenuItems { get; set; }

        public Guid CustomerId { get; set; }
        public string AvatarSrc { get; set; }
        public string NoAvatarSrc { get; set; }

        public bool DisplayCatalog { get; set; }
        public bool DisplayCustomers { get; set; }
        public bool DisplayOrders { get; set; }
        public bool DisplayCrm { get; set; }
        public bool DisplayCms { get; set; }
        public bool DisplaySettings { get; set; }

        public bool ShowAddMenu
        {
            get
            {
                return DisplayCatalog || DisplayCustomers || DisplayOrders || DisplayCrm || DisplayCms || DisplaySettings;
            }
        }

        public List<SalesChannel> SalesChannelsMenuItems { get; set; }
        public bool IsDashBoard { get; set; }
        public bool IsMobile { get; set; }
        public int NumberLimitationMenuItem { get; set; }
        
        public void ReplaceItems(bool isMobile = false)
        {
            if (!Trial.TrialService.IsTrialEnabled)
                return;

            foreach (var group in MenuItems)
            {
                for (int i = 0; i < group.Menu.Count; i++)
                {
                    if (isMobile && group.Menu[i].MobileTrialReplaceBy != null && group.Menu[i].MobileTrialReplaceBy.Any())
                    {
                        foreach (var replaceBy in group.Menu[i].MobileTrialReplaceBy)
                        {
                            AdminMenuModel menuItemReplace = null;
                            if (replaceBy.Module.IsNotEmpty())
                            {
                                var salesChannel = SalesChannelsMenuItems.FirstOrDefault(channel => channel.Enabled && channel.ModuleStringId == replaceBy.Module);
                                menuItemReplace = AdminMenuModel.FromSalesChannel(salesChannel);
                            }
                            else if (replaceBy.SalesChannel.IsNotEmpty())
                            {
                                var salesChannel = SalesChannelsMenuItems.FirstOrDefault(channel => channel.Enabled && channel.Type.ToString() == replaceBy.SalesChannel);
                                menuItemReplace = AdminMenuModel.FromSalesChannel(salesChannel);
                            }
                            else if (replaceBy.MenuItem != null)
                            {
                                var menuItem = MenuItems.SelectMany(x => x.Menu)
                                    .FirstOrDefault(x => x.Action == replaceBy.MenuItem.Action && x.Controller == replaceBy.MenuItem.Controller);
                                menuItemReplace = menuItem;
                            }

                            if (menuItemReplace != null)
                            {
                                group.Menu[i] = menuItemReplace;
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}
