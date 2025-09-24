using AdvantShop.Core.Common.Attributes;

namespace AdvantShop.Core.Services.Partners
{
    public enum EPartnerType
    {
        None = 0,
        [Localize("Core.Partners.EPartnerType.LegalEntity")]
        LegalEntity = 1,
        [Localize("Core.Partners.EPartnerType.NaturalPerson")]
        NaturalPerson = 2
    }
}
