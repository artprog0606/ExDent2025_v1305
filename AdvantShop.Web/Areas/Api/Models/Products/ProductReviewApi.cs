using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.CMS;
using AdvantShop.Core.Common.Extensions;

namespace AdvantShop.Areas.Api.Models.Products
{
    public class ProductReviewApi
    {
        public int ReviewId { get; }
        public int ParentId { get; }
        public string CustomerName { get; }
        public string Text { get; }
        public DateTime Date { get; set; }
        public string PreparedDate { get; }
        public int ChildrenCount { get; }
        public bool HasChildren => ChildrenCount > 0;
        public int LikesCount { get; }
        public int DislikesCount { get; }
        public int RatioByLikes { get; }

        public List<ReviewPhotoApi> Photos { get; }
        
        public ProductReviewApi(Review review)
        {
            ReviewId = review.ReviewId;
            ParentId = review.ParentId;
            CustomerName = review.Name;
            Text = review.Text;
            Date = review.AddDate;
            PreparedDate = review.AddDate.ToShortDateTime();
            ChildrenCount = review.ChildrenCount;
            LikesCount = review.LikesCount;
            DislikesCount = review.DislikesCount;
            RatioByLikes = review.RatioByLikes;
            Photos = review.Photos.Select(x => new ReviewPhotoApi(x)).ToList();
        }
    }
    
    public class ReviewPhotoApi
    {
        public string SmallSrc { get; }
        public string BigSrc { get; }

        public ReviewPhotoApi(ReviewPhoto photo)
        {
            SmallSrc = photo.ImageSrc();
            BigSrc = photo.BigImageSrc();
        }
    }
}