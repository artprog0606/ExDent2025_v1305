using System;
using AdvantShop.Core.Common.Attributes;

namespace AdvantShop.Customers
{
    public enum Role
    {
        [Localize("Core.Customers.Role.User")]
        User = 0,
        [Localize("Core.Customers.Role.Moderator")]
        Moderator = 50,
        [Localize("Core.Customers.Role.Administrator")]
        Administrator = 100,
        [Localize("Core.Customers.Role.Guest")]
        Guest = 150
    }
    
    public enum RoleAction
    {
        [Localize("Core.Customers.RoleActionCategory.None")]
        None,

        [Localize("Core.Customers.RoleActionCategory.Orders")]
        Orders,

        [Localize("Core.Customers.RoleActionCategory.Crm")]
        Crm,

        [Localize("Core.Customers.RoleActionCategory.Customers")]
        Customers,

        [Localize("Core.Customers.RoleActionCategory.Catalog")]
        Catalog,

        [Localize("Core.Customers.RoleActionCategory.Tasks")]
        Tasks,

        [Localize("Core.Customers.RoleActionCategory.Booking")]
        Booking,

        //[Localize("Core.Customers.RoleActionCategory.Marketing")]
        //Marketing,

        [Localize("Core.Customers.RoleActionCategory.Modules")]
        Modules,
        
        [Localize("Core.Customers.RoleActionCategory.Settings")]
        Settings,

        // new 
        [Localize("Core.Customers.RoleActionCategory.Store")]
        Store,

        [Localize("Core.Customers.RoleActionCategory.Landing")]
        Landing,

        [Localize("Core.Customers.RoleActionCategory.Triggers")]
        Triggers,

        [Localize("Core.Customers.RoleActionCategory.Yandex")]
        Yandex,

        [Localize("Core.Customers.RoleActionCategory.Avito")]
        Avito,

        [Localize("Core.Customers.RoleActionCategory.Google")]
        Google,

        [Localize("Core.Customers.RoleActionCategory.Reseller")]
        Reseller,

        [Localize("Core.Customers.RoleActionCategory.Vk")]
        Vk,

        [Localize("Core.Customers.RoleActionCategory.Ok")]
        Ok,

        [Localize("Core.Customers.RoleActionCategory.Telegram")]
        Telegram,

        [Localize("Core.Customers.RoleActionCategory.BonusSystem")]
        BonusSystem,

        [Localize("Core.Customers.RoleActionCategory.Partners")]
        Partners,

        [Localize("Core.Customers.RoleActionCategory.Instagram")]
        Instagram,

        //[Localize("Core.Customers.RoleActionCategory.Facebook")]
        //Facebook,

        [Localize("Core.Customers.RoleActionCategory.FacebookFeed")]
        FacebookFeed,

        [Localize("Core.Customers.RoleActionCategory.Analytics")]
        Analytics,

        [Localize("Core.Customers.RoleActionCategory.Desktop")]
        Desktop,
        
        [Localize("Core.Customers.RoleActionCategory.CouponsAndDiscounts")]
        CouponsAndDiscounts,
        
        [Localize("Core.Customers.RoleActionCategory.Api")]
        Api,
    }

    public class CustomerRoleAction
    {
        public CustomerRoleAction()
        {
            CustomerId = Guid.Empty;
        }

        public Guid CustomerId { get; set; }        
        public RoleAction Role { get; set; }
    }

    /// <summary>
    /// Какие заказы может видеть менеджер 
    /// </summary>
    public enum ManagersOrderConstraint
    {
        /// <summary>
        /// Все заказы
        /// </summary>
        [Localize("Core.Customers.ManagersOrderConstraint.All")]
        All = 0,

        /// <summary>
        /// Только назначенные заказы
        /// </summary>
        [Localize("Core.Customers.ManagersOrderConstraint.Assigned")]
        Assigned = 1,

        /// <summary>
        /// Назначенные и свободные заказы
        /// </summary>
        [Localize("Core.Customers.ManagersOrderConstraint.AssignedAndFree")]
        AssignedAndFree = 2
    }

    /// <summary>
    /// Какие лиды может видеть менеджер
    /// </summary>
    public enum ManagersLeadConstraint
    {
        [Localize("Core.Customers.ManagersLeadConstraint.All")]
        All = 0,

        [Localize("Core.Customers.ManagersLeadConstraint.Assigned")]
        Assigned = 1,

        [Localize("Core.Customers.ManagersLeadConstraint.AssignedAndFree")]
        AssignedAndFree = 2
    }

    /// <summary>
    /// Каких пользователей может видеть менеджер
    /// </summary>
    public enum ManagersCustomerConstraint
    {
        [Localize("Core.Customers.ManagersCustomerConstraint.All")]
        All = 0,

        [Localize("Core.Customers.ManagersCustomerConstraint.Assigned")]
        Assigned = 1,

        [Localize("Core.Customers.ManagersCustomerConstraint.AssignedAndFree")]
        AssignedAndFree = 2
    }

    /// <summary>
    /// Какие задачи может видеть менеджер
    /// </summary>
    public enum ManagersTaskConstraint
    {
        [Localize("Core.Customers.ManagersTaskConstraint.All")]
        All = 0,

        [Localize("Core.Customers.ManagersTaskConstraint.Assigned")]
        Assigned = 1,

        [Localize("Core.Customers.ManagersTaskConstraint.AssignedAndFree")]
        AssignedAndFree = 2
    }
}