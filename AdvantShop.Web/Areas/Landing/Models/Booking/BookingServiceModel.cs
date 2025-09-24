using System;
using System.Collections.Generic;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Booking;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.FilePath;
using AdvantShop.Repository.Currencies;

namespace AdvantShop.App.Landing.Models
{
    public class BookingServiceCategoryModel
    {
        public BookingServiceCategoryModel()
        {
            Services = new List<BookingServiceModel>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public int SortOrder { get; set; }
        public bool Enabled { get; set; }

        public List<BookingServiceModel> Services { get; set; }

        public static explicit operator BookingServiceCategoryModel(Category category)
        {
            return category == null ? null : new BookingServiceCategoryModel
            {
                Id = category.Id,
                Name = category.Name,
                Image = category.Image,
                SortOrder = category.SortOrder,
                Enabled = category.Enabled
            };
        }
    }


    public class BookingServiceModel
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public int CurrencyId { get; set; }
        private Currency _currency;
        public Currency Currency { get { return _currency ?? (_currency = CurrencyService.GetCurrency(CurrencyId, true)); } }

        public float Price { get; set; }
        public string PriceFormatted
        {
            get { return PriceFormatService.FormatPrice(Price); }
        }

        public string Image { get; set; }
        public string ImageSrc
        {
            get
            {
                return Image.IsNotEmpty() 
                    ? FoldersHelper.GetPath(FolderType.BookingService, Image, false) 
                    : UrlService.GetUrl() + "images/nophoto_small.jpg";
            }
        }

        public TimeSpan? Duration { get; set; }
        public string DurationFormatted
        {
            get
            {
                if (!Duration.HasValue)
                    return string.Empty;
                return Duration.Value.ToReadableString();

            }
        }

        public int SortOrder { get; set; }
        public bool Enabled { get; set; }

        public static explicit operator BookingServiceModel(Service service)
        {
            return service == null ? null : new BookingServiceModel()
            {
                Id = service.Id,
                CategoryId = service.CategoryId,
                CurrencyId = service.CurrencyId,
                Name = service.Name,
                Price = service.RoundedPrice,
                Description = service.Description.Replace("\n", "<br>"),
                SortOrder = service.SortOrder,
                Enabled = service.Enabled,
                Image = service.Image,
                Duration = service.Duration,
            };
        }
    }
}
