using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Localization;

namespace AdvantShop.Core.Services.Bonuses.Notification.Template
{
    public class BaseNotificationTemplate
    {
        private static readonly Type LocalizeAttributeType = typeof(LocalizeAttribute);
        public const string ModelTag = "#";
        public virtual object Prepare()
        {
            return new { };
        }

        public static BaseNotificationTemplate Factory(ENotifcationType type)
        {
            var typeInst = AvalibleType(type);
            return (BaseNotificationTemplate)Activator.CreateInstance(typeInst);
        }


        public static Type AvalibleType(Enum enumValue)
        {
            var attrValue = AttributeHelper.GetAttributeValueField<LinkedClass, Type>(enumValue);
            return attrValue;
        }

        public static List<KeyValuePair<string, string>> AvalibleVarible(Enum enumValue)
        {
            var attrValue = AttributeHelper.GetAttributeValueField<LinkedClass, Type>(enumValue);
            if (attrValue == null) return new List<KeyValuePair<string, string>>();
            var t = attrValue.GetProperties()
                             .Select(x =>
                                  new KeyValuePair<string, string>(
                                      $"{ModelTag}{x.Name}{ModelTag}",
                                      x.IsDefined(LocalizeAttributeType, false)
                                          ? (x.GetCustomAttributes(LocalizeAttributeType, false)[0] as LocalizeAttribute).Value
                                          : null
                                  ))
                             .ToList();
            return t;
        }
    }

    public class OnClientRegistTempalte : BaseNotificationTemplate
    {
        public long CardNumber { get; set; }
        public string CompanyName { get; set; }

        public override object Prepare()
        {
            return new
            {
                CardNumber = CardNumber.ToString(CultureInfo.InvariantCulture),
                CompanyName
            };
        }
    }

    public class OnPurchaseTempalte : BaseNotificationTemplate
    {
        
        [Localize("Core.Bonuses.OnPurchaseTempalte.CompanyName")]
        public string CompanyName { get; set; }
        
        [Localize("Core.Bonuses.OnPurchaseTempalte.PurchaseFull")]
        public decimal PurchaseFull { get; set; }
        
        [Localize("Core.Bonuses.OnPurchaseTempalte.Purchase")]
        public decimal Purchase { get; set; }
        
        [Localize("Core.Bonuses.OnPurchaseTempalte.UsedBonus")]
        public decimal UsedBonus { get; set; }
        
        [Localize("Core.Bonuses.OnPurchaseTempalte.AddBonus")]
        public decimal AddBonus { get; set; }
        
        [Localize("Core.Bonuses.OnPurchaseTempalte.Balance")]
        public decimal Balance { get; set; }    
        
        [Localize("Core.Bonuses.OnPurchaseTempalte.TotalSum")]
        public float TotalSum { get; set; }
        
        [Localize("Core.Bonuses.OnPurchaseTempalte.ProductsSum")]
        public float ProductsSum { get; set; }

        public override object Prepare()
        {
            return new
            {
                CompanyName,
                PurchaseFull = PurchaseFull.ToInvatiant(),
                Purchase = Purchase.ToInvatiant(),
                UsedBonus = UsedBonus.ToInvatiant(),
                AddBonus = AddBonus.ToInvatiant(),
                Balance = Balance.ToInvatiant(),
                TotalSum = ((decimal)TotalSum).ToInvatiant(),
                ProductsSum = ((decimal)ProductsSum).ToInvatiant(),
            };
        }
    }

    public class OnAddBonusTempalte : BaseNotificationTemplate
    {
        [Localize("Core.Bonuses.OnAddBonusTempalte.CompanyName")]
        public string CompanyName { get; set; }
        
        [Localize("Core.Bonuses.OnAddBonusTempalte.Bonus")]
        public decimal Bonus { get; set; }
        
        [Localize("Core.Bonuses.OnAddBonusTempalte.Balance")]
        public decimal Balance { get; set; }
        
        [Localize("Core.Bonuses.OnAddBonusTempalte.BalanceWithNewBonus")]
        public decimal? BalanceWithNewBonus { get; set; }

