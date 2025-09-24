using AdvantShop.Core;
using AdvantShop.Core.Services.Partners;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Partners
{
    public class ChangePasswordHandler : AbstractCommandHandler
    {
        private readonly int _id;
        private readonly string _password;
        private readonly string _passwordConfirm;
        private Partner _partner;

        public ChangePasswordHandler(int id, string password, string passwordConfirm)
        {
            _id = id;
            _password = password;
            _passwordConfirm = passwordConfirm;
        }

        protected override void Load()
        {
            _partner = PartnerService.GetPartner(_id);
        }

        protected override void Validate()
        {
            if (_partner == null)
                throw new BlException(T("Admin.Partner.Errors.NotFound"));
            if (string.IsNullOrWhiteSpace(_password) || _password != _passwordConfirm || _password.Length < 6)
                throw new BlException(T("Admin.Customers.Password6Chars"));
        }

        protected override void Handle()
        {
            PartnerService.ChangePassword(_id, _password, false);
        }
    }
}
