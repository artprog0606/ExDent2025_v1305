using System;
using AdvantShop.Areas.Api.Models.Users;
using AdvantShop.Core;
using AdvantShop.Customers;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Users
{
    public class MeUpdateFcmToken : AbstractCommandHandler<UpdateFcmTokenResponse>
    {
        private readonly Guid _customerId;
        private readonly string _token;
        private Customer _customer;

        public MeUpdateFcmToken(Guid customerId, string token)
        {
            _customerId = customerId;
            _token = token;
        }

        protected override void Validate()
        {
            _customer = CustomerContext.CurrentCustomer;
            
            if (!_customer.RegistredUser)
                throw new BlException("Пользователь не авторизован");
        }

        protected override UpdateFcmTokenResponse Handle()
        {
            CustomerService.UpdateFcmToken(_customer.Id, _token);
            return new UpdateFcmTokenResponse(true);
        }
    }
}