using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Bonuses;
using AdvantShop.Core.Services.Bonuses.Notification;
using AdvantShop.Core.Services.Bonuses.Notification.Template;

namespace AdvantShop.Web.Admin.Models.Bonuses.NotificationTemplates
{
    public class NotificationTemplateModel : IValidatableObject
    {
        public ENotifcationType NotificationTypeId { get; set; }
        public EBonusNotificationMethod? BonusNotificationMethod { get; set; }
        public string NotificationBody { get; set; }
        public bool IsNew { get; set; }


        public List<SelectListItem> NotificationTypes { get; set; }
        public List<SelectListItem> NotificationMethods { get; set; }
        public List<KeyValuePair<string,string>> AvalibleVarible { get; set; }

        public NotificationTemplateModel()
        {
        }

        public NotificationTemplateModel(ENotifcationType type, EBonusNotificationMethod method)
        {
            NotificationTypeId = type;
            BonusNotificationMethod = method;
            NotificationTypes = EnumExtensions.ToDictionary<ENotifcationType>().Where(x => x.Key != ENotifcationType.None).Select(x => new SelectListItem { Text = x.Key.Localize(), Value = x.Value }).ToList();
            NotificationMethods = EnumExtensions.ToDictionary<EBonusNotificationMethod>()
                .Select(x => new SelectListItem { Text = x.Key.Localize(), Value = x.Value })
                .ToList();
            AvalibleVarible = BaseNotificationTemplate.AvalibleVarible(NotificationTypeId);
        }
        
        public NotificationTemplateModel(ENotifcationType type)
        {
            NotificationTypeId = type;
            NotificationTypes = EnumExtensions.ToDictionary<ENotifcationType>().Where(x => x.Key != ENotifcationType.None).Select(x => new SelectListItem { Text = x.Key.Localize(), Value = x.Value }).ToList();
            NotificationMethods = EnumExtensions.ToDictionary<EBonusNotificationMethod>()
                .Select(x => new SelectListItem { Text = x.Key.Localize(), Value = x.Value })
                .ToList();
            AvalibleVarible = BaseNotificationTemplate.AvalibleVarible(NotificationTypeId);
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!NotificationService.Valid(NotificationBody, NotificationTypeId))
                yield return new ValidationResult("Смс-сообщение содержит не корректные переменные");
        }
    }
}
