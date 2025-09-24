using System;

namespace AdvantShop.Core.Services.Partners
{
    public class LegalEntity
    {
        public int PartnerId { get; set; }
        public string CompanyName { get; set; }
        public string INN { get; set; }
        public string KPP { get; set; }
        public string LegalAddress { get; set; }
        public string ActualAddress { get; set; }
        public string SettlementAccount { get; set; }
        public string Bank { get; set; }
        public string CorrespondentAccount { get; set; }
        public string BIK { get; set; }
        public string PostAddress { get; set; }
        public string Zip { get; set; }
        public string Phone { get; set; }
        public string ContactPerson { get; set; }
        public string Director { get; set; }
        public string Accountant { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
    }
}
