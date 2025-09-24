using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Controls;
using AdvantShop.Core.Services.SalesChannels;
using AdvantShop.Customers;
using AdvantShop.News;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Cms.News;
using AdvantShop.Web.Admin.Models.Cms.News;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Extensions;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Web.Admin.Controllers.Cms
{
    [Auth(RoleAction.Store)]
    [SalesChannel(ESalesChannelType.Store)]
    public partial class NewsController : BaseAdminController
    {
        public ActionResult Index()
        {
            SetMetaInformation(T("Admin.News.Index.Title"));
            SetNgController(NgControllers.NgControllersTypes.NewsCtrl);

            return View();
        }

        public JsonResult GetNews(NewsFilterModel model)
        {
            return Json(new GetNews(model).Execute());
        }

        #region Add/Edit

        public ActionResult Add()
        {
            var newsCategories = NewsService.GetNewsCategories().ToList();
            var productsDisplayMods = Enum.GetValues(typeof(NewsItem.ENewsProductsDisplayMode)).Cast<NewsItem.ENewsProductsDisplayMode>().Select(x => new SelectListItem()
            {
                Value = ((short)x).ToString(),
                Text = x.Localize(),
            }).ToList();

            var model = new AddEditNewsModel
            {
                NewsId = -1,
                NewsCategoryId = newsCategories.Any() ? newsCategories[0].NewsCategoryId : -1,
                ProductsDisplayMode = NewsItem.ENewsProductsDisplayMode.Vertically,
                IsEditMode = false,
                DefaultMeta = true,
                NewsCategory = new List<SelectListItem>() { new SelectListItem { Text = T("Admin.Cms.NotSelected"), Value = "-1" } },
                ProductsDisplayMods = productsDisplayMods,
                AddingDate = DateTime.Now,
                PhotoSrc = "../images/nophoto_small.png",
                PhotoId = 0,
                ShowOnMainPage = true,
                Enabled = true,
            };
            
            if (newsCategories.Any())
            {
                model.NewsCategory.AddRange(newsCategories.Select(
                        x => new SelectListItem() { Text = x.Name, Value = x.NewsCategoryId.ToString() }).ToList());
            }

            SetMetaInformation(T("Admin.NewsItem.Index.Title"));
            SetNgController(NgControllers.NgControllersTypes.NewsItemCtrl);

            return View("AddEdit", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Add(AddEditNewsModel model)
        {
            if (ModelState.IsValid)
            {
                var id = new AddUpdateNewsItem(model).Execute();
                if (id != 0)
                {
                    ShowMessage(NotifyType.Success, T("Admin.ChangesSuccessfullySaved"));
                    return RedirectToAction("Edit", new { id = id });
                }
            }

            ShowErrorMessages();

            SetMetaInformation(T("Admin.NewsItem.Index.Title"));
            SetNgController(NgControllers.NgControllersTypes.NewsItemCtrl);

            var category = NewsService.GetNewsCategories();
            model.NewsCategory = category.Select(x =>
                    new SelectListItem() { Text = x.Name, Value = x.NewsCategoryId.ToString() }).ToList();
            model.ProductsDisplayMods = Enum.GetValues(typeof(NewsItem.ENewsProductsDisplayMode)).Cast<NewsItem.ENewsProductsDisplayMode>().Select(x => new SelectListItem()
            {
                Value = ((short)x).ToString(),
                Text = x.Localize(),
            }).ToList();
            model.AddingDate = Convert.ToDateTime(model.AddingDates);

            return View("AddEdit", model);
        }

        public ActionResult Edit(int id)
        {
            var news = NewsService.GetNewsById(id);

            if (news == null)
                return Error404();

            var model = new GetNewsModel(news).Execute();

            SetMetaInformation(T("Admin.NewsItem.Index.Title") + " - " + news.Title);
            SetNgController(NgControllers.NgControllersTypes.NewsItemCtrl);

            return View("AddEdit", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit(AddEditNewsModel model)
        {
            if (ModelState.IsValid)
            {
                var handler = new AddUpdateNewsItem(model);
                var result = handler.Execute();

                if (result != 0)
                {
                    ShowMessage(NotifyType.Success, T("Admin.ChangesSuccessfullySaved"));
                    return RedirectToAction("Edit", new { id = model.NewsId });
                }
            }

            ShowErrorMessages();

            SetMetaInformation(T("Admin.NewsItem.Index.Title") + " - " + model.Title);
            SetNgController(NgControllers.NgControllersTypes.NewsItemCtrl);

            var category = NewsService.GetNewsCategories();
            model.NewsCategory = category.Select(x =>
                    new SelectListItem() { Text = x.Name, Value = x.NewsCategoryId.ToString() }).ToList();
            model.ProductsDisplayMods = Enum.GetValues(typeof(NewsItem.ENewsProductsDisplayMode)).Cast<NewsItem.ENewsProductsDisplayMode>().Select(x => new SelectListItem()
            {
                Value = ((short)x).ToString(),
                Text = x.Localize(),
            }).ToList();
            model.AddingDate = Convert.ToDateTime(model.AddingDates);

            return View("AddEdit", model);
        }

        #endregion

        #region Inplace

        [HttpPost]
        [ValidateJsonAntiForgeryToken]
        public JsonResult Inplace(NewsModel model)
        {
            var dbModel = NewsService.GetNewsById(model.NewsId);
            if (dbModel == null)
                return Json(new { result = false });
            dbModel.Enabled = model.Enabled;
            dbModel.ShowOnMainPage = model.ShowOnMainPage;
            NewsService.UpdateNews(dbModel);

            return Json(new { result = true });
        }

        #endregion

        #region Commands

        private void Command(NewsFilterModel command, Func<int, NewsFilterModel, bool> func)
        {
            if (command.SelectMode == SelectModeCommand.None)
            {
                foreach (var id in command.Ids)
                {
                    func(id, command);
                }
            }
            else
            {
                var handler = new GetNews(command);
                var ids = handler.GetItemsIds();

                foreach (int id in ids)
                {
                    if (command.Ids == null || !command.Ids.Contains(id))
                        func(id, command);
                }
            }
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteNews(NewsFilterModel command)
        {
            Command(command, (id, c) =>
            {
                NewsService.DeleteNews(id);
                return true;
            });
            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SetNewsEnabled(NewsFilterModel command)
        {
            Command(command, (id, c) =>
            {
                NewsService.ChangeNewsEnabled(id, true);
                return true;
            });
            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SetNewsDisabled(NewsFilterModel command)
        {
            Command(command, (id, c) =>
            {
                NewsService.ChangeNewsEnabled(id, false);
                return true;
            });
            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SetNewsOnMainPage(NewsFilterModel command)
        {
            Command(command, (id, c) =>
            {
                NewsService.SetNewsOnMainPage(id, true);
                return true;
            });
            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SetNewsNotOnMainPage(NewsFilterModel command)
        {
            Command(command, (id, c) =>
            {
                NewsService.SetNewsOnMainPage(id, false);
                return true;
            });
            return JsonOk();
        }

        #endregion

        public JsonResult GetNewsCategories()
        {
            var categories = NewsService.GetNewsCategories();

            return Json(categories.Select(x => new { label = x.Name, value = x.NewsCategoryId.ToString() }));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteNewsItem(int newsId)
        {
            NewsService.DeleteNews(newsId);
            return JsonOk();
        }

        #region UploadImage

        [HttpPost, ValidateJsonAntiForgeryToken]

        public JsonResult UploadPicture(int? objId)
        {
            var result = new UploadNewsPicture(objId).Execute();
            return result.Result
                ? JsonOk(new { picture = result.Picture, pictureId = result.PictureId, pictureName = result.FileName })
                : JsonError(result.Error);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]

        public JsonResult DeletePicture(int? objId)
        {
            var result = new DeleteNewsPicture(objId).Execute();
            return
                result.Result
                    ? JsonOk(new { picture = result.Picture, pictureId = result.PictureId, pictureName = result.FileName })
                    : JsonError(T("Admin.Cms.ErrorWhileDeletingImage"));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]

        public JsonResult UploadPictureByLink(int? objId, string fileLink)
        {
            var result = new UploadNewsPictureByLink(objId, fileLink).Execute();
            return result.Result
                ? JsonOk(new { picture = result.Picture, pictureId = result.PictureId, pictureName = result.FileName })
                : JsonError(result.Error);
        }

        #endregion

        #region News Products

        [HttpGet]
        public JsonResult GetNewsProducts(int newsId)
        {
            var products = NewsService.GetAllNewsProducts(newsId).Select(x => new
            {
                x.ProductId,
                x.Name,
                x.ArtNo,
                Url = Url.AbsoluteActionUrl("Edit", "Product", new { id = x.ProductId }),
                ImageSrc = x.Photo.ImageSrcSmall()
            });

            return Json(new
            {
                products,
                productIds = products.Select(x => x.ProductId)
            });
        }

        [HttpGet]
        public JsonResult GetProducts(int[] productIds)
        {
            var products = NewsService.GetProductsForNews(productIds.ToList()).Select(x => new
            {
                x.ProductId,
                x.Name,
                x.ArtNo,
                Url = Url.AbsoluteActionUrl("Edit", "Product", new { id = x.ProductId }),
                ImageSrc = x.Photo.ImageSrcSmall()
            });

            return Json(products);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteNewsProduct(int newsId, int productId)
        {
            NewsService.DeleteNewsProduct(newsId, productId);
            return Json(new { result = true });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddNewsProduct(int newsId, List<int> ids)
        {
            if (ids == null || ids.Count == 0)
                return Json(new { result = false });

            NewsService.ClearNewsProducts(newsId);
            foreach (var id in ids)
                NewsService.AddNewsProduct(newsId, id);

            return Json(new { result = true });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ChangeNewsProductsSorting(int newsId, int id, int? prevId, int? nextId)
        {
            NewsService.ChangeNewsProductsSorting(newsId, id, prevId, nextId);

            return JsonOk();
        }

        #endregion

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ChangeRssViewNews(bool enabled)
        {
            SettingsNews.RssViewNews = enabled;
            return JsonOk();
        }
    }
}
