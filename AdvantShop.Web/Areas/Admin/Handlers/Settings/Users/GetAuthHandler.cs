using System;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Customers;

namespace AdvantShop.Web.Admin.Handlers.Settings.Users
{
    public class GetAuthHandler
    {
        private readonly string _email;
        private readonly string _password;
        private readonly string _moduleId;
        
        public GetAuthHandler(string email, string password, string moduleId)
        {
            _email = email;
            _password = password;
            _moduleId = moduleId;
        }
        
        public bool Execute()
        {
            if (_email == null || _password == null || _moduleId == null)
                return false;
            var module = AttachedModules.GetModuleById(_moduleId);
            if (module == null)
                return false;
            var customer = CustomerService.GetCustomerByEmailAndPassword(_email, _password, false);
            var moduleInstance = (ITwoFactorAuthentication)Activator.CreateInstance(module, null);
            if (customer != null && (customer.IsAdmin || customer.IsModerator)) return moduleInstance.HasUserEnabledAuthentication(customer.Id);

            return false;
        }
    }
}