using System;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Partners;
using AdvantShop.Localization;

namespace AdvantShop.Web.Admin.Models.Partners
{
    public class PartnerModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public DateTime DateCreated { get; set; }
        public bool Enabled { get; set; }
        public decimal Balance { get; set; }
        public EPartnerType Type { get; set; }

        public string BalanceFormatted { get { return Balance.FormatRoundPriceDefault(); } }

        public string DateCreatedFormatted
        {
            get { return Culture.ConvertDate(DateCreated); }
        }

        public string TypeFormatted
        {
            get { return Type.Localize(); }
        }
    }
}
