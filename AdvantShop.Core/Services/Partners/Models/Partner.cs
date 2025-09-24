using System;
using AdvantShop.Configuration;

namespace AdvantShop.Core.Services.Partners
{
    public class Partner
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string City { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public EPartnerMessageType SendMessages { get; set; }
        public string AdminComment { get; set; }
        public bool Enabled { get; set; }
        /// <summary>
        /// stored in base currency
        /// </summary>
        public decimal Balance { get; set; }
        public EPartnerType Type { get; set; }

        public int? CouponId { get; set; }
        public float RewardPercent { get; set; }

        public bool ContractConcluded { get; set; }
        public string ContractNumber { get; set; }
        public DateTime? ContractDate { get; set; }
        public string ContractScan { get; set; }

        public LegalEntity LegalEntity { get; set; }
        public NaturalPerson NaturalPerson { get; set; }

        public bool RegistrationComplete
        {
            get
            {
                return 
                    (Type == EPartnerType.LegalEntity && LegalEntity != null) ||
                    (Type == EPartnerType.NaturalPerson && (NaturalPerson != null || SettingsPartners.SkipRegPasportStepForIndividualEntity));
            }
        }
    }
}
