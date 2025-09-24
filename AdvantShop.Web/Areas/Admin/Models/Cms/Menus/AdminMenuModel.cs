using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Configuration;
using AdvantShop.Core.Services.SalesChannels;
using AdvantShop.Customers;
using AdvantShop.Saas;
using AdvantShop.Shipping.RussianPost.Api;
using AdvantShop.Track;

namespace AdvantShop.Web.Admin.Models.Cms.Menus
{
    [Serializable]
    public class AdminGroupMenuModel
    {
        public string Name { get; set; }
        public List<AdminMenuModel> Menu { get; set; }
    }

    [Serializable]
    public class AdminMenuModel
    {
        public string Controller { get; set; }
        public string Action { get; set; }
        public Dictionary<string, object> Route { get; set; }
        
        public RouteValueDictionary RouteDictionary { get { return Route != null ? new RouteValueDictionary(Route) : null; } }

        public string Name { get; set; }
        public string MobileName { get; set; }
        public string Class { get; set; }

        /// <summary>
        /// Роли для модератора
        /// roles - ["None"] - разрешено всем модераторам
        /// roles - [] или null - только админу
        /// roles - ["DisplayCatalog", "DisplayMainPageBestsellers"] - если есть одна из ролей, то разрешено
        /// </summary>
        public List<RoleAction> Roles { get; set; }

        public bool Visible { get; private set; }
        
        public bool HideChildsInLeftMenu { get; set; }
        public bool ShowChildsInNavMenu { get; set; }
        public bool IsHidden { get; set; }
        public bool IsHiddenMobile { get; set; }
        public bool Selected { get; set; }
        public bool ActiveInSaas { get; set; }
        public bool ActiveBySetting { get; set; }

        public ESaasProperty? SaasFeature { get; set; }
        public ETrackEvent? TrackEvent { get; set; }
        public EProviderSetting[] SettingValue { get; set; }
        public List<string> InstancePropToCheckAccess { get; set; }

        /// <summary>
        /// Если включена экспериментальная фича, то показываем пункт меню
        /// </summary>
        public EFeature? ExpFeature { get; set; }

        /// <summary>
        /// Если включена экспериментальная фича, то скрываем пункт меню
        /// </summary>
        public EFeature? ExpFeatureHide { get; set; }


        private bool? _activeByInstanceProp;
        public bool ActiveByInstanceProp
        {
            get
            {
                if (!_activeByInstanceProp.HasValue)
                    _activeByInstanceProp = InstancePropToCheckAccess == null ||
                                            InstancePropToCheckAccess.All(CheckAccessByProperty);

                return _activeByInstanceProp.Value;
            }
        }

        public List<AdminMenuModel> MenuItems { get; set; }

        public string Icon { get; set; }
        public string IconContent { get; set; }
        public string StatisticsDataType { get; set; }

        public AdminMenuModel ChildMenuRoute { get; set; }

        public List<AdminMenuReplaceModel> MobileTrialReplaceBy { get; set; }

        public bool IsEmptyUrl()
        {
            return string.IsNullOrWhiteSpace(Controller) || string.IsNullOrWhiteSpace(Action);
        }
        
        public bool IsAccessibleToUser()
        {
            if (!ActiveBySetting)
                return false;

            if (!Visible && MenuItems != null)
            {
                Controller = null;
                Action = null;

                return MenuItems.Any(HasVisible);
            }

            if (!ActiveByInstanceProp)
                return false;

            return Visible;
        }

        public void SetVisible(AdminMenuModel menuItem, bool isMobileTemplate)
        {
            var customer = CustomerContext.CurrentCustomer;

            if (IsHidden)
            {
                Visible = false;
            }
            //else if (IsHiddenMobile && isMobileTemplate)
            //{
            //    Visible = false;
            //}
            else if (customer.IsAdmin) //  || customer.IsVirtual
            {
                Visible = true;
            }
            else if (Roles == null || Roles.Count == 0)
            {
                Visible = false;
            }
            else
            {
                Visible = Roles.Any(role => role == RoleAction.None || RoleActionService.GetCustomerRoleActionsByCustomerId(customer.Id).Any(item => item.Role == role));
            }

            if (ExpFeature != null && FeaturesService.IsEnabled(ExpFeature.Value))
                Visible = true;

            if (Visible && ExpFeatureHide != null && FeaturesService.IsEnabled(ExpFeatureHide.Value))
                Visible = false;

            if (menuItem.MenuItems != null)
            {
                foreach (var item in menuItem.MenuItems)
                {
                    item.SetVisible(item, isMobileTemplate);
                }
            }
        }

        public bool SetSelected(string action, string controller)
        {
            if (Controller == null || Action == null)
            {
                return false;
            }

            if (MenuItems != null)
            {
                foreach (var item in MenuItems)
                {
                    if (item.SetSelected(action, controller))
                    {
                        Selected = true;
                        return true;
                    }
                }
            }

            if (Controller.ToLower() == controller && Action.ToLower() == action)
            {
                Selected = true;
                return true;
            }

            return false;
        }

