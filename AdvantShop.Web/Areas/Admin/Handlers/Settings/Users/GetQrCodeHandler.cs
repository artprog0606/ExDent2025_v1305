using System;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Customers;

namespace AdvantShop.Web.Admin.Handlers.Settings.Users
{
    public class GetQrCodeHandler
    {
        private readonly string _email;
        private readonly string _password;
        private readonly string _moduleId;
        
        public GetQrCodeHandler(string email, string password, string moduleId)
        {
            _email = email;
            _password = password;
            _moduleId = moduleId;
        }
        
        public ITwoFactorAuthenticationOptions Execute()
        {
            if (_email == null || _password == null || _moduleId == null)
                return null;
            var module = AttachedModules.GetModuleById(_moduleId);
            if (module == null)
                return null;
            var customer = CustomerService.GetCustomerByEmailAndPassword(_email, _password, false);
            var moduleInstance = (ITwoFactorAuthentication)Activator.CreateInstance(module, null);
            if (customer != null && (customer.IsAdmin || customer.IsModerator)) return moduleInstance.GetCodes(customer.Id, _email);

            return null;
        }
    }
}