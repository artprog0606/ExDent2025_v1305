using System.Collections.Generic;
using System.Web.Mvc;
using AdvantShop.Core.Services.Auth.Calls;

namespace AdvantShop.Web.Admin.Handlers.Settings.Mails
{
    public class GetAuthCallModsHandler
    {
        private readonly string _moduleStringId;
        
        public GetAuthCallModsHandler(string moduleStringId)
        {
            _moduleStringId = moduleStringId;
        }

        public List<SelectListItem> Execute()
        {
            return new CallPhoneConfirmationService().GetAuthCallModsByModuleStringId(_moduleStringId);
        }
    }
}