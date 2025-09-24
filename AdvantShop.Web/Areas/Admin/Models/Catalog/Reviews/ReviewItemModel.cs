using System;
using System.Collections.Generic;
using AdvantShop.CMS;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.FilePath;
using AdvantShop.Localization;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Models.Catalog.Reviews
{
    public class ReviewsFilterModel : BaseFilterModel
    {
        public int EntityId { get; set; }
        public EntityType Type { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Text { get; set; }
        public string ArtNo { get; set; }

        public bool? HasPhoto { get; set; }

        public string DateFrom { get; set; }
        public string DateTo { get; set; }

        public bool? Checked { get; set; }
        public bool? ShowOnMain { get; set; }
        public int ProductId { get; set; }
    }

    public class ReviewItemModel
    {
        public int ReviewId { get; set; }
        public string PhotoName { get; set; }

        public string PhotoSrc
        {
            get
            {
                return
                    PhotoName.IsNotEmpty()
                        ? FoldersHelper.GetImageReviewPath(ReviewImageType.Small, PhotoName, true)
                        : UrlService.GetUrl("images/nophoto_small.png");
            }
        }

        public List<ReviewPhoto> Photos { get; set; }

        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime? AddDate { get; set; }

        public string AddDateFormatted
        {
            get { return AddDate != null ? Culture.ConvertDate(AddDate.Value) : ""; }
        }

        public bool Checked { get; set; }
        public string Text { get; set; }
        public EntityType Type { get; set; }
        public string EntityId { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductUrl { get; set; }
        public string ArtNo { get; set; }
        public bool ShowOnMain { get; set; }
        public string Ip { get; set; }
        public List<int> DeletedPhotoIds { get; set; }
        public int ParentId { get; set; }
        public int LikesCount { get; set; }
        public int DislikesCount { get; set; }
        public int? Rating { get; set; }
    }

    public class ReviewPhoto
    {
        public int PhotoId { get; set; }
        public string ImageSrc { get; set; }
        public string BigImageSrc { get; set; }
    }
}
