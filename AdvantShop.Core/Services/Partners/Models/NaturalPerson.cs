using System;

namespace AdvantShop.Core.Services.Partners
{
    public class NaturalPerson
    {
        public int PartnerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        public string PassportSeria { get; set; }
        public string PassportNumber { get; set; }
        public string PassportWhoGive { get; set; }
        public DateTime? PassportWhenGive { get; set; }
        public string RegistrationAddress { get; set; }
        public string Zip { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }

        public int? PaymentTypeId { get; set; }
        public string PaymentAccountNumber { get; set; }

        public string PassportWhenGiveFormatted
        {
            get { return PassportWhenGive.HasValue ? PassportWhenGive.Value.ToString("dd.MM.yyyy") : string.Empty; }
        }
    }
}
