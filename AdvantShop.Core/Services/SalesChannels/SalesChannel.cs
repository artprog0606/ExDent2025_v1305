using System;
using System.Collections.Generic;
using System.Web.Routing;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Scheduler;
using AdvantShop.Core.Services.Configuration;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Customers;
using AdvantShop.Saas;

namespace AdvantShop.Core.Services.SalesChannels
{
    [Serializable]
    public class SalesChannel : ISalesChannelModule
    {
        public SalesChannel()
        {
        }

        public SalesChannel(SalesChannelConfigModel model)
        {
            Type = model.Type;
            Name = LocalizationService.GetResource(model.Name);
            MobileName = LocalizationService.GetResource(model.MobileName ?? "");
            Description = LocalizationService.GetResource(model.Description);
            Icon = model.Icon;
            Role = model.Role;
            SaasFeature = model.SaasFeature;
            PlanNameSaasFeatureAvailableFrom =
                model.SaasFeatureAvailableFrom != null
                    ? SaasDataService.GetFeature(model.SaasFeatureAvailableFrom.Value)
                    : null;
            SettingValue = model.SettingValue;
            ExpFeatureHide = model.ExpFeatureHide;
            Hide = model.Hide;
            ExpFeature = model.ExpFeature;

            MenuName = LocalizationService.GetResource(model.MenuName);
            MenuIcon = model.MenuIcon;
            MenuUrlAction = model.MenuUrlAction;
            Url = model.Url;
            IsNew = model.IsNew;

            var items = new List<SalesChannelSubMenuItem>();

            if (model.MenuItems != null)
            {
                foreach (var item in model.MenuItems)
                {
                    if (item.SaasFeature != null && !SaasDataService.IsEnabledFeature(item.SaasFeature.Value))
                        continue;

                    if (item.Feature != null && !FeaturesService.IsEnabled(item.Feature.Value))
                        continue;

                    items.Add(item);
                }
            }

            MenuItems = items;
            HideMenuItemsInLeftMenu = model.HideMenuItemsInLeftMenu;
            ShowMenuItemsInNavMenu = model.ShowMenuItemsInNavMenu;

            IsHiddenMobile = model.IsHiddenMobile;

            if (MenuItems != null)
            {
                foreach (var menuItem in MenuItems)
                    menuItem.Name = menuItem.Name != null ? LocalizationService.GetResource(menuItem.Name) : null;
            }

            Details = model.Details;

            ChildMenuRoute = model.ChildMenuRoute;

            ModuleId = model.ModuleId;
            ModuleStringId = model.ModuleStringId;
            ModuleVersion = model.ModuleVersion;

            ShowInstalledAndPreview = model.ShowInstalledAndPreview;
            ShowPreview = model.ShowPreview;
            PreviewRightText = model.PreviewRightText;
            PreviewLeftText = model.PreviewLeftText;
            PreviewButtonText = model.PreviewButtonText;
        }

        public ESalesChannelType Type { get; private set; }
        public string TypeStr { get { return Type.ToString().ToLower(); } }
        public string Name { get; private set; }
        public string MobileName { get; private set; }
        public string Description { get; private set; }
        public string Url { get; set; }
        public string Icon { get; private set; }
        public RoleAction Role { get; private set; }
        public ESaasProperty? SaasFeature { get; private set; }
        public string PlanNameSaasFeatureAvailableFrom { get; private set; }
        public EProviderSetting SettingValue { get; private set; }
        public EFeature? ExpFeatureHide { get; set; }
        public bool Hide { get; set; }
        public EFeature? ExpFeature { get; set; }

        public string MenuName { get; set; }
        public string MenuIcon { get; set; }
        public SalesChannelMenuUrlAction MenuUrlAction { get; set; }
        public List<SalesChannelSubMenuItem> MenuItems { get; set; }
        public bool HideMenuItemsInLeftMenu { get; set; }
        public bool ShowMenuItemsInNavMenu { get; set; }
        public SalesChannelDetails Details { get; set; }

        public SalesChannelChildMenuRoute ChildMenuRoute { get; set; }
        public bool IsHiddenMobile { get; set; }


        public bool Enabled
        {
            get
            {
                if (Type == ESalesChannelType.Module)
                {
                    var isModuleEnabled = AttachedModules.GetModuleById(ModuleStringId, true) != null;
                    if (isModuleEnabled)
                        return true;

                    if (ShowInstalledAndPreview && !SalesChannelService.GetNotShowInstalled(ModuleStringId))
                        return true;

                    return false;
                }

                var result = SettingProvider.GetSqlSettingValue(SettingValue).TryParseBool();
                
                //JobActivationManager.SettingUpdated();
                return result;
            }
            set
            {
                if (Type == ESalesChannelType.Module)
                {
                    ModulesRepository.SetActiveModule(ModuleStringId, value);

                    if (ShowInstalledAndPreview)
                        SalesChannelService.SetNotShowInstalled(ModuleStringId, false);
                }
                else
                {
                    if (value)
                        Track.TrackService.TrackEvent(Track.ETrackEvent.SalesChannels_SalesChannelAdded, Type.ToString());

                    if (Type == ESalesChannelType.Bonus)
                        SettingProvider.SetSqlSettingValue("BonusSystem.IsActive", value.ToString());
                    
                    SettingProvider.SetSqlSettingValue(SettingValue, value.ToString());
                    JobActivationManager.SettingUpdated();
                    if (Type == ESalesChannelType.Dashboard) // пункт меню скрывается в зависимости от активности канала "Мои сайты", поэтому чистим кэш с пунктами меню
                        CacheManager.RemoveByPattern(CacheNames.AdminMenu);

                    Track.TrackService.TrackSalesChannel(Type, value);
                }
            }
        }

