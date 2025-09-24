using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Core.Services.Partners;

namespace AdvantShop.Web.Admin.Models.Partners
{
    public class PartnerEditModel
    {
        public PartnerEditModel()
        {
            PaymentTypes = new List<SelectListItem> { new SelectListItem() { Text = "-", Value = string.Empty } };
            PaymentTypes.AddRange(PaymentTypeService.GetPaymentTypes().Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }));

            SendMessages = new Dictionary<string, bool>();
            foreach (EPartnerMessageType messageType in Enum.GetValues(typeof(EPartnerMessageType))
                .Cast<EPartnerMessageType>().Where(x => x != EPartnerMessageType.None))
            {
                SendMessages.Add(messageType.ToString(), true);
            }

        }

        public int PartnerId { get; set; }
        public Partner Partner { get; set; }
        public bool IsEditMode { get; set; }

        public string CouponCode { get; set; }

        public Dictionary<string, bool> SendMessages { get; set; }

        public List<SelectListItem> PaymentTypes { get; set; }
    }
}
