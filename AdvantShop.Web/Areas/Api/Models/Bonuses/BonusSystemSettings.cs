using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Api;
using AdvantShop.Core.Services.Bonuses;
using AdvantShop.Core.Services.Bonuses.Service;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdvantShop.Areas.Api.Models.Bonuses
{
    public class BonusSystemSettings : IApiResponse
    {
        public long CardNumberFrom { get; set; }
        public long CardNumberTo { get; set; }

        public int BonusGradeId { get; set; }
        public List<BonusSystemSettingsListItem> Grades { get; set; }

        public bool IsEnabled { get; set; }
        public float MaxOrderPercent { get; set; }

        public EBonusType BonusType { get; set; }
        public List<BonusSystemSettingsListItem> BonusTypes { get; set; }

        public string BonusTextBlock { get; set; }
        public string BonusRightTextBlock { get; set; }
        public bool DisallowUseWithCoupon { get; set; }
        public bool SmsNotificationEnabled { get; set; }
        public bool EmailNotificationEnabled { get; set; }
        public bool PushNotificationEnabled { get; set; }
        

        public BonusSystemSettings()
        {
            Grades = GradeService.GetAll()
                .Select(x => new BonusSystemSettingsListItem { Label = x.Name, Value = x.Id.ToString() })
                .ToList();

            BonusTypes = new List<BonusSystemSettingsListItem>();
            foreach (EBonusType type in Enum.GetValues(typeof(EBonusType)))
            {
                BonusTypes.Add(new BonusSystemSettingsListItem() { Label = type.Localize(), Value = ((int)type).ToString() });
            }
        }
    }

    public class BonusSystemSettingsListItem
    {
        public string Label { get; set; }
        public string Value { get; set; }
        public bool Selected { get; set; }
    }
}