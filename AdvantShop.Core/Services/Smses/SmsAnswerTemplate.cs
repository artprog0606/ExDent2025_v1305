//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

namespace AdvantShop.Core.Services.Smses
{
    public class SmsAnswerTemplate
    {
        public int TemplateId { get; set; }

        public string Name { get; set; }

        public string Text { get; set; }

        public int SortOrder { get; set; }

        public bool Active { get; set; }      
    }
}