        public void SetActiveInSaas(AdminMenuModel menuItem)
        {
            ActiveInSaas = true;
            if (SaasFeature.HasValue)
            {
                ActiveInSaas &= SaasDataService.IsEnabledFeature(SaasFeature.Value);
            }

            if (menuItem.MenuItems != null)
            {
                foreach (var item in MenuItems)
                {
                    item.SetActiveInSaas(item);                
                }
            }
        }

        public void SetActiveBySetting(AdminMenuModel menuItem)
        {
            ActiveBySetting = true;
            if (SettingValue != null && SettingValue.Length > 0)
            {
                ActiveBySetting = false;
                foreach (var settignValue in SettingValue.Select(SettingProvider.GetSqlSettingValue))
                {
                    ActiveBySetting |= string.IsNullOrEmpty(settignValue) || settignValue.TryParseBool();
                }
            }

            if (menuItem.MenuItems != null)
            {
                foreach (var item in MenuItems)
                {
                    item.SetActiveBySetting(item);                
                }
            }
        }

        private bool HasVisible(AdminMenuModel item)
        {
            if (item.MenuItems == null)
                return item.Visible;

            foreach (var subItem in item.MenuItems)
            {
                if (subItem.Visible)
                    return true;
                if (subItem.MenuItems != null && subItem.MenuItems.Count > 0 && HasVisible(subItem))
                    return true;
            }
            return false;
        }

        private static bool CheckAccessByProperty(string instancePropToCheckAccess)
        {
            bool propValue = false;
            string typeString, propertyName;

            var commaIndex = instancePropToCheckAccess.IndexOf(',');
            if (commaIndex == -1)
            {
                var lastDot = instancePropToCheckAccess.LastIndexOf('.');
                typeString = instancePropToCheckAccess.Substring(0, lastDot);
                propertyName = instancePropToCheckAccess.Remove(0, lastDot + 1);
            }
            else
            {
                var lastDot = instancePropToCheckAccess.LastIndexOf('.', commaIndex);
                typeString = instancePropToCheckAccess.Remove(lastDot, commaIndex - lastDot);
                propertyName = instancePropToCheckAccess.Substring(lastDot + 1, commaIndex - lastDot - 1);
            }

            var type = Type.GetType(typeString);
            if (type != null)
            {
                if (type.IsClass && !type.IsAbstract)
                {
                    var property = type.GetProperties().FirstOrDefault(x => x.Name == propertyName);
                    if (property != null)
                    {
                        if (property.PropertyType == typeof (bool))
                        {
                            var clsObj = Activator.CreateInstance(type, null);
                            propValue = (bool) property.GetValue(clsObj);
                        }
                        else throw new Exception("Property is not a Boolean type. " + instancePropToCheckAccess);
                    }
                    else throw new Exception("Property not found. " + instancePropToCheckAccess);
                }
                else throw new Exception("Type is not a class or is abstract. " + instancePropToCheckAccess);
            }
            else throw new TypeLoadException("Type is null. " + instancePropToCheckAccess);

            return propValue;
        }

        public static AdminMenuModel FromSalesChannel(SalesChannel salesChannel)
        {
            if (salesChannel == null)
                return null;

            if (salesChannel.MenuUrlAction == null)
            {
                var parts = salesChannel.Url.DefaultOrEmpty().Split("/");
                salesChannel.MenuUrlAction = new SalesChannelMenuUrlAction
                {
                    Controller = parts.Length > 0 ? parts[0] : null,
                    Action = parts.Length > 1 ? parts[1] : null,
                    RouteValues = parts.Length > 2 ? new Dictionary<string, object> { { "id", parts[2] } } : null,
                };
            }
            return new AdminMenuModel
            {
                Controller = salesChannel.MenuUrlAction.Controller,
                Action = salesChannel.MenuUrlAction.Action,
                Route = salesChannel.MenuUrlAction.RouteValues,
                Name = salesChannel.MenuName,
                MobileName = salesChannel.MobileName,
                //Roles = new List<RoleAction> { salesChannel.Role },
                ShowChildsInNavMenu = salesChannel.ShowMenuItemsInNavMenu,
                HideChildsInLeftMenu = salesChannel.HideMenuItemsInLeftMenu,
                IsHiddenMobile = salesChannel.IsHiddenMobile,
                Selected = salesChannel.Selected,
                SaasFeature = salesChannel.SaasFeature,
                //SettingValue = new[] { salesChannel.SettingValue },
                ExpFeature = salesChannel.ExpFeature,
                ExpFeatureHide = salesChannel.ExpFeatureHide,
                IconContent = salesChannel.MenuIcon,
                Visible = true,
                ActiveInSaas = true,
                ActiveBySetting = true
            };
        }
    }
}