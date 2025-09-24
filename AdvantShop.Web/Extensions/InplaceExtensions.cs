using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AdvantShop.Core.Services.InplaceEditor;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Customers;
using AdvantShop.SEO;
using AdvantShop.FilePath;
using AdvantShop.Helpers;

namespace AdvantShop.Extensions
{
    public static class InplaceExtensions
    {

        const string richSimple = "{editorSimple: true}";

        const string richTpl = "data-inplace-rich=\"{4}\" data-inplace-url=\"{3}\" data-inplace-params=\"{{id: {0}, type: '{1}', field: '{2}'}}\" {6} placeholder=\"{5}\" data-inplace-on-save=\"{7}\"";

        public static string InplaceStringFormat<T>(string id, InplaceType type, T field, string url, string rich, string placeholder, bool isBindable = false, string ngSaveCallback = "") where T: IComparable, IFormattable, IConvertible
        {
            return String.Format(richTpl, id, type, field, url, rich, placeholder, !isBindable ? "ng-non-bindable" : string.Empty, ngSaveCallback);
        }

        public static string InplaceStringFormat(string id, string type, string field, string url, string rich, string placeholder, bool isBindable = false, string ngSaveCallback = "")
        {
            return String.Format(richTpl, id, type, field, url, rich, placeholder, !isBindable ? "ng-non-bindable" : string.Empty, ngSaveCallback);
        }

        // Static page
        public static HtmlString InplaceStaticPage(this HtmlHelper helper, int staticPageId, StaticPageInplaceField field)
        {
            if (!InplaceEditorService.CanUseInplace(RoleAction.Store))
                return new HtmlString("");
            
            return new HtmlString(InplaceStringFormat(staticPageId.ToString(), InplaceType.StaticPage, field, "inplaceeditor/staticpage", string.Empty, "Нажмите сюда, чтобы добавить описание"));
        }

        // Static block
        public static HtmlString InplaceStaticBlock(this HtmlHelper helper, int staticBlockId)
        {
            if (!InplaceEditorService.CanUseInplace(RoleAction.Store))
                return new HtmlString("");
            
            return new HtmlString(InplaceStringFormat(staticBlockId.ToString(), InplaceType.StaticBlock, StaticBlockInplaceField.Content, "inplaceeditor/staticblock", string.Empty, "Нажмите сюда, чтобы добавить описание"));
        }

        // News
        public static HtmlString InplaceNews(this HtmlHelper helper, int newsId, NewsInplaceField field)
        {
            if (!InplaceEditorService.CanUseInplace(RoleAction.Store))
                return new HtmlString("");

            return new HtmlString(InplaceStringFormat(newsId.ToString(), InplaceType.NewsItem, field, "inplaceeditor/news", string.Empty, "Нажмите сюда, чтобы добавить описание"));
        }

        // Tags

        public static HtmlString InplaceTag(this HtmlHelper helper, int TagId, TagInplaceField field)
        {
            if (!InplaceEditorService.CanUseInplace(RoleAction.Catalog))
                return new HtmlString("");

            return new HtmlString(InplaceStringFormat(TagId.ToString(), InplaceType.Category, field, "inplaceeditor/tag", string.Empty, "Нажмите сюда, чтобы добавить описание"));
        }

        // Category

        public static HtmlString InplaceCategory(this HtmlHelper helper, int categoryId, CategoryInplaceField field)
        {
            if (!InplaceEditorService.CanUseInplace(RoleAction.Catalog))
                return new HtmlString("");

            return new HtmlString(InplaceStringFormat(categoryId.ToString(), InplaceType.Category, field, "inplaceeditor/category", string.Empty, "Нажмите сюда, чтобы добавить описание"));
        }
        
        /// <summary>
        /// reviewId принимает либо int id отзыва, либо имя параметра в модели ангуляра типа string
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="reviewId"></param>
        /// <param name="field"></param>
        /// <param name="editorSimple"></param>
        /// <returns></returns>
        public static HtmlString InplaceReview(this HtmlHelper helper, string reviewId, ReviewInplaceField field, bool editorSimple = false, bool isBindable = false)
        {
            if (!InplaceEditorService.CanUseInplace(RoleAction.Catalog))
                return new HtmlString("");

            return new HtmlString(InplaceStringFormat(reviewId, InplaceType.Review, field, "inplaceeditor/review", editorSimple ? richSimple : string.Empty, "Нажмите сюда, чтобы добавить описание", isBindable));
        }


