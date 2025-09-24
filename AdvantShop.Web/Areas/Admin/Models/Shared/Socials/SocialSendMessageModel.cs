using System;
using System.Collections.Generic;

namespace AdvantShop.Web.Admin.Models.Shared.Socials
{
    public class SocialSendMessageModel
    {
        public Guid? CustomerId { get; set; } 

        public int? CustomerSegmentId { get; set; }

        public List<Guid> CustomerIds { get; set; }

        public string Message { get; set; }
    }
}
