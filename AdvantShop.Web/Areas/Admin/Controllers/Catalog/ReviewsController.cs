using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AdvantShop.Catalog;
using AdvantShop.CMS;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Catalog.Reviews;
using AdvantShop.Web.Admin.Models.Catalog.Reviews;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Filters;
using AdvantShop.Core.Services;

namespace AdvantShop.Web.Admin.Controllers.Catalog
{
    [Auth(RoleAction.Catalog)]
    public class ReviewsController : BaseAdminController
    {
        public ActionResult Index(ReviewsFilterModel model)
        {
            SetMetaInformation(T("Admin.Reviews.Index.Title"));
            SetNgController(NgControllers.NgControllersTypes.ReviewsCtrl);

            return View(model);
        }

        public JsonResult GetReviews(ReviewsFilterModel model)
        {
            return Json(new GetReviewsHandler(model).Execute());
        }

        public JsonResult GetArtNoByProductId(int id)
        {
            return Json(ProductService.GetProductArtNoByProductID(id));
        }

        #region Commands

        private void Command(ReviewsFilterModel model, Action<int, ReviewsFilterModel> func)
        {
            if (model.SelectMode == SelectModeCommand.None)
            {
                foreach (var id in model.Ids)
                    func(id, model);
            }
            else
            {
                var ids = new GetReviewsHandler(model).GetItemsIds("ReviewId");
                foreach (int id in ids)
                {
                    if (model.Ids == null || !model.Ids.Contains(id))
                        func(id, model);
                }
            }
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteReviews(ReviewsFilterModel model)
        {
            Command(model, (id, c) => ReviewService.DeleteReview(id));
            return JsonOk();
        }

        #endregion

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteReview(int reviewId)
        {
            ReviewService.DeleteReview(reviewId);
            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult Inplace(int reviewId, bool Checked)
        {
            var review = ReviewService.GetReview(reviewId);
            if (review == null)
                return JsonError();

            review.Checked = Checked;
            ReviewService.UpdateReview(review);

            return JsonOk();
        }


        #region Get | Add | Update review

        [HttpGet]
        public JsonResult GetReview(int reviewId)
        {
            var review = ReviewService.GetReview(reviewId);
            if (review == null)
                return JsonError("Отзыв не существует");

            var model = new ReviewItemModel()
            {
                ReviewId = review.ReviewId,
                Type = EntityType.Product,
                Checked = review.Checked,
                Name = review.Name,
                Email = review.Email,
                Text = review.Text,
                AddDate = review.AddDate,
                Ip = review.Ip,
                ShowOnMain = review.ShowOnMain,
                LikesCount = review.LikesCount,
                DislikesCount = review.DislikesCount,
                Rating = review.Rating
            };

            var product = ProductService.GetProduct(review.EntityId);
            if (product != null)
            {
                model.EntityId = product.ProductId.ToString();
                model.ArtNo = product.ArtNo;
                model.ProductName = product.Name;
                model.ProductUrl = UrlService.GetUrl(UrlService.GetLink(ParamType.Product, product.UrlPath, product.ProductId));
            }

            model.Photos = PhotoService.GetPhotos<AdvantShop.Catalog.ReviewPhoto>(review.ReviewId, PhotoType.Review).Select(photo => new Models.Catalog.Reviews.ReviewPhoto
            {
                PhotoId = photo.PhotoId,
                ImageSrc = FoldersHelper.GetImageReviewPath(ReviewImageType.Small, photo.PhotoName, true),
                BigImageSrc = FoldersHelper.GetImageReviewPath(ReviewImageType.Big, photo.PhotoName, true)
            }).ToList();

            return JsonOk(model);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddReview(List<HttpPostedFileBase> file, ReviewItemModel model)
        { 
            int productId = 0;
            if (model.ParentId == 0 && (string.IsNullOrWhiteSpace(model.ArtNo) 
                                        || (productId = ProductService.GetProductId(model.ArtNo)) == 0 
                                        && (productId = ProductService.GetProductIDByOfferArtNo(model.ArtNo)) == 0))
                return JsonError(T("Admin.Catalog.NoProductsWithVendorCode"));
            else if (model.ParentId != 0)
                productId = ReviewService.GetReview(model.ParentId).EntityId;

            var review = new Review()
            {
                EntityId = productId,
                Type = EntityType.Product,
                CustomerId = CustomerContext.CustomerId,
                Name = HttpUtility.HtmlEncode(model.Name.DefaultOrEmpty()),
                Text = model.Text.DefaultOrEmpty(),
                Checked = model.Checked,
                Email = HttpUtility.HtmlEncode(model.Email.DefaultOrEmpty()),
                AddDate = model.AddDate ?? DateTime.Now,
                Ip = !Request.UserHostAddress.IsLocalIP() ? Request.UserHostAddress : "local",
                ShowOnMain = model.ShowOnMain,
                ParentId = model.ParentId
            };

            ReviewService.AddReview(review);

            if (file != null && file.Count > 0)
                foreach (var fPhoto in file.Where(x => FileHelpers.CheckFileExtensionByType(x.FileName, EFileType.Image)))
                    AddPhoto(review.ReviewId, fPhoto);

            Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Products_AddReview);

            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UpdateReview(List<HttpPostedFileBase> file, ReviewItemModel model)
        {
            var review = ReviewService.GetReview(model.ReviewId);
            if (review == null)
                return JsonError(T("Admin.Catalog.ReviewNotFound"));

            review.Name = HttpUtility.HtmlEncode(model.Name.DefaultOrEmpty());
            review.Email = HttpUtility.HtmlEncode(model.Email.DefaultOrEmpty());
            review.Text = model.Text.DefaultOrEmpty();
            review.Checked = model.Checked;
            review.AddDate = model.AddDate ?? DateTime.Now;
            review.ShowOnMain = model.ShowOnMain;

            ReviewService.UpdateReview(review);

            if (model.DeletedPhotoIds != null)
                foreach (var photoId in model.DeletedPhotoIds)
                    PhotoService.DeleteReviewPhoto(photoId);

            if (file != null && file.Count > 0)
                foreach (var fPhoto in file.Where(x => FileHelpers.CheckFileExtensionByType(x.FileName, EFileType.Image)))
                    AddPhoto(review.ReviewId, fPhoto);

            return JsonOk();
        }

        private void AddPhoto(int reviewId, HttpPostedFileBase photoFile)
        {
            try
            {
                var photoName =
                    PhotoService.AddPhoto(new Photo(0, reviewId, PhotoType.Review) {OriginName = photoFile.FileName});
                
                using (Image image = Image.FromStream(photoFile.InputStream))
                {
                    var isRotated = FileHelpers.RotateImageIfNeed(image);

                    FileHelpers.SaveResizePhotoFile(FoldersHelper.GetImageReviewPathAbsolut(ReviewImageType.Small, photoName),
                        SettingsPictureSize.ReviewImageWidth, SettingsPictureSize.ReviewImageHeight, image, isRotated: isRotated);
                    FileHelpers.SaveResizePhotoFile(FoldersHelper.GetImageReviewPathAbsolut(ReviewImageType.Big, photoName),
                        image.Width, image.Height, image, isRotated: isRotated);
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex.Message + " at ReviewController AddPhoto", ex);
            }
        }

        #endregion

        [HttpPost]
        public JsonResult GetFormData()
        {
            return Json(new
            {
                filesHelpText = FileHelpers.GetFilesHelpText(EFileType.Image, "15MB"),
                reviewImageHeight = SettingsPictureSize.ReviewImageHeight + "px",
                reviewImageWidth = SettingsPictureSize.ReviewImageWidth + "px"
            });
        }
    }
}
