using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using AdvantShop.Configuration;
using AdvantShop.Core.Controls;
using AdvantShop.Core.Services.Localization;
using AdvantShop.SEO;
using AdvantShop.Web.Infrastructure.ActionResults;
using AdvantShop.Web.Infrastructure.Extensions;
using AdvantShop.Web.Infrastructure.Filters;
using AdvantShop.Web.Infrastructure.Handlers;
using AdvantShop.Core;
using AdvantShop.Core.Common;
using Newtonsoft.Json;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Core.UrlRewriter;

namespace AdvantShop.Web.Infrastructure.Controllers
{
    [SaasStore]
    [CacheFilter()]
    [ValidateInput(false)]
    public abstract partial class BaseController : Controller
    {
        public const string NotifyMessages = "Page_NotifyMessages";
        public const string Notifications = "Page_Notifications";

        public BaseController()
        {
            if (SettingsMain.IsTechDomain(System.Web.HttpContext.Current.Request.Url))
            {
                SetNoFollowNoIndex();
            }
        }

        protected void ShowMessage(NotifyType notifyType, string message)
        {
            var str = TempData[NotifyMessages] as string;
            var prevMessages = str.IsNotEmpty() ? JsonConvert.DeserializeObject<List<Notify>>(str) : new List<Notify>();
            prevMessages.Add(new Notify(notifyType, message));
            TempData[NotifyMessages] = JsonConvert.SerializeObject(prevMessages);
        }

        protected void ShowNotification(NotifyType notifyType, string message)
        {
            var str = TempData[Notifications] as string;
            var prevMessages = str.IsNotEmpty() ? JsonConvert.DeserializeObject<List<Notify>>(str) : new List<Notify>();
            prevMessages.Add(new Notify(notifyType, message));
            TempData[Notifications] = JsonConvert.SerializeObject(prevMessages);
        }

        protected virtual MetaInfo SetMetaInformation(string pageName)
        {
            return SetMetaInformation(new MetaInfo(string.Format("{0} - {1}", pageName, SettingsMain.ShopName)), string.Empty);
        }

        protected virtual MetaInfo SetMetaInformation(MetaInfo meta, string name = null, string categoryName = null,
                                                    string brandName = null, int page = 0, List<string> tags = null,
                                                    string price = null, int totalPages = 0, string offerArtNo = null,
                                                    string productArtNo = null)
        {
            var newMeta = meta != null ? (MetaInfo) meta.Clone() : MetaInfoService.GetDefaultMetaInfo();
            var formattedMeta = MetaInfoService.GetFormatedMetaInfo(newMeta, name, categoryName, brandName, tags: tags,
                price: price, offerArtNo: offerArtNo, productArtNo: productArtNo, page: page);

            LayoutExtensions.Title = formattedMeta.Title;
            LayoutExtensions.H1 = formattedMeta.H1;
            LayoutExtensions.Description = formattedMeta.MetaDescription;
            LayoutExtensions.Keywords = formattedMeta.MetaKeywords;

            LayoutExtensions.MetaType = newMeta.Type;
            LayoutExtensions.CurrentPage = page;
            LayoutExtensions.TotalPages = totalPages;

            return newMeta;
        }

        protected void SetMobileTitle(string title)
        {
            LayoutExtensions.MobileTitleText = title;
        }

        protected void SetNoFollowNoIndex(bool nofollow = true, bool noindex = true)
        {
            LayoutExtensions.NoFollow = nofollow;
            LayoutExtensions.NoIndex = noindex;
        }

        protected FileStreamResult FileDeleteOnUpload(string fileName, string contentType)
        {
            return FileDeleteOnUpload(fileName, contentType, (string) null, null);
        }
        protected FileStreamResult FileDeleteOnUpload(string fileName, string contentType, Action actionOnDeleted)
        {
            return FileDeleteOnUpload(fileName, contentType, (string) null, actionOnDeleted);
        }

        protected FileStreamResult FileDeleteOnUpload(string fileName, string contentType, string fileDownloadName)
        {
            return FileDeleteOnUpload(fileName, contentType, fileDownloadName, null);
        }

        protected FileStreamResult FileDeleteOnUpload(string fileName, string contentType, string fileDownloadName, Action actionOnDeleted)
        {
            return File(new FileStreamDeleteOnClose(fileName, System.IO.FileMode.Open, actionOnDeleted), contentType, fileDownloadName);
        }

        protected JsonResult ProcessJsonResult<T>(ICommandHandler<T> handler)
        {
            try
            {
                return JsonOk(handler.Execute());
            }
            catch (BlException e)
            {
                ModelState.AddModelError(e.Property, e.Message);
                return JsonError();
            }
        }

        protected JsonResult ProcessJsonResult<TIn, TOut>(ICommandHandler<TIn, TOut> handler, TIn argument)
        {
            try
            {
                TOut result = handler.Execute(argument);
                return JsonOk(result);
            }
            catch (BlException e)
            {
                ModelState.AddModelError(e.Property, e.Message);
                return JsonError();
            }
        }

        protected JsonResult ProcessJsonResult(ICommandHandler handler)
        {
            try
            {
                handler.Execute();
                return JsonOk();
            }
            catch (BlException e)
            {
                ModelState.AddModelError(e.Property, e.Message);
                return JsonError();
            }
        }

        protected JsonResult ProcessJsonResult(Action action)
        {
            try
            {
                action();
                return JsonOk();
            }
            catch (BlException e)
            {
                ModelState.AddModelError(e.Property, e.Message);
                return JsonError();
            }
        }

        protected JsonResult JsonOk(object data = null, string message = null)
        {
            return Json(new CommandResult() { Result = true, Obj = data, Message = message });
        }

        protected JsonResult JsonError(string error)
        {
            this.ViewData.ModelState.AddModelError("", error);
            return JsonError();
        }

        protected JsonResult JsonError(string[] errors)
        {
            foreach (var error in errors)
                this.ViewData.ModelState.AddModelError("", error);
            return JsonError();
        }

        protected JsonResult JsonError()
        {
            var modelState = this.ViewData.ModelState;

            var errors = new List<string>();
            foreach (var state in modelState.Values)
            {
                foreach (var error in state.Errors)
                {
                    errors.Add(error.ErrorMessage);
                }
            }
            return Json(new CommandResult() { Errors = errors });
        }

        protected JsonResult JsonAccessDenied()
        {
            return JsonError(T("Infrastructure.Controllers.BaseController.AccessDenied"));
        }

        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding)
        {
            return new JsonNetResult
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
            };
        }

        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new JsonNetResult
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior
            };
        }

        protected JsonResult JsonCamelCase(object data)
        {
            return JsonCamelCase(data, null, null);
        }

        protected JsonResult JsonCamelCase(object data, string contentType, Encoding contentEncoding)
        {
            return new JsonNetResultCamelCase
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
            };
        }

        protected string T(string resourceKey)
        {
            return LocalizationService.GetResource(resourceKey);
        }

        protected string T(string resourceKey, params object[] parametres)
        {
            return LocalizationService.GetResourceFormat(resourceKey, parametres);
        }

        protected override ITempDataProvider CreateTempDataProvider()
        {
            return new CookieTempDataProvider();
        }
    }
}