        // Product
        public static HtmlString InplaceProduct(this HtmlHelper helper, int productId, ProductInplaceField field, bool editorSimple = false, bool bindable = false)
        {
            if (!InplaceEditorService.CanUseInplace(RoleAction.Catalog))
                return new HtmlString("");

            return new HtmlString(InplaceStringFormat(productId.ToString(), InplaceType.Product, field, "inplaceeditor/product", editorSimple ? richSimple : string.Empty, "Нажмите сюда, чтобы добавить описание", bindable));
        }

        public static HtmlString InplaceProductUnit(int productId, ProductInplaceField field)
        {
            if (!InplaceEditorService.CanUseInplace(RoleAction.Catalog))
                return new HtmlString("");

            return new HtmlString(InplaceStringFormat(productId.ToString(), InplaceType.Product, field, "inplaceeditor/product", richSimple, LocalizationService.GetResource("Inplace.InplaceProductUnit.Unit").ToLower()));
        }

        // Offer
        public static HtmlString InplaceOfferPrice(this HtmlHelper helper)
        {
            if (!InplaceEditorService.CanUseInplace(RoleAction.Catalog))
                return new HtmlString("");


            return
                new HtmlString(String.Format("data-inplace-price data-inplace-url=\"inplaceeditor/offer\" data-inplace-params=\"{{'type':'{0}'}}\"",
                    InplaceType.Offer));
        }

        public static HtmlString InplaceOfferAmount(int offerId)
        {
            if (!InplaceEditorService.CanUseInplace(RoleAction.Catalog))
                return new HtmlString("");

            return
                new HtmlString(InplaceStringFormat(offerId.ToString(), InplaceType.Offer, OfferInplaceField.Amount, "inplaceeditor/offer", richSimple, "Нажмите сюда, чтобы добавить описание"));
        }

        public static HtmlString InplaceOfferArtNo(this HtmlHelper helper, string ngOfferId, bool bindable = false, string ngSaveCallback = "")
        {
            if (!InplaceEditorService.CanUseInplace(RoleAction.Catalog))
                return new HtmlString("");

            return
                new HtmlString(InplaceStringFormat(ngOfferId, InplaceType.Offer, OfferInplaceField.ArtNo, "inplaceeditor/offer", richSimple, "Нажмите сюда, чтобы добавить описание", bindable, ngSaveCallback));
        }

        public static HtmlString InplaceOffer(this HtmlHelper helper, string ngOfferId, OfferInplaceField field, string ngSaveCallback = "", bool bindable = false)
        {
            if (string.IsNullOrEmpty(ngOfferId) || !InplaceEditorService.CanUseInplace(RoleAction.Catalog))
                return new HtmlString("");

            return
                new HtmlString(InplaceStringFormat(ngOfferId, InplaceType.Offer, field, "inplaceeditor/offer",
                    richSimple, "Нажмите сюда, чтобы добавить описание", bindable, ngSaveCallback: ngSaveCallback));
        }

        // Brand
        public static HtmlString InplaceBrand(this HtmlHelper helper, int brandId, BrandInplaceField field)
        {
            if (!InplaceEditorService.CanUseInplace(RoleAction.Catalog))
                return new HtmlString("");

            return
                new HtmlString(InplaceStringFormat(brandId.ToString(), InplaceType.Brand, field, "inplaceeditor/brand", string.Empty, "Нажмите сюда, чтобы добавить описание"));
        }

        // Property
        public static HtmlString InplaceProperty(this HtmlHelper helper, int propertyValueId, int propertyId, int productId, PropertyInplaceField field, string selectorBlock = "")
        {
            if (!InplaceEditorService.CanUseInplace(RoleAction.Catalog))
                return new HtmlString("");

            return
                new HtmlString(String.Format(
                        "data-inplace-autocomplete=\"{{propertyId: {1}, productId: {2}}}\" data-inplace-url=\"inplaceeditor/property\" data-inplace-params=\"{{'id':{0}, propertyId: {1}, productId: {2}, 'type':'{3}', 'field':'{4}'}}\" data-inplace-autocomplete-selector-block=\"{5}\"",
                        propertyValueId,
                        propertyId,
                        productId,
                        InplaceType.Property,
                        field,
                        selectorBlock));
        }

