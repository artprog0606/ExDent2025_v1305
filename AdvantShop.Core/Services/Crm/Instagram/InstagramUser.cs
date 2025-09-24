using System;
using AdvantShop.Customers;
using AdvantShop.Helpers;

namespace AdvantShop.Core.Services.Crm.Instagram
{
    public class InstagramUser
    {
        public string Pk { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string ProfilePicture { get; set; }


        public Guid CustomerId { get; set; }
    }

    public static class InstagramUserExtensions
    {
        public static Customer ToCustomer(this InstagramUser user)
        {
            return new Customer(CustomerGroupService.DefaultCustomerGroup)
            {
                FirstName = !string.IsNullOrEmpty(user.FullName) ? user.FullName : user.UserName,
                EMail = user.Email,
                Phone = user.PhoneNumber,
                StandardPhone =
                    !string.IsNullOrEmpty(user.PhoneNumber)
                        ? StringHelper.ConvertToStandardPhone(user.PhoneNumber)
                        : null,
                IsAgreeForPromotionalNewsletter = false
            };
        }
    }
}
