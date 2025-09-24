using System;

namespace AdvantShop.Web.Admin.Models.Shared.Smses
{
    public class GetSmsToCustomerModel
    {
        public Guid? CustomerId { get; set; }
        public int? OrderId { get; set; }
        public int? LeadId { get; set; }
        public int TemplateId { get; set; }
        public bool IsMassSending { get; set; }
    }
}
