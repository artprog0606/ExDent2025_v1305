using System;
using System.Collections.Generic;
using System.Web.Mvc;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Customers;

namespace AdvantShop.Web.Admin.ViewModels.Settings
{
    public class UsersSettingsViewModel
    {
        public UsersSettingsViewModel()
        {
            ManagersOrderConstraintList = new List<SelectListItem>();
            foreach (ManagersOrderConstraint value in Enum.GetValues(typeof(ManagersOrderConstraint)))
            {
                ManagersOrderConstraintList.Add(new SelectListItem() { Text = value.Localize(), Value = ((int)value).ToString()});
            }

            ManagersLeadConstraintList = new List<SelectListItem>();
            foreach (ManagersLeadConstraint value in Enum.GetValues(typeof(ManagersLeadConstraint)))
            {
                ManagersLeadConstraintList.Add(new SelectListItem() { Text = value.Localize(), Value = ((int)value).ToString() });
            }

            ManagersCustomerConstraintList = new List<SelectListItem>();
            foreach (ManagersCustomerConstraint value in Enum.GetValues(typeof(ManagersCustomerConstraint)))
            {
                ManagersCustomerConstraintList.Add(new SelectListItem() { Text = value.Localize(), Value = ((int)value).ToString() });
            }

            ManagersTaskConstraintList = new List<SelectListItem>();
            foreach (ManagersTaskConstraint value in Enum.GetValues(typeof(ManagersTaskConstraint)))
            {
                ManagersTaskConstraintList.Add(new SelectListItem() { Text = value.Localize(), Value = ((int)value).ToString() });
            }
            
            TwoFactorAuthModules = new List<SelectListItem>();
            TwoFactorAuthModules.Add(new SelectListItem() { Text = "Выберите модуль", Value = "-1" });
            foreach (var value in AttachedModules.GetModules<ITwoFactorAuthentication>())
            {
                var classInstance = (ITwoFactorAuthentication)Activator.CreateInstance(value, null);
                TwoFactorAuthModules.Add(new SelectListItem() { Text = classInstance.ModuleName, Value = classInstance.ModuleStringId });
            }
        }

        public UsersViewModel UsersViewModel { get; set; }

        public ManagersOrderConstraint ManagersOrderConstraint { get; set; }
        public List<SelectListItem> ManagersOrderConstraintList { get; set; }


        public ManagersLeadConstraint ManagersLeadConstraint { get; set; }
        public List<SelectListItem> ManagersLeadConstraintList { get; set; }

        public ManagersCustomerConstraint ManagersCustomerConstraint { get; set; }
        public List<SelectListItem> ManagersCustomerConstraintList { get; set; }

        public ManagersTaskConstraint ManagersTaskConstraint { get; set; }
        public List<SelectListItem> ManagersTaskConstraintList { get; set; }
        
        public bool TwoFactorAuthActive { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ActiveTwoFactorAuthModule { get; set; }
        public List<SelectListItem> TwoFactorAuthModules { get; set; }
    }
}
