using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Booking;
using AdvantShop.FilePath;
using AdvantShop.Web.Admin.Models.Booking.Journal;

namespace AdvantShop.Web.Admin.Handlers.Booking.Journal
{
    public class AddUpdateBookingItemsHandler
    {
        private readonly AddUpdateBookingItemsModel _model;
        private readonly BookingCurrency _bookingCurrency;
        public List<string> Errors { get; set; }

        public AddUpdateBookingItemsHandler(AddUpdateBookingItemsModel model)
        {
            _model = model;
            _bookingCurrency = BookingCurrencyService.Get(_model.BookingId);
            Errors = new List<string>();
        }

        public List<BookingItemModel> Execute()
        {
            if (_model.CurrentItems == null)
                _model.CurrentItems = new List<BookingItemModel>();

            if (_model.NewServiceIds != null && _model.NewServiceIds.Count > 0)
            {
                BookingItemModel currentItem = null;
                foreach (var newServiceId in _model.NewServiceIds)
                {
                    if ((currentItem = _model.CurrentItems.FirstOrDefault(x => x.ServiceId == newServiceId)) == null)
                    {
                        var newService = ServiceService.Get(newServiceId);

                        _model.CurrentItems.Add(new BookingItemModel(
                            new BookingItem()
                            {
                                ServiceId = newService.Id,
                                ArtNo = newService.ArtNo,
                                Name = newService.Name,
                                Price = newService.RoundedPrice,
                                Amount = 1,
                            },
                            _bookingCurrency)
                        {
                            ImageSrc =
                                newService.Image.IsNotEmpty()
                                    ? FoldersHelper.GetPath(FolderType.BookingService, newService.Image, false)
                                    : string.Empty
                        });
                    }
                    else
                        currentItem.Amount += 1;
                }
            }

            return _model.CurrentItems;
        }
    }
}
