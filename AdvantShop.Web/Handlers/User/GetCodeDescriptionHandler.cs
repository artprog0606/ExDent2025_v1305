using AdvantShop.Core.Services.Auth;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Handlers.User
{
    public class GetCodeDescriptionHandler : ICommandHandler<string>
    {
        public string Execute()
        {
            return new PhoneConfirmationService().GetHintDescription();
        }
    }
}