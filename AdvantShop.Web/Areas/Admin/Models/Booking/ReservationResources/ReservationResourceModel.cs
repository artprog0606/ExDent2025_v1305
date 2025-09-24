using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Booking;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.FilePath;

namespace AdvantShop.Web.Admin.Models.Booking.ReservationResources
{
    public class ReservationResourceModel
    {
        public int Id { get; set; }
        public int? ManagerId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Enabled { get; set; }
        public int SortOrder { get; set; }
        public string Image { get; set; }

        public string PhotoSrc
        {
            get
            {
                return Image.IsNotEmpty()
                    ? FoldersHelper.GetPath(FolderType.BookingReservationResource, Image, false)
                    : UrlService.GetUrl() + "images/nophoto_small.png";
            }
        }

        private bool _canDeleteInited;
        private void InitCanDelete()
        {
            if (_canDeleteInited)
                return;
            //if (CustomerId != Guid.Empty)
            //{
            //    //List<string> messages;
            _canBeDeleted = true;
            //    //_canBeDeleted = CustomerService.CanDelete(CustomerId, out messages);
            //    //_cantDeleteMessage = messages.AggregateString("<br/>");
            //}
            _canDeleteInited = true;
        }

        private string _cantDeleteMessage;
        public string CantDeleteMessage
        {
            get
            {
                InitCanDelete();
                return _cantDeleteMessage;
            }
        }

        private bool _canBeDeleted;
        public bool CanBeDeleted
        {
            get
            {
                InitCanDelete();
                return _canBeDeleted;
            }
        }


        public static explicit operator ReservationResourceModel(ReservationResource reservationResource)
        {
            var model = new ReservationResourceModel()
            {
                Id = reservationResource.Id,
                ManagerId = reservationResource.ManagerId,
                Name = reservationResource.Name,
                Description = reservationResource.Description,
                Enabled = reservationResource.Enabled,
                SortOrder = reservationResource.SortOrder,
                Image = reservationResource.Image
            };

            return model;
        }
    }
}
