using AdvantShop.Core;
using AdvantShop.Core.Services.Bonuses.Model;
using AdvantShop.Core.Services.Bonuses.Service;
using AdvantShop.Web.Admin.Models.Bonuses.NotificationTemplates;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Bonuses.NotificationTemplates
{
    public class AddEditNotificationTemplateHandler : AbstractCommandHandler<bool>
    {
        private readonly NotificationTemplateModel _model;
        private NotificationTemplate _smsTemplate;

        public AddEditNotificationTemplateHandler(NotificationTemplateModel model)
        {
            _model = model;
        }

        protected override void Load()
        {
            _smsTemplate = NotificationTemplateService.Get(_model.NotificationTypeId, _model.BonusNotificationMethod.Value);
        }
        protected override void Validate()
        {
            if (_model.IsNew)
            {
                if (_smsTemplate != null)
                    throw new BlException(T("Admin.NotificationTemplates.Error.TemplateExist"));
            }
        }

        protected override bool Handle()
        {
            if (_model.IsNew)
            {
                var obj = new NotificationTemplate();
                obj.NotificationTypeId = _model.NotificationTypeId;
                obj.NotificationBody = _model.NotificationBody;
                obj.NotificationMethod = _model.BonusNotificationMethod.Value;
                NotificationTemplateService.Add(obj);
            }
            else
            {
                _smsTemplate.NotificationBody = _model.NotificationBody;
                NotificationTemplateService.Update(_smsTemplate);
            }
            
            Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Bonuses_AddEditNotificationTemplate);
            
            return true;
        }
    }
}