        [Localize("Core.Bonuses.OnAddBonusTempalte.Basis")]
        public string Basis { get; set; }

        public override object Prepare()
        {
            return new
            {
                CompanyName,
                Bonus = Bonus.ToInvatiant(),
                Balance = Balance.ToInvatiant(),
                BalanceWithNewBonus = (BalanceWithNewBonus ?? Balance).ToInvatiant(),
                Basis
            };
        }
    }

    public class OnSubtractBonusTempalte : BaseNotificationTemplate
    {
        
        [Localize("Core.Bonuses.OnSubtractBonusTempalte.CompanyName")]
        public string CompanyName { get; set; }
        
        [Localize("Core.Bonuses.OnSubtractBonusTempalte.Bonus")]
        public decimal Bonus { get; set; }
        
        [Localize("Core.Bonuses.OnSubtractBonusTempalte.Balance")]
        public decimal Balance { get; set; }
        
        [Localize("Core.Bonuses.OnSubtractBonusTempalte.Basis")]
        public string Basis { get; set; }

        public override object Prepare()
        {
            return new
            {
                CompanyName,
                Bonus = Bonus.ToInvatiant(),
                Balance = Balance.ToInvatiant(),
                Basis
            };
        }
    }

    public class OnCheckBalanceTempalte : BaseNotificationTemplate
    {
        public string CompanyName { get; set; }
        public decimal Balance { get; set; }

        public override object Prepare()
        {
            return new
            {
                CompanyName,
                Balance = Balance.ToInvatiant()
            };
        }
    }

    public class OnUpgradePercentTempalte : BaseNotificationTemplate
    {
        
        [Localize("Core.Bonuses.OnUpgradePercentTempalte.CardNumber")]
        public long CardNumber { get; set; }
        
        [Localize("Core.Bonuses.OnUpgradePercentTempalte.CompanyName")]
        public string CompanyName { get; set; }
        
        [Localize("Core.Bonuses.OnUpgradePercentTempalte.GradeName")]
        public string GradeName { get; set; }
        
        [Localize("Core.Bonuses.OnUpgradePercentTempalte.GradePercent")]
        public decimal GradePercent { get; set; }
        
        [Localize("Core.Bonuses.OnUpgradePercentTempalte.Balance")]
        public decimal Balance { get; set; }

        public override object Prepare()
        {
            return new
            {
                CardNumber = CardNumber.ToString(CultureInfo.InvariantCulture),
                CompanyName,
                GradeName,
                GradePercent = GradePercent.ToInvatiant(),
                Balance = Balance.ToInvatiant(),
            };
        }
    }

    public class OnNotificationCodeTempalte : BaseNotificationTemplate
    {
        public int Code { get; set; }

        public override object Prepare()
        {
            return new { Code = Code.ToString(CultureInfo.InvariantCulture) };
        }
    }

    public class CancelPurchaseTempalte : BaseNotificationTemplate
    {
        public string CompanyName { get; set; }
        public decimal Purchase { get; set; }
        public decimal UsedBonus { get; set; }
        public decimal AddBonus { get; set; }
        [Localize("Core.Bonuses.CancelPurchaseTempalte.Balance")]
        public decimal Balance { get; set; }

        public override object Prepare()
        {
            return new
            {
                CompanyName,
                Balance = Balance.ToInvatiant(),
                Purchase = Purchase.ToInvatiant(),
                UsedBonus = UsedBonus.ToInvatiant(),
                AddBonus = AddBonus.ToInvatiant(),
            };
        }
    }

    public class OnBirthdayRuleTempalte : BaseNotificationTemplate
    {

        [Localize("Core.Bonuses.OnBirthdayRuleTempalte.CompanyName")]
        public string CompanyName { get; set; }

        [Localize("Core.Bonuses.OnBirthdayRuleTempalte.AddBonus")]
        public decimal AddBonus { get; set; }

        [Localize("Core.Bonuses.OnBirthdayRuleTempalte.ToDate")]
        public DateTime? ToDate { get; set; }

        [Localize("Core.Bonuses.OnBirthdayRuleTempalte.Balance")]
        public decimal Balance { get; set; }

