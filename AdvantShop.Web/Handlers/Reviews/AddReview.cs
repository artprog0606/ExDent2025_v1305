using System;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AdvantShop.Catalog;
using AdvantShop.CMS;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Core.Services.Bonuses;
using AdvantShop.Core.Services.Bonuses.Model.Rules;
using AdvantShop.Core.Services.Customers.AdminInformers;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Mails;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Mails;
using AdvantShop.Models.Review;
using AdvantShop.Orders;
using AdvantShop.Web.Infrastructure.Extensions;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Handlers.Reviews
{
    public class AddReview : AbstractCommandHandler<AddReviewResponse>
    {
        private readonly ReviewModel _review;
        private readonly UrlHelper _urlHelper;

        public AddReview(ReviewModel review)
        {
            _review = review;
            _urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
        }

        protected override void Validate()
        {
            if (string.IsNullOrWhiteSpace(_review.Text) ||
                string.IsNullOrWhiteSpace(_review.Name) ||
                string.IsNullOrWhiteSpace(_review.Email))
            {
                throw new BlException("Заполните обязательные поля");
            }

            if (_review.ParentId != null && _review.ParentId != 0 && ReviewService.GetReview(_review.ParentId.Value) == null)
                throw new BlException("parentId не найден");

            if (!string.IsNullOrEmpty(_review.Email) && !ValidationHelper.IsValidEmail(_review.Email))
                throw new BlException("Укажите правильный email");
            
            if (_review.Agreement != SettingsCheckout.IsShowUserAgreementText)
                throw new BlException(LocalizationService.GetResource("Js.Subscribe.ErrorAgreement"));
            
            if (!ProductService.GetProduct(_review.EntityId).Enabled)
                throw new BlException(LocalizationService.GetResource("Common.ProductNotActive"));

            var allowAdd = ModulesExecuter.CheckInfo(HttpContext.Current, ECheckType.ProductReviews, _review.Email, _review.Name, message: _review.Text);
            if (!allowAdd)
                throw new BlException(LocalizationService.GetResource("Common.SpamCheckFailed"));

            var customer = CustomerContext.CurrentCustomer;
            if (_review.EntityType == (int)EntityType.Product && !customer.IsAdmin)
            {
                if (SettingsDesign.WhoAllowReviews == SettingsDesign.eWhoAllowReviews.None)
                    throw new BlException("Добавление отзывов не разрешено никому");

                if (SettingsDesign.WhoAllowReviews == SettingsDesign.eWhoAllowReviews.Registered &&
                    !customer.RegistredUser)
                {
                    throw new BlException("Добавлять отзывы могут только зарегистрированные пользователи");
                }

                if (SettingsDesign.WhoAllowReviews == SettingsDesign.eWhoAllowReviews.BoughtUser &&
                    !OrderService.IsCustomerHasPaidOrderWithProduct(customer.Id, _review.EntityId, false, true))
                {
                    throw new BlException("Добавлять отзывы могут только пользователи, купившие этот товар");
                }
            }

            if (!customer.IsAdmin && (!_review.ParentId.HasValue || _review.ParentId == 0))
            {
                if (ReviewService.IsCustomerHaveReview(_review.EntityId, (EntityType)_review.EntityType, customer.Id))
                    throw new BlException("Нельзя добавить больше одного отзыва");
                if (_review.Rating == 0)
                    throw new BlException("Не указан рейтинг");
            }
        }

        protected override AddReviewResponse Handle()
        {
            var text = HttpUtility.HtmlEncode(_review.Text.Trim()).Replace("\n", "<br />");
            var name = HttpUtility.HtmlEncode(_review.Name.Trim());
            var email = HttpUtility.HtmlEncode(_review.Email.Trim());
            var files = _review.File;

            var reviewItem = new Review
            {
                ParentId = _review.ParentId ?? 0,
                EntityId = _review.EntityId,
                CustomerId = CustomerContext.CustomerId,
                Text = text,
                Type = (EntityType) _review.EntityType,
                Name = name,
                Email = email,
                Ip = HttpContext.Current.Request.UserHostAddress.IsLocalIP()
                    ? "local"
                    : HttpContext.Current.Request.UserHostAddress,
                AddDate = DateTime.Now
            };
            
            var customer = CustomerContext.CurrentCustomer;
            if (customer.BonusCardNumber.HasValue
                && BonusSystem.IsActive)
                new PostingReviewBonusRule().Execute(customer.Id, customer.BonusCardNumber.Value, _review.EntityId);

            ReviewService.AddReview(reviewItem);
            if (_review.EntityType == (int)EntityType.Product && _review.Rating > 0 && (!_review.ParentId.HasValue || _review.ParentId == 0))
                RatingService.Vote(_review.EntityId, _review.Rating);

            if (SettingsCatalog.AllowReviewsImageUploading && files != null && files.Count > 0)
            {
                foreach (var fPhoto in files.Where(x => FileHelpers.CheckFileExtensionByType(x.FileName, EFileType.Image)))
                {
                    var photoName = PhotoService.AddPhoto(new Photo(0, reviewItem.ReviewId, PhotoType.Review)
                    {
                        OriginName = fPhoto.FileName
                    });

                    if (string.IsNullOrWhiteSpace(photoName)) 
                        continue;
                    
                    using (var image = Image.FromStream(fPhoto.InputStream))
                    {
                        var isRotated = FileHelpers.RotateImageIfNeed(image);

                        FileHelpers.SaveResizePhotoFile(FoldersHelper.GetImageReviewPathAbsolut(ReviewImageType.Small, photoName),
                            SettingsPictureSize.ReviewImageWidth, SettingsPictureSize.ReviewImageHeight, image, isRotated: isRotated);
                        FileHelpers.SaveResizePhotoFile(FoldersHelper.GetImageReviewPathAbsolut(ReviewImageType.Big, photoName),
                            image.Width, image.Height, image, isRotated: isRotated);
                    }
                }
            }

            try
            {
                var p = ProductService.GetProduct(_review.EntityId);
                if (p != null)
                {
                    var mailTemplate = new ProductDiscussMailTemplate(p.ArtNo, p.Name,
                        _urlHelper.AbsoluteRouteUrl("Product", new { url = p.UrlPath }), name, DateTime.Now.ToString(), text,
                        _urlHelper.AbsoluteRouteUrl("Product", new { url = p.UrlPath }), email);
                    
                    MailService.SendMailNow(reviewItem.CustomerId, SettingsMail.EmailForProductDiscuss, mailTemplate);

                    AdminInformerService.Add(new AdminInformer(AdminInformerType.Review, reviewItem.ReviewId, reviewItem.CustomerId)
                    {
                        EntityId = _review.EntityId,
                        Title = $"Новый комментарий к товару \"{p.Name}\""
                    });

                    if (reviewItem.ParentId != 0)
                    {
                        var previousReview = ReviewService.GetReview(reviewItem.ParentId);
                        if (previousReview != null && !string.IsNullOrWhiteSpace(previousReview.Email))
                        {
                            var mailAnswerTemplate = new ProductDiscussAnswerMailTemplate(p.ArtNo, p.Name,
                                _urlHelper.AbsoluteRouteUrl("Product", new { url = p.UrlPath, tab = "tabReviews" }), name, DateTime.Now.ToString(),
                                previousReview.Text,
                                text);
                            
                            MailService.SendMailNow(previousReview.Email, mailAnswerTemplate);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            return new AddReviewResponse
            {
                Error = false,
                Review = new AddReviewItem()
                {
                    ParentId = reviewItem.ParentId,
                    ReviewId = reviewItem.ReviewId,
                    Name = reviewItem.Name,
                    Text = reviewItem.Text,
                    Photos = reviewItem.Photos.Where(x => x.PhotoName.IsNotEmpty()).Select(x =>
                        new AddReviewItemPhoto()
                        {
                            PhotoId = x.PhotoId,
                            Small = x.ImageSrc(),
                            Big = x.BigImageSrc()
                        }).ToList(),
                    Likes = reviewItem.LikesCount,
                    Dislikes = reviewItem.DislikesCount,
                    RatioByLikes = reviewItem.RatioByLikes,
                }
            };
        }
    }
}