        // Meta
        public static HtmlString InplaceMeta(this HtmlHelper helper, int id, MetaType metaType)
        {
            if ((metaType == MetaType.Brand || metaType == MetaType.Category || metaType == MetaType.Product || metaType == MetaType.Tag || metaType == MetaType.MainPageProducts || metaType == MetaType.MainPageBrands)
                && !InplaceEditorService.CanUseInplace(RoleAction.Catalog))
            {
                return new HtmlString("");
            }

            if ((metaType == MetaType.News || metaType == MetaType.NewsCategory || metaType == MetaType.StaticPage || metaType == MetaType.PageWarehouses)
                && !InplaceEditorService.CanUseInplace(RoleAction.Store))
            {
                return new HtmlString("");
            }

            return
                new HtmlString(String.Format(
                        " data-inplace-modal data-inplace-url=\"inplaceeditor/meta\" data-inplace-params=\"{{'id':{0}, 'type':'{1}', metaType: '{2}'}}\"",
                        id, InplaceType.Meta, metaType));
        }

        // Image
        public static string InplaceImageLogo(bool ignoreCheck = false)
        {
            if (ignoreCheck == false && !InplaceEditorService.CanUseInplace(RoleAction.Settings))
                return string.Empty;

            return String.Format(
                        "data-inplace-image data-ngf-drop=\"\" accept=\"{2}\" data-ngf-accept=\"'{2}'\" data-ngf-change=\"inplaceImage.fileDrop($files, $event, $rejectedFiles)\"  " +
                        "data-inplace-url=\"inplaceeditor/image\" data-inplace-params=\"{{field: '{0}'}}\" {1}",
                        ImageInplaceField.Logo, InplaceImageButtonsLogo(updateVisible: false), 
                        string.Join("," ,FileHelpers.GetAllowedFileExtensions(EFileType.Image | EFileType.Svg)));
        }

        public static HtmlString InplaceImageBrand(this HtmlHelper helper, int brandId)
        {
            if (!InplaceEditorService.CanUseInplace(RoleAction.Catalog))
                return new HtmlString("");

            return
                new HtmlString(String.Format(
                        "data-inplace-image data-ngf-drop=\"\" accept=\"image/*\" data-ngf-accept=\"'image/*'\" data-ngf-change=\"inplaceImage.fileDrop($files, $event, $rejectedFiles)\" " +
                        "data-inplace-url=\"inplaceeditor/image\" data-inplace-params=\"{{'id':{0}, 'type':'{1}', 'field':'{2}'}}\" {3}",
                        brandId,
                        InplaceType.Image,
                        ImageInplaceField.Brand,
                        InplaceImageButtons(false)));
        }

        public static HtmlString InplaceImageNews(this HtmlHelper helper, int newsId)
        {
            if (!InplaceEditorService.CanUseInplace(RoleAction.Store))
                return new HtmlString("");

            return
                new HtmlString(String.Format(
                        "data-inplace-image data-ngf-drop=\"\" accept=\"image/*\" data-ngf-accept=\"'image/*'\" data-ngf-change=\"inplaceImage.fileDrop($files, $event, $rejectedFiles)\" " +
                        "data-inplace-url=\"inplaceeditor/image\" data-inplace-params=\"{{'id':{0}, 'type':'{1}', 'field':'{2}'}}\" {3}",
                        newsId,
                        InplaceType.Image,
                        ImageInplaceField.News,
                        InplaceImageButtons(false)));
        }

        public static HtmlString InplaceImageCarousel(this HtmlHelper helper, int slideId, bool specialSlideForInplace)
        {
            if (!InplaceEditorService.CanUseInplace(RoleAction.Store))
                return new HtmlString("");

            return
                new HtmlString(String.Format(
                        "data-inplace-image data-ngf-drop=\"\" accept=\"image/*\" data-ngf-accept=\"'image/*'\" data-ngf-change=\"inplaceImage.fileDrop($files, $event, $rejectedFiles)\" " +
                        "data-inplace-url=\"inplaceeditor/image\" data-inplace-params=\"{{'id':{0}, 'type':'{1}', 'field':'{2}'}}\" {3}",
                        slideId,
                        InplaceType.Image,
                        ImageInplaceField.Carousel,
                        InplaceImageButtons(true, !specialSlideForInplace, !specialSlideForInplace, specialSlideForInplace)));
        }

