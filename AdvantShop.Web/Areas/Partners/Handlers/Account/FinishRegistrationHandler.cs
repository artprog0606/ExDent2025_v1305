using AdvantShop.Areas.Partners.ViewModels.Account;
using AdvantShop.Core.Services.Partners;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Partners.Handlers.Account
{
    public class FinishRegistrationHandler : AbstractCommandHandler
    {
        private FinishRegistrationViewModel _model;

        public FinishRegistrationHandler(FinishRegistrationViewModel model)
        {
            _model = model;
        }

        protected override void Load()
        {
        }

        protected override void Validate()
        {
        }

        protected override void Handle()
        {
            var partner = PartnerContext.CurrentPartner;

            switch (partner.Type)
            {
                case EPartnerType.LegalEntity:
                    partner.LegalEntity = _model.LegalEntity;
                    break;
                case EPartnerType.NaturalPerson:
                    partner.NaturalPerson = _model.NaturalPerson;
                    break;
            }

            PartnerService.UpdatePartner(partner);
        }
    }
}