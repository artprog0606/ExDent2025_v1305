using System;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Orders;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Models.Marketing.Certificates
{
    public class CertificatesFilterModel : BaseFilterModel
    {
        public int CertificateId { get; set; }
        public string CertificateCode { get; set; }

        public string OrderId { get; set; }
        public string ApplyOrderNumber { get; set; }

        public string FromName { get; set; }
        public string ToName { get; set; }

        public string Sum { get; set; }

        public string CertificateMessage { get; set; }

        public bool? Used { get; set; }
        public bool? Enable { get; set; }
        public bool? Paid { get; set; }

        public string ToEmail { get; set; }
        public string FromEmail { get; set; }
        
        public DateTime CreationDate { get; set; }

        public string CreationDateFrom { get; set; }
        public string CreationDateTo { get; set; }

        public bool OrderCertificatePaid { get { return OrderService.IsPaidOrder(OrderId.TryParseInt()); } }
    }    
}
