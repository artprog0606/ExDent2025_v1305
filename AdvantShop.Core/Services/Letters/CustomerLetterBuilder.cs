using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Bonuses;
using AdvantShop.Core.Services.Bonuses.Service;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Customers;

namespace AdvantShop.Letters
{
    public sealed class CustomerLetterBuilder : BaseLetterTemplateBuilder<Customer, CustomerLetterTemplateKey>
    {
        public CustomerLetterBuilder(Customer customer) : base(customer, null)
        {
        }

        protected override string GetValue(CustomerLetterTemplateKey key)
        {
            var customer = _entity;

            switch (key)
            {
                case CustomerLetterTemplateKey.Email: return customer.EMail;
                case CustomerLetterTemplateKey.FirstName: return customer.FirstName;
                case CustomerLetterTemplateKey.LastName: return customer.LastName;
                case CustomerLetterTemplateKey.Patronymic: return customer.Patronymic;
                case CustomerLetterTemplateKey.FullName: return customer.GetFullName();
                case CustomerLetterTemplateKey.Phone: return customer.Phone;
                case CustomerLetterTemplateKey.Organization: return customer.Organization;
                case CustomerLetterTemplateKey.BirthDay: return customer.BirthDay?.ToString("dd.MM.yy");
                case CustomerLetterTemplateKey.NewsSubscription:
                    return customer.IsAgreeForPromotionalNewsletter
                        ? LocalizationService.GetResource("User.Registration.Yes")
                        : LocalizationService.GetResource("User.Registration.No");
                case CustomerLetterTemplateKey.BonusBalance:
                    if (!customer.BonusCardNumber.HasValue || !BonusSystem.IsActive)
                        return null;
                    return ((float)BonusService.ActualSum(customer.Id).SimpleRoundPrice()).FormatBonuses();

                default:
                    throw new ArgumentOutOfRangeException(nameof(key), key, null);
            }
        }

        public override List<LetterFormatKey> GetKeyDescriptions()
        {
            return BonusSystem.IsActive
                    ? LetterBuilderHelper.GetLetterFormatKeys<CustomerLetterTemplateKey>()
                    : LetterBuilderHelper.GetLetterFormatKeys(Enum.GetValues(typeof(CustomerLetterTemplateKey))
                    .Cast<CustomerLetterTemplateKey>()
                    .Where(x => x != CustomerLetterTemplateKey.BonusBalance)
                    .ToList());
        }
    }
}