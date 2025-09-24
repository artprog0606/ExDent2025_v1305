using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.SessionState;
using AdvantShop.App.Landing.Domain.Booking;
using AdvantShop.App.Landing.Domain.Common;
using AdvantShop.App.Landing.Extensions;
using AdvantShop.App.Landing.Filters;
using AdvantShop.App.Landing.Handlers.Inplace;
using AdvantShop.App.Landing.Handlers.Install;
using AdvantShop.App.Landing.Handlers.Landings;
using AdvantShop.App.Landing.Handlers.Pictures;
using AdvantShop.App.Landing.Models.Catalogs;
using AdvantShop.App.Landing.Models.Inplace;
using AdvantShop.App.Landing.Models.Pictures;
using AdvantShop.Catalog;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Crm.SalesFunnels;
using AdvantShop.Core.Services.ImageSearches;
using AdvantShop.Core.Services.Translators;
using AdvantShop.Diagnostics;
using AdvantShop.Web.Infrastructure.Filters;
using Newtonsoft.Json;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Helpers;
using AdvantShop.App.Landing.Models;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.Services.Landing.Blocks;
using AdvantShop.Core.Services.Landing.Forms;
using AdvantShop.Core.Services.Landing.LandingEmails;
using AdvantShop.Core.Services.Landing.Templates;
using AdvantShop.App.Landing.Domain;
using AdvantShop.Core.Services.Landing.Pictures;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.FilePath;

namespace AdvantShop.App.Landing.Controllers
{
    [AdminAuth]
    [AuthLp]
    [LogActivity]
    [SessionState(SessionStateBehavior.Disabled)]
    public partial class LandingInplaceController : LandingBaseController
    {
        #region Ctor

        private readonly LpService _lpService;
        private readonly LpBlockService _lpBlockService;
        private readonly LpFormService _formService;

        public LandingInplaceController()
        {
            _lpService = new LpService();
            _lpBlockService = new LpBlockService();
            _formService = new LpFormService();
        }

        #endregion

        #region Blocks

        // Получаем список блоков для лендинга
        public JsonResult GetBlocks(int landingPageId)
        {
            return Json(new GetBlocks(landingPageId).Execute());
        }

        // Добавление блока
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddBlock(AddBlockModel model, bool? top, int? blockIdSibling)
        {
            var result = new AddBlock(model, top, blockIdSibling).Execute();
            if (result.Result)
                Track.TrackService.TrackEvent(Track.ETrackEvent.Shop_Funnels_AddBlock, model.Name);
            return Json(result);
        }

        // Добавление блока
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddAllBlocksByCategory(AddAllBlocksModel model, bool? top, int? blockIdSibling)
        {
            var result = new List<AddBlockResultModel>();
            var sortOrder = model.SortOrder;
            var success = true;

            foreach (var item in model.Blocks)
            {
                if (result.Any())
                    sortOrder = result.Last().Block.SortOrder + 100;

                var itemModel = new AddBlockModel()
                {
                    LpId = model.LpId,
                    Name = item.Name,
                    SortOrder = sortOrder
                };

                var resultItem = new AddBlock(itemModel, top, blockIdSibling).Execute();

                result.Add(resultItem);

                if (!resultItem.Result)
                    success = resultItem.Result;

                top = false;
            }

            return Json(new { result = success, data = result });
        }

        // Сохранение сортировки
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SaveBlockSortOrder(int blockId, bool top)
        {
            var block = _lpBlockService.Get(blockId);
            if (block == null)
                return JsonError();

            var blocks = _lpBlockService.GetList(block.LandingId);
            var index = blocks.FindIndex(x => x.Id == block.Id);
            var swapIndex = index + (top ? -1 : 1);

            for (int i = 0; i < blocks.Count; i++)
            {
                var sorting = i;

                if (i == swapIndex)
                    sorting = index;
                else if (i == index)
                    sorting = swapIndex;

                blocks[i].SortOrder = sorting * 100;

                _lpBlockService.Update(blocks[i]);
            }

            LpSiteService.UpdateModifiedDateByLandingId(block.LandingId);

            return JsonOk();
        }

