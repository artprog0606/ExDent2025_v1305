using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.App.Landing.Models.Landing;
using AdvantShop.Core.Caching;
using AdvantShop.App.Landing.Models;
using System.Collections.Generic;
using System.Web;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.Services.Landing.Blocks;
using AdvantShop.Core.Services.Landing.Forms;
using Newtonsoft.Json;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Landing.Pictures;

namespace AdvantShop.App.Landing.Extensions
{
    public static class LpExtensions
    {
        private static double CacheTime(LpBlock block)
        {
            return !block.NoCache && !(block.ShowOnAllPagesTop || block.ShowOnAllPagesBottom)
                ? LpConstants.LpCacheTime
                : 0;
        }

        public static MvcHtmlString Block(this HtmlHelper helper, int blockId, bool? useCache = true, bool? inplace = null)
        {
            var cacheName = string.Format(LpConstants.BlockRenderedCachePrefix, LpService.Inplace, LpService.EntityId, blockId, useCache);
            var cacheTime =
                useCache != null && useCache.Value && LpService.EntityId == 0
                    ? LpConstants.LpBlocksCacheTime
                    : 0;

            return CacheManager.Get(cacheName, cacheTime,
                () =>
                    helper.Action("Block", "Landing", new RouteValueDictionary()
                    {
                        {"id", blockId},
                        {"inplace", inplace}
                    }));
        }

        public static MvcHtmlString SubBlock(this HtmlHelper helper, BlockModel model, string name, bool? inplace = null, bool? hidePlaceholder = null)
        {
            var block = model.Block;

            if (inplace != null)
                return helper.Action("SubBlock", "Landing", new RouteValueDictionary()
                {
                    {"blockId", block.Id},
                    {"name", name},
                    {"inplace", inplace.Value},
                    {"hidePlaceholder", hidePlaceholder}
                });

            var cacheName = string.Format(LpConstants.SubBlockRenderedCachePrefix, model.Inplace, LpService.EntityId, block.Id, name);

            return CacheManager.Get(cacheName, CacheTime(block),
                () =>
                    helper.Action("SubBlock", "Landing", new RouteValueDictionary()
                    {
                        {"blockId", block.Id},
                        {"name", name},
                        {"inplace", model.Inplace},
                        {"hidePlaceholder", hidePlaceholder}
                    }));
        }

        public static MvcHtmlString SubBlock(this HtmlHelper helper, LpBlock block, string name, bool? inplace = null, bool? hidePlaceholder = null)
        {
            if (inplace != null)
                return helper.Action("SubBlock", "Landing", new RouteValueDictionary()
                {
                    {"blockId", block.Id},
                    {"name", name},
                    {"inplace", inplace.Value},
                    {"hidePlaceholder", hidePlaceholder}
                });

            var cacheName = string.Format(LpConstants.SubBlockRenderedCachePrefix, LpService.Inplace, LpService.EntityId, block.Id, name);

            return CacheManager.Get(cacheName, CacheTime(block),
                () =>
                    helper.Action("SubBlock", "Landing", new RouteValueDictionary()
                    {
                        {"blockId", block.Id},
                        {"name", name},
                        {"inplace", LpService.Inplace},
                        {"hidePlaceholder", hidePlaceholder}
                    }));
        }

        public static MvcHtmlString SubBlockPicture(this HtmlHelper helper, LpBlock block, string name, PictureLoaderTriggerModel pictureLoaderModel, bool? inplace = null)
        {
            if (inplace != null)
                return helper.Action("SubBlockPicture", "Landing",
                    new RouteValueDictionary()
                    {
                        {"blockId", block.Id},
                        {"name", name},
                        {"pictureLoaderModel", pictureLoaderModel},
                        {"inplace", inplace.Value}
                    });

            var cacheName = string.Format(LpConstants.SubBlockRenderedCachePrefix, LpService.Inplace, LpService.EntityId, block.Id, name);

            return CacheManager.Get(cacheName, CacheTime(block),
                () => helper.Action("SubBlockPicture", "Landing", new RouteValueDictionary() {{"blockId", block.Id}, {"name", name}}));
        }


        //public static MvcHtmlString SubBlock(this HtmlHelper helper, LpSubBlock subblock, string name)
        //{
        //    var cacheName = string.Format(LpConstants.SubBlockRendered2CachePrefix, LpService.Inplace, subblock.LandingBlockId, name);

        //    return CacheManager.Get(cacheName, LpConstants.LpCacheTime,
        //        () => helper.Action("SubBlock", "Landing", new RouteValueDictionary() {{"blockId", subblock.LandingBlockId}, {"name", name}}));
        //}

        public static MvcHtmlString Form(this HtmlHelper helper, LpForm form, bool isVertical = true, eFormAlign align = eFormAlign.None)
        {
            //default align for horizontal form
            if (!isVertical && align == eFormAlign.None)
            {
                align = eFormAlign.Center;
            }

           return helper.Partial("_Form", new LpFormModel() {Form = form, IsVertical = isVertical, Align = align });
        }

