using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using AdvantShop.Catalog;
using AdvantShop.Core;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using AdvantShop.Web.Admin.ViewModels.Shared.Common;
using AdvantShop.Web.Infrastructure.Admin.Buttons;
using AdvantShop.Web.Infrastructure.Localization;

namespace AdvantShop.Web.Admin.Extensions
{
    public static class HtmlExtensions
    {
        public static MvcHtmlString Label(this HtmlHelper html, string expression, LocalizedString labelText)
        {
            return html.Label(expression, labelText.ToString());
        }

        public static MvcHtmlString Label(this HtmlHelper html, string expression, LocalizedString labelText, object htmlAttributes)
        {
            return html.Label(expression, labelText.ToString(), htmlAttributes);
        }

        public static MvcHtmlString LabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, LocalizedString labelText)
        {
            return html.LabelFor(expression, labelText.ToString());
        }

        public static MvcHtmlString LabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, LocalizedString labelText, object htmlAttributes)
        {
            return html.LabelFor(expression, labelText.ToString(), htmlAttributes);
        }

        public static MvcHtmlString Button(this HtmlHelper helper, LocalizedString text, eButtonType type = eButtonType.Simple, eButtonSize size = eButtonSize.Small,
                                           eColorType colorType = eColorType.Success, string link = null, string cssClass = null, string name = null, bool validation = false, 
                                           string[] attributes = null, bool isOutline = false, ButtonIcon icon = null, List<eButtonModificators> modificators = null, string wrapStart = null, string wrapEnd = null, string htmlElement = null)
        {
            var model = new ButtonModel()
            {
                Text = text,
                Name = name,
                Size = size,
                Type = type,
                ColorType = colorType,
                Link = link,
                Attributes = attributes,
                CssClass = cssClass,
                Validation = validation,
                IsOutline = isOutline,
                Icon = icon,
                Modificators = modificators,
                WrapStart = wrapStart,
                WrapEnd = wrapEnd,
                HtmlElement = htmlElement
            };

            return helper.Partial("_Button", model);
        }


        public static MvcHtmlString MoreButton(this HtmlHelper helper, LocalizedString text, List<MoreButtonPopoverItem> items, string ngTemplateId, eButtonType type = eButtonType.Simple, eButtonSize size = eButtonSize.Small,
                                   eColorType colorType = eColorType.Success, string link = null, string cssClass = null, string name = null, bool validation = false,
                                   string[] attributes = null, bool isOutline = false, ButtonIcon icon = null, List<eButtonModificators> modificators = null, string wrapStart = null, string wrapEnd = null, string ngIsOpen = null)
        {
            var model = new MoreButtonModel()
            {
                Text = text,
                Items = items,
                NgTemplateId = ngTemplateId,
                Name = name,
                Size = size,
                Type = type,
                ColorType = colorType,
                Link = link,
                Attributes = attributes,
                CssClass = cssClass,
                Validation = validation,
                IsOutline = isOutline,
                Icon = icon,
                Modificators = modificators,
                WrapStart = wrapStart,
                WrapEnd = wrapEnd,
                NgIsOpen = ngIsOpen
            };

            return helper.Partial("_MoreButton", model);
        }

        public static MvcHtmlString MoreButton(this HtmlHelper helper, MoreButtonModel moreButtonModel)
        {
            return helper.Partial("_MoreButton", moreButtonModel);
        }
        public static IHtmlString RenderModuleCssBundles(this HtmlHelper helper, string bundleName)
        {
            var bundles = new List<string>();

            foreach (var module in AttachedModules.GetModules<IAdminBundles>())
            {
                var instance = (IAdminBundles)Activator.CreateInstance(module);
                var items = instance.AdminCssBottom();
                if (items != null && items.Count > 0)
                    bundles.AddRange(items);
            }

            if (bundles.Count == 0)
                return new HtmlString("");

            var link = JsCssTool.MiniCss(bundles, bundleName);
            var absoluteBaseLink = UrlService.GetAbsoluteBaseLink();
            var replaced = Regex.Replace(link, "href=\"(.*)\"", $"href=\"{absoluteBaseLink.TrimEnd('/')}$1\"");
            return new HtmlString(replaced);
        }

        public static IHtmlString RenderModuleJsBundles(this HtmlHelper helper, string bundleName)
        {
            var bundles = new List<string>();

            foreach (var module in AttachedModules.GetModules<IAdminBundles>())
            {
                var instance = (IAdminBundles)Activator.CreateInstance(module);
                var items = instance.AdminJsBottom();
                if (items != null && items.Count > 0)
                    bundles.AddRange(items);
            }

            if (bundles.Count == 0)
                return new HtmlString("");
            
            var link = JsCssTool.MiniJs(bundles, bundleName);
            var absoluteBaseLink = UrlService.GetAbsoluteBaseLink();
            var replaced = Regex.Replace(link, "href=\"(.*)\"", $"href=\"{absoluteBaseLink.TrimEnd('/')}$1\"");
            return new HtmlString(replaced);
        }

        public static MvcHtmlString BootstrapPager(this HtmlHelper helper, int currentPageIndex, Func<int, string> action, int totalItems, int pageSize = 10, int numberOfLinks = 5)
        {
            if (totalItems <= 0)
            {
                return MvcHtmlString.Empty;
            }

            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            var lastPageNumber = (int)Math.Ceiling((double)currentPageIndex / numberOfLinks) * numberOfLinks;
            var firstPageNumber = lastPageNumber - (numberOfLinks - 1);
            var hasPreviousPage = currentPageIndex > 1;
            var hasNextPage = currentPageIndex < totalPages;
            if (lastPageNumber > totalPages)
            {
                lastPageNumber = totalPages;
            }

            var ul = new TagBuilder("ul");
            ul.AddCssClass("pagination");
            ul.InnerHtml += AddLink(1, action, currentPageIndex == 1, "disabled", "«", "First Page");
            ul.InnerHtml += AddLink(currentPageIndex - 1, action, !hasPreviousPage, "disabled", "‹", "Previous Page");
            for (int i = firstPageNumber; i <= lastPageNumber; i++)
            {
                ul.InnerHtml += AddLink(i, action, i == currentPageIndex, "active", i.ToString(), i.ToString());
            }

            ul.InnerHtml += AddLink(currentPageIndex + 1, action, !hasNextPage, "disabled", "›", "Next Page");
            ul.InnerHtml += AddLink(totalPages, action, currentPageIndex == totalPages, "disabled", "»", "Last Page");
            return MvcHtmlString.Create(ul.ToString());
        }

        private static TagBuilder AddLink(int index, Func<int, string> action, bool condition, string classToAdd, string linkText, string tooltip)
        {
            var li = new TagBuilder("li");
            li.MergeAttribute("title", tooltip);
            if (condition)
            {
                li.AddCssClass(classToAdd);
            }
            var a = new TagBuilder("a");
            a.MergeAttribute("href", !condition ? action(index) : "javascript:");
            a.SetInnerText(linkText);
            li.InnerHtml = a.ToString();
            return li;
        }

        public static MvcHtmlString PictureUploader(this HtmlHelper helper, PhotoType photoType, int objId,
                                                    string startSrc, string ngOnUpdateCallback = null, object htmlAttributes = null, 
                                                    int? pictureId = null, 
                                                    EFileType fileTypes = EFileType.Image)
        {
            var model = new PictureUploader
            {
                StartSrc = startSrc,
                ObjId = objId,
                PhotoType = photoType,
                NgOnUpdateCallback = ngOnUpdateCallback,
                PictureId = pictureId,
                FileTypes = FileHelpers.GetAllowedFileExtensions(fileTypes)
            };

            var attrs = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

            foreach (var item in attrs)
            {
                model.HtmlAttributes += $" {item.Key}={item.Value}";
            }

            switch (photoType)
            {
                case PhotoType.CategoryIcon:
                case PhotoType.CategorySmall:
                case PhotoType.CategoryBig:
                    model.UploadUrl = "/category/uploadPicture";
                    model.UploadByLinkUrl = "/category/uploadPictureByLink";
                    model.DeleteUrl = "/category/deletePicture";
                    break;
                case PhotoType.Brand:
                    model.UploadUrl = "/brands/uploadpicture";
                    model.UploadByLinkUrl = "/brands/uploadPictureByLink";
                    model.DeleteUrl = "/brands/deletePicture";
                    break;
                case PhotoType.News:
                    model.UploadUrl = "/news/uploadpicture";
                    model.UploadByLinkUrl = "/news/uploadPictureByLink";
                    model.DeleteUrl = "/news/deletePicture";
                    break;
                case PhotoType.Logo:
                    model.UploadUrl = "/settings/uploadLogo";
                    model.UploadByLinkUrl = "/settings/uploadLogoByLink";
                    model.DeleteUrl = "/settings/deleteLogo";
                    break;
                case PhotoType.LogoSvg:
                    model.UploadUrl = "/settings/uploadLogoSvg";
                    model.UploadByLinkUrl = "/settings/uploadLogoSvgByLink";
                    model.DeleteUrl = "/settings/deleteLogoSvg";
                    break;
                case PhotoType.LogoMobile:
                    model.UploadUrl = "/settings/uploadLogoMobile";
                    model.UploadByLinkUrl = "/settings/uploadLogoMobileByLink";
                    model.DeleteUrl = "/settings/deleteLogoMobile";
                    break;
                case PhotoType.Favicon:
                    model.UploadUrl = "/settings/uploadFavicon";
                    model.UploadByLinkUrl = "/settings/uploadFaviconByLink";
                    model.DeleteUrl = "/settings/deleteFavicon";
                    break;
                case PhotoType.MobileApp:
                    model.UploadUrl = "/mobileApp/UploadIcon";
                    model.UploadByLinkUrl = "/mobileApp/UploadIconByLink";
                    model.DeleteUrl = "/mobileApp/DeleteIcon";
                    break;
                case PhotoType.LandingMobileApp:
                    model.UploadUrl = "/funnels/UploadIcon";
                    model.UploadByLinkUrl = "/funnels/UploadIconByLink";
                    model.DeleteUrl = "/funnels/DeleteIcon";
                    break;
                case PhotoType.LogoBlog:
                    model.UploadUrl = "/settings/uploadLogoBlog";
                    model.UploadByLinkUrl = "/settings/uploadLogoBlogByLink";
                    model.DeleteUrl = "/settings/deleteLogoBlog";
                    break;
                case PhotoType.DarkThemeLogo:
                    model.UploadUrl = "/settings/uploadDarkThemeLogo";
                    model.UploadByLinkUrl = "/settings/uploadDarkThemeLogoByLink";
                    model.DeleteUrl = "/settings/deleteDarkThemeLogo";
                    break;
                default:
                    throw new Exception($"Not support photoType {photoType.ToString()} for pictureUploader");
            }

            return helper.Action("PictureUploader", "Common", model);
        }

        public static MvcHtmlString Back(this HtmlHelper helper, string text, string url, string classes = "", string attributes = "", string ngBackTriggerCallback = "")
        {
            return helper.Action("Back", "Common", new BackViewModel() { Text = text, Url = url, Classes = classes, Attributes = attributes, NgBackTriggerCallback = ngBackTriggerCallback });
        }

        public static MvcHtmlString TextBoxSuggest(this HtmlHelper helper, string name, object value, Dictionary<string, object> htmlAttributes, int customerFieldId, CustomerFieldAssignment fieldAssignment = CustomerFieldAssignment.None, string onSelectFunc = null)
        {
            ModulesExecuter.GetSuggestionsHtmlAttributes(customerFieldId, htmlAttributes, fieldAssignment, onSelectFunc);

            return helper.TextBox(name, value, htmlAttributes);
        }

        public static MvcHtmlString TextAreaSuggest(this HtmlHelper helper, string name, string value, Dictionary<string, object> htmlAttributes, int customerFieldId, CustomerFieldAssignment fieldAssignment = CustomerFieldAssignment.None, string onSelectFunc = null)
        {
            ModulesExecuter.GetSuggestionsHtmlAttributes(customerFieldId, htmlAttributes, fieldAssignment, onSelectFunc);

            return helper.TextArea(name, value, htmlAttributes);
        }

        public static MvcHtmlString Header(this HtmlHelper helper, HeaderViewModel model)
        {
            return helper.Action("Header", "Common", model);
        }
    }
}