using System;
using System.Collections.Generic;
using System.Web;
using AdvantShop.App.Landing.Extensions;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.Services.Landing.Blocks;
using AdvantShop.Core.Services.Landing.Reviews;
using AdvantShop.Core.Services.Mails;
using AdvantShop.Diagnostics;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.App.Landing.Handlers.Reviews
{
    public class AddLpReview : AbstractCommandHandler<AddLpReviewResult>
    {
        private readonly int _blockId;
        private readonly LpReview _review;

        private readonly LpBlockService _blockService;
        private readonly LpService _lpService;
        private readonly LpSiteService _siteService;

        public AddLpReview(int blockId, LpReview review)
        {
            _blockId = blockId;
            _review = review;

            _blockService = new LpBlockService();
            _lpService = new LpService();
            _siteService = new LpSiteService();
        }

        protected override AddLpReviewResult Handle()
        {
            var block = _blockService.Get(_blockId);
            if (block == null)
                throw new BlException("Блок не найден");

            var model = new AddLpReviewResult()
            {
                WithoutPreModerate = block.TryGetSetting("show_without_premoderate") == true
            };

            try
            {
                var items = block.TryGetSettingAsList<LpReview>("items") ?? new List<LpReview>();

                var review = new LpReview()
                {
                    Name = HttpUtility.HtmlEncode(_review.Name.DefaultOrEmpty()),
                    Email = HttpUtility.HtmlEncode(_review.Email.DefaultOrEmpty()),
                    Text = HttpUtility.HtmlEncode(_review.Text.DefaultOrEmpty()).Replace("\n", "<br/>"),
                    Rating = _review.Rating,
                    Date = DateTime.Now,
                    Enabled = model.WithoutPreModerate
                };

                items.Insert(0, review);

                block.TrySetSetting("items", items);

                _blockService.Update(block);
                
                // send letter to admin
                var lp = _lpService.Get(block.LandingId);
                var site = _siteService.Get(lp.LandingSiteId);

                var blockEmail = block.TryGetSetting("admin_email") as string;
                var email = !string.IsNullOrWhiteSpace(blockEmail) ? blockEmail : SettingsMail.EmailForProductDiscuss;

                var subject = "Новый отзыв в воронке";
                var text = 
                    string.Format(
                        "Новый отзыв в воронке <a href=\"{1}\" traget=\"_blank\">{0}</a> <br/> " +
                        "Имя: {2} <br/> " +
                        "Email: {3} <br/> " +
                        "Рейтинг: {4} <br/> " +
                        "Текст отзыва: {5} <br/> ",
                        site.Name, 
                        _lpService.GetLpLink(HttpContext.Current.Request.Url.Host, lp),
                        review.Name,
                        review.Email,
                        review.Rating,
                        review.Text);

                MailService.SendMailNow(Guid.Empty, email, subject, text, true, logging: false);
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            model.Result = true;

            return model;
        }
    }

    public class AddLpReviewResult
    {
        public bool Result { get; set; }
        public bool WithoutPreModerate { get; set; }
    }
}
