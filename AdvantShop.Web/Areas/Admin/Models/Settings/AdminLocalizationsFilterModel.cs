using AdvantShop.Core.Services.Localization;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Repository;

namespace AdvantShop.Web.Admin.Models.Settings
{
    public class AdminLocalizationsFilterModel : BaseFilterModel
    {
        public string ResourceKey { get; set; }

        public string ResourceValue { get; set; }

        public string Value { get; set; }

        private int? _languageId { get; set; }

        public int? LanguageId => _languageId ?? (_languageId = LanguageService.GetLanguage(Value)?.LanguageId);

        public string Text { get; set; }

        public bool ChangeAll { get; set; }
    }
}