        // Удаление блока
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult RemoveBlock(int blockId)
        {
            var block = _lpBlockService.Get(blockId);
            if (block == null)
                return JsonError();

            _lpBlockService.Delete(block.Id);

            var removePictures = new RemoveBlockPicturesHandler(block.LandingId, block.Id).Execute();

            LpSiteService.UpdateModifiedDateByLandingId(block.LandingId);

            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult RemoveAllBlockByCategory(int lpId, LpBlockListItem category)
        {
            var blocks = _lpBlockService.GetList(lpId).Where(x => category.Blocks.Any(k => x.Type == k.Type));

            foreach (var item in blocks)
            {
                var block = _lpBlockService.Get(item.Id);
                if (block == null)
                    return JsonError();

                _lpBlockService.Delete(block.Id);

                var removePictures = new RemoveBlockPicturesHandler(block.LandingId, block.Id).Execute();
            }

            return JsonOk();
        }

        // Конвертировать блок в html блок
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ConvertToHtmlBlock(int blockId)
        {
            return Json(new ConvertToHtmlBlock(blockId, this).Execute());
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult TryUpdateBlock(int blockId)
        {
            return Json(new TryUpdateBlock(blockId).Execute());
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult RecreateBlock(int blockId)
        {
            return Json(new RecreateBlock(blockId).Execute());
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult CopyBlock(int blockId)
        {
            var result = new CopyBlock(blockId).Execute();
            return Json(result != null);
        }

        // for testing
        public ActionResult CreateAllBlocks(int lpId)
        {
            var lp = _lpService.Get(lpId);
            var sortOrder = 0;

            foreach (var blockItem in new LpTemplateService().GetAllBlocks())
            foreach (var block in blockItem.Blocks)
            {
                new InstallBlockHandler(block.Name, lp.Template, lp.Id, sortOrder, new LpConfiguration()).Execute();
                sortOrder += 100;
            }

            return Content("done");
        }

        #endregion

        #region SubBlocks

        /// <summary>
        /// Обновление настроек подблока
        /// </summary>
        /// <returns>Json(new { result = false })</returns>
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UpdateSubBlockSettings(int subBlockId, string settings)
        {
            var subBlock = _lpBlockService.GetSubBlock(subBlockId);
            if (subBlock == null)
                return JsonError();

            try
            {
                var settingsOld = JsonConvert.DeserializeObject<Dictionary<string, object>>(subBlock.Settings);
                var settingsNew = JsonConvert.DeserializeObject<Dictionary<string, object>>(settings);

                if (settingsNew == null)
                    return JsonError();

                if (settingsOld == null)
                    settingsOld = new Dictionary<string, object>();

                foreach (var key in settingsNew.Keys)
                {
                    if (settingsOld.ContainsKey(key))
                    {
                        settingsOld[key] = settingsNew[key];
                    }
                    else
                    {
                        settingsOld.Add(key, settingsNew[key]);
                    }
                }

                subBlock.Settings = JsonConvert.SerializeObject(settingsOld);

                _lpBlockService.UpdateSubBlock(subBlock);

                LpSiteService.UpdateModifiedDateByLandingId(subBlock.LandingBlockId);

                return JsonOk();
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            return JsonError();
        }

        #endregion

        #region Pictures

        [HttpPost, CheckLp, ValidateJsonAntiForgeryToken]
        public JsonResult UploadPicture(int lpId, int blockId, int? maxWidth = null, int? maxHeight = null,
            List<PictureParameters> parameters = null, string picture = null)
        {
            if (picture.IsNotEmpty())
                new RemovePictureHandler(picture, parameters).Execute();

            if (System.Web.HttpContext.Current.Request.Files.Count > 0)
            {
                var file = System.Web.HttpContext.Current.Request.Files[0];

                if (file != null && file.ContentLength > 0 && file.ContentLength < 10000000)
                    return Json(
                        new UploadPictureHandler(lpId, blockId, file, maxWidth, maxHeight, parameters).Execute());
            }

            return Json(new UploadPictureResult()
            {
                Error =
                    "Не корректное изображение. Изображение должно быть формата .jpg, .jpeg, .png, .bmp или .gif и не больше 10 Мб"
            });
        }

        [HttpPost, CheckLp, ValidateJsonAntiForgeryToken]
        public JsonResult UploadPictureByUrl(int lpId, int blockId, string url, int? maxWidth, int? maxHeight,
            List<PictureParameters> parameters = null, string picture = null)
        {
            if (picture.IsNotEmpty())
                new RemovePictureHandler(picture, parameters).Execute();

            if (url.IsNotEmpty())
                return Json(
                    new UploadPictureByUrlHandler(lpId, blockId, url, maxWidth, maxHeight, parameters).Execute());

            return Json(new UploadPictureResult() { Error = "Не передана ссылка на загрузку изображения" });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult GetBase64PictureByUrl(string url)
        {
            if (url.IsNotEmpty())
                return Json(new GetBase64PictureByUrlHandler(url).Execute());

            return Json(new UploadPictureResult()
                { Error = "Не переданы обязательные параметры на загрузку изображения" });
        }

        [HttpPost, CheckLp, ValidateJsonAntiForgeryToken]
        public JsonResult UploadPictureCropped(int lpId, int blockId, string base64String, string ext,
            int? maxWidth = null, int? maxHeight = null, List<PictureParameters> parameters = null,
            string picture = null)
        {
            if (picture.IsNotEmpty())
                new RemovePictureHandler(picture, parameters).Execute();

            if (base64String.IsNotEmpty())
            {
                var result = new UploadPictureCropped(lpId, blockId, base64String, ext, maxWidth, maxHeight, parameters)
                    .Execute();
                return Json(result);
            }

            return Json(new UploadPictureResult()
                { Error = "Не переданы обязательные параметры на загрузку изображения" });
        }


        [HttpPost, CheckLp, ValidateJsonAntiForgeryToken]
        public JsonResult RemovePicture(int lpId, int blockId, string picture,
            List<PictureParameters> parameters = null)
        {
            if (string.IsNullOrEmpty(picture))
                return Json(new UploadPictureResult() { Error = "Not found picture" });

            return Json(new UploadPictureResult() { Result = new RemovePictureHandler(picture, parameters).Execute() });
        }

        [HttpPost, CheckLp, ValidateJsonAntiForgeryToken]
        public JsonResult ResizePicture(int lpId, int blockId, string picture, int maxWidth, int maxHeight,
            List<PictureParameters> parameters = null)
        {
            if (picture.IsNullOrEmpty())
                return Json(new UploadPictureResult()
                {
                    Error = "Не передан путь до изображения"
                });

            if (PicturesHelper.IsDefaultPicture(picture))
            {
                var url = UrlService.GetUrl(picture);
                return Json(new UploadPictureByUrlHandler(lpId, blockId, url, maxWidth, maxHeight, parameters).Execute()); 
            }
            else
            {
                return Json(new ResizePictureHandler(lpId, blockId, picture, maxWidth, maxHeight, parameters).Execute());
            }
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult LogoGeneratorProcessPicture(string dataUrl, string fileExtension, int lpId, int blockId)
        {
            return Json(new LogoGeneratorProccessPicture(fileExtension, dataUrl, lpId, blockId).Execute());
        }

        public JsonResult SearchImages(string term, int page)
        {
            try
            {
                return Json(new PexelsImageSearch().Search(term, page));
            }
            catch (Exception ex)
            {
                return JsonError(ex.Message);
            }
        }

        public JsonResult GetPopularImages(int page)
        {
            try
            {
                return Json(new PexelsImageSearch().GetPopular(page));
            }
            catch (Exception ex)
            {
                return JsonError(ex.Message);
            }
        }

        public JsonResult GetProductsByIds(List<int> productIds)
        {
            try
            {
                if (productIds == null || productIds.Count == 0)
                    return null;

                var products = ProductService.GetAllProductsByIds(productIds);
                return Json(new ProductViewModel(products));
            }
            catch (Exception ex)
            {
                return JsonError(ex.Message);
            }
        }

        public JsonResult GetCategories(List<CategoryTreeViewModel> categoriesSelected)
        {
            try
            {
                List<int> categoryIds = new List<int>();

                foreach (var item in categoriesSelected)
                {
                    categoryIds.Add(item.CategoryId);

                    if (!item.Opened)
                    {
                        categoryIds.AddRange(CategoryService.GetAllChildCategoriesIdsByCategoryId(item.CategoryId));
                    }
                }

                return JsonOk(CategoryService.GetCategoriesByCategoryIds(categoryIds));
            }
            catch (Exception ex)
            {
                return JsonError(ex.Message);
            }
        }

        public JsonResult GetCategoryStringByProductId(List<int> productIds, string separator, bool onlySort)
        {
            try
            {
                if (productIds == null || productIds.Count == 0)
                    return null;

                var categoriesIds = "";

                foreach (int id in productIds)
                {
                    categoriesIds = categoriesIds +
                                    CategoryService.GetCategoryStringByProductId(id, separator, onlySort) + separator;
                }

                var categoriesIdsList = categoriesIds.Split(separator).Select(x => Convert.ToInt32(x)).Distinct()
                    .ToList<int>();

                return Json(categoriesIdsList);
            }
            catch (Exception ex)
            {
                return JsonError(ex.Message);
            }
        }

        public JsonResult Translate(string term)
        {
            return Json(new AdvantshopTranslateService().Translate(term));
        }

        #endregion

        #region Catalog Tree view

        public JsonResult CategoriesTree(int categoryId = 0)
        {
            var categories =
                CategoryService.GetChildCategoriesByCategoryId(categoryId, true).Select(x => new CategoryLpModel()
                {
                    id = x.CategoryId.ToString(),
                    parent = x.ParentCategoryId == 0 ? "#" : x.ParentCategoryId.ToString(),
                    text = x.Name.ToString(),
                    children = x.HasChild
                });

            return Json(categories);
        }

        public JsonResult ProductsForGrid(CatalogFilterModel model)
        {
            return Json(new GetCatalogProducts(model).Execute());
        }

        #endregion

        #region Booking

        public JsonResult GetReservationResources(int affiliateId)
        {
            return Json(LpReservationResourceService.GetLpReservationResources(affiliateId).Select(x => new
            {
                x.Id,
                x.Name,
                x.Description
            }));
        }

        #endregion

        #region Settings

        // Настройки старицы лендинга
        [CheckLp]
        public JsonResult GetSettings(int lpId)
        {
            return Json(new GetSettings(lpId).Execute());
        }

        // Сохранение настроек старицы лендинга
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SaveSettings(int lpId, InplaceSettingsModel settings)
        {
            Track.TrackService.TrackEvent(Track.ETrackEvent.Shop_Funnels_EditPageSettings);
            return Json(new SaveLpPageSettings(lpId, settings, true).Execute());
        }

        // Настройки блока
        public JsonResult GetBlockSettings(int blockId)
        {
            return Json(new GetBlockSettings(blockId).Execute());
        }

        public JsonResult GetCrmFields(int? salesFunnelId)
        {
            var fields = _formService.GetAllFieldsList(salesFunnelId);
            return Json(fields);
        }

        // Сохранение настроек блока
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SaveBlockSettings(int blockId, string settings)
        {
            Track.TrackService.TrackEvent(Track.ETrackEvent.Shop_Funnels_EditBlock);
            return Json(new SaveBlockSettings(blockId, settings).Execute());
        }


        // Сохраняем product Ids
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SaveProductIds(int blockId, string ids)
        {
            var block = _lpBlockService.Get(blockId);
            if (block == null)
                return Json(new { result = false });

            block.TrySetSetting("product_ids", ids.Split(',').Select(x => x.TryParseInt()).ToList());

            var settings = JsonConvert.SerializeObject(block.MappedSettings);

            var result = new SaveBlockSettingsOld(blockId, settings).Execute();
            return Json(result);
        }


        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SetCountdownCookie(string cookieName, string cookieValue, int minutes, bool httpOnly,
            bool crossSubDomains = true)
        {
            var ts = new TimeSpan(0, minutes, 0);

            CommonHelper.SetCookie(cookieName, cookieValue, ts, httpOnly, crossSubDomains);
            return Json(true);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteCookie(string cookieName, bool crossSubDomains = true)
        {
            CommonHelper.DeleteCookie(cookieName, crossSubDomains);
            return Json(true);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult GetCookie(string cookieName)
        {
            return Json(CommonHelper.GetCookie(cookieName));
        }

        [HttpGet]
        public JsonResult GetFormSettings(int id)
        {
            var formService = new LpFormService();

            return Json(new
            {
                form = formService.Get(id),
                postActions = new List<AdvListItem>()
                {
                    new AdvListItem("Показать сообщение", (int)FormPostAction.ShowMessage),
                    new AdvListItem("Переход на страницу", (int)FormPostAction.RedrectToUrl),
                    new AdvListItem("Переход на страницу и отправка письма", (int)FormPostAction.RedrectToUrlAndEmail),
                    new AdvListItem("Переход на оплату", (int)FormPostAction.RedirectToCheckout)
                },
                crmFields = formService.GetAllFieldsList(),
                salesFunnels = SalesFunnelService.GetList()
            });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SaveFormSettings(LpForm form)
        {
            return ProcessJsonResult(new SaveFormSettings(form));
        }

        #endregion

        #region Landing email template

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SaveLandingEmailTemplate(LandingEmailTemplate model)
        {
            return ProcessJsonResult(new SaveLandingEmailTemplate(model));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteLandingEmailTemplate(int id)
        {
            new LandingEmailTemplateService().Delete(id);
            return Json(true);
        }

        public JsonResult GetLandingEmailTemplate(int id)
        {
            return Json(new LandingEmailTemplateService().Get(id));
        }

        public JsonResult GetLandingEmailTemplates(int blockId)
        {
            return Json(new LandingEmailTemplateService().GetList(blockId));
        }

        #endregion

        #region Favicon

        [HttpPost, CheckLp, ValidateJsonAntiForgeryToken]
        public JsonResult UploadFavicon(int lpId)
        {
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];

                if (file != null && file.ContentLength > 0 && file.ContentLength < 10000000)
                {
                    var result = new UploadFavicon(lpId, file).Execute();
                    return Json(result);
                }
            }

            return Json(new UploadPictureResult()
            {
                Error =
                    "Не корректное изображение. Изображение должно быть формата .ico, .png, или .gif и не больше 10 мб"
            });
        }

        [HttpPost, CheckLp, ValidateJsonAntiForgeryToken]
        public JsonResult UploadFaviconByUrl(int lpId, string url)
        {
            if (url.IsNotEmpty())
                return Json(new UploadFaviconByUrl(lpId, url).Execute());

            return Json(new UploadPictureResult() { Error = "Не передана ссылка на загрузку изображения" });
        }


        [HttpPost, CheckLp, ValidateJsonAntiForgeryToken]
        public JsonResult UploadFaviconCropped(int lpId, string base64String, string ext, int? maxWidth = null,
            int? maxHeight = null, string picture = null)
        {
            if (picture.IsNotEmpty())
                new RemovePictureHandler(picture, null).Execute();

            if (base64String.IsNotEmpty())
            {
                var result = new UploadFaviconCropped(lpId, base64String, ext, maxWidth, maxHeight).Execute();
                return Json(result);
            }

            return Json(new UploadPictureResult()
                { Error = "Не переданы обязательные параметры на загрузку изображения" });
        }

        [HttpPost, CheckLp, ValidateJsonAntiForgeryToken]
        public JsonResult RemoveFavicon(int lpId)
        {
            return Json(new UploadPictureResult() { Result = new RemoveFavicon(lpId).Execute() });
        }

        #endregion

        public ActionResult PictureLoaderTrigger(PictureLoaderTriggerModel model)
        {
            return PartialView("_PictureLoaderTrigger", model);
        }

        #region Video

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UploadVideo(List<HttpPostedFileBase> file)
        {
            if (file == null || file.Count == 0)
                return JsonError("Нет видео файлов");

            var videoFolder = FoldersHelper.GetPathAbsolut(FolderType.LandingVideo);

            FileHelpers.CreateDirectory(videoFolder);

            var fileBase = file[0];

            var fileName = fileBase.FileName;
            var ext = System.IO.Path.GetExtension(fileBase.FileName);
            var availableExtensions = new List<string>() { ".mp4" };

            if (!availableExtensions.Contains(ext))
                return JsonError("Неверный формат. Поддерживаются только mp4 видео");

            if (System.IO.File.Exists(videoFolder + fileName))
            {
                fileName = fileName.Replace(ext, "") + "_" + DateTime.Now.ToString("u").Replace(":", "-") + ext;
            }

            fileBase.SaveAs(videoFolder + fileName);

            return Json(new { url = "userfiles/lp-video/" + fileName });
        }

        public JsonResult GetVideos()
        {
            var videoFolder = FoldersHelper.GetPathAbsolut(FolderType.LandingVideo);
            var files = Directory.GetFiles(videoFolder, "*.mp4")
                .Select(x => x.Split(new[] { '\\', '/' }).LastOrDefault());

            return Json(files);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteVideo(string name)
        {
            var videoFile = FoldersHelper.GetPathAbsolut(FolderType.LandingVideo, name);
            FileHelpers.DeleteFile(videoFile);

            return JsonOk();
        }

        #endregion
    }
}