        public static HtmlString InplaceImageCategory(this HtmlHelper helper, int categoryId, ImageInplaceField field)
        {
            if (!InplaceEditorService.CanUseInplace(RoleAction.Catalog))
                return new HtmlString("");

            return
                new HtmlString(String.Format(
                        "data-inplace-image data-ngf-drop=\"\" accept=\"image/*\" data-ngf-accept=\"'image/*'\" data-ngf-change=\"inplaceImage.fileDrop($files, $event, $rejectedFiles)\" " +
                        "data-inplace-url=\"inplaceeditor/image\" data-inplace-params=\"{{'id':{0}, 'type':'{1}', 'field':'{2}'}}\" {3}",
                        categoryId,
                        InplaceType.Image,
                        field.ToString(),
                        InplaceImageButtons(false)));
        }

        /// <summary>
        /// reviewId принимает либо int id отзыва, либо имя параметра в модели ангуляра типа string
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="ngModelReviewId"></param>
        /// <returns></returns>
        public static HtmlString InplaceImageReview(this HtmlHelper helper, string photoId, string reviewId, ReviewImageType reviewImageType = ReviewImageType.Small)
        {
            if (!InplaceEditorService.CanUseInplace(RoleAction.Catalog))
                return new HtmlString("");

            return
                new HtmlString(String.Format(
                        "data-inplace-image data-ngf-drop=\"\" accept=\"image/*\" data-ngf-accept=\"'image/*'\" data-ngf-change=\"inplaceImage.fileDrop($files, $event, $rejectedFiles)\" " +
                        "data-inplace-url=\"inplaceeditor/image\" data-inplace-params=\"{{'id':{0}, 'type':'{1}', 'field':'{2}', objId:{3}, additionalData: '{4}'}}\" {5}",
                        photoId,
                        InplaceType.Image,
                        ImageInplaceField.Review,
                        reviewId,
                        reviewImageType.ToString(),
                        InplaceImageButtons(false)));
        }

        public static HtmlString InplaceImageProduct(this HtmlHelper helper, int photoId, int productId, ProductImageType productImageType)
        {
            if (!InplaceEditorService.CanUseInplace(RoleAction.Catalog))
                return new HtmlString("");

            return new HtmlString(String.Format(
                    "data-inplace-image data-ngf-drop=\"\" accept=\"image/*\" data-ngf-accept=\"'image/*'\" data-ngf-change=\"inplaceImage.fileDrop($files, $event, $rejectedFiles)\" " +
                    "data-inplace-url=\"inplaceeditor/image\" data-inplace-params=\"{{'id':{0}, 'type':'{1}', 'field':'{2}', objId:{3}, additionalData: '{4}'}}\" {5}",
                    photoId,
                    InplaceType.Image,
                    ImageInplaceField.Product,
                    productId,
                    productImageType.ToString(),
                    InplaceImageButtons(updateVisible: photoId != 0)));
        }

        private static string InplaceImageButtons(bool addVisible = true, bool updateVisible = true, bool deleteVisible = true, bool permanentVisible = false)
        {
            return string.Format("data-inplace-image-buttons-visible=\"{{'add': {0}, 'update': {1}, 'delete' : {2}, 'permanentVisible': {3}}}\"",
                addVisible.ToString().ToLower(),
                updateVisible.ToString().ToLower(),
                deleteVisible.ToString().ToLower(),
                permanentVisible.ToString().ToLower());
        }

        private static string InplaceImageButtonsLogo(bool addVisible = true, bool updateVisible = true, bool deleteVisible = true, bool permanentVisible = false, bool editLogo = true)
        {
            return string.Format("data-inplace-image-buttons-visible=\"{{'add': {0}, 'update': {1}, 'delete' : {2}, 'permanentVisible': {3}, 'editLogo': {4}}}\"",
                addVisible.ToString().ToLower(),
                updateVisible.ToString().ToLower(),
                deleteVisible.ToString().ToLower(),
                permanentVisible.ToString().ToLower(),
                editLogo.ToString().ToLower());
        }

        // Modules
        public static HtmlString InplaceModule(this HtmlHelper helper, int id, string entityType, string field)
        {
            if (!InplaceEditorService.CanUseInplace(RoleAction.Modules))
                return new HtmlString("");

            return new HtmlString(InplaceStringFormat(id.ToString(), entityType, field, "inplaceeditor/module", string.Empty, "Нажмите сюда, чтобы добавить описание"));
        }
    }
}