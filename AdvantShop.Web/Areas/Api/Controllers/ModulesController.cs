using System;
using System.Web.Mvc;
using AdvantShop.Areas.Api.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.MobileApp;
using AdvantShop.Security;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Areas.Api.Controllers
{
    [LogRequest, AuthApiKeyByUser]
    public class ModulesController : BaseApiController
    {
        // GET api/modules/{id}/block
        [ExcludeFilter(typeof(TechDomainGuardAttribute))]
        [AuthUserApi(UseCookie = true)]
        [HttpGet]
        public ActionResult GetBlock(string id)
        {
            var cookie = CommonHelper.GetCookie(MobileAppConst.CookieName);
            if (cookie == null || string.IsNullOrEmpty(cookie.Value))
            {
                CommonHelper.SetCookie(MobileAppConst.CookieName, "true", new TimeSpan(90, 0, 0, 0), true);
            }

            var customer = CustomerContext.CurrentCustomer;
            if (customer.RegistredUser)
            {
                if (string.IsNullOrEmpty(customer.Password))
                {
                    var password = StringHelper.GeneratePassword(8);
                    customer.Password = SecurityHelper.GetPasswordHash(password);

                    CustomerService.UpdateCustomerPassword(customer.Id, customer.Password);
                }

                if (customer.EMail.IsNotEmpty())
                    AuthorizeService.SignIn(customer.EMail, customer.Password, true, true);
                else if (customer.StandardPhone != null)
                    AuthorizeService.SignInByPhone(customer.StandardPhone, customer.Password, true, true);
            }
            else
                CustomerContext.SetCustomerCookie(customer.Id);
            
            try
            {
                var moduleType = AttachedModules.GetModuleById(id, true);
                if (moduleType == null)
                    return Error404(); //throw new BlException("Модуль не найден");
                
                var module = Activator.CreateInstance(moduleType);
                if (module == null)
                    return Error404();
                
                var mobileAppRenderBlock = module as IMobileAppRenderBlock;
                if (mobileAppRenderBlock == null)
                    return Error404();

                var route = mobileAppRenderBlock.GetMobileAppBlockRoute();
                if (route == null)
                    return Error404();
                
                if (route.IsSimpleText)
                    return new ContentResult() { Content = route.Content };
                
                return RedirectToAction(route.ActionName, route.ControllerName, route.RouteValues);
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            return new EmptyResult();
        }
    }
}