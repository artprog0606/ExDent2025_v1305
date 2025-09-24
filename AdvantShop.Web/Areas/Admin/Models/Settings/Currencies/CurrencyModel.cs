using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Repository.Currencies;

namespace AdvantShop.Web.Admin.Models.Settings.Currencies
{
    public class CurrencyModel : Currency, IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Name) || 
                string.IsNullOrWhiteSpace(Iso3) || 
                string.IsNullOrWhiteSpace(Symbol))
            {
                yield return new ValidationResult("Заполните обязательные поля");
            }

            Name = Name.DefaultOrEmpty().Reduce(30);
            Iso3 = Iso3.DefaultOrEmpty();
            Symbol = Symbol;
            EnablePriceRounding = RoundNumbers != -1;

            var currencyByIso = CurrencyService.GetAllCurrencies().FirstOrDefault(x => x.Iso3 == Iso3 || x.NumIso3 == NumIso3);
            if (currencyByIso != null && currencyByIso.CurrencyId != CurrencyId)
            {
                yield return new ValidationResult("Код ISO занят");
            }
        }
    }
}
