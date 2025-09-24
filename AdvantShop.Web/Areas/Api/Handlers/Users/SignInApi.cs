using AdvantShop.Areas.Api.Models.Customers;
using AdvantShop.Areas.Api.Models.Users;
using AdvantShop.Areas.Api.Services;
using AdvantShop.Core;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Users
{
    public class SignInApi : AbstractCommandHandler<SignInResponse>
    {
        private readonly SignInModel _model;

        public SignInApi(SignInModel model)
        {
            _model = model;
        }

        protected override void Validate()
        {
            if (string.IsNullOrWhiteSpace(_model.Email) || string.IsNullOrWhiteSpace(_model.Password))
                throw new BlException("Укажите эл. почту и пароль");
        }

        protected override SignInResponse Handle()
        {
            if (!new ApiAuthorizeService().SignIn(_model.Email.Trim(), _model.Password.Trim(), out var customer, out string userKey, out string userId))
                throw new BlException("Не верный логин или пароль");

            return new SignInResponse()
            {
                UserKey = userKey,
                UserId = userId,
                Customer = new GetCustomerResponse(customer)
            };
        }
    }
}