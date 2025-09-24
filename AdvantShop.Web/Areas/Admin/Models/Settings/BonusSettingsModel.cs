using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Bonuses;
using AdvantShop.Core.Services.Bonuses.Service;
using AdvantShop.Core.Services.Localization;

namespace AdvantShop.Web.Admin.Models.Settings
{
    public class BonusSettingsModel : IValidatableObject
    {
        public bool NotificationEnabled { get; set; }
        public int BonusGradeId { get; set; }
        public long CardNumFrom { get; set; }
        public long CardNumTo { get; set; }

        public List<SelectListItem> Grades { get; set; }
        public bool IsEnabled { get; set; }
        public float MaxOrderPercent { get; set; }

        public EBonusType BonusType { get; set; }
        public List<SelectListItem> BonusTypes { get; set; }

        public string BonusTextBlock { get; set; }
        public string BonusRightTextBlock { get; set; }
        

        public bool ForbidOnCoupon { get; set; }
        public bool SmsNotificationEnabled { get; set; }
        public bool EmailNotificationEnabled { get; set; }
        public bool PushNotificationEnabled { get; set; }
        
        public EBonusNotificationMethod? AdditionalNotification { get; set; }

        public bool AllowSpecifyBonusAmount { get; set; }
        public bool ProhibitAccrualAndSubstractBonuses { get; set; }
        
        public List<SelectListItem> AdditionalNotificationList{ get; set; }

        public BonusSettingsModel()
        {
            Grades = GradeService.GetAll()
                .Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
                .ToList();

            BonusTypes = new List<SelectListItem>();
            foreach (EBonusType type in Enum.GetValues(typeof(EBonusType)))
            {
                BonusTypes.Add(new SelectListItem() { Text = type.Localize(), Value = ((int)type).ToString() });
            }
            
            AdditionalNotificationList = new List<SelectListItem>()
            {
                new SelectListItem { Text = LocalizationService.GetResource("Core.Bonuses.BonusSystem.EBonusNotificationMethod.Nothing"), Value = String.Empty },
                new SelectListItem { Text = EBonusNotificationMethod.Sms.Localize(), Value = EBonusNotificationMethod.Sms.ToString() },
                new SelectListItem { Text = EBonusNotificationMethod.Email.Localize(), Value = EBonusNotificationMethod.Email.ToString() },
            };
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (CardNumTo <= CardNumFrom)
                yield return new ValidationResult("Проверьте диапазон карт");

            if (MaxOrderPercent < 0)
                MaxOrderPercent = 0;

            if (MaxOrderPercent > 100)
                MaxOrderPercent = 100;
        }
    }
}