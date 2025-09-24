//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;

namespace AdvantShop.Mails
{
    public class MailAnswerTemplate
    {
        public int TemplateId { get; set; }

        public string Name { get; set; }

        public string Body { get; set; }

        public string Subject { get; set; }

        public int SortOrder { get; set; }

        public bool Active { get; set; }      
    }
}
