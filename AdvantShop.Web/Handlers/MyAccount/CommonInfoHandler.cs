using System.Linq;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Customers;
using AdvantShop.Models.MyAccount;

namespace AdvantShop.Handlers.MyAccount
{
    public class CommonInfoHandler
    {
        public UserInfoModel Get()
        {
            var customer = CustomerContext.CurrentCustomer;

            var model = new UserInfoModel
            {
                Email = customer.EMail,
                RegistrationDate = customer.RegistrationDateTime.ToString("dd.MM.yyyy"),
                Organization = customer.Organization,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Patronymic = customer.Patronymic,
                Phone = customer.Phone,
                ShowCustomerGroup = customer.CustomerGroup.CustomerGroupId != CustomerGroupService.DefaultCustomerGroup,
                CustomerGroup = customer.CustomerGroup.GroupName,
                CustomerFields = CustomerFieldService.GetCustomerFieldsWithValue(customer.Id)
                    .Where(x => (x.ShowInRegistration || x.ShowInCheckout) &&
                                (x.CustomerType == customer.CustomerType || x.CustomerType == CustomerType.All))
                    .ToList(),
                BirthDay = customer.BirthDay,
                CustomerTypeEntity = customer.CustomerType,
                IsEnabledBothTypesOfCustomers = SettingsCustomers.IsRegistrationAsLegalEntity &&
                                                SettingsCustomers.IsRegistrationAsPhysicalEntity,
                IsAgreeForPromotionalNewsletter = customer.IsAgreeForPromotionalNewsletter
            };
            
            switch (customer.CustomerRole)
            {
                case Role.User:
                    model.CustomerType = LocalizationService.GetResource("MyAccount.CommonInfo.User");
                    model.ShowCustomerRole = false;
                    break;

                case Role.Moderator:
                    model.CustomerType = LocalizationService.GetResource("MyAccount.CommonInfo.Moderator");
                    model.ShowCustomerRole = true;
                    break;

                case Role.Administrator:
                    model.CustomerType = LocalizationService.GetResource("MyAccount.CommonInfo.Administrator");
                    model.ShowCustomerRole = true;
                    break;
            }

            return model;
        }
    }
}