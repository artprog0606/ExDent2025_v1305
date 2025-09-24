using System;
using AdvantShop.Core.Services.Api;

namespace AdvantShop.Areas.Api.Models.Users
{
    public class AddUpdateContactResponse : IApiResponse
    {
        public AddUpdateContactResponse(Guid contactId)
        {
            ContactId = contactId;
        }
        
        public Guid ContactId { get; }
    }
}