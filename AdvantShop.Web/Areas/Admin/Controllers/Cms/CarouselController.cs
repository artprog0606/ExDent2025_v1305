using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.CMS;
using AdvantShop.Core.Services.SalesChannels;
using AdvantShop.Core.Services.Screenshot;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Cms.Carousel;
using AdvantShop.Web.Admin.Models.Cms.Carousel;
using AdvantShop.Web.Admin.ViewModels.Cms.Carousel;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Web.Admin.Controllers.Cms
{
    [Auth(RoleAction.Store)]
    [SalesChannel(ESalesChannelType.Store)]
    public partial class CarouselController : BaseAdminController
    {
        public ActionResult Index()
        {
            var model = new CarouselViewModel();
            SetMetaInformation(T("Admin.Carousel.Index.Title"));
            SetNgController(NgControllers.NgControllersTypes.CarouselPageCtrl);

            return View(model);
        }

        #region Add/Edit/Get/Delete

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddCarousel(CarouselFilterModel model)
        {
            try
            {
                var fullfilename = FoldersHelper.GetPathAbsolut(FolderType.ImageTemp, model.ImageSrc);
                if (model.ImageSrc.IsNullOrEmpty() || !System.IO.File.Exists(fullfilename))
                    return JsonError();

                var carousel = new Carousel()
                {
                    Url = model.CarouselUrl,
                    SortOrder = model.SortOrder.TryParseInt(),
                    Enabled = Convert.ToBoolean(model.Enabled),
                    DisplayInOneColumn = Convert.ToBoolean(model.DisplayInOneColumn),
                    DisplayInTwoColumns = Convert.ToBoolean(model.DisplayInTwoColumns),
                    DisplayInMobile = Convert.ToBoolean(model.DisplayInMobile),
                    Blank = Convert.ToBoolean(model.Blank),
                };
                var carouselId = CarouselService.AddCarousel(carousel);
                var tempName = PhotoService.AddPhoto(new AdvantShop.Catalog.Photo(0, carouselId, PhotoType.Carousel)
                {
                    Description = model.Description,
                    OriginName = Path.GetFileName(fullfilename),
                    PhotoSortOrder = 0,
                    ColorID = null
                });

                if (!string.IsNullOrEmpty(tempName))
                {
                    using (System.Drawing.Image image = System.Drawing.Image.FromFile(fullfilename))
                    {
                        FileHelpers.SaveResizePhotoFile(FoldersHelper.GetPathAbsolut(FolderType.Carousel, tempName), SettingsPictureSize.CarouselBigWidth, SettingsPictureSize.CarouselBigHeight, image);
                    }
                    FileHelpers.DeleteFilesFromImageTemp();
                }

                var minSortOrder = CarouselService.GetAllCarousels().Min(x => x.SortOrder);
                if (minSortOrder == carousel.SortOrder)
                    new ScreenshotService().UpdateStoreScreenShotInBackground();

                Track.TrackService.TrackEvent(Track.ETrackEvent.Shop_Carousel_AddSlide);
            }
            catch (Exception ex)
            {
                Debug.Log.Error("", ex);
                return Json(new { result = false });
            }

            return Json(new { result = true });
        }
        
        public JsonResult GetCarousel(CarouselFilterModel model)
        {
            var result = new GetCarousel(model).Execute();

            for(var i = 0; i < result.DataItems.Count; i++)
            {
                result.DataItems[i].ImageSrc = result.DataItems[i].Picture.ImageSrc();
            }

            return Json(result);
        }

        public JsonResult DeleteCarousel(CarouselFilterModel model)
        {
            Command(model, (id, c) =>
            {
                CarouselService.DeleteCarousel(id);
                return true;
            });

            new ScreenshotService().UpdateStoreScreenShotInBackground();
            Track.TrackService.TrackEvent(Track.ETrackEvent.Shop_Carousel_DeleteSlide);

            return Json(true);
        }

        #endregion

        #region Upload

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult Upload()
        {            
            var result = new UploadPicture().Execute();
            return result.Result
                ? JsonOk(new { picture = result.Picture, pictureName = result.FileName })
                : JsonError(result.Error);
        }

        [HttpPost]
        [ValidateJsonAntiForgeryToken]
        public JsonResult UploadByLink(string fileLink)
        {            
            var result = new UploadPictureByLink(fileLink).Execute();
            return result.Result
                ? JsonOk(new { picture = result.Picture, pictureName = result.FileName })
                : JsonError(result.Error);
        }

        [HttpPost]
        [ValidateJsonAntiForgeryToken]
        public JsonResult DeleteFile()
        {
            FileHelpers.DeleteFilesFromImageTemp();
            return JsonOk(new { picture = "../images/nophoto_small.png", pictureName = string.Empty });
        }

        #endregion

        #region Inplace

        public JsonResult InplaceCarousel(CarouselFilterModel model)
        {
            if (model.CarouselId == 0)
                return Json(new { result = false });
            
            var carousel = CarouselService.GetCarousel(model.CarouselId);

            carousel.Url = model.CarouselUrl ?? string.Empty;
            carousel.SortOrder = Convert.ToInt32(model.SortOrder);
            carousel.Enabled = (bool)model.Enabled;
            carousel.DisplayInOneColumn = (bool)model.DisplayInOneColumn;
            carousel.DisplayInTwoColumns = (bool)model.DisplayInTwoColumns;
            carousel.DisplayInMobile = (bool)model.DisplayInMobile;
            carousel.Blank = (bool)model.Blank;

            CarouselService.UpdateCarousel(carousel);

            if(carousel.Picture != null && carousel.Picture.Description != model.Description)
            {
                var photo = PhotoService.GetPhoto(carousel.Picture.PhotoId);
                if(photo != null)
                {
                    photo.Description = model.Description;
                    PhotoService.UpdatePhoto(photo);
                }
            }

            var minSortOrder = CarouselService.GetAllCarousels().Min(x => x.SortOrder);
            if (minSortOrder == carousel.SortOrder)
                new ScreenshotService().UpdateStoreScreenShotInBackground();

            return Json(new {result = true});
        }

        #endregion

        #region Command

        private void Command(CarouselFilterModel model, Func<int, CarouselFilterModel, bool> func)
        {
            if (model.SelectMode == SelectModeCommand.None)
            {
                foreach (var id in model.Ids)
                {
                    func(id, model);
                }
            }
            else
            {
                var handler = new GetCarousel(model);
                var ids = handler.GetItemsIds();

                foreach (int id in ids)
                {
                    if (model.Ids == null || !model.Ids.Contains(id))
                        func(id, model);
                }
            }
        }


        #endregion
    }
}
