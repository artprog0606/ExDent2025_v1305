using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Web.Admin.Handlers.Catalog.CustomOptions;
using AdvantShop.Web.Infrastructure.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Core;
using System.Web;

namespace AdvantShop.Web.Admin.Controllers.Catalog
{
    public class CustomOptionsController : BaseAdminController
    {
        [HttpGet]
        public JsonResult GetCustomOptions(int productId)
        {
            if (productId == 0)
                return JsonError();

            var customOptions = CustomOptionsService.GetCustomOptionsByProductId(productId);

            return JsonOk(customOptions);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SaveCustomOptions(List<CustomOption> customOptions, int productId)
        {
            if (!ModelState.IsValid)
                return JsonError();

            CustomOptionsService.SubmitCustomOptionsWithSameProductId(productId, customOptions ?? new List<CustomOption>());
            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteCustomOption(int customOptionId)
        {
            if (FeaturesService.IsEnabled(EFeature.CustomOptionPicture))
            {
                var options = CustomOptionsService.GetCustomOptionItems(customOptionId);
                foreach (var option in options)
                {
                    if (option.PictureData != null)
                    {
                        option.PictureData.PhotoName = null;
                        PhotoService.DeletePhotos(option.ID, PhotoType.CustomOptions);
                    }
                        
                }
            }

            CustomOptionsService.DeleteCustomOption(customOptionId);

            return JsonOk();
        }

        [HttpGet]
        public JsonResult GetDefaultCustomOptionValues(int productId)
        {
            var option = GetDefaultOption();

            return JsonOk(new CustomOption
            {
                Title = string.Empty,
                ProductId = productId,
                IsRequired = false,
                InputType = CustomOptionInputType.DropDownList,
                Options = new List<OptionItem> { option },
                SelectedOptions = new List<OptionItem> { option }
            });
        }

        private OptionItem GetDefaultOption()
        {
            var noPhotoUrl = UrlService.GetUrl() + "images/nophoto_xsmall.png";
            return new OptionItem
            {
                Title = string.Empty,
                PriceType = OptionPriceType.Fixed,
                PictureData = new OptionItemPhoto()
                {
                    PictureUrl = noPhotoUrl
                }
            };
        }

        [HttpGet]
        public JsonResult GetFormData()
        {
            var usePicture = FeaturesService.IsEnabled(EFeature.CustomOptionPicture);
            var comboEnabled = FeaturesService.IsEnabled(EFeature.CustomOptionCombo);
            var noPhotoUrl = UrlService.GetUrl() + "images/nophoto_xsmall.png";
            var inputTypeList = Enum.GetValues(typeof(CustomOptionInputType))
                .Cast<CustomOptionInputType>()
                .Where(x => x != CustomOptionInputType.ChoiceOfProduct)
                .Select(x => 
                    new
                    {
                        Text = x.Localize(),
                        Value = (int)x
                    }).ToList();

            var priceTypeList = Enum.GetValues(typeof(OptionPriceType))
                .Cast<OptionPriceType>()
                .Select(x =>
                    new
                    {
                        Text = x.Localize(),
                        Value = (int)x
                    }).ToList();

            return Json(new
            {
                inputTypeList = inputTypeList,
                priceTypeList = priceTypeList,
                usePicture = usePicture,
                noPhotoUrl = noPhotoUrl,
                comboEnabled = comboEnabled
            });
        }

        #region Options grid

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteOption(int optionId)
        {
            if (FeaturesService.IsEnabled(EFeature.CustomOptionPicture))
                PhotoService.DeletePhotos(optionId, PhotoType.CustomOptions);

            CustomOptionsService.DeleteOption(optionId);

            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UpdateOption(OptionItem model)
        {
            CustomOptionsService.UpdateOption(model, model.CustomOptionsId);
            return JsonOk();
        }

        [HttpGet]
        public JsonResult GetDefaultOptionValues()
        {
            return JsonOk(GetDefaultOption());
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UpdatePhoto(int optionId, HttpPostedFileBase file)
        {
            return ProcessJsonResult(() =>
                new UploadCustomOptionPhoto(optionId, file).Execute());
        }

        [HttpGet]
        public JsonResult GetProductPhotosByOfferIds(List<int> offerIds)
        {
            var result = CustomOptionsService.GetOptionsWithProductPhotoByOfferIds(offerIds);
            return JsonOk(result);
        }

        #endregion
    }
}
