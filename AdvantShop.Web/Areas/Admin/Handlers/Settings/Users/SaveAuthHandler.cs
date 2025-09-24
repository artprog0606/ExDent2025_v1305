using System;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Customers;

namespace AdvantShop.Web.Admin.Handlers.Settings.Users
{
    public class SaveAuthHandler
    {
        private readonly string _email;
        private readonly string _password;
        private readonly string _moduleId;
        private readonly bool _twoFactorAuthActive;
        
        public SaveAuthHandler(string email, string password, string moduleId, bool twoFactorAuthActive)
        {
            _email = email;
            _password = password;
            _moduleId = moduleId;
            _twoFactorAuthActive = twoFactorAuthActive;
        }
        
        public void Execute()
        {
            if (_email == null || _password == null || _moduleId == null)
                return;
            var module = AttachedModules.GetModuleById(_moduleId);
            if (module == null)
                return;
            var customer = CustomerService.GetCustomerByEmailAndPassword(_email, _password, false);
            var moduleInstance = (ITwoFactorAuthentication)Activator.CreateInstance(module, null);
            if (customer != null && (customer.IsAdmin || customer.IsModerator)) moduleInstance.SaveUserAuthenticationEnabled(customer.Id, _twoFactorAuthActive);
        }
    }
}