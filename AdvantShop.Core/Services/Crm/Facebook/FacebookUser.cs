using System;
using AdvantShop.Core.Services.Crm.Facebook.Models;

namespace AdvantShop.Core.Services.Crm.Facebook
{
    public class FacebookUser
    {
        public string Id { get; set; }
        
        public string FirstName { get; set; }
        
        public string LastName { get; set; }

        public string Email { get; set; }

        public string Gender { get; set; }
        
        public Guid CustomerId { get; set; }

        /// <summary>
        /// Id в мессенджере
        /// </summary>
        public string PsyId { get; set; }

        /// <summary>
        /// Аватар из мессенджера
        /// </summary>
        public string PhotoPicByPsyId { get; set; }


        public FacebookUser() { }

        public FacebookUser(FbUser user)
        {
            Id = user.Id;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Email = user.Email;
            Gender = user.Gender;
        }

        public FacebookUser(string psyId, FbMessengerUser user)
        {
            Id = psyId;
            PsyId = psyId;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Gender = user.Gender;
            PhotoPicByPsyId = user.ProfilePic;
        }
    }
}
