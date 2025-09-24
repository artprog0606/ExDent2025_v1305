using System;
using AdvantShop.Catalog;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Diagnostics;
using AdvantShop.News;
using AdvantShop.SEO;
using AdvantShop.Web.Admin.Models.Cms.News;

namespace AdvantShop.Web.Admin.Handlers.Cms.News
{
    public class AddUpdateNewsItem
    {
        private readonly AddEditNewsModel _model;

        public AddUpdateNewsItem(AddEditNewsModel model)
        {
            _model = model;
        }

        public int Execute()
        {
            var news = new NewsItem()
            {
                NewsId = _model.NewsId,
                NewsCategoryId = _model.NewsCategoryId,
                ProductsDisplayMode = _model.ProductsDisplayMode,
                Title = _model.Title,
                TextAnnotation = _model.TextAnnotation,
                TextToPublication = _model.TextToPublication,
                ShowOnMainPage = _model.ShowOnMainPage,
                Enabled = _model.Enabled,
                AddingDate = Convert.ToDateTime(_model.AddingDates),
                UrlPath = UrlService.GetAvailableValidUrl(_model.NewsId, ParamType.News, _model.UrlPath.Trim()),
                Meta =
                    new MetaInfo(0, _model.NewsId, MetaType.News, _model.SeoTitle.DefaultOrEmpty(),
                        _model.SeoKeywords.DefaultOrEmpty(), _model.SeoDescription.DefaultOrEmpty(),
                        _model.SeoH1.DefaultOrEmpty()),
            };

            try
            {
                if (_model.IsEditMode)
                {
                    NewsService.UpdateNews(news);
                }
                else
                {
                    news.NewsId = NewsService.InsertNews(news);
                    if (_model.ProductIds != null)
                    {
                        foreach (var productId in _model.ProductIds)
                            NewsService.AddNewsProduct(news.NewsId, productId);
                    }
                    Track.TrackService.TrackEvent(Track.ETrackEvent.Shop_News_NewsCreated);
                }

                if (!_model.IsEditMode)
                {
                    AddPictureLink(_model.PhotoId, news.NewsId);
                }

                return news.NewsId;
            }
            catch (Exception ex)
            {
                Debug.Log.Error("AddUpdateNewsItem handler", ex);
            }

            return 0;
        }

        private void AddPictureLink(int pictureId, int id)
        {
            if (pictureId == 0) return;

            var photo = PhotoService.GetPhoto(pictureId);
            if (photo != null)
                PhotoService.UpdateObjId(photo.PhotoId, id);
        }
    }
}
