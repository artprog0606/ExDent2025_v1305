using System;
using System.Linq;
using AdvantShop.Core.Services.Partners;
using AdvantShop.Web.Admin.Models.Partners;

namespace AdvantShop.Web.Admin.Handlers.Partners
{
    public class GetPartnerHandler
    {
        private readonly int _id;
        private readonly bool _isEditMode;
        
        public GetPartnerHandler(int id)
        {
            _id = id;
            _isEditMode = true;
        }

        public PartnerEditModel Execute()
        {
            var model = new PartnerEditModel()
            {
                PartnerId = _id,
                IsEditMode = _isEditMode,
            };

            if (!_isEditMode)
            {
                if (model.Partner == null)
                    model.Partner = new Partner();

                return model;
            }

            var partner = PartnerService.GetPartner(_id);
            if (partner == null)
                return null;

            model.Partner = partner;
            foreach (EPartnerMessageType messageType in Enum.GetValues(typeof(EPartnerMessageType))
                .Cast<EPartnerMessageType>().Where(x => x != EPartnerMessageType.None))
            {
                model.SendMessages[messageType.ToString()] = partner.SendMessages.HasFlag(messageType);
            }

            return model;
        }
    }
}
