using AdvantShop.Core.Services.Crm.OK.Domain;
using System;
using System.Linq;

namespace AdvantShop.Core.Services.Crm.Ok
{
    public class OkUser
    {
        public string Id { get; set; }
        public Guid CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Photo { get; set; }

        public OkUser() { }
        public OkUser(OkWebhookSender user)
        {
            this.Id = user.UserID.Split(':').Last();
            if (user.Name != null)
            {
                var name = user.Name.Split(' ');
                this.FirstName = name[0];
                this.LastName = name[1];
            }
        }
    }
}