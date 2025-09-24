using AdvantShop.Core.Services.Localization;
using AdvantShop.Helpers;
using AdvantShop.Web.Infrastructure.Localization;

namespace AdvantShop.Web.Admin.ViewModels.Cms.Carousel
{
    public class CarouselViewModel
    {
        public CarouselViewModel()
        {
            Header = LocalizationService.GetResource("Admin.Carousel.Index.Title");
            ButtonTextAdd = new LocalizedString(LocalizationService.GetResource("Admin.Carousel.Index.AddCarousel"));
            ExtensionsImage = string.Join(", ", FileHelpers.GetAllowedFileExtensions(EFileType.Image));
        }
        public string Header { get; set; }

        public LocalizedString ButtonTextAdd { get; set; }
        
        public string ExtensionsImage { get; set; }
    }
}
