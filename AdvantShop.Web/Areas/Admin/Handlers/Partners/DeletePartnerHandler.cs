using AdvantShop.Core;
using AdvantShop.Core.Services.Partners;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Partners
{
    public class DeletePartnerHandler : AbstractCommandHandler
    {
        private readonly int _id;

        public DeletePartnerHandler(int partnerId)
        {
            _id = partnerId;
        }

        protected override void Validate()
        {
            if (PartnerService.GetBindedCustomersCount(_id) > 0)
                throw new BlException("Удаление невозможно. Имеются привязанные покупатели");
            if (TransactionService.PartnerHasTransactions(_id))
                throw new BlException("Удаление невозможно. Имеются транзакции");
        }

        protected override void Handle()
        {
            PartnerService.DeletePartner(_id);
        }
    }
}