        public static MvcHtmlString Button(this HtmlHelper helper, LpBlock block, string buttonName, Dictionary<string, string> customFields = null)
        {
            var button = block.TryGetSetting<LpButton>(buttonName);
            if (button == null)
                return new MvcHtmlString("");

            if (customFields != null || (button.ActionOfferId != null && (button.UseManyOffers == null || button.UseManyOffers == false)))
                button.AdditionalData = JsonConvert.SerializeObject(new
                {
                    customFields,
                    offerId = button.UseManyOffers != true ? button.ActionOfferId : null
                });

            button.BlockId = block.Id;
            button.Name = buttonName;

            return helper.Partial("_Button", button);
        }

        public static MvcHtmlString Button(this HtmlHelper helper, LpBlock block, LpButton button, Dictionary<string, string> customFields = null, string buttonTitle = null)
        {
            if (button == null)
                return new MvcHtmlString("");

            if (customFields != null || button.ActionOfferId != null || buttonTitle != null)
                button.AdditionalData = JsonConvert.SerializeObject(new { customFields, offerId = button.ActionOfferId, buttonTitle });

            button.BlockId = block.Id;

            return helper.Partial("_Button", button);
        }

        public static MvcHtmlString Button(this HtmlHelper helper, LpBlock block, LpButton button, int offerId, Dictionary<string, string> customFields = null)
        {
            if (button == null)
                return new MvcHtmlString("");

            button.AdditionalData = JsonConvert.SerializeObject(new {customFields, offerId});
            button.BlockId = block.Id;

            return helper.Partial("_Button", button);
        }


        public static string ButtonHref(this HtmlHelper helper, LpButton button)
        {
            string href = "javascript:void(0)";

            if (button.Action == LpButtonAction.Url.ToString())
            {
                if (button.ActionUrlLpId != null)
                {
                    href = LpService.IgnoredLpId != null && LpService.IgnoredLpId.Value.ToString() == button.ActionUrlLpId
                            ? LpService.GetUrlWithEntityPrefix("checkout/success")
                            : LpService.GetUrlWithEntityPrefix(new LpService().GetLpLink(button.ActionUrlLpId.TryParseInt()));
                }
                else if (!string.IsNullOrWhiteSpace(button.ActionUrl))
                {
                    href = button.ActionUrl[0] == '#' ? UrlService.GetCurrentUrl(button.ActionUrl) : LpService.GetUrlWithEntityPrefix(button.ActionUrl);
                }
            }
            return href;
        }

        public static string LStaticUrl(this HtmlHelper helper)
        {
            return UrlService.GetUrl() + "landing/";
        }

        public static MvcHtmlString InplaceSettings(this HtmlHelper helper, int blockId, string settings, string property, string placeholder = "Нажмите сюда, чтобы добавить описание")
        {
            return new MvcHtmlString(LpService.Inplace ? string.Format("data-inplace-rich placeholder=\"{3}\" inplace-on-save='blocksConstructorMain.saveSettings(value, $scope, {0}, {1}, \"{2}\")'", blockId, settings, property, placeholder) : "");
        }

        public static MvcHtmlString InplaceSubBlockContent(this HtmlHelper helper, int subBlockId, string subblockname, string placeholder = "Нажмите сюда, чтобы добавить описание", bool hideplaceholder = false)
        {
            return new MvcHtmlString(LpService.Inplace ? string.Format("data-inplace-rich data-inplace-on-save=\"blocksConstructorContainer.onInplaceSaveSubblock('{2}', 'ContentHtml', value)\"  placeholder=\"{1} \" data-inplace-params=\"{{ subBlockId : {0}}}\" ", subBlockId, placeholder, subblockname) : "");

        }

        public static MvcHtmlString InplaceFormSettings(this HtmlHelper helper, int blockId, string settings, string property, string placeholder = "Нажмите сюда, чтобы добавить описание")
        {
            return new MvcHtmlString(LpService.Inplace ? string.Format("data-inplace-rich placeholder=\"{3}\" inplace-on-save='blocksConstructorMain.saveFormSettings(value, {0}, {1}, \"{2}\")'", blockId, settings, property, placeholder) : "");
        }

        public static MvcHtmlString BlockHeader(this HtmlHelper helper, BlockModel model)
        {
            return helper.Partial("_BlockHeader", model);
        }

        public static MvcHtmlString PictureLoaderTrigger(this HtmlHelper helper, PictureLoaderTriggerModel model)
        {
            return helper.Action("PictureLoaderTrigger", "LandingInplace", model);
        }

        public static MvcHtmlString InplaceEscapeScripts(this HtmlHelper helper, string content)
        {
            return new MvcHtmlString(LpService.Inplace ? content.Replace("<script>", "<script type=\"inplace\">").Replace("type=\"text/javascript\"", "type=\"inplace\"") : content);
        }

        public static IHtmlString GetLandingCanonicalTag(this HtmlHelper helper)
        {
            var url = new LpService().GetLpLink(HttpContext.Current.Request.Url.Host, LpService.CurrentLanding);

            return new HtmlString(url != null ? "<link rel=\"canonical\" href=\"" + url + "\" />" : null);
        }
    }
}
