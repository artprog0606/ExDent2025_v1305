//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.FilePath;
using Newtonsoft.Json;

namespace AdvantShop.Core.Services.Booking
{
    public class ReservationResource
    {
        public int Id { get; set; }
        public int? ManagerId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public int SortOrder { get; set; }
        public bool Enabled { get; set; }

        private Manager _manager;
        [JsonIgnore]
        public Manager Manager
        {
            get
            {
                if (!ManagerId.HasValue)
                    return null;

                return _manager ?? (_manager = ManagerService.GetManager(ManagerId.Value));
            }
        }

        public string ImageSrc
        {
            get
            {
                return Image.IsNotEmpty()
                    ? FoldersHelper.GetPath(FolderType.BookingReservationResource, Image, false)
                    : UrlService.GetUrl() + "images/nophoto_small.png";
            }
        }


        public override string ToString()
        {
            return Name;
        }

    }
}
