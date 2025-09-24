using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Booking;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.FilePath;
using AdvantShop.Repository.Currencies;

namespace AdvantShop.Web.Admin.Models.Booking.Journal
{
    public class BookingItemModel
    {
        public int Id { get; set; }
        public int? ServiceId { get; set; }
        public string ArtNo { get; set; }
        public string Name { get; set; }
        public float Price { get; set; }

        public string Cost
        {
            get { return (Price*Amount).FormatPrice(Currency); }
        }

        public float Amount { get; set; }
        public string ImageSrc { get; set; }

        private BookingCurrency _currency;
        public BookingCurrency Currency
        {
            get { return _currency ?? (_currency = CurrencyService.CurrentCurrency); }
            set { _currency = value; }
        }

        public BookingItemModel()
        {
        }

        public BookingItemModel(BookingItem item, BookingCurrency currency)
        {
            Id = item.Id;
            ServiceId = item.ServiceId;
            ArtNo = item.ArtNo;
            Name = item.Name;
            Price = item.Price;
            Amount = item.Amount;
            Currency = currency;

            if (item.Service != null)
            {
                ImageSrc = item.Service.Image.IsNotEmpty()
                    ? FoldersHelper.GetPath(FolderType.BookingService, item.Service.Image, false)
                    : string.Empty;
            }
        }
    }
}
