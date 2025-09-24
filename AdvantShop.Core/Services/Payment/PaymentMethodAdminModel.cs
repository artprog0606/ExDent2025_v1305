using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Services.Configuration;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Customers;
using AdvantShop.Payment;
using AdvantShop.Repository.Currencies;

namespace AdvantShop.Core.Services.Payment
{
    public class PaymentMethodAdminModel
    {
        public PaymentMethodAdminModel()
        {
            Parameters = new Dictionary<string, string>();
        }

        public int PaymentMethodId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Enabled { get; set; }
        public int SortOrder { get; set; }

        public float ExtrachargeInNumbers { get; set; }
        public float ExtrachargeInPercents { get; set; }


        public string PaymentKey { get; set; }
        public string PaymentTypeLocalized
        {
            get
            {
                var list = AdvantshopConfigService.GetDropdownPayments();
                var types = list.Where(x =>
                    string.Equals(x.Value, PaymentKey, StringComparison.OrdinalIgnoreCase)).ToList();
                ListItemModel type = null;
                if (types.Count > 1) {
                    type = types.Where(x => x.Code == Parameters[UniversalPayGateTemplate.Code]).FirstOrDefault();
                }
                else
                {
                    type = types.FirstOrDefault();
                }

                if (type == null)
                    type = ModulesExecuter.GetDropdownPayments().FirstOrDefault(x => string.Equals(x.Value, PaymentKey, StringComparison.OrdinalIgnoreCase));

                return type != null ? type.Text : PaymentKey;
            }
        }

        public int CurrencyId { get; set; }
        public bool CurrencyAllAvailable { get; set; }
        public string[] CurrencyIso3Available { get; set; }
        public string CurrencySymbol { get; set; }

        public List<SelectListItem> Currencies
        {
            get
            {
                var currencies = new List<SelectListItem>();
                var currentCurrency = CurrencyService.CurrentCurrency;
                var currencyIso3Available = CurrencyIso3Available ?? new string[] { };

                foreach (var currency in CurrencyService.GetAllCurrencies()
                    .Where(x => CurrencyAllAvailable || currencyIso3Available.Contains(x.Iso3, StringComparer.OrdinalIgnoreCase))
                    .OrderBy(x => x.CurrencyId != currentCurrency.CurrencyId))
                {
                    currencies.Add(new SelectListItem() { Text = currency.Name, Value = currency.CurrencyId.ToString() });
                }

                var selected = currencies.Find(x => x.Value == CurrencyId.ToString());
                if (selected != null)
                {
                    selected.Selected = true;
                    CurrencySymbol = CurrencyService.GetAllCurrencies().First(x => x.CurrencyId == CurrencyId).Symbol;
                }
                else
                {
                    currencies[0].Selected = true;
                    CurrencyId = currencies[0].Value.TryParseInt();
                }

                return currencies;
            }
        }

        public string Icon { get; set; }

        public Dictionary<string, string> Parameters { get; set; }

        public virtual string ModelType
        {
            get { return this.GetType().AssemblyQualifiedName; }
        }

        public virtual string PaymentViewPath
        {
            get { return "~/Areas/Admin/Views/PaymentMethods/_" + PaymentKey + ".cshtml"; }
        }

        public virtual Tuple<string, string> Instruction { get { return null; } }


        public ProcessType ProcessType { get; set; }
        public NotificationType NotificationType { get; set; }
        public UrlStatus ShowUrls { get; set; }
        public virtual string SuccessUrlLabel => LocalizationService.GetResource("Core.Payment.PaymentMethod.SuccessUrlLabel");
        public virtual string CancelUrlLabel => LocalizationService.GetResource("Core.Payment.PaymentMethod.CancelUrlLabel");
        public virtual string FailUrlLabel => LocalizationService.GetResource("Core.Payment.PaymentMethod.FailUrlLabel");
        public virtual string NotificationUrlLabel => LocalizationService.GetResource("Core.Payment.PaymentMethod.NotificationUrlLabel");

        public string SuccessUrl { get; set; }
        public string CancelUrl { get; set; }
        public string FailUrl { get; set; }
        public string NotificationUrl { get; set; }

        public bool ShowCurrency { get; set; }

        public int? TaxId { get; set; }

        public List<SelectListItem> Taxes { get; set; }

        public CustomerType CustomerType { get; set; }

        public List<SelectListItem> CustomerTypes { get; set; }

    }
}