        public bool IsShowInMenu()
        {
            return Enabled &&
                   (RoleActionService.HasCurrentCustomerRoleAction(Role) || ModuleStringId == "YandexDirect") &&
                   (SaasFeature == null || SaasDataService.IsEnabledFeature(SaasFeature.Value)) &&
                   (ExpFeature == null || FeaturesService.IsEnabled(ExpFeature.Value)) &&
                   (ExpFeatureHide == null || !FeaturesService.IsEnabled(ExpFeatureHide.Value)) && 
                   !Hide;
        }

        public bool IsShowInList()
        {
            return
                (ExpFeature == null || FeaturesService.IsEnabled(ExpFeature.Value)) &&
                (ExpFeatureHide == null || !FeaturesService.IsEnabled(ExpFeatureHide.Value)) && 
                !Hide;
        }

        public bool Selected { get; set; }

        public bool SetSelected(string action, string controller, string url)
        {
            if (MenuUrlAction == null)
                return false;

            if (MenuItems != null)
                foreach (var item in MenuItems)
                {
                    if (item.SetSelected(action, controller))
                    {
                        Selected = true;
                        return true;
                    }
                }

            if (MenuUrlAction.Controller.ToLower() == controller && MenuUrlAction.Action.ToLower() == action)
            {
                Selected = true;
                return true;
            }

            if (!string.IsNullOrEmpty(Url) && url.EndsWith(Url))
            {
                Selected = true;
                return true;
            }

            return false;
        }

        public bool IsNew { get; set; }

        public int ModuleId { get; set; }
        public string ModuleStringId { get; set; }
        public string ModuleVersion { get; set; }

        public bool ShowInstalledAndPreview { get; set; }
        public bool ShowPreview { get; set; }
        public string PreviewRightText { get; set; }
        public string PreviewLeftText { get; set; }
        public string PreviewButtonText { get; set; }
    }

    public class SalesChannelConfigModel : ISalesChannelModule
    {
        public ESalesChannelType Type { get; set; }
        public string Name { get; set; }
        public string MobileName { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string Icon { get; set; }
        public RoleAction Role { get; set; }
        public ESaasProperty? SaasFeature { get; set; }
        public ESaasProperty? SaasFeatureAvailableFrom { get; set; }
        public EProviderSetting SettingValue { get; set; }
        public EFeature? ExpFeatureHide { get; set; }
        public EFeature? ExpFeature { get; set; }

        public bool Hide { get; set; }

        public string MenuName { get; set; }
        public string MenuIcon { get; set; }
        public SalesChannelMenuUrlAction MenuUrlAction { get; set; }
        public List<SalesChannelSubMenuItem> MenuItems { get; set; }
        public bool IsNew { get; set; }
        public SalesChannelDetails Details { get; set; }

        public SalesChannelChildMenuRoute ChildMenuRoute { get; set; }
        public bool HideMenuItemsInLeftMenu { get; set; }
        public bool ShowMenuItemsInNavMenu { get; set; }

        public int ModuleId { get; set; }
        public string ModuleStringId { get; set; }
        public string ModuleVersion { get; set; }
        public bool ShowInstalledAndPreview { get; set; }
        public bool ShowPreview { get; set; }
        public string PreviewRightText { get; set; }
        public string PreviewLeftText { get; set; }
        public string PreviewButtonText { get; set; }
        public bool IsHiddenMobile { get; set; }
    }

    [Serializable]
    public class SalesChannelMenuUrlAction
    {
        public string Action { get; set; }
        public string Controller { get; set; }
        public RouteValueDictionary RouteDictionary { get { return RouteValues != null ? new RouteValueDictionary(RouteValues) : null; } }
        public Dictionary<string, object> RouteValues { get; set; }
    }

    [Serializable]
    public class SalesChannelDetails
    {
        public List<SalesChannelPicture> Images { get; set; }
        public List<SalesChannelVideos> Videos { get; set; }

        private string _text;
        public string Text
        {
            get { return _text; }
            set { _text = LocalizationService.GetResource(value); }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set { _title = LocalizationService.GetResource(value); }
        }

        public string PriceString { get; set; }
        public float Price { get; set; }
    }

    [Serializable]
    public class SalesChannelPicture
    {
        public string Src { get; set; }
        public string Alt { get; set; }
    }
    [Serializable]
    public class SalesChannelVideos
    {
        public string Src { get; set; }
    }

    [Serializable]
    public class SalesChannelSubMenuItem
    {
        public string Name { get; set; }
        public SalesChannelMenuUrlAction MenuUrlAction { get; set; }
        public bool IsHidden { get; set; }

        public ESaasProperty? SaasFeature { get; set; }
        public EFeature? Feature { get; set; }

        public List<SalesChannelSubMenuItem> MenuItems { get; set; }

        public bool Selected { get; set; }
        public bool SetSelected(string action, string controller)
        {
            if (MenuUrlAction == null)
                return false;

            if (MenuItems != null)
                foreach (var item in MenuItems)
                {
                    if (item.SetSelected(action, controller))
                    {
                        Selected = true;
                        return true;
                    }
                }

            if (MenuUrlAction.Controller.ToLower() == controller && MenuUrlAction.Action.ToLower() == action)
            {
                Selected = true;
                return true;
            }
            return false;
        }

        public string MenuIcon { get; set; }
    }

    [Serializable]
    public class SalesChannelChildMenuRoute
    {
        public string Controller { get; set; }
        public string Action { get; set; }
        public Dictionary<string, object> Route { get; set; }
        public RouteValueDictionary RouteDictionary { get { return Route != null ? new RouteValueDictionary(Route) : null; } }
    }


    public interface ISalesChannelModule
    {
        int ModuleId { get; set; }
        string ModuleStringId { get; set; }
        string ModuleVersion { get; set; }
    }
}
