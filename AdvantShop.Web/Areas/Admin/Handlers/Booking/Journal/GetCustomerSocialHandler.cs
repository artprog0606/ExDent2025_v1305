using System;
using AdvantShop.Core.Services.Crm.Facebook;
using AdvantShop.Core.Services.Crm.Instagram;
using AdvantShop.Core.Services.Crm.Vk;
using AdvantShop.Web.Admin.Models.Booking.Journal;

namespace AdvantShop.Web.Admin.Handlers.Booking.Journal
{
    public class GetCustomerSocialHandler
    {
        private readonly Guid _customerId;

        public GetCustomerSocialHandler(Guid customerId)
        {
            _customerId = customerId;
        }

        public BookingCustomerSocial Execute()
        {
            var model = new BookingCustomerSocial
            {
                VkUser = VkService.GetUser(_customerId),
                InstagramUser = InstagramService.GetUserByCustomerId(_customerId),
                FacebookUser = FacebookService.GetUser(_customerId)
            };

            return model;
        }
    }
}
