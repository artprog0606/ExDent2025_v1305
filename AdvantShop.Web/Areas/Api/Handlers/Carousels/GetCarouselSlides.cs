using System.Linq;
using AdvantShop.Areas.Api.Models.Carousels;
using AdvantShop.Core.Services.CMS;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Carousels
{
    public class GetCarouselSlides : AbstractCommandHandler<GetCarouselSlidesResponse>
    {
        private readonly CarouselSlidesFilter _filter;

        public GetCarouselSlides(CarouselSlidesFilter filter)
        {
            _filter = filter;
        }
        
        protected override GetCarouselSlidesResponse Handle()
        {
            var slides = CarouselService.GetAllCarousels().AsEnumerable();

            if (_filter.Enabled.HasValue)
                slides = slides.Where(x => x.Enabled == _filter.Enabled.Value);
            
            if (_filter.DisplayInOneColumn.HasValue)
                slides = slides.Where(x => x.DisplayInOneColumn == _filter.DisplayInOneColumn.Value);
            
            if (_filter.DisplayInTwoColumns.HasValue)
                slides = slides.Where(x => x.DisplayInTwoColumns == _filter.DisplayInTwoColumns.Value);
            
            if (_filter.DisplayInMobile.HasValue)
                slides = slides.Where(x => x.DisplayInMobile == _filter.DisplayInMobile.Value);

            var result = slides.Select(x => new CarouselSlideItem()
                {
                    CarouselId = x.CarouselId,
                    SortOrder = x.SortOrder,
                    Url = x.Url,
                    Enabled = x.Enabled,
                    DisplayInOneColumn = x.DisplayInOneColumn,
                    DisplayInTwoColumns = x.DisplayInTwoColumns,
                    DisplayInMobile = x.DisplayInMobile,
                    Blank = x.Blank,
                    Picture = new CarouselSlideItemPicture()
                    {
                        Src = x.Picture.ImageSrc(),
                        Alt = x.Picture.Description
                    }
                })
                .ToList();
            return new GetCarouselSlidesResponse(result);
        }
    }
}