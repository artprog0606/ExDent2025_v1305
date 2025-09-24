using AdvantShop.Core;
using AdvantShop.Core.Services.Partners;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Partners
{
    public class UpdateAdminCommentHandler : AbstractCommandHandler
    {
        private readonly int _id;
        private readonly string _comment;
        private Partner _partner;

        public UpdateAdminCommentHandler(int id, string comment)
        {
            _id = id;
            _comment = comment;
        }

        protected override void Load()
        {
            _partner = PartnerService.GetPartner(_id);
        }

        protected override void Validate()
        {
            if (_partner == null)
                throw new BlException(T("Admin.Partner.Errors.NotFound"));
        }

        protected override void Handle()
        {
            _partner.AdminComment = _comment;
            PartnerService.UpdatePartner(_partner);
        }
    }
}
