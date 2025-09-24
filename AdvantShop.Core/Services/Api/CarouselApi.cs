using System;
using AdvantShop.Catalog;

namespace AdvantShop.Core.Services.Api
{
    public class CarouselApi
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public bool Enabled { set; get; }
        public int SortOrder { get; set; }
        
        public string FullDescription { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public bool ShowOnMain { get; set; }
        public string CouponCode { get; set; }
        public int? ProductId { get; set; }
        public int? CategoryId { get; set; }
        
        
        private CarouselApiPhoto _picture;
        public CarouselApiPhoto Picture
        {
            get =>
                _picture ??
                (_picture = PhotoService.GetPhotoByObjId<CarouselApiPhoto>(Id, PhotoType.CarouselApi));
            set => _picture = value;
        }
    }
}