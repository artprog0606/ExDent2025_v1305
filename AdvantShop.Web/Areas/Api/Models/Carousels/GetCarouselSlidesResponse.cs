using System.Collections.Generic;
using AdvantShop.Core.Services.Api;
using AdvantShop.Core.Services.CMS;

namespace AdvantShop.Areas.Api.Models.Carousels
{
    public class GetCarouselSlidesResponse : List<CarouselSlideItem>, IApiResponse
    {
        public GetCarouselSlidesResponse(List<CarouselSlideItem> items)
        {
            this.AddRange(items);
        }
    }

    public class CarouselSlideItem
    {
        public int CarouselId { get; set; }
        public int SortOrder { get; set; }
        public string Url { get; set; }
        public bool Enabled { set; get; }
        public bool DisplayInOneColumn { get; set; }
        public bool DisplayInTwoColumns { get; set; }
        public bool DisplayInMobile { get; set; }
        public bool Blank { get; set; }
        public CarouselSlideItemPicture Picture { get; set; }
    }

    public class CarouselSlideItemPicture
    {
        public string Src { get; set; }
        public string Alt { get; set; }
    }
}