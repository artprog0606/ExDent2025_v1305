using AdvantShop.Areas.Api.Models.Preorder;
using AdvantShop.CMS;
using AdvantShop.Configuration;
using AdvantShop.Customers;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Preorder
{
    public sealed class GetPreorderSettings : AbstractCommandHandler<PreorderSettingsResponse>
    {
        protected override PreorderSettingsResponse Handle()
        {
            var customer = CustomerContext.CurrentCustomer;
            
            return new PreorderSettingsResponse()
            {
                Customer = new PreorderSettingsCustomer
                {
                    FirstName = customer.RegistredUser ? customer.FirstName : null,
                    LastName = customer.RegistredUser ? customer.LastName : null,
                    Email = customer.RegistredUser ? customer.EMail : null,
                    Phone = customer.RegistredUser ? customer.Phone : null
                },
                Settings = new PreorderSettings
                {
                    IsShowUserAgreementText = SettingsCheckout.IsShowUserAgreementText,
                    UserAgreementText = SettingsCheckout.UserAgreementText,
                    AgreementDefaultChecked = SettingsCheckout.AgreementDefaultChecked,
                    PreOrderText = StaticBlockService.GetPagePartByKeyWithCache("requestOnProduct").Content
                }
            };
        }
    }
}