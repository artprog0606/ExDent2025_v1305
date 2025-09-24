//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Web;
using AdvantShop.Helpers;
using AdvantShop.Security;
using System.Collections.Generic;
using System.Web.Caching;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;

namespace AdvantShop.Customers
{
    public static class CustomerContext
    {
        private const string CustomerContextKey = "CustomerContext";
        private const string CustomerCookieName = "customer";
        private const string IsDebugCookieName = "isDebug";
        private const string DontDisturbByNotifyCookieName = "dontDisturbByNotify";
        
        public static Customer CurrentCustomer
        {
            get
            {
                if (HttpContext.Current == null)
                    return null;

                var cachedCustomer = HttpContext.Current.Items[CustomerContextKey] as Customer;
                if (cachedCustomer != null)
                    return cachedCustomer;

                //registered user
                var customer = AuthorizeService.GetAuthenticatedCustomer();
                CustomerType customerType;

                if (SettingsCustomers.IsRegistrationAsLegalEntity && SettingsCustomers.IsRegistrationAsPhysicalEntity == false)
                {
                    customerType = CustomerType.LegalEntity;
                }
                else
                {
                    customerType = CustomerType.PhysicalEntity;
                }
                
                //load guest customer
                if (customer == null)
                {
                    var customerCookie = CommonHelper.GetCookie(CustomerCookieName);
                    if (customerCookie != null && !String.IsNullOrEmpty(customerCookie.Value))
                    {
                        Guid customerGuid;
                        if (Guid.TryParse(customerCookie.Value, out customerGuid) &&
                            !CustomerService.ExistsCustomer(customerGuid))
                        {
                            customer = new Customer { Id = customerGuid, CustomerRole = Role.Guest, CustomerType = customerType};
                        }
                    }
                }

                //create guest if not exists
                if (customer == null)
                {
                    var customerId = Guid.NewGuid();
                    while (CustomerService.ExistsCustomer(customerId))
                        customerId = Guid.NewGuid();

                    customer = new Customer { Id = customerId, CustomerRole = Role.Guest, CustomerType = customerType };
                    SetCustomerCookie(customer.Id);
                }

                if (IsDebug)
                {
                    customer.IsVirtual = true;
                    customer.Enabled = true;
                    customer.CustomerRole = Role.Administrator;
                }

                HttpContext.Current.Items[CustomerContextKey] = customer;
                return customer;
            }
        }

        public static Guid CustomerId => CurrentCustomer.Id;

        public static bool IsDebug
        {
            get
            {
                var cookie = CommonHelper.GetCookie(IsDebugCookieName);
                if (cookie == null || string.IsNullOrEmpty(cookie.Value))
                    return false;

                var isDebugList = CacheManager.Get<List<Guid>>(CacheNames.IsDebug);

                return isDebugList != null ? isDebugList.Contains(Guid.Parse(cookie.Value)) : false;
            }
            set
            {
                if (value)
                {
                    var debugSessionId = Guid.NewGuid();
                    var isDebugList = CacheManager.Get<List<Guid>>(CacheNames.IsDebug) ?? new List<Guid>();

                    if (!isDebugList.Contains(debugSessionId))
                    {
                        isDebugList.Add(debugSessionId);
                        if (CommonHelper.GetCookie(IsDebugCookieName) != null)
                            CommonHelper.DeleteCookie(IsDebugCookieName);

                        CommonHelper.SetCookie(IsDebugCookieName, debugSessionId.ToString(), new TimeSpan(0, 0, 20, 0), false);
                    }
                    CacheManager.Insert(CacheNames.IsDebug, isDebugList, 20, null, CacheItemPriority.AboveNormal);
                }
                else
                {
                    var cookie = CommonHelper.GetCookie(IsDebugCookieName);

                    if (cookie != null && !string.IsNullOrEmpty(cookie.Value))
                    {
                        var debugSessionId = Guid.Parse(cookie.Value);
                        var isDebugList = CacheManager.Get<List<Guid>>(CacheNames.IsDebug) ?? new List<Guid>();

                        if (isDebugList.Contains(debugSessionId))
                            isDebugList.Remove(debugSessionId);

                        CacheManager.Remove(CacheNames.IsDebug);
                        CacheManager.Insert(CacheNames.IsDebug, isDebugList, 20, null, CacheItemPriority.AboveNormal);

                        CommonHelper.DeleteCookie(IsDebugCookieName);
                    }
                }
            }
        }

        public static void SetCustomerCookie(Guid userId)
        {
            CommonHelper.SetCookie(CustomerCookieName, userId.ToString(), new TimeSpan(90, 0, 0, 0), httpOnly: true, setOnMainDomain: true);
        }

        public static void DeleteCustomerCookie()
        {
            CommonHelper.DeleteCookie(CustomerCookieName);
        }

        public static void SetDontDisturbByNotifyCookie(TimeSpan ttl)
        {
            CommonHelper.SetCookie(DontDisturbByNotifyCookieName, "true", ttl, false);
        }

        public static void DeleteDontDisturbByNotifyCookie()
        {
            CommonHelper.DeleteCookie(DontDisturbByNotifyCookieName);
        }
    }
}