        public override object Prepare()
        {
            return new
            {
                CompanyName,
                Balance = Balance.ToInvatiant(),
                AddBonus = AddBonus.ToInvatiant(),
                ToDate = ToDate.HasValue ? Culture.ConvertShortDate(ToDate.Value) : LocalizationService.GetResource("Core.Bonuses.OnBirthdayRuleTempalte.Unlimited")
            };
        }
    }

    public class OnCancellationsBonusTempalte : BaseNotificationTemplate
    {
        
        [Localize("Core.Bonuses.OnCancellationsBonusTempalte.CompanyName")]
        public string CompanyName { get; set; }
        
        [Localize("Core.Bonuses.OnCancellationsBonusTempalte.Balance")]
        public decimal Balance { get; set; }
        
        [Localize("Core.Bonuses.OnCancellationsBonusTempalte.CleanBonuses")]
        public decimal CleanBonuses { get; set; }
        
        [Localize("Core.Bonuses.OnCancellationsBonusTempalte.DayLeft")]
        public int DayLeft { get; set; }

        public override object Prepare()
        {
            return new
            {
                CompanyName,
                Balance = Balance.ToInvatiant(),
                CleanBonuses = CleanBonuses.ToInvatiant(),
                DayLeft = DayLeft.ToString(CultureInfo.InvariantCulture)
            };
        }
    }

    public class ChangeGradePassivityCardTempalte : BaseNotificationTemplate
    {
        public string CompanyName { get; set; }
        public int PassiveMouth { get; set; }
        public DateTime OnDate { get; set; }
        public string GradeName { get; set; }
        public decimal GradePercent { get; set; }
        public override object Prepare()
        {
            return new
            {
                CompanyName,
                PassiveMouth = PassiveMouth.ToString(CultureInfo.InvariantCulture),
                OnDate = OnDate.ToString("dd.MM.yyyy"),
                GradeName,
                GradePercent = GradePercent.ToInvatiant()
            };
        }
    }

    public class OnAddAfilateTempalte : BaseNotificationTemplate
    {
        public string CompanyName { get; set; }
        public decimal Purchase { get; set; }
        public decimal UsedBonus { get; set; }
        public decimal AddBonus { get; set; }
        public decimal Balance { get; set; }

        public override object Prepare()
        {
            return new
            {
                CompanyName,
                Purchase = Purchase.ToInvatiant(),
                UsedBonus = UsedBonus.ToInvatiant(),
                AddBonus = AddBonus.ToInvatiant(),
                Balance = Balance.ToInvatiant(),
            };
        }
    }

    public class OnSubtractAfilateTempalte : BaseNotificationTemplate
    {
        public string CompanyName { get; set; }
        public decimal Purchase { get; set; }
        public decimal UsedBonus { get; set; }
        public decimal AddBonus { get; set; }
        public decimal Balance { get; set; }

        public override object Prepare()
        {
            return new
            {
                CompanyName,
                Purchase = Purchase.ToInvatiant(),
                UsedBonus = UsedBonus.ToInvatiant(),
                AddBonus = AddBonus.ToInvatiant(),
                Balance = Balance.ToInvatiant(),
            };
        }
    }

    public class OnCleanExpiredBonusTempalte : BaseNotificationTemplate
    {
        
        [Localize("Core.Bonuses.OnCleanExpiredBonusTempalte.CompanyName")]
        public string CompanyName { get; set; }
        
        [Localize("Core.Bonuses.OnCleanExpiredBonusTempalte.Balance")]
        public decimal Balance { get; set; }
        
        [Localize("Core.Bonuses.OnCleanExpiredBonusTempalte.CleanBonuses")]
        public decimal CleanBonuses { get; set; }
        
        [Localize("Core.Bonuses.OnCleanExpiredBonusTempalte.DayLeft")]
        public int DayLeft { get; set; }

        public override object Prepare()
        {
            return new
            {
                CompanyName,
                Balance = Balance.ToInvatiant(),
                CleanBonuses = CleanBonuses.ToInvatiant(),
                DayLeft = DayLeft.ToString()
            };
        }
    }
}
