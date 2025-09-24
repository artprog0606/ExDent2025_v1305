using AdvantShop.Core.Services.Localization;
using AdvantShop.Web.Infrastructure.Localization;

namespace AdvantShop.Web.Admin.ViewModels.Cms.News
{
    public class NewsCategoryViewModel
    {
        public NewsCategoryViewModel()
        {
            Header = LocalizationService.GetResource("Admin.NewsCategory.Index.Title");
            ButtonTextAdd = new LocalizedString(LocalizationService.GetResource("Admin.NewsCategory.Index.AddCategory"));
        }
        public string Header { get; set; }

        public LocalizedString ButtonTextAdd { get; set; }
    }
}
